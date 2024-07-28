// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.AnalyticsComponent2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  internal class AnalyticsComponent2 : AnalyticsComponent
  {
    public override List<TaskDefinitionReference> QueryTaskDefinitionReferencesByReferenceId(
      int dataspaceId,
      int batchSize,
      int fromReferenceId)
    {
      this.PrepareStoredProcedure("Task.prc_QueryTaskDefinitionReferencesByReferenceId1");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt(nameof (batchSize), batchSize);
      this.BindInt(nameof (fromReferenceId), fromReferenceId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TaskDefinitionReference>((ObjectBinder<TaskDefinitionReference>) this.GetTaskDefinitionReferenceBinder(dataspaceId));
        return resultCollection.GetCurrent<TaskDefinitionReference>().Items;
      }
    }

    public override List<TaskDefinitionTimelineRecord> QueryPlanTaskReferencesByChangedDate(
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermark)
    {
      this.PrepareStoredProcedure("Task.prc_QueryPlanTaskReferencesByChangedDate1");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt("internalId", fromWatermark.InternalId);
      this.BindDateTime("fromLastUpdatedDate", fromWatermark.LastUpdated, true);
      this.BindInt(nameof (batchSize), batchSize);
      List<TaskDefinitionTimelineRecord> definitionTimelineRecordList = new List<TaskDefinitionTimelineRecord>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TaskDefinitionTimelineRecord>((ObjectBinder<TaskDefinitionTimelineRecord>) this.GetTaskDefinitionTimelineRecordBinder(dataspaceId));
        resultCollection.AddBinder<TaskDefinitionTimelineRecord>((ObjectBinder<TaskDefinitionTimelineRecord>) this.GetTaskDefinitionTimelineRecordBinder(dataspaceId));
        definitionTimelineRecordList.AddRange((IEnumerable<TaskDefinitionTimelineRecord>) resultCollection.GetCurrent<TaskDefinitionTimelineRecord>().Items);
        if (resultCollection.TryNextResult())
          definitionTimelineRecordList.AddRange((IEnumerable<TaskDefinitionTimelineRecord>) resultCollection.GetCurrent<TaskDefinitionTimelineRecord>().Items);
      }
      return definitionTimelineRecordList;
    }

    public override List<TaskPlan> QueryPlansByChangedDate(
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermark)
    {
      this.PrepareStoredProcedure("Task.prc_QueryPlansByChangedDate1");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt(nameof (batchSize), batchSize);
      this.BindInt("internalId", fromWatermark.InternalId);
      this.BindDateTime("fromChangedDate", fromWatermark.LastUpdated, true);
      List<TaskPlan> taskPlanList = new List<TaskPlan>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TaskPlan>((ObjectBinder<TaskPlan>) this.GetTaskPlanBinder(dataspaceId));
        resultCollection.AddBinder<TaskPlan>((ObjectBinder<TaskPlan>) this.GetTaskPlanBinder(dataspaceId));
        taskPlanList.AddRange((IEnumerable<TaskPlan>) resultCollection.GetCurrent<TaskPlan>().Items);
        if (resultCollection.TryNextResult())
          taskPlanList.AddRange((IEnumerable<TaskPlan>) resultCollection.GetCurrent<TaskPlan>().Items);
      }
      return taskPlanList;
    }

    public override List<TaskTimelineRecord> QueryTimelineRecordsByChangedDate(
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermark)
    {
      this.PrepareStoredProcedure("Task.prc_QueryTimelineRecordsByChangedDate1");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt("internalId", fromWatermark.InternalId);
      this.BindDateTime("fromLastUpdatedDate", fromWatermark.LastUpdated, true);
      this.BindInt(nameof (batchSize), batchSize);
      List<TaskTimelineRecord> taskTimelineRecordList = new List<TaskTimelineRecord>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TaskTimelineRecord>((ObjectBinder<TaskTimelineRecord>) this.GetTaskTimelineRecordBinder(dataspaceId));
        resultCollection.AddBinder<TaskTimelineRecord>((ObjectBinder<TaskTimelineRecord>) this.GetTaskTimelineRecordBinder(dataspaceId));
        taskTimelineRecordList.AddRange((IEnumerable<TaskTimelineRecord>) resultCollection.GetCurrent<TaskTimelineRecord>().Items);
        if (resultCollection.TryNextResult())
          taskTimelineRecordList.AddRange((IEnumerable<TaskTimelineRecord>) resultCollection.GetCurrent<TaskTimelineRecord>().Items);
      }
      return taskTimelineRecordList;
    }

    protected virtual TaskDefinitionReferenceBinder GetTaskDefinitionReferenceBinder(int dataspaceId) => new TaskDefinitionReferenceBinder((TaskSqlComponentBase) this, this.GetDataspaceIdentifier(dataspaceId));

    protected virtual TaskDefinitionTimelineRecordBinder GetTaskDefinitionTimelineRecordBinder(
      int dataspaceId)
    {
      return new TaskDefinitionTimelineRecordBinder((TaskSqlComponentBase) this, this.GetDataspaceIdentifier(dataspaceId));
    }

    protected virtual TaskPlanBinder GetTaskPlanBinder(int dataspaceId) => new TaskPlanBinder((TaskSqlComponentBase) this, this.GetDataspaceIdentifier(dataspaceId));

    protected virtual ShallowTaskPlanBinder GetShallowTaskPlanBinder(int dataspaceId) => new ShallowTaskPlanBinder((TaskSqlComponentBase) this, this.GetDataspaceIdentifier(dataspaceId));

    protected virtual TaskTimelineRecordBinder GetTaskTimelineRecordBinder(int dataspaceId) => new TaskTimelineRecordBinder((TaskSqlComponentBase) this, this.GetDataspaceIdentifier(dataspaceId));
  }
}
