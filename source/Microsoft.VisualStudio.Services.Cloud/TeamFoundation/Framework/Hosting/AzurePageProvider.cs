// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.AzurePageProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Shared.Protocol;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  internal class AzurePageProvider : AzureProviderBase
  {
    private TimeSpan GetPageRangesTimeout = TimeSpan.FromMilliseconds(16000.0);

    protected override ICloudBlob GetCloudBlobReference(
      CloudBlobContainer container,
      string resourceId)
    {
      return (ICloudBlob) container.GetPageBlobReference(resourceId);
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
      TimeSpan? clientTimeout)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderRead.DownloadToStream." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(circuitBreakerExecutionTimeout));
      CloudPageBlob blob1 = (CloudPageBlob) blob;
      foreach (PageRange pageRange in this.GetPageRanges(requestContext, blob1, clientTimeout))
      {
        long pageRangeBytes = pageRange.EndOffset - pageRange.StartOffset + 1L;
        long num1 = Math.Min((long) blockSizeBytes, pageRangeBytes);
        long num2 = Math.Min(length, pageRangeBytes);
        long offset = pageRange.StartOffset;
        for (long remainder = num2; remainder > 0L; remainder -= num1)
        {
          Action run = (Action) (() =>
          {
            try
            {
              using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
                blob.DownloadRangeToStream(targetStream, new long?(offset), new long?(Math.Min(pageRangeBytes, remainder)), options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.DownloadToStreamClientTimeout));
            }
            catch (StorageException ex)
            {
              this.FilterStorageExceptionForCircuitBreaker(ex);
              throw;
            }
          });
          new CommandService(requestContext, setter, run).Execute();
          offset += num1;
        }
        length -= num2;
        if (length <= 0L)
          break;
      }
    }

    private IEnumerable<PageRange> GetPageRanges(
      IVssRequestContext requestContext,
      CloudPageBlob blob,
      TimeSpan? clientTimeout)
    {
      IEnumerable<PageRange> pageRanges = (IEnumerable<PageRange>) null;
      Action run = (Action) (() =>
      {
        try
        {
          CloudPageBlob cloudPageBlob = blob;
          BlobRequestOptions blobRequestOptions = this.GetBlobRequestOptions(clientTimeout, this.m_settings.GetPageRangesClientTimeout);
          long? offset = new long?();
          long? length = new long?();
          BlobRequestOptions options = blobRequestOptions;
          pageRanges = cloudPageBlob.GetPageRanges(offset, length, options: options);
        }
        catch (StorageException ex)
        {
          this.FilterStorageExceptionForCircuitBreaker(ex);
          throw;
        }
      });
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderRead.GetPageRanges." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.GetPageRangesTimeout));
      new CommandService(requestContext, setter, run).Execute();
      return pageRanges;
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
      CloudPageBlob cloudPageBlob = (CloudPageBlob) blob;
      if (contentBlockLength <= 0)
        return;
      byte[] hash = MD5.Create().ComputeHash(contentBlock, 0, contentBlockLength);
      using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
        cloudPageBlob.WritePages((Stream) new MemoryStream(contentBlock, 0, contentBlockLength, false), offset, (Checksum) Convert.ToBase64String(hash), options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.PutBlockClientTimeout));
    }

    protected override void StartCopy(
      ICloudBlob sourceBlob,
      ICloudBlob targetBlob,
      BlobRequestOptions requestOptions)
    {
      CloudPageBlob source = (CloudPageBlob) sourceBlob;
      ((CloudPageBlob) targetBlob).StartCopy(source, options: requestOptions);
    }
  }
}
