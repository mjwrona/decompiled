// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemStateDefinitionComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemStateDefinitionComponent : WorkItemTrackingResourceComponent
  {
    private static readonly IDictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<WorkItemStateDefinitionComponent>(1),
      (IComponentCreator) new ComponentCreator<WorkItemStateDefinitionComponent2>(2)
    }, "WorkItemStateDefinition", "WorkItem");

    protected virtual SqlParameter BindStates(
      string parameterName,
      IEnumerable<WorkItemStateDeclaration> states)
    {
      return this.BindBasicTvp<WorkItemStateDeclaration>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemStateDeclaration>) new WorkItemStateDefinitionComponent.WorkItemStateBinder(), parameterName, states);
    }

    protected virtual SqlParameter BindStateOrders(
      string parameterName,
      IDictionary<Guid, int> stateOrders)
    {
      return this.BindBasicTvp<KeyValuePair<Guid, int>>((WorkItemTrackingResourceComponent.TvpRecordBinder<KeyValuePair<Guid, int>>) new WorkItemStateDefinitionComponent.WorkItemStateOrderBinder(), parameterName, (IEnumerable<KeyValuePair<Guid, int>>) stateOrders);
    }

    static WorkItemStateDefinitionComponent()
    {
      WorkItemStateDefinitionComponent.s_sqlExceptionFactories[600503] = WorkItemTrackingResourceComponent.CreateFactory<WorkItemStateDefinitionNotFoundException>();
      WorkItemStateDefinitionComponent.s_sqlExceptionFactories[600504] = WorkItemTrackingResourceComponent.CreateFactory<WorkItemStateOrderInvalidException>();
      WorkItemStateDefinitionComponent.s_sqlExceptionFactories[600505] = WorkItemTrackingResourceComponent.CreateFactory<WorkItemStateNameAlreadyInUseException>();
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => WorkItemStateDefinitionComponent.s_sqlExceptionFactories;

    protected virtual ObjectBinder<WorkItemStateDefinitionRecord> CreateStateRecordBinder() => (ObjectBinder<WorkItemStateDefinitionRecord>) new WorkItemStateDefinitionComponent.WorkItemStateDefinitionRecordBinder();

    public virtual IReadOnlyCollection<WorkItemStateDefinitionRecord> GetProcessStateDefinitions(
      Guid processId)
    {
      this.PrepareStoredProcedure("prc_GetProcessStateDefinition");
      this.BindGuid("@processId", processId);
      ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<WorkItemStateDefinitionRecord>(this.CreateStateRecordBinder());
      return (IReadOnlyCollection<WorkItemStateDefinitionRecord>) resultCollection.GetCurrent<WorkItemStateDefinitionRecord>().Items;
    }

    public virtual void CreateWorkItemStateDefinition(
      Guid processId,
      Guid workItemTypeId,
      Guid changerId,
      WorkItemStateDeclaration state,
      IDictionary<Guid, int> updatedStateOrders)
    {
      this.PrepareStoredProcedure("prc_CreateWorkItemStateDefinition");
      this.BindGuid("@processId", processId);
      this.BindGuid("@workItemTypeId", workItemTypeId);
      this.BindStates("@states", (IEnumerable<WorkItemStateDeclaration>) new List<WorkItemStateDeclaration>()
      {
        state
      });
      this.BindStateOrders("@stateOrders", updatedStateOrders);
      this.BindGuid("@changerId", changerId);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateWorkItemStateDefinition(
      Guid processId,
      Guid workItemTypeId,
      Guid changerId,
      WorkItemStateDeclaration state,
      IDictionary<Guid, int> updatedStateOrders,
      bool orderChanged)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkItemStateDefinition");
      this.BindGuid("@processId", processId);
      this.BindGuid("@workItemTypeId", workItemTypeId);
      this.BindGuid("@stateId", state.Id.Value);
      this.BindString("@color", state.Color, 10, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      if (orderChanged)
      {
        this.BindInt("@stateCategory", (int) state.StateCategory);
        this.BindInt("@stateOrder", state.Order);
        this.BindStateOrders("@stateOrders", updatedStateOrders);
      }
      else
      {
        this.BindNullValue("@stateCategory", SqlDbType.Int);
        this.BindNullValue("@stateOrder", SqlDbType.Int);
      }
      this.BindGuid("@changerId", changerId);
      this.ExecuteNonQuery();
    }

    public virtual void DeleteWorkItemStateDefinition(
      Guid processId,
      Guid workItemTypeId,
      Guid changerId,
      Guid stateId,
      IDictionary<Guid, int> updatedStateOrders)
    {
      this.PrepareStoredProcedure("prc_DeleteWorkItemStateDefinition");
      this.BindGuid("@processId", processId);
      this.BindGuid("@workItemTypeId", workItemTypeId);
      this.BindGuid("@stateId", stateId);
      this.BindStateOrders("@stateOrders", updatedStateOrders);
      this.BindGuid("@changerId", changerId);
      this.ExecuteNonQuery();
    }

    public virtual void HideParentStateDefinition(
      Guid processId,
      Guid workItemTypeId,
      Guid changerId,
      WorkItemStateDeclaration state)
    {
      this.PrepareStoredProcedure("prc_HideWorkItemStateDefinition");
      this.BindGuid("@processId", processId);
      this.BindGuid("@workItemTypeId", workItemTypeId);
      this.BindGuid("@stateId", state.Id.Value);
      this.BindString("@name", state.Name, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@color", state.Color, 10, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@stateCategory", (int) state.StateCategory);
      this.BindInt("@stateOrder", state.Order);
      this.BindGuid("@changerId", changerId);
      this.ExecuteNonQuery();
    }

    public virtual IReadOnlyCollection<ProcessIdDateTimeWatermarkPair> GetChangedStateProcessesSinceWatermark(
      DateTime watermark)
    {
      throw new NotSupportedException();
    }

    protected class WorkItemStateDefinitionRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemStateDefinitionRecord>
    {
      private SqlColumnBinder m_processIdCol = new SqlColumnBinder("ProcessId");
      private SqlColumnBinder m_workItemTypeId = new SqlColumnBinder("WorkItemTypeId");
      private SqlColumnBinder m_stateId = new SqlColumnBinder("StateId");
      private SqlColumnBinder m_text = new SqlColumnBinder("Name");
      private SqlColumnBinder m_stateOrder = new SqlColumnBinder("StateOrder");
      private SqlColumnBinder m_color = new SqlColumnBinder("Color");
      private SqlColumnBinder m_stateCategory = new SqlColumnBinder("StateCategory");
      private SqlColumnBinder m_hidden = new SqlColumnBinder("Hidden");
      private SqlColumnBinder m_autorizedDate = new SqlColumnBinder("AuthorizedDate");

      public override WorkItemStateDefinitionRecord Bind(IDataReader reader) => new WorkItemStateDefinitionRecord()
      {
        ProcessId = this.m_processIdCol.GetGuid(reader),
        WorkItemTypeId = this.m_workItemTypeId.GetGuid(reader),
        StateId = this.m_stateId.GetGuid(reader),
        Name = this.m_text.GetString(reader, false),
        StateOrder = this.m_stateOrder.GetInt32(reader),
        Color = this.m_color.GetString(reader, false),
        StateCategory = this.m_stateCategory.GetInt32(reader),
        Hidden = this.m_hidden.GetBoolean(reader, false),
        AuthorizedDate = this.m_autorizedDate.GetDateTime(reader)
      };
    }

    protected class WorkItemStateBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemStateDeclaration>
    {
      protected static readonly SqlMetaData[] s_metadata = new SqlMetaData[5]
      {
        new SqlMetaData("StateId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("Name", SqlDbType.NVarChar, 128L),
        new SqlMetaData("Color", SqlDbType.NVarChar, 10L),
        new SqlMetaData("StateCategory", SqlDbType.Int),
        new SqlMetaData("StateOrder", SqlDbType.Int)
      };

      public override string TypeName => "typ_WitStateDefinitionTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemStateDefinitionComponent.WorkItemStateBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemStateDeclaration state)
      {
        if (state.Id.HasValue && state.Id.Value != Guid.Empty)
          record.SetGuid(0, state.Id.Value);
        record.SetString(1, state.Name);
        record.SetString(2, state.Color);
        record.SetInt32(3, (int) state.StateCategory);
        record.SetInt32(4, state.Order);
      }
    }

    protected class WorkItemStateOrderBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<KeyValuePair<Guid, int>>
    {
      protected static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
      {
        new SqlMetaData("StateId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("StateOrder", SqlDbType.Int)
      };

      public override string TypeName => "typ_WitStateOrderTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemStateDefinitionComponent.WorkItemStateOrderBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        KeyValuePair<Guid, int> stateOrder)
      {
        record.SetGuid(0, stateOrder.Key);
        record.SetInt32(1, stateOrder.Value);
      }
    }

    protected class ProcessIdDateTimeWatermarkPairRowBinder : 
      WorkItemTrackingObjectBinder<ProcessIdDateTimeWatermarkPair>
    {
      private SqlColumnBinder m_processIdColumn = new SqlColumnBinder("ProcessId");
      private SqlColumnBinder m_watermarkColumn = new SqlColumnBinder("MaxAuthorizedDate");

      public override ProcessIdDateTimeWatermarkPair Bind(IDataReader reader) => new ProcessIdDateTimeWatermarkPair()
      {
        ProcessId = this.m_processIdColumn.GetGuid(this.Reader),
        LatestWatermark = this.m_watermarkColumn.GetDateTime(this.Reader)
      };
    }
  }
}
