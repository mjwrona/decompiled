// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.BulkCodeIndexRequest
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.CustomRepository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [DataContract]
  public class BulkCodeIndexRequest
  {
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "virtual needed to mock the object in tests.")]
    public BulkCodeIndexRequest() => this.FileDetail = new Collection<Microsoft.VisualStudio.Services.Search.WebApi.FileDetail>();

    [DataMember(Name = "CustomIndexingMode")]
    public virtual CustomIndexingMode CustomIndexingMode { get; set; }

    [DataMember(Name = "project")]
    public string ProjectName { get; set; }

    [DataMember(Name = "repository")]
    public string RepositoryName { get; set; }

    [DataMember(Name = "topFolder")]
    public string TopFolder { get; set; }

    [DataMember(Name = "docCountInTopFolder")]
    public int DocCountInTopFolder { get; set; }

    [DataMember(Name = "fileDetail")]
    public virtual Collection<Microsoft.VisualStudio.Services.Search.WebApi.FileDetail> FileDetail { get; set; }

    [DataMember(Name = "isLastRequest")]
    public virtual bool IsLastRequest { get; set; }

    [DataMember(Name = "correlationId")]
    public virtual string CorrelationId { get; set; }

    [DataMember(Name = "triggerTimeUTC")]
    public virtual long TriggerTimeUTC { get; set; }

    [DataMember(Name = "BranchToLatestChangeMap")]
    public virtual Dictionary<string, DepotLastChangeInfo> BranchToLatestChangeMap { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "indentLevel+1", Justification = "indentLevel is not likely to exceed int.Max range.")]
    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "ProjectName: ").AppendLine(this.ProjectName);
      sb.Append(indentSpacing, "RepositoryName: ").AppendLine(this.RepositoryName);
      sb.Append(indentSpacing, "TopFolder: ").AppendLine(this.TopFolder);
      sb.Append(indentSpacing, "TopFolderDocCount: ").AppendLine(this.DocCountInTopFolder.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      sb.Append(indentSpacing, "FileDetail: ");
      foreach (Microsoft.VisualStudio.Services.Search.WebApi.FileDetail fileDetail in this.FileDetail)
        sb.AppendLine(fileDetail.ToString(indentLevel + 1));
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
