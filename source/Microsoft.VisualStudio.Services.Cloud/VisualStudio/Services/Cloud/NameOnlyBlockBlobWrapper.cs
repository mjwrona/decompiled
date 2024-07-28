// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.NameOnlyBlockBlobWrapper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class NameOnlyBlockBlobWrapper : ICloudBlobReadOnlyInfo
  {
    private string name;

    internal NameOnlyBlockBlobWrapper(string name) => this.name = name;

    public string Name => this.name;

    public BlobType BlobType => BlobType.BlockBlob;

    public string ContainerName => "";

    public IDictionary<string, string> Metadata => (IDictionary<string, string>) new Dictionary<string, string>();

    public Uri Uri => new Uri("https://nonexist/");

    public string ETag => (string) null;

    public string ContentMD5 => (string) null;

    public bool IsServerEncrypted => false;

    public DateTimeOffset? GetLastModified() => new DateTimeOffset?();

    public CopyState CopyState => (CopyState) null;

    public long GetLength() => 0;
  }
}
