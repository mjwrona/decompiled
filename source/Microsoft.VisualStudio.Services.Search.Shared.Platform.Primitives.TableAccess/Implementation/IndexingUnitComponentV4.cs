// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitComponentV4
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
  internal class IndexingUnitComponentV4 : IndexingUnitComponentV3
  {
    public IndexingUnitComponentV4()
    {
    }

    internal IndexingUnitComponentV4(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public override List<IndexingUnit> GetIndexingUnits(
      string indexingUnitType,
      IEntityType entityType,
      int topCount)
    {
      int parameterValue = 0;
      int num = topCount > 500 || topCount <= 0 ? 500 : topCount;
      int val1 = topCount <= 0 ? -1 : topCount;
      bool flag = false;
      List<IndexingUnit> indexingUnits = new List<IndexingUnit>();
      try
      {
        while (!flag)
        {
          this.PrepareStoredProcedure("Search.prc_GetIndexingUnitsEntityTypeAndIndexingUnitType");
          this.BindString("@indexingUnitType", indexingUnitType, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
          this.BindString("@entityType", entityType.Name, 32, BindStringBehavior.Unchanged, SqlDbType.VarChar);
          this.BindInt("@startingId", parameterValue);
          this.BindInt("@count", num);
          using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
          {
            resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
            ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
            if (current?.Items != null && current.Items.Count > 0)
            {
              if (current.Items.Count < num)
                flag = true;
              if (val1 > 0)
              {
                val1 -= num;
                flag = val1 <= 0;
                num = Math.Min(val1, num);
              }
              indexingUnits.AddRange((IEnumerable<IndexingUnit>) current.Items.ToList<IndexingUnit>());
              parameterValue = indexingUnits[indexingUnits.Count - 1].IndexingUnitId;
            }
            else
              flag = true;
          }
        }
        return indexingUnits;
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve Indexing Unit with IndexingUnitType " + indexingUnitType + " and EntityType : " + entityType.Name);
      }
    }

    public override List<IndexingUnit> GetIndexingUnitsWithGivenParentId(
      int parentIndexingUnitId,
      int topCount)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_GetIndexingUnitsWithParentUnitId");
        this.BindInt("@parentUnitId", parentIndexingUnitId);
        this.BindInt("@count", topCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
          ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
          if (current?.Items != null)
          {
            if (current.Items.Count > 0)
              return current.Items;
          }
        }
        return new List<IndexingUnit>();
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to retrieve Indexing Unit with ParentIndexingUnitId {0}", (object) parentIndexingUnitId)));
      }
    }
  }
}
