// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.ConcurrentRandom
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal static class ConcurrentRandom
  {
    private static readonly Random seedGenerator = new Random();
    [ThreadStatic]
    private static Random threadLocalRandom;

    public static int Next(int minValue, int maxValue) => ConcurrentRandom.GetThreadLocalRandom().Next(minValue, maxValue);

    public static long NextPositiveLong()
    {
      byte[] buffer = new byte[8];
      ConcurrentRandom.GetThreadLocalRandom().NextBytes(buffer);
      return Math.Abs((long) BitConverter.ToUInt64(buffer, 0));
    }

    private static Random GetThreadLocalRandom()
    {
      if (ConcurrentRandom.threadLocalRandom == null)
      {
        int Seed;
        lock (ConcurrentRandom.seedGenerator)
          Seed = ConcurrentRandom.seedGenerator.Next();
        ConcurrentRandom.threadLocalRandom = new Random(Seed);
      }
      return ConcurrentRandom.threadLocalRandom;
    }
  }
}
