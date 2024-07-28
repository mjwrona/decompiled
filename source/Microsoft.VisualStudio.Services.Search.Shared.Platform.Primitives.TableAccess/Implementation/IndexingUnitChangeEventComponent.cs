// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitChangeEventComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class IndexingUnitChangeEventComponent : SQLTable<IndexingUnitChangeEvent>
  {
    private const string ServiceName = "Search_IndexingUnitChangeEvent";
    protected internal const int BatchCountForQueries = 500;
    protected IEnumerable<IEntityType> m_entityTypes;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[11]
    {
      (IComponentCreator) new ComponentCreator<IndexingUnitChangeEventComponent>(1, true),
      (IComponentCreator) new ComponentCreator<IndexingUnitChangeEventComponentV2>(2, false),
      (IComponentCreator) new ComponentCreator<IndexingUnitChangeEventComponentV3>(3, false),
      (IComponentCreator) new ComponentCreator<IndexingUnitChangeEventComponentV4>(4, false),
      (IComponentCreator) new ComponentCreator<IndexingUnitChangeEventComponentV5>(5, false),
      (IComponentCreator) new ComponentCreator<IndexingUnitChangeEventComponentV6>(6, false),
      (IComponentCreator) new ComponentCreator<IndexingUnitChangeEventComponentV7>(7, false),
      (IComponentCreator) new ComponentCreator<IndexingUnitChangeEventComponentV8>(8, false),
      (IComponentCreator) new ComponentCreator<IndexingUnitChangeEventComponentV9>(9, false),
      (IComponentCreator) new ComponentCreator<IndexingUnitChangeEventComponentV10>(10, false),
      (IComponentCreator) new ComponentCreator<IndexingUnitChangeEventComponentV11>(11, false)
    }, "Search_IndexingUnitChangeEvent");
    protected IEnumerable<Type> m_changeEventDataKnownTypes;
    private static readonly SqlMetaData[] s_indexingUnitChangeEventStateTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.VarChar, -1L)
    };
    private static readonly SqlMetaData[] s_indexingUnitChangeEventChangeTypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.VarChar, -1L)
    };
    private static readonly SqlMetaData[] s_indexingUnitChangeEventJobTriggerTable = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] s_lockDetailsType = new SqlMetaData[3]
    {
      new SqlMetaData("ResourceName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("LockMode", SqlDbType.TinyInt),
      new SqlMetaData("LockOwner", SqlDbType.NVarChar, 128L)
    };

    public IndexingUnitChangeEventComponent()
      : base()
    {
    }

    internal IndexingUnitChangeEventComponent(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId)
    {
      this.m_entityTypes = requestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntityTypes();
      this.m_changeEventDataKnownTypes = requestContext.To(TeamFoundationHostType.Deployment).GetService<DataContractKnownTypesPluginService>().GetKnownTypes(typeof (ChangeEventData));
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.m_entityTypes = requestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntityTypes();
      this.m_changeEventDataKnownTypes = requestContext.To(TeamFoundationHostType.Deployment).GetService<DataContractKnownTypesPluginService>().GetKnownTypes(typeof (ChangeEventData));
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    public virtual IList<IndexingUnitChangeEvent> Update(
      IList<IndexingUnitChangeEvent> indexingUnitChangeEvents)
    {
      throw new NotImplementedException();
    }

    public virtual void DeleteIndexingUnitChangeEventBatch(
      List<IndexingUnitChangeEvent> indexingUnitChangeEventList)
    {
      throw new NotImplementedException();
    }

    public virtual List<IndexingUnitChangeEventDetails> RetrieveIndexingUnitChangeEventDetailsByState(
      int count,
      IndexingUnitChangeEventState state)
    {
      throw new NotImplementedException();
    }

    public virtual List<IndexingUnitChangeEventDetails> RetrieveIndexingUnitChangeEventDetailsByIndexingUnitAndState(
      int count,
      int indexingUnitId,
      IndexingUnitChangeEventState state)
    {
      throw new NotImplementedException();
    }

    public virtual int GetCountOfIndexingUnitChangeEventsInProgressOrQueued(IEntityType entityType) => throw new NotImplementedException();

    public virtual int GetCountOfIndexingUnitChangeEvents() => throw new NotImplementedException();

    public virtual int GetCountOfIndexingUnitChangeEvents(IndexingUnitChangeEventState state) => throw new NotImplementedException();

    public virtual List<IndexingUnitChangeEventDetails> RetrieveIndexingUnitChangeEventDetails(
      int count,
      TableEntityFilterList filterList,
      bool isAdvancedQuery = false)
    {
      throw new NotImplementedException();
    }

    public virtual void ArchiveCompletedIndexingUnitChangeEvents(int periodicCleanupOldEvents) => throw new NotImplementedException();

    public virtual IDictionary<string, TimeSpan> GetMaxTimeForEachChangeType() => throw new NotImplementedException();

    public virtual List<Tuple<IEntityType, string, int>> GetindexingOperationsInProgress(
      List<string> changeTypeList,
      List<int> jobTrigger)
    {
      return new List<Tuple<IEntityType, string, int>>();
    }

    public virtual List<Tuple<IEntityType, int, string, int, ChangeEventData>> RetrieveChangeEventColumnsForJobTriggerAndState(
      List<string> changeTypeList,
      List<int> jobTrigger,
      List<IndexingUnitChangeEventState> stateList)
    {
      throw new NotImplementedException();
    }

    public virtual Dictionary<long, IndexingUnitChangeEventState> RetrieveStateOfIndexingUnitChangeEventsForListOfIds(
      IList<long> ids)
    {
      throw new NotImplementedException();
    }

    internal LockStatus AcquireLockAndMarkEventsAsQueued(
      IList<LockDetails> lockingRequirements,
      IEnumerable<IndexingUnitChangeEvent> changeEvents)
    {
      try
      {
        SqlCommand sqlCommand = this.PrepareStoredProcedure("Search.prc_AcquireLockAndMarkEventsAsQueued");
        this.BindLockDetails("@lockList", (IEnumerable<LockDetails>) lockingRequirements);
        this.BindIndexingUnitChangeEventIdTable("@idList", changeEvents.Select<IndexingUnitChangeEvent, long>((System.Func<IndexingUnitChangeEvent, long>) (x => x.Id)));
        SqlParameter sqlParameter1 = new SqlParameter("@acquired", SqlDbType.Bit);
        sqlParameter1.Direction = ParameterDirection.Output;
        sqlCommand.Parameters.Add(sqlParameter1);
        SqlParameter sqlParameter2 = new SqlParameter("@leaseId", SqlDbType.NVarChar, (int) byte.MaxValue);
        sqlParameter2.Direction = ParameterDirection.Output;
        sqlCommand.Parameters.Add(sqlParameter2);
        this.ExecuteNonQuery(false);
        return new LockStatus((bool) sqlParameter1.Value, sqlParameter2.Value is DBNull ? string.Empty : (string) sqlParameter2.Value);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format("Failed to acquire locks and mark events as {0}", (object) IndexingUnitChangeEventState.Queued));
      }
    }

    public virtual void ReleaseLocksOfCompletedEvents()
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_ReleaseLocksOfCompletedEvents");
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to release locks of completed events.");
      }
    }

    public virtual int DeleteEventsOfDeletedIndexingUnits()
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_DeleteIndexingUnitChangeEventsOfDeletedIndexingUnits");
        return this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to delete events of deleted indexing units.");
      }
    }

    public virtual IndexingUnitChangeEvent AddNewEventIfNotAlreadyPresent(
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      out bool newEventAdded)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      int count,
      int indexingUnitId)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      int count,
      IEntityType entityType)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      int count)
    {
      throw new NotImplementedException();
    }

    public virtual IDictionary<string, int> GetCountOfEventsBreachingThreshold(
      int bulkIndexThreshold,
      int updateIndexThreshold)
    {
      throw new NotImplementedException();
    }

    protected SqlParameter BindIndexingUnitChangeEventIdTable(
      string parameterName,
      IEnumerable<long> rows)
    {
      rows = rows ?? Enumerable.Empty<long>();
      System.Func<long, SqlDataRecord> selector = (System.Func<long, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitChangeEventComponent.s_indexingUnitChangeEventStateTable);
        sqlDataRecord.SetValue(0, (object) entity.ToString());
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_IndexingUnitChangeEventIdDescriptor", rows.Select<long, SqlDataRecord>(selector));
    }

    protected SqlParameter BindIndexingUnitChangeEventStateTable(
      string parameterName,
      IEnumerable<IndexingUnitChangeEventState> rows)
    {
      rows = rows ?? Enumerable.Empty<IndexingUnitChangeEventState>();
      System.Func<IndexingUnitChangeEventState, SqlDataRecord> selector = (System.Func<IndexingUnitChangeEventState, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitChangeEventComponent.s_indexingUnitChangeEventStateTable);
        sqlDataRecord.SetValue(0, (object) entity.ToString());
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_StringVarcharTable", rows.Select<IndexingUnitChangeEventState, SqlDataRecord>(selector));
    }

    protected SqlParameter BindIndexingUnitChangeEventChangeTypeTable(
      string parameterName,
      IEnumerable<string> rows)
    {
      rows = rows ?? Enumerable.Empty<string>();
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitChangeEventComponent.s_indexingUnitChangeEventChangeTypeTable);
        sqlDataRecord.SetValue(0, (object) entity);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_StringVarcharTable", rows.Select<string, SqlDataRecord>(selector));
    }

    protected SqlParameter BindIndexingUnitChangeEventJobTriggerTable(
      string parameterName,
      IEnumerable<byte> rows)
    {
      rows = rows ?? Enumerable.Empty<byte>();
      System.Func<byte, SqlDataRecord> selector = (System.Func<byte, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitChangeEventComponent.s_indexingUnitChangeEventJobTriggerTable);
        sqlDataRecord.SetByte(0, entity);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_TinyIntTable", rows.Select<byte, SqlDataRecord>(selector));
    }

    internal void BindLockDetails(string parameterName, IEnumerable<LockDetails> rows)
    {
      rows = rows ?? Enumerable.Empty<LockDetails>();
      System.Func<LockDetails, SqlDataRecord> selector = (System.Func<LockDetails, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitChangeEventComponent.s_lockDetailsType);
        sqlDataRecord.SetString(0, row.ResourceName);
        sqlDataRecord.SetByte(1, (byte) row.LockMode);
        sqlDataRecord.SetString(2, row.LockOwner);
        return sqlDataRecord;
      });
      this.BindTable(parameterName, "Search.typ_LockDetails", rows.Select<LockDetails, SqlDataRecord>(selector));
    }
  }
}
