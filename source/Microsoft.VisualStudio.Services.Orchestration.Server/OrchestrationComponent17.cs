// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent17
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationComponent17 : OrchestrationComponent16
  {
    internal override IList<OrchestrationHubMessageCount> GetOrchestrationHubMessageCounts(
      IEnumerable<string> hubNames,
      int? sqlCommandTimeout = null)
    {
      if (!sqlCommandTimeout.HasValue)
        this.PrepareStoredProcedure("prc_GetOrchestrationHubsMessageCount");
      else
        this.PrepareStoredProcedure("prc_GetOrchestrationHubsMessageCount", sqlCommandTimeout.Value);
      this.BindStringTable(nameof (hubNames), hubNames);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationHubMessageCount>((ObjectBinder<OrchestrationHubMessageCount>) new OrchestrationHubMessageCountBinder());
        return (IList<OrchestrationHubMessageCount>) resultCollection.GetCurrent<OrchestrationHubMessageCount>().Items;
      }
    }

    public override async Task<RunnableOrchestrationSessionsBatch> GetRunnableSessionsV2Async(
      string hubName,
      OrchestrationHubDispatcherType dispatcherType,
      int maxSessionCount)
    {
      OrchestrationComponent17 orchestrationComponent17 = this;
      orchestrationComponent17.PrepareStoredProcedure("prc_GetRunnableOrchestrationSessionsV2");
      orchestrationComponent17.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      orchestrationComponent17.BindByte("@dispatcherType", (byte) dispatcherType);
      orchestrationComponent17.BindInt("@maxSessionCount", maxSessionCount);
      RunnableOrchestrationSessionsBatch runnableSessionsV2Async;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await orchestrationComponent17.ExecuteReaderAsync(), orchestrationComponent17.ProcedureName, orchestrationComponent17.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationSession>(orchestrationComponent17.GetOrchestrationSessionBinder());
        resultCollection.AddBinder<OrchestrationMessage>((ObjectBinder<OrchestrationMessage>) new OrchestrationMessage2Binder());
        resultCollection.AddBinder<OrchestrationSessionNextRun>((ObjectBinder<OrchestrationSessionNextRun>) new OrchestrationNextSessionRunBuinder());
        Dictionary<string, RunnableOrchestrationSession> dictionary = resultCollection.GetCurrent<OrchestrationSession>().Items.ToDictionary<OrchestrationSession, string, RunnableOrchestrationSession>((System.Func<OrchestrationSession, string>) (x => x.SessionId), (System.Func<OrchestrationSession, RunnableOrchestrationSession>) (x => new RunnableOrchestrationSession()
        {
          Session = x
        }), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        resultCollection.NextResult();
        foreach (OrchestrationMessage orchestrationMessage in resultCollection.GetCurrent<OrchestrationMessage>().Items)
        {
          RunnableOrchestrationSession orchestrationSession;
          if (dictionary.TryGetValue(orchestrationMessage.SessionId, out orchestrationSession))
            orchestrationSession.Messages.Add(orchestrationMessage);
        }
        resultCollection.NextResult();
        OrchestrationSessionNextRun orchestrationSessionNextRun = resultCollection.GetCurrent<OrchestrationSessionNextRun>().Items.FirstOrDefault<OrchestrationSessionNextRun>();
        runnableSessionsV2Async = new RunnableOrchestrationSessionsBatch()
        {
          Sessions = dictionary.Values.ToList<RunnableOrchestrationSession>(),
          NextSessionRun = orchestrationSessionNextRun
        };
      }
      return runnableSessionsV2Async;
    }
  }
}
