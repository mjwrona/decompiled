// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.ObjectIdentifier
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.KeyVault
{
  public class ObjectIdentifier
  {
    private string _vault;
    private string _vaultWithoutScheme;
    private string _name;
    private string _version;
    private string _baseIdentifier;
    private string _identifier;

    protected static bool IsObjectIdentifier(string collection, string identifier)
    {
      if (string.IsNullOrEmpty(collection))
        throw new ArgumentNullException(nameof (collection));
      if (string.IsNullOrEmpty(identifier))
        return false;
      try
      {
        Uri uri = new Uri(identifier, UriKind.Absolute);
        return (uri.Segments.Length == 3 || uri.Segments.Length == 4) && string.Equals(uri.Segments[1], collection + "/", StringComparison.OrdinalIgnoreCase);
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    protected ObjectIdentifier()
    {
    }

    protected ObjectIdentifier(
      string vaultBaseUrl,
      string collection,
      string name,
      string version = "")
    {
      if (string.IsNullOrEmpty(vaultBaseUrl))
        throw new ArgumentNullException(nameof (vaultBaseUrl));
      if (string.IsNullOrEmpty(collection))
        throw new ArgumentNullException(nameof (collection));
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("keyName");
      Uri uri = new Uri(vaultBaseUrl, UriKind.Absolute);
      this._name = name;
      this._version = version;
      this._vault = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}", new object[2]
      {
        (object) uri.Scheme,
        (object) uri.FullAuthority()
      });
      this._vaultWithoutScheme = uri.Authority;
      this._baseIdentifier = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[3]
      {
        (object) this._vault,
        (object) collection,
        (object) this._name
      });
      string str;
      if (!string.IsNullOrEmpty(this._version))
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
        {
          (object) this._name,
          (object) this._version
        });
      else
        str = this._name;
      this._identifier = str;
      this._identifier = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[3]
      {
        (object) this._vault,
        (object) collection,
        (object) this._identifier
      });
    }

    protected ObjectIdentifier(string collection, string identifier)
    {
      if (string.IsNullOrEmpty(collection))
        throw new ArgumentNullException(nameof (collection));
      Uri uri = !string.IsNullOrEmpty(identifier) ? new Uri(identifier, UriKind.Absolute) : throw new ArgumentNullException(nameof (identifier));
      if (uri.Segments.Length != 3 && uri.Segments.Length != 4)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid ObjectIdentifier: {0}. Bad number of segments: {1}", new object[2]
        {
          (object) identifier,
          (object) uri.Segments.Length
        }));
      if (!string.Equals(uri.Segments[1], collection + "/", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid ObjectIdentifier: {0}. segment [1] should be '{1}/', found '{2}'", new object[3]
        {
          (object) identifier,
          (object) collection,
          (object) uri.Segments[1]
        }));
      this._name = uri.Segments[2].Substring(0, uri.Segments[2].Length).TrimEnd('/');
      if (uri.Segments.Length == 4)
        this._version = uri.Segments[3].Substring(0, uri.Segments[3].Length).TrimEnd('/');
      else
        this._version = string.Empty;
      this._vault = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}", new object[2]
      {
        (object) uri.Scheme,
        (object) uri.FullAuthority()
      });
      this._vaultWithoutScheme = uri.Authority;
      this._baseIdentifier = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[3]
      {
        (object) this._vault,
        (object) collection,
        (object) this._name
      });
      string str;
      if (!string.IsNullOrEmpty(this._version))
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
        {
          (object) this._name,
          (object) this._version
        });
      else
        str = this._name;
      this._identifier = str;
      this._identifier = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[3]
      {
        (object) this._vault,
        (object) collection,
        (object) this._identifier
      });
    }

    public string BaseIdentifier
    {
      get => this._baseIdentifier;
      protected set => this._baseIdentifier = value;
    }

    public string Identifier
    {
      get => this._identifier;
      protected set => this._identifier = value;
    }

    public string Name
    {
      get => this._name;
      protected set => this._name = value;
    }

    public string Vault
    {
      get => this._vault;
      protected set => this._vault = value;
    }

    public string VaultWithoutScheme
    {
      get => this._vaultWithoutScheme;
      protected set => this._vaultWithoutScheme = value;
    }

    public string Version
    {
      get => this._version;
      protected set => this._version = value;
    }

    public override string ToString() => this._identifier;
  }
}
