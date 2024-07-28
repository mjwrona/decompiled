// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.AzureProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Shared.Protocol;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  public class AzureProvider : AzureProviderBase
  {
    protected override ICloudBlob GetCloudBlobReference(
      CloudBlobContainer container,
      string resourceId)
    {
      return (ICloudBlob) container.GetBlockBlobReference(resourceId);
    }

    internal override void DownloadToStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      CloudBlobContainer container,
      ICloudBlob blob,
      long length,
      int blockSizeBytes,
      TimeSpan circuitBreakerExecutionTimeout,
      TimeSpan? clientTimeout = null)
    {
      this.DownloadToStream(requestContext, containerId.ToString("n"), resourceId, targetStream, container, blob, length, blockSizeBytes, circuitBreakerExecutionTimeout, clientTimeout);
    }

    internal override void DownloadToStream(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      Stream targetStream,
      CloudBlobContainer container,
      ICloudBlob blob,
      long length,
      int blockSizeBytes,
      TimeSpan circuitBreakerExecutionTimeout,
      TimeSpan? clientTimeout = null)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderRead.DownloadToStream." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(circuitBreakerExecutionTimeout));
      long offset = 0;
      for (long remainder = length; remainder > 0L; remainder -= (long) blockSizeBytes)
      {
        Action run = (Action) (() =>
        {
          try
          {
            using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
              blob.DownloadRangeToStream(targetStream, new long?(offset), new long?(Math.Min((long) blockSizeBytes, remainder)), options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.DownloadToStreamClientTimeout));
          }
          catch (StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        new CommandService(requestContext, setter, run).Execute();
        offset += (long) blockSizeBytes;
      }
    }

    protected override void PutChunks(
      IVssRequestContext requestContext,
      CloudBlobContainer container,
      ICloudBlob blob,
      byte[] contentBlock,
      int contentBlockLength,
      long compressedLength,
      long offset,
      bool isLastChunk,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout)
    {
      CloudBlockBlob cloudBlockBlob = blob as CloudBlockBlob;
      string chunkName = this.GetChunkName(offset, (long) contentBlockLength);
      if (contentBlockLength > 0)
      {
        byte[] hash = MD5.Create().ComputeHash(contentBlock, 0, contentBlockLength);
        requestContext.Trace(14521, TraceLevel.Info, "FileService", "BlobStorage", string.Format("Uploading block store block: name: {0}, size: {1}, ", (object) chunkName, (object) contentBlock.Length) + string.Format("content length {0}, blob name: {1}", (object) contentBlockLength, (object) blob?.Name));
        using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
          cloudBlockBlob.PutBlock(chunkName, (Stream) new MemoryStream(contentBlock, 0, contentBlockLength, false), (Checksum) Convert.ToBase64String(hash), options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.PutBlockClientTimeout));
      }
      if (!isLastChunk)
        return;
      requestContext.Trace(14522, TraceLevel.Info, "FileService", "BlobStorage", "Chunk " + chunkName + " is last chunk");
      using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
        this.Commit(requestContext, blob, compressedLength, true, clientTimeout);
    }

    private string GetChunkName(long offset, long length)
    {
      byte[] inArray = new byte[16];
      long maxValue = (long) byte.MaxValue;
      for (byte index = 0; index < (byte) 8; ++index)
      {
        inArray[(int) index] = (byte) ((offset & maxValue) >> 8 * (int) index);
        inArray[(int) index + 8] = (byte) ((length & maxValue) >> 8 * (int) index);
        maxValue <<= 8;
      }
      return Convert.ToBase64String(inArray);
    }

    private void ParseChunkName(string chunkName, out long offset, out long length)
    {
      byte[] numArray = Convert.FromBase64String(chunkName);
      offset = BitConverter.ToInt64(numArray, 0);
      length = BitConverter.ToInt64(numArray, 8);
    }

    private void Commit(
      IVssRequestContext requestContext,
      ICloudBlob blob,
      long compressedLength,
      bool validateBlocks,
      TimeSpan? clientTimeout)
    {
      CloudBlockBlob cloudBlockBlob = (CloudBlockBlob) blob;
      List<ListBlockItem> list = cloudBlockBlob.DownloadBlockList(BlockListingFilter.Uncommitted).ToList<ListBlockItem>();
      list.Sort(AzureProvider.BlobChunkComparer.Comparer);
      int tracepoint = 14523;
      if (requestContext.IsTracing(tracepoint, TraceLevel.Info, "FileService", "BlobStorage"))
      {
        string str = JsonConvert.SerializeObject((object) list);
        requestContext.Trace(tracepoint, TraceLevel.Info, "FileService", "BlobStorage", "Commiting chunks: " + str);
      }
      if (validateBlocks)
      {
        long num = 0;
        foreach (ListBlockItem listBlockItem in list)
        {
          long offset;
          long length;
          this.ParseChunkName(listBlockItem.Name, out offset, out length);
          if (listBlockItem.Length != length)
            throw new InvalidOperationException(string.Format("Blob is invalid (incorrect block length). Blob: {0}, Expected Length: {1}, Actual Length: {2}", (object) blob.Name, (object) length, (object) listBlockItem.Length));
          if (num != offset)
            throw new InvalidOperationException(string.Format("Blob is incomplete (missing block). Blob: {0}, Expected Offset: {1}, Actual Offset: {2}", (object) blob.Name, (object) num, (object) offset));
          num += length;
        }
        if (compressedLength != num)
          throw new InvalidOperationException(string.Format("Blob is incomplete (incorrect length). Blob: {0}, Expected Length: {1}, Actual Length: {2}", (object) blob.Name, (object) compressedLength, (object) num));
      }
      if (clientTimeout.HasValue)
        clientTimeout = new TimeSpan?(TimeSpan.FromMilliseconds(clientTimeout.Value.TotalMilliseconds * (double) list.Count));
      cloudBlockBlob.PutBlockList(list.Select<ListBlockItem, string>((Func<ListBlockItem, string>) (c => c.Name)), options: this.GetBlobRequestOptions(clientTimeout, TimeSpan.FromSeconds(this.m_settings.PutBlockClientTimeout.TotalSeconds * (double) list.Count)));
    }

    protected override void StartCopy(
      ICloudBlob sourceBlob,
      ICloudBlob targetBlob,
      BlobRequestOptions requestOptions)
    {
      CloudBlockBlob cloudBlockBlob1 = (CloudBlockBlob) sourceBlob;
      CloudBlockBlob cloudBlockBlob2 = (CloudBlockBlob) targetBlob;
      CloudBlockBlob source = cloudBlockBlob1;
      BlobRequestOptions blobRequestOptions = requestOptions;
      StandardBlobTier? standardBlockBlobTier = new StandardBlobTier?();
      RehydratePriority? rehydratePriority = new RehydratePriority?();
      BlobRequestOptions options = blobRequestOptions;
      cloudBlockBlob2.StartCopy(source, standardBlockBlobTier, rehydratePriority, options: options);
    }

    private class BlobChunkComparer : IComparer<ListBlockItem>
    {
      private static IComparer<ListBlockItem> s_comparer;

      public int Compare(ListBlockItem x, ListBlockItem y)
      {
        byte[] numArray1 = Convert.FromBase64String(x.Name);
        byte[] numArray2 = Convert.FromBase64String(y.Name);
        long int64_1 = BitConverter.ToInt64(numArray1, 0);
        long int64_2 = BitConverter.ToInt64(numArray2, 0);
        if (int64_1 > int64_2)
          return 1;
        return int64_1 == int64_2 ? 0 : -1;
      }

      public static IComparer<ListBlockItem> Comparer
      {
        get
        {
          if (AzureProvider.BlobChunkComparer.s_comparer == null)
            AzureProvider.BlobChunkComparer.s_comparer = (IComparer<ListBlockItem>) new AzureProvider.BlobChunkComparer();
          return AzureProvider.BlobChunkComparer.s_comparer;
        }
      }
    }
  }
}
