// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.IDistributedTaskAnalyticsService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  [DefaultServiceImplementation(typeof (DistributedTaskAnalyticsService))]
  public interface IDistributedTaskAnalyticsService : IVssFrameworkService
  {
    List<ProjectDataspace> QueryProjects(IVssRequestContext requestContext);

    List<TaskDefinitionReference> QueryTaskDefinitionReferencesByReferenceId(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      int fromReferenceId,
      out int toReferenceId);

    List<TaskDefinitionTimelineRecord> QueryPlanTaskReferencesByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int planTaskReferencesBatchSize,
      TaskPlanWatermark fromWatermark);

    List<TaskPlan> QueryPlansByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermak);

    List<ShallowTaskPlan> QueryShallowPlansByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermak);

    List<TaskTimelineRecord> QueryTimelineRecordsByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int timelineRecordBatchSize,
      TaskPlanWatermark fromWatermark);

    List<TaskTimelineRecord> QueryTimelineRecordsByChangedDateWithIssues(
      IVssRequestContext requestContext,
      int dataspaceId,
      int timelineRecordBatchSize,
      TaskPlanWatermark fromWatermark);
  }
}
