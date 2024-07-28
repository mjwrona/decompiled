// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseStoreCosmos
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal sealed class DocumentServiceLeaseStoreCosmos : DocumentServiceLeaseStore
  {
    private readonly Container container;
    private readonly string containerNamePrefix;
    private readonly RequestOptionsFactory requestOptionsFactory;
    private string lockETag;

    public DocumentServiceLeaseStoreCosmos(
      Container container,
      string containerNamePrefix,
      RequestOptionsFactory requestOptionsFactory)
    {
      this.container = container;
      this.containerNamePrefix = containerNamePrefix;
      this.requestOptionsFactory = requestOptionsFactory;
    }

    public override async Task<bool> IsInitializedAsync()
    {
      string storeMarkerName = this.GetStoreMarkerName();
      return await this.container.ItemExistsAsync(this.requestOptionsFactory.GetPartitionKey(storeMarkerName, storeMarkerName), storeMarkerName).ConfigureAwait(false);
    }

    public override async Task MarkInitializedAsync()
    {
      string storeMarkerName = this.GetStoreMarkerName();
      DocumentServiceLeaseStoreCosmos.InitializedDocument containerDocument = new DocumentServiceLeaseStoreCosmos.InitializedDocument()
      {
        Id = storeMarkerName
      };
      this.requestOptionsFactory.AddPartitionKeyIfNeeded((Action<string>) (pk => containerDocument.PartitionKey = pk), storeMarkerName);
      using (Stream itemStream = CosmosContainerExtensions.DefaultJsonSerializer.ToStream<DocumentServiceLeaseStoreCosmos.InitializedDocument>(containerDocument))
      {
        using (ResponseMessage responseMessage = await this.container.CreateItemStreamAsync(itemStream, this.requestOptionsFactory.GetPartitionKey(storeMarkerName, storeMarkerName)).ConfigureAwait(false))
          responseMessage.EnsureSuccessStatusCode();
      }
    }

    public override async Task<bool> AcquireInitializationLockAsync(TimeSpan lockTime)
    {
      string storeLockName = this.GetStoreLockName();
      DocumentServiceLeaseStoreCosmos.LockDocument containerDocument = new DocumentServiceLeaseStoreCosmos.LockDocument()
      {
        Id = storeLockName,
        TimeToLive = (int) lockTime.TotalSeconds
      };
      this.requestOptionsFactory.AddPartitionKeyIfNeeded((Action<string>) (pk => containerDocument.PartitionKey = pk), storeLockName);
      ItemResponse<DocumentServiceLeaseStoreCosmos.LockDocument> itemResponse = await this.container.TryCreateItemAsync<DocumentServiceLeaseStoreCosmos.LockDocument>(this.requestOptionsFactory.GetPartitionKey(storeLockName, storeLockName), containerDocument).ConfigureAwait(false);
      if (itemResponse == null)
        return false;
      this.lockETag = itemResponse.ETag;
      return true;
    }

    public override async Task<bool> ReleaseInitializationLockAsync()
    {
      string storeLockName = this.GetStoreLockName();
      ItemRequestOptions itemRequestOptions = new ItemRequestOptions();
      itemRequestOptions.IfMatchEtag = this.lockETag;
      ItemRequestOptions cosmosItemRequestOptions = itemRequestOptions;
      if (!await this.container.TryDeleteItemAsync<DocumentServiceLeaseStoreCosmos.LockDocument>(this.requestOptionsFactory.GetPartitionKey(storeLockName, storeLockName), storeLockName, cosmosItemRequestOptions).ConfigureAwait(false))
        return false;
      this.lockETag = (string) null;
      return true;
    }

    private string GetStoreMarkerName() => this.containerNamePrefix + ".info";

    private string GetStoreLockName() => this.containerNamePrefix + ".lock";

    private class LockDocument
    {
      [JsonProperty("id")]
      public string Id { get; set; }

      [JsonProperty("partitionKey", NullValueHandling = NullValueHandling.Ignore)]
      public string PartitionKey { get; set; }

      [JsonProperty("ttl")]
      public int TimeToLive { get; set; }
    }

    private class InitializedDocument
    {
      [JsonProperty("id")]
      public string Id { get; set; }

      [JsonProperty("partitionKey", NullValueHandling = NullValueHandling.Ignore)]
      public string PartitionKey { get; set; }
    }
  }
}
