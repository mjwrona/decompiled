// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ViewStateKeyGenerator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class ViewStateKeyGenerator
  {
    public static string GenerateDecryptionKey() => ViewStateKeyGenerator.GenerateKey(64);

    public static string GenerateValidationKey() => ViewStateKeyGenerator.GenerateKey(128);

    private static string GenerateKey(int keyLength)
    {
      using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
      {
        byte[] data = new byte[keyLength / 2];
        cryptoServiceProvider.GetBytes(data);
        return HexConverter.ToString(data);
      }
    }
  }
}
