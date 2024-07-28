// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SigningManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class SigningManager
  {
    private const int c_minimumKeyLength = 1024;

    internal static ISigner GetSigner(
      byte[] keyData,
      SigningAlgorithm algorithm,
      SigningKeyType keyType = SigningKeyType.RSAStored)
    {
      try
      {
        if (keyType == SigningKeyType.KeyEncryptionKey || keyType == SigningKeyType.MasterWrappingKey || keyType == SigningKeyType.RsaSecuredByKeyEncryptionKey)
          throw new NotSupportedException(string.Format("BCryptSigner is not supported for {0}", (object) keyType));
        return (ISigner) new BCryptSigner(keyData, algorithm, keyType);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(532126484, "Signing", "Service", ex);
        throw;
      }
    }

    internal static ISigner GetSigner(
      IVssRequestContext requestContext,
      byte[] keyData,
      SigningAlgorithm algorithm,
      SigningKeyType keyType = SigningKeyType.RSAStored)
    {
      try
      {
        if (keyType == SigningKeyType.KeyEncryptionKey)
          return (ISigner) new AesSigner(keyData, SigningKeyType.KeyEncryptionKey);
        return keyType == SigningKeyType.MasterWrappingKey ? (ISigner) new WrappingSigner(requestContext, keyData) : (ISigner) new BCryptSigner(keyData, algorithm, keyType);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(532126484, "Signing", "Service", ex);
        throw;
      }
    }

    internal static ISigner GetSigner(X509Certificate2 certificate, SigningAlgorithm algorithm)
    {
      try
      {
        return (ISigner) new CngSigner(certificate, algorithm);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(800298613, "Signing", "Service", ex);
        throw;
      }
    }

    internal static byte[] GenerateKeyPair(int keyLengthInBits) => SigningManager.IsValidRsaKeyLength(keyLengthInBits) ? BCryptSigner.GenerateKeyPair(keyLengthInBits) : throw new ArgumentOutOfRangeException(nameof (keyLengthInBits), CommonResources.ValueOutOfRange((object) keyLengthInBits, (object) nameof (keyLengthInBits), (object) 1024, (object) int.MaxValue));

    internal static byte[] GenerateKey(SigningKeyType keyType, int keyLengthInBits = 0)
    {
      switch (keyType)
      {
        case SigningKeyType.RSAStored:
        case SigningKeyType.RSASecured:
        case SigningKeyType.PartitionSecured:
        case SigningKeyType.RsaSecuredByKeyEncryptionKey:
          return SigningManager.IsValidRsaKeyLength(keyLengthInBits) ? BCryptSigner.GenerateKeyPair(keyLengthInBits) : throw new ArgumentOutOfRangeException(nameof (keyLengthInBits), CommonResources.ValueOutOfRange((object) keyLengthInBits, (object) nameof (keyLengthInBits), (object) 1024, (object) int.MaxValue));
        case SigningKeyType.KeyEncryptionKey:
          return AesSigner.GenerateKey();
        default:
          throw new NotSupportedException(FrameworkResources.KeyTypeMustBeRSASecuredOrStored((object) keyType));
      }
    }

    private static bool IsValidRsaKeyLength(int keyLength) => keyLength >= 1024 && keyLength % 512 == 0;

    internal static int GetHashSize(SigningAlgorithm algorithm)
    {
      switch (algorithm)
      {
        case SigningAlgorithm.SHA1:
          return 160;
        case SigningAlgorithm.SHA256:
          return 256;
        case SigningAlgorithm.SHA384:
          return 384;
        case SigningAlgorithm.SHA512:
          return 512;
        default:
          throw new NotSupportedException();
      }
    }

    internal static HashAlgorithmName GetHashAlgorithmName(SigningAlgorithm algorithm)
    {
      switch (algorithm)
      {
        case SigningAlgorithm.SHA1:
          return HashAlgorithmName.SHA1;
        case SigningAlgorithm.SHA256:
          return HashAlgorithmName.SHA256;
        case SigningAlgorithm.SHA384:
          return HashAlgorithmName.SHA384;
        case SigningAlgorithm.SHA512:
          return HashAlgorithmName.SHA512;
        default:
          throw new NotSupportedException();
      }
    }
  }
}
