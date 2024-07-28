// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ConnectedServerPublicKeyExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class ConnectedServerPublicKeyExtensions
  {
    public static RSA GetRSA(this ConnectedServerPublicKey key)
    {
      RSACng rsa = new RSACng(2048);
      rsa.ImportParameters(new RSAParameters()
      {
        Exponent = key.Exponent,
        Modulus = key.Modulus
      });
      return (RSA) rsa;
    }

    public static string ToXmlString(this ConnectedServerPublicKey key)
    {
      using (RSA rsa = key.GetRSA())
        return rsa.ToXmlString(false);
    }
  }
}
