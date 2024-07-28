// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.FilesMetadataRequest
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.CustomRepository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [DataContract]
  [Serializable]
  public class FilesMetadataRequest
  {
    public FilesMetadataRequest() => this.RequiredMetadata = (IEnumerable<MetadataType>) new List<MetadataType>();

    [DataMember(Name = "requiredMetadata")]
    public IEnumerable<MetadataType> RequiredMetadata { get; set; }

    [DataMember(Name = "paginationInfo")]
    public PaginationDetails PaginationInfo { get; set; }

    [DataMember(Name = "ScopePaths")]
    public List<string> ScopePaths { get; set; }

    [DataMember(Name = "NumberOfDocsToFetch")]
    public int NumberOfDocsToFetch { get; set; }

    [DataMember(Name = "CustomIndexingMode")]
    public virtual CustomIndexingMode CustomIndexingMode { get; set; }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "Required Metadata: ");
      foreach (MetadataType metadataType in this.RequiredMetadata)
        sb.AppendLine(metadataType.ToString());
      sb.Append(indentSpacing, "PaginationInfo: ").AppendLine(this.PaginationInfo.ToString());
      sb.Append(indentSpacing, "ScopePaths: ").AppendLine(this.ScopePaths.ToString());
      sb.Append(indentSpacing, "NumberOfDocsToFetch: ").AppendLine(this.NumberOfDocsToFetch.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      sb.Append(indentSpacing, "CustomIndexingMode: ").AppendLine(this.CustomIndexingMode.ToString());
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
