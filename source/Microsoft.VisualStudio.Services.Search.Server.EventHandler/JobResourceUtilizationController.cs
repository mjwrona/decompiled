// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.JobResourceUtilizationController
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler.ResourceCalculators;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  [Export(typeof (IJobResourceUtilizationController))]
  public sealed class JobResourceUtilizationController : IJobResourceUtilizationController
  {
    private const int TracePoint = 1080540;
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "Job";
    private readonly TraceMetaData m_traceMetaData = new TraceMetaData(1080540, "Indexing Pipeline", "Job");
    private int m_callbackDelayInMinutes;
    private const int ElapsedTimeInMinsForFallbackReset = 120;
    [StaticSafe]
    private static int s_failureCount;
    private BaseJobResourcesCalculator m_resourceCalculator;
    private readonly SimpleJobResourcesCalculator m_jobResourcesCalculator = new SimpleJobResourcesCalculator();
    private readonly NullJobResourcesCalculator m_nullJobResourcesCalculator = new NullJobResourcesCalculator();
    private ResourceCalculatorRegistryService m_registryService;

    [Info("InternalForTestPurpose")]
    internal bool IsFallbackEnabled { get; set; }

    [Info("InternalForTestPurpose")]
    internal DateTime FailureEncounterTime { get; set; }

    public IEntityType[] SupportedEntityTypes => new IEntityType[8]
    {
      (IEntityType) CodeEntityType.GetInstance(),
      (IEntityType) ProjectRepoEntityType.GetInstance(),
      (IEntityType) TenantCodeEntityType.GetInstance(),
      (IEntityType) TenantWikiEntityType.GetInstance(),
      (IEntityType) WorkItemEntityType.GetInstance(),
      (IEntityType) FileEntityType.GetInstance(),
      (IEntityType) WikiEntityType.GetInstance(),
      (IEntityType) PackageEntityType.GetInstance()
    };

    public void Initialize(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
      this.FailureEncounterTime = DateTime.UtcNow;
      this.m_registryService = requestContext.GetService<ResourceCalculatorRegistryService>();
      this.m_jobResourcesCalculator.Initialize(requestContext);
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment || requestContext.IsFeatureEnabled("Search.Server.JobResourceUtilizationControl"))
      {
        this.IsFallbackEnabled = this.m_registryService.RegistrySettings.IsFallbackEnabled;
        this.m_resourceCalculator = (BaseJobResourcesCalculator) this.m_jobResourcesCalculator;
        this.m_callbackDelayInMinutes = this.m_registryService.RegistrySettings.CallbackDelayInMins;
        this.FetchJobQueueData(requestContext);
      }
      else
      {
        this.IsFallbackEnabled = true;
        this.m_resourceCalculator = (BaseJobResourcesCalculator) this.m_nullJobResourcesCalculator;
      }
    }

    public JobResourcesResponse GetResourcesToQueueJobs(
      ExecutionContext executionContext,
      JobResourcesRequest jobResourcesRequest)
    {
      this.m_resourceCalculator = this.ObtainResourceCalculatorObject(executionContext.RequestContext);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Using {0} calculator object to GetResources and QueueJobs", (object) this.m_resourceCalculator.GetType())));
      return this.m_resourceCalculator.GetResourcesToQueueJobs(executionContext, jobResourcesRequest);
    }

    [Info("InternalForTestPurpose")]
    internal void FetchJobQueueData(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080540, "Indexing Pipeline", "Job", nameof (FetchJobQueueData));
      try
      {
        this.m_resourceCalculator = this.ObtainResourceCalculatorObject(requestContext);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Using {0} calculator object to RefreshCurrentResourceConsumptionData", (object) this.m_resourceCalculator.GetType())));
        this.m_resourceCalculator.RefreshCurrentResourceConsumptionData(requestContext);
      }
      catch (Exception ex)
      {
        ++JobResourceUtilizationController.s_failureCount;
        if (JobResourceUtilizationController.s_failureCount >= 2)
        {
          this.IsFallbackEnabled = true;
          this.FailureEncounterTime = DateTime.UtcNow;
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(this.m_traceMetaData, ex);
      }
      finally
      {
        ITeamFoundationTaskService service = IVssRequestContextExtensions.ElevateAsNeeded(requestContext).GetService<ITeamFoundationTaskService>();
        this.m_callbackDelayInMinutes = this.m_registryService.RegistrySettings.CallbackDelayInMins;
        TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new Microsoft.TeamFoundation.Framework.Server.TeamFoundationTaskCallback(this.TeamFoundationTaskCallback), (object) null, DateTime.UtcNow.AddMinutes((double) this.m_callbackDelayInMinutes), 0);
        IVssRequestContext requestContext1 = requestContext;
        TeamFoundationTask task = teamFoundationTask;
        service.AddTask(requestContext1, task);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080540, "Indexing Pipeline", "Job", nameof (FetchJobQueueData));
      }
    }

    [Info("InternalForTestPurpose")]
    internal void TeamFoundationTaskCallback(IVssRequestContext requestContext, object args)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080540, "Indexing Pipeline", "Job", nameof (TeamFoundationTaskCallback));
      try
      {
        this.FetchJobQueueData(requestContext);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080540, "Indexing Pipeline", "Job", nameof (TeamFoundationTaskCallback));
      }
    }

    [Info("InternalForTestPurpose")]
    internal BaseJobResourcesCalculator ObtainResourceCalculatorObject(
      IVssRequestContext requestContext)
    {
      if ((int) (DateTime.UtcNow - this.FailureEncounterTime).TotalMinutes > 120 && JobResourceUtilizationController.s_failureCount >= 2)
      {
        this.IsFallbackEnabled = this.m_registryService.RegistrySettings.IsFallbackEnabled;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Resetting failure count to 0 from {0} with the fallback status as: {1}", (object) JobResourceUtilizationController.s_failureCount, (object) this.IsFallbackEnabled)));
        JobResourceUtilizationController.s_failureCount = 0;
      }
      this.m_resourceCalculator = requestContext.ExecutionEnvironment.IsDevFabricDeployment || requestContext.IsFeatureEnabled("Search.Server.JobResourceUtilizationControl") && !this.IsFallbackEnabled ? (BaseJobResourcesCalculator) this.m_jobResourcesCalculator : (BaseJobResourcesCalculator) this.m_nullJobResourcesCalculator;
      return this.m_resourceCalculator;
    }
  }
}
