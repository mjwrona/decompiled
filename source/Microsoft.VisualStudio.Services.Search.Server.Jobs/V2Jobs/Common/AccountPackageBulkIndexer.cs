// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.V2Jobs.Common.AccountPackageBulkIndexer
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
  public static class AccountPackageBulkIndexer
  {
    private const int TracePoint = 1080600;
    private static string s_traceArea = "Indexing Pipeline";
    private static string s_traceLayer = "Indexer";

    public static void TriggerIndexing(ExecutionContext executionContext, bool queueAccountFaultIn = false)
    {
      IVssRequestContext requestContext = executionContext.RequestContext;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        if (!requestContext.IsSearchConfigured() || !requestContext.IsPackageIndexingEnabled())
          return;
        AccountPackageBulkIndexer.CheckJobDefinitionAndQueueAccountFaultInJob(executionContext);
      }
      else
      {
        if (!(!requestContext.IsPackageIndexingEnabled() | queueAccountFaultIn))
          return;
        requestContext.SetFeatureFlagState("Search.Server.Package.Indexing", FeatureAvailabilityState.On, new TraceMetaData(1080600, AccountPackageBulkIndexer.s_traceArea, AccountPackageBulkIndexer.s_traceLayer));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080600, AccountPackageBulkIndexer.s_traceArea, AccountPackageBulkIndexer.s_traceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Queuing PackageAccountFaultIn job for hostId {0}", (object) requestContext.GetCollectionID())));
        requestContext.QueueDelayedNamedJob(JobConstants.PackageAccountFaultInJobId, executionContext.ServiceSettings.JobSettings.AccountFaultInJobDelayInSec, JobPriorityLevel.Normal);
        executionContext.ExecutionTracerContext.PublishKpi("SearchExtensionAccountFaultInJobTrigger", "Indexing Pipeline", (double) PackageEntityType.GetInstance().ID);
      }
    }

    private static void CheckJobDefinitionAndQueueAccountFaultInJob(
      ExecutionContext executionContext)
    {
      AccountPackageBulkIndexer.CreateAccountFaultInJobDefinitionIfDoesNotExists(executionContext.RequestContext);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080600, AccountPackageBulkIndexer.s_traceArea, AccountPackageBulkIndexer.s_traceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Queuing PackageAccountFaultInJob for hostId {0}", (object) executionContext.RequestContext.GetCollectionID())));
      executionContext.RequestContext.QueueDelayedNamedJob(JobConstants.PackageAccountFaultInJobId, executionContext.ServiceSettings.JobSettings.AccountFaultInJobDelayInSec, JobPriorityLevel.Normal);
      executionContext.ExecutionTracerContext.PublishKpi("SearchExtensionAccountFaultInJobTrigger", "Indexing Pipeline", (double) PackageEntityType.GetInstance().ID, true);
    }

    private static void CreateAccountFaultInJobDefinitionIfDoesNotExists(
      IVssRequestContext requestContext)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      if (service.QueryJobDefinition(requestContext, JobConstants.PackageAccountFaultInJobId) != null)
        return;
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(JobConstants.PackageAccountFaultInJobId, "Package Account Fault in Job", "Microsoft.VisualStudio.Services.Search.Server.Jobs.PackageAccountFaultInJob")
      {
        DisableDuringUpgrade = true,
        PriorityClass = JobPriorityClass.AboveNormal,
        EnabledState = TeamFoundationJobEnabledState.Enabled
      };
      service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
      {
        foundationJobDefinition
      });
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080600, AccountPackageBulkIndexer.s_traceArea, AccountPackageBulkIndexer.s_traceLayer, "Created a new Job Definition for Microsoft.VisualStudio.Services.Search.Server.Jobs.PackageAccountFaultInJob.");
    }
  }
}
