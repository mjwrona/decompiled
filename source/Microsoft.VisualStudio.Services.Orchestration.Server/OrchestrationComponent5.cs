// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent5
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationComponent5 : OrchestrationComponent4
  {
    public override Task UpdateOrchestrationSessionAsync(
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
      this.PrepareStoredProcedure("prc_UpdateOrchestrationSession");
      this.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindByte("@dispatcherType", (byte) dispatcherType);
      this.BindOrchestrationSessionTable("@sessionUpdates", sessions);
      this.BindOrchestrationMessageIdTable("@messagesToDelete", messagesToDelete);
      this.BindOrchestrationMessageTable("@activityMessages", activityMessages);
      this.BindOrchestrationMessageTable("@orchestrationMessages", orchestrationMessages);
      this.BindOrchestrationStateTable("@stateUpdates", instances);
      this.BindBoolean("@ensureSessionExists", ensureSessionExists);
      return (Task) this.ExecuteNonQueryAsync();
    }
  }
}
