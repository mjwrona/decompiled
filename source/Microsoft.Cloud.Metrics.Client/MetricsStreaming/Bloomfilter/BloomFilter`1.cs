// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsStreaming.Bloomfilter.BloomFilter`1
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.MetricsStreaming.Bloomfilter
{
  [Serializable]
  public class BloomFilter<T> : IEquatable<BloomFilter<T>>
  {
    private int keyCapacity;
    private int bitCapacity;
    private int hashFunctionCount;
    private int keyCount;
    private byte[] bitArray;
    private double falsePositive;
    private DateTime lastUpdateTime;
    [NonSerialized]
    private Func<T, byte[]> bitConverter;

    public BloomFilter(int keyCapacity, double falsePositiveRate, Func<T, byte[]> bitConverter)
    {
      try
      {
        double bitsPerKey = 1.44 * (Math.Log10(1.0 / falsePositiveRate) / Math.Log10(2.0));
        this.Init(keyCapacity, bitsPerKey, bitConverter);
      }
      catch (DivideByZeroException ex)
      {
        throw new DivideByZeroException("falsePositiveRate cannot be 0");
      }
    }

    public BloomFilter(int keyCapacity, int bitsPerKey, Func<T, byte[]> bitConverter) => this.Init(keyCapacity, (double) bitsPerKey, bitConverter);

    public BloomFilter()
    {
    }

    public int Count
    {
      get => this.keyCount;
      set => this.keyCount = value;
    }

    public Func<T, byte[]> Converter
    {
      set => this.bitConverter = value;
    }

    public int BitCapacity
    {
      get => this.bitCapacity;
      set => this.bitCapacity = value;
    }

    public int KeyCapacity
    {
      get => this.keyCapacity;
      set => this.keyCapacity = value;
    }

    public int HashFunctionsCount
    {
      get => this.hashFunctionCount;
      set => this.hashFunctionCount = value;
    }

    public byte[] Data
    {
      get => this.bitArray;
      set => this.bitArray = value;
    }

    public DateTime LastUpdateTimeUtc
    {
      get => this.lastUpdateTime;
      set => this.lastUpdateTime = value;
    }

    public double ExpectedFalsePositiveRate
    {
      get => this.falsePositive;
      set => this.falsePositive = value;
    }

    public static BloomFilter<T> Create(int keyCapacity, int bitsPerKey) => new BloomFilter<T>(keyCapacity, bitsPerKey, Converters.GetConverter<T>());

    public static BloomFilter<T> Create(int keyCapacity, double falsePositiveRate) => new BloomFilter<T>(keyCapacity, falsePositiveRate, Converters.GetConverter<T>());

    public static BloomFilter<T> Create(
      byte[] data,
      int hashFunctionsCount,
      int bitCapacity,
      Func<T, byte[]> converter)
    {
      return new BloomFilter<T>()
      {
        bitArray = data,
        bitCapacity = bitCapacity,
        bitConverter = converter,
        keyCapacity = int.MaxValue,
        hashFunctionCount = hashFunctionsCount
      };
    }

    public static BloomFilter<T> Create(
      byte[] data,
      int hashFunctionsCount,
      int bitCapacity,
      Func<T, byte[]> converter,
      DateTime lastUpdateDtTime)
    {
      return new BloomFilter<T>()
      {
        bitArray = data,
        bitCapacity = bitCapacity,
        bitConverter = converter,
        keyCapacity = int.MaxValue,
        hashFunctionCount = hashFunctionsCount,
        lastUpdateTime = lastUpdateDtTime
      };
    }

    public static BloomFilter<T> Create(
      byte[] data,
      int hashFunctionsCount,
      int bitCapacity,
      int keyCapacity,
      int currentKeyCount,
      Func<T, byte[]> converter)
    {
      return new BloomFilter<T>()
      {
        bitArray = data,
        bitCapacity = bitCapacity,
        bitConverter = converter,
        keyCapacity = keyCapacity,
        hashFunctionCount = hashFunctionsCount,
        keyCount = currentKeyCount
      };
    }

    public static int CalculateExpectedMemorySize(int keyCapacity, double falsePositiveRate)
    {
      double num = Math.Log10(1.0 / falsePositiveRate) / Math.Log10(2.0);
      return (int) ((double) keyCapacity * 1.44 * num) / 8;
    }

    public void Insert(T t)
    {
      if (this.keyCount == this.keyCapacity)
        throw new Exception(string.Format("Tried to add too many keys to this bloom filter. Filter capacity is {0}. Bump this up if you want to add more keys", (object) this.keyCapacity));
      ++this.keyCount;
      for (int seed = 0; seed < this.hashFunctionCount; ++seed)
        this.SetBit(this.HashKey(t, seed));
      this.lastUpdateTime = DateTime.UtcNow;
    }

    public bool Contains(T t)
    {
      for (int seed = 0; seed < this.hashFunctionCount; ++seed)
      {
        if (!this.GetBit(this.HashKey(t, seed)))
          return false;
      }
      return true;
    }

    public bool Equals(BloomFilter<T> other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.bitCapacity == other.bitCapacity && this.hashFunctionCount == other.hashFunctionCount && ((IEnumerable<byte>) this.bitArray).SequenceEqual<byte>((IEnumerable<byte>) other.bitArray);
    }

    public void Reset()
    {
      this.keyCount = 0;
      Array.Clear((Array) this.bitArray, 0, this.bitArray.Length);
    }

    private void Init(int keyCapacity, double bitsPerKey, Func<T, byte[]> bitConverter)
    {
      this.keyCapacity = keyCapacity;
      this.bitCapacity = (int) Math.Ceiling((double) this.keyCapacity * bitsPerKey);
      int length = (this.bitCapacity + 7) / 8;
      this.bitArray = length <= int.MaxValue ? new byte[length] : throw new ArgumentException(string.Format("The total bit capacity of the filter (key count times bits per key) must be less than or equal to {0}", (object) int.MaxValue));
      this.hashFunctionCount = (int) Math.Ceiling(0.7 * bitsPerKey);
      this.falsePositive = 1.0 / Math.Pow(2.0, bitsPerKey / 1.44);
      this.bitConverter = bitConverter;
    }

    private void SetBit(ulong bit)
    {
      ulong index = bit / 8UL;
      int num = (int) (bit % 8UL);
      if (index >= (ulong) this.bitArray.Length)
        throw new ArgumentOutOfRangeException(nameof (bit), string.Format("bit index {0} is out of range, given bit capacity of {1} and bit storage array of length {2}", (object) bit, (object) this.bitCapacity, (object) this.bitArray.Length));
      this.bitArray[index] |= (byte) (1 << num);
    }

    private bool GetBit(ulong bit)
    {
      ulong index = bit / 8UL;
      int num = (int) (bit % 8UL);
      if (index >= (ulong) this.bitArray.Length)
        throw new ArgumentOutOfRangeException(nameof (bit), string.Format("bit index {0} is out of range, given bit capacity of {1} and bit storage array of length {2}", (object) bit, (object) this.bitCapacity, (object) this.bitArray.Length));
      return ((uint) this.bitArray[index] & (uint) (byte) (1 << num)) > 0U;
    }

    private ulong HashKey(T t, int seed) => JenkinsHash.HashSafe(this.bitConverter(t), (ulong) seed) % (ulong) this.bitCapacity;
  }
}
