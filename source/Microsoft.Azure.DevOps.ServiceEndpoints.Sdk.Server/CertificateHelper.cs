// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.CertificateHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class CertificateHelper
  {
    private const string certHeader = "-----BEGIN CERTIFICATE-----";
    private const string certFooter = "-----END CERTIFICATE-----";

    public string StripCertHeaderFooter(string cert)
    {
      if (cert.IndexOf("-----BEGIN CERTIFICATE-----") >= 0)
      {
        int startIndex = cert.IndexOf("-----BEGIN CERTIFICATE-----") + "-----BEGIN CERTIFICATE-----".Length;
        int num = cert.IndexOf("-----END CERTIFICATE-----");
        cert = cert.Substring(startIndex, num - startIndex).Trim();
      }
      return cert;
    }

    public static string ExtractPrivateKey(string pemContent) => CertificateHelper.ExtractKey(pemContent, "-----BEGINPRIVATEKEY-----", "-----ENDPRIVATEKEY-----");

    public static string ExtractCertificate(string pemContent) => CertificateHelper.ExtractKey(pemContent, "-----BEGINCERTIFICATE-----", "-----ENDCERTIFICATE-----");

    public static string ExtractRSAPrivateKey(string pemContent) => CertificateHelper.ExtractKey(pemContent, "-----BEGINRSAPRIVATEKEY-----", "-----ENDRSAPRIVATEKEY-----");

    private static string ExtractKey(string content, string beginMark, string endMark)
    {
      string str1 = content.Replace("\\n", string.Empty).Replace(" ", string.Empty);
      int num = str1.IndexOf(beginMark, StringComparison.Ordinal);
      string str2 = num < 0 ? str1 : str1.Remove(0, num + beginMark.Length);
      int startIndex = str2.IndexOf(endMark, StringComparison.Ordinal);
      return startIndex >= 0 ? str2.Remove(startIndex) : str2;
    }
  }
}
