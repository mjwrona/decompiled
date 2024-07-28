// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Misc.MurmurHash
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime.Misc
{
  internal sealed class MurmurHash
  {
    private const int DefaultSeed = 0;

    public static int Initialize() => MurmurHash.Initialize(0);

    public static int Initialize(int seed) => seed;

    public static int Update(int hash, int value)
    {
      int num1 = -862048943;
      int num2 = 461845907;
      int num3 = 15;
      int num4 = 13;
      int num5 = 5;
      int num6 = -430675100;
      int num7 = value * num1;
      int num8 = (num7 << num3 | num7 >>> 32 - num3) * num2;
      hash ^= num8;
      hash = hash << num4 | hash >>> 32 - num4;
      hash = hash * num5 + num6;
      return hash;
    }

    public static int Update(int hash, object value) => MurmurHash.Update(hash, value != null ? value.GetHashCode() : 0);

    public static int Finish(int hash, int numberOfWords)
    {
      hash ^= numberOfWords * 4;
      hash ^= hash >>> 16;
      hash *= -2048144789;
      hash ^= hash >>> 13;
      hash *= -1028477387;
      hash ^= hash >>> 16;
      return hash;
    }

    public static int HashCode<T>(T[] data, int seed)
    {
      int hash = MurmurHash.Initialize(seed);
      foreach (T obj in data)
        hash = MurmurHash.Update(hash, (object) obj);
      return MurmurHash.Finish(hash, data.Length);
    }

    private MurmurHash()
    {
    }
  }
}
