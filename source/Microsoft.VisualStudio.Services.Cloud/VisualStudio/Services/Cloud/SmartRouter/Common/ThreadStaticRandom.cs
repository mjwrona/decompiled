// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common.ThreadStaticRandom
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common
{
  public static class ThreadStaticRandom
  {
    private static readonly Random s_globalRandom = new Random();
    [ThreadStatic]
    private static Random? s_threadLocalRandom = (Random) null;

    public static Random Get()
    {
      Random random = ThreadStaticRandom.s_threadLocalRandom;
      if (random == null)
      {
        int Seed;
        lock (ThreadStaticRandom.s_globalRandom)
          Seed = ThreadStaticRandom.s_globalRandom.Next();
        ThreadStaticRandom.s_threadLocalRandom = random = new Random(Seed);
      }
      return random;
    }
  }
}
