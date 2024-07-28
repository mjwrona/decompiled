// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent8
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationComponent8 : OrchestrationComponent7
  {
    private static readonly SqlMetaData[] typ_OrchestrationMessageIdTable2 = new SqlMetaData[1]
    {
      new SqlMetaData("MessageId", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] typ_OrchestrationSessionTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("SessionId", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Completed", SqlDbType.Bit),
      new SqlMetaData("CompressionType", SqlDbType.TinyInt),
      new SqlMetaData("State", SqlDbType.VarBinary, -1L)
    };
    private static readonly SqlMetaData[] typ_OrchestrationStateTable2 = new SqlMetaData[13]
    {
      new SqlMetaData("InstanceIdentifier", SqlDbType.NVarChar, 260L),
      new SqlMetaData("ExecutionIdentifier", SqlDbType.NVarChar, 260L),
      new SqlMetaData("ParentInstanceIdentifier", SqlDbType.NVarChar, 260L),
      new SqlMetaData("ParentExecutionIdentifier", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Name", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Version", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("CreatedOn", SqlDbType.DateTime),
      new SqlMetaData("CompletedOn", SqlDbType.DateTime),
      new SqlMetaData("Size", SqlDbType.BigInt),
      new SqlMetaData("CompressedSize", SqlDbType.BigInt),
      new SqlMetaData("Output", SqlDbType.NVarChar, -1L),
      new SqlMetaData("State", SqlDbType.NVarChar, -1L)
    };

    public override Task CompleteActivityMessageAsync(
      string hubName,
      string dispatcherType,
      long messageIdToDelete,
      OrchestrationMessage newMessage)
    {
      this.PrepareStoredProcedure("prc_CompleteOrchestrationActivity");
      this.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindLong("@messageIdToDelete", messageIdToDelete);
      this.BindOrchestrationMessageTable("@orchestrationMessages", (IEnumerable<OrchestrationMessage>) new OrchestrationMessage[1]
      {
        newMessage
      });
      return (Task) this.ExecuteNonQueryAsync();
    }

    public override async Task<IList<OrchestrationMessage>> GetActivityMessagesAsync(
      string hubName,
      string dispatcherType,
      int maxMessageCount,
      long? lastMessageId)
    {
      OrchestrationComponent8 orchestrationComponent8 = this;
      orchestrationComponent8.PrepareStoredProcedure("prc_GetRunnableActivityMessages");
      orchestrationComponent8.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      orchestrationComponent8.BindInt("@maxMessageCount", maxMessageCount);
      if (lastMessageId.HasValue)
        orchestrationComponent8.BindLong("@lastMessageId", lastMessageId.Value);
      IList<OrchestrationMessage> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await orchestrationComponent8.ExecuteReaderAsync(), orchestrationComponent8.ProcedureName, orchestrationComponent8.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationMessage>((ObjectBinder<OrchestrationMessage>) new OrchestrationMessageBinder());
        items = (IList<OrchestrationMessage>) resultCollection.GetCurrent<OrchestrationMessage>().Items;
      }
      return items;
    }

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
      OrchestrationComponent8 orchestrationComponent8 = this;
      orchestrationComponent8.TraceEnter(0, nameof (UpdateOrchestrationSessionAsync));
      orchestrationComponent8.PrepareStoredProcedure("prc_UpdateOrchestrationSession");
      orchestrationComponent8.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      orchestrationComponent8.BindByte("@dispatcherType", (byte) dispatcherType);
      orchestrationComponent8.BindOrchestrationSessionTable("@sessionUpdates", sessions);
      orchestrationComponent8.BindOrchestrationMessageIdTable("@messagesToDelete", messagesToDelete);
      orchestrationComponent8.BindOrchestrationMessageTable("@activityMessages", activityMessages);
      orchestrationComponent8.BindOrchestrationMessageTable("@orchestrationMessages", orchestrationMessages);
      orchestrationComponent8.BindOrchestrationStateTable("@stateUpdates", instances);
      orchestrationComponent8.BindBoolean("@ensureSessionExists", ensureSessionExists);
      int num = await orchestrationComponent8.ExecuteNonQueryAsync();
      orchestrationComponent8.TraceLeave(0, nameof (UpdateOrchestrationSessionAsync));
    }

    protected override SqlParameter BindOrchestrationMessageIdTable(
      string parameterName,
      IEnumerable<long> rows)
    {
      return this.BindTable(parameterName, "typ_OrchestrationMessageIdTable2", (rows ?? Enumerable.Empty<long>()).Select<long, SqlDataRecord>(new System.Func<long, SqlDataRecord>(((OrchestrationComponent2) this).ConvertToSqlRecord)));
    }

    protected override SqlParameter BindOrchestrationSessionTable(
      string parameterName,
      IEnumerable<OrchestrationSession> rows)
    {
      return this.BindTable(parameterName, "typ_OrchestrationSessionTable2", (rows ?? Enumerable.Empty<OrchestrationSession>()).Select<OrchestrationSession, SqlDataRecord>(new System.Func<OrchestrationSession, SqlDataRecord>(((OrchestrationComponent2) this).ConvertToSqlRecord)));
    }

    protected override SqlParameter BindOrchestrationStateTable(
      string parameterName,
      IEnumerable<OrchestrationState> rows)
    {
      return this.BindTable(parameterName, "typ_OrchestrationStateTable2", (rows ?? Enumerable.Empty<OrchestrationState>()).Select<OrchestrationState, SqlDataRecord>(new System.Func<OrchestrationState, SqlDataRecord>(((OrchestrationComponent2) this).ConvertToSqlRecord)));
    }
  }
}
