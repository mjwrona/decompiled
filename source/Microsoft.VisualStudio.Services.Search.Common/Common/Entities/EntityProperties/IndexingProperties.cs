// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.IndexingProperties
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties
{
  [DataContract]
  public class IndexingProperties : IExtensibleDataObject
  {
    public ExtensionDataObject ExtensionData { get; set; }

    [DataMember(Order = 0)]
    public List<IndexInfo> Indices { get; set; }

    [DataMember(Order = 1)]
    public string Name { get; set; }

    [DataMember(Order = 2)]
    public bool IsDisabled { get; set; }

    [DataMember(Order = 3)]
    public List<IndexInfo> QueryIndices { get; set; }

    [DataMember(Order = 4)]
    public List<IndexInfo> IndexIndices { get; set; }

    [DataMember(Order = 5)]
    public IndexInfo MigrateIndexInfo { get; set; }

    [DataMember(Order = 6)]
    public List<IndexInfo> IndexIndicesPreReindexing { get; set; }

    [DataMember(Order = 7)]
    public List<IndexInfo> QueryIndicesPreReindexing { get; set; }

    public IndexingProperties()
    {
      this.Indices = new List<IndexInfo>();
      this.QueryIndices = new List<IndexInfo>();
      this.IndexIndices = new List<IndexInfo>();
    }

    public virtual void Initialize()
    {
      if (this.Indices == null || !this.Indices.Any<IndexInfo>() || this.IndexIndices != null && this.IndexIndices.Any<IndexInfo>())
        return;
      this.IndexIndices = this.Indices.Select<IndexInfo, IndexInfo>((Func<IndexInfo, IndexInfo>) (i => new IndexInfo()
      {
        EntityName = string.IsNullOrWhiteSpace(i.EntityName) ? this.Name : i.EntityName,
        IndexName = i.IndexName,
        Routing = i.Routing,
        Version = i.Version
      })).ToList<IndexInfo>();
    }

    public virtual bool EraseIndexingWaterMarks(bool isShadowIndexing = false) => false;

    public virtual void SaveIndexingStatePreReindexing()
    {
      this.IndexIndicesPreReindexing = this.IndexIndices;
      this.QueryIndicesPreReindexing = this.QueryIndices;
    }

    public virtual void ResetIndexingStatePreReindexing()
    {
    }

    public virtual void ErasePreReindexingState()
    {
      this.IndexIndicesPreReindexing = (List<IndexInfo>) null;
      this.QueryIndicesPreReindexing = (List<IndexInfo>) null;
    }
  }
}
