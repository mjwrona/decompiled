// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitChangeEventComponentV2
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
  public class IndexingUnitChangeEventComponentV2 : IndexingUnitChangeEventComponent
  {
    private static readonly SqlMetaData[] s_indexingUnitChangeEventLookupTable = new SqlMetaData[8]
    {
      new SqlMetaData("IndexingUnitId", SqlDbType.Int),
      new SqlMetaData("ChangeType", SqlDbType.VarChar, 100L),
      new SqlMetaData("ChangeData", SqlDbType.NVarChar, -1L),
      new SqlMetaData("CurrentStage", SqlDbType.VarChar, 100L),
      new SqlMetaData("State", SqlDbType.VarChar, 24L),
      new SqlMetaData("AttemptCount", SqlDbType.TinyInt),
      new SqlMetaData("Prerequisites", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LeaseId", SqlDbType.NVarChar, (long) byte.MaxValue)
    };
    private static readonly SqlMetaData[] s_indexingUnitChangeEventsUpdateTable = new SqlMetaData[9]
    {
      new SqlMetaData("Id", SqlDbType.BigInt),
      new SqlMetaData("IndexingUnitId", SqlDbType.Int),
      new SqlMetaData("ChangeType", SqlDbType.VarChar, 100L),
      new SqlMetaData("ChangeData", SqlDbType.NVarChar, -1L),
      new SqlMetaData("CurrentStage", SqlDbType.VarChar, 100L),
      new SqlMetaData("State", SqlDbType.VarChar, 24L),
      new SqlMetaData("AttemptCount", SqlDbType.TinyInt),
      new SqlMetaData("Prerequisites", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LeaseId", SqlDbType.NVarChar, (long) byte.MaxValue)
    };

    public IndexingUnitChangeEventComponentV2()
    {
    }

    internal IndexingUnitChangeEventComponentV2(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
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

    public override void Delete(IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      if (indexingUnitChangeEvent == null)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (indexingUnitChangeEvent)));
      try
      {
        this.PrepareStoredProcedure("Search.prc_DeleteIndexingUnitChangeEvent");
        this.BindLong("@id", indexingUnitChangeEvent.Id);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to Delete IndexingUnitChangeEvent {0} with SQL Azure platform", (object) indexingUnitChangeEvent.ToString()));
      }
    }

    public override IndexingUnitChangeEvent Update(IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      IList<IndexingUnitChangeEvent> source = indexingUnitChangeEvent != null ? this.Update((IList<IndexingUnitChangeEvent>) new List<IndexingUnitChangeEvent>()
      {
        indexingUnitChangeEvent
      }) : throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (indexingUnitChangeEvent)));
      return source != null && source.Any<IndexingUnitChangeEvent>() ? source.First<IndexingUnitChangeEvent>() : throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to update IndexingUnitChangeEvent {0} with SQL Azure platform", (object) indexingUnitChangeEvent.ToString()));
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
              resultCollection.AddBinder<IndexingUnitChangeEventDetails>((ObjectBinder<IndexingUnitChangeEventDetails>) new IndexingUnitChangeEventComponentV2.IndexingUnitChangeEventColumnsV2(this.m_changeEventDataKnownTypes));
              ObjectBinder<IndexingUnitChangeEventDetails> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetails>();
              if (current != null && current.Items != null && current.Items.Count >= 1)
                return (IList<IndexingUnitChangeEvent>) current.Items.Select<IndexingUnitChangeEventDetails, IndexingUnitChangeEvent>((System.Func<IndexingUnitChangeEventDetails, IndexingUnitChangeEvent>) (x => x.IndexingUnitChangeEvent)).ToList<IndexingUnitChangeEvent>();
              throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to update IndexingUnitChangeEvents with SQL Azure platform"));
            }
          }
          catch (Exception ex)
          {
            throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to update IndexingUnitChangeEvent with with SQL Azure platform"));
          }
        }
      }
      throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("indexingUnitChangeEvents is null or empty"));
    }

    public override void DeleteIndexingUnitChangeEventBatch(
      List<IndexingUnitChangeEvent> indexingUnitChangeEventList)
    {
      this.ValidateNotNullOrEmptyList<IndexingUnitChangeEvent>(nameof (indexingUnitChangeEventList), (IList<IndexingUnitChangeEvent>) indexingUnitChangeEventList);
      List<long> list = indexingUnitChangeEventList.Select<IndexingUnitChangeEvent, long>((System.Func<IndexingUnitChangeEvent, long>) (item => item.Id)).ToList<long>();
      try
      {
        this.PrepareStoredProcedure("Search.prc_DeleteIndexingUnitChangeEventBatch");
        this.BindIndexingUnitChangeEventIdTable("@idList", (IEnumerable<long>) list);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to execute DeleteIndexingUnitChangeEventBatch operation with SQL Azure Platform");
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
          resultCollection.AddBinder<IndexingUnitChangeEventDetails>((ObjectBinder<IndexingUnitChangeEventDetails>) new IndexingUnitChangeEventComponentV2.IndexingUnitChangeEventDetailsColumnsV2(this.m_changeEventDataKnownTypes));
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
          resultCollection.AddBinder<IndexingUnitChangeEventDetails>((ObjectBinder<IndexingUnitChangeEventDetails>) new IndexingUnitChangeEventComponentV2.IndexingUnitChangeEventDetailsColumnsV2(this.m_changeEventDataKnownTypes));
          ObjectBinder<IndexingUnitChangeEventDetails> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetails>();
          return current != null && current.Items != null && current.Items.Count > 0 ? current.Items : new List<IndexingUnitChangeEventDetails>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    public override int GetCountOfIndexingUnitChangeEvents()
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryCountOfEvents");
        return (int) this.ExecuteScalar();
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve count of IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    public override int GetCountOfIndexingUnitChangeEvents(IndexingUnitChangeEventState state)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryCountOfEventsByState");
        this.BindString("@state", state.ToString(), 24, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        return (int) this.ExecuteScalar();
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve count of IndexingUnitChangeEvents from SQL Azure Platform");
      }
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
          resultCollection.AddBinder<IndexingUnitChangeEventDetails>((ObjectBinder<IndexingUnitChangeEventDetails>) new IndexingUnitChangeEventComponentV2.IndexingUnitChangeEventDetailsColumnsV2(this.m_changeEventDataKnownTypes));
          ObjectBinder<IndexingUnitChangeEventDetails> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetails>();
          return current != null && current.Items != null && current.Items.Count > 0 ? current.Items : new List<IndexingUnitChangeEventDetails>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    public override void ArchiveCompletedIndexingUnitChangeEvents(int periodicCleanupOldEvents)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_ArchiveIndexingUnitChangeEvents");
        this.BindInt("@hours", periodicCleanupOldEvents);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to Archive.");
      }
    }

    public override IDictionary<string, TimeSpan> GetMaxTimeForEachChangeType()
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryMaxTimeForEachChangeType");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Tuple<string, TimeSpan>>((ObjectBinder<Tuple<string, TimeSpan>>) new IndexingUnitChangeEventComponentV2.ChangeTypeMaxValues());
          ObjectBinder<Tuple<string, TimeSpan>> current = resultCollection.GetCurrent<Tuple<string, TimeSpan>>();
          return current != null && current.Items != null && current.Items.Count > 0 ? (IDictionary<string, TimeSpan>) current.Items.ToDictionary<Tuple<string, TimeSpan>, string, TimeSpan>((System.Func<Tuple<string, TimeSpan>, string>) (x => x.Item1), (System.Func<Tuple<string, TimeSpan>, TimeSpan>) (x => x.Item2)) : (IDictionary<string, TimeSpan>) new Dictionary<string, TimeSpan>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve Maximum time taken for change types from SQL Azure Platform");
      }
    }

    protected virtual SqlParameter BindIndexingUnitChangeEventLookupTable(
      string parameterName,
      IEnumerable<IndexingUnitChangeEvent> rows)
    {
      rows = rows ?? Enumerable.Empty<IndexingUnitChangeEvent>();
      System.Func<IndexingUnitChangeEvent, SqlDataRecord> selector = (System.Func<IndexingUnitChangeEvent, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitChangeEventComponentV2.s_indexingUnitChangeEventLookupTable);
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
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_IndexingUnitChangeEventDescriptorV2", rows.Select<IndexingUnitChangeEvent, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindIndexingUnitChangeEventsUpdateTable(
      string parameterName,
      IEnumerable<IndexingUnitChangeEvent> rows)
    {
      rows = rows ?? Enumerable.Empty<IndexingUnitChangeEvent>();
      System.Func<IndexingUnitChangeEvent, SqlDataRecord> selector = (System.Func<IndexingUnitChangeEvent, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitChangeEventComponentV2.s_indexingUnitChangeEventsUpdateTable);
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
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_IndexingUnitChangeEventWithIdDescriptorV2", rows.Select<IndexingUnitChangeEvent, SqlDataRecord>(selector));
    }

    internal class InternalIndexingUnitChangeEventColumnsV2
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
      private IEnumerable<Type> m_changeEventDataKnownTypes;

      public InternalIndexingUnitChangeEventColumnsV2(IEnumerable<Type> knownTypes) => this.m_changeEventDataKnownTypes = knownTypes;

      internal IndexingUnitChangeEvent Bind(SqlDataReader reader)
      {
        if (this.m_id.IsNull((IDataReader) reader))
          return (IndexingUnitChangeEvent) null;
        return new IndexingUnitChangeEvent()
        {
          Id = this.m_id.GetInt64((IDataReader) reader),
          IndexingUnitId = this.m_indexingUnitId.GetInt32((IDataReader) reader),
          ChangeType = this.m_changeType.GetString((IDataReader) reader, false),
          ChangeData = (ChangeEventData) SQLTable<IndexingUnitChangeEvent>.FromString(this.m_changeData.GetString((IDataReader) reader, false), typeof (ChangeEventData), this.m_changeEventDataKnownTypes),
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

    protected class IndexingUnitChangeEventColumnsV2 : ObjectBinder<IndexingUnitChangeEventDetails>
    {
      private IEnumerable<Type> m_changeEventDataKnownTypes;

      public IndexingUnitChangeEventColumnsV2(IEnumerable<Type> knownTypes) => this.m_changeEventDataKnownTypes = knownTypes;

      protected override IndexingUnitChangeEventDetails Bind() => new IndexingUnitChangeEventDetails()
      {
        IndexingUnitChangeEvent = new IndexingUnitChangeEventComponentV2.InternalIndexingUnitChangeEventColumnsV2(this.m_changeEventDataKnownTypes).Bind(this.Reader)
      };
    }

    protected class IndexingUnitChangeEventDetailsColumnsV2 : 
      IndexingUnitChangeEventComponentV2.IndexingUnitChangeEventColumnsV2
    {
      private SqlColumnBinder m_associatedJobId = new SqlColumnBinder("AssociatedJobId");

      public IndexingUnitChangeEventDetailsColumnsV2(IEnumerable<Type> knownTypes)
        : base(knownTypes)
      {
      }

      protected override IndexingUnitChangeEventDetails Bind()
      {
        IndexingUnitChangeEventDetails changeEventDetails = base.Bind();
        Guid guid = this.m_associatedJobId.GetGuid((IDataReader) this.Reader, true);
        changeEventDetails.AssociatedJobId = guid.Equals(Guid.Empty) ? new Guid?() : new Guid?(guid);
        return changeEventDetails;
      }
    }

    protected class IndexingUnitChangeEventDetailsWithEntityTypeColumns : 
      ObjectBinder<IndexingUnitChangeEventDetailsWithEntityType>
    {
      private SqlColumnBinder m_associatedJobId = new SqlColumnBinder("AssociatedJobId");
      private SqlColumnBinder m_entityType = new SqlColumnBinder("EntityType");
      private IEnumerable<IEntityType> m_entityTypes;
      private IEnumerable<Type> m_changeEventDataKnownTypes;

      protected override IndexingUnitChangeEventDetailsWithEntityType Bind()
      {
        IndexingUnitChangeEvent indexingUnitChangeEvent = new IndexingUnitChangeEventComponentV2.InternalIndexingUnitChangeEventColumnsV2(this.m_changeEventDataKnownTypes).Bind(this.Reader);
        Guid guid = this.m_associatedJobId.GetGuid((IDataReader) this.Reader, true);
        IndexingUnitChangeEventDetailsWithEntityType detailsWithEntityType = new IndexingUnitChangeEventDetailsWithEntityType();
        detailsWithEntityType.IndexingUnitChangeEvent = indexingUnitChangeEvent;
        detailsWithEntityType.AssociatedJobId = guid.Equals(Guid.Empty) ? new Guid?() : new Guid?(guid);
        detailsWithEntityType.EntityType = this.m_entityType.ColumnExists((IDataReader) this.Reader) ? EntityPluginsFactory.GetEntityType(this.m_entityTypes, this.m_entityType.GetString((IDataReader) this.Reader, false)) : (IEntityType) null;
        return detailsWithEntityType;
      }

      public IndexingUnitChangeEventDetailsWithEntityTypeColumns(
        IEnumerable<IEntityType> entityTypes,
        IEnumerable<Type> knownTypes)
      {
        this.m_entityTypes = entityTypes;
        this.m_changeEventDataKnownTypes = knownTypes;
      }
    }

    private class ChangeTypeMaxValues : ObjectBinder<Tuple<string, TimeSpan>>
    {
      private SqlColumnBinder m_changeType = new SqlColumnBinder("ChangeType");
      private SqlColumnBinder m_maxTimeTaken = new SqlColumnBinder("MaxTimeTaken");

      protected override Tuple<string, TimeSpan> Bind() => this.m_changeType.IsNull((IDataReader) this.Reader) ? (Tuple<string, TimeSpan>) null : new Tuple<string, TimeSpan>(this.m_changeType.GetString((IDataReader) this.Reader, false), TimeSpan.FromMinutes((double) this.m_maxTimeTaken.GetInt32((IDataReader) this.Reader)));
    }
  }
}
