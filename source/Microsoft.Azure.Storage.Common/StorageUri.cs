// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.StorageUri
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Storage
{
  public sealed class StorageUri : IEquatable<StorageUri>
  {
    private Uri primaryUri;
    private Uri secondaryUri;

    public Uri PrimaryUri
    {
      get => this.primaryUri;
      private set
      {
        StorageUri.AssertAbsoluteUri(value);
        this.primaryUri = value;
      }
    }

    public Uri SecondaryUri
    {
      get => this.secondaryUri;
      private set
      {
        StorageUri.AssertAbsoluteUri(value);
        this.secondaryUri = value;
      }
    }

    public StorageUri(Uri primaryUri)
      : this(primaryUri, (Uri) null)
    {
    }

    public StorageUri(Uri primaryUri, Uri secondaryUri)
    {
      if (primaryUri != (Uri) null && secondaryUri != (Uri) null)
      {
        bool flag1 = CommonUtility.UsePathStyleAddressing(primaryUri);
        bool flag2 = CommonUtility.UsePathStyleAddressing(secondaryUri);
        if (!flag1 && !flag2)
        {
          if (primaryUri.PathAndQuery != secondaryUri.PathAndQuery)
            throw new ArgumentException("Primary and secondary location URIs in a StorageUri must point to the same resource.", nameof (secondaryUri));
        }
        else if (!((IEnumerable<string>) primaryUri.Segments).Skip<string>(flag1 ? 2 : 0).SequenceEqual<string>(((IEnumerable<string>) secondaryUri.Segments).Skip<string>(flag2 ? 2 : 0)) || primaryUri.Query != secondaryUri.Query)
          throw new ArgumentException("Primary and secondary location URIs in a StorageUri must point to the same resource.", nameof (secondaryUri));
      }
      this.PrimaryUri = primaryUri;
      this.SecondaryUri = secondaryUri;
    }

    public Uri GetUri(StorageLocation location)
    {
      if (location == StorageLocation.Primary)
        return this.PrimaryUri;
      if (location == StorageLocation.Secondary)
        return this.SecondaryUri;
      CommonUtility.ArgumentOutOfRange(nameof (location), (object) location);
      return (Uri) null;
    }

    internal bool ValidateLocationMode(LocationMode mode)
    {
      if (mode == LocationMode.PrimaryOnly)
        return this.PrimaryUri != (Uri) null;
      if (mode == LocationMode.SecondaryOnly)
        return this.SecondaryUri != (Uri) null;
      return this.PrimaryUri != (Uri) null && this.SecondaryUri != (Uri) null;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Primary = '{0}'; Secondary = '{1}'", (object) this.PrimaryUri, (object) this.SecondaryUri);

    public override int GetHashCode() => (this.PrimaryUri != (Uri) null ? this.PrimaryUri.GetHashCode() : 0) ^ (this.SecondaryUri != (Uri) null ? this.SecondaryUri.GetHashCode() : 0);

    public override bool Equals(object obj) => this.Equals(obj as StorageUri);

    public bool Equals(StorageUri other) => other != (StorageUri) null && this.PrimaryUri == other.PrimaryUri && this.SecondaryUri == other.SecondaryUri;

    public static bool operator ==(StorageUri uri1, StorageUri uri2)
    {
      if ((object) uri1 == (object) uri2)
        return true;
      return (object) uri1 != null && uri1.Equals(uri2);
    }

    public static bool operator !=(StorageUri uri1, StorageUri uri2) => !(uri1 == uri2);

    private static void AssertAbsoluteUri(Uri uri)
    {
      if (uri != (Uri) null && !uri.IsAbsoluteUri)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Address '{0}' is a relative address. Only absolute addresses are permitted.", (object) uri.ToString()), nameof (uri));
    }
  }
}
