// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.RsaSignatureValidatorRolloverCookieTransform
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  internal class RsaSignatureValidatorRolloverCookieTransform : CookieTransform
  {
    private readonly RSA m_signingKey;
    private readonly ReadOnlyCollection<RSA> m_verificationKeys;
    private readonly bool m_issue;
    private readonly bool m_required;
    private static readonly byte[] s_magic = new byte[8]
    {
      (byte) 209,
      (byte) 135,
      (byte) 66,
      (byte) 89,
      (byte) 150,
      (byte) 78,
      (byte) 152,
      (byte) 142
    };
    private static VssPerformanceCounter s_legacyCookiesPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_LegacyFedAuthCookiesPerSec");
    private static VssPerformanceCounter s_newCookiesPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_NewFedAuthCookiesPerSec");

    public RsaSignatureValidatorRolloverCookieTransform(
      X509Certificate2 certificate,
      X509Certificate2 secondaryVerificationKey,
      bool issueSignature = false,
      bool signatureRequired = false)
      : this(RsaSignatureValidatorRolloverCookieTransform.CertificatesToRSAAlgorithms(certificate, secondaryVerificationKey), issueSignature, signatureRequired)
    {
    }

    internal RsaSignatureValidatorRolloverCookieTransform(
      IEnumerable<RSA> keys,
      bool issueSignature = false,
      bool signatureRequired = false)
    {
      this.m_signingKey = keys.First<RSA>();
      this.m_verificationKeys = new ReadOnlyCollection<RSA>((IList<RSA>) keys.ToList<RSA>());
      this.m_issue = !this.m_required || this.m_issue ? issueSignature : throw new ArgumentException("RsaSignatureValidatorRolloverCookieTransform: Cannot require signature but not issue it");
      this.m_required = signatureRequired;
    }

    private static IEnumerable<RSA> CertificatesToRSAAlgorithms(
      X509Certificate2 certificate,
      X509Certificate2 secondaryVerificationKey)
    {
      RSA rsaAlgorithm = (RSA) null;
      if (certificate.HasPrivateKey)
        rsaAlgorithm = certificate.GetRSAPrivateKey();
      if (rsaAlgorithm == null)
        throw new ArgumentException("RsaSignatureValidatorRolloverCookieTransform: Could not obtain private key of signing key");
      yield return rsaAlgorithm;
      if (secondaryVerificationKey != null && !string.Equals(certificate.Thumbprint, secondaryVerificationKey.Thumbprint, StringComparison.OrdinalIgnoreCase) && secondaryVerificationKey.HasPrivateKey)
        yield return secondaryVerificationKey.GetRSAPrivateKey();
    }

    public override byte[] Encode(byte[] value)
    {
      if (!this.m_issue)
        return value;
      byte[] sourceArray = this.InnerEncode(value);
      byte[] destinationArray = new byte[RsaSignatureValidatorRolloverCookieTransform.s_magic.Length + sourceArray.Length];
      Array.Copy((Array) RsaSignatureValidatorRolloverCookieTransform.s_magic, 0, (Array) destinationArray, 0, RsaSignatureValidatorRolloverCookieTransform.s_magic.Length);
      Array.Copy((Array) sourceArray, 0, (Array) destinationArray, RsaSignatureValidatorRolloverCookieTransform.s_magic.Length, sourceArray.Length);
      return destinationArray;
    }

    public override byte[] Decode(byte[] encoded)
    {
      ArgumentUtility.CheckForNull<byte[]>(encoded, nameof (encoded));
      bool flag = false;
      if (encoded.Length >= RsaSignatureValidatorRolloverCookieTransform.s_magic.Length && ArrayUtility.Equals(RsaSignatureValidatorRolloverCookieTransform.s_magic, encoded, RsaSignatureValidatorRolloverCookieTransform.s_magic.Length))
        flag = true;
      if (flag)
      {
        RsaSignatureValidatorRolloverCookieTransform.s_newCookiesPerSec.Increment();
        byte[] numArray = new byte[encoded.Length - RsaSignatureValidatorRolloverCookieTransform.s_magic.Length];
        Array.Copy((Array) encoded, RsaSignatureValidatorRolloverCookieTransform.s_magic.Length, (Array) numArray, 0, numArray.Length);
        return this.InnerDecode(numArray);
      }
      RsaSignatureValidatorRolloverCookieTransform.s_legacyCookiesPerSec.Increment();
      if (this.m_required)
        throw new CryptographicException(FrameworkResources.RequiredSignatureMissing());
      return encoded;
    }

    private byte[] InnerDecode(byte[] encoded)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) encoded, nameof (encoded));
      int startIndex = 0;
      int length = encoded.Length >= 4 ? BitConverter.ToInt32(encoded, startIndex) : throw new FormatException();
      if (length < 0)
        throw new FormatException();
      if (length >= encoded.Length - 4)
        throw new FormatException();
      int sourceIndex1 = startIndex + 4;
      byte[] numArray1 = new byte[length];
      Array.Copy((Array) encoded, sourceIndex1, (Array) numArray1, 0, numArray1.Length);
      int sourceIndex2 = sourceIndex1 + numArray1.Length;
      byte[] numArray2 = new byte[encoded.Length - sourceIndex2];
      Array.Copy((Array) encoded, sourceIndex2, (Array) numArray2, 0, numArray2.Length);
      bool flag = false;
      using (SHA256Cng hash = new SHA256Cng())
      {
        hash.ComputeHash(numArray2);
        foreach (AsymmetricAlgorithm verificationKey in this.m_verificationKeys)
        {
          if (new RSAPKCS1SignatureDeformatter(verificationKey).VerifySignature((HashAlgorithm) hash, numArray1))
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
        throw new CryptographicException("Invalid signature");
      return numArray2;
    }

    private byte[] InnerEncode(byte[] value)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) value, nameof (value));
      byte[] signature;
      using (SHA256Cng hash = new SHA256Cng())
      {
        hash.ComputeHash(value);
        signature = new RSAPKCS1SignatureFormatter((AsymmetricAlgorithm) this.m_signingKey).CreateSignature((HashAlgorithm) hash);
      }
      byte[] bytes = BitConverter.GetBytes(signature.Length);
      int destinationIndex1 = 0;
      byte[] destinationArray = new byte[bytes.Length + signature.Length + value.Length];
      Array.Copy((Array) bytes, 0, (Array) destinationArray, destinationIndex1, bytes.Length);
      int destinationIndex2 = destinationIndex1 + bytes.Length;
      Array.Copy((Array) signature, 0, (Array) destinationArray, destinationIndex2, signature.Length);
      int destinationIndex3 = destinationIndex2 + signature.Length;
      Array.Copy((Array) value, 0, (Array) destinationArray, destinationIndex3, value.Length);
      return destinationArray;
    }
  }
}
