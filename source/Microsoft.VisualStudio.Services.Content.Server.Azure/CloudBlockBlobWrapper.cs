// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.CloudBlockBlobWrapper
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.Content.Server.Azure.Blob;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class CloudBlockBlobWrapper : ICloudBlockBlob, IListBlobItem
  {
    private static readonly string UserAgent = CloudBlockBlobWrapper.GetUserAgent();
    private readonly CloudBlockBlob blockBlob;
    private readonly CloudBlobContainerWrapper container;
    private const string VersionHeader = "x-ms-version";
    private const string DateHeader = "x-ms-date";
    private const string ClientRequestIdHeader = "x-ms-client-request-id";
    private const string ContentMd5 = "Content-MD5";
    private const string BlobContentMd5 = "x-ms-blob-content-md5";
    private const string BlobContentEncoding = "x-ms-blob-content-encoding";
    private const string keepUntilProperty = "keepUntil";
    private const string BlobKeepUntil = "x-ms-meta-keepUntil";
    private const string BlobType = "x-ms-blob-type";
    private static readonly string AssemblyVersion = FileVersionHelpers<CloudBlockBlobWrapper>.GetAssemblyVersion();
    private const int MaxHttpRequestRetryAttempts = 3;
    private static readonly HttpClient httpClient = new HttpClient();

    public CloudBlockBlobWrapper(CloudBlobContainerWrapper container, CloudBlockBlob blockBlob)
    {
      this.blockBlob = blockBlob;
      this.container = container;
    }

    static CloudBlockBlobWrapper() => BlobPropertyExtensions.InitBlobProperties();

    public StorageUri StorageUri => this.blockBlob.StorageUri;

    public Uri Uri => this.StorageUri.PrimaryUri;

    public CloudBlobDirectory Parent => this.blockBlob.Parent;

    public CloudBlobContainer Container => this.blockBlob.Container;

    public bool? IsDeleted { get; set; } = new bool?(false);

    public DateTimeOffset? DeletedTime { get; set; }

    public int RemainingRetentionDays { get; set; }

    public BlobProperties Properties => this.blockBlob.Properties;

    public string Name => this.blockBlob.Name;

    public IDictionary<string, string> Metadata => this.blockBlob.Metadata;

    public ICloudBlobContainer GetContainer() => (ICloudBlobContainer) this.container;

    public string GetSharedAccessSignature(
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers,
      string policyName)
    {
      return this.blockBlob.GetSharedAccessSignature(policy, headers, policyName, new SharedAccessProtocol?(this.container.SupportsHttps ? SharedAccessProtocol.HttpsOnly : SharedAccessProtocol.HttpsOrHttp), (IPAddressOrRange) null);
    }

    public Task PutBlockByteArrayAsync(
      VssRequestPump.Processor processor,
      string blockId,
      ArraySegment<byte> blockData,
      [Optional] OperationContext operationContext)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        return this.PutBlockHttpAsyncInternal(processor, blockId, blockData, operationContext);
      }
    }

    public Task PutBlockStreamAsync(
      VssRequestPump.Processor processor,
      string blockId,
      Stream blockData,
      OperationContext operationContext = null)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        blockData.Position = 0L;
        return this.blockBlob.PutBlockAsync(blockId, blockData, (string) null, (AccessCondition) null, (BlobRequestOptions) null, operationContext, processor.CancellationToken);
      }
    }

    public Task PutBlockListAsync(
      VssRequestPump.Processor processor,
      IEnumerable<string> blockList,
      [Optional] AccessCondition accessCondition,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        return this.blockBlob.PutBlockListAsync(blockList, accessCondition, options, operationContext, processor.CancellationToken);
      }
    }

    public Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      [Optional] DeleteSnapshotsOption deleteSnapshotsOption,
      [Optional] AccessCondition accessCondition,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        return this.blockBlob.DeleteIfExistsAsync(deleteSnapshotsOption, accessCondition, options, operationContext, processor.CancellationToken);
      }
    }

    public async Task<bool> UndeleteAsync(
      VssRequestPump.Processor processor,
      [Optional] AccessCondition accessCondition,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        try
        {
          operationContext = processor.CreateStorageContext(operationContext);
          await this.blockBlob.UndeleteAsync(accessCondition, options, operationContext, processor.CancellationToken);
          return true;
        }
        catch (StorageException ex)
        {
          if (ex.RequestInformation.HttpStatusCode == 404)
            return false;
          throw;
        }
      }
    }

    public Task<IEnumerable<ListBlockItem>> DownloadBlockListAsync(
      VssRequestPump.Processor processor,
      [Optional] BlockListingFilter blockListingFilter,
      [Optional] AccessCondition accessCondition,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        return this.blockBlob.DownloadBlockListAsync(blockListingFilter, accessCondition, options, operationContext, processor.CancellationToken);
      }
    }

    public Task<Stream> OpenReadNeedsRetryAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        return this.blockBlob.OpenReadAsync((AccessCondition) null, (BlobRequestOptions) null, operationContext, processor.CancellationToken);
      }
    }

    public Task<bool> ExistsAsync(
      VssRequestPump.Processor processor,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        return this.blockBlob.ExistsAsync((BlobRequestOptions) null, operationContext, processor.CancellationToken);
      }
    }

    public Task UploadFromByteArrayAsync(
      VssRequestPump.Processor processor,
      ArraySegment<byte> buffer,
      bool useHttpClient,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        if (useHttpClient)
          return this.UploadFromByteArrayHttpAsync(processor, buffer, this.TranslateAccessCondition(accessCondition), options, operationContext);
        operationContext = processor.CreateStorageContext(operationContext);
        return this.blockBlob.UploadFromByteArrayAsync(buffer.Array, buffer.Offset, buffer.Count, accessCondition, options, operationContext, processor.CancellationToken);
      }
    }

    public Task DownloadToStreamNeedsRetryAsync(
      VssRequestPump.Processor processor,
      Stream target,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        return this.blockBlob.DownloadToStreamAsync(target, accessCondition, options, operationContext, processor.CancellationToken);
      }
    }

    public Task FetchAttributesAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        return this.blockBlob.FetchAttributesAsync(accessCondition, options, operationContext, processor.CancellationToken);
      }
    }

    public Task SetMetadataAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        return this.blockBlob.SetMetadataAsync(accessCondition, options, operationContext, processor.CancellationToken);
      }
    }

    private Task UploadFromByteArrayHttpAsync(
      VssRequestPump.Processor processor,
      ArraySegment<byte> buffer,
      Tuple<string, string> accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      operationContext = processor.CreateStorageContext(operationContext);
      return this.UploadFromByteArrayHttpAsyncInternal(processor, buffer, accessCondition, options, operationContext);
    }

    private async Task UploadFromByteArrayHttpAsyncInternal(
      VssRequestPump.Processor processor,
      ArraySegment<byte> buffer,
      Tuple<string, string> accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      HttpRequestMessage request = (HttpRequestMessage) null;
      HttpResponseMessage response = (HttpResponseMessage) null;
      try
      {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        string sharedAccessSignature = this.blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
        {
          Permissions = SharedAccessBlobPermissions.Write,
          SharedAccessExpiryTime = new DateTimeOffset?(now.AddHours(1.0)),
          SharedAccessStartTime = new DateTimeOffset?(now.AddHours(-1.0))
        });
        string propertyName;
        if (!BlobPropertyExtensions.ArePropertiesValid(this.blockBlob.Properties, out propertyName))
          throw new PutBlobUsingHttpException("Blob " + this.blockBlob.Name + " has property " + propertyName + " that was unexpected.");
        ProductInfoHeaderValue userAgentInfo = new ProductInfoHeaderValue(nameof (UploadFromByteArrayHttpAsyncInternal), CloudBlockBlobWrapper.UserAgent);
        string query = sharedAccessSignature ?? "";
        bool appendAccessCondition = accessCondition != null;
        bool appendRequestId = operationContext?.ClientRequestID != null;
        byte[] md5BlockBlob = this.GetMd5Hash(buffer);
        string md5Base64 = Convert.ToBase64String(md5BlockBlob);
        bool appendContentEncoding = !string.IsNullOrWhiteSpace(this.blockBlob.Properties?.ContentEncoding);
        string keepUntil = (string) null;
        IDictionary<string, string> metadata1 = this.blockBlob.Metadata;
        if ((metadata1 != null ? (metadata1.Count > 1 ? 1 : 0) : 0) == 0)
        {
          IDictionary<string, string> metadata2 = this.blockBlob.Metadata;
          if ((metadata2 != null ? (metadata2.Count == 1 ? 1 : 0) : 0) == 0 || this.blockBlob.Metadata.ContainsKey("keepUntil"))
          {
            IDictionary<string, string> metadata3 = this.blockBlob.Metadata;
            bool addKeepUntil = (metadata3 != null ? (metadata3.TryGetValue("keepUntil", out keepUntil) ? 1 : 0) : 0) != 0;
            // ISSUE: variable of a compiler-generated type
            CloudBlockBlobWrapper.\u003C\u003Ec__DisplayClass58_0 cDisplayClass580;
            response = await AsyncHttpRetryHelper.InvokeAsync<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (async () =>
            {
              HttpResponseMessage httpResponseMessage;
              using (new ActivityLogPerfTimer(processor, "BlobStorage"))
              {
                // ISSUE: reference to a compiler-generated field
                cDisplayClass580.request = new HttpRequestMessage(HttpMethod.Put, this.blockBlob.Uri.AbsoluteUri + query)
                {
                  Content = (HttpContent) new ByteArrayContent(buffer.Array, buffer.Offset, buffer.Count)
                };
                request.Headers.Add("x-ms-date", now.ToString("r"));
                request.Headers.Add("x-ms-version", "2018-11-09");
                request.Headers.UserAgent.Add(userAgentInfo);
                request.Headers.Add("x-ms-blob-type", "BlockBlob");
                if (appendAccessCondition)
                  request.Headers.Add(accessCondition.Item1, accessCondition.Item2);
                if (appendRequestId)
                  request.Headers.Add("x-ms-client-request-id", operationContext.ClientRequestID);
                request.Content.Headers.ContentMD5 = md5BlockBlob;
                request.Headers.Add("x-ms-blob-content-md5", md5Base64);
                if (appendContentEncoding)
                  request.Headers.Add("x-ms-blob-content-encoding", this.blockBlob.Properties.ContentEncoding);
                if (addKeepUntil)
                  request.Headers.Add("x-ms-meta-keepUntil", keepUntil);
                httpResponseMessage = await CloudBlockBlobWrapper.httpClient.SendAsync(request, processor.CancellationToken).ConfigureAwait(false);
              }
              return httpResponseMessage;
            }), 3, (IAppTraceSource) NoopAppTraceSource.Instance, processor.CancellationToken, false, nameof (UploadFromByteArrayHttpAsyncInternal)).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
              string reasonPhrase = response.ReasonPhrase;
              HttpStatusCode statusCode = response.StatusCode;
              if (request.Content?.Headers?.ContentMD5 != null && response.Content?.Headers?.ContentMD5 != null)
              {
                string base64String1 = Convert.ToBase64String(request.Content.Headers.ContentMD5);
                string base64String2 = Convert.ToBase64String(response.Content.Headers.ContentMD5);
                if (!base64String1.Equals(base64String2, StringComparison.Ordinal))
                  throw new PutBlobUsingHttpException(SafeStringFormat.FormatSafe(string.Format("Response Code: {0}, ", (object) (int) statusCode) + "Phrase: " + reasonPhrase + ", Block blob faulted: " + this.blockBlob.Name + " md5 mismatch Request Md5: " + base64String1 + " Response Md5: " + base64String2));
              }
              throw new StorageException(new RequestResult()
              {
                HttpStatusCode = (int) statusCode
              }, reasonPhrase, (Exception) new HttpResponseException(statusCode));
            }
            string base64String3 = Convert.ToBase64String(request.Content.Headers.ContentMD5);
            string base64String4 = Convert.ToBase64String(response.Content.Headers.ContentMD5);
            if (!base64String3.Equals(base64String4, StringComparison.Ordinal))
              throw new PutBlobUsingHttpException(SafeStringFormat.FormatSafe("Fatal exception occurred - aborting... " + string.Format("Response Code: {0}, ", (object) (int) response.StatusCode) + "Phrase: " + response.ReasonPhrase + ", Block blob faulted: " + this.blockBlob.Name + " md5 mismatch Request Md5: " + base64String3 + " Response Md5: " + base64String4));
            this.blockBlob.Properties.SetBlobEtag(response.Headers.ETag.ToString());
            goto label_18;
          }
        }
        throw new PutBlobUsingHttpException("Invalid blob metadata encountered.");
      }
      finally
      {
        response?.Dispose();
      }
label_18:
      response = (HttpResponseMessage) null;
    }

    private async Task PutBlockHttpAsyncInternal(
      VssRequestPump.Processor processor,
      string blockId,
      ArraySegment<byte> blockData,
      [Optional] OperationContext operationContext)
    {
      HttpRequestMessage request = (HttpRequestMessage) null;
      HttpResponseMessage response = (HttpResponseMessage) null;
      try
      {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        string sharedAccessSignature = this.blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
        {
          Permissions = SharedAccessBlobPermissions.Write,
          SharedAccessExpiryTime = new DateTimeOffset?(now.AddHours(1.0)),
          SharedAccessStartTime = new DateTimeOffset?(now.AddHours(-1.0))
        });
        string propertyName;
        if (!BlobPropertyExtensions.ArePropertiesValid(this.blockBlob.Properties, out propertyName))
          throw new PutBlobUsingHttpException("Blob " + this.blockBlob.Name + " has property " + propertyName + " that was unexpected.");
        byte[] md5BlockBlob = this.GetMd5Hash(blockData);
        ProductInfoHeaderValue userAgentInfo = new ProductInfoHeaderValue(nameof (PutBlockHttpAsyncInternal), CloudBlockBlobWrapper.UserAgent);
        string query = sharedAccessSignature + "&comp=block&blockid=" + blockId;
        bool appendRequestId = operationContext?.ClientRequestID != null;
        // ISSUE: variable of a compiler-generated type
        CloudBlockBlobWrapper.\u003C\u003Ec__DisplayClass59_0 cDisplayClass590;
        response = await AsyncHttpRetryHelper.InvokeAsync<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (async () =>
        {
          HttpResponseMessage httpResponseMessage;
          using (new ActivityLogPerfTimer(processor, "BlobStorage"))
          {
            // ISSUE: reference to a compiler-generated field
            cDisplayClass590.request = new HttpRequestMessage(HttpMethod.Put, this.blockBlob.Uri.AbsoluteUri + query)
            {
              Content = (HttpContent) new ByteArrayContent(blockData.Array, blockData.Offset, blockData.Count)
            };
            request.Headers.Add("x-ms-date", now.ToString("r"));
            request.Headers.Add("x-ms-version", "2018-11-09");
            request.Headers.UserAgent.Add(userAgentInfo);
            request.Content.Headers.ContentMD5 = md5BlockBlob;
            if (appendRequestId)
              request.Headers.Add("x-ms-client-request-id", operationContext.ClientRequestID);
            httpResponseMessage = await CloudBlockBlobWrapper.httpClient.SendAsync(request, processor.CancellationToken).ConfigureAwait(false);
          }
          return httpResponseMessage;
        }), 3, (IAppTraceSource) NoopAppTraceSource.Instance, processor.CancellationToken, false, nameof (PutBlockHttpAsyncInternal)).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
          string reasonPhrase = response.ReasonPhrase;
          HttpStatusCode statusCode = response.StatusCode;
          if (request.Content?.Headers?.ContentMD5 != null && response.Content?.Headers?.ContentMD5 != null)
          {
            string base64String1 = Convert.ToBase64String(request.Content.Headers.ContentMD5);
            string base64String2 = Convert.ToBase64String(response.Content.Headers.ContentMD5);
            if (!base64String1.Equals(base64String2, StringComparison.Ordinal))
              throw new PutBlobUsingHttpException(SafeStringFormat.FormatSafe(string.Format("Response Code: {0}, ", (object) (int) statusCode) + "Phrase: " + reasonPhrase + ", Block faulted: " + blockId + " md5 mismatch Request Md5: " + base64String1 + " Response Md5: " + base64String2));
          }
          throw new StorageException(new RequestResult()
          {
            HttpStatusCode = (int) statusCode
          }, response.ReasonPhrase, (Exception) new HttpResponseException(statusCode));
        }
        string base64String3 = Convert.ToBase64String(request.Content.Headers.ContentMD5);
        string base64String4 = Convert.ToBase64String(response.Content.Headers.ContentMD5);
        if (!base64String3.Equals(base64String4, StringComparison.Ordinal))
          throw new PutBlobUsingHttpException(SafeStringFormat.FormatSafe("Fatal exception occurred - aborting... " + string.Format("Response Code: {0}, ", (object) (int) response.StatusCode) + "Phrase: " + response.ReasonPhrase + ", Block faulted: " + blockId + " md5 mismatch Request Md5: " + base64String3 + " Response Md5: " + base64String4));
      }
      finally
      {
        response?.Dispose();
      }
      response = (HttpResponseMessage) null;
    }

    private Tuple<string, string> TranslateAccessCondition(AccessCondition accessCondition)
    {
      Tuple<string, string> tuple = (Tuple<string, string>) null;
      if (accessCondition == null)
        return tuple;
      if (!string.IsNullOrWhiteSpace(accessCondition.IfNoneMatchETag) & !string.IsNullOrWhiteSpace(accessCondition.IfMatchETag))
        throw new PutBlobUsingHttpException("Both conditional headers were set.");
      if (!string.IsNullOrWhiteSpace(accessCondition.IfNoneMatchETag))
        return new Tuple<string, string>("If-None-Match", accessCondition.IfNoneMatchETag);
      return !string.IsNullOrWhiteSpace(accessCondition.IfMatchETag) ? new Tuple<string, string>("If-Match", accessCondition.IfMatchETag) : throw new PutBlobUsingHttpException("Unexpected conditional header was set.");
    }

    private static string GetUserAgent()
    {
      try
      {
        AssemblyFileVersionAttribute customAttribute = typeof (CloudBlockBlobWrapper).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
        if (customAttribute != null)
          return customAttribute.Version;
      }
      catch (Exception ex)
      {
      }
      return "Undefined";
    }

    private byte[] GetMd5Hash(ArraySegment<byte> blockData)
    {
      using (MD5 md5 = MD5.Create())
        return md5.ComputeHash(blockData.Array, blockData.Offset, blockData.Count);
    }
  }
}
