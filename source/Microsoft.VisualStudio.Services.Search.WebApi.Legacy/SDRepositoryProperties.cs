// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.SDRepositoryProperties
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [DataContract]
  public class SDRepositoryProperties : CustomRepositoryProperties
  {
    public SDRepositoryProperties()
      : base(CustomRepositoryPropertiesType.SourceDepot)
    {
    }

    [DataMember(Name = "mappingDetails")]
    public IEnumerable<SDMappingDetail> MappingDetails { get; set; }

    [DataMember(Name = "branchDetails")]
    public IEnumerable<SDBranchDetail> BranchDetails { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "indentLevel+1", Justification = "indentLevel is not likely to exceed int.Max range.")]
    public override string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "Mapping Details: ");
      if (this.MappingDetails != null)
      {
        foreach (SDMappingDetail mappingDetail in this.MappingDetails)
          sb.AppendLine(mappingDetail.ToString(indentLevel + 1));
      }
      sb.Append(indentSpacing, "Branch Details: ");
      if (this.BranchDetails != null)
      {
        foreach (SDBranchDetail branchDetail in this.BranchDetails)
          sb.AppendLine(branchDetail.ToString(indentLevel + 1));
      }
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
