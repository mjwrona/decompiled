// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CngSigner
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CngSigner : ISigner, IDisposable
  {
    private readonly SigningAlgorithm m_algorithm;
    private readonly CngKey m_cngKey;
    private readonly RSA m_rsa;
    private readonly HashAlgorithmName m_hashAlgorithm;
    private readonly RSAEncryptionPadding m_encryptionPadding;
    private const byte PRIVATEKEYBLOB = 7;
    private const string LEGACY_RSAPRIVATE_BLOB = "CAPIPRIVATEBLOB";
    private const string LEGACY_RSAPUBLIC_BLOB = "CAPIPUBLICBLOB";

    public CngSigner(X509Certificate2 certificate, SigningAlgorithm algorithm)
    {
      this.KeyType = SigningKeyType.CertificatePassthrough;
      switch (algorithm)
      {
        case SigningAlgorithm.SHA1:
        case SigningAlgorithm.SHA256:
        case SigningAlgorithm.SHA384:
        case SigningAlgorithm.SHA512:
          this.m_algorithm = algorithm;
          this.m_hashAlgorithm = SigningManager.GetHashAlgorithmName(this.m_algorithm);
          this.m_encryptionPadding = CngSigner.GetRSAEncryptionPadding(this.m_algorithm);
          this.m_cngKey = CngSigner.CngKeyFromX509Certificate2(certificate);
          this.m_rsa = (RSA) new RSACng(this.m_cngKey);
          break;
        default:
          throw new NotSupportedException();
      }
    }

    public void Dispose()
    {
      this.m_rsa.Dispose();
      this.m_cngKey.Dispose();
    }

    internal static CngKey CngKeyFromSigningServiceKey(SigningKeyType keyType, byte[] keyData)
    {
      switch (keyType)
      {
        case SigningKeyType.RSAStored:
        case SigningKeyType.RSASecured:
          return CngSigner.CngKeyFromPublicKeyStruc(keyData);
        case SigningKeyType.CertificatePassthrough:
          using (X509Certificate2 certificate = new X509Certificate2(keyData))
            return CngSigner.CngKeyFromX509Certificate2(certificate);
        default:
          throw new NotSupportedException();
      }
    }

    private static CngKey CngKeyFromPublicKeyStruc(byte[] publicKeyStruc) => publicKeyStruc.Length != 0 && publicKeyStruc[0] == (byte) 7 ? CngKey.Import(publicKeyStruc, new CngKeyBlobFormat("CAPIPRIVATEBLOB")) : CngKey.Import(publicKeyStruc, new CngKeyBlobFormat("CAPIPUBLICBLOB"));

    private static CngKey CngKeyFromX509Certificate2(X509Certificate2 certificate)
    {
      bool pfCallerFreeProvOrNCryptKey = true;
      SafeNCryptKeyHandle phCryptProvOrNCryptKey = (SafeNCryptKeyHandle) null;
      try
      {
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
          if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.CryptAcquireCertificatePrivateKey(certificate.Handle, Microsoft.TeamFoundation.Common.Internal.NativeMethods.CRYPT_ACQUIRE_SILENT_FLAG | Microsoft.TeamFoundation.Common.Internal.NativeMethods.CRYPT_ACQUIRE_ONLY_NCRYPT_KEY_FLAG, IntPtr.Zero, out phCryptProvOrNCryptKey, out uint _, out pfCallerFreeProvOrNCryptKey))
            throw new CryptographicException(string.Format("Failed to acquire private key for certificate with thumbprint {0}. Win32 Error: {1}", (object) certificate.Thumbprint, (object) Marshal.GetLastWin32Error()));
        }
        finally
        {
          if (phCryptProvOrNCryptKey != null && !pfCallerFreeProvOrNCryptKey)
          {
            bool success = false;
            phCryptProvOrNCryptKey.DangerousAddRef(ref success);
          }
        }
        return CngKey.Open(phCryptProvOrNCryptKey, CngKeyHandleOpenOptions.EphemeralKey);
      }
      finally
      {
        phCryptProvOrNCryptKey?.Dispose();
      }
    }

    public SigningKeyType KeyType { get; }

    public SigningAlgorithm GetSigningAlgorithm() => this.m_algorithm;

    private static HashAlgorithmName GetHashAlgorithmName(SigningAlgorithm algorithm)
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

    private static RSAEncryptionPadding GetRSAEncryptionPadding(SigningAlgorithm algorithm)
    {
      switch (algorithm)
      {
        case SigningAlgorithm.SHA1:
          return RSAEncryptionPadding.OaepSHA1;
        case SigningAlgorithm.SHA256:
          return RSAEncryptionPadding.OaepSHA256;
        case SigningAlgorithm.SHA384:
          return RSAEncryptionPadding.OaepSHA384;
        case SigningAlgorithm.SHA512:
          return RSAEncryptionPadding.OaepSHA512;
        default:
          throw new NotSupportedException();
      }
    }

    public byte[] SignHash(byte[] hash) => this.m_rsa.SignHash(hash, this.m_hashAlgorithm, RSASignaturePadding.Pkcs1);

    public bool VerifyHash(byte[] hash, byte[] signature) => this.m_rsa.VerifyHash(hash, signature, this.m_hashAlgorithm, RSASignaturePadding.Pkcs1);

    public byte[] Encrypt(byte[] data) => this.m_rsa.Encrypt(data, this.m_encryptionPadding);

    public byte[] Decrypt(byte[] data) => this.m_rsa.Decrypt(data, this.m_encryptionPadding);

    public byte[] ExportPublicKey() => this.m_cngKey.Export(new CngKeyBlobFormat("CAPIPUBLICBLOB"));

    public int GetKeySize() => this.m_rsa.KeySize;

    [DllImport("crypt32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CryptAcquireCertificatePrivateKey(
      IntPtr pCert,
      int dwFlags,
      IntPtr pvReserved,
      out SafeNCryptKeyHandle phCryptProvOrNCryptKey,
      out int dwKeySpec,
      [MarshalAs(UnmanagedType.Bool)] out bool pfCallerFreeProvOrNCryptKey);
  }
}
