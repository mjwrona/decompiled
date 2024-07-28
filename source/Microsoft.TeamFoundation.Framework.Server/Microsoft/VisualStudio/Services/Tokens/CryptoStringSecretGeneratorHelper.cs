// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.CryptoStringSecretGeneratorHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.Tokens
{
  public static class CryptoStringSecretGeneratorHelper
  {
    internal const string c_TokenSecretsDrawerName = "DelegatedAuthorizationSecrets";
    internal const string c_TokenFrameworkAccessTokenKeySecret = "FrameworkAccessTokenKeySecret";
    private const uint c_PATMagicNumber = 3770247469;
    private const uint c_ApplicationRegistrationMagicNumber = 270137646;
    private const string c_Area = "Token";
    private const string c_Layer = "CryptoStringSecretGeneratorHelper";

    public static byte[] GetFrameworkAccessTokenSecret(
      IVssRequestContext deploymentRequestContext,
      string drawerNotFoundMessage)
    {
      ITeamFoundationStrongBoxService service = deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(deploymentRequestContext.Elevate(), "DelegatedAuthorizationSecrets", false);
      if (drawerId == Guid.Empty)
      {
        deploymentRequestContext.Trace(1430113, TraceLevel.Error, "Token", nameof (CryptoStringSecretGeneratorHelper), drawerNotFoundMessage);
        throw new StrongBoxDrawerNotFoundException(drawerNotFoundMessage);
      }
      try
      {
        string s = service.GetString(deploymentRequestContext.Elevate(), drawerId, "FrameworkAccessTokenKeySecret");
        return !string.IsNullOrEmpty(s) ? Convert.FromBase64String(s) : throw new StrongBoxItemNotFoundException("FrameworkAccessTokenKeySecret");
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        deploymentRequestContext.Trace(1430114, TraceLevel.Error, "Token", nameof (CryptoStringSecretGeneratorHelper), "Access token key secrets are not found in strong box.");
        throw;
      }
    }

    internal static (string cryptoString, string accessHash) GenerateCryptoString(
      IVssRequestContext requestContext,
      CryptoStringType cryptoStringType,
      string drawerNotFoundMessage,
      string rehashString = null)
    {
      byte[] numArray1 = new byte[28];
      using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
        randomNumberGenerator.GetBytes(numArray1);
      Crc32 crc32 = new Crc32();
      crc32.UpdateCRC32(numArray1, 0, numArray1.Length);
      uint num1 = 0;
      switch (cryptoStringType)
      {
        case CryptoStringType.PAT:
          num1 = 3770247469U;
          break;
        case CryptoStringType.ApplicationSecret:
          num1 = 270137646U;
          break;
      }
      uint num2 = crc32.Crc32Value ^ num1;
      byte[] numArray2 = new byte[32];
      Array.Copy((Array) numArray1, 0, (Array) numArray2, 0, 2);
      Array.Copy((Array) numArray1, 2, (Array) numArray2, 3, 4);
      Array.Copy((Array) numArray1, 6, (Array) numArray2, 9, 13);
      Array.Copy((Array) numArray1, 19, (Array) numArray2, 23, 9);
      numArray2[2] = (byte) (num2 >> 24);
      numArray2[7] = (byte) (num2 >> 16);
      numArray2[8] = (byte) (num2 >> 8);
      numArray2[22] = (byte) num2;
      string lowerInvariant = Base32Encoder.Encode(numArray2).TrimEnd('=').ToLowerInvariant();
      return (lowerInvariant, CryptoStringSecretGeneratorHelper.GenerateAcccessHash(requestContext, lowerInvariant, drawerNotFoundMessage, rehashString));
    }

    internal static (string cryptoString, string accessHash) GenerateCryptoString(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid deploymentHostId,
      string publicData,
      string drawerNotFoundMessage)
    {
      Guid guid = Guid.Empty;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && hostId != deploymentHostId)
        guid = hostId;
      string stringToHash = string.Format("{0}:{1}", (object) guid, (object) publicData);
      return (stringToHash, CryptoStringSecretGeneratorHelper.GenerateAcccessHash(requestContext, stringToHash, drawerNotFoundMessage));
    }

    internal static string GenerateAcccessHash(
      IVssRequestContext requestContext,
      string stringToHash,
      string drawerNotFoundMessage,
      string rehashString = null)
    {
      byte[] accessTokenSecret = CryptoStringSecretGeneratorHelper.GetFrameworkAccessTokenSecret(requestContext, drawerNotFoundMessage);
      string hashBase32Encoded;
      using (HMACSHA256Hash hmacshA256Hash = new HMACSHA256Hash(stringToHash, accessTokenSecret))
        hashBase32Encoded = hmacshA256Hash.HashBase32Encoded;
      if (!string.IsNullOrEmpty(rehashString))
      {
        using (HMACSHA256Hash hmacshA256Hash = new HMACSHA256Hash(hashBase32Encoded, Encoding.ASCII.GetBytes(rehashString)))
          hashBase32Encoded = hmacshA256Hash.HashBase32Encoded;
      }
      return hashBase32Encoded;
    }
  }
}
