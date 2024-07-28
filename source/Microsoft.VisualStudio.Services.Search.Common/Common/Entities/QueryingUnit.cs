// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.QueryingUnit
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class QueryingUnit : ICloneable
  {
    public IEnumerable<IndexInfo> QueryIndexInfos { get; set; }

    public string IndexingUnitType { get; set; }

    public IDictionary<string, QueryingUnit> ChildRoutingUnits { get; set; }

    public bool IsLargeRepo { get; set; }

    public string EntityName { get; set; }

    public int IndexingUnitId { get; set; }

    public Guid TFSEntityId { get; set; }

    public QueryingUnit(IEnumerable<IndexInfo> queryIndexInfos, string indexingUnitType)
    {
      this.QueryIndexInfos = queryIndexInfos;
      this.IndexingUnitType = indexingUnitType;
      this.ChildRoutingUnits = (IDictionary<string, QueryingUnit>) null;
    }

    public QueryingUnit()
    {
    }

    public QueryingUnit(IndexingUnit indexingUnit)
    {
      this.IndexingUnitType = indexingUnit != null ? indexingUnit.IndexingUnitType : throw new ArgumentNullException(nameof (indexingUnit));
      this.EntityName = indexingUnit.GetTFSEntityName().NormalizePath();
      this.IndexingUnitId = indexingUnit.IndexingUnitId;
      this.TFSEntityId = indexingUnit.TFSEntityId;
      this.QueryIndexInfos = (IEnumerable<IndexInfo>) null;
      this.IsLargeRepo = false;
      this.ChildRoutingUnits = (IDictionary<string, QueryingUnit>) null;
    }

    public virtual object Clone() => (object) new QueryingUnit()
    {
      QueryIndexInfos = this.QueryIndexInfos,
      IndexingUnitType = this.IndexingUnitType,
      ChildRoutingUnits = this.ChildRoutingUnits,
      IsLargeRepo = this.IsLargeRepo,
      EntityName = this.EntityName,
      IndexingUnitId = this.IndexingUnitId,
      TFSEntityId = this.TFSEntityId
    };

    public void AddChildRoutingUnit(QueryingUnit queryingUnit)
    {
      if (queryingUnit == null)
        throw new ArgumentNullException(nameof (queryingUnit));
      if (this.ChildRoutingUnits == null)
        this.ChildRoutingUnits = (IDictionary<string, QueryingUnit>) new ConcurrentDictionary<string, QueryingUnit>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (queryingUnit.EntityName == null)
        return;
      if (this.ChildRoutingUnits.ContainsKey(queryingUnit.EntityName))
      {
        this.ChildRoutingUnits.Remove(queryingUnit.EntityName);
        if (queryingUnit.IndexingUnitType != "CustomRepository")
          queryingUnit.QueryIndexInfos = (IEnumerable<IndexInfo>) null;
      }
      this.ChildRoutingUnits.Add(queryingUnit.EntityName, queryingUnit);
    }

    public bool RemoveChildQueryingUnit(string entityName)
    {
      IDictionary<string, QueryingUnit> childRoutingUnits = this.ChildRoutingUnits;
      if ((childRoutingUnits != null ? (childRoutingUnits.ContainsKey(entityName) ? 1 : 0) : 0) == 0)
        return false;
      this.ChildRoutingUnits.GetValueOrDefault<string, QueryingUnit>(entityName).ChildRoutingUnits?.Clear();
      this.ChildRoutingUnits.Remove(entityName);
      return true;
    }

    public QueryingUnit GetChilQueryingUnit(string childName)
    {
      if (string.IsNullOrWhiteSpace(childName))
        return (QueryingUnit) null;
      IDictionary<string, QueryingUnit> childRoutingUnits = this.ChildRoutingUnits;
      return childRoutingUnits == null ? (QueryingUnit) null : childRoutingUnits.GetValueOrDefault<string, QueryingUnit>(childName);
    }
  }
}
