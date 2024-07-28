// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.AdminAzureTableBlobMetadataProviderWithTestHooks
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public class AdminAzureTableBlobMetadataProviderWithTestHooks : 
    AdminAzureTableBlobMetadataProvider,
    IAdminBlobMetadataProviderWithTestHooks,
    IAdminBlobMetadataProvider,
    IBlobMetadataProvider,
    IDisposable
  {
    private DateTime? keepUntilTimeReference;
    internal readonly ConcurrentDictionary<BlobIdentifier, TableBatchOperationResult> BatchOperationInjections = new ConcurrentDictionary<BlobIdentifier, TableBatchOperationResult>();

    public AdminAzureTableBlobMetadataProviderWithTestHooks(ITableClientFactory tableClientFactory)
      : base(tableClientFactory, new AzureTableBlobMetadataProviderOptions())
    {
    }

    public AdminAzureTableBlobMetadataProviderWithTestHooks(
      ITableClientFactory tableClientFactory,
      AzureTableBlobMetadataProviderOptions options)
      : base(tableClientFactory, options)
    {
    }

    public Task<IEnumerable<IdBlobReference>> GetIdReferencesAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      return this.GetIdReferencesInternalAsync(processor, blobId, new int?(int.MaxValue));
    }

    public void OverrideKeepUntilTimeReference(DateTime? timeReference) => this.keepUntilTimeReference = timeReference;

    public override bool HasReferences(IBlobMetadata metadata) => this.HasReferencesAtTime(metadata, this.keepUntilTimeReference.HasValue ? this.keepUntilTimeReference.Value : DateTime.UtcNow);

    public IConcurrentIterator<BlobIdentifier> EnumerateAllBlobIdentifiers(
      VssRequestPump.Processor processor)
    {
      return this.GetAllBlobIdentifiersConcurrentIterator(processor);
    }

    internal override Task<TableBatchOperationResult> RunBatchOperationAsync(
      VssRequestPump.Processor processor,
      ITable table,
      BlobIdentifier blobId,
      TableBatchOperationDescriptor batchOperation)
    {
      TableBatchOperationResult result;
      return this.BatchOperationInjections.TryRemove(blobId, out result) ? Task.FromResult<TableBatchOperationResult>(result) : base.RunBatchOperationAsync(processor, table, blobId, batchOperation);
    }
  }
}
