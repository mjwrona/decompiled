// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AADTenantMetadata
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class AADTenantMetadata : IAADTenantMetadata
  {
    private AADTenantMetadata()
    {
    }

    public string TenantId { get; private set; }

    public string IssuanceEndpoint { get; private set; }

    public string Issuer { get; private set; }

    public IEnumerable<ISigningKey> SigningKeys { get; private set; }

    internal static IAADTenantMetadata FromFedAuthResponse(XElement metadata)
    {
      XNamespace xnamespace1 = (XNamespace) "urn:oasis:names:tc:SAML:2.0:metadata";
      XNamespace xsiNs = (XNamespace) "http://www.w3.org/2001/XMLSchema-instance";
      XNamespace xnamespace2 = (XNamespace) "http://docs.oasis-open.org/wsfed/federation/200706";
      XNamespace xnamespace3 = (XNamespace) "http://www.w3.org/2005/08/addressing";
      XNamespace dsigNs = (XNamespace) "http://www.w3.org/2000/09/xmldsig#";
      AADTenantMetadata aadTenantMetadata = new AADTenantMetadata();
      string a = new Uri(metadata.Attribute((XName) "entityID").Value).LocalPath.Replace("/", "");
      if (!string.Equals(a, "{tenantId}", StringComparison.OrdinalIgnoreCase))
        aadTenantMetadata.TenantId = a;
      XElement xelement1 = metadata.Elements(xnamespace1 + "RoleDescriptor").FirstOrDefault<XElement>((Func<XElement, bool>) (x => x.Attribute(xsiNs + "type").Value == "fed:SecurityTokenServiceType"));
      IEnumerable<XElement> xelements = xelement1.Elements(xnamespace1 + "KeyDescriptor").Where<XElement>((Func<XElement, bool>) (x => x.Attribute((XName) "use").Value == "signing")).SelectMany<XElement, XElement>((Func<XElement, IEnumerable<XElement>>) (x => x.Descendants(dsigNs + "X509Certificate")));
      List<AADTenantMetadataSigningKey> metadataSigningKeyList = new List<AADTenantMetadataSigningKey>();
      foreach (XElement xelement2 in xelements)
      {
        string s = xelement2.Value;
        X509Certificate2 x509Certificate2 = new X509Certificate2(Convert.FromBase64String(s));
        metadataSigningKeyList.Add(new AADTenantMetadataSigningKey()
        {
          CertValue = s,
          Thumbprint = x509Certificate2.GetCertHash().ToBase64StringNoPadding()
        });
      }
      aadTenantMetadata.SigningKeys = (IEnumerable<ISigningKey>) metadataSigningKeyList;
      XElement xelement3 = xelement1.Elements(xnamespace2 + "SecurityTokenServiceEndpoint").Descendants<XElement>(xnamespace3 + "Address").FirstOrDefault<XElement>();
      if (xelement3 != null)
        aadTenantMetadata.IssuanceEndpoint = xelement3.Value;
      return (IAADTenantMetadata) aadTenantMetadata;
    }

    internal static IAADTenantMetadata FromOpenIdResponse(
      JObject metadata,
      JObject keyData,
      IEnumerable<string> validEndorsements = null)
    {
      AADTenantMetadata aadTenantMetadata = new AADTenantMetadata();
      aadTenantMetadata.Issuer = (string) metadata["issuer"];
      string a = new Uri(aadTenantMetadata.Issuer).LocalPath.Replace("/", "");
      if (!string.Equals(a, "(tenantId}", StringComparison.OrdinalIgnoreCase))
        aadTenantMetadata.TenantId = a;
      aadTenantMetadata.IssuanceEndpoint = (string) metadata["token_endpoint"];
      List<AADTenantMetadataSigningKey> metadataSigningKeyList = new List<AADTenantMetadataSigningKey>();
      foreach (JToken key in (IEnumerable<JToken>) keyData["keys"])
      {
        string str1 = (string) key[(object) "use"];
        if (str1 != null && str1.Equals("sig", StringComparison.OrdinalIgnoreCase))
        {
          string str2 = (string) key[(object) "kty"];
          if (!string.IsNullOrEmpty(str2) && str2.Equals("RSA", StringComparison.OrdinalIgnoreCase))
          {
            string str3 = (string) key[(object) "x5t"];
            string str4 = (string) key[(object) "x5c"].First<JToken>();
            if (!string.IsNullOrEmpty(str3) && !string.IsNullOrEmpty(str4) && AADTenantMetadata.CheckEndorsements(key, validEndorsements))
              metadataSigningKeyList.Add(new AADTenantMetadataSigningKey()
              {
                Thumbprint = str3,
                CertValue = str4
              });
          }
        }
      }
      aadTenantMetadata.SigningKeys = (IEnumerable<ISigningKey>) metadataSigningKeyList;
      return (IAADTenantMetadata) aadTenantMetadata;
    }

    private static bool CheckEndorsements(JToken key, IEnumerable<string> validEndorsements)
    {
      JToken source = key[(object) "endorsements"];
      return validEndorsements.IsNullOrEmpty<string>() || source == null || source.Type != JTokenType.Array || !source.Any<JToken>() || source.Any<JToken>((Func<JToken, bool>) (e => e.Type == JTokenType.String && validEndorsements.Contains<string>((string) e)));
    }
  }
}
