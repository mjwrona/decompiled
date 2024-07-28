// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PublicKeyExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class PublicKeyExtensions
  {
    public static byte[] Encrypt(this PublicKey key, byte[] value)
    {
      using (RSA rsa = key.GetRSA())
        return rsa.Encrypt(value, RSAEncryptionPadding.OaepSHA1);
    }

    public static RSA GetRSA(this PublicKey key)
    {
      RSACng rsa = new RSACng(2048);
      rsa.ImportParameters(new RSAParameters()
      {
        Exponent = key.Exponent,
        Modulus = key.Modulus
      });
      return (RSA) rsa;
    }

    public static string ToXmlString(this PublicKey key)
    {
      using (RSA rsa = key.GetRSA())
        return rsa.ToXmlString(false);
    }
  }
}
