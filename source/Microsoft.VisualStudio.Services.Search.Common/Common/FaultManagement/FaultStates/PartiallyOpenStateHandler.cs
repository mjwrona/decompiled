// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.FaultStates.PartiallyOpenStateHandler
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Definitions;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.FaultStateHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.FaultStates
{
  public class PartiallyOpenStateHandler : IStateHandler
  {
    private readonly FaultManagementSettings m_settings;

    public PartiallyOpenStateHandler(FaultManagementSettings settings) => this.m_settings = settings;

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
      {
        this.StateExpired(indexerFaultStatus, incomingFault, transientFaultState);
      }
      else
      {
        if (incomingFault.Severity.Equals((object) IndexerFaultSeverity.Critical))
        {
          indexerFaultStatus.Severity = incomingFault.Severity;
          transientFaultState.StateSeverity = incomingFault.Severity;
        }
        transientFaultState.StateTimeStamp = DateTime.UtcNow;
        transientFaultState.SuccessCount = 0;
      }
    }

    public void Success(
      IndexerFaultStatus indexerFaultStatus,
      IndexerFault incomingFault,
      TransientFaultState transientFaultState)
    {
      if (transientFaultState == null)
        throw new ArgumentNullException(nameof (transientFaultState));
      if (transientFaultState.IsExpired(this.m_settings))
      {
        this.StateExpired(indexerFaultStatus, incomingFault, transientFaultState);
      }
      else
      {
        ++transientFaultState.SuccessCount;
        this.StateChangeCheck(indexerFaultStatus, transientFaultState);
      }
    }

    private void StateExpired(
      IndexerFaultStatus indexerFaultStatus,
      IndexerFault incomingFault,
      TransientFaultState transientFaultState)
    {
      Tracer.PublishKpi("PartiallyOpenStateExpired", "Indexing Pipeline", 1.0);
      transientFaultState.Reset();
      indexerFaultStatus.Reset();
      ClosedStateHandler closedStateHandler = new ClosedStateHandler(this.m_settings);
      if (incomingFault.Severity.Equals((object) IndexerFaultSeverity.Critical) || incomingFault.Severity.Equals((object) IndexerFaultSeverity.Medium))
      {
        closedStateHandler.Failure(indexerFaultStatus, incomingFault, transientFaultState);
      }
      else
      {
        if (!incomingFault.Severity.Equals((object) IndexerFaultSeverity.Healthy))
          return;
        closedStateHandler.Success(indexerFaultStatus, incomingFault, transientFaultState);
      }
    }

    private void StateChangeCheck(
      IndexerFaultStatus indexerFaultStatus,
      TransientFaultState transientFaultState)
    {
      if (transientFaultState.SuccessCount <= this.m_settings.MaxSuccessCount)
        return;
      transientFaultState.Reset();
      indexerFaultStatus.Reset();
      Tracer.PublishKpi("HealthyClosedStateTransition", "Indexing Pipeline", 1.0);
    }
  }
}
