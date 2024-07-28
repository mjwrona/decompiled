// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitChangeEventComponentV4
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class IndexingUnitChangeEventComponentV4 : IndexingUnitChangeEventComponentV3
  {
    public IndexingUnitChangeEventComponentV4()
    {
    }

    internal IndexingUnitChangeEventComponentV4(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
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
        newEventAdded = false;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Tuple<IndexingUnitChangeEvent, bool>>((ObjectBinder<Tuple<IndexingUnitChangeEvent, bool>>) new IndexingUnitChangeEventComponentV4.IndexingUnitChangeEventWithEventAddedColumns(this.m_changeEventDataKnownTypes));
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

    protected class IndexingUnitChangeEventWithEventAddedColumns : 
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
      private IEnumerable<Type> m_changeEventDataKnownTypes;

      public IndexingUnitChangeEventWithEventAddedColumns(IEnumerable<Type> knownTypes) => this.m_changeEventDataKnownTypes = knownTypes;

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
  }
}
