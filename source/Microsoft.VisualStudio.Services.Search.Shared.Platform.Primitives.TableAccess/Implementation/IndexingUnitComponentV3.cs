// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitComponentV3
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  internal class IndexingUnitComponentV3 : IndexingUnitComponentV2
  {
    private static readonly SqlMetaData[] s_indexingUnitTypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.VarChar, -1L)
    };

    public IndexingUnitComponentV3()
    {
    }

    internal IndexingUnitComponentV3(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public override List<IndexingUnit> GetDeletedIndexingUnitList(int count)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryDeletedIndexingUnits");
        this.BindInt("@count", count);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
          ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
          return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<IndexingUnit>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to Retrieve Deleted IndexingUnit List with SQL Azure Platform");
      }
    }

    public override List<IndexingUnit> GetIndexingUnitsRoutingInfo(
      IEntityType entityType,
      List<string> indexingUnitTypes)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryIndexingUnitsRoutingInfo");
        this.BindString("@entityType", entityType.Name, 32, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindIndexingUnitTypeTable("@indexingUnitTypeList", (IEnumerable<string>) indexingUnitTypes);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitRoutingColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes));
          ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
          return current?.Items != null ? current.Items : new List<IndexingUnit>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to Retrieve IndexingUnitList of Collection with SQL Azure Platform");
      }
    }

    public override List<IndexingUnit> GetChildIndexingUnitsRoutingInfo(
      string indexingUnitType,
      int parentUnitId)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryChildIndexingUnitsRoutingInfo");
        this.BindString("@indexingUnitType", indexingUnitType, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindInt("@parentUnitId", parentUnitId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitRoutingColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes));
          ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
          return current?.Items != null ? current.Items : new List<IndexingUnit>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to Retrieve Scoped IndexingUnitList of repository with repository Id {0} with SQL Azure Platform", (object) parentUnitId)));
      }
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

    protected SqlParameter BindIndexingUnitTypeTable(string parameterName, IEnumerable<string> rows)
    {
      rows = rows ?? Enumerable.Empty<string>();
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (indexingUnitType =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitComponentV3.s_indexingUnitTypeTable);
        sqlDataRecord.SetValue(0, (object) indexingUnitType);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_StringVarcharTable", rows.Select<string, SqlDataRecord>(selector));
    }
  }
}
