// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsStreaming.StreamingBloomfilter
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.MetricsStreaming.Bloomfilter;
using System;
using System.Text;

namespace Microsoft.Cloud.Metrics.Client.MetricsStreaming
{
  public class StreamingBloomfilter : BloomFilter<string>
  {
    private int version;
    private string dimensionName;
    private StreamingBloomfilterData bloomfilterData;

    private StreamingBloomfilter()
    {
    }

    public StreamingBloomfilter(
      int version,
      string dimensionName,
      int keyCapacity,
      int bitsPerKey)
      : base(keyCapacity, bitsPerKey, (Func<string, byte[]>) (item => Encoding.UTF8.GetBytes(item)))
    {
      this.version = version > 0 ? version : throw new ArgumentException("Streaming Bloomfilter version should be greater than 0");
      this.dimensionName = dimensionName ?? throw new ArgumentNullException(nameof (dimensionName));
    }

    public StreamingBloomfilter(
      int version,
      string dimensionName,
      int keyCapacity,
      double falsePositiveRate)
      : base(keyCapacity, falsePositiveRate, (Func<string, byte[]>) (item => Encoding.UTF8.GetBytes(item)))
    {
      this.version = version > 0 ? version : throw new ArgumentException("Streaming Bloomfilter version should be greater than 0");
      this.dimensionName = dimensionName ?? throw new ArgumentNullException(nameof (dimensionName));
    }

    public StreamingBloomfilterData BloomfilterData
    {
      get
      {
        if (this.bloomfilterData != null)
          return this.bloomfilterData;
        this.bloomfilterData = new StreamingBloomfilterData()
        {
          BloomfilterDataVersion = this.version,
          HashFuncCount = this.HashFunctionsCount,
          BitCapacity = this.BitCapacity,
          EstimatedCount = this.KeyCapacity,
          DimensionName = this.dimensionName,
          KeyCount = this.Count,
          ExpectedFalsePositiveRate = this.ExpectedFalsePositiveRate,
          LastUpdateTimeUtc = this.LastUpdateTimeUtc,
          Data = this.Data
        };
        return this.bloomfilterData;
      }
    }

    public static StreamingBloomfilter Load(StreamingBloomfilterData bloomFilterData)
    {
      if (bloomFilterData == null)
        throw new ArgumentException("'bloomFilterData' cannot be null.", nameof (bloomFilterData));
      StreamingBloomfilter streamingBloomfilter = new StreamingBloomfilter();
      streamingBloomfilter.version = bloomFilterData.BloomfilterDataVersion;
      streamingBloomfilter.dimensionName = bloomFilterData.DimensionName;
      streamingBloomfilter.KeyCapacity = bloomFilterData.EstimatedCount;
      streamingBloomfilter.BitCapacity = bloomFilterData.BitCapacity;
      streamingBloomfilter.Count = bloomFilterData.KeyCount;
      streamingBloomfilter.ExpectedFalsePositiveRate = bloomFilterData.ExpectedFalsePositiveRate;
      streamingBloomfilter.HashFunctionsCount = bloomFilterData.HashFuncCount;
      streamingBloomfilter.LastUpdateTimeUtc = DateTime.UtcNow;
      streamingBloomfilter.Data = bloomFilterData.Data;
      streamingBloomfilter.Converter = (Func<string, byte[]>) (item => Encoding.UTF8.GetBytes(item));
      return streamingBloomfilter;
    }
  }
}
