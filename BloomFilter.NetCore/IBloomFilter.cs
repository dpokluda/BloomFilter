// -------------------------------------------------------------------------
// <copyright file="IBloomFilter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

namespace BloomFilter.NetCore;

/// <summary>
///     Represents a Bloom filter.
/// </summary>
public interface IBloomFilter : IDisposable
{
    /// <summary>
    ///     Adds the passed value to the filter.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    bool Add(string element);

    /// <summary>
    ///     Tests whether an element is present in the filter
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    bool Contains(string element);

    /// <summary>
    ///     Removes all elements from the filter
    /// </summary>
    void Clear();
}