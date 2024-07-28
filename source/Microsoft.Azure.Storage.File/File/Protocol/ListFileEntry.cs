// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.ListFileEntry
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Storage.File.Protocol
{
  public sealed class ListFileEntry : IListFileEntry
  {
    internal ListFileEntry(string name, CloudFileAttributes attributes)
    {
      this.Name = name;
      this.Attributes = attributes;
    }

    internal CloudFileAttributes Attributes { get; private set; }

    public string Name { get; private set; }

    public FileProperties Properties => this.Attributes.Properties;

    public IDictionary<string, string> Metadata => this.Attributes.Metadata;

    public Uri Uri => this.Attributes.Uri;
  }
}
