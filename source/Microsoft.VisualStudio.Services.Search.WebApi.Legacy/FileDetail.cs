// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.FileDetail
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [DataContract]
  public class FileDetail
  {
    [DataMember(Name = "operation")]
    public FileOperation Operation { get; set; }

    [DataMember(Name = "path")]
    public string Path { get; set; }

    [DataMember(Name = "content")]
    public string Content { get; set; }

    [DataMember(Name = "branches")]
    public IEnumerable<string> Branches { get; set; }

    [DataMember(Name = "branchesInfo")]
    public IEnumerable<CustomBranchInfo> BranchesInfo { get; set; }

    [DataMember(Name = "contentHash")]
    public string ContentHash { get; set; }

    [DataMember(Name = "fileType")]
    public FileTypeEnum FileType { get; set; }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "Operation: ").AppendLine(this.Operation.ToString());
      sb.Append(indentSpacing, "Path: ").AppendLine(this.Path);
      foreach (string branch in this.Branches)
        sb.Append(indentSpacing, "branch: ").AppendLine(branch);
      if (this.BranchesInfo != null)
      {
        foreach (CustomBranchInfo customBranchInfo in this.BranchesInfo)
          sb.Append(indentSpacing, "branchesInfo: ").AppendLine(customBranchInfo.ToString(indentLevel));
      }
      sb.Append(indentSpacing, "ContentHash: ").AppendLine(this.ContentHash);
      sb.Append(indentSpacing, "FileType: ").AppendLine(this.FileType.ToString());
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
