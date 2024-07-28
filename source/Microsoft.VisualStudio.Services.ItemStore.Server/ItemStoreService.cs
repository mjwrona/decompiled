// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Server.ItemStoreService
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E6307531-8252-47C3-B21C-ECA66F38ED4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Azure.Table.MemImpl;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.ItemStore.AzureStorage;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ItemStore.Server
{
  public abstract class ItemStoreService : 
    ArtifactsServiceBase,
    IItemStore,
    IVssFrameworkService,
    IDisposable
  {
    private const int MaxBlobReferenceDeletesInBatch = 1000;
    private const int DefaultMaxWindowForGroupingReferenceDeletesInBatch = 1000;
    private const int MaxConcurrentReferences = 2000;
    private const int MaxGetOrAddItemRetries = 10;
    private const int MaxGetOrAddContainerRetries = 10;
    private readonly int MaxItemStoreParallelism = Environment.ProcessorCount * 8;
    private int MaxWindowForGroupingReferenceDeletesInBatch = 1000;
    internal static readonly TimeSpan DefaultMaxKeepUntilSpan = TimeSpan.FromDays(363.0);
    internal TimeSpan maxKeepUntilSpan = ItemStoreService.DefaultMaxKeepUntilSpan;
    internal static readonly DateTime EmptyBlobKeepUntilTime = new DateTime(3000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly Locator ContainerContainerId = Locator.Parse("0");
    private readonly Lazy<List<StrongBoxItemChangeHandler>> strongBoxItemChangeHandlers = new Lazy<List<StrongBoxItemChangeHandler>>((Func<List<StrongBoxItemChangeHandler>>) (() => new List<StrongBoxItemChangeHandler>()));

    public bool IsReadOnly => this.ItemProvider.IsReadOnly;

    protected override string ProductTraceArea => "ItemStore";

    public abstract bool CanUseTimeBasedReferences { get; }

    protected IItemProvider ItemProvider { get; set; }

    public virtual void ConfigureService(
      IVssRequestContext tfsRequestContext,
      string providerType,
      string shardingStrategy,
      int concurrentGetItemRequestCount)
    {
      shardingStrategy = shardingStrategy ?? "ConsistentHashing";
      if (string.Equals(providerType, "MemoryTable", StringComparison.OrdinalIgnoreCase))
      {
        string tableName = tfsRequestContext.GetTableName(this.GetExperienceName(), true);
        this.ItemProvider = (IItemProvider) new ItemTableProvider((ITableClientFactory) new MemoryTableClientFactory(MemoryTableStorage.Global, tableName), 1);
      }
      else if (string.Equals(providerType, "AzureTable", StringComparison.OrdinalIgnoreCase))
      {
        LocationMode? azureLocationMode = this.GetAzureLocationMode(tfsRequestContext);
        IEnumerable<StrongBoxConnectionString> connectionStrings = this.GetAzureConnectionStrings(tfsRequestContext);
        string tableName = tfsRequestContext.GetTableName(this.GetExperienceName(), true);
        Func<StrongBoxConnectionString, ITableClient> getTableClient = new Func<StrongBoxConnectionString, ITableClient>(tfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<IAzureCloudTableClientProvider>().GetTableClient);
        LocationMode? locationMode = azureLocationMode;
        string defaultTableName = tableName;
        string shardingStrategy1 = shardingStrategy;
        int num = tfsRequestContext.IsFeatureEnabled("Blobstore.Features.AzureBlobTelemetry") ? 1 : 0;
        this.ItemProvider = (IItemProvider) new ItemTableProvider((ITableClientFactory) new ItemShardingAzureCloudTableClientFactory(connectionStrings, getTableClient, locationMode, defaultTableName, shardingStrategy1, num != 0), concurrentGetItemRequestCount);
      }
      else
      {
        if (!string.Equals(providerType, "SQLTable", StringComparison.OrdinalIgnoreCase))
          throw new ArgumentException(Resources.UnknownProviderType((object) providerType));
        this.ItemProvider = (IItemProvider) new ItemTableProvider((ITableClientFactory) new SQLTableClientFactory(tfsRequestContext.GetTableName(this.GetExperienceName(), false)), concurrentGetItemRequestCount);
      }
    }

    public virtual async Task<ContainerItem> GetContainerAsync(
      IVssRequestContext requestContext,
      Locator name)
    {
      ContainerItem containerAsync;
      using (requestContext.Enter(ContentTracePoints.ItemStore.GetContainerAsyncCall, nameof (GetContainerAsync)))
      {
        try
        {
          containerAsync = await this.PumpOrInlineFromAsync<ContainerItem>(requestContext, (Func<VssRequestPump.Processor, Task<ContainerItem>>) (vssProcessor => this.ItemProvider.GetItemAsync<ContainerItem>(vssProcessor, ItemStoreService.GetItemShardedLocator(ItemStoreService.ContainerContainerId, name)))).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.GetContainerAsyncException, ex);
          throw;
        }
      }
      return containerAsync;
    }

    public virtual async Task<ContainerItem> GetOrAddContainerAsync(
      IVssRequestContext requestContext,
      ContainerItem container)
    {
      ContainerItem addContainerAsync;
      using (requestContext.Enter(ContentTracePoints.ItemStore.GetOrAddContainerAsyncCall, nameof (GetOrAddContainerAsync)))
      {
        try
        {
          addContainerAsync = await this.GetOrAddContainerInternalAsync(requestContext, container).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.GetOrAddContainerAsyncException, ex);
          throw;
        }
      }
      return addContainerAsync;
    }

    public virtual async Task<AssociationsStatus> AssociateItemsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      AssociationsItem associations)
    {
      AssociationsStatus associationsStatus;
      using (requestContext.Enter(ContentTracePoints.ItemStore.AssociateItemsAsyncCall, nameof (AssociateItemsAsync)))
      {
        try
        {
          ArgumentUtility.CheckForNull<AssociationsItem>(associations, nameof (associations));
          ArgumentUtility.CheckForNull<ItemTreeInfo>(associations.ItemTreeInfo, "ItemTreeInfo");
          ArgumentUtility.CheckForNull<IDomainId>(domainId, nameof (domainId));
          Locator containerLocator = associations.ItemTreeInfo.Root;
          ContainerItem container = await this.GetContainerAsync(requestContext, containerLocator).ConfigureAwait(true);
          if (container == null)
            throw ContainerNotFoundException.Create(containerLocator.Value);
          if (container.DeletePending)
            throw ContainerIsDeletePendingException.Create(containerLocator.Value);
          if (container.Sealed)
            throw ContainerIsSealedException.Create(containerLocator.Value);
          if (container.IsAppendOnly)
          {
            KeyValuePair<Locator, StoredItem> keyValuePair = associations.Items.FirstOrDefault<KeyValuePair<Locator, StoredItem>>((Func<KeyValuePair<Locator, StoredItem>, bool>) (kvp => kvp.Value.StorageETag != null));
            if (keyValuePair.Value != null)
              throw new ArgumentException("Container is append-only, but at least one item has StorageEtag specified:" + keyValuePair.Key.Value);
          }
          bool isEmptyBlobOptimizationEnabled = requestContext.IsFeatureEnabled("ItemStore.Features.EnableEmptyBlobReferenceOptimization");
          IReadOnlyDictionary<Locator, BlobItem> blobItems;
          IReadOnlyDictionary<BlobIdentifier, IEnumerable<BlobReference>> references;
          IReadOnlyDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> uploadedReferences;
          IReadOnlyDictionary<Locator, StoredItem> bloblessItems;
          this.SiftAssociationItemWithScope(associations, container, this.GetReferenceScope(), isEmptyBlobOptimizationEnabled, out IReadOnlyDictionary<BlobIdentifier, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks> _, out bloblessItems, out blobItems, out references, out uploadedReferences);
          await this.AddItemsToContainerAsync(requestContext, (IEnumerable<KeyValuePair<Locator, StoredItem>>) bloblessItems, containerLocator, container, associations.AbortIfAlreadyExists).ConfigureAwait(true);
          IBlobStore blobStore = requestContext.GetService<IBlobStore>();
          HashSet<BlobIdentifier> uniqueMissingContent = new HashSet<BlobIdentifier>();
          DateTime? untilReferenceTime = this.GetKeepUntilReferenceTime(container);
          int? maxRefsPerBlobId = container.IsAppendOnly || untilReferenceTime.HasValue ? new int?(1) : new int?();
          if (uploadedReferences.Any<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>>())
          {
            foreach (KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>> keyValuePair in (IEnumerable<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>>) await blobStore.TryReferenceWithBlocksAsync(requestContext, domainId, uploadedReferences.ToDictionaryOfEnumerables<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, BlobReference>(maxRefsPerBlobId)).ConfigureAwait(true))
            {
              if (keyValuePair.Value.Any<BlobReference>())
                uniqueMissingContent.Add(keyValuePair.Key);
            }
          }
          if (references.Any<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>>())
          {
            foreach (KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>> keyValuePair in (IEnumerable<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>>) await blobStore.TryReferenceAsync(requestContext, domainId, references.ToDictionaryOfEnumerables<BlobIdentifier, BlobReference>(maxRefsPerBlobId)).ConfigureAwait(true))
            {
              if (keyValuePair.Value.Any<BlobReference>())
                uniqueMissingContent.Add(keyValuePair.Key);
            }
          }
          Dictionary<Locator, StoredItem> dictionary = new Dictionary<Locator, StoredItem>();
          foreach (KeyValuePair<Locator, BlobItem> keyValuePair in (IEnumerable<KeyValuePair<Locator, BlobItem>>) blobItems)
          {
            if (!uniqueMissingContent.Contains(keyValuePair.Value.BlobIdentifier))
            {
              keyValuePair.Value.DomainId = domainId;
              dictionary.Add(keyValuePair.Key, (StoredItem) keyValuePair.Value);
            }
          }
          this.ValidateAndGetDomainId(dictionary.Select<KeyValuePair<Locator, StoredItem>, IDomainId>((Func<KeyValuePair<Locator, StoredItem>, IDomainId>) (itemPair => itemPair.Value.Convert<BlobItem>().DomainId)).Distinct<IDomainId>());
          await this.AddItemsToContainerAsync(requestContext, (IEnumerable<KeyValuePair<Locator, StoredItem>>) dictionary, containerLocator, container, associations.AbortIfAlreadyExists).ConfigureAwait(true);
          associationsStatus = new AssociationsStatus()
          {
            Missing = (IEnumerable<BlobIdentifier>) uniqueMissingContent,
            ItemTreeInfo = new ItemTreeInfo()
            {
              Root = containerLocator,
              BlobStoreUri = FrameworkBlobStore.GetBlobStoreServiceUri(requestContext)
            }
          };
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.AssociateItemsAsyncException, ex);
          throw;
        }
      }
      return associationsStatus;
    }

    public virtual async Task UpdateContainerAsync(
      IVssRequestContext requestContext,
      Locator name,
      ContainerItem newContainer)
    {
      using (requestContext.Enter(ContentTracePoints.ItemStore.UpdateContainerAsyncCall, nameof (UpdateContainerAsync)))
      {
        try
        {
          bool requiresWrite = false;
          bool flag1;
          do
          {
            bool allowUndoSoftDelete = requestContext.IsFeatureEnabled("ItemStore.Features.Internal.AllowUndoSoftDelete");
            ContainerItem existingContainer = await this.GetContainerAsync(requestContext, name).ConfigureAwait(true);
            if (existingContainer == null)
              throw ContainerNotFoundException.Create(name.Value);
            if (existingContainer.IsAppendOnly != newContainer.IsAppendOnly)
              throw new InvalidOperationException("Append-only attribute can only be set at creation.");
            bool flag2 = ItemStoreService.UpdateContainerSealAndCheckChangeRequiresWrite(existingContainer, newContainer);
            bool flag3 = ItemStoreService.UpdateContainerDeletePendingAndCheckChangeRequiresWrite(existingContainer, newContainer, allowUndoSoftDelete);
            bool flag4 = this.CanUseTimeBasedReferences && ItemStoreService.UpdateContainerExpirationAndCheckChangeRequiresWrite(existingContainer, newContainer);
            requiresWrite = flag2 | flag4 | flag3;
            if (flag4)
              await this.AddIdReferencesForExistingContainerBlobItemsAsync(requestContext, existingContainer).ConfigureAwait(true);
            flag1 = await this.PumpOrInlineFromAsync<bool>(requestContext, (Func<VssRequestPump.Processor, Task<bool>>) (vssProcessor => this.ItemProvider.CompareSwapItemAsync(vssProcessor, ItemStoreService.GetItemShardedLocator(ItemStoreService.ContainerContainerId, name), (StoredItem) existingContainer))).ConfigureAwait(true);
            if (!requiresWrite)
              break;
          }
          while (!flag1);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.UpdateContainerAsyncException, ex);
          throw;
        }
      }
    }

    public virtual async Task<bool> DeleteContainerAsync(
      IVssRequestContext requestContext,
      ContainerItem givenContainerItem)
    {
      using (requestContext.Enter(ContentTracePoints.ItemStore.DeleteContainerAsyncCall, nameof (DeleteContainerAsync)))
      {
        try
        {
          Locator containerName = givenContainerItem.Name;
          ContainerItem containerItem = await this.GetContainerAsync(requestContext, containerName).ConfigureAwait(true);
          if (containerItem == null || containerItem.StorageETag != givenContainerItem.StorageETag)
            return false;
          IConcurrentIterator<KeyValuePair<ShardableLocator, StoredItem>> enumerator = await this.PumpOrInlineFromAsync<IConcurrentIterator<KeyValuePair<ShardableLocator, StoredItem>>>(requestContext, (Func<VssRequestPump.Processor, Task<IConcurrentIterator<KeyValuePair<ShardableLocator, StoredItem>>>>) (vssProcessor => this.ForceEnumerationIfNecessaryAsync<KeyValuePair<ShardableLocator, StoredItem>>(this.ItemProvider.GetItemsConcurrentIterator<StoredItem>(vssProcessor, ItemStoreService.GetItemShardedLocator(containerName, Locator.Root), PathOptions.AllChildren), vssProcessor.CancellationToken))).ConfigureAwait(true);
          bool deleteSucceeded = true;
          int referenceDeletesInBatch = this.MaxWindowForGroupingReferenceDeletesInBatch;
          CancellationToken cancellationToken = requestContext.CancellationToken;
          await enumerator.GetPages<KeyValuePair<ShardableLocator, StoredItem>>(referenceDeletesInBatch, cancellationToken).DoWhileAsyncCaptureContext<IReadOnlyCollection<KeyValuePair<ShardableLocator, StoredItem>>>(requestContext.CancellationToken, (Func<IReadOnlyCollection<KeyValuePair<ShardableLocator, StoredItem>>, Task<bool>>) (async itemsToDeletePage =>
          {
            Dictionary<ShardableLocator, StoredItem> dictionary = itemsToDeletePage.ToDictionary<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator, StoredItem>((Func<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator>) (kvp => kvp.Key), (Func<KeyValuePair<ShardableLocator, StoredItem>, StoredItem>) (kvp => kvp.Value));
            deleteSucceeded = await this.RemoveBlobReferencesWithCorrespondingContainerItemsInBatchesAsync(requestContext, containerItem, (IDictionary<ShardableLocator, StoredItem>) dictionary, !containerItem.Sealed).ConfigureAwait(true);
            return deleteSucceeded;
          })).ConfigureAwait(true);
          if (!deleteSucceeded)
            return false;
          ShardableLocator containerItemPath = ItemStoreService.GetItemShardedLocator(ItemStoreService.ContainerContainerId, containerName);
          return await this.PumpOrInlineFromAsync<bool>(requestContext, (Func<VssRequestPump.Processor, Task<bool>>) (vssProcessor => this.ItemProvider.RemoveItemAsync(vssProcessor, containerItemPath, containerItem.StorageETag))).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.DeleteContainerAsyncException, ex);
          throw;
        }
      }
    }

    public virtual async Task<bool> DeleteItemAsync(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator path,
      StoredItem item)
    {
      bool flag;
      using (requestContext.Enter(ContentTracePoints.ItemStore.DeleteItemAsyncCall, nameof (DeleteItemAsync)))
      {
        try
        {
          if (item == null)
            throw new ArgumentNullException(nameof (item), string.Format("{0} was null in call to {1}.{2}(...{3}:\"{4}\", {5}:\"{6}\", {7}:null)", (object) "StoredItem", (object) nameof (ItemStoreService), (object) nameof (DeleteItemAsync), (object) nameof (containerName), (object) containerName, (object) nameof (path), (object) path, (object) nameof (item)));
          ContainerItem containerItem = await this.GetContainerAsync(requestContext, containerName).ConfigureAwait(true);
          if (containerItem == null)
            throw ContainerNotFoundException.Create(containerName.Value);
          if (containerItem.DeletePending)
            throw ContainerIsDeletePendingException.Create(containerName.Value);
          if (containerItem.Sealed)
            throw ContainerIsSealedException.Create(containerName.Value);
          if (containerItem.IsAppendOnly)
            throw new ArgumentException(string.Format("Cannot delete items from append-only container: \"{0}\"", (object) containerName));
          flag = await this.DeleteItemInternalAsync(requestContext, containerName, path, item).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.DeleteItemAsyncException, ex);
          throw;
        }
      }
      return flag;
    }

    public virtual async Task<IReadOnlyDictionary<Locator, bool>> DeleteItemsAsync(
      IVssRequestContext requestContext,
      Locator container,
      IReadOnlyDictionary<Locator, StoredItem> itemsToDelete)
    {
      IReadOnlyDictionary<Locator, bool> readOnlyDictionary;
      using (requestContext.Enter(ContentTracePoints.ItemStore.DeleteItemsAsyncCall, nameof (DeleteItemsAsync)))
      {
        Dictionary<Locator, bool> dictionary = new Dictionary<Locator, bool>();
        try
        {
          ContainerItem containerItem = await this.GetContainerAsync(requestContext, container).ConfigureAwait(true);
          if (containerItem == null)
            throw ContainerNotFoundException.Create(container.Value);
          if (containerItem.DeletePending)
            throw ContainerIsDeletePendingException.Create(container.Value);
          if (containerItem.Sealed)
            throw ContainerIsSealedException.Create(container.Value);
          if (containerItem.IsAppendOnly)
            throw new ArgumentException("Cannot delete items from append-only container.");
          readOnlyDictionary = await this.DeleteItemsInternalAsync(requestContext, container, itemsToDelete).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.DeleteItemsAsyncException, ex);
          throw;
        }
      }
      return readOnlyDictionary;
    }

    public virtual async Task<bool> TryAddBlobItemAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator path,
      T itemToAdd,
      Func<Task<Stream>> blobStreamFactory,
      IDomainId domainId,
      bool repairManifest = false)
      where T : BlobItem
    {
      Stream stream = (Stream) null;
      BlobIdentifier blobId = (BlobIdentifier) null;
      IdBlobReference? refId = new IdBlobReference?();
      using (requestContext.Enter(ContentTracePoints.ItemStore.TryAddBlobItemAsyncCall, nameof (TryAddBlobItemAsync)))
      {
        IBlobStore blobStore = requestContext.GetService<IBlobStore>();
        requestContext.TraceAlways(ContentTracePoints.ItemStore.MaterializeFileList, string.Format("[{0}]: Attempting to materialize file list for drop. Domain {1}. Repair enabled: {2}.", (object) nameof (TryAddBlobItemAsync), (object) domainId, (object) repairManifest));
        object obj1 = (object) null;
        int num = 0;
        bool flag;
        try
        {
          ContainerItem container = await this.GetContainerAsync(requestContext, containerName).ConfigureAwait(true);
          if (container == null)
            throw ContainerNotFoundException.Create(containerName.Value);
          if (container.DeletePending)
            throw ContainerIsDeletePendingException.Create(containerName.Value);
          if (container.Sealed)
            throw ContainerIsSealedException.Create(containerName.Value);
          if (!BlobItem.HasBlobId((Microsoft.VisualStudio.Services.ItemStore.Common.Item) itemToAdd))
          {
            stream = await blobStreamFactory().ConfigureAwait(true);
            T obj2 = itemToAdd;
            obj2.BlobIdentifier = (await stream.CalculateBlobIdentifierWithBlocksAsync((IBlobHasher) Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.Instance).ConfigureAwait(true)).BlobId;
            obj2 = default (T);
          }
          if (!BlobItem.HasDomainId((Microsoft.VisualStudio.Services.ItemStore.Common.Item) itemToAdd))
            ((T) itemToAdd).DomainId = domainId;
          blobId = ((T) itemToAdd).BlobIdentifier;
          ShardableLocator itemShardedLocator = ItemStoreService.GetItemShardedLocator(containerName, path);
          DateTime? untilReferenceTime = this.GetKeepUntilReferenceTime(container);
          BlobReference reference;
          if (untilReferenceTime.HasValue)
          {
            reference = new BlobReference(untilReferenceTime.Value);
          }
          else
          {
            refId = new IdBlobReference?(this.GetIdBlobReference(itemShardedLocator));
            reference = new BlobReference(refId.Value);
          }
          bool flag1 = repairManifest;
          if (!flag1)
            flag1 = !await blobStore.TryReferenceAsync(requestContext, domainId, ((T) itemToAdd).BlobIdentifier, reference).ConfigureAwait(true);
          if (flag1)
          {
            requestContext.TraceAlways(ContentTracePoints.ItemStore.FileListDoesNotExist, "[TryAddBlobItemAsync]: File list [" + ((T) itemToAdd).BlobIdentifier.ValueString + "] for drop doesn't exist. Attempt adding it now...");
            Stream stream1 = stream;
            if (stream1 == null)
              stream1 = await blobStreamFactory().ConfigureAwait(true);
            stream = stream1;
            await blobStore.PutBlobAndReferenceAsync(requestContext, domainId, ((T) itemToAdd).BlobIdentifier, stream, reference).ConfigureAwait(true);
          }
          if (repairManifest)
          {
            BlobItem blobItem = await this.GetItemAsync<BlobItem>(requestContext, containerName, path, LatencyPreference.PreferHighThroughput).ConfigureAwait(true);
            if (blobItem != null)
            {
              if (blobItem.BlobIdentifier == blobId)
              {
                refId = new IdBlobReference?();
                flag = true;
                goto label_39;
              }
              else if (!await this.DeleteItemAsync(requestContext, containerName, path, (StoredItem) blobItem).ConfigureAwait(true))
                throw new InvalidOperationException("Unable to delete manifest blob item. Aborting repair-manifest operation. Attempt deletion manually.");
            }
          }
          if (!await this.CompareSwapItemAsync(requestContext, containerName, path, (StoredItem) itemToAdd).ConfigureAwait(true))
          {
            requestContext.TraceAlways(ContentTracePoints.ItemStore.UpdatingDropItemFailed, "[TryAddBlobItemAsync]: Updating drop item for file list failed.");
            ContainedItem possibleBlobItem = await this.GetItemAsync<ContainedItem>(requestContext, containerName, path, LatencyPreference.PreferHighThroughput).ConfigureAwait(true);
            if (possibleBlobItem == null || !BlobItem.HasBlobId((Microsoft.VisualStudio.Services.ItemStore.Common.Item) possibleBlobItem))
            {
              refId = new IdBlobReference?();
              throw DropManifestItemException.Create(containerName.Value, path.Value, ((T) itemToAdd).BlobIdentifier.ValueString);
            }
            if (possibleBlobItem.Equals((Microsoft.VisualStudio.Services.ItemStore.Common.Item) itemToAdd))
            {
              requestContext.TraceAlways(ContentTracePoints.ItemStore.UpdatingDropItemLostRace, "[TryAddBlobItemAsync]: Updating drop item for file list failed because the current thread lost and the file list [" + ((T) itemToAdd).BlobIdentifier.ValueString + "] was already journaled by another thread.");
              refId = new IdBlobReference?();
              flag = false;
            }
            else
            {
              requestContext.TraceAlways(ContentTracePoints.ItemStore.UpdatingDropItemLostRaceMismatch, "[TryAddBlobItemAsync]: Updating drop item for file list failed because the current thread lost and the manifest blob id journaled by the winner thread doesn't match up with the one to add.");
              flag = false;
            }
          }
          else
          {
            requestContext.TraceAlways(ContentTracePoints.ItemStore.UpdatingDropItemSuccess, "[TryAddBlobItemAsync]: Updating drop item for file list succeeded and the current thread won. The file list [" + ((T) itemToAdd).BlobIdentifier.ValueString + "] was successfully journaled.");
            refId = new IdBlobReference?();
            flag = true;
          }
label_39:
          num = 1;
        }
        catch (object ex)
        {
          obj1 = ex;
        }
        stream?.Dispose();
        if (blobId != (BlobIdentifier) null && refId.HasValue)
        {
          IdBlobReference idBlobReference;
          if (refId.Value.IdReferenceScopedToFileList())
          {
            IVssRequestContext context = requestContext;
            SingleLocationTracePoint manifestRollbackAttempt = ContentTracePoints.ItemStore.ManifestRollbackAttempt;
            string[] strArray = new string[6]
            {
              "Attempting manifest blob reference rollback: Blob ID=",
              blobId.ValueString,
              ", Reference ID=",
              null,
              null,
              null
            };
            idBlobReference = refId.Value;
            strArray[3] = idBlobReference.Name;
            strArray[4] = ", Container=";
            strArray[5] = containerName.Value;
            string messageFormat = string.Concat(strArray);
            object[] objArray = Array.Empty<object>();
            context.TraceAlways(manifestRollbackAttempt, messageFormat, objArray);
          }
          await blobStore.RemoveReferenceAsync(requestContext, domainId, blobId, refId.Value).ConfigureAwait(true);
          IVssRequestContext context1 = requestContext;
          SingleLocationTracePoint manifestRollbackSuccess = ContentTracePoints.ItemStore.ManifestRollbackSuccess;
          string[] strArray1 = new string[7]
          {
            "Successfully rolled back manifest blob reference : Blob ID=",
            blobId.ValueString,
            ", Reference ID=",
            null,
            null,
            null,
            null
          };
          idBlobReference = refId.Value;
          strArray1[3] = idBlobReference.Name;
          strArray1[4] = ", Container=";
          strArray1[5] = containerName.Value;
          strArray1[6] = ".";
          string messageFormat1 = string.Concat(strArray1);
          object[] objArray1 = Array.Empty<object>();
          context1.TraceAlways(manifestRollbackSuccess, messageFormat1, objArray1);
        }
        object obj = obj1;
        if (obj != null)
        {
          if (!(obj is Exception source))
            throw obj;
          ExceptionDispatchInfo.Capture(source).Throw();
        }
        if (num == 1)
          return flag;
        obj1 = (object) null;
        blobStore = (IBlobStore) null;
      }
      stream = (Stream) null;
      blobId = (BlobIdentifier) null;
      refId = new IdBlobReference?();
      bool flag2;
      return flag2;
    }

    public DateTime? GetKeepUntilReferenceTime(ContainerItem containerItem)
    {
      if (containerItem.UseIdReferences.HasValue)
      {
        if (containerItem.UseIdReferences.Value)
          return new DateTime?();
        DateTime? expirationTime;
        if (!containerItem.TryGetExpirationTime(out expirationTime))
          throw new InvalidOperationException("Container is set for KeepUntilReferences, but has no expiration set.");
        return expirationTime.HasValue ? new DateTime?(expirationTime.Value) : throw new InvalidOperationException("Container is set for KeepUntilReferences, but has infinite expiration set.");
      }
      DateTime? expirationTime1;
      if (containerItem.TryGetExpirationTime(out expirationTime1) && expirationTime1.HasValue)
      {
        DateTime? nullable = expirationTime1;
        DateTime dateTime = DateTime.UtcNow + this.maxKeepUntilSpan;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() < dateTime ? 1 : 0) : 0) != 0)
          return new DateTime?(expirationTime1.Value);
      }
      return new DateTime?();
    }

    public virtual async Task<bool> CompareSwapItemAsync(
      IVssRequestContext requestContext,
      Locator containerId,
      Locator path,
      StoredItem item)
    {
      using (requestContext.Enter(ContentTracePoints.ItemStore.CompareSwapItemAsyncCall, nameof (CompareSwapItemAsync)))
      {
        try
        {
          ContainerItem containerItem = await this.GetContainerAsync(requestContext, containerId).ConfigureAwait(true);
          if (containerItem == null)
            throw ContainerNotFoundException.Create(containerId.Value);
          if (containerItem.DeletePending)
            throw ContainerIsDeletePendingException.Create(containerId.Value);
          if (containerItem.Sealed)
            throw ContainerIsSealedException.Create(containerId.Value);
          if (containerItem.IsAppendOnly && item.StorageETag != null)
            throw new ArgumentException("Container is append-only, but StorageETag is specified.");
          ShardableLocator shardableLocator = ItemStoreService.GetItemShardedLocator(containerId, path);
          if (await this.PumpOrInlineFromAsync<bool>(requestContext, (Func<VssRequestPump.Processor, Task<bool>>) (vssProcessor => this.ItemProvider.CompareSwapItemAsync(vssProcessor, shardableLocator, item))).ConfigureAwait(true))
            return true;
          if (item.StorageETag != null)
            return false;
          StoredItem possibleBlobItem = await this.PumpOrInlineFromAsync<StoredItem>(requestContext, (Func<VssRequestPump.Processor, Task<StoredItem>>) (vssProcessor => this.ItemProvider.GetItemAsync<StoredItem>(vssProcessor, shardableLocator))).ConfigureAwait(true);
          ConfiguredTaskAwaitable<bool> configuredTaskAwaitable;
          if (possibleBlobItem != null && BlobItem.IsDeleteInProgress((Microsoft.VisualStudio.Services.ItemStore.Common.Item) possibleBlobItem))
          {
            configuredTaskAwaitable = this.DeleteItemHelperAsync(requestContext, possibleBlobItem, shardableLocator).ConfigureAwait(true);
            int num = await configuredTaskAwaitable ? 1 : 0;
          }
          configuredTaskAwaitable = this.PumpOrInlineFromAsync<bool>(requestContext, (Func<VssRequestPump.Processor, Task<bool>>) (vssProcessor => this.ItemProvider.CompareSwapItemAsync(vssProcessor, shardableLocator, item))).ConfigureAwait(true);
          return await configuredTaskAwaitable;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.CompareSwapItemAsyncException, ex);
          throw;
        }
      }
    }

    public virtual async Task<IDictionary<Locator, bool>> CompareSwapItemsAsync(
      IVssRequestContext requestContext,
      Locator containerId,
      IReadOnlyDictionary<Locator, StoredItem> items,
      bool atomicDirectory = false)
    {
      IDictionary<Locator, bool> dictionary1;
      using (requestContext.Enter(ContentTracePoints.ItemStore.CompareSwapItemsAsyncCall, nameof (CompareSwapItemsAsync)))
      {
        try
        {
          IVssRequestContext requestContext1 = requestContext;
          Locator containerId1 = containerId;
          ContainerItem containerItem = await this.GetContainerAsync(requestContext, containerId).ConfigureAwait(true);
          IDictionary<Locator, bool> dictionary2 = await this.CompareSwapItemsInternalAsync(requestContext1, containerId1, containerItem, items, atomicDirectory).ConfigureAwait(true);
          requestContext1 = (IVssRequestContext) null;
          containerId1 = (Locator) null;
          dictionary1 = dictionary2;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.CompareSwapItemsAsyncException, ex);
          throw;
        }
      }
      return dictionary1;
    }

    public virtual async Task<T> GetItemAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator path,
      LatencyPreference latencyPreference = LatencyPreference.PreferHighThroughput)
      where T : StoredItem
    {
      T itemAsync;
      using (requestContext.Enter(ContentTracePoints.ItemStore.GetItemAsyncCall, nameof (GetItemAsync)))
      {
        try
        {
          ShardableLocator locator = ItemStoreService.GetItemShardedLocator(containerName, path);
          T possibleBlobItem = await this.PumpOrInlineFromAsync<T>(requestContext, (Func<VssRequestPump.Processor, Task<T>>) (vssProcessor => this.ItemProvider.GetItemAsync<T>(vssProcessor, locator, latencyPreference))).ConfigureAwait(true);
          itemAsync = (object) possibleBlobItem == null || !BlobItem.IsDeleteInProgress((Microsoft.VisualStudio.Services.ItemStore.Common.Item) possibleBlobItem) ? possibleBlobItem : default (T);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.GetItemAsyncException, ex);
          throw;
        }
      }
      return itemAsync;
    }

    public virtual async Task<T> GetOrAddItemAsync<T>(
      IVssRequestContext requestContext,
      Locator containerId,
      Locator path,
      T item)
      where T : StoredItem
    {
      T orAddItemAsync;
      using (requestContext.Enter(ContentTracePoints.ItemStore.GetOrAddContainerAsyncCall, nameof (GetOrAddItemAsync)))
      {
        try
        {
          int retries = 10;
          T obj1 = default (T);
          T obj2;
          do
          {
            if (retries <= 0)
              throw new ChangeConflictException("Could not GetOrAddItem.");
            --retries;
            if (await this.CompareSwapItemAsync(requestContext, containerId, path, (StoredItem) item).ConfigureAwait(true))
              obj2 = item;
            else
              obj2 = await this.PumpOrInlineFromAsync<T>(requestContext, (Func<VssRequestPump.Processor, Task<T>>) (vssProcessor => this.ItemProvider.GetItemAsync<T>(vssProcessor, ItemStoreService.GetItemShardedLocator(containerId, path)))).ConfigureAwait(true);
          }
          while ((object) obj2 == null);
          orAddItemAsync = obj2;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.GetOrAddItemAsyncException, ex);
          throw;
        }
      }
      return orAddItemAsync;
    }

    public virtual Task<IConcurrentIterator<KeyValuePair<Locator, ContainerItem>>> GetContainersConcurrentIteratorAsync(
      IVssRequestContext requestContext,
      Locator name,
      PathOptions pathOptions)
    {
      using (requestContext.Enter(ContentTracePoints.ItemStore.GetContainersConcurrentIteratorAsyncCall, nameof (GetContainersConcurrentIteratorAsync)))
      {
        try
        {
          return this.GetItemsConcurrentIteratorAsync<ContainerItem>(requestContext, ItemStoreService.ContainerContainerId, name, pathOptions);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.GetContainersConcurrentIteratorAsyncException, ex);
          throw;
        }
      }
    }

    public virtual async Task<IConcurrentIterator<KeyValuePair<Locator, T>>> GetItemsConcurrentIteratorAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator pathPrefix,
      PathOptions pathOptions)
      where T : StoredItem
    {
      IConcurrentIterator<KeyValuePair<Locator, T>> concurrentIteratorAsync;
      using (requestContext.Enter(ContentTracePoints.ItemStore.GetItemsConcurrentIteratorAsyncCall, nameof (GetItemsConcurrentIteratorAsync)))
      {
        try
        {
          concurrentIteratorAsync = (await this.PumpOrInlineFromAsync<IConcurrentIterator<KeyValuePair<ShardableLocator, T>>>(requestContext, (Func<VssRequestPump.Processor, Task<IConcurrentIterator<KeyValuePair<ShardableLocator, T>>>>) (vssProcessor => this.ForceEnumerationIfNecessaryAsync<KeyValuePair<ShardableLocator, T>>(this.ItemProvider.GetItemsConcurrentIterator<T>(vssProcessor, ItemStoreService.GetItemShardedLocator(containerName, pathPrefix), pathOptions), vssProcessor.CancellationToken))).ConfigureAwait(true)).Where<KeyValuePair<ShardableLocator, T>>((Func<KeyValuePair<ShardableLocator, T>, bool>) (pathAndItem => (object) pathAndItem.Value != null && !BlobItem.IsDeleteInProgress((Microsoft.VisualStudio.Services.ItemStore.Common.Item) pathAndItem.Value))).Select<KeyValuePair<ShardableLocator, T>, KeyValuePair<Locator, T>>((Func<KeyValuePair<ShardableLocator, T>, KeyValuePair<Locator, T>>) (kvp => new KeyValuePair<Locator, T>(ItemStoreService.ExtractPathWithinContainer(kvp.Key), kvp.Value)));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.GetItemsConcurrentIteratorAsyncException, ex);
          throw;
        }
      }
      return concurrentIteratorAsync;
    }

    public virtual async Task<IConcurrentIterator<KeyValuePair<Locator, T>>> GetResumableItemsConcurrentIteratorAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator pathPrefix,
      Locator resumePath,
      PathOptions pathOptions,
      IteratorPartition partition,
      FilterOptions filterOptions = null,
      LocatorRange locatorRange = null)
      where T : StoredItem
    {
      IConcurrentIterator<KeyValuePair<Locator, T>> concurrentIteratorAsync;
      using (requestContext.Enter(ContentTracePoints.ItemStore.GetResumableItemsConcurrentIteratorAsyncCall, nameof (GetResumableItemsConcurrentIteratorAsync)))
      {
        try
        {
          concurrentIteratorAsync = (await this.PumpOrInlineFromAsync<IConcurrentIterator<KeyValuePair<ShardableLocator, T>>>(requestContext, (Func<VssRequestPump.Processor, Task<IConcurrentIterator<KeyValuePair<ShardableLocator, T>>>>) (vssProcessor => this.ForceEnumerationIfNecessaryAsync<KeyValuePair<ShardableLocator, T>>(this.ItemProvider.GetResumableItemsConcurrentIterator<T>(vssProcessor, ItemStoreService.GetItemShardedLocator(containerName, pathPrefix), ItemStoreService.GetItemShardedLocator(containerName, resumePath), pathOptions, partition, filterOptions, locatorRange?.ToShardable((Func<Locator, ShardableLocator>) (path => ItemStoreService.GetItemShardedLocator(containerName, path)))), vssProcessor.CancellationToken))).ConfigureAwait(true)).Where<KeyValuePair<ShardableLocator, T>>((Func<KeyValuePair<ShardableLocator, T>, bool>) (pathAndItem => (object) pathAndItem.Value != null && !BlobItem.IsDeleteInProgress((Microsoft.VisualStudio.Services.ItemStore.Common.Item) pathAndItem.Value))).Select<KeyValuePair<ShardableLocator, T>, KeyValuePair<Locator, T>>((Func<KeyValuePair<ShardableLocator, T>, KeyValuePair<Locator, T>>) (kvp => new KeyValuePair<Locator, T>(ItemStoreService.ExtractPathWithinContainer(kvp.Key), kvp.Value)));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.GetResumableItemsConcurrentIteratorAsyncException, ex);
          throw;
        }
      }
      return concurrentIteratorAsync;
    }

    public virtual async Task<IConcurrentIterator<IEnumerable<KeyValuePair<Locator, T>>>> GetItemPagesConcurrentIteratorAsync<T>(
      IVssRequestContext requestContext,
      Locator containerName,
      Locator pathPrefix,
      PathOptions pathOptions)
      where T : StoredItem
    {
      IConcurrentIterator<IEnumerable<KeyValuePair<Locator, T>>> concurrentIteratorAsync;
      using (requestContext.Enter(ContentTracePoints.ItemStore.GetItemPagesConcurrentIteratorAsyncCall, nameof (GetItemPagesConcurrentIteratorAsync)))
      {
        try
        {
          concurrentIteratorAsync = (await this.PumpOrInlineFromAsync<IConcurrentIterator<IEnumerable<KeyValuePair<ShardableLocator, T>>>>(requestContext, (Func<VssRequestPump.Processor, Task<IConcurrentIterator<IEnumerable<KeyValuePair<ShardableLocator, T>>>>>) (vssProcessor => this.ForceEnumerationIfNecessaryAsync<IEnumerable<KeyValuePair<ShardableLocator, T>>>(this.ItemProvider.GetItemPagesConcurrentIterator<T>(vssProcessor, ItemStoreService.GetItemShardedLocator(containerName, pathPrefix), pathOptions), vssProcessor.CancellationToken))).ConfigureAwait(true)).Select<IEnumerable<KeyValuePair<ShardableLocator, T>>, IEnumerable<KeyValuePair<Locator, T>>>((Func<IEnumerable<KeyValuePair<ShardableLocator, T>>, IEnumerable<KeyValuePair<Locator, T>>>) (page => page.Where<KeyValuePair<ShardableLocator, T>>((Func<KeyValuePair<ShardableLocator, T>, bool>) (pathAndItem => (object) pathAndItem.Value != null && !BlobItem.IsDeleteInProgress((Microsoft.VisualStudio.Services.ItemStore.Common.Item) pathAndItem.Value))).Select<KeyValuePair<ShardableLocator, T>, KeyValuePair<Locator, T>>((Func<KeyValuePair<ShardableLocator, T>, KeyValuePair<Locator, T>>) (kvp => new KeyValuePair<Locator, T>(ItemStoreService.ExtractPathWithinContainer(kvp.Key), kvp.Value)))));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.GetItemPagesConcurrentIteratorAsyncException, ex);
          throw;
        }
      }
      return concurrentIteratorAsync;
    }

    public virtual async Task<IConcurrentIterator<KeyValuePair<Locator, T>>> GetItemsConcurrentIteratorAsync<T>(
      IVssRequestContext requestContext,
      Locator container,
      IReadOnlyCollection<Locator> itemLocators)
      where T : StoredItem
    {
      IConcurrentIterator<KeyValuePair<Locator, T>> concurrentIteratorAsync;
      using (requestContext.Enter(ContentTracePoints.ItemStore.GetItemsConcurrentIteratorAsync2Call, nameof (GetItemsConcurrentIteratorAsync)))
      {
        try
        {
          concurrentIteratorAsync = await this.PumpOrInlineFromAsync<IConcurrentIterator<KeyValuePair<Locator, T>>>(requestContext, (Func<VssRequestPump.Processor, Task<IConcurrentIterator<KeyValuePair<Locator, T>>>>) (vssProcessor => this.ForceEnumerationIfNecessaryAsync<KeyValuePair<Locator, T>>(this.ItemProvider.GetItemsConcurrentIterator<T>(vssProcessor, itemLocators.Select<Locator, ShardableLocator>((Func<Locator, ShardableLocator>) (locator => ItemStoreService.GetItemShardedLocator(container, locator)))).Where<KeyValuePair<ShardableLocator, T>>((Func<KeyValuePair<ShardableLocator, T>, bool>) (pathAndItem => (object) pathAndItem.Value != null && !BlobItem.IsDeleteInProgress((Microsoft.VisualStudio.Services.ItemStore.Common.Item) pathAndItem.Value))).Select<KeyValuePair<ShardableLocator, T>, KeyValuePair<Locator, T>>((Func<KeyValuePair<ShardableLocator, T>, KeyValuePair<Locator, T>>) (kvp => new KeyValuePair<Locator, T>(ItemStoreService.ExtractPathWithinContainer(kvp.Key), kvp.Value))), vssProcessor.CancellationToken))).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.GetItemsConcurrentIteratorAsync2Exception, ex);
          throw;
        }
      }
      return concurrentIteratorAsync;
    }

    public virtual async Task<IDictionary<MoveOperation, bool>> MoveItemsAsync<T>(
      IVssRequestContext requestContext,
      Locator container,
      MoveItemOperations moveOperations,
      MoveItemOptions options)
      where T : StoredItem
    {
      IDictionary<MoveOperation, bool> dictionary1;
      using (requestContext.Enter(ContentTracePoints.ItemStore.MoveItemsAsyncCall, nameof (MoveItemsAsync)))
      {
        try
        {
          Dictionary<MoveOperation, bool> moveResults = new Dictionary<MoveOperation, bool>();
          IDictionary<Locator, T> existingSources = await (await this.GetItemsConcurrentIteratorAsync<T>(requestContext, container, (IReadOnlyCollection<Locator>) moveOperations.Select<MoveOperation, Locator>((Func<MoveOperation, Locator>) (moveOperation => moveOperation.Source)).ToList<Locator>()).ConfigureAwait(true)).ToDictionaryAsync<Locator, T>(requestContext.CancellationToken).ConfigureAwait(true);
          IDictionary<Locator, string> originalETags = (IDictionary<Locator, string>) existingSources.ToDictionary<KeyValuePair<Locator, T>, Locator, string>((Func<KeyValuePair<Locator, T>, Locator>) (kvp => kvp.Key), (Func<KeyValuePair<Locator, T>, string>) (kvp => kvp.Value.StorageETag));
          foreach (MoveOperation key in moveOperations.Where<MoveOperation>((Func<MoveOperation, bool>) (moveOperation => !existingSources.ContainsKey(moveOperation.Source))))
            moveResults[key] = false;
          Dictionary<Locator, StoredItem> dictionary2 = existingSources.ToDictionary<KeyValuePair<Locator, T>, Locator, StoredItem>((Func<KeyValuePair<Locator, T>, Locator>) (kvp => moveOperations[kvp.Key]), (Func<KeyValuePair<Locator, T>, StoredItem>) (kvp => (StoredItem) kvp.Value));
          foreach (Locator key in dictionary2.Keys)
            dictionary2[key].StorageETag = (string) null;
          IDictionary<Locator, bool> itemsToWriteResults = await this.CompareSwapItemsAsync(requestContext, container, (IReadOnlyDictionary<Locator, StoredItem>) dictionary2, false).ConfigureAwait(true);
          foreach (KeyValuePair<Locator, T> keyValuePair in (IEnumerable<KeyValuePair<Locator, T>>) existingSources)
            keyValuePair.Value.StorageETag = originalETags[keyValuePair.Key];
          IReadOnlyDictionary<Locator, bool> readOnlyDictionary = await this.DeleteItemsAsync(requestContext, container, (IReadOnlyDictionary<Locator, StoredItem>) moveOperations.Where<MoveOperation>((Func<MoveOperation, bool>) (operation =>
          {
            if (!existingSources.ContainsKey(operation.Source))
              return false;
            return itemsToWriteResults[operation.Destination] || options.HasFlag((Enum) MoveItemOptions.DeleteSourceIfDestinationExists);
          })).ToDictionary<MoveOperation, Locator, StoredItem>((Func<MoveOperation, Locator>) (operation => operation.Source), (Func<MoveOperation, StoredItem>) (operation => (StoredItem) existingSources[operation.Source]))).ConfigureAwait(true);
          foreach (MoveOperation moveOperation in moveOperations)
            moveResults[moveOperation] = readOnlyDictionary.ContainsKey(moveOperation.Source) && readOnlyDictionary[moveOperation.Source];
          dictionary1 = (IDictionary<MoveOperation, bool>) moveResults;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.ItemStore.MoveItemsAsyncException, ex);
          throw;
        }
      }
      return dictionary1;
    }

    internal void SiftAssociationItemWithScope(
      AssociationsItem associations,
      ContainerItem containerItem,
      string referenceScope,
      bool isEmptyBlobOptimizationEnabled,
      out IReadOnlyDictionary<BlobIdentifier, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks> recentlyUploadedBlobs,
      out IReadOnlyDictionary<Locator, StoredItem> bloblessItems,
      out IReadOnlyDictionary<Locator, BlobItem> blobItems,
      out IReadOnlyDictionary<BlobIdentifier, IEnumerable<BlobReference>> references,
      out IReadOnlyDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> uploadedReferences)
    {
      Dictionary<BlobIdentifier, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks> dictionary1 = new Dictionary<BlobIdentifier, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>();
      if (associations.BlobsUploaded != null)
      {
        foreach (Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks identifierWithBlocks in associations.BlobsUploaded)
          dictionary1[identifierWithBlocks.BlobId] = identifierWithBlocks;
      }
      Dictionary<Locator, StoredItem> dictionary2 = new Dictionary<Locator, StoredItem>();
      Dictionary<Locator, BlobItem> dictionary3 = new Dictionary<Locator, BlobItem>();
      Dictionary<BlobIdentifier, List<BlobReference>> source1 = new Dictionary<BlobIdentifier, List<BlobReference>>();
      Dictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, List<BlobReference>> source2 = new Dictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, List<BlobReference>>();
      DateTime? untilReferenceTime = this.GetKeepUntilReferenceTime(containerItem);
      BlobReference blobReference1 = untilReferenceTime.HasValue ? new BlobReference(untilReferenceTime.Value) : (BlobReference) null;
      foreach (KeyValuePair<Locator, StoredItem> keyValuePair in associations.Items)
      {
        if (BlobItem.HasVsoHashBlobId((Microsoft.VisualStudio.Services.ItemStore.Common.Item) keyValuePair.Value))
        {
          BlobItem blobItem = keyValuePair.Value.Convert<BlobItem>();
          dictionary3.Add(keyValuePair.Key, blobItem);
          BlobIdentifier blobIdentifier = blobItem.BlobIdentifier;
          BlobReference blobReference2;
          if (isEmptyBlobOptimizationEnabled && blobIdentifier.IsOfNothing())
          {
            blobReference2 = new BlobReference(ItemStoreService.EmptyBlobKeepUntilTime);
          }
          else
          {
            BlobReference blobReference3 = blobReference1;
            if ((object) blobReference3 == null)
              blobReference3 = new BlobReference(ItemStoreService.GetBlobReferenceIdWithScope(containerItem.Name, keyValuePair.Key, referenceScope));
            blobReference2 = blobReference3;
          }
          Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks key;
          if (dictionary1.TryGetValue(blobIdentifier, out key))
          {
            List<BlobReference> blobReferenceList;
            if (!source2.TryGetValue(key, out blobReferenceList))
            {
              blobReferenceList = new List<BlobReference>();
              source2.Add(key, blobReferenceList);
            }
            blobReferenceList.Add(blobReference2);
          }
          else
          {
            List<BlobReference> blobReferenceList;
            if (!source1.TryGetValue(blobIdentifier, out blobReferenceList))
            {
              blobReferenceList = new List<BlobReference>();
              source1.Add(blobIdentifier, blobReferenceList);
            }
            blobReferenceList.Add(blobReference2);
          }
        }
        else
          dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
      }
      recentlyUploadedBlobs = (IReadOnlyDictionary<BlobIdentifier, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>) dictionary1;
      bloblessItems = (IReadOnlyDictionary<Locator, StoredItem>) dictionary2;
      blobItems = (IReadOnlyDictionary<Locator, BlobItem>) dictionary3;
      references = (IReadOnlyDictionary<BlobIdentifier, IEnumerable<BlobReference>>) source1.ToDictionary<KeyValuePair<BlobIdentifier, List<BlobReference>>, BlobIdentifier, IEnumerable<BlobReference>>((Func<KeyValuePair<BlobIdentifier, List<BlobReference>>, BlobIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<BlobIdentifier, List<BlobReference>>, IEnumerable<BlobReference>>) (kvp => (IEnumerable<BlobReference>) kvp.Value));
      uploadedReferences = (IReadOnlyDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>) source2.ToDictionary<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, List<BlobReference>>, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>((Func<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, List<BlobReference>>, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>) (kvp => kvp.Key), (Func<KeyValuePair<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, List<BlobReference>>, IEnumerable<BlobReference>>) (kvp => (IEnumerable<BlobReference>) kvp.Value));
    }

    protected static ShardableLocator GetItemShardedLocator(Locator container, Locator path)
    {
      if (path == (Locator) null)
        throw new ArgumentNullException(nameof (path));
      if (container == ItemStoreService.ContainerContainerId)
        ItemStoreService.AssertContainerName(path);
      else
        ItemStoreService.AssertContainerName(container);
      return new ShardableLocator(new Locator(container.PathSegments.Concat<string>((IEnumerable<string>) ItemStoreServerConstants.LocatorSeparators).Concat<string>((IEnumerable<string>) path.PathSegments)), new Locator(new string[1]
      {
        "INVALID"
      }).Value);
    }

    protected virtual void Dispose(bool disposing) => this.ItemProvider?.Dispose();

    protected abstract string GetExperienceName();

    protected virtual string GetReferenceScope() => this.GetExperienceName();

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      string experienceName = this.GetExperienceName();
      if (experienceName == null)
        throw new ArgumentException(Resources.ExperienceNameNotSpecified());
      if (experienceName.Length > 20 || Regex.IsMatch(experienceName, "[^a-z]"))
        throw new ArgumentException(Resources.InvalidExperienceName((object) experienceName));
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      string itemProviderType = this.GetItemProviderType(systemRequestContext);
      string shardingStrategy = this.GetShardingStrategy(systemRequestContext);
      int itemRequestCount = this.GetConcurrentGetItemRequestCount(systemRequestContext);
      this.maxKeepUntilSpan = this.GetMaxKeepUntilSpan(systemRequestContext);
      this.ConfigureService(systemRequestContext, itemProviderType, shardingStrategy, itemRequestCount);
      this.ConfigureThreading(systemRequestContext);
      this.MaxWindowForGroupingReferenceDeletesInBatch = service.GetValue<int>(systemRequestContext, (RegistryQuery) ServiceRegistryConstants.GetMaxWindowForGroupingReferenceDeletesInBatchRegistryPath(this.GetExperienceName()), true, 1000);
      ShardableLocator containerContainerLocator = ItemStoreService.GetItemShardedLocator(ItemStoreService.ContainerContainerId, ItemStoreService.ContainerContainerId);
      Func<VssRequestPump.Processor, Task<bool>> func;
      int num;
      AsyncPump.Run((Func<Task>) (async () => num = await this.PumpOrInlineFromAsync<bool>(systemRequestContext, func ?? (func = (Func<VssRequestPump.Processor, Task<bool>>) (async processor =>
      {
        bool flag = !this.ItemProvider.IsReadOnly;
        if (flag)
          flag = await this.ItemProvider.GetItemAsync<ContainerItem>(processor, containerContainerLocator).ConfigureAwait(false) == null;
        return !flag || await this.ItemProvider.CompareSwapItemAsync(processor, containerContainerLocator, (StoredItem) new ContainerItem()).ConfigureAwait(false);
      }))).ConfigureAwait(true) ? 1 : 0));
    }

    protected virtual void ConfigureThreading(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      ThreadPoolHelper.IncreaseThreadCounts(service.GetValue<int>(systemRequestContext, (RegistryQuery) ServiceRegistryConstants.GetWorkerThreadsPerCoreRegistryPath(this.GetExperienceName()), true, 2048), service.GetValue<int>(systemRequestContext, (RegistryQuery) ServiceRegistryConstants.GetCompletionThreadsPerCoreRegistryPath(this.GetExperienceName()), true, 2048));
    }

    protected virtual string GetShardingStrategy(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().GetValue<string>(systemRequestContext, (RegistryQuery) ServiceRegistryConstants.GetShardingStrategyRegistryPath(this.GetExperienceName()), true, (string) null);

    protected virtual string GetItemProviderType(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      string str = systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? "SQLTable" : "AzureTable";
      IVssRequestContext requestContext = systemRequestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) ServiceRegistryConstants.GetItemProviderImplementationRegistryPath(this.GetExperienceName());
      string defaultValue = str;
      return service.GetValue<string>(requestContext, in local, true, defaultValue);
    }

    protected virtual int GetConcurrentGetItemRequestCount(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) ServiceRegistryConstants.GetConcurrentGetItemRequestCountRegistryPath(this.GetExperienceName()), true, 2);

    protected virtual TimeSpan GetMaxKeepUntilSpan(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(requestContext, (RegistryQuery) ServiceRegistryConstants.GetMaxKeepUntilSpanRegistryPath(this.GetExperienceName()), true, ItemStoreService.DefaultMaxKeepUntilSpan);

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.strongBoxItemChangeHandlers.IsValueCreated)
      {
        IVssRequestContext elevatedRequestContext = this.GetElevatedRequestContext(systemRequestContext);
        ITeamFoundationStrongBoxService service = elevatedRequestContext.GetService<ITeamFoundationStrongBoxService>();
        foreach (StrongBoxItemChangeHandler itemChangeHandler in this.strongBoxItemChangeHandlers.Value)
          service.UnregisterNotification(elevatedRequestContext, new StrongBoxItemChangedCallback(itemChangeHandler.OnStrongBoxItemChanged));
      }
      this.Dispose(true);
    }

    private static Locator ExtractPathWithinContainer(ShardableLocator shardableLocator)
    {
      IList<string> pathSegments = shardableLocator.Locator.PathSegments;
      return new Locator(pathSegments.Skip<string>(pathSegments.IndexOf("$$") + 1));
    }

    private static IdBlobReference GetBlobReferenceIdWithScope(
      Locator container,
      Locator path,
      string scope)
    {
      return ItemStoreService.GetIdBlobReferenceWithScope(ItemStoreService.GetItemShardedLocator(container, path), scope);
    }

    private static IdBlobReference GetIdBlobReferenceWithScope(
      ShardableLocator locator,
      string scope)
    {
      return new IdBlobReference(locator.Locator.Value, scope);
    }

    private static void AssertContainerName(Locator containerLocator)
    {
      if (containerLocator.PathSegments.Contains("$$"))
        throw new ArgumentException("The path segment $$ is reserved.");
    }

    private IdBlobReference GetIdBlobReference(ShardableLocator locator) => new IdBlobReference(locator.Locator.Value, this.GetReferenceScope());

    private async Task<IDictionary<Locator, bool>> CompareSwapItemsInternalAsync(
      IVssRequestContext requestContext,
      Locator containerId,
      ContainerItem containerItem,
      IReadOnlyDictionary<Locator, StoredItem> items,
      bool atomicDirectory = false)
    {
      Dictionary<ShardableLocator, StoredItem> itemsAndEtagsByShardedLocator = new Dictionary<ShardableLocator, StoredItem>();
      HashSet<Locator> parentDirectories = (HashSet<Locator>) null;
      ContainerItem containerItem1 = containerItem;
      if (containerItem1 == null)
        containerItem1 = await this.GetContainerAsync(requestContext, containerId).ConfigureAwait(true);
      containerItem = containerItem1;
      if (containerItem == null)
        throw ContainerNotFoundException.Create(containerId.Value);
      if (containerItem.DeletePending)
        throw ContainerIsDeletePendingException.Create(containerId.Value);
      if (containerItem.Sealed)
        throw ContainerIsSealedException.Create(containerId.Value);
      bool flag = false;
      foreach (KeyValuePair<Locator, StoredItem> keyValuePair in (IEnumerable<KeyValuePair<Locator, StoredItem>>) items)
      {
        if (atomicDirectory)
        {
          parentDirectories = parentDirectories ?? new HashSet<Locator>();
          parentDirectories.Add(keyValuePair.Key.GetParent());
          if (parentDirectories.Count > 1)
            throw new ArgumentException("Atomic batch operations must be performed in the same directory.");
        }
        flag |= keyValuePair.Value.StorageETag != null;
        itemsAndEtagsByShardedLocator.Add(ItemStoreService.GetItemShardedLocator(containerId, keyValuePair.Key), keyValuePair.Value);
      }
      if (containerItem.IsAppendOnly & flag)
        throw new ArgumentException("Container is append-only, but a passed item has a StorageETag to match.");
      IDictionary<ShardableLocator, bool> resultsByShardedLocator;
      if (((flag ? 1 : (!containerItem.IsAppendOnly ? 1 : 0)) | (atomicDirectory ? 1 : 0)) != 0)
      {
        resultsByShardedLocator = await this.PumpOrInlineFromAsync<IDictionary<ShardableLocator, bool>>(requestContext, (Func<VssRequestPump.Processor, Task<IDictionary<ShardableLocator, bool>>>) (vssProcessor => this.ItemProvider.CompareSwapItemsConcurrentIterator<StoredItem>(vssProcessor, (IReadOnlyDictionary<ShardableLocator, StoredItem>) itemsAndEtagsByShardedLocator, atomicDirectory).ToDictionaryAsync<ShardableLocator, bool>(vssProcessor.CancellationToken))).ConfigureAwait(true);
      }
      else
      {
        resultsByShardedLocator = await this.PumpOrInlineFromAsync<IDictionary<ShardableLocator, bool>>(requestContext, (Func<VssRequestPump.Processor, Task<IDictionary<ShardableLocator, bool>>>) (vssProcessor => this.ItemProvider.CompareSwapItemsConcurrentIterator<StoredItem>(vssProcessor, (IReadOnlyDictionary<ShardableLocator, StoredItem>) itemsAndEtagsByShardedLocator, true).ToDictionaryAsync<ShardableLocator, bool>(vssProcessor.CancellationToken))).ConfigureAwait(true);
        foreach (KeyValuePair<ShardableLocator, bool> keyValuePair in resultsByShardedLocator.Where<KeyValuePair<ShardableLocator, bool>>((Func<KeyValuePair<ShardableLocator, bool>, bool>) (kvp => kvp.Value)))
          itemsAndEtagsByShardedLocator.Remove(keyValuePair.Key);
        if (itemsAndEtagsByShardedLocator.Any<KeyValuePair<ShardableLocator, StoredItem>>())
        {
          foreach (KeyValuePair<ShardableLocator, StoredItem> keyValuePair in (IEnumerable<KeyValuePair<ShardableLocator, StoredItem>>) await this.PumpOrInlineFromAsync<IDictionary<ShardableLocator, StoredItem>>(requestContext, (Func<VssRequestPump.Processor, Task<IDictionary<ShardableLocator, StoredItem>>>) (vssProcessor => this.ItemProvider.GetItemsConcurrentIterator<StoredItem>(vssProcessor, (IEnumerable<ShardableLocator>) itemsAndEtagsByShardedLocator.Keys).ToDictionaryAsync<ShardableLocator, StoredItem>(vssProcessor.CancellationToken))).ConfigureAwait(true))
          {
            if (keyValuePair.Value != null)
              itemsAndEtagsByShardedLocator.Remove(keyValuePair.Key);
          }
          if (itemsAndEtagsByShardedLocator.Any<KeyValuePair<ShardableLocator, StoredItem>>())
          {
            Action<KeyValuePair<ShardableLocator, bool>> action;
            int num = await this.PumpOrInlineFromAsync<int>(requestContext, (Func<VssRequestPump.Processor, Task<int>>) (async vssProcessor =>
            {
              await this.ItemProvider.CompareSwapItemsConcurrentIterator<StoredItem>(vssProcessor, (IReadOnlyDictionary<ShardableLocator, StoredItem>) itemsAndEtagsByShardedLocator).ForEachAsyncNoContext<KeyValuePair<ShardableLocator, bool>>(vssProcessor.CancellationToken, action ?? (action = (Action<KeyValuePair<ShardableLocator, bool>>) (kvp => resultsByShardedLocator[kvp.Key] = kvp.Value))).ConfigureAwait(false);
              return 0;
            })).ConfigureAwait(true);
          }
        }
      }
      if (!containerItem.IsAppendOnly && !resultsByShardedLocator.All<KeyValuePair<ShardableLocator, bool>>((Func<KeyValuePair<ShardableLocator, bool>, bool>) (result => result.Value)))
      {
        IEnumerable<KeyValuePair<ShardableLocator, StoredItem>> newItemsFailedToInsert = (IEnumerable<KeyValuePair<ShardableLocator, StoredItem>>) itemsAndEtagsByShardedLocator.Where<KeyValuePair<ShardableLocator, StoredItem>>((Func<KeyValuePair<ShardableLocator, StoredItem>, bool>) (pair => pair.Value.StorageETag == null && !resultsByShardedLocator[pair.Key])).ToList<KeyValuePair<ShardableLocator, StoredItem>>();
        if (newItemsFailedToInsert.Any<KeyValuePair<ShardableLocator, StoredItem>>())
        {
          IDictionary<ShardableLocator, StoredItem> itemsToRetry = await this.PumpOrInlineFromAsync<IDictionary<ShardableLocator, StoredItem>>(requestContext, (Func<VssRequestPump.Processor, Task<IDictionary<ShardableLocator, StoredItem>>>) (vssProcessor => this.ItemProvider.GetItemsConcurrentIterator<StoredItem>(vssProcessor, newItemsFailedToInsert.Select<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator>((Func<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator>) (itemPair => itemPair.Key))).Where<KeyValuePair<ShardableLocator, StoredItem>>((Func<KeyValuePair<ShardableLocator, StoredItem>, bool>) (itemPair => itemPair.Value == null || BlobItem.IsDeleteInProgress((Microsoft.VisualStudio.Services.ItemStore.Common.Item) itemPair.Value))).ToDictionaryAsync<ShardableLocator, StoredItem>(vssProcessor.CancellationToken))).ConfigureAwait(true);
          if (itemsToRetry.Any<KeyValuePair<ShardableLocator, StoredItem>>())
          {
            Dictionary<ShardableLocator, StoredItem> dictionary = itemsToRetry.Where<KeyValuePair<ShardableLocator, StoredItem>>((Func<KeyValuePair<ShardableLocator, StoredItem>, bool>) (kvp => kvp.Value != null)).ToDictionary<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator, StoredItem>((Func<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator>) (itemPair => itemPair.Key), (Func<KeyValuePair<ShardableLocator, StoredItem>, StoredItem>) (itemPair => itemPair.Value));
            if (await this.RemoveBlobReferencesWithCorrespondingContainerItemsInBatchesAsync(requestContext, containerItem, (IDictionary<ShardableLocator, StoredItem>) dictionary, false).ConfigureAwait(true))
            {
              Dictionary<ShardableLocator, StoredItem> itemsToReinsert = atomicDirectory ? itemsAndEtagsByShardedLocator : newItemsFailedToInsert.Where<KeyValuePair<ShardableLocator, StoredItem>>((Func<KeyValuePair<ShardableLocator, StoredItem>, bool>) (itemPair => itemsToRetry.ContainsKey(itemPair.Key))).ToDictionary<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator, StoredItem>((Func<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator>) (itemPair => itemPair.Key), (Func<KeyValuePair<ShardableLocator, StoredItem>, StoredItem>) (itemPair => itemPair.Value));
              foreach (KeyValuePair<ShardableLocator, bool> keyValuePair in (IEnumerable<KeyValuePair<ShardableLocator, bool>>) await this.PumpOrInlineFromAsync<IDictionary<ShardableLocator, bool>>(requestContext, (Func<VssRequestPump.Processor, Task<IDictionary<ShardableLocator, bool>>>) (vssProcessor => this.ItemProvider.CompareSwapItemsConcurrentIterator<StoredItem>(vssProcessor, (IReadOnlyDictionary<ShardableLocator, StoredItem>) itemsToReinsert, atomicDirectory).ToDictionaryAsync<ShardableLocator, bool>(vssProcessor.CancellationToken))).ConfigureAwait(true))
              {
                if (keyValuePair.Value)
                  resultsByShardedLocator[keyValuePair.Key] = true;
              }
            }
          }
        }
      }
      IDictionary<Locator, bool> dictionary1 = (IDictionary<Locator, bool>) resultsByShardedLocator.ToDictionary<KeyValuePair<ShardableLocator, bool>, Locator, bool>((Func<KeyValuePair<ShardableLocator, bool>, Locator>) (kvp => ItemStoreService.ExtractPathWithinContainer(kvp.Key)), (Func<KeyValuePair<ShardableLocator, bool>, bool>) (kvp => kvp.Value));
      parentDirectories = (HashSet<Locator>) null;
      return dictionary1;
    }

    private async Task<bool> DeleteItemHelperAsync(
      IVssRequestContext requestContext,
      StoredItem item,
      ShardableLocator locator)
    {
      ItemStoreService itemStoreService = this;
      if (BlobItem.HasBlobId((Microsoft.VisualStudio.Services.ItemStore.Common.Item) item) && !BlobItem.IsDeleteInProgress((Microsoft.VisualStudio.Services.ItemStore.Common.Item) item))
        throw new ArgumentException("item is not in deletedInProgress state", nameof (item));
      return (await itemStoreService.DeleteItemsHelperAsync(requestContext, (IReadOnlyDictionary<ShardableLocator, StoredItem>) new Dictionary<ShardableLocator, StoredItem>()
      {
        {
          locator,
          item
        }
      }).ConfigureAwait(true))[locator];
    }

    private async Task<IReadOnlyDictionary<ShardableLocator, bool>> DeleteItemsHelperAsync(
      IVssRequestContext requestContext,
      IReadOnlyDictionary<ShardableLocator, StoredItem> itemsToDelete)
    {
      Dictionary<ShardableLocator, StoredItem> itemsToProcess = itemsToDelete.Where<KeyValuePair<ShardableLocator, StoredItem>>((Func<KeyValuePair<ShardableLocator, StoredItem>, bool>) (kvp => !BlobItem.HasBlobId((Microsoft.VisualStudio.Services.ItemStore.Common.Item) kvp.Value) || BlobItem.IsDeleteInProgress((Microsoft.VisualStudio.Services.ItemStore.Common.Item) kvp.Value))).ToDictionary<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator, StoredItem>((Func<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator>) (kvp => kvp.Key), (Func<KeyValuePair<ShardableLocator, StoredItem>, StoredItem>) (kvp => kvp.Value));
      await this.RemoveBlobReferencesAsync(requestContext, (IReadOnlyDictionary<ShardableLocator, StoredItem>) itemsToProcess).ConfigureAwait(true);
      IDictionary<ShardableLocator, bool> results = await this.PumpOrInlineFromAsync<IDictionary<ShardableLocator, bool>>(requestContext, (Func<VssRequestPump.Processor, Task<IDictionary<ShardableLocator, bool>>>) (vssProcessor => this.ItemProvider.RemoveItemsConcurrentIterator(vssProcessor, (IReadOnlyDictionary<ShardableLocator, string>) itemsToProcess.ToDictionary<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator, string>((Func<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator>) (kvp => kvp.Key), (Func<KeyValuePair<ShardableLocator, StoredItem>, string>) (kvp => kvp.Value.StorageETag))).ToDictionaryAsync<ShardableLocator, bool>(vssProcessor.CancellationToken))).ConfigureAwait(true);
      foreach (KeyValuePair<ShardableLocator, StoredItem> keyValuePair in (IEnumerable<KeyValuePair<ShardableLocator, StoredItem>>) await this.PumpOrInlineFromAsync<IDictionary<ShardableLocator, StoredItem>>(requestContext, (Func<VssRequestPump.Processor, Task<IDictionary<ShardableLocator, StoredItem>>>) (vssProcessor => this.ItemProvider.GetItemsConcurrentIterator<StoredItem>(vssProcessor, itemsToDelete.Where<KeyValuePair<ShardableLocator, StoredItem>>((Func<KeyValuePair<ShardableLocator, StoredItem>, bool>) (kvp => !results.ContainsKey(kvp.Key) || !results[kvp.Key])).Select<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator>((Func<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator>) (kvp => kvp.Key))).ToDictionaryAsync<ShardableLocator, StoredItem>(vssProcessor.CancellationToken))).ConfigureAwait(true))
        results[keyValuePair.Key] = keyValuePair.Value == null;
      foreach (KeyValuePair<ShardableLocator, StoredItem> keyValuePair in itemsToDelete.Where<KeyValuePair<ShardableLocator, StoredItem>>((Func<KeyValuePair<ShardableLocator, StoredItem>, bool>) (kvp => !results.ContainsKey(kvp.Key))))
        results[keyValuePair.Key] = false;
      return (IReadOnlyDictionary<ShardableLocator, bool>) results;
    }

    private async Task<bool> DeleteItemInternalAsync(
      IVssRequestContext requestContext,
      Locator container,
      Locator path,
      StoredItem item)
    {
      return (await this.DeleteItemsInternalAsync(requestContext, container, (IReadOnlyDictionary<Locator, StoredItem>) new Dictionary<Locator, StoredItem>()
      {
        {
          path,
          item
        }
      }).ConfigureAwait(true))[path];
    }

    private async Task<IReadOnlyDictionary<Locator, bool>> DeleteItemsInternalAsync(
      IVssRequestContext requestContext,
      Locator container,
      IReadOnlyDictionary<Locator, StoredItem> itemsToDelete)
    {
      Dictionary<Locator, bool> results = new Dictionary<Locator, bool>();
      foreach (ShardableLocator shardableLocator in (await this.PumpOrInlineFromAsync<IDictionary<ShardableLocator, bool>>(requestContext, (Func<VssRequestPump.Processor, Task<IDictionary<ShardableLocator, bool>>>) (vssProcessor => this.ItemProvider.CompareSwapItemsConcurrentIterator<StoredItem>(vssProcessor, (IReadOnlyDictionary<ShardableLocator, StoredItem>) itemsToDelete.Where<KeyValuePair<Locator, StoredItem>>((Func<KeyValuePair<Locator, StoredItem>, bool>) (kvp => BlobItem.TrySetDeleteInProgress(kvp.Value))).ToDictionary<KeyValuePair<Locator, StoredItem>, ShardableLocator, StoredItem>((Func<KeyValuePair<Locator, StoredItem>, ShardableLocator>) (kvp => ItemStoreService.GetItemShardedLocator(container, kvp.Key)), (Func<KeyValuePair<Locator, StoredItem>, StoredItem>) (kvp => kvp.Value))).ToDictionaryAsync<ShardableLocator, bool>(vssProcessor.CancellationToken))).ConfigureAwait(true)).Where<KeyValuePair<ShardableLocator, bool>>((Func<KeyValuePair<ShardableLocator, bool>, bool>) (kvp => !kvp.Value)).Select<KeyValuePair<ShardableLocator, bool>, ShardableLocator>((Func<KeyValuePair<ShardableLocator, bool>, ShardableLocator>) (kvp => kvp.Key)))
      {
        ShardableLocator locator = shardableLocator;
        StoredItem possibleBlobItem = await this.PumpOrInlineFromAsync<StoredItem>(requestContext, (Func<VssRequestPump.Processor, Task<StoredItem>>) (vssProcessor => this.ItemProvider.GetItemAsync<StoredItem>(vssProcessor, locator))).ConfigureAwait(true);
        if (possibleBlobItem == null)
          results[ItemStoreService.ExtractPathWithinContainer(locator)] = true;
        else if (!BlobItem.IsDeleteInProgress((Microsoft.VisualStudio.Services.ItemStore.Common.Item) possibleBlobItem))
          results[ItemStoreService.ExtractPathWithinContainer(locator)] = false;
      }
      foreach (KeyValuePair<ShardableLocator, bool> keyValuePair in (IEnumerable<KeyValuePair<ShardableLocator, bool>>) await this.DeleteItemsHelperAsync(requestContext, (IReadOnlyDictionary<ShardableLocator, StoredItem>) itemsToDelete.Where<KeyValuePair<Locator, StoredItem>>((Func<KeyValuePair<Locator, StoredItem>, bool>) (kvp => !results.ContainsKey(kvp.Key))).ToDictionary<KeyValuePair<Locator, StoredItem>, ShardableLocator, StoredItem>((Func<KeyValuePair<Locator, StoredItem>, ShardableLocator>) (kvp => ItemStoreService.GetItemShardedLocator(container, kvp.Key)), (Func<KeyValuePair<Locator, StoredItem>, StoredItem>) (kvp => kvp.Value))))
        results[ItemStoreService.ExtractPathWithinContainer(keyValuePair.Key)] = keyValuePair.Value;
      return (IReadOnlyDictionary<Locator, bool>) results;
    }

    private IEnumerable<StrongBoxConnectionString> GetAzureConnectionStrings(
      IVssRequestContext tfsRequestContext)
    {
      return StorageAccountConfigurationFacade.ReadAllStorageAccounts(this.GetElevatedRequestContext(tfsRequestContext));
    }

    private LocationMode? GetAzureLocationMode(IVssRequestContext tfsRequestContext) => StorageAccountConfigurationFacade.GetTableLocationMode(this.GetElevatedRequestContext(tfsRequestContext));

    private IVssRequestContext GetElevatedRequestContext(IVssRequestContext tfsRequestContext) => tfsRequestContext.To(TeamFoundationHostType.Deployment).Elevate();

    private bool TryGetBlobIdentifier(Microsoft.VisualStudio.Services.ItemStore.Common.Item item, out BlobIdentifier blobId)
    {
      if (BlobItem.HasBlobId(item))
      {
        BlobItem blobItem = item.Convert<BlobItem>();
        blobId = blobItem.BlobIdentifier;
        return true;
      }
      blobId = (BlobIdentifier) null;
      return false;
    }

    private async Task<bool> RemoveBlobReferencesWithCorrespondingContainerItemsInBatchesAsync(
      IVssRequestContext requestContext,
      ContainerItem containerItem,
      IDictionary<ShardableLocator, StoredItem> itemsToDelete,
      bool setItemsToDeleteInProgressState)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary<ShardableLocator, StoredItem>>(itemsToDelete, nameof (itemsToDelete));
      bool removalSucceeded = true;
      foreach (List<KeyValuePair<ShardableLocator, StoredItem>> pageOfItemsToDelete in itemsToDelete.GetPages<KeyValuePair<ShardableLocator, StoredItem>>(this.MaxWindowForGroupingReferenceDeletesInBatch))
      {
        Dictionary<ShardableLocator, string> deleteRequest = pageOfItemsToDelete.Where<KeyValuePair<ShardableLocator, StoredItem>>((Func<KeyValuePair<ShardableLocator, StoredItem>, bool>) (item => !BlobItem.HasBlobId((Microsoft.VisualStudio.Services.ItemStore.Common.Item) item.Value))).ToDictionary<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator, string>((Func<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator>) (item => item.Key), (Func<KeyValuePair<ShardableLocator, StoredItem>, string>) (item => item.Value.StorageETag));
        bool flag1 = removalSucceeded;
        removalSucceeded = flag1 & await this.PumpOrInlineFromAsync<bool>(requestContext, (Func<VssRequestPump.Processor, Task<bool>>) (vssProcessor => this.ItemProvider.RemoveItemsConcurrentIterator(vssProcessor, (IReadOnlyDictionary<ShardableLocator, string>) deleteRequest).AllAsyncNoContext<KeyValuePair<ShardableLocator, bool>>(vssProcessor.CancellationToken, (Func<KeyValuePair<ShardableLocator, bool>, bool>) (result => result.Value)))).ConfigureAwait(true);
        Dictionary<ShardableLocator, StoredItem> itemsWithReferencesToDelete = pageOfItemsToDelete.Where<KeyValuePair<ShardableLocator, StoredItem>>((Func<KeyValuePair<ShardableLocator, StoredItem>, bool>) (item => BlobItem.HasBlobId((Microsoft.VisualStudio.Services.ItemStore.Common.Item) item.Value))).ToDictionary<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator, StoredItem>((Func<KeyValuePair<ShardableLocator, StoredItem>, ShardableLocator>) (kvp => kvp.Key), (Func<KeyValuePair<ShardableLocator, StoredItem>, StoredItem>) (kvp => kvp.Value));
        if (setItemsToDeleteInProgressState)
        {
          foreach (KeyValuePair<ShardableLocator, StoredItem> keyValuePair in itemsWithReferencesToDelete)
            BlobItem.TrySetDeleteInProgress(keyValuePair.Value);
          if (!(await this.PumpOrInlineFromAsync<IDictionary<ShardableLocator, bool>>(requestContext, (Func<VssRequestPump.Processor, Task<IDictionary<ShardableLocator, bool>>>) (vssProcessor => this.ItemProvider.CompareSwapItemsConcurrentIterator<StoredItem>(vssProcessor, (IReadOnlyDictionary<ShardableLocator, StoredItem>) itemsWithReferencesToDelete).ToDictionaryAsync<ShardableLocator, bool>(vssProcessor.CancellationToken))).ConfigureAwait(true)).Values.All<bool>((Func<bool, bool>) (value => value)))
          {
            removalSucceeded = false;
            continue;
          }
        }
        if ((!containerItem.UseIdReferences.HasValue ? 0 : (!containerItem.UseIdReferences.Value ? 1 : 0)) == 0)
        {
          List<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>> list = itemsWithReferencesToDelete.Select<KeyValuePair<ShardableLocator, StoredItem>, Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>>(closure_7 ?? (closure_7 = (Func<KeyValuePair<ShardableLocator, StoredItem>, Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>>) (itemPair => Tuple.Create<BlobIdentifier, IdBlobReference, ShardableLocator, string>(itemPair.Value.Convert<BlobItem>().BlobIdentifier, this.GetIdBlobReference(itemPair.Key), itemPair.Key, itemPair.Value.StorageETag)))).ToList<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>>();
          IDomainId domainId = this.ValidateAndGetDomainId(itemsWithReferencesToDelete.Select<KeyValuePair<ShardableLocator, StoredItem>, IDomainId>((Func<KeyValuePair<ShardableLocator, StoredItem>, IDomainId>) (itemPair => itemPair.Value.Convert<BlobItem>().DomainId)).Distinct<IDomainId>());
          list.Sort((Comparison<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>>) ((kvp1, kvp2) => kvp1.Item1.CompareTo((object) kvp2.Item1)));
          IEnumerable<List<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>>> pages = list.GetPages<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>>(1000);
          List<KeyValuePair<BlobIdentifier, IdBlobReference>> keyValuePairList = new List<KeyValuePair<BlobIdentifier, IdBlobReference>>();
          foreach (List<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>> tupleList in pages)
          {
            foreach (Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string> tuple in tupleList)
            {
              if (tuple.Item2.IdReferenceScopedToFileList())
                keyValuePairList.Add(new KeyValuePair<BlobIdentifier, IdBlobReference>(tuple.Item1, tuple.Item2));
            }
          }
          foreach (KeyValuePair<BlobIdentifier, IdBlobReference> keyValuePair in keyValuePairList)
            requestContext.TraceAlways(ContentTracePoints.ItemStore.TraceBlobIdAndReference, "Blob ID=" + keyValuePair.Key.ValueString + "Reference name=" + keyValuePair.Value.Name + ", scope=" + keyValuePair.Value.Scope);
          await requestContext.ForkChildrenAsync<List<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>>, ItemStoreService.IRemoveBlobReferencesWithCorrespondingContainerItemsInBatchesTaskService>(8, pages, (Func<IVssRequestContext, List<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>>, Task>) (async (childRequestContext, pendingDeletePage) =>
          {
            using (requestContext.Enter(ContentTracePoints.ItemStore.RemoveBlobItemReferencesAsyncCall, nameof (RemoveBlobReferencesWithCorrespondingContainerItemsInBatchesAsync)))
            {
              bool flag2 = removalSucceeded;
              bool flag = await this.RemoveBlobItemReferencesAsync(childRequestContext, domainId, pendingDeletePage).ConfigureAwait(true);
              removalSucceeded = flag2 & flag;
            }
          })).ConfigureAwait(true);
        }
      }
      return removalSucceeded;
    }

    private async Task<bool> RemoveBlobItemReferencesAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      List<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>> pendingDeletePage)
    {
      IEnumerable<IGrouping<BlobIdentifier, IdBlobReference>> source = pendingDeletePage.GroupBy<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>, BlobIdentifier, IdBlobReference>((Func<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>, BlobIdentifier>) (kvp => kvp.Item1), (Func<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>, IdBlobReference>) (kvp => kvp.Item2));
      IBlobStore service = requestContext.GetService<IBlobStore>();
      bool flag;
      try
      {
        IEnumerable<IGrouping<BlobIdentifier, IdBlobReference>> groupings = source.Where<IGrouping<BlobIdentifier, IdBlobReference>>((Func<IGrouping<BlobIdentifier, IdBlobReference>, bool>) (g => !g.Key.IsOfNothing()));
        await service.RemoveReferencesAsync(requestContext, domainId, groupings.ToDictionaryOfEnumerables<BlobIdentifier, IdBlobReference>()).ConfigureAwait(true);
        Dictionary<ShardableLocator, string> itemsWithDeletedReferences = pendingDeletePage.ToDictionary<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>, ShardableLocator, string>((Func<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>, ShardableLocator>) (data => data.Item3), (Func<Tuple<BlobIdentifier, IdBlobReference, ShardableLocator, string>, string>) (data => data.Item4));
        flag = await this.PumpOrInlineFromAsync<bool>(requestContext, (Func<VssRequestPump.Processor, Task<bool>>) (vssProcessor => this.ItemProvider.RemoveItemsConcurrentIterator(vssProcessor, (IReadOnlyDictionary<ShardableLocator, string>) itemsWithDeletedReferences).AllAsyncNoContext<KeyValuePair<ShardableLocator, bool>>(vssProcessor.CancellationToken, (Func<KeyValuePair<ShardableLocator, bool>, bool>) (result => result.Value)))).ConfigureAwait(true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(ContentTracePoints.ItemStore.RemoveBlobItemReferencesAsyncException, ex);
        flag = false;
      }
      return flag;
    }

    private async Task RemoveBlobReferencesAsync(
      IVssRequestContext requestContext,
      IReadOnlyDictionary<ShardableLocator, StoredItem> itemsToRemoveReferencesFor)
    {
      IBlobStore blobStore;
      if (itemsToRemoveReferencesFor.IsNullOrEmpty<KeyValuePair<ShardableLocator, StoredItem>>())
      {
        blobStore = (IBlobStore) null;
      }
      else
      {
        blobStore = requestContext.GetService<IBlobStore>();
        using (requestContext.Enter(ContentTracePoints.ItemStore.RemoveBlobReferencesAsyncCall, nameof (RemoveBlobReferencesAsync)))
        {
          if (requestContext.IsFeatureEnabled("ItemStore.Features.BulkRemoveBlobRefs"))
          {
            IDictionary<BlobIdentifier, List<IdBlobReference>> source = (IDictionary<BlobIdentifier, List<IdBlobReference>>) new Dictionary<BlobIdentifier, List<IdBlobReference>>();
            StoredItem possibleBlobItem = itemsToRemoveReferencesFor.First<KeyValuePair<ShardableLocator, StoredItem>>().Value;
            IDomainId domainId = BlobItem.HasDomainId((Microsoft.VisualStudio.Services.ItemStore.Common.Item) possibleBlobItem) ? possibleBlobItem.Convert<BlobItem>().DomainId : WellKnownDomainIds.DefaultDomainId;
            foreach (KeyValuePair<ShardableLocator, StoredItem> keyValuePair in (IEnumerable<KeyValuePair<ShardableLocator, StoredItem>>) itemsToRemoveReferencesFor)
            {
              BlobIdentifier blobId;
              if (this.TryGetBlobIdentifier((Microsoft.VisualStudio.Services.ItemStore.Common.Item) keyValuePair.Value, out blobId))
              {
                IdBlobReference idBlobReference = this.GetIdBlobReference(keyValuePair.Key);
                if (idBlobReference.IdReferenceScopedToFileList())
                  requestContext.TraceAlways(ContentTracePoints.ItemStore.RemoveBlobBlobId, "Blob ID=" + blobId.ValueString + ", Reference name=" + idBlobReference.Name + ", scope=" + idBlobReference.Scope);
                if (source.ContainsKey(blobId))
                  source[blobId].Add(idBlobReference);
                else
                  source.Add(blobId, new List<IdBlobReference>()
                  {
                    idBlobReference
                  });
              }
            }
            Dictionary<BlobIdentifier, IEnumerable<IdBlobReference>> dictionary = source.ToDictionary<KeyValuePair<BlobIdentifier, List<IdBlobReference>>, BlobIdentifier, IEnumerable<IdBlobReference>>((Func<KeyValuePair<BlobIdentifier, List<IdBlobReference>>, BlobIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<BlobIdentifier, List<IdBlobReference>>, IEnumerable<IdBlobReference>>) (kvp => (IEnumerable<IdBlobReference>) kvp.Value));
            await blobStore.RemoveReferencesAsync(requestContext, domainId, (IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>>) dictionary).ConfigureAwait(true);
          }
          else
          {
            foreach (KeyValuePair<ShardableLocator, StoredItem> keyValuePair in (IEnumerable<KeyValuePair<ShardableLocator, StoredItem>>) itemsToRemoveReferencesFor)
            {
              BlobIdentifier blobId;
              if (this.TryGetBlobIdentifier((Microsoft.VisualStudio.Services.ItemStore.Common.Item) keyValuePair.Value, out blobId))
              {
                IdBlobReference idBlobReference = this.GetIdBlobReference(keyValuePair.Key);
                if (idBlobReference.IdReferenceScopedToFileList())
                  requestContext.TraceAlways(ContentTracePoints.ItemStore.RemoveBlobBlobId, "Blob ID=" + blobId.ValueString + ", Reference name=" + idBlobReference.Name + ", scope=" + idBlobReference.Scope);
                await blobStore.RemoveReferenceAsync(requestContext, BlobItem.HasDomainId((Microsoft.VisualStudio.Services.ItemStore.Common.Item) keyValuePair.Value) ? keyValuePair.Value.Convert<BlobItem>().DomainId : WellKnownDomainIds.DefaultDomainId, blobId, idBlobReference).ConfigureAwait(true);
              }
            }
          }
        }
        blobStore = (IBlobStore) null;
      }
    }

    private async Task AddItemsToContainerAsync(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Locator, StoredItem>> itemsByPath,
      Locator containerId,
      ContainerItem containerItem,
      bool throwIfAnyAlreadyExist = false)
    {
      if (itemsByPath == null)
        return;
      Dictionary<Locator, StoredItem> dictionary = itemsByPath.ToDictionary<KeyValuePair<Locator, StoredItem>, Locator, StoredItem>((Func<KeyValuePair<Locator, StoredItem>, Locator>) (kvp => kvp.Key), (Func<KeyValuePair<Locator, StoredItem>, StoredItem>) (kvp => kvp.Value));
      if (!dictionary.Any<KeyValuePair<Locator, StoredItem>>())
        return;
      if (!(await this.CompareSwapItemsInternalAsync(requestContext, containerId, containerItem, (IReadOnlyDictionary<Locator, StoredItem>) dictionary).ConfigureAwait(true)).All<KeyValuePair<Locator, bool>>((Func<KeyValuePair<Locator, bool>, bool>) (result => result.Value)) && throwIfAnyAlreadyExist)
        throw new ItemAlreadyExistsException("Item already exists in container.");
    }

    private async Task<ContainerItem> GetOrAddContainerInternalAsync(
      IVssRequestContext requestContext,
      ContainerItem container)
    {
      container = Microsoft.VisualStudio.Services.ItemStore.Common.Item.CloneWithEtag<ContainerItem>(container);
      container.SetCreationTimeToNow();
      DateTime? expirationTime;
      if (!container.TryGetExpirationTime(out expirationTime))
        container.ExpirationTime = new DateTime?();
      container.UseIdReferences = !this.CanUseTimeBasedReferences || !expirationTime.HasValue || !(expirationTime.Value < container.CreationTime.Value + this.maxKeepUntilSpan) ? new bool?(true) : new bool?(false);
      int retries = 10;
      bool tryGetFirst = this.IsReadOnly;
      Locator containerLocator = container.Name;
      while (retries > 0)
      {
        --retries;
        ContainerItem containerItem1;
        if (tryGetFirst)
        {
          containerItem1 = await this.GetContainerAsync(requestContext, containerLocator).ConfigureAwait(true);
          tryGetFirst = false;
        }
        else
        {
          container.StorageETag = (string) null;
          ContainerItem containerItem2;
          if (await this.CompareSwapContainerAsync(requestContext, container))
            containerItem2 = container;
          else
            containerItem2 = await this.GetContainerAsync(requestContext, containerLocator).ConfigureAwait(true);
          containerItem1 = containerItem2;
        }
        if (containerItem1 != null)
        {
          ContainerItem containerInternalAsync = containerItem1;
          containerLocator = (Locator) null;
          return containerInternalAsync;
        }
      }
      throw new ChangeConflictException("Could not GetOrAddContainer.");
    }

    internal Task<bool> CompareSwapContainerAsync(
      IVssRequestContext requestContext,
      ContainerItem container)
    {
      return this.PumpOrInlineFromAsync<bool>(requestContext, (Func<VssRequestPump.Processor, Task<bool>>) (vssProcessor => this.ItemProvider.CompareSwapItemAsync(vssProcessor, ItemStoreService.GetItemShardedLocator(ItemStoreService.ContainerContainerId, container.Name), (StoredItem) container)));
    }

    private static bool UpdateContainerExpirationAndCheckChangeRequiresWrite(
      ContainerItem existingContainer,
      ContainerItem newContainer)
    {
      if (existingContainer.SameExpirationValues((IExpiringItem) newContainer))
        return false;
      if (existingContainer.DeletePending)
        throw new InvalidOperationException("Cannot change the ExpirationTime of DeletePending containers");
      if (!existingContainer.Sealed)
        throw new InvalidOperationException("Container must be sealed to alter expiration.");
      DateTime? expirationTime;
      if (!newContainer.TryGetExpirationTime(out expirationTime))
        throw new InvalidOperationException("Container can only be updated with a valid expiration");
      existingContainer.ExpirationTime = expirationTime;
      existingContainer.UseIdReferences = new bool?(true);
      return true;
    }

    private static bool UpdateContainerSealAndCheckChangeRequiresWrite(
      ContainerItem existingContainer,
      ContainerItem newContainer)
    {
      if (existingContainer.Sealed != newContainer.Sealed && existingContainer.DeletePending)
        throw new InvalidOperationException("Cannot change the Sealed state of DeletePending containers");
      if (existingContainer.Sealed && !newContainer.Sealed)
        throw new InvalidOperationException("Sealed container cannot be unsealed.");
      if (existingContainer.Sealed || !newContainer.Sealed)
        return false;
      existingContainer.Sealed = true;
      return true;
    }

    private static bool UpdateContainerDeletePendingAndCheckChangeRequiresWrite(
      ContainerItem existingContainer,
      ContainerItem newContainer,
      bool undoSoftDeleteAllowed = false)
    {
      if (existingContainer.DeletePending && !newContainer.DeletePending && !undoSoftDeleteAllowed)
        throw new InvalidOperationException("Cannot change DeletePending from true to false.");
      if (!existingContainer.DeletePending && newContainer.DeletePending)
      {
        existingContainer.DeletePending = true;
        return true;
      }
      if (!undoSoftDeleteAllowed || !existingContainer.DeletePending || newContainer.DeletePending)
        return false;
      existingContainer.DeletePending = false;
      return true;
    }

    private async Task AddIdReferencesForExistingContainerBlobItemsAsync(
      IVssRequestContext requestContext,
      ContainerItem existingContainer)
    {
      string scope = this.GetReferenceScope();
      IDictionary<ShardableLocator, StoredItem> source1 = await this.PumpOrInlineFromAsync<IDictionary<ShardableLocator, StoredItem>>(requestContext, (Func<VssRequestPump.Processor, Task<IDictionary<ShardableLocator, StoredItem>>>) (vssProcessor => this.ItemProvider.GetItemsConcurrentIterator<StoredItem>(vssProcessor, ItemStoreService.GetItemShardedLocator(existingContainer.Name, Locator.Root), PathOptions.AllChildren).ToDictionaryAsync<ShardableLocator, StoredItem>(vssProcessor.CancellationToken))).ConfigureAwait(true);
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> dictionaryOfEnumerables = source1.Where<KeyValuePair<ShardableLocator, StoredItem>>((Func<KeyValuePair<ShardableLocator, StoredItem>, bool>) (kvp => BlobItem.HasVsoHashBlobId((Microsoft.VisualStudio.Services.ItemStore.Common.Item) kvp.Value))).Select<KeyValuePair<ShardableLocator, StoredItem>, KeyValuePair<BlobIdentifier, BlobReference>>((Func<KeyValuePair<ShardableLocator, StoredItem>, KeyValuePair<BlobIdentifier, BlobReference>>) (kvp => new KeyValuePair<BlobIdentifier, BlobReference>(kvp.Value.Convert<BlobItem>().BlobIdentifier, new BlobReference(ItemStoreService.GetIdBlobReferenceWithScope(kvp.Key, scope))))).GroupBy<KeyValuePair<BlobIdentifier, BlobReference>, BlobIdentifier, BlobReference>((Func<KeyValuePair<BlobIdentifier, BlobReference>, BlobIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<BlobIdentifier, BlobReference>, BlobReference>) (kvp => kvp.Value)).ToDictionaryOfEnumerables<BlobIdentifier, BlobReference>(new int?(1));
      IDomainId domainId = this.ValidateAndGetDomainId(source1.Where<KeyValuePair<ShardableLocator, StoredItem>>((Func<KeyValuePair<ShardableLocator, StoredItem>, bool>) (kvp => BlobItem.HasVsoHashBlobId((Microsoft.VisualStudio.Services.ItemStore.Common.Item) kvp.Value))).Select<KeyValuePair<ShardableLocator, StoredItem>, IDomainId>((Func<KeyValuePair<ShardableLocator, StoredItem>, IDomainId>) (kvp => kvp.Value.Convert<BlobItem>().DomainId)).Distinct<IDomainId>());
      IBlobStore blobStore = requestContext.GetService<IBlobStore>();
      IEnumerable<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> dictionaryPages = dictionaryOfEnumerables.GetDictionaryPages<BlobIdentifier, BlobReference>(2000);
      await requestContext.ForkChildrenAsync<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>, ItemStoreService.IAddIdReferencesForExistingContainerBlobItemsTaskService>(this.MaxItemStoreParallelism, dictionaryPages, (Func<IVssRequestContext, IDictionary<BlobIdentifier, IEnumerable<BlobReference>>, Task>) (async (childRequestContext, page) =>
      {
        IDictionary<BlobIdentifier, IEnumerable<BlobReference>> source2 = await blobStore.TryReferenceAsync(childRequestContext, domainId, page).ConfigureAwait(true);
        if (source2.Any<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>>())
          throw CouldNotReferenceBlobException.Create((IEnumerable<BlobIdentifier>) source2.Keys);
      })).ConfigureAwait(true);
    }

    private IDomainId ValidateAndGetDomainId(IEnumerable<IDomainId> domainIds)
    {
      IDomainId domainId = WellKnownDomainIds.DefaultDomainId;
      if (domainIds != null && domainIds.Any<IDomainId>())
        domainId = domainIds.Count<IDomainId>() <= 1 ? domainIds.Single<IDomainId>() : throw BlobItemInvalidDomainException.Create(domainIds.Count<IDomainId>().ToString());
      return domainId;
    }

    private async Task<T> PumpOrInlineFromAsync<T>(
      IVssRequestContext requestContext,
      Func<VssRequestPump.Processor, Task<T>> callback)
    {
      T retValue = default (T);
      await requestContext.PumpOrInlineFromAsync((Func<VssRequestPump.Processor, Task>) (async processor => retValue = await callback(processor).ConfigureAwait(false)), this.ItemProvider.RequiresVssRequestContext).ConfigureAwait(true);
      return retValue;
    }

    private async Task<IConcurrentIterator<T>> ForceEnumerationIfNecessaryAsync<T>(
      IConcurrentIterator<T> enumerable,
      CancellationToken cancellationToken)
    {
      if (this.ItemProvider.RequiresVssRequestContext)
        enumerable = (IConcurrentIterator<T>) new ConcurrentIterator<T>((IEnumerable<T>) await enumerable.ToListAsync<T>(cancellationToken).ConfigureAwait(true));
      return (IConcurrentIterator<T>) enumerable;
    }

    public void Dispose() => this.Dispose(true);

    [DefaultServiceImplementation(typeof (ItemStoreService.RemoveBlobReferencesWithCorrespondingContainerItemsInBatchesTaskService))]
    public interface IRemoveBlobReferencesWithCorrespondingContainerItemsInBatchesTaskService : 
      IVssTaskService,
      IVssFrameworkService
    {
    }

    private sealed class RemoveBlobReferencesWithCorrespondingContainerItemsInBatchesTaskService : 
      VssTaskService,
      ItemStoreService.IRemoveBlobReferencesWithCorrespondingContainerItemsInBatchesTaskService,
      IVssTaskService,
      IVssFrameworkService
    {
      protected override int DefaultThreadCount => 32;

      protected override TimeSpan DefaultTaskTimeout => DefaultThreadPool.DefaultDefaultTaskTimeout;
    }

    [DefaultServiceImplementation(typeof (ItemStoreService.AddIdReferencesForExistingContainerBlobItemsTaskService))]
    public interface IAddIdReferencesForExistingContainerBlobItemsTaskService : 
      IVssTaskService,
      IVssFrameworkService
    {
    }

    private sealed class AddIdReferencesForExistingContainerBlobItemsTaskService : 
      VssTaskService,
      ItemStoreService.IAddIdReferencesForExistingContainerBlobItemsTaskService,
      IVssTaskService,
      IVssFrameworkService
    {
      protected override int DefaultThreadCount => 32;

      protected override TimeSpan DefaultTaskTimeout => DefaultThreadPool.DefaultDefaultTaskTimeout;
    }
  }
}
