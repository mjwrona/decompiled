// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent10
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationComponent10 : OrchestrationComponent9
  {
    public override IList<OrchestrationState> GetRunningOrchestrationState(
      string hubName,
      string instanceId)
    {
      this.PrepareStoredProcedure("prc_GetRunningOrchestrationState");
      this.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@instanceIdentifier", instanceId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationState>((ObjectBinder<OrchestrationState>) new OrchestrationStateBinder());
        return (IList<OrchestrationState>) resultCollection.GetCurrent<OrchestrationState>().ToArray<OrchestrationState>();
      }
    }
  }
}
