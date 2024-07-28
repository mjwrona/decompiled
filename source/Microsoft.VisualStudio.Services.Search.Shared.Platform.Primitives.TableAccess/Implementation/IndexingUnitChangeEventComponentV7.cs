// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitChangeEventComponentV7
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class IndexingUnitChangeEventComponentV7 : IndexingUnitChangeEventComponentV6
  {
    private static readonly SqlMetaData[] s_indexingUnitChangeEventLookupTable = new SqlMetaData[9]
    {
      new SqlMetaData("IndexingUnitId", SqlDbType.Int),
      new SqlMetaData("ChangeType", SqlDbType.VarChar, 100L),
      new SqlMetaData("ChangeData", SqlDbType.NVarChar, -1L),
      new SqlMetaData("CurrentStage", SqlDbType.VarChar, 100L),
      new SqlMetaData("State", SqlDbType.VarChar, 24L),
      new SqlMetaData("AttemptCount", SqlDbType.TinyInt),
      new SqlMetaData("Prerequisites", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LeaseId", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("JobTrigger", SqlDbType.TinyInt)
    };

    public IndexingUnitChangeEventComponentV7()
    {
    }

    internal IndexingUnitChangeEventComponentV7(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public override List<Tuple<IEntityType, string, int>> GetindexingOperationsInProgress(
      List<string> changeTypeList,
      List<int> jobTrigger)
    {
      this.PrepareStoredProcedure("Search.prc_GetIndexingStatus");
      List<byte> list = ((IEnumerable<byte>) Array.ConvertAll<int, byte>(jobTrigger.ToArray(), (Converter<int, byte>) (value => (byte) value))).ToList<byte>();
      this.BindIndexingUnitChangeEventChangeTypeTable("@changeType", (IEnumerable<string>) changeTypeList);
      this.BindIndexingUnitChangeEventJobTriggerTable("@jobTrigger", (IEnumerable<byte>) list);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Tuple<IEntityType, string, int>>((ObjectBinder<Tuple<IEntityType, string, int>>) new IndexingUnitChangeEventComponentV7.IndexingStatusColumns(this.m_entityTypes));
        ObjectBinder<Tuple<IEntityType, string, int>> current = resultCollection.GetCurrent<Tuple<IEntityType, string, int>>();
        return current != null && current.Items != null && current.Items.Count > 0 ? current.Items : new List<Tuple<IEntityType, string, int>>();
      }
    }

    public override IndexingUnitChangeEvent Insert(IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      if (indexingUnitChangeEvent == null)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (indexingUnitChangeEvent)));
      try
      {
        this.PrepareStoredProcedure("Search.prc_AddEntryForIndexingUnitChangeEvent");
        IList<IndexingUnitChangeEvent> rows = (IList<IndexingUnitChangeEvent>) new List<IndexingUnitChangeEvent>();
        rows.Add(indexingUnitChangeEvent);
        this.BindIndexingUnitChangeEventLookupTable("@itemList", (IEnumerable<IndexingUnitChangeEvent>) rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnitChangeEventDetails>((ObjectBinder<IndexingUnitChangeEventDetails>) new IndexingUnitChangeEventComponentV2.IndexingUnitChangeEventColumnsV2(this.m_changeEventDataKnownTypes));
          ObjectBinder<IndexingUnitChangeEventDetails> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetails>();
          if (current != null && current.Items != null && current.Items.Count == 1)
            return current.Items.First<IndexingUnitChangeEventDetails>().IndexingUnitChangeEvent;
          throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to add IndexingUnitChangeEvent {0}  with SQL Azure platform", (object) indexingUnitChangeEvent.ToString()));
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to add IndexingUnitChangeEvent {0}  with SQL Azure platform", (object) indexingUnitChangeEvent.ToString()));
      }
    }

    public override List<IndexingUnitChangeEvent> AddTableEntityBatch(
      List<IndexingUnitChangeEvent> indexingUnitChangeEvents,
      bool merge)
    {
      if (indexingUnitChangeEvents != null)
      {
        if (indexingUnitChangeEvents.Count != 0)
        {
          try
          {
            if (merge)
              throw new InvalidOperationException("Merge is not supported");
            int count1 = indexingUnitChangeEvents.Count;
            List<IndexingUnitChangeEvent> indexingUnitChangeEventList = new List<IndexingUnitChangeEvent>();
            int val1 = count1;
            for (int index = 0; index < count1; index += 500)
            {
              int count2 = Math.Min(val1, 500);
              IList<IndexingUnitChangeEvent> range = (IList<IndexingUnitChangeEvent>) indexingUnitChangeEvents.GetRange(index, count2);
              val1 -= count2;
              if (range.Count > 0)
              {
                this.PrepareStoredProcedure("Search.prc_AddEntryForIndexingUnitChangeEvent");
                this.BindIndexingUnitChangeEventLookupTable("@itemList", (IEnumerable<IndexingUnitChangeEvent>) range);
                using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
                {
                  resultCollection.AddBinder<IndexingUnitChangeEventDetails>((ObjectBinder<IndexingUnitChangeEventDetails>) new IndexingUnitChangeEventComponentV2.IndexingUnitChangeEventColumnsV2(this.m_changeEventDataKnownTypes));
                  ObjectBinder<IndexingUnitChangeEventDetails> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetails>();
                  if (current?.Items == null || current.Items.Count <= 0)
                    throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to execute AddBatch operation with SQL Azure Platform"));
                  indexingUnitChangeEventList.AddRange(current.Items.Select<IndexingUnitChangeEventDetails, IndexingUnitChangeEvent>((System.Func<IndexingUnitChangeEventDetails, IndexingUnitChangeEvent>) (x => x.IndexingUnitChangeEvent)));
                }
              }
              else
                break;
            }
            return indexingUnitChangeEventList;
          }
          catch (Exception ex)
          {
            throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to execute AddBatch operation with SQL Azure Platform");
          }
        }
      }
      throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("indexingUnitChangeEvents is null or empty"));
    }

    public override IndexingUnitChangeEvent AddNewEventIfNotAlreadyPresent(
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      out bool newEventAdded)
    {
      if (indexingUnitChangeEvent == null)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (indexingUnitChangeEvent)));
      try
      {
        this.PrepareStoredProcedure("Search.prc_AddNewEventIfNotAlreadyPresent");
        this.BindInt("@indexingUnitId", indexingUnitChangeEvent.IndexingUnitId);
        this.BindString("@changeType", indexingUnitChangeEvent.ChangeType, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@changeData", SQLTable<IndexingUnitChangeEvent>.ToString((object) indexingUnitChangeEvent.ChangeData, typeof (ChangeEventData), this.m_changeEventDataKnownTypes), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@currentStage", indexingUnitChangeEvent.State.ToString(), 24, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@state", indexingUnitChangeEvent.State.ToString(), 24, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindByte("@attemptCount", indexingUnitChangeEvent.AttemptCount);
        this.BindString("@prerequisites", SQLTable<IndexingUnitChangeEvent>.ToString((object) indexingUnitChangeEvent.Prerequisites, typeof (IndexingUnitChangeEventPrerequisites)), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@leaseId", indexingUnitChangeEvent.LeaseId, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindByte("@jobTrigger", (byte) indexingUnitChangeEvent.ChangeData.Trigger);
        newEventAdded = false;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Tuple<IndexingUnitChangeEvent, bool>>((ObjectBinder<Tuple<IndexingUnitChangeEvent, bool>>) new IndexingUnitChangeEventComponentV7.IndexingUnitChangeEventWithEventAddedColumns2(this.m_changeEventDataKnownTypes));
          ObjectBinder<Tuple<IndexingUnitChangeEvent, bool>> current = resultCollection.GetCurrent<Tuple<IndexingUnitChangeEvent, bool>>();
          if (current == null || current.Items == null || current.Items.Count != 1)
            throw new Exception("Failed to add IndexingUnitChangeEvent " + indexingUnitChangeEvent.ToString() + "  with SQL Azure platform");
          Tuple<IndexingUnitChangeEvent, bool> tuple = current.Items.First<Tuple<IndexingUnitChangeEvent, bool>>();
          newEventAdded = tuple.Item2;
          return tuple.Item1;
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to add IndexingUnitChangeEvent " + indexingUnitChangeEvent.ToString() + "  with SQL Azure platform");
      }
    }

    protected override SqlParameter BindIndexingUnitChangeEventLookupTable(
      string parameterName,
      IEnumerable<IndexingUnitChangeEvent> rows)
    {
      rows = rows ?? Enumerable.Empty<IndexingUnitChangeEvent>();
      System.Func<IndexingUnitChangeEvent, SqlDataRecord> selector = (System.Func<IndexingUnitChangeEvent, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitChangeEventComponentV7.s_indexingUnitChangeEventLookupTable);
        sqlDataRecord.SetInt32(0, entity.IndexingUnitId);
        sqlDataRecord.SetString(1, entity.ChangeType);
        sqlDataRecord.SetString(2, SQLTable<IndexingUnitChangeEvent>.ToString((object) entity.ChangeData, typeof (ChangeEventData), this.m_changeEventDataKnownTypes));
        if (!string.IsNullOrWhiteSpace(entity.CurrentStage))
          sqlDataRecord.SetString(3, entity.CurrentStage);
        else
          sqlDataRecord.SetDBNull(3);
        sqlDataRecord.SetString(4, entity.State.ToString());
        sqlDataRecord.SetByte(5, entity.AttemptCount);
        sqlDataRecord.SetString(6, SQLTable<IndexingUnitChangeEvent>.ToString((object) entity.Prerequisites, typeof (IndexingUnitChangeEventPrerequisites)));
        if (!string.IsNullOrWhiteSpace(entity.LeaseId))
          sqlDataRecord.SetString(7, entity.LeaseId);
        else
          sqlDataRecord.SetDBNull(7);
        sqlDataRecord.SetByte(8, (byte) entity.ChangeData.Trigger);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_IndexingUnitChangeEventDescriptorV3", rows.Select<IndexingUnitChangeEvent, SqlDataRecord>(selector));
    }

    protected class IndexingUnitChangeEventWithEventAddedColumns2 : 
      ObjectBinder<Tuple<IndexingUnitChangeEvent, bool>>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_indexingUnitId = new SqlColumnBinder("IndexingUnitId");
      private SqlColumnBinder m_changeType = new SqlColumnBinder("ChangeType");
      private SqlColumnBinder m_changeData = new SqlColumnBinder("ChangeData");
      private SqlColumnBinder m_currentStage = new SqlColumnBinder("CurrentStage");
      private SqlColumnBinder m_state = new SqlColumnBinder("State");
      private SqlColumnBinder m_attemptCount = new SqlColumnBinder("AttemptCount");
      private SqlColumnBinder m_prerequisites = new SqlColumnBinder("Prerequisites");
      private SqlColumnBinder m_createdTimeUtc = new SqlColumnBinder("CreatedTimeUTC");
      private SqlColumnBinder m_lastModificationTimeUtc = new SqlColumnBinder("LastModificationTimeUTC");
      private SqlColumnBinder m_leaseId = new SqlColumnBinder("LeaseId");
      private SqlColumnBinder m_newEventAdded = new SqlColumnBinder("NewEventAdded");
      private SqlColumnBinder m_jobTrigger = new SqlColumnBinder("JobTrigger");
      private IEnumerable<Type> m_changeEventDataKnownTypes;

      public IndexingUnitChangeEventWithEventAddedColumns2(IEnumerable<Type> knownTypes) => this.m_changeEventDataKnownTypes = knownTypes;

      protected override Tuple<IndexingUnitChangeEvent, bool> Bind()
      {
        if (this.m_id.IsNull((IDataReader) this.Reader))
          return (Tuple<IndexingUnitChangeEvent, bool>) null;
        return Tuple.Create<IndexingUnitChangeEvent, bool>(new IndexingUnitChangeEvent()
        {
          Id = this.m_id.GetInt64((IDataReader) this.Reader),
          IndexingUnitId = this.m_indexingUnitId.GetInt32((IDataReader) this.Reader),
          ChangeType = this.m_changeType.GetString((IDataReader) this.Reader, false),
          ChangeData = (ChangeEventData) SQLTable<IndexingUnitChangeEvent>.FromString(this.m_changeData.GetString((IDataReader) this.Reader, false), typeof (ChangeEventData), this.m_changeEventDataKnownTypes),
          CreatedTimeUtc = this.m_createdTimeUtc.GetDateTime((IDataReader) this.Reader),
          CurrentStage = this.m_currentStage.GetString((IDataReader) this.Reader, true),
          State = (IndexingUnitChangeEventState) Enum.Parse(typeof (IndexingUnitChangeEventState), this.m_state.GetString((IDataReader) this.Reader, false), true),
          AttemptCount = this.m_attemptCount.GetByte((IDataReader) this.Reader),
          Prerequisites = (IndexingUnitChangeEventPrerequisites) SQLTable<IndexingUnitChangeEvent>.FromString(this.m_prerequisites.GetString((IDataReader) this.Reader, true), typeof (IndexingUnitChangeEventPrerequisites)),
          LastModificationTimeUtc = this.m_lastModificationTimeUtc.GetDateTime((IDataReader) this.Reader),
          LeaseId = this.m_leaseId.GetString((IDataReader) this.Reader, true)
        }, this.m_newEventAdded.GetInt32((IDataReader) this.Reader) == 1);
      }
    }

    protected class IndexingStatusColumns : ObjectBinder<Tuple<IEntityType, string, int>>
    {
      private SqlColumnBinder m_entityType = new SqlColumnBinder("EntityType");
      private SqlColumnBinder m_changeType = new SqlColumnBinder("ChangeType");
      private SqlColumnBinder m_jobTrigger = new SqlColumnBinder("JobTrigger");
      private IEnumerable<IEntityType> m_entityTypes;

      protected override Tuple<IEntityType, string, int> Bind()
      {
        IEntityType entityType = EntityPluginsFactory.GetEntityType(this.m_entityTypes, this.m_entityType.GetString((IDataReader) this.Reader, false));
        string str1 = this.m_changeType.GetString((IDataReader) this.Reader, false);
        int num1 = (int) this.m_jobTrigger.GetByte((IDataReader) this.Reader);
        string str2 = str1;
        int num2 = num1;
        return Tuple.Create<IEntityType, string, int>(entityType, str2, num2);
      }

      public IndexingStatusColumns(IEnumerable<IEntityType> entityTypes) => this.m_entityTypes = entityTypes;
    }
  }
}
