// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent13
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationComponent13 : OrchestrationComponent12
  {
    public override async Task<int> GetActivityMessagesCountAsync(
      string hubName,
      string dispatcherType)
    {
      OrchestrationComponent13 orchestrationComponent13 = this;
      orchestrationComponent13.PrepareStoredProcedure("prc_GetActivityMessagesCount");
      orchestrationComponent13.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      orchestrationComponent13.BindString("@dispatcherType", dispatcherType, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      int messagesCountAsync;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await orchestrationComponent13.ExecuteReaderAsync(), orchestrationComponent13.ProcedureName, orchestrationComponent13.RequestContext))
      {
        SqlColumnBinder column = new SqlColumnBinder("Count");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => column.GetInt32(reader))));
        messagesCountAsync = resultCollection.GetCurrent<int>().FirstOrDefault<int>();
      }
      return messagesCountAsync;
    }
  }
}
