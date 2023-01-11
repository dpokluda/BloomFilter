using System.Text;
using BloomFilter.NetCore.HashAlgorithms;
using Newtonsoft.Json;

namespace BloomFilter.NetCore;

public abstract class BaseFilter : IBloomFilter
{
    /// <summary>
    ///     <see cref="HashFunction" />
    /// </summary>
    protected HashFunction Hash { get; }

    /// <summary>
    ///     the Capacity of the Bloom filter
    /// </summary>
    [JsonIgnore]
    public int Capacity => _capacity;

    [JsonProperty(PropertyName = "Capacity")]
    private int _capacity;

    /// <summary>
    ///     number of hash functions
    /// </summary>
    [JsonIgnore]
    public int Hashes => _hashes;

    [JsonProperty(PropertyName = "Hashes")]
    private int _hashes;

    /// <summary>
    ///     the expected elements.
    /// </summary>
    [JsonIgnore]
    public int ExpectedElements => _expectedElements;

    [JsonProperty(PropertyName = "ExpectedElements")]
    private readonly int _expectedElements;

    /// <summary>
    ///     the number of expected elements
    /// </summary>
    [JsonIgnore]
    public double ErrorRate => _errorRate;

    [JsonProperty(PropertyName = "ErrorRate")]
    private readonly double _errorRate;

    internal BaseFilter()
    {}
    
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseFilter" /> class.
    /// </summary>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     expectedElements - expectedElements must be > 0
    ///     or
    ///     errorRate
    /// </exception>
    /// <exception cref="ArgumentNullException">hashFunction</exception>
    public BaseFilter(int expectedElements, double errorRate)
    {
        if (expectedElements < 1)
            throw new ArgumentOutOfRangeException("expectedElements", expectedElements, "expectedElements must be > 0");
        if (errorRate >= 1 || errorRate <= 0)
            throw new ArgumentOutOfRangeException("errorRate", errorRate, string.Format("errorRate must be between 0 and 1, exclusive. Was {0}", errorRate));

        _expectedElements = expectedElements;
        _errorRate = errorRate;
        Hash = new Murmur3KirschMitzenmacher();

        _capacity = BestM(expectedElements, errorRate);
        _hashes = BestK(expectedElements, Capacity);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseFilter" /> class.
    /// </summary>
    /// <param name="capacity">The capacity.</param>
    /// <param name="hashes">The hashes.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     capacity - capacity must be > 0
    ///     or
    ///     hashes - hashes must be > 0
    /// </exception>
    /// <exception cref="ArgumentNullException">hashFunction</exception>
    public BaseFilter(int capacity, int hashes)
    {
        if (capacity < 1)
            throw new ArgumentOutOfRangeException("capacity", capacity, "capacity must be > 0");
        if (hashes < 1)
            throw new ArgumentOutOfRangeException("hashes", hashes, "hashes must be > 0");

        _capacity = capacity;
        _hashes = hashes;
        Hash = new Murmur3KirschMitzenmacher();

        _expectedElements = BestN(hashes, capacity);
        _errorRate = BestP(hashes, capacity, ExpectedElements);
    }

    public abstract bool Add(string element);

    public abstract bool Contains(string element);

    public abstract void Clear();

    protected int[] ComputeHash(byte[] data)
    {
        return Hash.ComputeHash(data, Capacity, Hashes);
    }

    protected virtual byte[] ToBytes(string element)
    {
        return Encoding.UTF8.GetBytes(element!);
    }
    
    /// <summary>
    ///     Calculates the optimal size of the bloom filter in bits given expectedElements (expected
    ///     number of elements in bloom filter) and falsePositiveProbability (tolerable false positive rate).
    /// </summary>
    /// <param name="n">Expected number of elements inserted in the bloom filter</param>
    /// <param name="p">Tolerable false positive rate</param>
    /// <returns>the optimal siz of the bloom filter in bits</returns>
    public static int BestM(long n, double p)
    {
        return (int)Math.Ceiling(-1 * (n * Math.Log(p)) / Math.Pow(Math.Log(2), 2));
    }

    /// <summary>
    ///     Calculates the optimal hashes (number of hash function) given expectedElements (expected number of
    ///     elements in bloom filter) and size (size of bloom filter in bits).
    /// </summary>
    /// <param name="n">Expected number of elements inserted in the bloom filter</param>
    /// <param name="m">The size of the bloom filter in bits.</param>
    /// <returns>the optimal amount of hash functions hashes</returns>
    public static int BestK(long n, long m)
    {
        return (int)Math.Ceiling((Math.Log(2) * m) / n);
    }

    /// <summary>
    ///     Calculates the amount of elements a Bloom filter for which the given configuration of size and hashes is optimal.
    /// </summary>
    /// <param name="k">number of hashes</param>
    /// <param name="m">The size of the bloom filter in bits.</param>
    /// <returns>mount of elements a Bloom filter for which the given configuration of size and hashes is optimal</returns>
    public static int BestN(long k, long m)
    {
        return (int)Math.Ceiling((Math.Log(2) * m) / k);
    }

    /// <summary>
    ///     Calculates the best-case (uniform hash function) false positive probability.
    /// </summary>
    /// <param name="k"> number of hashes</param>
    /// <param name="m">The size of the bloom filter in bits.</param>
    /// <param name="insertedElements">number of elements inserted in the filter</param>
    /// <returns>The calculated false positive probability</returns>
    public static double BestP(long k, long m, double insertedElements)
    {
        return Math.Pow((1 - Math.Exp(-k * insertedElements / (double)m)), k);
    }

    public override string ToString()
    {
        return $"Capacity:{Capacity},Hashes:{Hashes},ExpectedElements:{ExpectedElements},ErrorRate:{ErrorRate}";
    }

    public abstract void Dispose();
}