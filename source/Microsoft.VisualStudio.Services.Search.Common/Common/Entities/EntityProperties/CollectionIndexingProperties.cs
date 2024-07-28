// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.CollectionIndexingProperties
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties
{
  [DataContract]
  [Export(typeof (IndexingProperties))]
  public class CollectionIndexingProperties : IndexingProperties
  {
    [DataMember(Order = 1)]
    public DocumentContractType IndexContractType { get; set; }

    [DataMember(Order = 2)]
    public DocumentContractType QueryContractType { get; set; }

    [DataMember(Order = 3)]
    public string IndexAlias { get; set; }

    [DataMember(Order = 4)]
    public string QueryAlias { get; set; }

    [DataMember(Order = 5)]
    public string IndexESConnectionString { get; set; }

    [DataMember(Order = 6)]
    public string QueryESConnectionString { get; set; }

    [DataMember(Order = 7)]
    public DocumentContractType IndexContractTypePreReindexing { get; set; }

    [DataMember(Order = 8)]
    public DocumentContractType QueryContractTypePreReindexing { get; set; }

    public override void Initialize()
    {
      base.Initialize();
      if (this.Indices == null || !this.Indices.Any<IndexInfo>() || this.QueryIndices != null && this.QueryIndices.Any<IndexInfo>())
        return;
      this.QueryIndices = this.Indices.Select<IndexInfo, IndexInfo>((Func<IndexInfo, IndexInfo>) (i => new IndexInfo()
      {
        EntityName = string.IsNullOrWhiteSpace(i.EntityName) ? this.Name : i.EntityName,
        IndexName = i.IndexName,
        Routing = i.Routing,
        Version = i.Version
      })).ToList<IndexInfo>();
    }

    public override void SaveIndexingStatePreReindexing()
    {
      base.SaveIndexingStatePreReindexing();
      this.IndexContractTypePreReindexing = this.IndexContractType;
      this.QueryContractTypePreReindexing = this.QueryContractType;
    }

    public override void ErasePreReindexingState()
    {
      base.ErasePreReindexingState();
      this.IndexContractTypePreReindexing = DocumentContractType.Unsupported;
      this.QueryContractTypePreReindexing = DocumentContractType.Unsupported;
    }
  }
}
