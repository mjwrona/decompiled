// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.X509CertGenerator
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Windows.CertEntroll;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class X509CertGenerator
  {
    public const string MicrosoftEnhancedCryptographicProvider = "Microsoft Enhanced Cryptographic Provider v1.0";
    public const string MicrosoftEnhancedRsaAndAesCryptographicProvider = "Microsoft Enhanced RSA and AES Cryptographic Provider";
    public const string MicrosoftSoftwareKeyStorageProvider = "Microsoft Software Key Storage Provider";
    private readonly ITFLogger m_logger;

    public X509CertGenerator(ITFLogger logger) => this.m_logger = logger ?? (ITFLogger) new NullLogger();

    public X509Certificate2 GenerateCngSslCertificate(
      string[] dnsName,
      DateTime notBefore,
      DateTime notAfter,
      string certificateStoreName)
    {
      return this.GenerateSslCertificate(dnsName, notBefore, notAfter, certificateStoreName, "Microsoft Software Key Storage Provider");
    }

    public X509Certificate2 GenerateSslCertificate(
      string[] dnsName,
      DateTime notBefore,
      DateTime notAfter,
      string certificateStoreName,
      string providerName = "Microsoft Enhanced Cryptographic Provider v1.0")
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) dnsName, nameof (dnsName));
      ArgumentUtility.CheckStringForNullOrEmpty(certificateStoreName, nameof (certificateStoreName));
      this.m_logger.Info("GenerateSslCertificate started. dnsName: {0}, notBefore: {1}, notAfter: {2}, certificateStoreName: {3}", (object) string.Join(",", dnsName), (object) notBefore, (object) notAfter, (object) certificateStoreName);
      string subject = "CN=" + dnsName[0] + ", OU=Created by Azure DevOps Server";
      string str = subject;
      this.m_logger.Info("subject: " + subject);
      this.m_logger.Info("issuer: " + str);
      string[] subjectAlternativeName = dnsName;
      string friendlyName = string.Format("TFS Self-Signed SSL ({0})", (object) string.Join(",", dnsName));
      this.m_logger.Info("friendlyName: " + friendlyName);
      System.Security.Cryptography.X509Certificates.X509KeyUsageFlags keyUsage = System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.KeyCertSign | System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.DataEncipherment | System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.KeyEncipherment | System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.DigitalSignature;
      Oid[] enhancedKeyUsage = new Oid[2]
      {
        new Oid("1.3.6.1.5.5.7.3.1"),
        new Oid("1.3.6.1.5.5.7.3.2")
      };
      X509Certificate2 cert = new X509CertGenerator.X509CertGeneratorImpl(this.m_logger)
      {
        NotAfter = notAfter,
        NotBefore = notBefore,
        ProviderName = providerName,
        IsCA = true,
        PathLength = 0,
        MachineContext = true
      }.CreateCert(subject, keyUsage, enhancedKeyUsage, subjectAlternativeName, friendlyName, str, str);
      string[] strArray = new string[2]
      {
        certificateStoreName,
        "Root"
      };
      foreach (string storeName in strArray)
      {
        using (X509Store x509Store = new X509Store(storeName, StoreLocation.LocalMachine))
        {
          this.m_logger.Info("Adding certificate to the " + storeName + " store.");
          x509Store.Open(OpenFlags.ReadWrite);
          x509Store.Add(cert);
        }
      }
      this.m_logger.Info("GenerateSslCertificate completed.");
      return cert;
    }

    public X509Certificate2 GenerateOAuthSigningCertificate()
    {
      DateTime utcNow = DateTime.UtcNow;
      DateTime notBefore = utcNow.AddDays(-1.0);
      utcNow = DateTime.UtcNow;
      DateTime notAfter = utcNow.AddYears(30);
      return this.GenerateOAuthSigningCertificate(notBefore, notAfter);
    }

    public X509Certificate2 GenerateOAuthSigningCertificate(DateTime notBefore, DateTime notAfter)
    {
      this.m_logger.Info("GenerateOAuthSigningCertificate started. notBefore: {0}, notAfter: {1}", (object) notBefore, (object) notAfter);
      string friendlyName = string.Format("TFS Self-Signed OAuth Signing Certificate({0})", (object) Environment.MachineName);
      this.m_logger.Info("friendlyName: " + friendlyName);
      string str = "CN=" + friendlyName;
      this.m_logger.Info("subject: " + str);
      string issuer = str;
      Oid[] enhancedKeyUsage = new Oid[1]
      {
        new Oid("1.3.6.1.5.5.7.3.1")
      };
      return new X509CertGenerator.X509CertGeneratorImpl(this.m_logger)
      {
        NotAfter = notAfter,
        NotBefore = notBefore,
        ProviderName = "Microsoft Enhanced RSA and AES Cryptographic Provider"
      }.CreateCert(str, System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.None, enhancedKeyUsage, (string[]) null, friendlyName, issuer, str);
    }

    private enum CertificateKeySpec : byte
    {
      None,
      Exchange,
      Signature,
    }

    private enum CertificateSignatureHashAlgorithm : byte
    {
      SHA1,
      SHA256,
      SHA384,
      SHA512,
    }

    private class X509CertGeneratorImpl
    {
      private readonly ITFLogger m_logger;

      public X509CertGeneratorImpl(ITFLogger logger) => this.m_logger = logger ?? (ITFLogger) new NullLogger();

      public DateTime NotBefore { get; set; } = DateTime.UtcNow.AddDays(-1.0);

      public DateTime NotAfter { get; set; } = DateTime.UtcNow.AddYears(1);

      public string ProviderName { get; set; } = "Microsoft Enhanced Cryptographic Provider v1.0";

      public string AlgorithmName { get; set; } = "RSA";

      public int KeyLength { get; set; } = 2048;

      public X509CertGenerator.CertificateKeySpec KeySpec { get; set; } = X509CertGenerator.CertificateKeySpec.Exchange;

      public X509CertGenerator.CertificateSignatureHashAlgorithm SignatureAlgorithm { get; set; } = X509CertGenerator.CertificateSignatureHashAlgorithm.SHA256;

      public bool MachineContext { get; set; }

      public bool IsCA { get; set; }

      public int PathLength { get; set; } = -1;

      public X509Certificate2 CreateCert(
        string subject,
        System.Security.Cryptography.X509Certificates.X509KeyUsageFlags keyUsage,
        Oid[] enhancedKeyUsage,
        string[] subjectAlternativeName,
        string friendlyName,
        string issuer,
        string issuedTo)
      {
        this.m_logger.Info("CreateCert is called.");
        // ISSUE: variable of a compiler-generated type
        CX509PrivateKey privateKey = this.CreatePrivateKey();
        // ISSUE: variable of a compiler-generated type
        IX509CertificateRequestCertificate instance1 = (IX509CertificateRequestCertificate) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2043-217D-11DA-B2A4-000E7BBB2B09")));
        this.m_logger.Info("Calling InitializeFromPrivateKey");
        // ISSUE: reference to a compiler-generated method
        instance1.InitializeFromPrivateKey(this.MachineContext ? X509CertificateEnrollmentContext.ContextMachine : X509CertificateEnrollmentContext.ContextUser, privateKey, "");
        instance1.NotBefore = this.NotBefore;
        instance1.NotAfter = this.NotAfter;
        this.m_logger.Info("Creating subjectDN");
        // ISSUE: variable of a compiler-generated type
        CX500DistinguishedName instance2 = (CX500DistinguishedName) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2003-217D-11DA-B2A4-000E7BBB2B09")));
        // ISSUE: reference to a compiler-generated method
        instance2.Encode(subject);
        instance1.Subject = instance2;
        this.m_logger.Info("Creating issuerDN");
        // ISSUE: variable of a compiler-generated type
        CX500DistinguishedName instance3 = (CX500DistinguishedName) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2003-217D-11DA-B2A4-000E7BBB2B09")));
        // ISSUE: reference to a compiler-generated method
        instance3.Encode(issuer);
        instance1.Issuer = instance3;
        this.m_logger.Info("Creating basicConstraintsExtension");
        // ISSUE: variable of a compiler-generated type
        CX509ExtensionBasicConstraints instance4 = (CX509ExtensionBasicConstraints) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2016-217D-11DA-B2A4-000E7BBB2B09")));
        instance4.Critical = true;
        // ISSUE: variable of a compiler-generated type
        CX509ExtensionBasicConstraints pVal = instance4;
        // ISSUE: reference to a compiler-generated method
        pVal.InitializeEncode(this.IsCA, this.IsCA ? this.PathLength : -1);
        // ISSUE: reference to a compiler-generated method
        instance1.X509Extensions.Add((CX509Extension) pVal);
        this.m_logger.Info("Creating Authority Key Identifier (AKI) extension");
        // ISSUE: variable of a compiler-generated type
        CX509ExtensionAuthorityKeyIdentifier authorityKeyIdentifier = this.CreateAuthorityKeyIdentifier((IX509PrivateKey) privateKey);
        // ISSUE: reference to a compiler-generated method
        instance1.X509Extensions.Add((CX509Extension) authorityKeyIdentifier);
        // ISSUE: variable of a compiler-generated type
        CX509ExtensionAlternativeNames sanExtension = this.CreateSanExtension(subjectAlternativeName);
        if (sanExtension != null)
        {
          // ISSUE: reference to a compiler-generated method
          instance1.X509Extensions.Add((CX509Extension) sanExtension);
        }
        this.m_logger.Info(string.Format("Creating X509ExtensionKeyUsage. keyUsage: {0}", (object) keyUsage));
        // ISSUE: variable of a compiler-generated type
        IX509ExtensionKeyUsage instance5 = (IX509ExtensionKeyUsage) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E200F-217D-11DA-B2A4-000E7BBB2B09")));
        // ISSUE: reference to a compiler-generated method
        instance5.InitializeEncode((Windows.CertEntroll.X509KeyUsageFlags) keyUsage);
        instance5.Critical = true;
        // ISSUE: reference to a compiler-generated method
        instance1.X509Extensions.Add((CX509Extension) instance5);
        this.m_logger.Info("Key Usage extension created.");
        // ISSUE: variable of a compiler-generated type
        IX509ExtensionEnhancedKeyUsage keyUsageExtension = this.CreateEnhancedKeyUsageExtension(enhancedKeyUsage);
        if (keyUsageExtension != null)
        {
          // ISSUE: reference to a compiler-generated method
          instance1.X509Extensions.Add((CX509Extension) keyUsageExtension);
        }
        // ISSUE: variable of a compiler-generated type
        CObjectId instance6 = (CObjectId) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2000-217D-11DA-B2A4-000E7BBB2B09")));
        // ISSUE: reference to a compiler-generated method
        instance6.InitializeFromValue(new Oid(this.SignatureAlgorithm.ToString()).Value);
        instance1.SignatureInformation.HashAlgorithm = instance6;
        // ISSUE: reference to a compiler-generated method
        instance1.Encode();
        this.m_logger.Info("Creating X509Enrollment");
        // ISSUE: variable of a compiler-generated type
        CX509Enrollment instance7 = (CX509Enrollment) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2046-217D-11DA-B2A4-000E7BBB2B09")));
        // ISSUE: reference to a compiler-generated method
        instance7.InitializeFromRequest((IX509CertificateRequest) instance1);
        instance7.CertificateFriendlyName = friendlyName;
        this.m_logger.Info("Calling enrollmentRequest.CreateRequest");
        // ISSUE: reference to a compiler-generated method
        string request = instance7.CreateRequest();
        // ISSUE: reference to a compiler-generated method
        instance7.InstallResponse(InstallResponseRestrictionFlags.AllowUntrustedCertificate, request, Windows.CertEntroll.EncodingType.XCN_CRYPT_STRING_BASE64, "");
        this.m_logger.Info("enrollmentRequest.CreateRequest complete.");
        string thumbprint = new X509Certificate2(Convert.FromBase64String(request)).Thumbprint;
        this.m_logger.Info("Thumbprint: " + thumbprint);
        X509Store x509Store = new X509Store(this.MachineContext ? StoreLocation.LocalMachine : StoreLocation.CurrentUser);
        try
        {
          this.m_logger.Info("Opening " + x509Store.Name + " store.");
          x509Store.Open(OpenFlags.ReadWrite);
          X509Certificate2 certificate = x509Store.Certificates.Find(X509FindType.FindByThumbprint, (object) thumbprint, false).Cast<X509Certificate2>().FirstOrDefault<X509Certificate2>();
          if (certificate == null)
          {
            MakeCertException makeCertException = new MakeCertException("Could not found certificate with thumbprint " + thumbprint);
          }
          this.m_logger.Info("Removing certificate from the store.");
          x509Store.Remove(certificate);
          return certificate;
        }
        finally
        {
          x509Store.Close();
        }
      }

      private CX509ExtensionAuthorityKeyIdentifier CreateAuthorityKeyIdentifier(
        IX509PrivateKey privateKey)
      {
        this.m_logger.Info("CreateAuthorityKeyIdentifier is called.");
        this.m_logger.Info("Retrieving  public key data...");
        // ISSUE: reference to a compiler-generated method
        // ISSUE: reference to a compiler-generated method
        byte[] buffer = Convert.FromBase64String(privateKey.ExportPublicKey().get_EncodedKey());
        // ISSUE: variable of a compiler-generated type
        CX509ExtensionAuthorityKeyIdentifier instance = (CX509ExtensionAuthorityKeyIdentifier) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2018-217D-11DA-B2A4-000E7BBB2B09")));
        this.m_logger.Info("Computing sha1 hash of the public key...");
        using (SHA1 shA1 = SHA1.Create())
        {
          string strKeyIdentifier = HexConverter.ToString(shA1.ComputeHash(buffer));
          this.m_logger.Info("sha1: " + strKeyIdentifier);
          // ISSUE: reference to a compiler-generated method
          instance.InitializeEncode(Windows.CertEntroll.EncodingType.XCN_CRYPT_STRING_HEX, strKeyIdentifier);
        }
        this.m_logger.Info("CreateAuthorityKeyIdentifier completed.");
        return instance;
      }

      private CX509ExtensionAlternativeNames CreateSanExtension(string[] subjectAlternativeName)
      {
        this.m_logger.Info("Creating San Extension");
        if (subjectAlternativeName == null || subjectAlternativeName.Length == 0)
        {
          this.m_logger.Info("subjectAlternativeName is null or empty.");
          return (CX509ExtensionAlternativeNames) null;
        }
        // ISSUE: variable of a compiler-generated type
        CAlternativeNames instance1 = (CAlternativeNames) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2014-217D-11DA-B2A4-000E7BBB2B09")));
        foreach (string str in subjectAlternativeName)
        {
          this.m_logger.Info("subjAltName: " + str);
          // ISSUE: variable of a compiler-generated type
          CAlternativeName instance2 = (CAlternativeName) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2013-217D-11DA-B2A4-000E7BBB2B09")));
          if (((IEnumerable<string>) subjectAlternativeName).Contains<string>("@"))
          {
            // ISSUE: reference to a compiler-generated method
            instance2.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_RFC822_NAME, str);
          }
          else
          {
            IPAddress address;
            if (IPAddress.TryParse(str, out address))
            {
              byte[] addressBytes = address.GetAddressBytes();
              // ISSUE: reference to a compiler-generated method
              instance2.InitializeFromRawData(AlternativeNameType.XCN_CERT_ALT_NAME_IP_ADDRESS, Windows.CertEntroll.EncodingType.XCN_CRYPT_STRING_BASE64, Convert.ToBase64String(addressBytes));
            }
            else
            {
              Guid result;
              if (Guid.TryParse(str, out result))
              {
                byte[] byteArray = result.ToByteArray();
                // ISSUE: reference to a compiler-generated method
                instance2.InitializeFromRawData(AlternativeNameType.XCN_CERT_ALT_NAME_GUID, Windows.CertEntroll.EncodingType.XCN_CRYPT_STRING_BASE64, Convert.ToBase64String(byteArray));
              }
              else
              {
                try
                {
                  byte[] rawData = new X500DistinguishedName(str).RawData;
                  // ISSUE: reference to a compiler-generated method
                  instance2.InitializeFromRawData(AlternativeNameType.XCN_CERT_ALT_NAME_DIRECTORY_NAME, Windows.CertEntroll.EncodingType.XCN_CRYPT_STRING_BASE64, Convert.ToBase64String(rawData));
                }
                catch
                {
                  // ISSUE: reference to a compiler-generated method
                  instance2.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_DNS_NAME, str);
                }
              }
            }
          }
          this.m_logger.Info(string.Format("altName.Type: {0}", (object) instance2.Type));
          // ISSUE: reference to a compiler-generated method
          instance1.Add(instance2);
        }
        // ISSUE: variable of a compiler-generated type
        CX509ExtensionAlternativeNames instance3 = (CX509ExtensionAlternativeNames) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2015-217D-11DA-B2A4-000E7BBB2B09")));
        // ISSUE: reference to a compiler-generated method
        instance3.InitializeEncode(instance1);
        this.m_logger.Info("CX509ExtensionAlternativeNames created successfully.");
        return instance3;
      }

      private CX509PrivateKey CreatePrivateKey()
      {
        this.m_logger.Info("CreatePrivateKey is called.");
        // ISSUE: variable of a compiler-generated type
        CX509PrivateKey instance1 = (CX509PrivateKey) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E200C-217D-11DA-B2A4-000E7BBB2B09")));
        instance1.ProviderName = this.ProviderName;
        // ISSUE: variable of a compiler-generated type
        CObjectId instance2 = (CObjectId) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2000-217D-11DA-B2A4-000E7BBB2B09")));
        // ISSUE: reference to a compiler-generated method
        instance2.InitializeFromValue(new Oid(this.AlgorithmName).Value);
        instance1.Algorithm = instance2;
        instance1.Length = this.KeyLength;
        instance1.KeySpec = (X509KeySpec) this.KeySpec;
        instance1.MachineContext = this.MachineContext;
        instance1.ExportPolicy = X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_EXPORT_FLAG;
        // ISSUE: reference to a compiler-generated method
        instance1.Create();
        this.m_logger.Info(string.Format("Private key created. Algorithm: {0}, Key Length: {1}, KeySpec: {2}", (object) instance1.Algorithm.FriendlyName, (object) instance1.Length, (object) instance1.KeySpec));
        this.m_logger.Info("CreatePrivateKey complete.");
        return instance1;
      }

      private IX509ExtensionEnhancedKeyUsage CreateEnhancedKeyUsageExtension(Oid[] enhancedKeyUsage)
      {
        this.m_logger.Info("Creating EKU extension");
        if (enhancedKeyUsage == null || enhancedKeyUsage.Length == 0)
        {
          this.m_logger.Info("enhancedKeyUsage is null or empty.");
          return (IX509ExtensionEnhancedKeyUsage) null;
        }
        // ISSUE: variable of a compiler-generated type
        CObjectIds instance1 = (CObjectIds) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2001-217D-11DA-B2A4-000E7BBB2B09")));
        foreach (Oid oid in enhancedKeyUsage)
        {
          this.m_logger.Info("Processing '" + oid.FriendlyName + "' (" + oid.Value + ")");
          // ISSUE: variable of a compiler-generated type
          CObjectId instance2 = (CObjectId) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2000-217D-11DA-B2A4-000E7BBB2B09")));
          // ISSUE: reference to a compiler-generated method
          instance2.InitializeFromValue(oid.Value);
          // ISSUE: reference to a compiler-generated method
          instance1.Add(instance2);
        }
        // ISSUE: variable of a compiler-generated type
        CX509ExtensionEnhancedKeyUsage instance3 = (CX509ExtensionEnhancedKeyUsage) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("884E2010-217D-11DA-B2A4-000E7BBB2B09")));
        // ISSUE: reference to a compiler-generated method
        instance3.InitializeEncode(instance1);
        this.m_logger.Info("eku is created.");
        this.m_logger.Info("CreateEnhancedKeyUsageExtension complete.");
        return (IX509ExtensionEnhancedKeyUsage) instance3;
      }
    }
  }
}
