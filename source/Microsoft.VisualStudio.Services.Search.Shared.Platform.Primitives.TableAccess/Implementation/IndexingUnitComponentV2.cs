// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitComponentV2
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  internal class IndexingUnitComponentV2 : IndexingUnitComponent
  {
    public IndexingUnitComponentV2()
    {
    }

    internal IndexingUnitComponentV2(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public override IndexingUnit GetIndexingUnit(
      Guid tfsEntityId,
      string indexingUnitType,
      IEntityType entityType)
    {
      this.PrepareStoredProcedure("Search.prc_QueryIndexingUnitByEntityTypeIndexingUnitTypeAndTFSEntityId");
      this.BindGuid("@TFSEntityId", tfsEntityId);
      this.BindString("@indexingUnitType", indexingUnitType, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@entityType", entityType.Name, 32, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
        ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
        if (current == null || current.Items == null || current.Items.Count <= 0)
          return (IndexingUnit) null;
        if (current.Items.Count == 1)
          return current.Items[0];
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: FormattableString.Invariant(FormattableStringFactory.Create("Failed to retrieve Indexing Unit with TFSEntityId: {0}, IndexingUnitType {1} and EntityType : {2}", (object) tfsEntityId, (object) indexingUnitType, (object) entityType)));
      }
    }

    public override IndexingUnit GetIndexingUnit(int indexingUnitId)
    {
      this.PrepareStoredProcedure("Search.prc_QueryIndexingUnitById");
      this.BindInt("@indexingUnitId", indexingUnitId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
        ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
        if (current == null || current.Items == null || current.Items.Count <= 0)
          return (IndexingUnit) null;
        if (current.Items.Count == 1)
          return current.Items[0];
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: FormattableString.Invariant(FormattableStringFactory.Create("Failed to retrieve Indexing Unit with IndexingUnitId: {0}", (object) indexingUnitId)));
      }
    }

    public override List<IndexingUnit> GetIndexingUnitsRoutingInfo(
      IEntityType entityType,
      List<string> indexingUnitTypes)
    {
      throw new NotImplementedException();
    }

    public override List<IndexingUnit> GetChildIndexingUnitsRoutingInfo(
      string indexingUnitType,
      int parentUnitId)
    {
      throw new NotImplementedException();
    }

    public override IDictionary<int, IndexingUnit> GetIndexingUnits(IEnumerable<int> indexingUnitIds)
    {
      IDictionary<int, IndexingUnit> indexingUnits = (IDictionary<int, IndexingUnit>) new Dictionary<int, IndexingUnit>();
      if (indexingUnitIds == null || !indexingUnitIds.Any<int>())
        return indexingUnits;
      List<int> list = indexingUnitIds.Distinct<int>().ToList<int>();
      int count1 = list.Count;
      int val1 = count1;
      for (int index = 0; index < count1; index += 500)
      {
        int count2 = Math.Min(val1, 500);
        IList<int> range = (IList<int>) list.GetRange(index, count2);
        val1 -= count2;
        if (range.Count > 0)
        {
          this.PrepareStoredProcedure("Search.prc_QueryIndexingUnitsByIds");
          this.BindIndexingUnitIdTable("@indexingUnitIdList", (IEnumerable<int>) range);
          using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
          {
            resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
            ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
            if (current?.Items != null)
            {
              foreach (IndexingUnit indexingUnit in current.Items)
                indexingUnits.Add(indexingUnit.IndexingUnitId, indexingUnit);
            }
          }
        }
        else
          break;
      }
      return indexingUnits;
    }

    public override List<IndexingUnit> GetIndexingUnits(
      string indexingUnitType,
      IEntityType entityType,
      int topCount)
    {
      throw new NotImplementedException();
    }

    public override List<IndexingUnit> GetIndexingUnitsWithGivenParentId(
      int parentIndexingUnitId,
      int topCount)
    {
      throw new NotImplementedException();
    }
  }
}
