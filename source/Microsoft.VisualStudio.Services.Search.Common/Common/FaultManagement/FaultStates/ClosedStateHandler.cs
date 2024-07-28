// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.FaultStates.ClosedStateHandler
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Definitions;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.FaultStateHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.FaultStates
{
  public class ClosedStateHandler : IStateHandler
  {
    private readonly FaultManagementSettings m_settings;

    public ClosedStateHandler(FaultManagementSettings settings) => this.m_settings = settings;

    public void Failure(
      IndexerFaultStatus indexerFaultStatus,
      IndexerFault incomingFault,
      TransientFaultState transientFaultState)
    {
      if (indexerFaultStatus == null)
        throw new ArgumentNullException(nameof (indexerFaultStatus));
      if (incomingFault == null)
        throw new ArgumentNullException(nameof (incomingFault));
      if (transientFaultState == null)
        throw new ArgumentNullException(nameof (transientFaultState));
      if (transientFaultState.IsExpired(this.m_settings))
        ClosedStateHandler.StateExpired(indexerFaultStatus, transientFaultState);
      ClosedStateHandler.VerifyHealthStatus(indexerFaultStatus);
      if (transientFaultState.FailureCount == 0)
        transientFaultState.StateTimeStamp = DateTime.UtcNow;
      ++transientFaultState.FailureCount;
      transientFaultState.StateSeverity = incomingFault.Severity;
      indexerFaultStatus.IsActive = true;
      this.StateChangeCheck(indexerFaultStatus, incomingFault, transientFaultState);
    }

    public void Success(
      IndexerFaultStatus indexerFaultStatus,
      IndexerFault incomingFault,
      TransientFaultState transientFaultState)
    {
      if (transientFaultState == null)
        throw new ArgumentNullException(nameof (transientFaultState));
      if (transientFaultState.IsExpired(this.m_settings))
        ClosedStateHandler.StateExpired(indexerFaultStatus, transientFaultState);
      else
        ClosedStateHandler.VerifyHealthStatus(indexerFaultStatus);
    }

    private static void VerifyHealthStatus(IndexerFaultStatus indexerFaultStatus)
    {
      if (indexerFaultStatus.Severity == IndexerFaultSeverity.Healthy)
        return;
      Tracer.PublishKpi("HealthyClosedForceStateTransition", "Indexing Pipeline", 1.0);
      indexerFaultStatus.ResetSeverity();
    }

    private static void StateExpired(
      IndexerFaultStatus indexerFaultStatus,
      TransientFaultState transientFaultState)
    {
      Tracer.PublishKpi("ClosedStateExpired", "Indexing Pipeline", 1.0);
      transientFaultState.Reset();
      indexerFaultStatus.Reset();
    }

    private void StateChangeCheck(
      IndexerFaultStatus indexerFaultStatus,
      IndexerFault incomingFault,
      TransientFaultState transientFaultState)
    {
      if (transientFaultState.FailureCount <= this.m_settings.MaxFailureCount && !incomingFault.Severity.Equals((object) IndexerFaultSeverity.Critical))
        return;
      transientFaultState.StateTimeStamp = DateTime.UtcNow;
      transientFaultState.ServiceFaultState = ServiceFaultState.PartiallyOpen;
      transientFaultState.SuccessCount = 0;
      indexerFaultStatus.Severity = incomingFault.Severity;
      ClosedStateHandler.TraceStateTransition(incomingFault.Severity);
    }

    private static void TraceStateTransition(IndexerFaultSeverity severity)
    {
      if (severity.Equals((object) IndexerFaultSeverity.Critical))
      {
        Tracer.PublishKpi("CritialPartiallyOpenStateTransition", "Indexing Pipeline", 1.0);
      }
      else
      {
        if (!severity.Equals((object) IndexerFaultSeverity.Medium))
          return;
        Tracer.PublishKpi("MediumPartiallyOpenStateTransition", "Indexing Pipeline", 1.0);
      }
    }
  }
}
