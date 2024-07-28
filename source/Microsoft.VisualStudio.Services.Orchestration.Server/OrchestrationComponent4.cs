// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent4
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationComponent4 : OrchestrationComponent3
  {
    public override OrchestrationSession GetOrchestrationSession(string hubName, string sessionId)
    {
      this.PrepareStoredProcedure("prc_GetOrchestrationSession");
      this.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@sessionIdentifier", sessionId, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationSession>(this.GetOrchestrationSessionBinder());
        return resultCollection.GetCurrent<OrchestrationSession>().FirstOrDefault<OrchestrationSession>();
      }
    }
  }
}
