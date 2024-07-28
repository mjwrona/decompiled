// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.ArtifactsServiceBase
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public abstract class ArtifactsServiceBase : IArtifactService, IVssFrameworkService
  {
    private const string ProductTraceServiceLayer = "Service";
    private TraceData traceDataPrivate;

    protected Guid ServiceStartHostId { get; private set; } = Guid.Empty;

    protected TraceData traceData
    {
      get
      {
        if (this.traceDataPrivate == null)
          this.traceDataPrivate = new TraceData()
          {
            Area = this.ProductTraceArea,
            Layer = "Service"
          };
        return this.traceDataPrivate;
      }
    }

    protected abstract string ProductTraceArea { get; }

    public ManifestCounters Counters { get; private set; }

    public bool IsUserPCA(IVssRequestContext requestContext) => requestContext.GetService<IdentityService>().IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, requestContext.GetUserIdentity().Descriptor);

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.ServiceStartHostId = systemRequestContext.ServiceHost.InstanceId;
      this.InitializeCounters(systemRequestContext);
      this.ServiceStart(systemRequestContext);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => this.ServiceEnd(systemRequestContext);

    protected virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    protected virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    protected virtual ManifestCounters GetManifestCounters(
      bool windowsCountersEnabledInConfiguration)
    {
      return (ManifestCounters) null;
    }

    private static void IncrementHeartbeatCounter(
      IVssRequestContext requestContext,
      object taskArgs)
    {
      ManifestCounters manifestCounters = (ManifestCounters) taskArgs;
      lock (manifestCounters)
      {
        PerfCounter heartbeatCounter = manifestCounters.HeartbeatCounter;
        if (heartbeatCounter.RawValue <= 0L)
          heartbeatCounter.Increment();
        else
          heartbeatCounter.Decrement();
      }
    }

    private void InitializeCounters(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ManifestCounters manifestCounters = this.GetManifestCounters(PerfCounterConfiguration.AreWindowsCountersEnabledInThisScaleUnit(vssRequestContext));
      if (manifestCounters == null)
        return;
      if (manifestCounters.HeartbeatCounter != null)
      {
        ITeamFoundationTaskService service = vssRequestContext.GetService<ITeamFoundationTaskService>();
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        TeamFoundationTask teamFoundationTask = new TeamFoundationTask(ArtifactsServiceBase.\u003C\u003EO.\u003C0\u003E__IncrementHeartbeatCounter ?? (ArtifactsServiceBase.\u003C\u003EO.\u003C0\u003E__IncrementHeartbeatCounter = new TeamFoundationTaskCallback(ArtifactsServiceBase.IncrementHeartbeatCounter)), (object) manifestCounters, PerfCounterConfiguration.HeartbeatIntervalMs);
        IVssRequestContext requestContext1 = vssRequestContext;
        TeamFoundationTask task = teamFoundationTask;
        service.AddTask(requestContext1, task);
      }
      this.Counters = manifestCounters;
    }
  }
}
