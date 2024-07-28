// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.V2Jobs.Common.AccountWorkItemBulkIndexer
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.V2Jobs.Common
{
  public static class AccountWorkItemBulkIndexer
  {
    private const int TracePoint = 1080600;
    private static string s_traceArea = "Indexing Pipeline";
    private static string s_traceLayer = "Indexer";

    public static void TriggerIndexing(ExecutionContext executionContext, bool queueAccountFaultIn = false)
    {
      IVssRequestContext requestContext = executionContext.RequestContext;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        if (!requestContext.IsSearchConfigured() || !requestContext.IsWorkItemIndexingEnabled())
          return;
        AccountWorkItemBulkIndexer.CheckJobDefinitionAndQueueAccountFaultInJob(executionContext);
      }
      else
      {
        if (!(!executionContext.RequestContext.IsWorkItemIndexingEnabled() | queueAccountFaultIn))
          return;
        requestContext.SetFeatureFlagState("Search.Server.WorkItem.Indexing", FeatureAvailabilityState.On, new TraceMetaData(1080600, AccountWorkItemBulkIndexer.s_traceArea, AccountWorkItemBulkIndexer.s_traceLayer));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080600, AccountWorkItemBulkIndexer.s_traceArea, AccountWorkItemBulkIndexer.s_traceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Queuing WorkItemAccountFaultIn job for hostId {0}", (object) requestContext.GetCollectionID())));
        requestContext.QueueDelayedNamedJob(JobConstants.WorkItemAccountFaultInJobId, executionContext.ServiceSettings.JobSettings.AccountFaultInJobDelayInSec, JobPriorityLevel.Normal);
        executionContext.ExecutionTracerContext.PublishKpi("SearchExtensionAccountFaultInJobTrigger", "Indexing Pipeline", (double) WorkItemEntityType.GetInstance().ID);
      }
    }

    private static void CheckJobDefinitionAndQueueAccountFaultInJob(
      ExecutionContext executionContext)
    {
      AccountWorkItemBulkIndexer.CreateAccountFaultInJobDefinitionIfDoesNotExists(executionContext.RequestContext);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080600, AccountWorkItemBulkIndexer.s_traceArea, AccountWorkItemBulkIndexer.s_traceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Queuing WorkItemAccountFaultInJob for hostId {0}", (object) executionContext.RequestContext.GetCollectionID())));
      executionContext.RequestContext.QueueDelayedNamedJob(JobConstants.WorkItemAccountFaultInJobId, executionContext.ServiceSettings.JobSettings.AccountFaultInJobDelayInSec, JobPriorityLevel.Normal);
      executionContext.ExecutionTracerContext.PublishKpi("SearchExtensionAccountFaultInJobTrigger", "Indexing Pipeline", (double) WorkItemEntityType.GetInstance().ID, true);
    }

    private static void CreateAccountFaultInJobDefinitionIfDoesNotExists(
      IVssRequestContext requestContext)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      if (service.QueryJobDefinition(requestContext, JobConstants.WorkItemAccountFaultInJobId) != null)
        return;
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(JobConstants.WorkItemAccountFaultInJobId, "WorkItem Account Fault in Job", "Microsoft.VisualStudio.Services.Search.Server.Jobs.WorkItemAccountFaultInJob")
      {
        DisableDuringUpgrade = true,
        PriorityClass = JobPriorityClass.AboveNormal,
        EnabledState = TeamFoundationJobEnabledState.Enabled
      };
      service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
      {
        foundationJobDefinition
      });
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080600, AccountWorkItemBulkIndexer.s_traceArea, AccountWorkItemBulkIndexer.s_traceLayer, "Created a new Job Definition for Microsoft.VisualStudio.Services.Search.Server.Jobs.WorkItemAccountFaultInJob.");
    }
  }
}
