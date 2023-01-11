// -------------------------------------------------------------------------
// <copyright file="HashFunction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

using System.Collections;

namespace BloomFilter.NetCore;

/// <summary>
///     An implemented to provide custom hash functions.
/// </summary>
public abstract class HashFunction
{
    private const int IntMax = 2147483647;
        
    /// <summary>
    ///     Hashes the specified value.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="m">integer output range.</param>
    /// <param name="k">number of hashes to be computed.</param>
    /// <returns>
    ///     int array of hashes hash values
    /// </returns>
    public abstract int[] ComputeHash(byte[] data, int m, int k);

    public static uint RotateLeft(uint original, int bits)
    {
        return (original << bits) | (original >> (32 - bits));
    }

    public static uint RightMove(uint value, int pos)
    {
        if (pos != 0)
        {
            uint mask = 0x7fffffff;
            value >>= 1;
            value &= mask;
            value >>= pos - 1;
        }

        return value;
    }
}