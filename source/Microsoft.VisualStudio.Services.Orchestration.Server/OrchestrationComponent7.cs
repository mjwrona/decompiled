// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent7
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationComponent7 : OrchestrationComponent6
  {
    public override async Task<int> CleanupOrchestrationStateHistoryAsync(
      DateTime cutOffTime,
      int sqlCommandTimeout)
    {
      OrchestrationComponent7 orchestrationComponent7 = this;
      orchestrationComponent7.TraceEnter(0, nameof (CleanupOrchestrationStateHistoryAsync));
      orchestrationComponent7.PrepareStoredProcedure("prc_CleanUpOrchestrationStateHistory", sqlCommandTimeout);
      orchestrationComponent7.BindDateTime(nameof (cutOffTime), cutOffTime);
      int num = await orchestrationComponent7.ExecuteNonQueryAsync();
      orchestrationComponent7.TraceLeave(0, nameof (CleanupOrchestrationStateHistoryAsync));
      return num;
    }
  }
}
