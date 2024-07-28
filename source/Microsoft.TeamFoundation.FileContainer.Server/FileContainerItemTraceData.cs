// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerItemTraceData
// Assembly: Microsoft.TeamFoundation.FileContainer.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D69E508F-D51F-4EF2-B979-7E96E61DA19E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.FileContainer.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerItemTraceData
  {
    public FileContainerItemTraceData(
      FileContainerItem containerItem,
      Microsoft.VisualStudio.Services.FileContainer.FileContainer container = null,
      bool exceptionThrown = false,
      long offset = 0)
    {
      this.Length = containerItem.FileLength;
      this.Prefix = container?.ArtifactUri?.AbsoluteUri ?? string.Empty;
      this.FileExtension = this.ExtractExtension(containerItem.Path);
      this.ItemType = (int) containerItem.ItemType;
      this.ExceptionThrown = exceptionThrown;
      this.Offset = offset;
      this.IsArtifact = containerItem.ArtifactId.HasValue;
    }

    private string ExtractExtension(string a)
    {
      int num = a.LastIndexOf('.');
      return num == -1 ? string.Empty : a.Substring(num + 1);
    }

    public long Length { get; private set; }

    public string Prefix { get; private set; }

    public string FileExtension { get; private set; }

    public int ItemType { get; private set; }

    public bool ExceptionThrown { get; private set; }

    public long Offset { get; private set; }

    public bool IsArtifact { get; private set; }
  }
}
