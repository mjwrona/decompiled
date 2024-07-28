// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Utility.CertificateHelper
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Cloud.Metrics.Client.Utility
{
  internal class CertificateHelper
  {
    internal static void ValidateCertificate(X509Certificate2 cert)
    {
      if (!cert.HasPrivateKey)
        throw new MetricsClientException("Cert with Thumbprint [" + cert.Thumbprint + "] and Subject Name [" + cert.Subject + "] doesn't have a private key");
      DateTime now = DateTime.Now;
      if (cert.NotBefore > now)
        throw new MetricsClientException(string.Format("The certificate is not valid until {0}.", (object) cert.GetEffectiveDateString()));
      if (cert.NotAfter < now)
        throw new MetricsClientException(string.Format("The certificate is not valid after {0}.", (object) cert.GetExpirationDateString()));
    }

    internal static X509Certificate2 FindX509Certificate(
      string thumbprintOrSubjectName,
      StoreLocation storeLocation)
    {
      X509Store x509Store = new X509Store(StoreName.My, storeLocation);
      x509Store.Open(OpenFlags.OpenExistingOnly);
      X509Certificate2Collection source;
      if (thumbprintOrSubjectName.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
      {
        source = x509Store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, (object) thumbprintOrSubjectName, false);
        if (source.Count == 0)
          throw new MetricsClientException(string.Format("No cert with subject distinguished name (full subject name) [{0}] is found in the [{1}] store", (object) thumbprintOrSubjectName, (object) storeLocation));
      }
      else
      {
        source = x509Store.Certificates.Find(X509FindType.FindByThumbprint, (object) thumbprintOrSubjectName, false);
        if (source.Count == 0)
        {
          source = x509Store.Certificates.Find(X509FindType.FindBySubjectName, (object) thumbprintOrSubjectName, false);
          if (source.Count == 0)
            throw new MetricsClientException(string.Format("No cert with thumbprint or subject name [{0}] is found in the [{1}] store", (object) thumbprintOrSubjectName, (object) storeLocation));
        }
      }
      return source.OfType<X509Certificate2>().OrderByDescending<X509Certificate2, DateTime>((Func<X509Certificate2, DateTime>) (c => c.NotBefore)).FirstOrDefault<X509Certificate2>((Func<X509Certificate2, bool>) (c => c.HasPrivateKey)) ?? throw new MetricsClientException("No cert with thumbprint or subject name [" + thumbprintOrSubjectName + "] has a private key");
    }
  }
}
