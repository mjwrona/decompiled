// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.IndexerV1FaultService
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Definitions;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.FaultStateHandler;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement
{
  public sealed class IndexerV1FaultService : BaseIndexerFaultService
  {
    public IndexerV1FaultService(IVssRequestContext requestContext, IFaultStore faultStore)
      : this(requestContext, faultStore, (IRequestStore) new RegistryServiceRequestStore(requestContext))
    {
    }

    public IndexerV1FaultService(
      IVssRequestContext requestContext,
      IFaultStore faultStore,
      IRequestStore requestStore)
      : base(requestContext, faultStore, requestStore)
    {
    }

    public override void SetFault(IndexerFault fault)
    {
      if (fault == null)
        throw new ArgumentNullException(nameof (fault));
      if (!StateHandlerFactory.IsSupported(fault.Severity))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080653, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("SetFault was a No-Op for Fault '{0}' as the Fault severity is unsupported.'", (object) fault.Severity)));
      }
      else
      {
        IndexerFaultStatus faultStatus = this.GetFaultStatus();
        if (faultStatus.Severity.Equals((object) IndexerFaultSeverity.Blocked))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080653, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("SetFault was a No-Op for Fault '{0}' as the Fault is set to '{1}'", (object) fault.Severity, (object) faultStatus.Severity)));
        }
        else
        {
          TransientFaultState currentTransientFaultState = this.GetTransientFaultState() ?? new TransientFaultState();
          this.ProcessFault(faultStatus, currentTransientFaultState, fault);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080653, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("Fault was set to '{0}'", (object) fault.Severity)));
        }
      }
    }

    public override void ResetFault(IndexerFault fault) => throw new NotImplementedException();

    public override void ResetFaults()
    {
      IndexerFaultStatus faultStatus = this.GetFaultStatus();
      if (faultStatus.Severity.Equals((object) IndexerFaultSeverity.Blocked))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080653, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("ResetFaults was a No-Op as the Fault is set to '{0}'", (object) faultStatus.Severity)));
      }
      else
      {
        if (!faultStatus.IsActive)
          return;
        TransientFaultState transientFaultState = this.GetTransientFaultState();
        if (transientFaultState == null)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080652, "Indexing Pipeline", "FaultManagement", "ResetFaults failed since TransientFaultState is null.");
        }
        else
        {
          this.ProcessFault(faultStatus, transientFaultState, new IndexerFault()
          {
            Severity = IndexerFaultSeverity.Healthy
          });
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080653, "Indexing Pipeline", "FaultManagement", "ResetFaults successful.");
        }
      }
    }

    public override IndexerFaultStatus GetFaultStatus()
    {
      IndexerFaultStatus status;
      this.FaultStore.TryGetFaultStatus(out status);
      if (status == null)
        status = new IndexerFaultStatus();
      return status;
    }

    public override bool Request(string feature, IndexerFaultSeverity severity, out TimeSpan delay)
    {
      delay = new TimeSpan(0L);
      if (!this.RequestContext.IsFeatureEnabled("Search.Server.FaultManagement"))
        return true;
      bool flag = true;
      try
      {
        string request = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}", (object) feature, (object) severity.ToString());
        RequestCounter requestCounter;
        if (this.RequestStore.TryGetRequestCounter(request, out requestCounter))
        {
          DateTime windowStartTime = requestCounter.WindowStartTime;
          int requestCount = requestCounter.RequestCount;
          if (DateTime.UtcNow - windowStartTime > new TimeSpan(1, 0, 0))
          {
            if (!this.RequestStore.ResetRequestCounter(request))
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("FaultManagementRequestRecordError", "Indexing Pipeline", 1.0);
          }
          else
          {
            int failureRequestLimit;
            int requestDelayInSec;
            if (severity == IndexerFaultSeverity.Medium)
            {
              failureRequestLimit = this.Settings.MidFailureRequestLimit;
              requestDelayInSec = this.Settings.MidFailureRequestDelayInSec;
            }
            else
            {
              failureRequestLimit = this.Settings.CritFailureRequestLimit;
              requestDelayInSec = this.Settings.CritFailureRequestDelayInSec;
            }
            if (requestCount >= failureRequestLimit)
            {
              Random random = new Random(DateTime.Now.Ticks.GetHashCode());
              delay = new TimeSpan(0, 0, random.Next((int) (0.8 * (double) requestDelayInSec), requestDelayInSec));
              flag = false;
            }
            else if (!this.RequestStore.IncrementRequestCounter(request, ++requestCounter.RequestCount))
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("FaultManagementRequestRecordError", "Indexing Pipeline", 1.0);
          }
        }
        else if (!this.RequestStore.ResetRequestCounter(request))
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("FaultManagementRequestRecordError", "Indexing Pipeline", 1.0);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080652, "Indexing Pipeline", "FaultManagement", FormattableString.Invariant(FormattableStringFactory.Create("Request() failed - {0}", (object) ex)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("FaultManagementRequestOpError", "Indexing Pipeline", 1.0);
      }
      return flag;
    }

    private TransientFaultState GetTransientFaultState()
    {
      TransientFaultState state;
      this.FaultStore.TryGetTransientFaultState(out state);
      return state;
    }

    private void ProcessFault(
      IndexerFaultStatus currentFaultStatus,
      TransientFaultState currentTransientFaultState,
      IndexerFault incomingFault)
    {
      IStateHandler stateHandler = StateHandlerFactory.GetStateHandler(this.Settings, currentTransientFaultState);
      if (incomingFault.Severity.Equals((object) IndexerFaultSeverity.Critical) || incomingFault.Severity.Equals((object) IndexerFaultSeverity.Medium))
        stateHandler.Failure(currentFaultStatus, incomingFault, currentTransientFaultState);
      else if (incomingFault.Severity.Equals((object) IndexerFaultSeverity.Healthy))
        stateHandler.Success(currentFaultStatus, incomingFault, currentTransientFaultState);
      if (this.FaultStore.TryAddOrUpdate(currentFaultStatus, currentTransientFaultState))
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("FaultManagementStateRecordError", "Indexing Pipeline", 1.0);
    }
  }
}
