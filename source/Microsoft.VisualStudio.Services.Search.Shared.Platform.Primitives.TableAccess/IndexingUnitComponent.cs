// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.IndexingUnitComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class IndexingUnitComponent : SQLTable<IndexingUnit>
  {
    private const string ServiceName = "Search_IndexingUnit";
    protected internal const int BatchCountForQueries = 500;
    protected IEnumerable<Type> m_indexingPropertiesKnownTypes;
    protected IEnumerable<Type> m_tfsEntityAttributesKnownTypes;
    protected IEnumerable<IEntityType> m_entityTypes;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[8]
    {
      (IComponentCreator) new ComponentCreator<IndexingUnitComponent>(1, true),
      (IComponentCreator) new ComponentCreator<IndexingUnitComponentV2>(2),
      (IComponentCreator) new ComponentCreator<IndexingUnitComponentV3>(3),
      (IComponentCreator) new ComponentCreator<IndexingUnitComponentV4>(4),
      (IComponentCreator) new ComponentCreator<IndexingUnitComponentV5>(5),
      (IComponentCreator) new ComponentCreator<IndexingUnitComponentV6>(6),
      (IComponentCreator) new ComponentCreator<IndexingUnitComponentV7>(7),
      (IComponentCreator) new ComponentCreator<IndexingUnitComponentV8>(8)
    }, "Search_IndexingUnit");
    private static readonly SqlMetaData[] s_indexingUnitSqlTableEntityLookupTable = new SqlMetaData[7]
    {
      new SqlMetaData("TFSEntityId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IndexingUnitType", SqlDbType.VarChar, 64L),
      new SqlMetaData("EntityType", SqlDbType.VarChar, 32L),
      new SqlMetaData("TFSEntityAttributes", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ParentUnitID", SqlDbType.Int),
      new SqlMetaData("Properties", SqlDbType.NVarChar, -1L),
      new SqlMetaData("AssociatedJobId", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] s_indexingUnitIdTable = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.Int)
    };

    public IndexingUnitComponent()
      : base()
    {
    }

    internal IndexingUnitComponent(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      DataContractKnownTypesPluginService service = context.GetService<DataContractKnownTypesPluginService>();
      this.m_indexingPropertiesKnownTypes = service.GetKnownTypes(typeof (IndexingProperties));
      this.m_tfsEntityAttributesKnownTypes = service.GetKnownTypes(typeof (TFSEntityAttributes));
      this.m_entityTypes = context.GetService<IEntityService>().GetEntityTypes();
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      DataContractKnownTypesPluginService service = context.GetService<DataContractKnownTypesPluginService>();
      this.m_indexingPropertiesKnownTypes = service.GetKnownTypes(typeof (IndexingProperties));
      this.m_tfsEntityAttributesKnownTypes = service.GetKnownTypes(typeof (TFSEntityAttributes));
      this.m_entityTypes = context.GetService<IEntityService>().GetEntityTypes();
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    internal static IndexingUnit CreateIndexingUnitObject(
      Guid tfsEntityId,
      string indexingUnitType_Extended,
      IEntityType entityType,
      int parentUnitId)
    {
      bool isShadow;
      return new IndexingUnit(tfsEntityId, IndexingUnit.ParseIndexingUnitTypeExtended(indexingUnitType_Extended, out isShadow), entityType, parentUnitId, isShadow);
    }

    public override IndexingUnit Insert(IndexingUnit indexingUnit)
    {
      this.ValidateNotNull<IndexingUnit>(nameof (indexingUnit), indexingUnit);
      try
      {
        this.PrepareStoredProcedure("Search.prc_AddIndexingUnits");
        IList<IndexingUnit> rows = (IList<IndexingUnit>) new List<IndexingUnit>();
        rows.Add(indexingUnit);
        this.BindIndexingUnitLookupTable("@itemList", (IEnumerable<IndexingUnit>) rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
          ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
          if (current?.Items != null && current.Items.Count == 1)
            return resultCollection.GetCurrent<IndexingUnit>().Items[0];
          throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to Add indexingUnit with TFSEntityID {0} and Type {1} with SQL Azure platform", (object) indexingUnit.TFSEntityId, (object) indexingUnit.IndexingUnitType));
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to Add indexingUnit with TFSEntityID {0} and Type {1} with SQL Azure platform", (object) indexingUnit.TFSEntityId, (object) indexingUnit.IndexingUnitType));
      }
    }

    public override void Delete(IndexingUnit indexingUnit)
    {
      this.ValidateNotNull<IndexingUnit>(nameof (indexingUnit), indexingUnit);
      try
      {
        this.PrepareStoredProcedure("Search.prc_DeleteIndexingUnit");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to Delete indexingUnit with ID {0}, TFSEntityID {1} and Type {2} with SQL Azure platform", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.TFSEntityId, (object) indexingUnit.IndexingUnitType));
      }
    }

    public override IndexingUnit Update(IndexingUnit indexingUnit)
    {
      this.ValidateNotNull<IndexingUnit>(nameof (indexingUnit), indexingUnit);
      try
      {
        this.PrepareStoredProcedure("Search.prc_UpdateIndexingUnit");
        this.BindInt("@IndexingUnitId", indexingUnit.IndexingUnitId);
        this.BindString("@tfsEntityAttributes", SQLTable<IndexingUnit>.ToString((object) indexingUnit.TFSEntityAttributes, typeof (TFSEntityAttributes), this.m_tfsEntityAttributesKnownTypes), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@properties", SQLTable<IndexingUnit>.ToString((object) indexingUnit.Properties, typeof (IndexingProperties), this.m_indexingPropertiesKnownTypes), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        Guid? associatedJobId = indexingUnit.AssociatedJobId;
        if (associatedJobId.HasValue)
        {
          associatedJobId = indexingUnit.AssociatedJobId;
          this.BindGuid("@associatedJobId", associatedJobId.Value);
        }
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
          ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
          if (current?.Items != null && current.Items.Count == 1)
            return resultCollection.GetCurrent<IndexingUnit>().Items[0];
          throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to update Indexing Unit with ID {0}, TFSEntityId {1}  and Type {2} with SQL Azure platform", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.TFSEntityId, (object) indexingUnit.IndexingUnitType));
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to update Indexing Unit with ID {0}, TFSEntityId {1}  and Type {2} with SQL Azure platform", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.TFSEntityId, (object) indexingUnit.IndexingUnitType));
      }
    }

    public List<IndexingUnit> UpdateIndexingUnitsBatch(List<IndexingUnit> indexingUnitList)
    {
      this.ValidateNotNullOrEmptyList<IndexingUnit>(nameof (indexingUnitList), (IList<IndexingUnit>) indexingUnitList);
      try
      {
        this.PrepareStoredProcedure("Search.prc_UpdateIndexingUnits");
        this.BindIndexingUnitLookupTable("@itemList", (IEnumerable<IndexingUnit>) indexingUnitList);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
          return resultCollection.GetCurrent<IndexingUnit>().Items;
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to execute UpdateIndexingUnitsBatch operation with SQL Azure Platform");
      }
    }

    public virtual IndexingUnit DisassociateJobId(IndexingUnit indexingUnit)
    {
      this.ValidateNotNull<IndexingUnit>(nameof (indexingUnit), indexingUnit);
      try
      {
        this.PrepareStoredProcedure("Search.prc_DisassociateJobId");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
          ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
          if (current?.Items != null && current.Items.Count == 1)
            return resultCollection.GetCurrent<IndexingUnit>().Items[0];
          throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to disassociate JobId from Indexing Unit with ID {0}, TFSEntityId {1}  and Type {2} with SQL Azure platform", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.TFSEntityId, (object) indexingUnit.IndexingUnitType));
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to disassociate Indexing Unit with ID {0}, TFSEntityId {1}  and Type {2} with SQL Azure platform", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.TFSEntityId, (object) indexingUnit.IndexingUnitType));
      }
    }

    public override IndexingUnit RetriveTableEntity(TableEntityFilterList filterList)
    {
      this.ValidateNotNull<TableEntityFilterList>(nameof (filterList), filterList);
      this.PrepareStoredProcedure("Search.prc_QueryIndexingUnits");
      TableEntityFilter propertyFilter;
      if (filterList.TryRetrieveFilter("IndexingUnitId", out propertyFilter))
        this.BindInt("@indexingUnitId", Convert.ToInt32(propertyFilter.Value));
      if (filterList.TryRetrieveFilter("TFSEntityId", out propertyFilter))
        this.BindGuid("@TFSEntityId", new Guid(propertyFilter.Value));
      if (filterList.TryRetrieveFilter("IndexingUnitType", out propertyFilter))
        this.BindString("@indexingUnitType", propertyFilter.Value, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      if (filterList.TryRetrieveFilter("EntityType", out propertyFilter))
        this.BindString("@entityType", propertyFilter.Value, 32, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      try
      {
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
          ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
          if (current?.Items == null || current.Items.Count <= 0)
            return (IndexingUnit) null;
          if (current.Items.Count == 1)
            return current.Items[0];
          throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: "More than one matching Indexing Units found for the input filter list");
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve Indexing Unit with SQL Azure Platform");
      }
    }

    public override List<IndexingUnit> AddTableEntityBatch(
      List<IndexingUnit> indexingUnitList,
      bool merge)
    {
      this.ValidateNotNullOrEmptyList<IndexingUnit>(nameof (indexingUnitList), (IList<IndexingUnit>) indexingUnitList);
      try
      {
        if (merge)
          this.PrepareStoredProcedure("Search.prc_AddOrUpdateIndexingUnits");
        else
          this.PrepareStoredProcedure("Search.prc_AddIndexingUnits");
        this.BindIndexingUnitLookupTable("@itemList", (IEnumerable<IndexingUnit>) indexingUnitList);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
          return resultCollection.GetCurrent<IndexingUnit>().Items;
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to execute AddTableEntityBatch operation with SQL Azure Platform");
      }
    }

    public virtual void DeleteIndexingUnitBatchPermanently(List<IndexingUnit> indexingUnitList)
    {
      this.ValidateNotNullOrEmptyList<IndexingUnit>(nameof (indexingUnitList), (IList<IndexingUnit>) indexingUnitList);
      List<int> list = indexingUnitList.Select<IndexingUnit, int>((System.Func<IndexingUnit, int>) (item => item.IndexingUnitId)).ToList<int>();
      try
      {
        this.PrepareStoredProcedure("Search.prc_DeleteIndexingUnitBatchPermanently");
        this.BindIndexingUnitIdTable("@idList", (IEnumerable<int>) list);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to execute DeleteIndexingUnitBatchPermanently operation with SQL Azure Platform");
      }
    }

    public override List<IndexingUnit> RetriveTableEntityList(
      int count,
      TableEntityFilterList filterList)
    {
      this.ValidateNotNull<TableEntityFilterList>(nameof (filterList), filterList);
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryIndexingUnits");
        this.BindInt("@count", count);
        TableEntityFilter propertyFilter;
        if (filterList.TryRetrieveFilter("ParentUnitID", out propertyFilter))
          this.BindInt("@parentUnitID", Convert.ToInt32(propertyFilter.Value));
        if (filterList.TryRetrieveFilter("IndexingUnitType", out propertyFilter))
          this.BindString("@indexingUnitType", propertyFilter.Value, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        if (filterList.TryRetrieveFilter("EntityType", out propertyFilter))
          this.BindString("@entityType", propertyFilter.Value, 32, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnit>((ObjectBinder<IndexingUnit>) new IndexingUnitComponent.IndexingUnitColumns(this.m_indexingPropertiesKnownTypes, this.m_tfsEntityAttributesKnownTypes, this.m_entityTypes));
          ObjectBinder<IndexingUnit> current = resultCollection.GetCurrent<IndexingUnit>();
          return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<IndexingUnit>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve IndexingUnit List with SQL Azure Platform");
      }
    }

    public virtual List<IndexingUnit> GetDeletedIndexingUnitList(int count)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryDeletedIndexingUnits");
        this.BindInt("@count", count);
        this.BindString("@entityType", "Code".ToString(), 32, BindStringBehavior.Unchanged, SqlDbType.VarChar);
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

    public virtual IndexingUnit GetIndexingUnit(
      Guid tfsEntityId,
      string indexingUnitType,
      IEntityType entityType)
    {
      throw new NotImplementedException();
    }

    public virtual List<IndexingUnit> GetIndexingUnitsWithGivenParentId(
      int parentIndexingUnitId,
      int topCount)
    {
      throw new NotImplementedException();
    }

    public virtual IndexingUnit GetIndexingUnit(int indexingUnitId) => throw new NotImplementedException();

    public virtual IDictionary<int, IndexingUnit> GetIndexingUnits(IEnumerable<int> indexingUnitIds) => throw new NotImplementedException();

    public virtual List<IndexingUnit> GetIndexingUnits(
      string indexingUnitType,
      IEntityType entityType,
      int topCount)
    {
      throw new NotImplementedException();
    }

    public virtual List<IndexingUnit> GetIndexingUnitsRoutingInfo(
      IEntityType entityType,
      List<string> indexingUnitTypes)
    {
      throw new NotImplementedException();
    }

    public virtual void PromoteShadowIndexingUnitsToPrimary(
      List<string> indexingUnitTypes,
      IEntityType entityType)
    {
      throw new NotImplementedException();
    }

    public virtual IndexingUnit GetIndexingUnit(
      Guid tfsEntityId,
      string indexingUnitType,
      bool isShadow,
      IEntityType entityType)
    {
      throw new NotImplementedException();
    }

    public virtual List<IndexingUnit> GetIndexingUnits(
      string indexingUnitType,
      bool isShadow,
      IEntityType entityType,
      int topCount)
    {
      throw new NotImplementedException();
    }

    public virtual List<IndexingUnit> GetChildIndexingUnitsRoutingInfo(
      string indexingUnitType,
      int parentUnitId)
    {
      throw new NotImplementedException();
    }

    public virtual List<IndexingUnit> GetOrphanIndexingUnitsOfTypeGitOrTfvcRepository() => new List<IndexingUnit>();

    public virtual void SoftDeleteIndexingUnitBatch(IEnumerable<int> indexingUnitIdList) => throw new NotImplementedException();

    private SqlParameter BindIndexingUnitLookupTable(
      string parameterName,
      IEnumerable<IndexingUnit> rows)
    {
      rows = rows ?? Enumerable.Empty<IndexingUnit>();
      System.Func<IndexingUnit, SqlDataRecord> selector = (System.Func<IndexingUnit, SqlDataRecord>) (indexingUnit =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitComponent.s_indexingUnitSqlTableEntityLookupTable);
        sqlDataRecord.SetGuid(0, indexingUnit.TFSEntityId);
        sqlDataRecord.SetString(1, indexingUnit.IndexingUnitType_Extended);
        sqlDataRecord.SetString(2, indexingUnit.EntityType.Name);
        sqlDataRecord.SetString(3, SQLTable<IndexingUnit>.ToString((object) indexingUnit.TFSEntityAttributes, typeof (TFSEntityAttributes), this.m_tfsEntityAttributesKnownTypes));
        sqlDataRecord.SetInt32(4, indexingUnit.ParentUnitId);
        sqlDataRecord.SetString(5, SQLTable<IndexingUnit>.ToString((object) indexingUnit.Properties, typeof (IndexingProperties), this.m_indexingPropertiesKnownTypes));
        if (indexingUnit.AssociatedJobId.HasValue)
          sqlDataRecord.SetGuid(6, indexingUnit.AssociatedJobId.Value);
        else
          sqlDataRecord.SetDBNull(6);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_IndexingUnitDescriptor", rows.Select<IndexingUnit, SqlDataRecord>(selector));
    }

    protected SqlParameter BindIndexingUnitIdTable(string parameterName, IEnumerable<int> rows)
    {
      rows = rows ?? Enumerable.Empty<int>();
      System.Func<int, SqlDataRecord> selector = (System.Func<int, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitComponent.s_indexingUnitIdTable);
        sqlDataRecord.SetInt32(0, entity);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_Int32Table", rows.Select<int, SqlDataRecord>(selector));
    }

    internal class IndexingUnitColumns : ObjectBinder<IndexingUnit>
    {
      private SqlColumnBinder m_tfsEntityId = new SqlColumnBinder("TFSEntityId");
      private SqlColumnBinder m_indexingUnitType = new SqlColumnBinder("IndexingUnitType");
      private SqlColumnBinder m_entityType = new SqlColumnBinder("EntityType");
      private SqlColumnBinder m_indexingUnitId = new SqlColumnBinder("IndexingUnitId");
      private SqlColumnBinder m_tfsEntityAttributes = new SqlColumnBinder("TFSEntityAttributes");
      private SqlColumnBinder m_parentUnitId = new SqlColumnBinder("ParentUnitID");
      private SqlColumnBinder m_properties = new SqlColumnBinder("Properties");
      private SqlColumnBinder m_associatedJobId = new SqlColumnBinder("AssociatedJobId");
      private SqlColumnBinder m_createdTimeUtc = new SqlColumnBinder("CreatedTimeUTC");
      private IEnumerable<Type> m_indexingPropertiesKnownTypes;
      private IEnumerable<Type> m_tfsEntityAttributesKnownTypes;
      private IEnumerable<IEntityType> m_entityTypes;

      public IndexingUnitColumns(
        IEnumerable<Type> indexingPropertiesKnownTypes,
        IEnumerable<Type> tfsEntityAttributesKnownTypes,
        IEnumerable<IEntityType> entityTypes)
      {
        this.m_indexingPropertiesKnownTypes = indexingPropertiesKnownTypes;
        this.m_tfsEntityAttributesKnownTypes = tfsEntityAttributesKnownTypes;
        this.m_entityTypes = entityTypes;
      }

      protected override IndexingUnit Bind()
      {
        if (this.m_indexingUnitId.IsNull((IDataReader) this.Reader))
          return (IndexingUnit) null;
        IndexingUnit indexingUnitObject = IndexingUnitComponent.CreateIndexingUnitObject(this.m_tfsEntityId.GetGuid((IDataReader) this.Reader), this.m_indexingUnitType.GetString((IDataReader) this.Reader, false), EntityPluginsFactory.GetEntityType(this.m_entityTypes, this.m_entityType.GetString((IDataReader) this.Reader, false)), this.m_parentUnitId.GetInt32((IDataReader) this.Reader, -1));
        indexingUnitObject.IndexingUnitId = this.m_indexingUnitId.GetInt32((IDataReader) this.Reader);
        indexingUnitObject.TFSEntityAttributes = (TFSEntityAttributes) SQLTable<IndexingUnit>.FromString(this.m_tfsEntityAttributes.GetString((IDataReader) this.Reader, false), typeof (TFSEntityAttributes), this.m_tfsEntityAttributesKnownTypes);
        indexingUnitObject.Properties = (IndexingProperties) SQLTable<IndexingUnit>.FromString(this.m_properties.GetString((IDataReader) this.Reader, false), typeof (IndexingProperties), this.m_indexingPropertiesKnownTypes);
        indexingUnitObject.CreatedTimeUTC = this.m_createdTimeUtc.GetDateTime((IDataReader) this.Reader);
        Guid guid = this.m_associatedJobId.GetGuid((IDataReader) this.Reader, true);
        indexingUnitObject.AssociatedJobId = guid.Equals(Guid.Empty) ? new Guid?() : new Guid?(guid);
        if (indexingUnitObject.IndexingUnitType == "Collection" && indexingUnitObject.EntityType.Name == "Code" && ((CollectionIndexingProperties) indexingUnitObject.Properties).IndexContractType == DocumentContractType.Unsupported)
        {
          ((CollectionIndexingProperties) indexingUnitObject.Properties).IndexContractType = DocumentContractType.SourceNoDedupeFileContractV3;
          ((CollectionIndexingProperties) indexingUnitObject.Properties).QueryContractType = DocumentContractType.SourceNoDedupeFileContractV3;
        }
        indexingUnitObject.Properties?.Initialize();
        return indexingUnitObject;
      }
    }

    internal class IndexingUnitRoutingColumns : ObjectBinder<IndexingUnit>
    {
      private SqlColumnBinder m_tfsEntityId = new SqlColumnBinder("TFSEntityId");
      private SqlColumnBinder m_indexingUnitType = new SqlColumnBinder("IndexingUnitType");
      private SqlColumnBinder m_tfsEntityAttributes = new SqlColumnBinder("TFSEntityAttributes");
      private SqlColumnBinder m_indexingUnitId = new SqlColumnBinder("IndexingUnitId");
      private SqlColumnBinder m_parentUnitId = new SqlColumnBinder("ParentUnitID");
      private SqlColumnBinder m_properties = new SqlColumnBinder("Properties");
      private IEnumerable<Type> m_indexingPropertiesKnownTypes;
      private IEnumerable<Type> m_tfsEntityAttributesKnownTypes;

      public IndexingUnitRoutingColumns(
        IEnumerable<Type> indexingPropertiesKnownTypes,
        IEnumerable<Type> tfsEntityAttributesKnownTypes)
      {
        this.m_indexingPropertiesKnownTypes = indexingPropertiesKnownTypes;
        this.m_tfsEntityAttributesKnownTypes = tfsEntityAttributesKnownTypes;
      }

      protected override IndexingUnit Bind()
      {
        if (this.m_indexingUnitId.IsNull((IDataReader) this.Reader))
          return (IndexingUnit) null;
        IndexingUnit indexingUnitObject = IndexingUnitComponent.CreateIndexingUnitObject(this.m_tfsEntityId.GetGuid((IDataReader) this.Reader), this.m_indexingUnitType.GetString((IDataReader) this.Reader, false), (IEntityType) CodeEntityType.GetInstance(), this.m_parentUnitId.GetInt32((IDataReader) this.Reader, -1));
        indexingUnitObject.IndexingUnitId = this.m_indexingUnitId.GetInt32((IDataReader) this.Reader);
        indexingUnitObject.TFSEntityAttributes = (TFSEntityAttributes) SQLTable<IndexingUnit>.FromString(this.m_tfsEntityAttributes.GetString((IDataReader) this.Reader, false), typeof (TFSEntityAttributes), this.m_tfsEntityAttributesKnownTypes);
        indexingUnitObject.Properties = (IndexingProperties) SQLTable<IndexingUnit>.FromString(this.m_properties.GetString((IDataReader) this.Reader, false), typeof (IndexingProperties), this.m_indexingPropertiesKnownTypes);
        if (indexingUnitObject.IndexingUnitType == "Collection" && indexingUnitObject.EntityType.Name == "Code" && ((CollectionIndexingProperties) indexingUnitObject.Properties).IndexContractType == DocumentContractType.Unsupported)
        {
          ((CollectionIndexingProperties) indexingUnitObject.Properties).IndexContractType = DocumentContractType.SourceNoDedupeFileContractV3;
          ((CollectionIndexingProperties) indexingUnitObject.Properties).QueryContractType = DocumentContractType.SourceNoDedupeFileContractV3;
        }
        indexingUnitObject.Properties?.Initialize();
        return indexingUnitObject;
      }
    }
  }
}
