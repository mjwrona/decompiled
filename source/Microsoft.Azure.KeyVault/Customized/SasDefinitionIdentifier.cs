// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.SasDefinitionIdentifier
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.KeyVault
{
  public sealed class SasDefinitionIdentifier : ObjectIdentifier
  {
    public string StorageAccount { get; set; }

    public static bool IsSasDefinitionIdentifier(string identifier)
    {
      if (string.IsNullOrEmpty(identifier))
        return false;
      Uri uri = new Uri(identifier, UriKind.Absolute);
      return uri.Segments.Length == 5 && string.Equals(uri.Segments[1], "storage/") && string.Equals(uri.Segments[3], "sas/");
    }

    public SasDefinitionIdentifier(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName)
    {
      if (string.IsNullOrEmpty(vaultBaseUrl))
        throw new ArgumentNullException(nameof (vaultBaseUrl));
      if (string.IsNullOrEmpty(storageAccountName))
        throw new ArgumentNullException(nameof (storageAccountName));
      if (string.IsNullOrEmpty(sasDefinitionName))
        throw new ArgumentNullException(nameof (sasDefinitionName));
      Uri uri = new Uri(vaultBaseUrl, UriKind.Absolute);
      this.StorageAccount = storageAccountName;
      this.Name = sasDefinitionName;
      this.Version = string.Empty;
      this.Vault = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}", new object[2]
      {
        (object) uri.Scheme,
        (object) uri.FullAuthority()
      });
      this.VaultWithoutScheme = uri.Authority;
      this.BaseIdentifier = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}", (object) this.Vault, (object) "storage", (object) this.StorageAccount, (object) "sas", (object) this.Name);
      this.Identifier = this.BaseIdentifier;
    }

    public SasDefinitionIdentifier(string identifier)
    {
      Uri uri = !string.IsNullOrEmpty(identifier) ? new Uri(identifier, UriKind.Absolute) : throw new ArgumentNullException(nameof (identifier));
      if (uri.Segments.Length != 5)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid ObjectIdentifier: {0}. Bad number of segments: {1}", new object[2]
        {
          (object) identifier,
          (object) uri.Segments.Length
        }));
      if (!string.Equals(uri.Segments[1], "storage/"))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid ObjectIdentifier: {0}. segment [1] should be '{1}', found '{2}'", new object[3]
        {
          (object) identifier,
          (object) "storage/",
          (object) uri.Segments[1]
        }));
      if (!string.Equals(uri.Segments[3], "sas/"))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid ObjectIdentifier: {0}. segment [3] should be '{1}', found '{2}'", new object[3]
        {
          (object) identifier,
          (object) "sas/",
          (object) uri.Segments[3]
        }));
      this.StorageAccount = uri.Segments[2].Substring(0, uri.Segments[2].Length).TrimEnd('/');
      this.Name = uri.Segments[4].Substring(0, uri.Segments[4].Length).TrimEnd('/');
      this.Version = string.Empty;
      this.Vault = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}", new object[2]
      {
        (object) uri.Scheme,
        (object) uri.FullAuthority()
      });
      this.VaultWithoutScheme = uri.Authority;
      this.BaseIdentifier = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}", (object) this.Vault, (object) "storage", (object) this.StorageAccount, (object) "sas", (object) this.Name);
      this.Identifier = this.BaseIdentifier;
    }
  }
}
