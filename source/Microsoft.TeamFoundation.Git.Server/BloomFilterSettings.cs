// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.BloomFilterSettings
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class BloomFilterSettings
  {
    public static BloomFilterSettings Default = new BloomFilterSettings();
    private const BloomFilterHashVersion c_defaultGetHashesVersion = BloomFilterHashVersion.Murmur3;

    private BloomFilterSettings()
    {
      this.NumBitsOnInMask = (ushort) 7;
      this.NumBitsPerEntry = (ushort) 10;
      this.HashFunctionVersion = BloomFilterHashVersion.Murmur3;
      this.HashFunction = BloomFilterSettings.GetDefaultHashFunction(BloomFilterHashVersion.Murmur3);
    }

    private BloomFilterSettings(
      ushort bitsOn,
      ushort bitsPerEntry,
      BloomFilterHashVersion hashFunctionVersion = BloomFilterHashVersion.Murmur3)
    {
      this.NumBitsOnInMask = bitsOn;
      this.NumBitsPerEntry = bitsPerEntry;
      this.HashFunctionVersion = hashFunctionVersion;
      this.HashFunction = BloomFilterSettings.GetDefaultHashFunction(this.HashFunctionVersion);
    }

    private BloomFilterSettings(
      ushort bitsOn,
      ushort bitsPerEntry,
      IBloomFilterHashFunction getHashes)
    {
      this.NumBitsOnInMask = bitsOn;
      this.NumBitsPerEntry = bitsPerEntry;
      this.HashFunctionVersion = BloomFilterHashVersion.TestVersion;
      this.HashFunction = getHashes;
    }

    public ushort NumBitsOnInMask { get; }

    public ushort NumBitsPerEntry { get; }

    public BloomFilterHashVersion HashFunctionVersion { get; }

    public double FalsePositiveRate => Math.Pow(1.0 - Math.Exp((double) -this.NumBitsOnInMask / (double) this.NumBitsPerEntry), (double) this.NumBitsOnInMask);

    public IBloomFilterHashFunction HashFunction { get; }

    public static BloomFilterSettings CreateCustomBloomFilterSettings(
      ushort bitsOn,
      ushort bitsPerEntry,
      BloomFilterHashVersion hashFunctionVersion)
    {
      return new BloomFilterSettings(bitsOn, bitsPerEntry, hashFunctionVersion);
    }

    public static BloomFilterSettings TEST_GetCustomBloomFilterSettings(
      ushort bitsOn,
      ushort bitsPerEntry,
      IBloomFilterHashFunction hashFunction)
    {
      return new BloomFilterSettings(bitsOn, bitsPerEntry, hashFunction);
    }

    private static IBloomFilterHashFunction GetDefaultHashFunction(BloomFilterHashVersion version)
    {
      if (version == BloomFilterHashVersion.Murmur3)
        return (IBloomFilterHashFunction) Murmur3HashFunction.Instance;
      if (version == BloomFilterHashVersion.TestVersion)
        return (IBloomFilterHashFunction) null;
      throw new InvalidBloomFilterSettingException(string.Format("Unknown hash function version: {0}", (object) version));
    }
  }
}
