// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.MemoryBlobReference
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public sealed class MemoryBlobReference : ICloudBlockBlob, IListBlobItem
  {
    public readonly MemoryBlobContainerReference ContainerReference;
    private readonly IBlobOperationListener m_opListener;

    public void UseBlob(Func<MemoryBlob, MemoryBlob> action) => this.ContainerReference.UseContainer((Func<MemoryBlobContainer, MemoryBlobContainer>) (container =>
    {
      if (container == null)
        throw TestUtilities.BuildStorageException(HttpStatusCode.NotFound);
      container.UseBlob(this.Name, (Func<MemoryBlob, MemoryBlob>) (blob =>
      {
        blob = action(blob);
        this.ReadAttributes(blob);
        return blob;
      }));
      return container;
    }));

    public MemoryBlobReference(
      MemoryBlobContainerReference containerReference,
      string name,
      MemoryBlob blob,
      IBlobOperationListener opListener)
    {
      this.ContainerReference = containerReference;
      this.Name = name;
      this.m_opListener = opListener;
      this.ReadAttributes(blob);
    }

    public MemoryBlobReference(
      MemoryBlobContainerReference containerReference,
      string name,
      IBlobOperationListener opListener)
    {
      this.ContainerReference = containerReference;
      this.Name = name;
      this.m_opListener = opListener;
    }

    public BlobProperties Properties { get; private set; } = new BlobProperties();

    public string Name { get; }

    public IDictionary<string, string> Metadata { get; private set; } = (IDictionary<string, string>) new Dictionary<string, string>();

    public StorageUri StorageUri => new StorageUri(new Uri("https://" + this.ContainerReference.AccountName + "/" + this.ContainerReference.Name + "/" + this.Name));

    public Uri Uri => (Uri) null;

    public CloudBlobDirectory Parent => throw new NotImplementedException();

    public CloudBlobContainer Container => throw new NotImplementedException();

    public ICloudBlobContainer GetContainer() => (ICloudBlobContainer) this.ContainerReference;

    public string GetSharedAccessSignature(
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers,
      string policyName)
    {
      return policyName + "_" + policy.Permissions.ToString();
    }

    public Task PutBlockByteArrayAsync(
      VssRequestPump.Processor processor,
      string blockId,
      ArraySegment<byte> blockData,
      OperationContext operationContext = null)
    {
      throw new NotImplementedException();
    }

    public Task PutBlockListAsync(
      VssRequestPump.Processor processor,
      IEnumerable<string> blockList,
      [Optional] AccessCondition accessCondition,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext)
    {
      throw new NotImplementedException();
    }

    public Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      [Optional] DeleteSnapshotsOption deleteSnapshotsOption,
      [Optional] AccessCondition accessCondition,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext)
    {
      bool deleted = false;
      this.UseBlob((Func<MemoryBlob, MemoryBlob>) (blob =>
      {
        this.m_opListener.OnDeleteIfExists(this.Name, blob);
        this.EnforceAccessCondition(accessCondition, blob);
        if (blob != null)
        {
          deleted = true;
          blob = (MemoryBlob) null;
        }
        return blob;
      }));
      return Task.FromResult<bool>(deleted);
    }

    public Task<bool> UndeleteAsync(
      VssRequestPump.Processor processor,
      [Optional] AccessCondition accessCondition,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext)
    {
      return Task.FromResult<bool>(false);
    }

    public Task<IEnumerable<ListBlockItem>> DownloadBlockListAsync(
      VssRequestPump.Processor processor,
      [Optional] BlockListingFilter blockListingFilter,
      [Optional] AccessCondition accessCondition,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext)
    {
      throw new NotImplementedException();
    }

    public Task UploadFromByteArrayHttpAsync(
      VssRequestPump.Processor processor,
      ArraySegment<byte> buffer,
      Tuple<string, string> accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      throw new NotImplementedException();
    }

    public Task<Stream> OpenReadNeedsRetryAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      Stream stream = (Stream) null;
      this.UseBlob((Func<MemoryBlob, MemoryBlob>) (blob =>
      {
        stream = blob != null ? (Stream) new MemoryStream(blob.Content.ToArray()) : throw TestUtilities.BuildStorageException(HttpStatusCode.NotFound);
        return blob;
      }));
      return Task.FromResult<Stream>(stream);
    }

    public Task DownloadToStreamNeedsRetryAsync(
      VssRequestPump.Processor processor,
      Stream target,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UseBlob((Func<MemoryBlob, MemoryBlob>) (blob =>
      {
        this.m_opListener.OnDownloadToStreamNeedsRetry(this.Name, blob);
        if (blob == null)
          throw TestUtilities.BuildStorageException(HttpStatusCode.NotFound);
        new MemoryStream(blob.Content.ToArray()).CopyTo(target);
        return blob;
      }));
      return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(
      VssRequestPump.Processor processor,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      bool result = false;
      this.UseBlob((Func<MemoryBlob, MemoryBlob>) (blob =>
      {
        result = blob != null;
        return blob;
      }));
      return Task.FromResult<bool>(result);
    }

    public Task UploadFromByteArrayAsync(
      VssRequestPump.Processor processor,
      ArraySegment<byte> buffer,
      bool useHttpClient,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UseBlob((Func<MemoryBlob, MemoryBlob>) (blob =>
      {
        this.EnforceAccessCondition(accessCondition, blob);
        return new MemoryBlob(this.Properties.ContentEncoding, this.Metadata.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, string>, string>) (kvp => kvp.Value)), new MemoryStream(buffer.CreateCopy<byte>()));
      }));
      return (Task) Task.FromResult<int>(0);
    }

    public Task FetchAttributesAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UseBlob((Func<MemoryBlob, MemoryBlob>) (blob =>
      {
        this.m_opListener.OnFetchAttributes(this.Name, blob);
        return blob != null ? blob : throw TestUtilities.BuildStorageException(HttpStatusCode.NotFound);
      }));
      return (Task) Task.FromResult<int>(0);
    }

    public Task SetMetadataAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UseBlob((Func<MemoryBlob, MemoryBlob>) (blob =>
      {
        this.m_opListener.OnSetMetadata(this.Name, blob);
        if (blob == null)
          throw TestUtilities.BuildStorageException(HttpStatusCode.NotFound);
        this.EnforceAccessCondition(accessCondition, blob);
        blob = new MemoryBlob(blob.ContentEncoding, this.Metadata.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, string>, string>) (kvp => kvp.Value)), blob.Content);
        return blob;
      }));
      return (Task) Task.FromResult<int>(0);
    }

    private void ReadAttributes(MemoryBlob blob)
    {
      this.Properties = new BlobProperties();
      if (blob == null)
      {
        this.Metadata.Clear();
      }
      else
      {
        typeof (BlobProperties).GetProperty("ETag", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue((object) this.Properties, (object) blob.ETag);
        this.Properties.ContentEncoding = blob.ContentEncoding;
        typeof (BlobProperties).GetProperty("Length", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue((object) this.Properties, (object) 1);
        this.Metadata = (IDictionary<string, string>) blob.Metadata.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, string>, string>) (kvp => kvp.Value));
      }
    }

    private void EnforceAccessCondition(AccessCondition accessCondition, MemoryBlob blob)
    {
      if (accessCondition == null)
        return;
      if (accessCondition.IfMatchETag != null && (blob == null || !accessCondition.IfMatchETag.Equals(blob.ETag)))
        throw TestUtilities.BuildStorageException(HttpStatusCode.PreconditionFailed);
      if (accessCondition.IfNoneMatchETag == null)
        return;
      if (accessCondition.IfNoneMatchETag != "*")
        throw new NotImplementedException();
      if (blob != null && blob.ETag != null)
        throw TestUtilities.BuildStorageException(HttpStatusCode.Conflict);
    }

    public Task PutBlockStreamAsync(
      VssRequestPump.Processor processor,
      string blockId,
      Stream blockData,
      OperationContext operationContext = null)
    {
      throw new NotImplementedException();
    }
  }
}
