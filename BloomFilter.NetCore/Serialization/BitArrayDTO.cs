// -------------------------------------------------------------------------
// <copyright file="BitArrayDTO.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

using System.Collections;
using System.IO.Compression;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace BloomFilter.NetCore.Serialization;

internal class BitArrayDTO
{
    private const int CompressLength = 257;

    public BitArrayDTO()
    { }

    public BitArrayDTO(BitArray ba)
    {
        ArgumentNullException.ThrowIfNull(ba, nameof(ba));

        L = ba.Length;
        var value = ba.BitArrayToByteArray();

        if (L >= CompressLength)
        {
            var ros = new ReadOnlySpan<byte>(value);
            int maxLength = BrotliEncoder.GetMaxCompressedLength(value.Length);
            var s = new Span<byte>(new byte[maxLength]);
            if (!BrotliEncoder.TryCompress(ros, s, out int bytesWritten, 11, 24))
                throw new Exception("Cannot compress!");
            B = s.Slice(0, bytesWritten).ToArray();
        }
        else
        {
            B = value;
        }
    }

    [JsonProperty(PropertyName = "b")]
    [JsonPropertyName("b")]
    public byte[] B { get; set; }

    [JsonProperty(PropertyName = "l")]
    [JsonPropertyName("l")]
    public int L { get; set; }

    public BitArray AsBitArray()
    {
        byte[] bytes;

        if (L >= CompressLength)
        {
            var ros = new ReadOnlySpan<byte>(B);
            var s = new Span<byte>(new byte[L.ByteArrayLength()]);

            if (!BrotliDecoder.TryDecompress(ros, s, out int bytesWritten))
                throw new Exception("Unable to decompress.");

            bytes = s.Slice(0, bytesWritten).ToArray();
        }
        else
        {
            bytes = B;
        }

        return new BitArray(bytes) { Length = L };
    }
}