// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.FaultStateHandler.StateHandlerFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Definitions;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.FaultStates;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.FaultStateHandler
{
  public static class StateHandlerFactory
  {
    public static IStateHandler GetStateHandler(
      FaultManagementSettings settings,
      TransientFaultState state)
    {
      if (state == null)
        throw new ArgumentNullException(nameof (state));
      IStateHandler stateHandler = (IStateHandler) null;
      if (state.ServiceFaultState.Equals((object) ServiceFaultState.Closed))
        stateHandler = (IStateHandler) new ClosedStateHandler(settings);
      if (state.ServiceFaultState.Equals((object) ServiceFaultState.PartiallyOpen))
        stateHandler = (IStateHandler) new PartiallyOpenStateHandler(settings);
      return stateHandler;
    }

    public static bool IsSupported(IndexerFaultSeverity severity) => severity == IndexerFaultSeverity.Healthy || severity == IndexerFaultSeverity.Medium || severity == IndexerFaultSeverity.Critical;
  }
}
