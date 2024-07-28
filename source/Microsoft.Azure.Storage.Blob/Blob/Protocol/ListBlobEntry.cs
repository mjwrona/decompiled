// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.ListBlobEntry
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  public sealed class ListBlobEntry : IListBlobEntry
  {
    internal ListBlobEntry(string name, BlobAttributes attributes)
    {
      this.Name = name;
      this.Attributes = attributes;
    }

    internal BlobAttributes Attributes { get; private set; }

    public string Name { get; private set; }

    public BlobProperties Properties => this.Attributes.Properties;

    public IDictionary<string, string> Metadata => this.Attributes.Metadata;

    public Uri Uri => this.Attributes.Uri;

    public DateTimeOffset? SnapshotTime => this.Attributes.SnapshotTime;

    public CopyState CopyState => this.Attributes.CopyState;
  }
}
