// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitComponentV7
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityTypes;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  internal class IndexingUnitComponentV7 : IndexingUnitComponentV6
  {
    private static readonly SqlMetaData[] s_entityTypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.VarChar, -1L)
    };

    public IndexingUnitComponentV7()
    {
    }

    internal IndexingUnitComponentV7(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public override IndexingUnit GetIndexingUnit(
      Guid tfsEntityId,
      string indexingUnitType,
      bool isShadow,
      IEntityType entityType)
    {
      return this.GetIndexingUnit(tfsEntityId, IndexingUnit.GetIndexingUnitTypeExtended(indexingUnitType, isShadow), entityType);
    }

    public override List<IndexingUnit> GetIndexingUnits(
      string indexingUnitType,
      bool isShadow,
      IEntityType entityType,
      int topCount)
    {
      return this.GetIndexingUnits(IndexingUnit.GetIndexingUnitTypeExtended(indexingUnitType, isShadow), entityType, topCount);
    }

    public virtual Dictionary<Guid, IndexingUnit> GetAssociatedJobIds(List<IEntityType> entityTypes)
    {
      try
      {
        Dictionary<Guid, IndexingUnit> collection = new Dictionary<Guid, IndexingUnit>();
        if (entityTypes == null || entityTypes.Contains<IEntityType>((IEntityType) AllEntityType.GetInstance(), (IEqualityComparer<IEntityType>) new EntityTypeComparer()))
        {
          this.PrepareStoredProcedure("Search.prc_QueryAssociatedJobIds");
        }
        else
        {
          this.PrepareStoredProcedure("Search.prc_QueryAssociatedJobIdsWithEntityType");
          this.BindEntityTypeTable("@entityType", (IEnumerable<IEntityType>) entityTypes);
        }
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<KeyValuePair<Guid, IndexingUnit>>((ObjectBinder<KeyValuePair<Guid, IndexingUnit>>) new IndexingUnitComponentV7.AssociatedJobIdColumnsV2(this.m_entityTypes));
          ObjectBinder<KeyValuePair<Guid, IndexingUnit>> current = resultCollection.GetCurrent<KeyValuePair<Guid, IndexingUnit>>();
          if (current != null && current.Items != null && current.Items.Count > 0)
            collection.AddRange<KeyValuePair<Guid, IndexingUnit>, Dictionary<Guid, IndexingUnit>>((IEnumerable<KeyValuePair<Guid, IndexingUnit>>) current.Items);
          return collection;
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to get AssociatedJobIds from tbl_IndexingUnits");
      }
    }

    public override void PromoteShadowIndexingUnitsToPrimary(
      List<string> indexingUnitTypes,
      IEntityType entityType)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_SwapShadowsAndPrimaries");
        this.BindIndexingUnitTypeTable("@indexingUnitTypeList", (IEnumerable<string>) indexingUnitTypes);
        this.BindString("@entityType", entityType.Name, 32, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to swap Shadow and Primary Indexing Units.");
      }
    }

    protected SqlParameter BindEntityTypeTable(string parameterName, IEnumerable<IEntityType> rows)
    {
      rows = rows ?? (IEnumerable<IEntityType>) Enumerable.Empty<EntityType>();
      System.Func<IEntityType, SqlDataRecord> selector = (System.Func<IEntityType, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitComponentV7.s_entityTypeTable);
        sqlDataRecord.SetValue(0, (object) entity.Name.ToString());
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_StringVarcharTable", rows.Select<IEntityType, SqlDataRecord>(selector));
    }

    internal class AssociatedJobIdColumnsV2 : ObjectBinder<KeyValuePair<Guid, IndexingUnit>>
    {
      private SqlColumnBinder m_tfsEntityId = new SqlColumnBinder("TFSEntityId");
      private SqlColumnBinder m_associatedJobId = new SqlColumnBinder("AssociatedJobId");
      private SqlColumnBinder m_indexingUnitType = new SqlColumnBinder("IndexingUnitType");
      private SqlColumnBinder m_indexingUnitId = new SqlColumnBinder("IndexingUnitId");
      private SqlColumnBinder m_parentUnitId = new SqlColumnBinder("ParentUnitId");
      private SqlColumnBinder m_entityType = new SqlColumnBinder("EntityType");
      private IEnumerable<IEntityType> m_entityTypes;

      protected override KeyValuePair<Guid, IndexingUnit> Bind()
      {
        IndexingUnit indexingUnitObject = IndexingUnitComponent.CreateIndexingUnitObject(this.m_tfsEntityId.GetGuid((IDataReader) this.Reader), this.m_indexingUnitType.GetString((IDataReader) this.Reader, false), EntityPluginsFactory.GetEntityType(this.m_entityTypes, this.m_entityType.GetString((IDataReader) this.Reader, false)), this.m_parentUnitId.GetInt32((IDataReader) this.Reader, -1));
        indexingUnitObject.IndexingUnitId = this.m_indexingUnitId.GetInt32((IDataReader) this.Reader);
        indexingUnitObject.AssociatedJobId = new Guid?(this.m_associatedJobId.GetGuid((IDataReader) this.Reader));
        return new KeyValuePair<Guid, IndexingUnit>(this.m_associatedJobId.GetGuid((IDataReader) this.Reader), indexingUnitObject);
      }

      public AssociatedJobIdColumnsV2(IEnumerable<IEntityType> m_entityTypes) => this.m_entityTypes = m_entityTypes;
    }
  }
}
