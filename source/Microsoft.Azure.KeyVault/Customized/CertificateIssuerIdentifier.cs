// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.CertificateIssuerIdentifier
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.KeyVault
{
  public sealed class CertificateIssuerIdentifier : ObjectIdentifier
  {
    public static bool IsIssuerIdentifier(string identifier)
    {
      if (string.IsNullOrEmpty(identifier))
        return false;
      Uri uri = new Uri(identifier, UriKind.Absolute);
      return uri.Segments.Length == 4 && string.Equals(uri.Segments[1], "certificates/") && string.Equals(uri.Segments[2], "issuers/");
    }

    public CertificateIssuerIdentifier(string vaultBaseUrl, string name)
    {
      if (string.IsNullOrEmpty(vaultBaseUrl))
        throw new ArgumentNullException(nameof (vaultBaseUrl));
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      Uri uri = new Uri(vaultBaseUrl, UriKind.Absolute);
      this.Name = name;
      this.Version = string.Empty;
      this.Vault = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}", new object[2]
      {
        (object) uri.Scheme,
        (object) uri.FullAuthority()
      });
      this.VaultWithoutScheme = uri.Authority;
      this.BaseIdentifier = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[3]
      {
        (object) this.Vault,
        (object) "certificates/issuers",
        (object) this.Name
      });
      string str;
      if (!string.IsNullOrEmpty(this.Version))
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
        {
          (object) this.Name,
          (object) this.Version
        });
      else
        str = this.Name;
      this.Identifier = str;
      this.Identifier = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[3]
      {
        (object) this.Vault,
        (object) "certificates/issuers",
        (object) this.Identifier
      });
    }

    public CertificateIssuerIdentifier(string identifier)
    {
      Uri uri = !string.IsNullOrEmpty(identifier) ? new Uri(identifier, UriKind.Absolute) : throw new ArgumentNullException(nameof (identifier));
      if (uri.Segments.Length != 4)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid ObjectIdentifier: {0}. Bad number of segments: {1}", new object[2]
        {
          (object) identifier,
          (object) uri.Segments.Length
        }));
      if (!string.Equals(uri.Segments[1], "certificates/"))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid ObjectIdentifier: {0}. segment [1] should be '{1}', found '{2}'", new object[3]
        {
          (object) identifier,
          (object) "certificates/",
          (object) uri.Segments[1]
        }));
      if (!string.Equals(uri.Segments[2], "issuers/"))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid ObjectIdentifier: {0}. segment [1] should be '{1}', found '{2}'", new object[3]
        {
          (object) identifier,
          (object) "issuers/",
          (object) uri.Segments[1]
        }));
      this.Name = uri.Segments[3].Substring(0, uri.Segments[3].Length).TrimEnd('/');
      this.Version = string.Empty;
      this.Vault = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}", new object[2]
      {
        (object) uri.Scheme,
        (object) uri.FullAuthority()
      });
      this.VaultWithoutScheme = uri.Authority;
      this.BaseIdentifier = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[3]
      {
        (object) this.Vault,
        (object) "certificates/issuers",
        (object) this.Name
      });
      string str;
      if (!string.IsNullOrEmpty(this.Version))
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
        {
          (object) this.Name,
          (object) this.Version
        });
      else
        str = this.Name;
      this.Identifier = str;
      this.Identifier = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[3]
      {
        (object) this.Vault,
        (object) "certificates/issuers",
        (object) this.Identifier
      });
    }
  }
}
