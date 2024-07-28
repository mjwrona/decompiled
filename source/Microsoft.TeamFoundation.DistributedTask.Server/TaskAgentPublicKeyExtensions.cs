// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentPublicKeyExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskAgentPublicKeyExtensions
  {
    public static byte[] Encrypt(this TaskAgentPublicKey key, byte[] value)
    {
      using (RSA rsa = key.GetRSA())
        return rsa.Encrypt(value, RSAEncryptionPadding.OaepSHA1);
    }

    public static RSA GetRSA(this TaskAgentPublicKey key)
    {
      RSACng rsa = new RSACng(2048);
      rsa.ImportParameters(new RSAParameters()
      {
        Exponent = key.Exponent,
        Modulus = key.Modulus
      });
      return (RSA) rsa;
    }

    public static string ToXmlString(this TaskAgentPublicKey key)
    {
      using (RSA rsa = key.GetRSA())
        return rsa.ToXmlString(false);
    }
  }
}
