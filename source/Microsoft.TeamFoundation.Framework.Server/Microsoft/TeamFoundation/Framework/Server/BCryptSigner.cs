// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BCryptSigner
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class BCryptSigner : ISigner, IDisposable
  {
    private readonly BCryptSigner.SafeBCryptAlgorithmHandle m_algHandle;
    private readonly BCryptSigner.SafeBCryptKeyHandle m_keyHandle;
    private readonly SigningAlgorithm m_algorithm;
    private readonly HashAlgorithmName m_hashAlgorithm;
    private int m_keyLengthInBits;
    private const int BCRYPT_PAD_PKCS1 = 2;
    private const int BCRYPT_PAD_OAEP = 4;
    private const int NTE_BUFFER_TOO_SMALL = -2146893784;
    private const int ALG_TYPE_RSA = 1024;
    private const int ALG_CLASS_SIGNATURE = 8192;
    private const int ALG_CLASS_KEY_EXCHANGE = 40960;
    private const int CALG_RSA_SIGN = 9216;
    private const int CALG_RSA_KEYX = 41984;
    private const byte PRIVATEKEYBLOB = 7;
    private const string LEGACY_RSAPRIVATE_BLOB = "CAPIPRIVATEBLOB";
    private const string LEGACY_RSAPUBLIC_BLOB = "CAPIPUBLICBLOB";
    private const string BCRYPT_RSAPUBLIC_BLOB = "RSAPUBLICBLOB";

    public BCryptSigner(byte[] publicKeyStruc, SigningAlgorithm algorithm, SigningKeyType keyType)
    {
      this.m_algHandle = this.OpenAlgorithmProvider("RSA");
      this.m_keyHandle = this.ImportKeyPairFromPublicKeyStruc(publicKeyStruc);
      this.m_algorithm = algorithm;
      this.m_hashAlgorithm = SigningManager.GetHashAlgorithmName(algorithm);
      this.KeyType = keyType;
    }

    protected BCryptSigner(int keyLengthInBits)
    {
      this.m_algHandle = this.OpenAlgorithmProvider("RSA");
      this.m_keyHandle = this.GenerateKeyPairHelper(keyLengthInBits);
    }

    public void Dispose()
    {
      if (this.m_keyHandle != null)
        this.m_keyHandle.Dispose();
      if (this.m_algHandle == null)
        return;
      this.m_algHandle.Dispose();
    }

    public static byte[] GenerateKeyPair(int keyLengthInBits) => BCryptSigner.GenerateKeyPairWithRetry(keyLengthInBits, (Func<byte[], BCryptSigner>) (keyData => new BCryptSigner(keyData, SigningAlgorithm.SHA1, SigningKeyType.RSASecured)));

    protected static byte[] GenerateKeyPairWithRetry(
      int keyLengthInBits,
      Func<byte[], BCryptSigner> testImportFunc)
    {
      for (int index = 0; index < 5; ++index)
      {
        using (BCryptSigner bcryptSigner1 = new BCryptSigner(keyLengthInBits))
        {
          byte[] keyPairWithRetry = bcryptSigner1.Export(true);
          using (BCryptSigner bcryptSigner2 = testImportFunc(keyPairWithRetry))
          {
            if (bcryptSigner2.VerifyValidKey())
              return keyPairWithRetry;
          }
        }
      }
      throw new CryptographicException("Unable to generate a valid key pair");
    }

    public byte[] Export(bool includePrivateKey)
    {
      int pcbResult;
      int hr1 = BCryptSigner.BCryptExportKey(this.m_keyHandle, IntPtr.Zero, includePrivateKey ? "CAPIPRIVATEBLOB" : "CAPIPUBLICBLOB", (byte[]) null, 0, out pcbResult, 0);
      if (hr1 != 0 && -2146893784 != hr1)
        throw new CryptographicException(hr1);
      byte[] pbOutput = new byte[pcbResult];
      int hr2 = BCryptSigner.BCryptExportKey(this.m_keyHandle, IntPtr.Zero, includePrivateKey ? "CAPIPRIVATEBLOB" : "CAPIPUBLICBLOB", pbOutput, pbOutput.Length, out pcbResult, 0);
      if (hr2 != 0)
        throw new CryptographicException(hr2);
      return pbOutput;
    }

    public SigningKeyType KeyType { get; }

    public SigningAlgorithm GetSigningAlgorithm() => this.m_algorithm;

    public int GetKeySize()
    {
      int pbOutput = this.m_keyLengthInBits;
      if (pbOutput == 0)
      {
        BCryptSigner.BCryptGetProperty(this.m_keyHandle, "KeyStrength", out pbOutput, 4, out int _, 0);
        this.m_keyLengthInBits = pbOutput;
      }
      return pbOutput;
    }

    public byte[] SignHash(byte[] hash)
    {
      BCryptSigner.BCRYPT_PKCS1_PADDING_INFO pPaddingInfo = new BCryptSigner.BCRYPT_PKCS1_PADDING_INFO();
      pPaddingInfo.pszAlgId = this.m_hashAlgorithm.Name;
      int pcbResult;
      int hr1 = BCryptSigner.BCryptSignHash(this.m_keyHandle, ref pPaddingInfo, hash, hash.Length, (byte[]) null, 0, out pcbResult, 2);
      if (hr1 != 0 && -2146893784 != hr1)
        throw new CryptographicException(hr1);
      byte[] pbOutput = new byte[pcbResult];
      int hr2 = BCryptSigner.BCryptSignHash(this.m_keyHandle, ref pPaddingInfo, hash, hash.Length, pbOutput, pbOutput.Length, out pcbResult, 2);
      if (hr2 != 0)
        throw new CryptographicException(hr2);
      return pbOutput;
    }

    public bool VerifyHash(byte[] hash, byte[] signature) => BCryptSigner.BCryptVerifySignature(this.m_keyHandle, ref new BCryptSigner.BCRYPT_PKCS1_PADDING_INFO()
    {
      pszAlgId = this.m_hashAlgorithm.Name
    }, hash, hash.Length, signature, signature.Length, 2) == 0;

    public byte[] Encrypt(byte[] data)
    {
      BCryptSigner.BCRYPT_OAEP_PADDING_INFO pPaddingInfo = new BCryptSigner.BCRYPT_OAEP_PADDING_INFO();
      pPaddingInfo.pszAlgId = this.m_hashAlgorithm.Name;
      int pcbResult;
      int hr1 = BCryptSigner.BCryptEncrypt(this.m_keyHandle, data, data.Length, ref pPaddingInfo, IntPtr.Zero, 0, (byte[]) null, 0, out pcbResult, 4);
      if (hr1 != 0 && -2146893784 != hr1)
        throw new CryptographicException(hr1);
      byte[] pbOutput = new byte[pcbResult];
      int hr2 = BCryptSigner.BCryptEncrypt(this.m_keyHandle, data, data.Length, ref pPaddingInfo, IntPtr.Zero, 0, pbOutput, pbOutput.Length, out pcbResult, 4);
      if (hr2 != 0)
        throw new CryptographicException(hr2);
      return pbOutput;
    }

    public virtual byte[] Decrypt(byte[] data)
    {
      BCryptSigner.BCRYPT_OAEP_PADDING_INFO pPaddingInfo = new BCryptSigner.BCRYPT_OAEP_PADDING_INFO();
      pPaddingInfo.pszAlgId = this.m_hashAlgorithm.Name;
      int pcbResult;
      int hr1 = BCryptSigner.BCryptDecrypt(this.m_keyHandle, data, data.Length, ref pPaddingInfo, IntPtr.Zero, 0, (byte[]) null, 0, out pcbResult, 4);
      if (hr1 != 0 && -2146893784 != hr1)
        throw new CryptographicException(hr1);
      byte[] array = new byte[pcbResult];
      int hr2 = BCryptSigner.BCryptDecrypt(this.m_keyHandle, data, data.Length, ref pPaddingInfo, IntPtr.Zero, 0, array, array.Length, out pcbResult, 4);
      if (hr2 != 0)
        throw new CryptographicException(hr2);
      if (array.Length < pcbResult)
        Array.Resize<byte>(ref array, pcbResult);
      return array;
    }

    public byte[] ExportPublicKey() => this.Export(false);

    protected bool VerifyValidKey()
    {
      string str = nameof (VerifyValidKey);
      byte[] bytes = Encoding.UTF8.GetBytes(str);
      try
      {
        string b = Encoding.UTF8.GetString(this.Decrypt(this.Encrypt(bytes)));
        if (!string.Equals(str, b, StringComparison.Ordinal))
          return false;
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    private BCryptSigner.SafeBCryptAlgorithmHandle OpenAlgorithmProvider(string algorithmId)
    {
      BCryptSigner.SafeBCryptAlgorithmHandle phAlgorithm = (BCryptSigner.SafeBCryptAlgorithmHandle) null;
      int hr = BCryptSigner.BCryptOpenAlgorithmProvider(out phAlgorithm, algorithmId, "Microsoft Primitive Provider", 0);
      if (hr != 0)
        throw new CryptographicException(hr);
      return phAlgorithm;
    }

    private BCryptSigner.SafeBCryptKeyHandle ImportKeyPairFromPublicKeyStruc(byte[] blob)
    {
      BCryptSigner.SafeBCryptKeyHandle phKey = (BCryptSigner.SafeBCryptKeyHandle) null;
      bool flag = blob.Length != 0 && blob[0] == (byte) 7;
      if (!flag)
        blob = BCryptSigner.LegacyRsaPublicBlobToBCryptRsaPublicBlob(blob);
      int hr = BCryptSigner.BCryptImportKeyPair(this.m_algHandle, IntPtr.Zero, flag ? "CAPIPRIVATEBLOB" : "RSAPUBLICBLOB", out phKey, blob, blob.Length, 0);
      if (hr != 0)
        throw new CryptographicException(hr);
      return phKey;
    }

    private static unsafe byte[] LegacyRsaPublicBlobToBCryptRsaPublicBlob(byte[] blob)
    {
      byte[] src1;
      byte[] src2;
      using (BinaryReader binaryReader = new BinaryReader((Stream) new MemoryStream(blob)))
      {
        if (binaryReader.ReadByte() != (byte) 6)
          throw new FormatException("bType");
        if (binaryReader.ReadByte() != (byte) 2)
          throw new FormatException("bVersion");
        if (binaryReader.ReadUInt16() != (ushort) 0)
          throw new FormatException("reserved");
        switch (binaryReader.ReadInt32())
        {
          case 9216:
          case 41984:
            int num = binaryReader.ReadInt32() == 826364754 ? binaryReader.ReadInt32() : throw new FormatException("magic");
            src1 = BCryptSigner.ExponentAsBytes(binaryReader.ReadUInt32());
            src2 = binaryReader.ReadBytes(num / 8);
            Array.Reverse((Array) src2);
            break;
          default:
            throw new FormatException("algId");
        }
      }
      blob = new byte[sizeof (BCryptSigner.BCRYPT_RSAKEY_BLOB) + src1.Length + src2.Length];
      fixed (byte* numPtr = &blob[0])
      {
        ((BCryptSigner.BCRYPT_RSAKEY_BLOB*) numPtr)->Magic = 826364754;
        ((BCryptSigner.BCRYPT_RSAKEY_BLOB*) numPtr)->BitLength = src2.Length * 8;
        ((BCryptSigner.BCRYPT_RSAKEY_BLOB*) numPtr)->cbPublicExp = src1.Length;
        ((BCryptSigner.BCRYPT_RSAKEY_BLOB*) numPtr)->cbModulus = src2.Length;
      }
      Buffer.BlockCopy((Array) src1, 0, (Array) blob, sizeof (BCryptSigner.BCRYPT_RSAKEY_BLOB), src1.Length);
      Buffer.BlockCopy((Array) src2, 0, (Array) blob, sizeof (BCryptSigner.BCRYPT_RSAKEY_BLOB) + src1.Length, src2.Length);
      return blob;
    }

    private static byte[] ExponentAsBytes(uint exponent) => exponent <= (uint) byte.MaxValue ? new byte[1]
    {
      (byte) exponent
    } : (exponent <= (uint) ushort.MaxValue ? new byte[2]
    {
      (byte) (exponent >> 8),
      (byte) exponent
    } : (exponent <= 16777215U ? new byte[3]
    {
      (byte) (exponent >> 16),
      (byte) (exponent >> 8),
      (byte) exponent
    } : new byte[4]
    {
      (byte) (exponent >> 24),
      (byte) (exponent >> 16),
      (byte) (exponent >> 8),
      (byte) exponent
    }));

    private BCryptSigner.SafeBCryptKeyHandle GenerateKeyPairHelper(int keyLengthInBits)
    {
      BCryptSigner.SafeBCryptKeyHandle phKey = (BCryptSigner.SafeBCryptKeyHandle) null;
      int keyPair = BCryptSigner.BCryptGenerateKeyPair(this.m_algHandle, out phKey, keyLengthInBits, 0);
      if (keyPair != 0)
        throw new CryptographicException(keyPair);
      int num = 2;
      int hr;
      do
      {
        hr = BCryptSigner.BCryptFinalizeKeyPair(phKey, 0);
      }
      while (hr == -1073741595 && num-- > 0);
      if (num != 2)
        TeamFoundationTracingService.TraceRaw(553401321, TraceLevel.Error, "Signing", "Service", num.ToString());
      if (hr != 0)
        throw new CryptographicException(hr);
      return phKey;
    }

    [DllImport("bcrypt", CharSet = CharSet.Unicode)]
    private static extern int BCryptOpenAlgorithmProvider(
      out BCryptSigner.SafeBCryptAlgorithmHandle phAlgorithm,
      string pszAlgId,
      string pszImplementation,
      int dwFlags);

    [DllImport("bcrypt", CharSet = CharSet.Unicode)]
    private static extern int BCryptImportKeyPair(
      BCryptSigner.SafeBCryptAlgorithmHandle hAlgorithm,
      IntPtr hImportKey,
      string pszBlobType,
      out BCryptSigner.SafeBCryptKeyHandle phKey,
      byte[] pbInput,
      int cbInput,
      int dwFlags);

    [DllImport("bcrypt")]
    private static extern int BCryptGenerateKeyPair(
      BCryptSigner.SafeBCryptAlgorithmHandle hAlgorithm,
      out BCryptSigner.SafeBCryptKeyHandle phKey,
      int dwLength,
      int dwFlags);

    [DllImport("bcrypt")]
    private static extern int BCryptFinalizeKeyPair(
      BCryptSigner.SafeBCryptKeyHandle phKey,
      int dwFlags);

    [DllImport("bcrypt", CharSet = CharSet.Unicode)]
    private static extern int BCryptExportKey(
      BCryptSigner.SafeBCryptKeyHandle hKey,
      IntPtr hExportKey,
      string pszBlobType,
      [MarshalAs(UnmanagedType.LPArray), Out] byte[] pbOutput,
      int cbOutput,
      out int pcbResult,
      int dwFlags);

    [DllImport("bcrypt", CharSet = CharSet.Unicode)]
    private static extern int BCryptGetProperty(
      BCryptSigner.SafeBCryptKeyHandle hKey,
      string pszProperty,
      out int pbOutput,
      int cbOutput,
      out int pcbResult,
      int dwFlags);

    [DllImport("bcrypt", CharSet = CharSet.Unicode)]
    private static extern int BCryptSignHash(
      BCryptSigner.SafeBCryptKeyHandle hKey,
      [In] ref BCryptSigner.BCRYPT_PKCS1_PADDING_INFO pPaddingInfo,
      [MarshalAs(UnmanagedType.LPArray), In] byte[] pbInput,
      int cbInput,
      [MarshalAs(UnmanagedType.LPArray), Out] byte[] pbOutput,
      int cbOutput,
      out int pcbResult,
      int dwFlags);

    [DllImport("bcrypt", CharSet = CharSet.Unicode)]
    private static extern int BCryptVerifySignature(
      BCryptSigner.SafeBCryptKeyHandle hKey,
      [In] ref BCryptSigner.BCRYPT_PKCS1_PADDING_INFO pPaddingInfo,
      [MarshalAs(UnmanagedType.LPArray), In] byte[] pbHash,
      int cbHash,
      [MarshalAs(UnmanagedType.LPArray), In] byte[] pbSignature,
      int cbSignature,
      int dwFlags);

    [DllImport("bcrypt", CharSet = CharSet.Unicode)]
    private static extern int BCryptEncrypt(
      BCryptSigner.SafeBCryptKeyHandle hKey,
      [MarshalAs(UnmanagedType.LPArray), In] byte[] pbInput,
      int cbInput,
      [In] ref BCryptSigner.BCRYPT_OAEP_PADDING_INFO pPaddingInfo,
      IntPtr pbIV,
      int cbIV,
      [MarshalAs(UnmanagedType.LPArray), Out] byte[] pbOutput,
      int cbOutput,
      out int pcbResult,
      int dwFlags);

    [DllImport("bcrypt", CharSet = CharSet.Unicode)]
    private static extern int BCryptDecrypt(
      BCryptSigner.SafeBCryptKeyHandle hKey,
      [MarshalAs(UnmanagedType.LPArray), In] byte[] pbInput,
      int cbInput,
      [In] ref BCryptSigner.BCRYPT_OAEP_PADDING_INFO pPaddingInfo,
      IntPtr pbIV,
      int cbIV,
      [MarshalAs(UnmanagedType.LPArray), Out] byte[] pbOutput,
      int cbOutput,
      out int pcbResult,
      int dwFlags);

    private struct BCRYPT_RSAKEY_BLOB
    {
      public int Magic;
      public int BitLength;
      public int cbPublicExp;
      public int cbModulus;
      public int cbPrime1;
      public int cbPrime2;
    }

    private struct BCRYPT_PKCS1_PADDING_INFO
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszAlgId;
    }

    private struct BCRYPT_OAEP_PADDING_INFO
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszAlgId;
      public IntPtr pbLabel;
      public int cbLabel;
    }

    private class SafeBCryptAlgorithmHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
      private SafeBCryptAlgorithmHandle()
        : base(true)
      {
      }

      protected override bool ReleaseHandle() => BCryptSigner.SafeBCryptAlgorithmHandle.BCryptCloseAlgorithmProvider(this.handle, 0) == 0;

      [DllImport("bcrypt")]
      private static extern int BCryptCloseAlgorithmProvider(IntPtr hAlgorithm, int dwFlags);
    }

    private class SafeBCryptKeyHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
      private SafeBCryptKeyHandle()
        : base(true)
      {
      }

      protected override bool ReleaseHandle() => BCryptSigner.SafeBCryptKeyHandle.BCryptDestroyKey(this.handle) == 0;

      [DllImport("bcrypt")]
      private static extern int BCryptDestroyKey(IntPtr hKey);
    }
  }
}
