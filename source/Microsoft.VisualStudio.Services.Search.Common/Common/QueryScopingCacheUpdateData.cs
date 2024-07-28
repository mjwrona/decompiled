// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.QueryScopingCacheUpdateData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class QueryScopingCacheUpdateData
  {
    public QueryScopingCacheUpdateData()
    {
    }

    public QueryScopingCacheUpdateData(
      IndexingUnit indexingUnit,
      QueryScopingCacheUpdateEvent updateEvent)
    {
      this.NewEntityName = indexingUnit != null ? indexingUnit.GetTFSEntityName() : throw new ArgumentNullException(nameof (indexingUnit));
      this.IndexingUnitType = indexingUnit.IndexingUnitType;
      this.TfsEntityId = indexingUnit.TFSEntityId;
      this.parentUnitId = indexingUnit.ParentUnitId;
      this.IndexingUnitId = indexingUnit.IndexingUnitId;
      this.EventType = updateEvent;
      this.EntityType = indexingUnit.EntityType;
    }

    [DataMember(Order = 0)]
    public Guid TfsEntityId { get; set; }

    [DataMember(Order = 1)]
    public string IndexingUnitType { get; set; }

    [DataMember(Order = 2)]
    public int parentUnitId { get; set; }

    [DataMember(Order = 3)]
    public string OldEntityName { get; set; }

    [DataMember(Order = 4)]
    public string NewEntityName { get; set; }

    [DataMember(Order = 5)]
    public QueryScopingCacheUpdateEvent EventType { get; set; }

    [DataMember(Order = 6)]
    public IDictionary<string, string> ParentHierarchy { get; set; }

    [DataMember(Order = 7)]
    public int IndexingUnitId { get; set; }

    [DataMember(Order = 8)]
    public IEntityType EntityType { get; set; }
  }
}
