// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent14
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationComponent14 : OrchestrationComponent13
  {
    public override void RemovePoisonedOrchestrations(
      IList<string> orchestrationIds,
      TimeSpan? timeout = null)
    {
      if (timeout.HasValue && timeout.Value.Seconds > 0)
        this.PrepareStoredProcedure("prc_RemovePoisonedOrchestrations", timeout.Value.Seconds);
      else
        this.PrepareStoredProcedure("prc_RemovePoisonedOrchestrations");
      this.BindStringTable("@orchestrationIds", (IEnumerable<string>) orchestrationIds);
      this.ExecuteNonQuery();
    }
  }
}
