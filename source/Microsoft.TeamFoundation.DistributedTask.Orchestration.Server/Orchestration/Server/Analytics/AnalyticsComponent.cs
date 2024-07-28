// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.AnalyticsComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  internal class AnalyticsComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(1, true),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent2>(2),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent3>(3),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent4>(4),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent5>(5),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent6>(6)
    }, "DistributedTaskAnalytics", "DistributedTaskOrchestration", 6);

    public AnalyticsComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual List<ProjectDataspace> QueryProjects()
    {
      this.PrepareStoredProcedure("Task.prc_QueryProjects");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectDataspace>((ObjectBinder<ProjectDataspace>) this.GetProjectDataspaceBinder());
        return resultCollection.GetCurrent<ProjectDataspace>().Items.ToList<ProjectDataspace>();
      }
    }

    public virtual List<TaskDefinitionReference> QueryTaskDefinitionReferencesByReferenceId(
      int dataspaceId,
      int batchSize,
      int fromReferenceId)
    {
      return new List<TaskDefinitionReference>();
    }

    public virtual List<TaskPlan> QueryPlansByChangedDate(
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermark)
    {
      return new List<TaskPlan>();
    }

    public virtual List<TaskTimelineRecord> QueryTimelineRecordsByChangedDate(
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermark)
    {
      return new List<TaskTimelineRecord>();
    }

    public virtual List<TaskTimelineRecord> QueryTimelineRecordsByChangedDateWithIssues(
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermark)
    {
      return new List<TaskTimelineRecord>();
    }

    public virtual List<TaskDefinitionTimelineRecord> QueryPlanTaskReferencesByChangedDate(
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermark)
    {
      return new List<TaskDefinitionTimelineRecord>();
    }

    public virtual List<ShallowTaskPlan> QueryShallowPlansByChangedDate(
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermark)
    {
      throw new NotImplementedException("This feature is not available below the Analytics service 4");
    }

    protected virtual ProjectDataspaceBinder GetProjectDataspaceBinder() => new ProjectDataspaceBinder((TaskSqlComponentBase) this);
  }
}
