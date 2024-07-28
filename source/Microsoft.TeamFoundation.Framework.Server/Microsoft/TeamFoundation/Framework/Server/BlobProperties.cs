// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BlobProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class BlobProperties
  {
    public BlobProperties(FileContainerItem fromItem)
      : this(fromItem.Path, new DateTimeOffset?((DateTimeOffset) fromItem.DateLastModified.ToUniversalTime()), fromItem.FileLength)
    {
    }

    public BlobProperties(string name, DateTimeOffset? lastModified, long length)
    {
      this.Name = name;
      this.LastModified = lastModified;
      this.Length = length;
    }

    public string Name { get; private set; }

    public DateTimeOffset? LastModified { get; private set; }

    public long Length { get; private set; }
  }
}
