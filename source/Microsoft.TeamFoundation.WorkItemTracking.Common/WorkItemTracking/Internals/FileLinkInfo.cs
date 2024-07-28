// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.FileLinkInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FileLinkInfo : LinkInfo
  {
    public string Path { get; set; }

    public string Attribute { get; set; }

    public int ExtId { get; set; }

    public FileLinkInfo()
    {
      this.Path = string.Empty;
      this.Attribute = string.Empty;
      this.ExtId = 0;
    }

    public FileLinkInfo(FileLinkInfo link)
      : base((LinkInfo) link)
    {
      this.Path = link.Path;
      this.Attribute = link.Attribute;
      this.ExtId = link.ExtId;
    }

    public override int GetHashCode() => string.IsNullOrEmpty(this.Path) ? base.GetHashCode() : this.Path.GetHashCode();

    public override bool Equals(object obj) => obj != null && obj is FileLinkInfo fileLinkInfo && (string.IsNullOrEmpty(this.Attribute) && string.IsNullOrEmpty(fileLinkInfo.Attribute) || VssStringComparer.ArtifactType.Equals(this.Attribute, fileLinkInfo.Attribute)) && string.Equals(this.Path, fileLinkInfo.Path, StringComparison.Ordinal);
  }
}
