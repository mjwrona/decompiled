// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AzureBlobService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class AzureBlobService : IBlobService
  {
    private readonly Guid activityId;
    private readonly Guid E2EId;
    private readonly IPackagingShardProvider<string, IResolvedCloudBlobContainerFactory> shardProvider;
    private readonly ContainerAddress containerAddress;
    private readonly IAsyncInvoker invoker;
    private readonly ITracerService tracerService;
    private readonly IFactory<IRetryPolicy> azureBlobRetryPolicyFactory;
    private readonly UriSafeLocatorCodec uriSafeLocatorCodec = new UriSafeLocatorCodec();
    private readonly ICancellationFacade cancellation;

    public AzureBlobService(
      Guid activityId,
      Guid E2EId,
      IPackagingShardProvider<string, IResolvedCloudBlobContainerFactory> shardProvider,
      ContainerAddress containerAddress,
      IAsyncInvoker invoker,
      ITracerService tracerService,
      IFactory<IRetryPolicy> azureBlobRetryPolicyFactory,
      ICancellationFacade cancellation)
    {
      this.activityId = activityId;
      this.E2EId = E2EId;
      this.shardProvider = shardProvider;
      this.containerAddress = containerAddress;
      this.invoker = invoker;
      this.tracerService = tracerService;
      this.azureBlobRetryPolicyFactory = azureBlobRetryPolicyFactory;
      this.cancellation = cancellation;
    }

    public async Task<string> GetBlobAsync(Locator blobPath, Stream stream)
    {
      AzureBlobService sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetBlobAsync)))
      {
        string blobName = sendInTheThisObject.LocatorToBlobName(blobPath);
        IResolvedCloudBlobContainer container = sendInTheThisObject.shardProvider.GetShard(blobName).Get(sendInTheThisObject.containerAddress);
        ICloudBlockBlob blob = container.GetBlockBlobReference(blobName);
        try
        {
          await sendInTheThisObject.invoker.Invoke((Func<Task>) (async () =>
          {
            tracer.TraceMarker("In Invoke", blobName);
            stream.SetLength(0L);
            string contentEncoding = (string) null;
            using (InflateIfNecessaryWritableWrapperStream wrappedStream = new InflateIfNecessaryWritableWrapperStream(stream, (Func<bool>) (() =>
            {
              contentEncoding = blob.Properties.ContentEncoding;
              return contentEncoding == "deflate";
            })))
            {
              ICloudBlockBlob cloudBlockBlob = blob;
              VssRequestPump.Processor withoutRequestContext = VssRequestPump.Processor.CreateWithoutRequestContext(this.cancellation.Token, this.activityId, this.E2EId, VssClientHttpRequestSettings.Default.SessionId);
              InflateIfNecessaryWritableWrapperStream target = wrappedStream;
              OperationContext operationContext1 = this.GetTracingOperationContext(true);
              BlobRequestOptions blobRequestOptions = this.GetBlobRequestOptions();
              OperationContext operationContext2 = operationContext1;
              await cloudBlockBlob.DownloadToStreamNeedsRetryAsync(withoutRequestContext, (Stream) target, options: blobRequestOptions, operationContext: operationContext2).ConfigureAwait(false);
              tracer.TraceInfo(string.Format("Got blob {0} from container {1} in account {2}.\r\nContent-Encoding in shouldInflate: {3}\r\nDetected zlib header: {4}\r\nShouldInflate result: {5}\r\nContent-Encoding after download: {6}", (object) blob.Name, (object) container.Name, (object) container.AccountName, (object) contentEncoding, (object) wrappedStream.DetectedZLibHeader, (object) wrappedStream.ShouldInflateResult, (object) blob.Properties.ContentEncoding));
            }
          }));
          return blob.Properties.ETag;
        }
        catch (StorageException ex)
        {
          if (AzureBlobService.HasHttpStatus(ex, HttpStatusCode.NotFound))
            return (string) null;
          tracer.TraceException((Exception) ex);
          throw;
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
    }

    public async Task<IEnumerable<EtagValue<Locator>>> Under(Locator blobDirectoryPath)
    {
      AzureBlobService sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Under)))
      {
        List<CloudBlockBlobWrapper> blobItems = new List<CloudBlockBlobWrapper>();
        foreach (IResolvedCloudBlobContainerFactory allShard in sendInTheThisObject.shardProvider.GetAllShards())
        {
          IResolvedCloudBlobContainer container = allShard.Get(sendInTheThisObject.containerAddress);
          BlobContinuationToken continuationToken = (BlobContinuationToken) null;
          try
          {
            do
            {
              IBlobResultSegment blobResultSegment = await sendInTheThisObject.invoker.Invoke<IBlobResultSegment>((Func<Task<IBlobResultSegment>>) (() =>
              {
                IResolvedCloudBlobContainer cloudBlobContainer = container;
                VssRequestPump.Processor withoutRequestContext = VssRequestPump.Processor.CreateWithoutRequestContext(this.cancellation.Token, this.activityId, Guid.Empty, VssClientHttpRequestSettings.Default.SessionId);
                string blobName = this.LocatorToBlobName(blobDirectoryPath);
                int? maxResults = new int?(1000);
                BlobContinuationToken currentToken = continuationToken;
                OperationContext operationContext1 = this.GetTracingOperationContext();
                BlobRequestOptions blobRequestOptions = this.GetBlobRequestOptions();
                OperationContext operationContext2 = operationContext1;
                return cloudBlobContainer.ListBlobsSegmentedAsync(withoutRequestContext, blobName, true, BlobListingDetails.Metadata, maxResults, currentToken, blobRequestOptions, operationContext2);
              }));
              blobItems.AddRange(blobResultSegment.Results.OfType<CloudBlockBlobWrapper>());
              continuationToken = blobResultSegment.ContinuationToken;
            }
            while (continuationToken != null);
          }
          catch (StorageException ex)
          {
            if (AzureBlobService.HasHttpStatus(ex, HttpStatusCode.NotFound))
              return (IEnumerable<EtagValue<Locator>>) Array.Empty<EtagValue<Locator>>();
            throw;
          }
        }
        return blobItems.Select<CloudBlockBlobWrapper, EtagValue<Locator>>((Func<CloudBlockBlobWrapper, EtagValue<Locator>>) (item => new EtagValue<Locator>(this.BlobNameToLocator(item.Name), item.Properties.ETag)));
      }
    }

    public Task<string> PutBlobAsync(
      Locator blobPath,
      ArraySegment<byte> contentStream,
      string etag)
    {
      return this.PutBlobAsync(blobPath, contentStream, etag, false);
    }

    public async Task<string> PutBlobAsync(
      Locator blobPath,
      ArraySegment<byte> contentStream,
      string etag,
      bool deflate)
    {
      AzureBlobService sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (PutBlobAsync)))
      {
        string blobName = sendInTheThisObject.LocatorToBlobName(blobPath);
        IResolvedCloudBlobContainer container = sendInTheThisObject.shardProvider.GetShard(blobName).Get(sendInTheThisObject.containerAddress);
        ICloudBlockBlob blob = container.GetBlockBlobReference(blobName);
        if (deflate)
        {
          contentStream = CompressionHelper.DeflateByteArray(contentStream, true).AsArraySegment();
          blob.Properties.ContentEncoding = nameof (deflate);
        }
        int maxAttempts = 3;
        for (int attempts = 1; attempts <= maxAttempts; ++attempts)
        {
          object obj;
          int num1;
          try
          {
            await sendInTheThisObject.invoker.Invoke((Func<Task>) (() =>
            {
              ICloudBlockBlob cloudBlockBlob = blob;
              VssRequestPump.Processor withoutRequestContext = VssRequestPump.Processor.CreateWithoutRequestContext(this.cancellation.Token, this.activityId, this.E2EId, VssClientHttpRequestSettings.Default.SessionId);
              ArraySegment<byte> buffer = contentStream;
              AccessCondition accessCondition = etag == null ? AccessCondition.GenerateIfNoneMatchCondition("*") : AccessCondition.GenerateIfMatchCondition(etag);
              OperationContext operationContext1 = this.GetTracingOperationContext();
              BlobRequestOptions blobRequestOptions = this.GetBlobRequestOptions();
              OperationContext operationContext2 = operationContext1;
              return cloudBlockBlob.UploadFromByteArrayAsync(withoutRequestContext, buffer, false, accessCondition, blobRequestOptions, operationContext2);
            }));
            tracer.TraceInfo(string.Format("successful azure blob put against path: {0} container: {1} resulting etag: {2}", (object) blobPath, (object) container.Name, (object) blob.Properties.ETag));
            return blob.Properties.ETag;
          }
          catch (StorageException ex)
          {
            obj = (object) ex;
            num1 = 1;
          }
          if (num1 == 1)
          {
            StorageException exc = (StorageException) obj;
            if (AzureBlobService.HasHttpStatus(exc, HttpStatusCode.NotFound))
            {
              int num2 = await sendInTheThisObject.invoker.Invoke<bool>((Func<Task<bool>>) (() =>
              {
                IResolvedCloudBlobContainer cloudBlobContainer = container;
                VssRequestPump.Processor withoutRequestContext = VssRequestPump.Processor.CreateWithoutRequestContext(this.cancellation.Token, this.activityId, this.E2EId, VssClientHttpRequestSettings.Default.SessionId);
                OperationContext operationContext3 = this.GetTracingOperationContext();
                BlobRequestOptions blobRequestOptions = this.GetBlobRequestOptions();
                OperationContext operationContext4 = operationContext3;
                return cloudBlobContainer.CreateIfNotExistsAsync(withoutRequestContext, options: blobRequestOptions, operationContext: operationContext4);
              })) ? 1 : 0;
              if (attempts == maxAttempts)
              {
                if (!(obj is Exception source))
                  throw obj;
                ExceptionDispatchInfo.Capture(source).Throw();
              }
            }
            else
            {
              if (AzureBlobService.HasHttpStatus(exc, HttpStatusCode.Conflict) || AzureBlobService.HasHttpStatus(exc, HttpStatusCode.PreconditionFailed))
                return (string) null;
              if (!(obj is Exception source))
                throw obj;
              ExceptionDispatchInfo.Capture(source).Throw();
            }
          }
          obj = (object) null;
        }
        throw new InvalidOperationException("Execution reached code expected to be unreachable");
      }
    }

    public async Task<bool> DeleteBlobAsync(Locator blobPath)
    {
      AzureBlobService sendInTheThisObject = this;
      IResolvedCloudBlobContainer container;
      bool flag;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (DeleteBlobAsync)))
      {
        string blobName = sendInTheThisObject.LocatorToBlobName(blobPath);
        container = sendInTheThisObject.shardProvider.GetShard(blobName).Get(sendInTheThisObject.containerAddress);
        ICloudBlockBlob blob = container.GetBlockBlobReference(blobName);
        int num = await sendInTheThisObject.invoker.Invoke<bool>((Func<Task<bool>>) (() =>
        {
          ICloudBlockBlob cloudBlockBlob = blob;
          VssRequestPump.Processor withoutRequestContext = VssRequestPump.Processor.CreateWithoutRequestContext(this.cancellation.Token, this.activityId, this.E2EId, VssClientHttpRequestSettings.Default.SessionId);
          OperationContext operationContext1 = this.GetTracingOperationContext();
          BlobRequestOptions blobRequestOptions = this.GetBlobRequestOptions();
          OperationContext operationContext2 = operationContext1;
          return cloudBlockBlob.DeleteIfExistsAsync(withoutRequestContext, DeleteSnapshotsOption.IncludeSnapshots, options: blobRequestOptions, operationContext: operationContext2);
        })) ? 1 : 0;
        tracer.TraceInfo(string.Format("successful azure blob deletion against path: {0} container: {1}", (object) blobPath, (object) container.Name));
        flag = num != 0;
      }
      container = (IResolvedCloudBlobContainer) null;
      return flag;
    }

    private BlobRequestOptions GetBlobRequestOptions()
    {
      if (this.azureBlobRetryPolicyFactory == null)
        return (BlobRequestOptions) null;
      return new BlobRequestOptions()
      {
        RetryPolicy = this.azureBlobRetryPolicyFactory.Get()
      };
    }

    private static bool HasHttpStatus(StorageException exc, HttpStatusCode statusCode)
    {
      int? httpStatusCode = exc.RequestInformation?.HttpStatusCode;
      int num = (int) statusCode;
      return httpStatusCode.GetValueOrDefault() == num & httpStatusCode.HasValue;
    }

    private string LocatorToBlobName(Locator locator) => this.uriSafeLocatorCodec.Encode(locator).Value.TrimStart('/');

    private Locator BlobNameToLocator(string blobName) => this.uriSafeLocatorCodec.Decode(UriSafeEncodedLocator.Parse(blobName));

    private OperationContext GetTracingOperationContext(bool acceptDeflateEncoding = false)
    {
      OperationContext operationContext = new OperationContext()
      {
        UserHeaders = (IDictionary<string, string>) new Dictionary<string, string>()
      };
      if (acceptDeflateEncoding)
        operationContext.UserHeaders.Add("Accept-Encoding", "deflate");
      operationContext.RequestCompleted += (EventHandler<RequestEventArgs>) ((o, args) =>
      {
        using (new VssActivityScope(this.activityId))
        {
          string afdRefInfo = string.Empty;
          int responseCode = -1;
          if (args.Response != null)
          {
            IEnumerable<string> values;
            if (args.Response.Headers.TryGetValues("X-MSEdge-Ref", out values))
              afdRefInfo = values.First<string>();
            responseCode = (int) args.Response.StatusCode;
          }
          int totalMilliseconds = (int) (args.RequestInformation.EndTime - args.RequestInformation.StartTime).TotalMilliseconds;
          TeamFoundationTracingService.TraceHttpOutgoingRequest(args.RequestInformation.StartTime, totalMilliseconds, "Packaging.AzureBlobService", args.Request.Method.ToString(), args.Request.RequestUri.Host, args.Request.RequestUri.AbsolutePath, responseCode, this.GetErrorMessage(args), this.E2EId, afdRefInfo, "", Guid.Empty, "", "");
        }
      });
      return operationContext;
    }

    private string GetErrorMessage(RequestEventArgs args)
    {
      HttpResponseMessage response = args.Response;
      if ((response != null ? (response.StatusCode == HttpStatusCode.NotFound ? 1 : 0) : 0) != 0)
        return "ErrorMessage: " + args.RequestInformation.ExtendedErrorInformation?.ErrorMessage + "\r\nRequestID: " + args.RequestInformation.ServiceRequestID;
      return args.RequestInformation.Exception?.ToString();
    }
  }
}
