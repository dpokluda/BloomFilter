// -------------------------------------------------------------------------
// <copyright file="SerializationTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

using System.Diagnostics;
using BloomFilter.NetCore.Serialization;
using Newtonsoft.Json;

namespace BloomFilter.NetCore.Tests;

[TestClass]
public class SerializationTests
{
    [TestMethod]
    public void Serialize()
    {   
        var filter = new Filter(100, 0.01);
        filter.Add("one");
        filter.Add("two");

        var jsonSettings = new JsonSerializerSettings();
        jsonSettings.Converters.Add(new BitArrayConverter());
        var serialized = JsonConvert.SerializeObject(filter, jsonSettings);
        
        Trace.WriteLine(serialized);
    }
    
    [TestMethod]
    public void Deserialize()
    {   
        var filter = new Filter(100, 0.01);
        filter.Add("one");
        filter.Add("two");

        var jsonSettings = new JsonSerializerSettings();
        jsonSettings.Converters.Add(new BitArrayConverter());
        var serialized = JsonConvert.SerializeObject(filter, jsonSettings);
        var deserialized = JsonConvert.DeserializeObject<Filter>(serialized, jsonSettings);
        
        Assert.IsNotNull(deserialized);
        Trace.WriteLine(JsonConvert.SerializeObject(deserialized, jsonSettings));
        
        Assert.IsTrue(filter.Contains("one"));
        Assert.IsTrue(filter.Contains("two"));
        Assert.IsFalse(filter.Contains("three"));
    }
    
    [TestMethod]
    public void SerializeToStringBits()
    {   
        var filter = new Filter(100, 0.01);
        filter.Add("one");
        filter.Add("two");

        var jsonSettings = new JsonSerializerSettings();
        jsonSettings.Converters.Add(new BitArrayToStringConverter());
        var serialized = JsonConvert.SerializeObject(filter, jsonSettings);
        var deserialized = JsonConvert.DeserializeObject<Filter>(serialized, jsonSettings);

        Assert.IsNotNull(deserialized);
        Trace.WriteLine(JsonConvert.SerializeObject(deserialized, jsonSettings));
        
        Assert.IsTrue(filter.Contains("one"));
        Assert.IsTrue(filter.Contains("two"));
        Assert.IsFalse(filter.Contains("three"));
    }
}