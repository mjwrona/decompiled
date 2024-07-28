// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.RsaUtils
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using PCLCrypto;
using System;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class RsaUtils
  {
    private const string BeginRsaPrivateKeyHeader = "-----BEGIN RSA PRIVATE KEY-----";
    private const string EndRsaPrivateKeyHeader = "-----END RSA PRIVATE KEY-----";
    private const string BeginPrivateKeyHeader = "-----BEGIN PRIVATE KEY-----";
    private const string EndPrivateKeyHeader = "-----END PRIVATE KEY-----";
    private string currentBeginHeader;
    private string currentEndHeader;
    private CryptographicPrivateKeyBlobType cryptographicPrivateKeyBlobType;

    public RsaUtils()
    {
      this.currentBeginHeader = "-----BEGIN RSA PRIVATE KEY-----";
      this.currentEndHeader = "-----END RSA PRIVATE KEY-----";
      this.cryptographicPrivateKeyBlobType = CryptographicPrivateKeyBlobType.Pkcs1RsaPrivateKey;
    }

    public System.Security.Cryptography.RSAParameters GetRsaParameters(string rsaKeyData)
    {
      byte[] keyBlob = !string.IsNullOrWhiteSpace(rsaKeyData) ? Convert.FromBase64String(this.InitializeKeyData(rsaKeyData)) : throw new ArgumentNullException(nameof (rsaKeyData));
      PCLCrypto.RSAParameters rsaParameters = WinRTCrypto.AsymmetricKeyAlgorithmProvider.OpenAlgorithm(PCLCrypto.AsymmetricAlgorithm.RsaPkcs1).ImportKeyPair(keyBlob, this.cryptographicPrivateKeyBlobType).ExportParameters(true);
      if (!RsaUtils.IsCAPICompatible(rsaParameters))
      {
        PCLCrypto.RSAParameters parameters = RsaUtils.NegotiateSizes(rsaParameters);
        if (RsaUtils.IsCAPICompatible(parameters))
          rsaParameters = parameters;
      }
      return this.ConvertToRsaParamaters(rsaParameters);
    }

    private static byte[] PrependLeadingZero(byte[] buffer)
    {
      if (buffer == null)
        return (byte[]) null;
      byte[] dst = new byte[buffer.Length + 1];
      Buffer.BlockCopy((Array) buffer, 0, (Array) dst, 1, buffer.Length);
      return dst;
    }

    private static byte[] TrimLeadingZero(byte[] buffer)
    {
      if (buffer == null)
        return (byte[]) null;
      if (buffer.Length == 0 || buffer[0] != (byte) 0)
        return buffer;
      byte[] dst = new byte[buffer.Length - 1];
      Buffer.BlockCopy((Array) buffer, 1, (Array) dst, 0, dst.Length);
      return dst;
    }

    private static byte[] TrimOrPadZeroToLength(byte[] buffer, int desiredLength)
    {
      if (desiredLength <= 0)
        return buffer;
      if (buffer == null)
        return (byte[]) null;
      byte[] length = buffer;
      if (buffer.Length > desiredLength)
        length = RsaUtils.TrimLeadingZero(buffer);
      else if (buffer.Length < desiredLength)
        length = RsaUtils.PrependLeadingZero(buffer);
      return length;
    }

    private static PCLCrypto.RSAParameters NegotiateSizes(PCLCrypto.RSAParameters parameters)
    {
      parameters.Modulus = RsaUtils.TrimLeadingZero(parameters.Modulus);
      parameters.D = RsaUtils.TrimLeadingZero(parameters.D);
      int length1 = parameters.Modulus.Length;
      byte[] d = parameters.D;
      int length2 = d != null ? d.Length : 0;
      int desiredLength1 = Math.Max(length1, length2);
      parameters.Modulus = RsaUtils.TrimOrPadZeroToLength(parameters.Modulus, desiredLength1);
      parameters.D = RsaUtils.TrimOrPadZeroToLength(parameters.D, desiredLength1);
      int desiredLength2 = (desiredLength1 + 1) / 2;
      parameters.P = RsaUtils.TrimOrPadZeroToLength(parameters.P, desiredLength2);
      parameters.Q = RsaUtils.TrimOrPadZeroToLength(parameters.Q, desiredLength2);
      parameters.DP = RsaUtils.TrimOrPadZeroToLength(parameters.DP, desiredLength2);
      parameters.DQ = RsaUtils.TrimOrPadZeroToLength(parameters.DQ, desiredLength2);
      parameters.InverseQ = RsaUtils.TrimOrPadZeroToLength(parameters.InverseQ, desiredLength2);
      return parameters;
    }

    private static bool IsCAPICompatible(PCLCrypto.RSAParameters parameters)
    {
      if (parameters.Modulus == null || parameters.P == null)
        return true;
      int num1 = (parameters.Modulus.Length + 1) / 2;
      int num2 = num1;
      int? length1 = parameters.P?.Length;
      int valueOrDefault1 = length1.GetValueOrDefault();
      if (num2 == valueOrDefault1 & length1.HasValue)
      {
        int num3 = num1;
        int? length2 = parameters.Q?.Length;
        int valueOrDefault2 = length2.GetValueOrDefault();
        if (num3 == valueOrDefault2 & length2.HasValue)
        {
          int num4 = num1;
          int? length3 = parameters.DP?.Length;
          int valueOrDefault3 = length3.GetValueOrDefault();
          if (num4 == valueOrDefault3 & length3.HasValue)
          {
            int num5 = num1;
            int? length4 = parameters.DQ?.Length;
            int valueOrDefault4 = length4.GetValueOrDefault();
            if (num5 == valueOrDefault4 & length4.HasValue)
            {
              int num6 = num1;
              int? length5 = parameters.InverseQ?.Length;
              int valueOrDefault5 = length5.GetValueOrDefault();
              if (num6 == valueOrDefault5 & length5.HasValue)
              {
                int length6 = parameters.Modulus.Length;
                int? length7 = parameters.D?.Length;
                int valueOrDefault6 = length7.GetValueOrDefault();
                return length6 == valueOrDefault6 & length7.HasValue;
              }
            }
          }
        }
      }
      return false;
    }

    private string InitializeKeyData(string rsaKeyData)
    {
      this.DetectTypeOfHeader(rsaKeyData);
      int startIndex1 = rsaKeyData.IndexOf(this.currentBeginHeader, StringComparison.Ordinal);
      string str = startIndex1 < 0 ? rsaKeyData : rsaKeyData.Remove(startIndex1, this.currentBeginHeader.Length);
      int startIndex2 = str.IndexOf(this.currentEndHeader, StringComparison.Ordinal);
      return (startIndex2 < 0 ? str : str.Remove(startIndex2)).Replace("\r\n", string.Empty);
    }

    private void DetectTypeOfHeader(string rsaKeyData)
    {
      if (rsaKeyData.StartsWith("-----BEGIN RSA PRIVATE KEY-----") && rsaKeyData.Contains("-----END RSA PRIVATE KEY-----"))
      {
        this.currentBeginHeader = "-----BEGIN RSA PRIVATE KEY-----";
        this.currentEndHeader = "-----END RSA PRIVATE KEY-----";
        this.cryptographicPrivateKeyBlobType = CryptographicPrivateKeyBlobType.Pkcs1RsaPrivateKey;
      }
      else
      {
        if (!rsaKeyData.StartsWith("-----BEGIN PRIVATE KEY-----") || !rsaKeyData.Contains("-----END PRIVATE KEY-----"))
          throw new InvalidOperationException(Resources.InvalidCertificate());
        this.currentBeginHeader = "-----BEGIN PRIVATE KEY-----";
        this.currentEndHeader = "-----END PRIVATE KEY-----";
        this.cryptographicPrivateKeyBlobType = CryptographicPrivateKeyBlobType.Pkcs8RawPrivateKeyInfo;
      }
    }

    private System.Security.Cryptography.RSAParameters ConvertToRsaParamaters(
      PCLCrypto.RSAParameters pclRsaParameters)
    {
      return new System.Security.Cryptography.RSAParameters()
      {
        D = pclRsaParameters.D,
        DP = pclRsaParameters.DP,
        DQ = pclRsaParameters.DQ,
        Exponent = pclRsaParameters.Exponent,
        InverseQ = pclRsaParameters.InverseQ,
        Modulus = pclRsaParameters.Modulus,
        P = pclRsaParameters.P,
        Q = pclRsaParameters.Q
      };
    }
  }
}
