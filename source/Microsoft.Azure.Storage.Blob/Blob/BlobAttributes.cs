// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobAttributes
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Storage.Blob
{
  internal sealed class BlobAttributes
  {
    internal BlobAttributes()
    {
      this.Properties = new BlobProperties();
      this.Metadata = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public BlobProperties Properties { get; internal set; }

    public IDictionary<string, string> Metadata { get; internal set; }

    public Uri Uri => this.StorageUri.PrimaryUri;

    public StorageUri StorageUri { get; internal set; }

    public DateTimeOffset? SnapshotTime { get; internal set; }

    public bool IsDeleted { get; internal set; }

    public CopyState CopyState { get; internal set; }

    internal void AssertNoSnapshot()
    {
      if (this.SnapshotTime.HasValue)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot perform this operation on a blob representing a snapshot."));
    }

    internal void AssertNotDeleted()
    {
      if (this.IsDeleted)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot perform this operation on a deleted blob."));
    }
  }
}
