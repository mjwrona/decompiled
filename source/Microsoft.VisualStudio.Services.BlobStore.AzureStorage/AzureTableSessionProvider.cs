// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.AzureTableSessionProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public class AzureTableSessionProvider : ISessionProvider
  {
    internal readonly ITableClientFactory tableClientFactory;
    private readonly LocationMode? locationMode;
    private const int MaxCompareSwapRetries = 5;

    public bool ProviderRequireVss => this.tableClientFactory.RequiresVssRequestContext;

    public AzureTableSessionProvider(
      ITableClientFactory tableClientFactory,
      LocationMode? locationMode)
    {
      this.tableClientFactory = tableClientFactory;
      this.locationMode = new LocationMode?(locationMode.GetValueOrDefault());
    }

    public async Task InitializeAsync(VssRequestPump.Processor processor)
    {
      LocationMode? locationMode1 = this.locationMode;
      LocationMode locationMode2 = LocationMode.SecondaryOnly;
      if (locationMode1.GetValueOrDefault() == locationMode2 & locationMode1.HasValue)
        return;
      int num;
      await NonSwallowingActionBlock.Create<ITable>((Func<ITable, Task>) (async table => num = await table.CreateIfNotExistsAsync(processor).ConfigureAwait(false) ? 1 : 0), processor.CancellationToken).SendAllAndCompleteSingleBlockNetworkAsync<ITable>(this.tableClientFactory.GetAllTables(), processor.CancellationToken).ConfigureAwait(false);
    }

    public async Task CreateSessionAsync(
      VssRequestPump.Processor processor,
      Guid sessionId,
      Guid owner,
      SessionState status,
      DateTime expiration,
      DedupIdentifier contentId,
      List<Part> parts)
    {
      SessionTableEntity entity = new SessionTableEntity(sessionId, owner, status, expiration, contentId, parts);
      TableOperationResult tableOperationResult = await this.GetTable(entity.Id).ExecuteAsync(processor, TableOperationDescriptor.Insert((ITableEntity) entity)).ConfigureAwait(false);
      switch (tableOperationResult.HttpStatusCode)
      {
        case HttpStatusCode.Created:
          break;
        case HttpStatusCode.NoContent:
          break;
        default:
          throw new InvalidSessionOperationException(string.Format("{0}: {1}", (object) nameof (CreateSessionAsync), (object) tableOperationResult.HttpStatusCode));
      }
    }

    public async Task<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session> GetSessionAsync(
      VssRequestPump.Processor processor,
      Guid sessionId)
    {
      PartitionScanQuery<SessionTableEntity> partitionScanQuery = new PartitionScanQuery<SessionTableEntity>((StringColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(sessionId.ToString()), maxRowsToTake: new int?(1));
      SessionTableEntity sessionTableEntity = await this.GetTable(sessionId).QuerySingleOrDefaultAsync<SessionTableEntity>(processor, (Query<SessionTableEntity>) partitionScanQuery).ConfigureAwait(false);
      if (sessionTableEntity == null)
        throw new SessionOrPartNotFoundException(string.Format("Session {0} not found.", (object) sessionId));
      return new Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session(sessionTableEntity.Id, sessionTableEntity.Owner, sessionTableEntity.Status, sessionTableEntity.Expiration, sessionTableEntity.ContentId, JsonSerializer.Deserialize<List<Part>>(sessionTableEntity.Parts));
    }

    public async Task UpdateSessionAsync(
      VssRequestPump.Processor processor,
      Guid sessionId,
      Guid owner,
      SessionState status,
      DateTime expiration,
      DedupIdentifier contentId,
      List<Part> parts)
    {
      await this.QueryAndPerformOperationAsync(processor, sessionId, (Func<ITable, SessionTableEntity, Task<HttpStatusCode>>) (async (table, entity) =>
      {
        entity.Status = status;
        entity.ContentId = contentId;
        entity.Parts = JsonSerializer.Serialize<List<Part>>(parts);
        return (await table.ExecuteAsync(processor, TableOperationDescriptor.Merge((ITableEntity) entity)).ConfigureAwait(false)).HttpStatusCode;
      })).ConfigureAwait(false);
    }

    public async Task DeleteSessionAsync(VssRequestPump.Processor processor, Guid sessionId) => await this.QueryAndPerformOperationAsync(processor, sessionId, (Func<ITable, SessionTableEntity, Task<HttpStatusCode>>) (async (table, entity) => (await table.ExecuteAsync(processor, TableOperationDescriptor.Delete((ITableEntity) entity)).ConfigureAwait(false)).HttpStatusCode)).ConfigureAwait(false);

    private async Task QueryAndPerformOperationAsync(
      VssRequestPump.Processor processor,
      Guid sessionId,
      Func<ITable, SessionTableEntity, Task<HttpStatusCode>> operation)
    {
      string partitionKey = sessionId.ToString();
      PartitionScanQuery<SessionTableEntity> query = new PartitionScanQuery<SessionTableEntity>((StringColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(partitionKey), maxRowsToTake: new int?(1));
      ITable table = this.GetTable(sessionId);
      int attempt = 0;
      bool flag;
      do
      {
        SessionTableEntity sessionTableEntity = await table.QuerySingleOrDefaultAsync<SessionTableEntity>(processor, (Query<SessionTableEntity>) query).ConfigureAwait(false);
        if (sessionTableEntity != null)
        {
          if (sessionTableEntity.PartitionKey != partitionKey)
            throw new InvalidSessionOperationException("The query returned invalid results. partitionKey: " + sessionTableEntity.PartitionKey + ", rowKey: " + sessionTableEntity.RowKey);
          HttpStatusCode httpStatusCode = await operation(table, sessionTableEntity).ConfigureAwait(false);
          int num;
          switch (httpStatusCode)
          {
            case HttpStatusCode.NoContent:
              goto label_14;
            case HttpStatusCode.NotFound:
              throw new SessionOrPartNotFoundException(string.Format("Session {0} not found.", (object) sessionId));
            case HttpStatusCode.PreconditionFailed:
              num = ++attempt < 5 ? 1 : 0;
              break;
            default:
              num = 0;
              break;
          }
          flag = num != 0;
          if (!flag)
            throw new InvalidSessionOperationException(string.Format("{0}: {1}", (object) nameof (QueryAndPerformOperationAsync), (object) httpStatusCode));
        }
        else
          goto label_2;
      }
      while (flag);
      goto label_6;
label_2:
      partitionKey = (string) null;
      query = (PartitionScanQuery<SessionTableEntity>) null;
      table = (ITable) null;
      return;
label_14:
      partitionKey = (string) null;
      query = (PartitionScanQuery<SessionTableEntity>) null;
      table = (ITable) null;
      return;
label_6:
      partitionKey = (string) null;
      query = (PartitionScanQuery<SessionTableEntity>) null;
      table = (ITable) null;
    }

    private ITable GetTable(Guid sessionId) => this.tableClientFactory.GetTable(sessionId.ToString());
  }
}
