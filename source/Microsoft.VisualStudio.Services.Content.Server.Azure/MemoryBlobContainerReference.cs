// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.MemoryBlobContainerReference
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public sealed class MemoryBlobContainerReference : ICloudBlobContainer
  {
    public readonly MemoryBlobAccount Account;
    private readonly int m_listingPageSize;
    private readonly IBlobOperationListener m_opListener;

    public void UseContainer(
      Func<MemoryBlobContainer, MemoryBlobContainer> action)
    {
      this.Account.UseContainer(this.Name, action);
    }

    public MemoryBlobContainerReference(
      MemoryBlobAccount account,
      string containerName,
      IBlobOperationListener listener = null,
      int listingPageSize = 5000)
    {
      this.Account = account;
      this.Name = containerName;
      this.m_opListener = listener ?? (IBlobOperationListener) new NoOpBlobOperationListener();
      this.m_listingPageSize = listingPageSize;
    }

    public bool RequiresVssRequestContext => false;

    public string Name { get; }

    public string AccountName => this.Account.Name;

    public StorageUri StorageUri => new StorageUri(new Uri("https://" + this.AccountName + "/" + this.Name));

    public Task<IBlobResultSegment> ListBlobsSegmentedAsync(
      VssRequestPump.Processor processor,
      string prefix,
      [Optional] bool useFlatListing,
      [Optional] BlobListingDetails blobListingDetails,
      [Optional] int? maxResults,
      [Optional] BlobContinuationToken currentToken,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext)
    {
      MemoryBlobContainerReference.BlobResultSegment segment = new MemoryBlobContainerReference.BlobResultSegment();
      this.UseContainer((Func<MemoryBlobContainer, MemoryBlobContainer>) (container =>
      {
        maxResults = new int?(maxResults ?? this.m_listingPageSize);
        List<MemoryBlobReference> list = container.Blobs.Where<KeyValuePair<string, MemoryBlob>>((Func<KeyValuePair<string, MemoryBlob>, bool>) (b => b.Key.StartsWith(prefix, StringComparison.Ordinal))).Where<KeyValuePair<string, MemoryBlob>>((Func<KeyValuePair<string, MemoryBlob>, bool>) (b => currentToken == null || StringComparer.Ordinal.Compare(b.Key, currentToken.NextMarker) > 0)).OrderBy<KeyValuePair<string, MemoryBlob>, string>((Func<KeyValuePair<string, MemoryBlob>, string>) (kvp => kvp.Key)).Take<KeyValuePair<string, MemoryBlob>>(maxResults.Value).Select<KeyValuePair<string, MemoryBlob>, MemoryBlobReference>((Func<KeyValuePair<string, MemoryBlob>, MemoryBlobReference>) (b => new MemoryBlobReference(this, b.Key, b.Value, this.m_opListener))).ToList<MemoryBlobReference>();
        segment.Results = (IReadOnlyList<IListBlobItem>) list;
        if (maxResults.Value == list.Count)
          segment.ContinuationToken = new BlobContinuationToken()
          {
            NextMarker = list.Last<MemoryBlobReference>().Name
          };
        return container;
      }));
      return Task.FromResult<IBlobResultSegment>((IBlobResultSegment) segment);
    }

    public ICloudBlockBlob GetBlockBlobReference(string blobName) => (ICloudBlockBlob) new MemoryBlobReference(this, blobName, this.m_opListener);

    public Task<bool> CreateIfNotExistsAsync(
      VssRequestPump.Processor processor,
      BlobContainerPublicAccessType accessType,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      bool result = false;
      this.UseContainer((Func<MemoryBlobContainer, MemoryBlobContainer>) (container =>
      {
        if (container == null)
        {
          container = new MemoryBlobContainer(this.Account, this.Name);
          result = true;
        }
        return container;
      }));
      return Task.FromResult<bool>(result);
    }

    public Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      throw new NotImplementedException();
    }

    public string GetSharedAccessSignature(
      VssRequestPump.Processor processor,
      SharedAccessBlobPolicy policy,
      string policyName)
    {
      throw new NotImplementedException();
    }

    public Task<BlobContainerPermissions> GetPermissionsAsync(
      VssRequestPump.Processor processor,
      [Optional] OperationContext operationContext)
    {
      throw new NotImplementedException();
    }

    public Task SetPermissionsAsync(
      VssRequestPump.Processor processor,
      BlobContainerPermissions permissions,
      [Optional] OperationContext operationContext)
    {
      throw new NotImplementedException();
    }

    private class BlobResultSegment : IBlobResultSegment
    {
      public BlobContinuationToken ContinuationToken { get; set; }

      public IReadOnlyList<IListBlobItem> Results { get; set; }
    }
  }
}
