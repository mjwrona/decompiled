// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.SDMappingDetail
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [DataContract]
  public class SDMappingDetail
  {
    [DataMember(Name = "mappingType")]
    public SDMappingType MappingType { get; set; }

    [DataMember(Name = "sdServerName")]
    public string SDServerName { get; set; }

    [DataMember(Name = "sdServerPortNumber")]
    public string SDServerPortNumber { get; set; }

    [DataMember(Name = "folderPath")]
    public string FolderPath { get; set; }

    [DataMember(Name = "depot")]
    public string Depot { get; set; }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "Mapping Type: ").AppendLine(this.MappingType.ToString());
      sb.Append(indentSpacing, "SD Server Name: ").AppendLine(this.SDServerName);
      sb.Append(indentSpacing, "SD Server Port Number: ").AppendLine(this.SDServerPortNumber);
      if (!string.IsNullOrWhiteSpace(this.FolderPath))
        sb.Append(indentSpacing, "Folder Path: ").AppendLine(this.FolderPath);
      if (!string.IsNullOrWhiteSpace(this.Depot))
        sb.Append(indentSpacing, "Depot: ").AppendLine(this.Depot);
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
