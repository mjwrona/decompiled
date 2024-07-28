// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.DistributedTaskAnalyticsService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  public class DistributedTaskAnalyticsService : 
    IDistributedTaskAnalyticsService,
    IVssFrameworkService
  {
    private readonly RegistryQuery s_readAXDataFromReplica = new RegistryQuery("/Service/DistributedTask/Settings/ReadAXDataFromReplica");
    private const string c_layer = "DistributedTaskAnalyticsService";

    public List<ProjectDataspace> QueryProjects(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (new MethodScope(requestContext, nameof (DistributedTaskAnalyticsService), nameof (QueryProjects)))
      {
        using (AnalyticsComponent component = requestContext.CreateComponent<AnalyticsComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
          return component.QueryProjects();
      }
    }

    public List<TaskDefinitionReference> QueryTaskDefinitionReferencesByReferenceId(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      int fromReferenceId,
      out int toReferenceId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      toReferenceId = fromReferenceId;
      using (new MethodScope(requestContext, nameof (DistributedTaskAnalyticsService), nameof (QueryTaskDefinitionReferencesByReferenceId)))
      {
        using (AnalyticsComponent component = requestContext.CreateComponent<AnalyticsComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
        {
          List<TaskDefinitionReference> source = component.QueryTaskDefinitionReferencesByReferenceId(dataspaceId, batchSize, fromReferenceId);
          TaskDefinitionReference definitionReference = source.OrderBy<TaskDefinitionReference, int>((Func<TaskDefinitionReference, int>) (r => r.TaskDefinitionReferenceId)).LastOrDefault<TaskDefinitionReference>();
          if (definitionReference != null)
            toReferenceId = definitionReference.TaskDefinitionReferenceId;
          return source;
        }
      }
    }

    public List<TaskDefinitionTimelineRecord> QueryPlanTaskReferencesByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int planTaskReferencebatchSize,
      TaskPlanWatermark fromWatermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (new MethodScope(requestContext, nameof (DistributedTaskAnalyticsService), nameof (QueryPlanTaskReferencesByChangedDate)))
      {
        using (AnalyticsComponent component = requestContext.CreateComponent<AnalyticsComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
          return component.QueryPlanTaskReferencesByChangedDate(dataspaceId, planTaskReferencebatchSize, fromWatermark);
      }
    }

    public List<TaskPlan> QueryPlansByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (new MethodScope(requestContext, nameof (DistributedTaskAnalyticsService), nameof (QueryPlansByChangedDate)))
      {
        using (AnalyticsComponent component = requestContext.CreateComponent<AnalyticsComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
          return component.QueryPlansByChangedDate(dataspaceId, batchSize, fromWatermark);
      }
    }

    public List<ShallowTaskPlan> QueryShallowPlansByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (new MethodScope(requestContext, nameof (DistributedTaskAnalyticsService), nameof (QueryShallowPlansByChangedDate)))
      {
        using (AnalyticsComponent component = requestContext.CreateComponent<AnalyticsComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
          return component.QueryShallowPlansByChangedDate(dataspaceId, batchSize, fromWatermark);
      }
    }

    public List<TaskTimelineRecord> QueryTimelineRecordsByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int timelineRecordBatchSize,
      TaskPlanWatermark fromWatermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (new MethodScope(requestContext, nameof (DistributedTaskAnalyticsService), nameof (QueryTimelineRecordsByChangedDate)))
      {
        using (AnalyticsComponent component = requestContext.CreateComponent<AnalyticsComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
          return component.QueryTimelineRecordsByChangedDate(dataspaceId, timelineRecordBatchSize, fromWatermark);
      }
    }

    public List<TaskTimelineRecord> QueryTimelineRecordsByChangedDateWithIssues(
      IVssRequestContext requestContext,
      int dataspaceId,
      int timelineRecordBatchSize,
      TaskPlanWatermark fromWatermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (new MethodScope(requestContext, nameof (DistributedTaskAnalyticsService), nameof (QueryTimelineRecordsByChangedDateWithIssues)))
      {
        using (AnalyticsComponent component = requestContext.CreateComponent<AnalyticsComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
          return component.QueryTimelineRecordsByChangedDateWithIssues(dataspaceId, timelineRecordBatchSize, fromWatermark);
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    private DatabaseConnectionType GetDatabaseConnectionType(IVssRequestContext requestContext) => !requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in this.s_readAXDataFromReplica, true) ? DatabaseConnectionType.Default : DatabaseConnectionType.IntentReadOnly;
  }
}
