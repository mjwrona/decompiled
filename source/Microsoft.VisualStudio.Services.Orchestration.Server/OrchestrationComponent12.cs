// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent12
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationComponent12 : OrchestrationComponent11
  {
    public override async Task UpdateOrchestrationSessionAsync(
      string hubName,
      OrchestrationHubDispatcherType dispatcherType,
      IEnumerable<OrchestrationSession> sessions = null,
      IEnumerable<long> messagesToDelete = null,
      IEnumerable<OrchestrationMessage> activityMessages = null,
      IEnumerable<OrchestrationMessage> orchestrationMessages = null,
      IEnumerable<OrchestrationState> instances = null,
      bool ensureSessionExists = false,
      bool alwaysSaveToHistory = false)
    {
      OrchestrationComponent12 orchestrationComponent12 = this;
      orchestrationComponent12.TraceEnter(0, nameof (UpdateOrchestrationSessionAsync));
      orchestrationComponent12.PrepareStoredProcedure("prc_UpdateOrchestrationSession");
      orchestrationComponent12.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      orchestrationComponent12.BindByte("@dispatcherType", (byte) dispatcherType);
      orchestrationComponent12.BindOrchestrationSessionTable("@sessionUpdates", sessions);
      orchestrationComponent12.BindOrchestrationMessageIdTable("@messagesToDelete", messagesToDelete);
      orchestrationComponent12.BindOrchestrationMessageTable("@activityMessages", activityMessages);
      orchestrationComponent12.BindOrchestrationMessageTable("@orchestrationMessages", orchestrationMessages);
      orchestrationComponent12.BindOrchestrationStateTable("@stateUpdates", instances);
      orchestrationComponent12.BindBoolean("@ensureSessionExists", ensureSessionExists);
      orchestrationComponent12.BindBoolean("@alwaysSaveToHistory", alwaysSaveToHistory);
      int num = await orchestrationComponent12.ExecuteNonQueryAsync();
      orchestrationComponent12.TraceLeave(0, nameof (UpdateOrchestrationSessionAsync));
    }
  }
}
