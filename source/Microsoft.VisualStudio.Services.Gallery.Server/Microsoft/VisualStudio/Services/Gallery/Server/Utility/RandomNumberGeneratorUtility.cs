// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Utility.RandomNumberGeneratorUtility
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Utility
{
  public static class RandomNumberGeneratorUtility
  {
    [ThreadStatic]
    private static Random _local;
    private static RNGCryptoServiceProvider _global = new RNGCryptoServiceProvider();

    public static int Next()
    {
      Random random = RandomNumberGeneratorUtility._local;
      if (random == null)
      {
        byte[] data = new byte[4];
        RandomNumberGeneratorUtility._global.GetBytes(data);
        RandomNumberGeneratorUtility._local = random = new Random(BitConverter.ToInt32(data, 0));
      }
      return random.Next() % 100;
    }
  }
}
