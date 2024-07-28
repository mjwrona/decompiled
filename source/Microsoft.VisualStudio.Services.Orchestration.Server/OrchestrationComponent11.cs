// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent11
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
  internal class OrchestrationComponent11 : OrchestrationComponent10
  {
    private static readonly SqlMetaData[] typ_OrchestrationMessageTable2 = new SqlMetaData[6]
    {
      new SqlMetaData("MessageId", SqlDbType.Int),
      new SqlMetaData("SessionId", SqlDbType.NVarChar, 260L),
      new SqlMetaData("DispatcherType", SqlDbType.NVarChar, 260L),
      new SqlMetaData("ScheduledDeliveryTime", SqlDbType.DateTime),
      new SqlMetaData("CompressionType", SqlDbType.TinyInt),
      new SqlMetaData("Content", SqlDbType.VarBinary, -1L)
    };
    private int m_messageId;

    public override async Task<IEnumerable<ActivityDispatcherDescriptor>> AddActivityDispatchersAsync(
      string hubName,
      IEnumerable<ActivityDispatcherDescriptor> dispatchers)
    {
      OrchestrationComponent11 component = this;
      component.PrepareStoredProcedure("prc_AddActivityDispatchers");
      component.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      component.BindStringTable("@dispatcherTypes", (IEnumerable<string>) dispatchers.Select<ActivityDispatcherDescriptor, string>((System.Func<ActivityDispatcherDescriptor, string>) (x => x.Type)).ToList<string>());
      component.BindGuid("@writerId", component.Author);
      IEnumerable<ActivityDispatcherDescriptor> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
      {
        resultCollection.AddBinder<ActivityDispatcherDescriptor>((ObjectBinder<ActivityDispatcherDescriptor>) new ActivityDispatcherDescriptorBinder());
        items = (IEnumerable<ActivityDispatcherDescriptor>) resultCollection.GetCurrent<ActivityDispatcherDescriptor>().Items;
      }
      return items;
    }

    public override Task CompleteActivityMessageAsync(
      string hubName,
      string dispatcherType,
      long messageIdToDelete,
      OrchestrationMessage newMessage)
    {
      this.PrepareStoredProcedure("prc_CompleteOrchestrationActivity");
      this.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@dispatcherType", dispatcherType, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
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
      OrchestrationComponent11 orchestrationComponent11 = this;
      orchestrationComponent11.PrepareStoredProcedure("prc_GetRunnableActivityMessages");
      orchestrationComponent11.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      orchestrationComponent11.BindString("@dispatcherType", dispatcherType, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      orchestrationComponent11.BindInt("@maxMessageCount", maxMessageCount);
      if (lastMessageId.HasValue)
        orchestrationComponent11.BindLong("@lastMessageId", lastMessageId.Value);
      IList<OrchestrationMessage> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await orchestrationComponent11.ExecuteReaderAsync(), orchestrationComponent11.ProcedureName, orchestrationComponent11.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationMessage>((ObjectBinder<OrchestrationMessage>) new OrchestrationMessageBinder());
        items = (IList<OrchestrationMessage>) resultCollection.GetCurrent<OrchestrationMessage>().Items;
      }
      return items;
    }

    public override IEnumerable<OrchestrationHubDescription> GetHubs(string hubName)
    {
      this.PrepareStoredProcedure("prc_GetOrchestrationHub");
      this.BindString("@hubName", hubName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationHubDescription>((ObjectBinder<OrchestrationHubDescription>) new OrchestrationHubDescriptionBinder2());
        resultCollection.AddBinder<ActivityDispatcherDescriptor>((ObjectBinder<ActivityDispatcherDescriptor>) new ActivityDispatcherDescriptorBinder());
        Dictionary<string, OrchestrationHubDescription> dictionary = resultCollection.GetCurrent<OrchestrationHubDescription>().Items.ToDictionary<OrchestrationHubDescription, string>((System.Func<OrchestrationHubDescription, string>) (x => x.HubName));
        resultCollection.NextResult();
        foreach (ActivityDispatcherDescriptor dispatcherDescriptor in resultCollection.GetCurrent<ActivityDispatcherDescriptor>())
          dictionary[dispatcherDescriptor.HubName].ActivityDispatchers[dispatcherDescriptor.Type] = dispatcherDescriptor;
        return (IEnumerable<OrchestrationHubDescription>) dictionary.Values;
      }
    }

    protected override SqlParameter BindOrchestrationMessageTable(
      string parameterName,
      IEnumerable<OrchestrationMessage> rows)
    {
      return this.BindTable(parameterName, "typ_OrchestrationMessageTable2", (rows ?? Enumerable.Empty<OrchestrationMessage>()).Select<OrchestrationMessage, SqlDataRecord>(new System.Func<OrchestrationMessage, SqlDataRecord>(((OrchestrationComponent2) this).ConvertToSqlRecord)));
    }

    protected override SqlDataRecord ConvertToSqlRecord(OrchestrationMessage row)
    {
      SqlDataRecord record = new SqlDataRecord(OrchestrationComponent11.typ_OrchestrationMessageTable2);
      record.SetInt32(0, ++this.m_messageId);
      record.SetString(1, row.SessionId, BindStringBehavior.EmptyStringToNull);
      record.SetString(2, row.DispatcherType, BindStringBehavior.EmptyStringToNull);
      if (!row.ScheduledDeliveryTime.HasValue)
        record.SetDBNull(3);
      else
        record.SetDateTime(3, row.ScheduledDeliveryTime.Value);
      record.SetByte(4, (byte) row.CompressionType);
      record.SetBytes(5, 0L, row.Content, 0, row.Content.Length);
      return record;
    }
  }
}
