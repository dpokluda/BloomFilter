using System.Collections;
using Newtonsoft.Json;

namespace BloomFilter.NetCore;

public class Filter : BaseFilter
{
    private readonly object sync = new();

    private static readonly Task Empty = Task.FromResult(0);

    [JsonConstructor]
    internal Filter()
        : base()
    {}

    public Filter(int expectedElements, double errorRate)
        : base(expectedElements, errorRate)
    {
        _hashBits = new BitArray(Capacity);
    }

    public Filter(int capacity, int hashes)
        : base(capacity, hashes)
    {
        _hashBits = new BitArray(Capacity);
    }

    [JsonIgnore]
    public BitArray HashBits => _hashBits;

    [JsonProperty(PropertyName = "HashBits")]
    private readonly BitArray _hashBits;
    
    /// <summary>
    /// Adds the passed value to the filter.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public override bool Add(string element)
    {
        bool added = false;
        var positions = ComputeHash(ToBytes(element));
        lock (sync)
        {
            foreach (int position in positions)
            {
                if (!HashBits.Get(position))
                {
                    added = true;
                    HashBits.Set(position, true);
                }
            }
        }
        return added;
    }

    /// <summary>
    /// Tests whether an element is present in the filter
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public override bool Contains(string element)
    {
        var positions = ComputeHash(ToBytes(element));
        lock (sync)
        {
            foreach (int position in positions)
            {
                if (!HashBits.Get(position))
                    return false;
            }
        }
        return true;
    }

    public override void Clear()
    {
        lock (sync)
        {
            HashBits.SetAll(false);
        }
    }

    public override void Dispose()
    {
    }
}