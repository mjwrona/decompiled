// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.ICertHandler
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public interface ICertHandler
  {
    void Add(StoreName storeName, StoreLocation storeLocation, X509Certificate2 cert);

    void AddAccessRule(
      X509Certificate2 certificate,
      IdentityReference identity,
      CryptoKeyRights rights,
      AccessControlType accessType);

    bool CheckSslCertDomain(X509Certificate2 certificate, string domain);

    bool ExecuteMakeCert(string args, out string errorMsg);

    X509Certificate2 FindCertificate(
      StoreName storeName,
      StoreLocation storeLocation,
      X509FindType findType,
      object findValue,
      bool validOnly);

    X509Certificate2 FindCertificate(
      string storeName,
      StoreLocation storeLocation,
      X509FindType findType,
      object findValue,
      bool validOnly);

    X509Certificate2 FindCertificateByThumbprint(
      StoreName storeName,
      StoreLocation storeLocation,
      string thumbprint,
      bool validOnly);

    X509Certificate2 FindCertificateByThumbprint(string thumbprint);

    string[] GetSubjectAlternativeNames(X509Certificate2 certificate);

    string GetSubjectCommonName(X509Certificate2 certificate);

    void GrantReadAccessToNetworkService(X509Certificate2 certificate);

    bool IsSelfSigned(X509Certificate2 certificate);

    void LogCertificateInfo(X509Certificate2 cert);

    X509Certificate2 MakeOAuthSigningCertificate();

    X509Certificate2 MakeServiceCert();

    X509Certificate2 MakeServiceCert(string makeCertArgs);

    X509Certificate2 MakeServiceCert(string makeCertArgs, string certName);

    X509Certificate2 MakeServiceCertWithName(string certName);

    void Remove(string storeName, StoreLocation storeLocation, string thumbprint);
  }
}
