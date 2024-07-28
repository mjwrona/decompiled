// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitChangeEventComponentV9
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
  public class IndexingUnitChangeEventComponentV9 : IndexingUnitChangeEventComponentV8
  {
    private static readonly SqlMetaData[] s_indexingUnitChangeEventsUpdateTable = new SqlMetaData[10]
    {
      new SqlMetaData("Id", SqlDbType.BigInt),
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

    public IndexingUnitChangeEventComponentV9()
    {
    }

    internal IndexingUnitChangeEventComponentV9(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public override List<IndexingUnitChangeEventDetails> RetrieveIndexingUnitChangeEventDetails(
      int count,
      TableEntityFilterList filterList,
      bool isAdvancedQuery = false)
    {
      if (filterList == null)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (filterList)));
      if (filterList.Count == 0)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("Expecting at least 1 filter"));
      try
      {
        if (isAdvancedQuery)
          this.PrepareStoredProcedure("Search.prc_AdvancedQueryOnIndexingUnitChangeEventTable");
        else
          this.PrepareStoredProcedure("Search.prc_QueryOnIndexingUnitChangeEventTable");
        this.BindInt("@count", count);
        TableEntityFilter propertyFilter;
        if (filterList.TryRetrieveFilter("Id", out propertyFilter))
          this.BindLong("@id", long.Parse(propertyFilter.Value));
        if (filterList.TryRetrieveFilter("IndexingUnitId", out propertyFilter))
          this.BindInt("@indexingUnitId", int.Parse(propertyFilter.Value));
        if (filterList.TryRetrieveFilter("State", out propertyFilter))
        {
          if (isAdvancedQuery)
            this.BindIndexingUnitChangeEventStateTable("@state", (IEnumerable<IndexingUnitChangeEventState>) propertyFilter.ObjectValue);
          else
            this.BindString("@state", propertyFilter.Value, 24, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        }
        if (filterList.TryRetrieveFilter("ChangeType", out propertyFilter))
        {
          if (isAdvancedQuery)
            this.BindIndexingUnitChangeEventChangeTypeTable("@changeType", (IEnumerable<string>) propertyFilter.ObjectValue);
          else
            this.BindString("@changeType", propertyFilter.Value, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        }
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnitChangeEventDetails>((ObjectBinder<IndexingUnitChangeEventDetails>) new IndexingUnitChangeEventComponentV9.IndexingUnitChangeEventDetailsColumnsV3(this.m_changeEventDataKnownTypes));
          ObjectBinder<IndexingUnitChangeEventDetails> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetails>();
          return current != null && current.Items != null && current.Items.Count > 0 ? current.Items : new List<IndexingUnitChangeEventDetails>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    public override List<IndexingUnitChangeEventDetails> RetrieveIndexingUnitChangeEventDetailsByState(
      int count,
      IndexingUnitChangeEventState state)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryIndexingUnitChangeEventsOnState");
        this.BindInt("@count", count);
        this.BindString("@state", state.ToString(), 24, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnitChangeEventDetails>((ObjectBinder<IndexingUnitChangeEventDetails>) new IndexingUnitChangeEventComponentV9.IndexingUnitChangeEventDetailsColumnsV3(this.m_changeEventDataKnownTypes));
          ObjectBinder<IndexingUnitChangeEventDetails> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetails>();
          return current != null && current.Items != null && current.Items.Count > 0 ? current.Items : new List<IndexingUnitChangeEventDetails>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    public override List<IndexingUnitChangeEventDetails> RetrieveIndexingUnitChangeEventDetailsByIndexingUnitAndState(
      int count,
      int indexingUnitId,
      IndexingUnitChangeEventState state)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryIndexingUnitChangeEventsOnIndexingUnitAndState");
        this.BindInt("@count", count);
        this.BindString("@state", state.ToString(), 24, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindInt("@indexingUnitId", indexingUnitId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnitChangeEventDetails>((ObjectBinder<IndexingUnitChangeEventDetails>) new IndexingUnitChangeEventComponentV9.IndexingUnitChangeEventDetailsColumnsV3(this.m_changeEventDataKnownTypes));
          ObjectBinder<IndexingUnitChangeEventDetails> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetails>();
          return current != null && current.Items != null && current.Items.Count > 0 ? current.Items : new List<IndexingUnitChangeEventDetails>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    public override IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      int count,
      int indexingUnitId)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryNextSetOfEventsToBeProcessedForIndexingUnit");
        this.BindInt("@count", count);
        this.BindInt("@indexingUnitId", indexingUnitId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnitChangeEventDetailsWithEntityType>((ObjectBinder<IndexingUnitChangeEventDetailsWithEntityType>) new IndexingUnitChangeEventComponentV9.IndexingUnitChangeEventDetailsWithEntityTypeColumnsV2(this.m_entityTypes, this.m_changeEventDataKnownTypes));
          ObjectBinder<IndexingUnitChangeEventDetailsWithEntityType> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetailsWithEntityType>();
          return current != null && current.Items != null && current.Items.Count > 0 ? (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) current.Items : (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) new List<IndexingUnitChangeEventDetailsWithEntityType>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    public override IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      int count)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryNextSetOfEventsToBeProcessed");
        this.BindInt("@count", count);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnitChangeEventDetailsWithEntityType>((ObjectBinder<IndexingUnitChangeEventDetailsWithEntityType>) new IndexingUnitChangeEventComponentV9.IndexingUnitChangeEventDetailsWithEntityTypeColumnsV2(this.m_entityTypes, this.m_changeEventDataKnownTypes));
          ObjectBinder<IndexingUnitChangeEventDetailsWithEntityType> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetailsWithEntityType>();
          return current?.Items != null && current.Items.Count > 0 ? (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) current.Items : (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) new List<IndexingUnitChangeEventDetailsWithEntityType>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    public override IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      int count,
      IEntityType entityType)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryNextSetOfEventsToBeProcessedForEntityType");
        this.BindInt("@count", count);
        this.BindString("@entityType", entityType.Name.ToString(), 32, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnitChangeEventDetailsWithEntityType>((ObjectBinder<IndexingUnitChangeEventDetailsWithEntityType>) new IndexingUnitChangeEventComponentV9.IndexingUnitChangeEventDetailsWithEntityTypeColumnsV2(this.m_entityTypes, this.m_changeEventDataKnownTypes));
          ObjectBinder<IndexingUnitChangeEventDetailsWithEntityType> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetailsWithEntityType>();
          return current?.Items != null && current.Items.Count > 0 ? (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) current.Items : (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) new List<IndexingUnitChangeEventDetailsWithEntityType>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    public override IndexingUnitChangeEvent Update(IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      IList<IndexingUnitChangeEvent> source = indexingUnitChangeEvent != null ? this.Update((IList<IndexingUnitChangeEvent>) new List<IndexingUnitChangeEvent>()
      {
        indexingUnitChangeEvent
      }) : throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (indexingUnitChangeEvent)));
      return source != null && source.Any<IndexingUnitChangeEvent>() ? source.First<IndexingUnitChangeEvent>() : throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to update IndexingUnitChangeEvent {0} with SQL Azure platform", (object) indexingUnitChangeEvent.ToString()));
    }

    public override IList<IndexingUnitChangeEvent> Update(
      IList<IndexingUnitChangeEvent> indexingUnitChangeEvents)
    {
      if (indexingUnitChangeEvents != null)
      {
        if (indexingUnitChangeEvents.Count != 0)
        {
          try
          {
            this.PrepareStoredProcedure("Search.prc_UpdateIndexingUnitChangeEvents");
            this.BindIndexingUnitChangeEventsUpdateTable("@itemList", (IEnumerable<IndexingUnitChangeEvent>) indexingUnitChangeEvents);
            using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
            {
              resultCollection.AddBinder<IndexingUnitChangeEventDetails>((ObjectBinder<IndexingUnitChangeEventDetails>) new IndexingUnitChangeEventComponentV9.IndexingUnitChangeEventColumnsV3(this.m_changeEventDataKnownTypes));
              ObjectBinder<IndexingUnitChangeEventDetails> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetails>();
              if (current != null)
              {
                if (current.Items != null)
                {
                  if (current.Items.Count >= 1)
                    return (IList<IndexingUnitChangeEvent>) current.Items.Select<IndexingUnitChangeEventDetails, IndexingUnitChangeEvent>((System.Func<IndexingUnitChangeEventDetails, IndexingUnitChangeEvent>) (x => x.IndexingUnitChangeEvent)).ToList<IndexingUnitChangeEvent>();
                }
              }
            }
          }
          catch (Exception ex)
          {
            throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to update IndexingUnitChangeEvent with with SQL Azure platform"));
          }
          throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to update IndexingUnitChangeEvents with SQL Azure platform"));
        }
      }
      throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("indexingUnitChangeEvents is null or empty"));
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
          resultCollection.AddBinder<IndexingUnitChangeEventDetails>((ObjectBinder<IndexingUnitChangeEventDetails>) new IndexingUnitChangeEventComponentV9.IndexingUnitChangeEventColumnsV3(this.m_changeEventDataKnownTypes));
          ObjectBinder<IndexingUnitChangeEventDetails> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetails>();
          if (current != null)
          {
            if (current.Items != null)
            {
              if (current.Items.Count == 1)
                return current.Items.First<IndexingUnitChangeEventDetails>().IndexingUnitChangeEvent;
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to add IndexingUnitChangeEvent {0}  with SQL Azure platform", (object) indexingUnitChangeEvent.ToString()));
      }
      throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to add IndexingUnitChangeEvent {0}  with SQL Azure platform", (object) indexingUnitChangeEvent.ToString()));
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
                  resultCollection.AddBinder<IndexingUnitChangeEventDetails>((ObjectBinder<IndexingUnitChangeEventDetails>) new IndexingUnitChangeEventComponentV9.IndexingUnitChangeEventColumnsV3(this.m_changeEventDataKnownTypes));
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
          resultCollection.AddBinder<Tuple<IndexingUnitChangeEvent, bool>>((ObjectBinder<Tuple<IndexingUnitChangeEvent, bool>>) new IndexingUnitChangeEventComponentV9.IndexingUnitChangeEventWithEventAddedColumns3(this.m_changeEventDataKnownTypes));
          ObjectBinder<Tuple<IndexingUnitChangeEvent, bool>> current = resultCollection.GetCurrent<Tuple<IndexingUnitChangeEvent, bool>>();
          if (current != null)
          {
            if (current.Items != null)
            {
              if (current.Items.Count == 1)
              {
                Tuple<IndexingUnitChangeEvent, bool> tuple = current.Items.First<Tuple<IndexingUnitChangeEvent, bool>>();
                newEventAdded = tuple.Item2;
                return tuple.Item1;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to add IndexingUnitChangeEvent " + indexingUnitChangeEvent.ToString() + "  with SQL Azure platform");
      }
      throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to add IndexingUnitChangeEvent {0}  with SQL Azure platform", (object) indexingUnitChangeEvent.ToString()));
    }

    protected override SqlParameter BindIndexingUnitChangeEventsUpdateTable(
      string parameterName,
      IEnumerable<IndexingUnitChangeEvent> rows)
    {
      rows = rows ?? Enumerable.Empty<IndexingUnitChangeEvent>();
      System.Func<IndexingUnitChangeEvent, SqlDataRecord> selector = (System.Func<IndexingUnitChangeEvent, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitChangeEventComponentV9.s_indexingUnitChangeEventsUpdateTable);
        sqlDataRecord.SetInt64(0, entity.Id);
        sqlDataRecord.SetInt32(1, entity.IndexingUnitId);
        sqlDataRecord.SetString(2, entity.ChangeType);
        sqlDataRecord.SetString(3, SQLTable<IndexingUnitChangeEvent>.ToString((object) entity.ChangeData, typeof (ChangeEventData), this.m_changeEventDataKnownTypes));
        if (!string.IsNullOrWhiteSpace(entity.CurrentStage))
          sqlDataRecord.SetString(4, entity.CurrentStage);
        else
          sqlDataRecord.SetDBNull(4);
        sqlDataRecord.SetString(5, entity.State.ToString());
        sqlDataRecord.SetByte(6, entity.AttemptCount);
        sqlDataRecord.SetString(7, SQLTable<IndexingUnitChangeEvent>.ToString((object) entity.Prerequisites, typeof (IndexingUnitChangeEventPrerequisites)));
        if (!string.IsNullOrWhiteSpace(entity.LeaseId))
          sqlDataRecord.SetString(8, entity.LeaseId);
        else
          sqlDataRecord.SetDBNull(8);
        sqlDataRecord.SetByte(9, (byte) entity.ChangeData.Trigger);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_IndexingUnitChangeEventWithIdDescriptorV3", rows.Select<IndexingUnitChangeEvent, SqlDataRecord>(selector));
    }

    protected class IndexingUnitChangeEventWithEventAddedColumns3 : 
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

      public IndexingUnitChangeEventWithEventAddedColumns3(IEnumerable<Type> knownTypes) => this.m_changeEventDataKnownTypes = knownTypes;

      protected override Tuple<IndexingUnitChangeEvent, bool> Bind()
      {
        if (this.m_id.IsNull((IDataReader) this.Reader))
          return (Tuple<IndexingUnitChangeEvent, bool>) null;
        int num = (int) this.m_jobTrigger.GetByte((IDataReader) this.Reader);
        ChangeEventData changeEventData = (ChangeEventData) SQLTable<IndexingUnitChangeEvent>.FromString(this.m_changeData.GetString((IDataReader) this.Reader, false), typeof (ChangeEventData), this.m_changeEventDataKnownTypes);
        changeEventData.Trigger = num;
        return Tuple.Create<IndexingUnitChangeEvent, bool>(new IndexingUnitChangeEvent()
        {
          Id = this.m_id.GetInt64((IDataReader) this.Reader),
          IndexingUnitId = this.m_indexingUnitId.GetInt32((IDataReader) this.Reader),
          ChangeType = this.m_changeType.GetString((IDataReader) this.Reader, false),
          ChangeData = changeEventData,
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

    protected class IndexingUnitChangeEventColumnsV3 : ObjectBinder<IndexingUnitChangeEventDetails>
    {
      private IEnumerable<Type> m_changeEventDataKnownTypes;

      public IndexingUnitChangeEventColumnsV3(IEnumerable<Type> knownTypes) => this.m_changeEventDataKnownTypes = knownTypes;

      protected override IndexingUnitChangeEventDetails Bind() => new IndexingUnitChangeEventDetails()
      {
        IndexingUnitChangeEvent = new IndexingUnitChangeEventComponentV9.InternalIndexingUnitChangeEventColumnsV3(this.m_changeEventDataKnownTypes).Bind(this.Reader)
      };
    }

    protected class InternalIndexingUnitChangeEventColumnsV3
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
      private SqlColumnBinder m_jobTrigger = new SqlColumnBinder("JobTrigger");
      private IEnumerable<Type> m_changeEventDataKnownTypes;

      public InternalIndexingUnitChangeEventColumnsV3(IEnumerable<Type> knownTypes) => this.m_changeEventDataKnownTypes = knownTypes;

      internal IndexingUnitChangeEvent Bind(SqlDataReader reader)
      {
        if (this.m_id.IsNull((IDataReader) reader))
          return (IndexingUnitChangeEvent) null;
        int num = (int) this.m_jobTrigger.GetByte((IDataReader) reader);
        ChangeEventData changeEventData = (ChangeEventData) SQLTable<IndexingUnitChangeEvent>.FromString(this.m_changeData.GetString((IDataReader) reader, false), typeof (ChangeEventData), this.m_changeEventDataKnownTypes);
        changeEventData.Trigger = num;
        return new IndexingUnitChangeEvent()
        {
          Id = this.m_id.GetInt64((IDataReader) reader),
          IndexingUnitId = this.m_indexingUnitId.GetInt32((IDataReader) reader),
          ChangeType = this.m_changeType.GetString((IDataReader) reader, false),
          ChangeData = changeEventData,
          CreatedTimeUtc = this.m_createdTimeUtc.GetDateTime((IDataReader) reader),
          CurrentStage = this.m_currentStage.GetString((IDataReader) reader, true),
          State = (IndexingUnitChangeEventState) Enum.Parse(typeof (IndexingUnitChangeEventState), this.m_state.GetString((IDataReader) reader, false), true),
          AttemptCount = this.m_attemptCount.GetByte((IDataReader) reader),
          Prerequisites = (IndexingUnitChangeEventPrerequisites) SQLTable<IndexingUnitChangeEvent>.FromString(this.m_prerequisites.GetString((IDataReader) reader, true), typeof (IndexingUnitChangeEventPrerequisites)),
          LastModificationTimeUtc = this.m_lastModificationTimeUtc.GetDateTime((IDataReader) reader),
          LeaseId = this.m_leaseId.GetString((IDataReader) reader, true)
        };
      }
    }

    protected class IndexingUnitChangeEventDetailsColumnsV3 : 
      IndexingUnitChangeEventComponentV2.IndexingUnitChangeEventDetailsColumnsV2
    {
      private SqlColumnBinder m_jobTrigger = new SqlColumnBinder("JobTrigger");

      public IndexingUnitChangeEventDetailsColumnsV3(IEnumerable<Type> knownTypes)
        : base(knownTypes)
      {
      }

      protected override IndexingUnitChangeEventDetails Bind()
      {
        IndexingUnitChangeEventDetails changeEventDetails = base.Bind();
        changeEventDetails.IndexingUnitChangeEvent.ChangeData.Trigger = (int) this.m_jobTrigger.GetByte((IDataReader) this.Reader);
        return changeEventDetails;
      }
    }

    protected class IndexingUnitChangeEventDetailsWithEntityTypeColumnsV2 : 
      IndexingUnitChangeEventComponentV2.IndexingUnitChangeEventDetailsWithEntityTypeColumns
    {
      private SqlColumnBinder m_jobTrigger = new SqlColumnBinder("JobTrigger");

      protected override IndexingUnitChangeEventDetailsWithEntityType Bind()
      {
        IndexingUnitChangeEventDetailsWithEntityType detailsWithEntityType = base.Bind();
        detailsWithEntityType.IndexingUnitChangeEvent.ChangeData.Trigger = (int) this.m_jobTrigger.GetByte((IDataReader) this.Reader);
        return detailsWithEntityType;
      }

      public IndexingUnitChangeEventDetailsWithEntityTypeColumnsV2(
        IEnumerable<IEntityType> entityTypes,
        IEnumerable<Type> changeEventDataKnownTypes)
        : base(entityTypes, changeEventDataKnownTypes)
      {
      }
    }
  }
}
