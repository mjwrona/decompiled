// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.ListFileDirectoryEntry
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using System;

namespace Microsoft.Azure.Storage.File.Protocol
{
  public sealed class ListFileDirectoryEntry : IListFileEntry
  {
    internal ListFileDirectoryEntry(string name, Uri uri, FileDirectoryProperties properties)
    {
      this.Name = name;
      this.Uri = uri;
      this.Properties = properties;
    }

    public string Name { get; internal set; }

    public Uri Uri { get; internal set; }

    public FileDirectoryProperties Properties { get; internal set; }
  }
}
