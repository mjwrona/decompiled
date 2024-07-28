// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.CodeReviewHttpClient
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  public class CodeReviewHttpClient : CodeReviewHttpClientBase
  {
    private const string ReviewApiVersion = "2.1-preview.1";
    private HttpStatusCode? lastResponseStatusCode;
    private const string JsonPatchPathStartString = "/";

    public CodeReviewHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public CodeReviewHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public CodeReviewHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public CodeReviewHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public CodeReviewHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public HttpStatusCode? LastResponseStatusCode => this.lastResponseStatusCode;

    public override Task UploadContentAsync(
      Stream uploadStream,
      Guid project,
      int reviewId,
      string contentHash,
      string fileType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.UploadContentAsync(uploadStream, project.ToString(), reviewId, contentHash, fileType, userState, cancellationToken);
    }

    public override Task UploadContentAsync(
      Stream uploadStream,
      string project,
      int reviewId,
      string contentHash,
      string fileType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("38f9ad45-10bc-4c0a-99ad-beaaa51ca027");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      if (uploadStream.Length > 0L)
        httpContent.Headers.ContentRange = new ContentRangeHeaderValue(0L, uploadStream.Length - 1L, uploadStream.Length);
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(contentHash))
        collection.Add(nameof (contentHash), contentHash);
      if (!string.IsNullOrEmpty(fileType))
        collection.Add(nameof (fileType), fileType);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("2.1-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return (Task) this.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public async Task UploadContentAsync(
      string fileName,
      Guid project,
      int reviewId,
      string contentHash,
      string fileType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (FileStream uploadStream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
        await this.UploadContentAsync((Stream) uploadStream, project, reviewId, contentHash, fileType, userState, cancellationToken);
    }

    public virtual async Task<ReviewFilesZipContentMetadata> DownloadContentsBatchZipAsync(
      DownloadContentsCriteria downloadContentsCriteria,
      string project,
      int reviewId,
      int? top = null,
      int? skip = null,
      string downloadFileName = null,
      string downloadDirectory = null,
      bool shouldOverwriteContent = true,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClient reviewHttpClient = this;
      if (!string.IsNullOrEmpty(downloadDirectory) && !Directory.Exists(downloadDirectory))
        throw new DirectoryNotFoundException(CodeReviewWebAPIResources.DownloadDirectoryNotFound((object) downloadDirectory));
      ReviewFilesZipContent reviewFilesZipContent = await reviewHttpClient.DownloadContentsBatchZipAsync(downloadContentsCriteria, project, reviewId, top, skip, downloadFileName, userState, cancellationToken);
      int num = reviewHttpClient.SaveZipStream(reviewFilesZipContent.ZipStream, downloadDirectory, shouldOverwriteContent);
      return new ReviewFilesZipContentMetadata()
      {
        NextTop = reviewFilesZipContent.NextTop,
        NextSkip = reviewFilesZipContent.NextSkip,
        FilesCount = num
      };
    }

    public virtual async Task<ReviewFilesZipContentMetadata> DownloadContentsBatchZipAsync(
      DownloadContentsCriteria downloadContentsCriteria,
      Guid project,
      int reviewId,
      int? top = null,
      int? skip = null,
      string downloadFileName = null,
      string downloadDirectory = null,
      bool shouldOverwriteContent = true,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.DownloadContentsBatchZipAsync(downloadContentsCriteria, project.ToString(), reviewId, top, skip, downloadFileName, downloadDirectory, shouldOverwriteContent, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> CreateReviewPropertiesAsync(
      PropertiesCollection properties,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.CreateReviewPropertiesAsync(properties, project.ToString(), reviewId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> CreateReviewPropertiesAsync(
      PropertiesCollection properties,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClient reviewHttpClient = this;
      return await reviewHttpClient.CreateOrUpdateReviewPropertiesAsync(reviewHttpClient.CreateJsonPatchDocument(properties, Operation.Add), project, reviewId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> UpdateReviewPropertiesAsync(
      PropertiesCollection properties,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.UpdateReviewPropertiesAsync(properties, project.ToString(), reviewId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> UpdateReviewPropertiesAsync(
      PropertiesCollection properties,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClient reviewHttpClient = this;
      return await reviewHttpClient.CreateOrUpdateReviewPropertiesAsync(reviewHttpClient.CreateJsonPatchDocument(properties, Operation.Replace), project, reviewId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> RemoveReviewPropertiesAsync(
      IEnumerable<string> properties,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.RemoveReviewPropertiesAsync(properties, project.ToString(), reviewId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> RemoveReviewPropertiesAsync(
      IEnumerable<string> properties,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClient reviewHttpClient = this;
      return await reviewHttpClient.CreateOrUpdateReviewPropertiesAsync(reviewHttpClient.CreateJsonPatchDocument(properties), project, reviewId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> CreateIterationPropertiesAsync(
      PropertiesCollection properties,
      Guid project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.CreateIterationPropertiesAsync(properties, project.ToString(), reviewId, iterationId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> CreateIterationPropertiesAsync(
      PropertiesCollection properties,
      string project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClient reviewHttpClient = this;
      return await reviewHttpClient.CreateOrUpdateIterationPropertiesAsync(reviewHttpClient.CreateJsonPatchDocument(properties, Operation.Add), project, reviewId, iterationId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> UpdateIterationPropertiesAsync(
      PropertiesCollection properties,
      Guid project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.UpdateIterationPropertiesAsync(properties, project.ToString(), reviewId, iterationId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> UpdateIterationPropertiesAsync(
      PropertiesCollection properties,
      string project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClient reviewHttpClient = this;
      return await reviewHttpClient.CreateOrUpdateIterationPropertiesAsync(reviewHttpClient.CreateJsonPatchDocument(properties, Operation.Replace), project, reviewId, iterationId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> RemoveIterationPropertiesAsync(
      IEnumerable<string> properties,
      Guid project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.RemoveIterationPropertiesAsync(properties, project.ToString(), reviewId, iterationId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> RemoveIterationPropertiesAsync(
      IEnumerable<string> properties,
      string project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClient reviewHttpClient = this;
      return await reviewHttpClient.CreateOrUpdateIterationPropertiesAsync(reviewHttpClient.CreateJsonPatchDocument(properties), project, reviewId, iterationId, userState, cancellationToken);
    }

    public virtual Task<IPagedList<Review>> GetReviewsPagedAsync(
      string project,
      ReviewSearchCriteria searchCriteria,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d17478c8-387d-4359-ba97-1414ae770b76");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<Review>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<Review>>>(this.GetPagedList<Review>));
    }

    public virtual Task<IPagedList<Review>> GetReviewsPagedAsync(
      Guid project,
      ReviewSearchCriteria searchCriteria,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d17478c8-387d-4359-ba97-1414ae770b76");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<Review>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<Review>>>(this.GetPagedList<Review>));
    }

    public virtual Task<IPagedList<Review>> GetReviewsPagedAsync(
      ReviewSearchCriteria searchCriteria,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d17478c8-387d-4359-ba97-1414ae770b76");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<Review>>(method, locationId, version: new ApiResourceVersion("3.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<Review>>>(this.GetPagedList<Review>));
    }

    protected Task<T> SendAsync<T>(
      HttpMethod method,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      return this.SendAsync<T>(method, (IEnumerable<KeyValuePair<string, string>>) null, locationId, routeValues, version, content, queryParameters, userState, cancellationToken, processResponse);
    }

    protected async Task<T> SendAsync<T>(
      HttpMethod method,
      IEnumerable<KeyValuePair<string, string>> additionalHeaders,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      CodeReviewHttpClient reviewHttpClient = this;
      T obj;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await reviewHttpClient.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          obj = await reviewHttpClient.SendAsync<T>(requestMessage, userState, cancellationToken, processResponse).ConfigureAwait(false);
      }
      return obj;
    }

    protected async Task<T> SendAsync<T>(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      CodeReviewHttpClient reviewHttpClient = this;
      if (processResponse == null)
        processResponse = new Func<HttpResponseMessage, CancellationToken, Task<T>>(((VssHttpClientBase) reviewHttpClient).ReadContentAsAsync<T>);
      T obj;
      using (HttpResponseMessage response = await reviewHttpClient.SendAsync(message, userState, cancellationToken).ConfigureAwait(false))
        obj = await processResponse(response, cancellationToken).ConfigureAwait(false);
      return obj;
    }

    protected async Task<IPagedList<T>> GetPagedList<T>(
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      CodeReviewHttpClient reviewHttpClient = this;
      string continuationToken = reviewHttpClient.GetContinuationToken(responseMessage);
      IPagedList<T> pagedList = (IPagedList<T>) new PagedList<T>((IEnumerable<T>) await reviewHttpClient.ReadContentAsAsync<List<T>>(responseMessage, cancellationToken).ConfigureAwait(false), continuationToken);
      continuationToken = (string) null;
      return pagedList;
    }

    protected string GetContinuationToken(HttpResponseMessage responseMessage)
    {
      string continuationToken = (string) null;
      IEnumerable<string> values = (IEnumerable<string>) null;
      if (responseMessage.Headers.TryGetValues("x-CodeReview-ContinuationToken", out values))
        continuationToken = values.FirstOrDefault<string>();
      return continuationToken;
    }

    protected override bool ShouldThrowError(HttpResponseMessage response) => base.ShouldThrowError(response) && response.StatusCode != HttpStatusCode.NotModified;

    protected override Task HandleResponseAsync(
      HttpResponseMessage response,
      CancellationToken cancellationToken)
    {
      this.lastResponseStatusCode = new HttpStatusCode?(response.StatusCode);
      return base.HandleResponseAsync(response, cancellationToken);
    }

    private int SaveZipStream(
      Stream zipStream,
      string downloadDirectory,
      bool shouldOverwriteContent)
    {
      int count;
      using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
      {
        count = zipArchive.Entries.Count;
        foreach (ZipArchiveEntry entry in zipArchive.Entries)
        {
          using (Stream stream = entry.Open())
          {
            string path = Path.Combine(downloadDirectory, entry.Name);
            if (!shouldOverwriteContent)
            {
              if (System.IO.File.Exists(path))
                continue;
            }
            using (FileStream destination = new FileStream(path, FileMode.Create, FileAccess.Write))
              stream.CopyTo((Stream) destination);
          }
        }
      }
      return count;
    }

    private JsonPatchDocument CreateJsonPatchDocument(
      PropertiesCollection properties,
      Operation operation)
    {
      JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
      if (properties != null && properties.Any<KeyValuePair<string, object>>())
      {
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
          jsonPatchDocument.Add(new JsonPatchOperation()
          {
            Path = this.NormalizeJsonPatchPath(property.Key),
            Value = property.Value,
            Operation = operation
          });
      }
      return jsonPatchDocument;
    }

    private JsonPatchDocument CreateJsonPatchDocument(IEnumerable<string> properties)
    {
      JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
      if (properties != null && properties.Any<string>())
      {
        foreach (string property in properties)
          jsonPatchDocument.Add(new JsonPatchOperation()
          {
            Path = this.NormalizeJsonPatchPath(property),
            Operation = Operation.Remove
          });
      }
      return jsonPatchDocument;
    }

    private string NormalizeJsonPatchPath(string key) => key.StartsWith("/") ? key : string.Format("{0}{1}", (object) "/", (object) key);
  }
}
