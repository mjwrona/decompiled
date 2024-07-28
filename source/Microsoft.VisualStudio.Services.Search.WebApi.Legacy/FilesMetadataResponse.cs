// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.FilesMetadataResponse
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
  public class FilesMetadataResponse
  {
    public FilesMetadataResponse() => this.FilesMetaData = (IEnumerable<IDictionary<MetadataType, List<string>>>) new List<Dictionary<MetadataType, List<string>>>();

    [DataMember(Name = "filesmetadata")]
    public IEnumerable<IDictionary<MetadataType, List<string>>> FilesMetaData { get; set; }

    [DataMember(Name = "paginationInfo")]
    public PaginationDetails PaginationInfo { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "indentLevel+1", Justification = "indentLevel is not likely to exceed int.Max range.")]
    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing1 = Extensions.GetIndentSpacing(indentLevel);
      if (this.FilesMetaData != null)
      {
        string indentSpacing2 = Extensions.GetIndentSpacing(indentLevel + 1);
        sb.AppendLine(indentSpacing1, "Files: ");
        foreach (IEnumerable<KeyValuePair<MetadataType, List<string>>> keyValuePairs in this.FilesMetaData)
        {
          foreach (KeyValuePair<MetadataType, List<string>> keyValuePair in keyValuePairs)
            sb.Append(indentSpacing2, keyValuePair.Key.ToString()).Append(":").AppendLine(string.Join(",", (IEnumerable<string>) keyValuePair.Value));
        }
      }
      sb.Append(indentSpacing1, "PaginationInfo: ").AppendLine(this.PaginationInfo.ToString());
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
