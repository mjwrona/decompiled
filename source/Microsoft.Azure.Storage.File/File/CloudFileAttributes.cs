// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.CloudFileAttributes
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Storage.File
{
  internal sealed class CloudFileAttributes
  {
    internal CloudFileAttributes()
    {
      this.Properties = new FileProperties();
      this.Metadata = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public FileProperties Properties { get; internal set; }

    public IDictionary<string, string> Metadata { get; internal set; }

    public Uri Uri => this.StorageUri.PrimaryUri;

    public StorageUri StorageUri { get; internal set; }

    public CopyState CopyState { get; internal set; }
  }
}
