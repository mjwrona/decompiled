// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestResults.WebApi.TestResultsHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.TestResults.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A1B70BC6-DD93-426A-A4F2-75066CF77D48
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestResults.WebApi.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.TestResults.WebApi
{
  [ResourceArea("C83EAF52-EDF3-4034-AE11-17D38F25404C")]
  public abstract class TestResultsHttpClientBase : TestResultsCompatHttpClientBase
  {
    public TestResultsHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestResultsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestResultsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestResultsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestResultsHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<TestAttachmentReference> CreateTestIterationResultAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      string project,
      int runId,
      int testCaseResultId,
      int iterationId,
      string actionPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object obj1 = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAttachmentRequestModel>(attachmentRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (iterationId), iterationId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (actionPath != null)
        collection.Add(nameof (actionPath), actionPath);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestAttachmentReference>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<TestAttachmentReference> CreateTestIterationResultAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      Guid project,
      int runId,
      int testCaseResultId,
      int iterationId,
      string actionPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object obj1 = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAttachmentRequestModel>(attachmentRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (iterationId), iterationId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (actionPath != null)
        collection.Add(nameof (actionPath), actionPath);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestAttachmentReference>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<TestAttachmentReference> CreateTestResultAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      string project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object obj1 = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAttachmentRequestModel>(attachmentRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestAttachmentReference>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestAttachmentReference> CreateTestResultAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      Guid project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object obj1 = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAttachmentRequestModel>(attachmentRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestAttachmentReference>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestAttachmentReference> CreateTestSubResultAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      string project,
      int runId,
      int testCaseResultId,
      int testSubResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object obj1 = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAttachmentRequestModel>(attachmentRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (testSubResultId), testSubResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestAttachmentReference>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<TestAttachmentReference> CreateTestSubResultAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      Guid project,
      int runId,
      int testCaseResultId,
      int testSubResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object obj1 = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAttachmentRequestModel>(attachmentRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (testSubResultId), testSubResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestAttachmentReference>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual async Task DeleteTestResultAttachmentAsync(
      string project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3"), (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestResultAttachmentAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3"), (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task<Stream> GetTestIterationAttachmentContentAsync(
      string project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (iterationId), iterationId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestIterationAttachmentContentAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (iterationId), iterationId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestIterationAttachmentZipAsync(
      string project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (iterationId), iterationId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestIterationAttachmentZipAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (iterationId), iterationId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestResultAttachmentContentAsync(
      string project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestResultAttachmentContentAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<TestAttachment>> GetTestResultAttachmentsAsync(
      string project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestAttachment>>(new HttpMethod("GET"), new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3"), (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestAttachment>> GetTestResultAttachmentsAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestAttachment>>(new HttpMethod("GET"), new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3"), (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetTestResultAttachmentZipAsync(
      string project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestResultAttachmentZipAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestSubResultAttachmentContentAsync(
      string project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      int testSubResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSubResultId), testSubResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestSubResultAttachmentContentAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      int testSubResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSubResultId), testSubResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<TestAttachment>> GetTestSubResultAttachmentsAsync(
      string project,
      int runId,
      int testCaseResultId,
      int testSubResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSubResultId), testSubResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestAttachment>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestAttachment>> GetTestSubResultAttachmentsAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int testSubResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSubResultId), testSubResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestAttachment>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetTestSubResultAttachmentZipAsync(
      string project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      int testSubResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSubResultId), testSubResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestSubResultAttachmentZipAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      int testSubResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2a632e97-e014-4275-978f-8e5c4906d4b3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSubResultId), testSubResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<TestAttachmentReference> CreateTestRunAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b5731898-8206-477a-a51d-3fdf116fc6bf");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAttachmentRequestModel>(attachmentRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestAttachmentReference>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestAttachmentReference> CreateTestRunAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b5731898-8206-477a-a51d-3fdf116fc6bf");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAttachmentRequestModel>(attachmentRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestAttachmentReference>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTestRunAttachmentAsync(
      string project,
      int runId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b5731898-8206-477a-a51d-3fdf116fc6bf"), (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestRunAttachmentAsync(
      Guid project,
      int runId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b5731898-8206-477a-a51d-3fdf116fc6bf"), (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task<Stream> GetTestRunAttachmentContentAsync(
      string project,
      int runId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b5731898-8206-477a-a51d-3fdf116fc6bf");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestRunAttachmentContentAsync(
      Guid project,
      int runId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b5731898-8206-477a-a51d-3fdf116fc6bf");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<TestAttachment>> GetTestRunAttachmentsAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestAttachment>>(new HttpMethod("GET"), new Guid("b5731898-8206-477a-a51d-3fdf116fc6bf"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestAttachment>> GetTestRunAttachmentsAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestAttachment>>(new HttpMethod("GET"), new Guid("b5731898-8206-477a-a51d-3fdf116fc6bf"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetTestRunAttachmentZipAsync(
      string project,
      int runId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b5731898-8206-477a-a51d-3fdf116fc6bf");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestRunAttachmentZipAsync(
      Guid project,
      int runId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b5731898-8206-477a-a51d-3fdf116fc6bf");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<WorkItemReference>> GetBugsLinkedToTestResultAsync(
      string project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemReference>>(new HttpMethod("GET"), new Guid("d8dbf98f-eb34-4f8d-8365-47972af34f29"), (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemReference>> GetBugsLinkedToTestResultAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemReference>>(new HttpMethod("GET"), new Guid("d8dbf98f-eb34-4f8d-8365-47972af34f29"), (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SourceViewBuildCoverage>> FetchSourceCodeCoverageReportAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a459e10b-d703-4193-b3c1-60f2287918b3");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<SourceViewBuildCoverage>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SourceViewBuildCoverage>> FetchSourceCodeCoverageReportAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a459e10b-d703-4193-b3c1-60f2287918b3");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<SourceViewBuildCoverage>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BuildCoverage>> GetBuildCodeCoverageAsync(
      string project,
      int buildId,
      int flags,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9b3e1ece-c6ab-4fbb-8167-8a32a0c92216");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (flags), flags.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<BuildCoverage>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BuildCoverage>> GetBuildCodeCoverageAsync(
      Guid project,
      int buildId,
      int flags,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9b3e1ece-c6ab-4fbb-8167-8a32a0c92216");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (flags), flags.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<BuildCoverage>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CodeCoverageSummary> GetCodeCoverageSummaryAsync(
      string project,
      int buildId,
      int? deltaBuildId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9b3e1ece-c6ab-4fbb-8167-8a32a0c92216");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (deltaBuildId.HasValue)
        keyValuePairList.Add(nameof (deltaBuildId), deltaBuildId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<CodeCoverageSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CodeCoverageSummary> GetCodeCoverageSummaryAsync(
      Guid project,
      int buildId,
      int? deltaBuildId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9b3e1ece-c6ab-4fbb-8167-8a32a0c92216");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (deltaBuildId.HasValue)
        keyValuePairList.Add(nameof (deltaBuildId), deltaBuildId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<CodeCoverageSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CodeCoverageSummary> UpdateCodeCoverageSummaryAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("9b3e1ece-c6ab-4fbb-8167-8a32a0c92216");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<CodeCoverageSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CodeCoverageSummary> UpdateCodeCoverageSummaryAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("9b3e1ece-c6ab-4fbb-8167-8a32a0c92216");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<CodeCoverageSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateCodeCoverageSummaryAsync(
      CodeCoverageData coverageData,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9b3e1ece-c6ab-4fbb-8167-8a32a0c92216");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CodeCoverageData>(coverageData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      TestResultsHttpClientBase resultsHttpClientBase2 = resultsHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await resultsHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdateCodeCoverageSummaryAsync(
      CodeCoverageData coverageData,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9b3e1ece-c6ab-4fbb-8167-8a32a0c92216");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CodeCoverageData>(coverageData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      TestResultsHttpClientBase resultsHttpClientBase2 = resultsHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await resultsHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<TestRunCoverage>> GetTestRunCodeCoverageAsync(
      string project,
      int runId,
      int flags,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5641efbc-6f9b-401a-baeb-d3da22489e5e");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (flags), flags.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestRunCoverage>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestRunCoverage>> GetTestRunCodeCoverageAsync(
      Guid project,
      int runId,
      int flags,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5641efbc-6f9b-401a-baeb-d3da22489e5e");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (flags), flags.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestRunCoverage>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetFileLevelCodeCoverageAsync(
      FileCoverageRequest fileCoverageRequest,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("4a6d0c46-51ca-45aa-9163-249cee3289b7");
      object routeValues = (object) new{ project = project };
      HttpContent content = (HttpContent) new ObjectContent<FileCoverageRequest>(fileCoverageRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), content, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetFileLevelCodeCoverageAsync(
      FileCoverageRequest fileCoverageRequest,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("4a6d0c46-51ca-45aa-9163-249cee3289b7");
      object routeValues = (object) new{ project = project };
      HttpContent content = (HttpContent) new ObjectContent<FileCoverageRequest>(fileCoverageRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), content, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<TestResultHistory> QueryTestResultHistoryAsync(
      ResultsFilter filter,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bdf7a97b-0395-4da8-9d5d-f957619327d1");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultsFilter>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultHistory>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestResultHistory> QueryTestResultHistoryAsync(
      ResultsFilter filter,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bdf7a97b-0395-4da8-9d5d-f957619327d1");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultsFilter>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultHistory>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestMessageLogDetails>> GetTestRunMessageLogsAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestMessageLogDetails>>(new HttpMethod("GET"), new Guid("e9ab0c6a-1984-418b-87c0-ee4202318ba3"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestMessageLogDetails>> GetTestRunMessageLogsAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestMessageLogDetails>>(new HttpMethod("GET"), new Guid("e9ab0c6a-1984-418b-87c0-ee4202318ba3"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PipelineTestMetrics> GetTestPipelineMetricsAsync(
      string project,
      int pipelineId,
      string stageName = null,
      string phaseName = null,
      string jobName = null,
      IEnumerable<Metrics> metricNames = null,
      bool? groupByNode = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("65f35817-86a1-4131-b38b-3ec2d4744e53");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pipelineId), pipelineId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (stageName != null)
        keyValuePairList.Add(nameof (stageName), stageName);
      if (phaseName != null)
        keyValuePairList.Add(nameof (phaseName), phaseName);
      if (jobName != null)
        keyValuePairList.Add(nameof (jobName), jobName);
      if (metricNames != null && metricNames.Any<Metrics>())
        keyValuePairList.Add(nameof (metricNames), string.Join<Metrics>(",", metricNames));
      if (groupByNode.HasValue)
        keyValuePairList.Add(nameof (groupByNode), groupByNode.Value.ToString());
      return this.SendAsync<PipelineTestMetrics>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PipelineTestMetrics> GetTestPipelineMetricsAsync(
      Guid project,
      int pipelineId,
      string stageName = null,
      string phaseName = null,
      string jobName = null,
      IEnumerable<Metrics> metricNames = null,
      bool? groupByNode = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("65f35817-86a1-4131-b38b-3ec2d4744e53");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pipelineId), pipelineId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (stageName != null)
        keyValuePairList.Add(nameof (stageName), stageName);
      if (phaseName != null)
        keyValuePairList.Add(nameof (phaseName), phaseName);
      if (jobName != null)
        keyValuePairList.Add(nameof (jobName), jobName);
      if (metricNames != null && metricNames.Any<Metrics>())
        keyValuePairList.Add(nameof (metricNames), string.Join<Metrics>(",", metricNames));
      if (groupByNode.HasValue)
        keyValuePairList.Add(nameof (groupByNode), groupByNode.Value.ToString());
      return this.SendAsync<PipelineTestMetrics>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultsDetails> GetTestResultDetailsForBuildAsync(
      string project,
      int buildId,
      string publishContext = null,
      string groupBy = null,
      string filter = null,
      string orderby = null,
      bool? shouldIncludeResults = null,
      bool? queryRunSummaryForInProgress = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a518c749-4524-45b2-a7ef-1ac009b312cd");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (groupBy != null)
        keyValuePairList.Add(nameof (groupBy), groupBy);
      if (filter != null)
        keyValuePairList.Add("$filter", filter);
      if (orderby != null)
        keyValuePairList.Add("$orderby", orderby);
      bool flag;
      if (shouldIncludeResults.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = shouldIncludeResults.Value;
        string str = flag.ToString();
        collection.Add(nameof (shouldIncludeResults), str);
      }
      if (queryRunSummaryForInProgress.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = queryRunSummaryForInProgress.Value;
        string str = flag.ToString();
        collection.Add(nameof (queryRunSummaryForInProgress), str);
      }
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultsDetails> GetTestResultDetailsForBuildAsync(
      Guid project,
      int buildId,
      string publishContext = null,
      string groupBy = null,
      string filter = null,
      string orderby = null,
      bool? shouldIncludeResults = null,
      bool? queryRunSummaryForInProgress = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a518c749-4524-45b2-a7ef-1ac009b312cd");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (groupBy != null)
        keyValuePairList.Add(nameof (groupBy), groupBy);
      if (filter != null)
        keyValuePairList.Add("$filter", filter);
      if (orderby != null)
        keyValuePairList.Add("$orderby", orderby);
      bool flag;
      if (shouldIncludeResults.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = shouldIncludeResults.Value;
        string str = flag.ToString();
        collection.Add(nameof (shouldIncludeResults), str);
      }
      if (queryRunSummaryForInProgress.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = queryRunSummaryForInProgress.Value;
        string str = flag.ToString();
        collection.Add(nameof (queryRunSummaryForInProgress), str);
      }
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultsDetails> GetTestResultDetailsForReleaseAsync(
      string project,
      int releaseId,
      int releaseEnvId,
      string publishContext = null,
      string groupBy = null,
      string filter = null,
      string orderby = null,
      bool? shouldIncludeResults = null,
      bool? queryRunSummaryForInProgress = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("19a8183a-69fb-47d7-bfbf-1b6b0d921294");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (groupBy != null)
        keyValuePairList.Add(nameof (groupBy), groupBy);
      if (filter != null)
        keyValuePairList.Add("$filter", filter);
      if (orderby != null)
        keyValuePairList.Add("$orderby", orderby);
      bool flag;
      if (shouldIncludeResults.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = shouldIncludeResults.Value;
        string str = flag.ToString();
        collection.Add(nameof (shouldIncludeResults), str);
      }
      if (queryRunSummaryForInProgress.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = queryRunSummaryForInProgress.Value;
        string str = flag.ToString();
        collection.Add(nameof (queryRunSummaryForInProgress), str);
      }
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultsDetails> GetTestResultDetailsForReleaseAsync(
      Guid project,
      int releaseId,
      int releaseEnvId,
      string publishContext = null,
      string groupBy = null,
      string filter = null,
      string orderby = null,
      bool? shouldIncludeResults = null,
      bool? queryRunSummaryForInProgress = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("19a8183a-69fb-47d7-bfbf-1b6b0d921294");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (groupBy != null)
        keyValuePairList.Add(nameof (groupBy), groupBy);
      if (filter != null)
        keyValuePairList.Add("$filter", filter);
      if (orderby != null)
        keyValuePairList.Add("$orderby", orderby);
      bool flag;
      if (shouldIncludeResults.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = shouldIncludeResults.Value;
        string str = flag.ToString();
        collection.Add(nameof (shouldIncludeResults), str);
      }
      if (queryRunSummaryForInProgress.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = queryRunSummaryForInProgress.Value;
        string str = flag.ToString();
        collection.Add(nameof (queryRunSummaryForInProgress), str);
      }
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultDocument> PublishTestResultDocumentAsync(
      TestResultDocument document,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("74838649-b038-42f1-a0e7-6deb3973bf14");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultDocument>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestResultDocument> PublishTestResultDocumentAsync(
      TestResultDocument document,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("74838649-b038-42f1-a0e7-6deb3973bf14");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultDocument>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PagedList<FieldDetailsForTestResults>> GetResultGroupsByBuildAsync(
      string project,
      int buildId,
      string publishContext,
      IEnumerable<string> fields = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e49244d1-c49f-49ad-a717-3bbaefe6a201");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<FieldDetailsForTestResults>> GetResultGroupsByBuildAsync(
      Guid project,
      int buildId,
      string publishContext,
      IEnumerable<string> fields = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e49244d1-c49f-49ad-a717-3bbaefe6a201");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<FieldDetailsForTestResults>> GetResultGroupsByReleaseAsync(
      string project,
      int releaseId,
      string publishContext,
      int? releaseEnvId = null,
      IEnumerable<string> fields = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3c2b6bb0-0620-434a-a5c3-26aa0fcfda15");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (publishContext), publishContext);
      if (releaseEnvId.HasValue)
        keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<FieldDetailsForTestResults>> GetResultGroupsByReleaseAsync(
      Guid project,
      int releaseId,
      string publishContext,
      int? releaseEnvId = null,
      IEnumerable<string> fields = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3c2b6bb0-0620-434a-a5c3-26aa0fcfda15");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (publishContext), publishContext);
      if (releaseEnvId.HasValue)
        keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestResultMetaData>> QueryTestResultsMetaDataAsync(
      IEnumerable<string> testCaseReferenceIds,
      string project,
      ResultMetaDataDetails? detailsToInclude = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b72ff4c0-4341-4213-ba27-f517cf341c95");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(testCaseReferenceIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        collection.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestResultMetaData>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<TestResultMetaData>> QueryTestResultsMetaDataAsync(
      IEnumerable<string> testCaseReferenceIds,
      Guid project,
      ResultMetaDataDetails? detailsToInclude = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b72ff4c0-4341-4213-ba27-f517cf341c95");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(testCaseReferenceIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        collection.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestResultMetaData>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<TestResultMetaData> UpdateTestResultsMetaDataAsync(
      TestResultMetaDataUpdateInput testResultMetaDataUpdateInput,
      string project,
      int testCaseReferenceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b72ff4c0-4341-4213-ba27-f517cf341c95");
      object obj1 = (object) new
      {
        project = project,
        testCaseReferenceId = testCaseReferenceId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultMetaDataUpdateInput>(testResultMetaDataUpdateInput, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultMetaData>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestResultMetaData> UpdateTestResultsMetaDataAsync(
      TestResultMetaDataUpdateInput testResultMetaDataUpdateInput,
      Guid project,
      int testCaseReferenceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b72ff4c0-4341-4213-ba27-f517cf341c95");
      object obj1 = (object) new
      {
        project = project,
        testCaseReferenceId = testCaseReferenceId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultMetaDataUpdateInput>(testResultMetaDataUpdateInput, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultMetaData>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestResultsQuery> GetTestResultsByQueryAsync(
      TestResultsQuery query,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("14033a2c-af25-4af1-9e39-8ef6900482e3");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultsQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultsQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestResultsQuery> GetTestResultsByQueryAsync(
      TestResultsQuery query,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("14033a2c-af25-4af1-9e39-8ef6900482e3");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultsQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultsQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestCaseResult>> GetTestResultsByQueryWiqlAsync(
      QueryModel queryModel,
      string project,
      bool? includeResultDetails = null,
      bool? includeIterationDetails = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5ea78be3-2f5a-4110-8034-c27f24c62db1");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryModel>(queryModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeResultDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeResultDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeResultDetails), str);
      }
      if (includeIterationDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIterationDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIterationDetails), str);
      }
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<TestCaseResult>> GetTestResultsByQueryWiqlAsync(
      QueryModel queryModel,
      Guid project,
      bool? includeResultDetails = null,
      bool? includeIterationDetails = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5ea78be3-2f5a-4110-8034-c27f24c62db1");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryModel>(queryModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeResultDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeResultDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeResultDetails), str);
      }
      if (includeIterationDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIterationDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIterationDetails), str);
      }
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<TestCaseResult>> AddTestResultsToTestRunAsync(
      TestCaseResult[] results,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("02afa165-e79a-4d70-8f0c-2af0f35b4e07");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestCaseResult[]>(results, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestCaseResult>> AddTestResultsToTestRunAsync(
      TestCaseResult[] results,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("02afa165-e79a-4d70-8f0c-2af0f35b4e07");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestCaseResult[]>(results, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestCaseResult> GetTestResultByIdAsync(
      string project,
      int runId,
      int testResultId,
      ResultDetails? detailsToInclude = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("02afa165-e79a-4d70-8f0c-2af0f35b4e07");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testResultId = testResultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        keyValuePairList.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      return this.SendAsync<TestCaseResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestCaseResult> GetTestResultByIdAsync(
      Guid project,
      int runId,
      int testResultId,
      ResultDetails? detailsToInclude = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("02afa165-e79a-4d70-8f0c-2af0f35b4e07");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testResultId = testResultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        keyValuePairList.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      return this.SendAsync<TestCaseResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestCaseResult>> GetTestResultsAsync(
      string project,
      int runId,
      ResultDetails? detailsToInclude = null,
      int? skip = null,
      int? top = null,
      IEnumerable<TestOutcome> outcomes = null,
      bool? newTestsOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("02afa165-e79a-4d70-8f0c-2af0f35b4e07");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        keyValuePairList.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (newTestsOnly.HasValue)
        keyValuePairList.Add("$newTestsOnly", newTestsOnly.Value.ToString());
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestCaseResult>> GetTestResultsAsync(
      Guid project,
      int runId,
      ResultDetails? detailsToInclude = null,
      int? skip = null,
      int? top = null,
      IEnumerable<TestOutcome> outcomes = null,
      bool? newTestsOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("02afa165-e79a-4d70-8f0c-2af0f35b4e07");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        keyValuePairList.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (newTestsOnly.HasValue)
        keyValuePairList.Add("$newTestsOnly", newTestsOnly.Value.ToString());
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestCaseResult>> UpdateTestResultsAsync(
      TestCaseResult[] results,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("02afa165-e79a-4d70-8f0c-2af0f35b4e07");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestCaseResult[]>(results, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestCaseResult>> UpdateTestResultsAsync(
      TestCaseResult[] results,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("02afa165-e79a-4d70-8f0c-2af0f35b4e07");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestCaseResult[]>(results, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PagedList<ShallowTestCaseResult>> GetTestResultsByBuildAsync(
      string project,
      int buildId,
      string publishContext = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f48cc885-dbc4-4efc-ab19-ae8c19d1e02a");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<ShallowTestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<ShallowTestCaseResult>> GetTestResultsByBuildAsync(
      Guid project,
      int buildId,
      string publishContext = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f48cc885-dbc4-4efc-ab19-ae8c19d1e02a");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<ShallowTestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<ShallowTestCaseResult>> GetTestResultsByPipelineAsync(
      string project,
      int pipelineId,
      string stageName = null,
      string phaseName = null,
      string jobName = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("80169dc2-30c3-4c25-84b2-dd67d7ff1f52");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (pipelineId), pipelineId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (stageName != null)
        keyValuePairList1.Add(nameof (stageName), stageName);
      if (phaseName != null)
        keyValuePairList1.Add(nameof (phaseName), phaseName);
      if (jobName != null)
        keyValuePairList1.Add(nameof (jobName), jobName);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList1.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
        keyValuePairList1.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<PagedList<ShallowTestCaseResult>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<ShallowTestCaseResult>> GetTestResultsByPipelineAsync(
      Guid project,
      int pipelineId,
      string stageName = null,
      string phaseName = null,
      string jobName = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("80169dc2-30c3-4c25-84b2-dd67d7ff1f52");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (pipelineId), pipelineId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (stageName != null)
        keyValuePairList1.Add(nameof (stageName), stageName);
      if (phaseName != null)
        keyValuePairList1.Add(nameof (phaseName), phaseName);
      if (jobName != null)
        keyValuePairList1.Add(nameof (jobName), jobName);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList1.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
        keyValuePairList1.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<PagedList<ShallowTestCaseResult>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<ShallowTestCaseResult>> GetTestResultsByReleaseAsync(
      string project,
      int releaseId,
      int? releaseEnvid = null,
      string publishContext = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3994b949-77e5-495d-8034-edf80d95b84e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      int num;
      if (releaseEnvid.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = releaseEnvid.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (releaseEnvid), str);
      }
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<ShallowTestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<ShallowTestCaseResult>> GetTestResultsByReleaseAsync(
      Guid project,
      int releaseId,
      int? releaseEnvid = null,
      string publishContext = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3994b949-77e5-495d-8034-edf80d95b84e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      int num;
      if (releaseEnvid.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = releaseEnvid.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (releaseEnvid), str);
      }
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<ShallowTestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultsDetails> TestResultsGroupDetailsAsync(
      string project,
      int pipelineId,
      string stageName = null,
      string phaseName = null,
      string jobName = null,
      bool? shouldIncludeFailedAndAbortedResults = null,
      bool? queryGroupSummaryForInProgress = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f903b850-06af-4b50-a344-d7bbfb19e93b");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pipelineId), pipelineId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (stageName != null)
        keyValuePairList.Add(nameof (stageName), stageName);
      if (phaseName != null)
        keyValuePairList.Add(nameof (phaseName), phaseName);
      if (jobName != null)
        keyValuePairList.Add(nameof (jobName), jobName);
      bool flag;
      if (shouldIncludeFailedAndAbortedResults.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = shouldIncludeFailedAndAbortedResults.Value;
        string str = flag.ToString();
        collection.Add(nameof (shouldIncludeFailedAndAbortedResults), str);
      }
      if (queryGroupSummaryForInProgress.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = queryGroupSummaryForInProgress.Value;
        string str = flag.ToString();
        collection.Add(nameof (queryGroupSummaryForInProgress), str);
      }
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultsDetails> TestResultsGroupDetailsAsync(
      Guid project,
      int pipelineId,
      string stageName = null,
      string phaseName = null,
      string jobName = null,
      bool? shouldIncludeFailedAndAbortedResults = null,
      bool? queryGroupSummaryForInProgress = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f903b850-06af-4b50-a344-d7bbfb19e93b");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pipelineId), pipelineId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (stageName != null)
        keyValuePairList.Add(nameof (stageName), stageName);
      if (phaseName != null)
        keyValuePairList.Add(nameof (phaseName), phaseName);
      if (jobName != null)
        keyValuePairList.Add(nameof (jobName), jobName);
      bool flag;
      if (shouldIncludeFailedAndAbortedResults.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = shouldIncludeFailedAndAbortedResults.Value;
        string str = flag.ToString();
        collection.Add(nameof (shouldIncludeFailedAndAbortedResults), str);
      }
      if (queryGroupSummaryForInProgress.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = queryGroupSummaryForInProgress.Value;
        string str = flag.ToString();
        collection.Add(nameof (queryGroupSummaryForInProgress), str);
      }
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultSummary> QueryTestResultsReportForBuildAsync(
      string project,
      int buildId,
      string publishContext = null,
      bool? includeFailureDetails = null,
      BuildReference buildToCompare = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e009fa95-95a5-4ad4-9681-590043ce2423");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (includeFailureDetails.HasValue)
        keyValuePairList.Add(nameof (includeFailureDetails), includeFailureDetails.Value.ToString());
      if (buildToCompare != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (buildToCompare), (object) buildToCompare);
      return this.SendAsync<TestResultSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultSummary> QueryTestResultsReportForBuildAsync(
      Guid project,
      int buildId,
      string publishContext = null,
      bool? includeFailureDetails = null,
      BuildReference buildToCompare = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e009fa95-95a5-4ad4-9681-590043ce2423");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (includeFailureDetails.HasValue)
        keyValuePairList.Add(nameof (includeFailureDetails), includeFailureDetails.Value.ToString());
      if (buildToCompare != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (buildToCompare), (object) buildToCompare);
      return this.SendAsync<TestResultSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultSummary> QueryTestResultsReportForPipelineAsync(
      string project,
      int pipelineId,
      string stageName = null,
      string phaseName = null,
      string jobName = null,
      bool? includeFailureDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("71f746a1-7d68-40fe-b705-9d821a73dff2");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pipelineId), pipelineId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (stageName != null)
        keyValuePairList.Add(nameof (stageName), stageName);
      if (phaseName != null)
        keyValuePairList.Add(nameof (phaseName), phaseName);
      if (jobName != null)
        keyValuePairList.Add(nameof (jobName), jobName);
      if (includeFailureDetails.HasValue)
        keyValuePairList.Add(nameof (includeFailureDetails), includeFailureDetails.Value.ToString());
      return this.SendAsync<TestResultSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultSummary> QueryTestResultsReportForPipelineAsync(
      Guid project,
      int pipelineId,
      string stageName = null,
      string phaseName = null,
      string jobName = null,
      bool? includeFailureDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("71f746a1-7d68-40fe-b705-9d821a73dff2");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pipelineId), pipelineId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (stageName != null)
        keyValuePairList.Add(nameof (stageName), stageName);
      if (phaseName != null)
        keyValuePairList.Add(nameof (phaseName), phaseName);
      if (jobName != null)
        keyValuePairList.Add(nameof (jobName), jobName);
      if (includeFailureDetails.HasValue)
        keyValuePairList.Add(nameof (includeFailureDetails), includeFailureDetails.Value.ToString());
      return this.SendAsync<TestResultSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultSummary> QueryTestResultsReportForReleaseAsync(
      string project,
      int releaseId,
      int releaseEnvId,
      string publishContext = null,
      bool? includeFailureDetails = null,
      ReleaseReference releaseToCompare = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f10f9577-2c04-45ab-8c99-b26567a7cd55");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (includeFailureDetails.HasValue)
        keyValuePairList.Add(nameof (includeFailureDetails), includeFailureDetails.Value.ToString());
      if (releaseToCompare != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (releaseToCompare), (object) releaseToCompare);
      return this.SendAsync<TestResultSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultSummary> QueryTestResultsReportForReleaseAsync(
      Guid project,
      int releaseId,
      int releaseEnvId,
      string publishContext = null,
      bool? includeFailureDetails = null,
      ReleaseReference releaseToCompare = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f10f9577-2c04-45ab-8c99-b26567a7cd55");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (includeFailureDetails.HasValue)
        keyValuePairList.Add(nameof (includeFailureDetails), includeFailureDetails.Value.ToString());
      if (releaseToCompare != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (releaseToCompare), (object) releaseToCompare);
      return this.SendAsync<TestResultSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestResultSummary>> QueryTestResultsSummaryForReleasesAsync(
      IEnumerable<ReleaseReference> releases,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f10f9577-2c04-45ab-8c99-b26567a7cd55");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ReleaseReference>>(releases, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestResultSummary>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestResultSummary>> QueryTestResultsSummaryForReleasesAsync(
      IEnumerable<ReleaseReference> releases,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f10f9577-2c04-45ab-8c99-b26567a7cd55");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ReleaseReference>>(releases, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestResultSummary>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestSummaryForWorkItem>> QueryTestSummaryByRequirementAsync(
      TestResultsContext resultsContext,
      string project,
      IEnumerable<int> workItemIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("3b7fd26f-c335-4e55-afc1-a588f5e2af3c");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultsContext>(resultsContext, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (workItemIds != null && workItemIds.Any<int>())
        collection.Add(nameof (workItemIds), string.Join<int>(",", workItemIds));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestSummaryForWorkItem>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<TestSummaryForWorkItem>> QueryTestSummaryByRequirementAsync(
      TestResultsContext resultsContext,
      Guid project,
      IEnumerable<int> workItemIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("3b7fd26f-c335-4e55-afc1-a588f5e2af3c");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultsContext>(resultsContext, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (workItemIds != null && workItemIds.Any<int>())
        collection.Add(nameof (workItemIds), string.Join<int>(",", workItemIds));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestSummaryForWorkItem>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<AggregatedDataForResultTrend>> QueryResultTrendForBuildAsync(
      TestResultTrendFilter filter,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0886a7ae-315a-4dba-9122-bcce93301f3a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultTrendFilter>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<AggregatedDataForResultTrend>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<AggregatedDataForResultTrend>> QueryResultTrendForBuildAsync(
      TestResultTrendFilter filter,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0886a7ae-315a-4dba-9122-bcce93301f3a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultTrendFilter>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<AggregatedDataForResultTrend>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<AggregatedDataForResultTrend>> QueryResultTrendForReleaseAsync(
      TestResultTrendFilter filter,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("107f23c3-359a-460a-a70c-63ee739f9f9a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultTrendFilter>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<AggregatedDataForResultTrend>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<AggregatedDataForResultTrend>> QueryResultTrendForReleaseAsync(
      TestResultTrendFilter filter,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("107f23c3-359a-460a-a70c-63ee739f9f9a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultTrendFilter>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<AggregatedDataForResultTrend>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestRun> CreateTestRunAsync(
      RunCreateModel testRun,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("364538f9-8062-4ce0-b024-75a0fb463f0d");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<RunCreateModel>(testRun, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestRun>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestRun> CreateTestRunAsync(
      RunCreateModel testRun,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("364538f9-8062-4ce0-b024-75a0fb463f0d");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<RunCreateModel>(testRun, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestRun>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTestRunAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("364538f9-8062-4ce0-b024-75a0fb463f0d"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestRunAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("364538f9-8062-4ce0-b024-75a0fb463f0d"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TestRun> GetTestRunByIdAsync(
      string project,
      int runId,
      bool? includeDetails = null,
      bool? includeTags = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("364538f9-8062-4ce0-b024-75a0fb463f0d");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDetails), str);
      }
      if (includeTags.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeTags.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeTags), str);
      }
      return this.SendAsync<TestRun>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestRun> GetTestRunByIdAsync(
      Guid project,
      int runId,
      bool? includeDetails = null,
      bool? includeTags = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("364538f9-8062-4ce0-b024-75a0fb463f0d");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDetails), str);
      }
      if (includeTags.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeTags.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeTags), str);
      }
      return this.SendAsync<TestRun>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestRun>> GetTestRunsAsync(
      string project,
      string buildUri = null,
      string owner = null,
      string tmiRunId = null,
      int? planId = null,
      bool? includeRunDetails = null,
      bool? automated = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("364538f9-8062-4ce0-b024-75a0fb463f0d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (buildUri != null)
        keyValuePairList.Add(nameof (buildUri), buildUri);
      if (owner != null)
        keyValuePairList.Add(nameof (owner), owner);
      if (tmiRunId != null)
        keyValuePairList.Add(nameof (tmiRunId), tmiRunId);
      int num;
      if (planId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = planId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (planId), str);
      }
      bool flag;
      if (includeRunDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeRunDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeRunDetails), str);
      }
      if (automated.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = automated.Value;
        string str = flag.ToString();
        collection.Add(nameof (automated), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<TestRun>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestRun>> GetTestRunsAsync(
      Guid project,
      string buildUri = null,
      string owner = null,
      string tmiRunId = null,
      int? planId = null,
      bool? includeRunDetails = null,
      bool? automated = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("364538f9-8062-4ce0-b024-75a0fb463f0d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (buildUri != null)
        keyValuePairList.Add(nameof (buildUri), buildUri);
      if (owner != null)
        keyValuePairList.Add(nameof (owner), owner);
      if (tmiRunId != null)
        keyValuePairList.Add(nameof (tmiRunId), tmiRunId);
      int num;
      if (planId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = planId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (planId), str);
      }
      bool flag;
      if (includeRunDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeRunDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeRunDetails), str);
      }
      if (automated.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = automated.Value;
        string str = flag.ToString();
        collection.Add(nameof (automated), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<TestRun>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestRun>> QueryTestRunsAsync(
      string project,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      TestRunState? state = null,
      IEnumerable<int> planIds = null,
      bool? isAutomated = null,
      TestRunPublishContext? publishContext = null,
      IEnumerable<int> buildIds = null,
      IEnumerable<int> buildDefIds = null,
      string branchName = null,
      IEnumerable<int> releaseIds = null,
      IEnumerable<int> releaseDefIds = null,
      IEnumerable<int> releaseEnvIds = null,
      IEnumerable<int> releaseEnvDefIds = null,
      string runTitle = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("364538f9-8062-4ce0-b024-75a0fb463f0d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minLastUpdatedDate), minLastUpdatedDate);
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxLastUpdatedDate), maxLastUpdatedDate);
      if (state.HasValue)
        keyValuePairList.Add(nameof (state), state.Value.ToString());
      if (planIds != null && planIds.Any<int>())
        keyValuePairList.Add(nameof (planIds), string.Join<int>(",", planIds));
      if (isAutomated.HasValue)
        keyValuePairList.Add(nameof (isAutomated), isAutomated.Value.ToString());
      if (publishContext.HasValue)
        keyValuePairList.Add(nameof (publishContext), publishContext.Value.ToString());
      if (buildIds != null && buildIds.Any<int>())
        keyValuePairList.Add(nameof (buildIds), string.Join<int>(",", buildIds));
      if (buildDefIds != null && buildDefIds.Any<int>())
        keyValuePairList.Add(nameof (buildDefIds), string.Join<int>(",", buildDefIds));
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      if (releaseIds != null && releaseIds.Any<int>())
        keyValuePairList.Add(nameof (releaseIds), string.Join<int>(",", releaseIds));
      if (releaseDefIds != null && releaseDefIds.Any<int>())
        keyValuePairList.Add(nameof (releaseDefIds), string.Join<int>(",", releaseDefIds));
      if (releaseEnvIds != null && releaseEnvIds.Any<int>())
        keyValuePairList.Add(nameof (releaseEnvIds), string.Join<int>(",", releaseEnvIds));
      if (releaseEnvDefIds != null && releaseEnvDefIds.Any<int>())
        keyValuePairList.Add(nameof (releaseEnvDefIds), string.Join<int>(",", releaseEnvDefIds));
      if (runTitle != null)
        keyValuePairList.Add(nameof (runTitle), runTitle);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<TestRun>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestRun>> QueryTestRunsAsync(
      Guid project,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      TestRunState? state = null,
      IEnumerable<int> planIds = null,
      bool? isAutomated = null,
      TestRunPublishContext? publishContext = null,
      IEnumerable<int> buildIds = null,
      IEnumerable<int> buildDefIds = null,
      string branchName = null,
      IEnumerable<int> releaseIds = null,
      IEnumerable<int> releaseDefIds = null,
      IEnumerable<int> releaseEnvIds = null,
      IEnumerable<int> releaseEnvDefIds = null,
      string runTitle = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("364538f9-8062-4ce0-b024-75a0fb463f0d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minLastUpdatedDate), minLastUpdatedDate);
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxLastUpdatedDate), maxLastUpdatedDate);
      if (state.HasValue)
        keyValuePairList.Add(nameof (state), state.Value.ToString());
      if (planIds != null && planIds.Any<int>())
        keyValuePairList.Add(nameof (planIds), string.Join<int>(",", planIds));
      if (isAutomated.HasValue)
        keyValuePairList.Add(nameof (isAutomated), isAutomated.Value.ToString());
      if (publishContext.HasValue)
        keyValuePairList.Add(nameof (publishContext), publishContext.Value.ToString());
      if (buildIds != null && buildIds.Any<int>())
        keyValuePairList.Add(nameof (buildIds), string.Join<int>(",", buildIds));
      if (buildDefIds != null && buildDefIds.Any<int>())
        keyValuePairList.Add(nameof (buildDefIds), string.Join<int>(",", buildDefIds));
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      if (releaseIds != null && releaseIds.Any<int>())
        keyValuePairList.Add(nameof (releaseIds), string.Join<int>(",", releaseIds));
      if (releaseDefIds != null && releaseDefIds.Any<int>())
        keyValuePairList.Add(nameof (releaseDefIds), string.Join<int>(",", releaseDefIds));
      if (releaseEnvIds != null && releaseEnvIds.Any<int>())
        keyValuePairList.Add(nameof (releaseEnvIds), string.Join<int>(",", releaseEnvIds));
      if (releaseEnvDefIds != null && releaseEnvDefIds.Any<int>())
        keyValuePairList.Add(nameof (releaseEnvDefIds), string.Join<int>(",", releaseEnvDefIds));
      if (runTitle != null)
        keyValuePairList.Add(nameof (runTitle), runTitle);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<TestRun>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestRun> UpdateTestRunAsync(
      RunUpdateModel runUpdateModel,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("364538f9-8062-4ce0-b024-75a0fb463f0d");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<RunUpdateModel>(runUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestRun>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestRun> UpdateTestRunAsync(
      RunUpdateModel runUpdateModel,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("364538f9-8062-4ce0-b024-75a0fb463f0d");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<RunUpdateModel>(runUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestRun>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestRunStatistic> GetTestRunSummaryByOutcomeAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestRunStatistic>(new HttpMethod("GET"), new Guid("5c6a250c-53b7-4851-990c-42a7a00c8b39"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestRunStatistic> GetTestRunSummaryByOutcomeAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestRunStatistic>(new HttpMethod("GET"), new Guid("5c6a250c-53b7-4851-990c-42a7a00c8b39"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultsSettings> GetTestResultsSettingsAsync(
      string project,
      TestResultsSettingsType? settingsType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7319952e-e5a9-4e19-a006-84f3be8b7c68");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (settingsType.HasValue)
        keyValuePairList.Add(nameof (settingsType), settingsType.Value.ToString());
      return this.SendAsync<TestResultsSettings>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultsSettings> GetTestResultsSettingsAsync(
      Guid project,
      TestResultsSettingsType? settingsType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7319952e-e5a9-4e19-a006-84f3be8b7c68");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (settingsType.HasValue)
        keyValuePairList.Add(nameof (settingsType), settingsType.Value.ToString());
      return this.SendAsync<TestResultsSettings>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultsSettings> UpdatePipelinesTestSettingsAsync(
      TestResultsUpdateSettings testResultsUpdateSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("7319952e-e5a9-4e19-a006-84f3be8b7c68");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultsUpdateSettings>(testResultsUpdateSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultsSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestResultsSettings> UpdatePipelinesTestSettingsAsync(
      TestResultsUpdateSettings testResultsUpdateSettings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("7319952e-e5a9-4e19-a006-84f3be8b7c68");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultsUpdateSettings>(testResultsUpdateSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultsSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestCaseResult>> GetSimilarTestResultsAsync(
      string project,
      int runId,
      int testResultId,
      int testSubResultId,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("67d0a074-b255-4902-a639-e3e6de7a3de6");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testResultId = testResultId
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (testSubResultId), testSubResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (top.HasValue)
        keyValuePairList1.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<List<TestCaseResult>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestCaseResult>> GetSimilarTestResultsAsync(
      Guid project,
      int runId,
      int testResultId,
      int testSubResultId,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("67d0a074-b255-4902-a639-e3e6de7a3de6");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testResultId = testResultId
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (testSubResultId), testSubResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (top.HasValue)
        keyValuePairList1.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<List<TestCaseResult>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestRunStatistic> GetTestRunStatisticsAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestRunStatistic>(new HttpMethod("GET"), new Guid("82b986e8-ca9e-4a89-b39e-f65c69bc104a"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestRunStatistic> GetTestRunStatisticsAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestRunStatistic>(new HttpMethod("GET"), new Guid("82b986e8-ca9e-4a89-b39e-f65c69bc104a"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<string> GetCoverageStatusBadgeAsync(
      string project,
      string definition,
      string branchName = null,
      string label = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("73b7c9d8-defb-4b60-b3d6-2162d60d6b13");
      object routeValues = (object) new
      {
        project = project,
        definition = definition
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      if (label != null)
        keyValuePairList.Add(nameof (label), label);
      return this.SendAsync<string>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<string> GetCoverageStatusBadgeAsync(
      Guid project,
      string definition,
      string branchName = null,
      string label = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("73b7c9d8-defb-4b60-b3d6-2162d60d6b13");
      object routeValues = (object) new
      {
        project = project,
        definition = definition
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      if (label != null)
        keyValuePairList.Add(nameof (label), label);
      return this.SendAsync<string>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestTag>> GetTestTagsForBuildAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("52ee2057-4b54-41a6-a18c-ed4375a00f8d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestTag>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestTag>> GetTestTagsForBuildAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("52ee2057-4b54-41a6-a18c-ed4375a00f8d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestTag>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestTag>> GetTestTagsForReleaseAsync(
      string project,
      int releaseId,
      int releaseEnvId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("52ee2057-4b54-41a6-a18c-ed4375a00f8d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestTag>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestTag>> GetTestTagsForReleaseAsync(
      Guid project,
      int releaseId,
      int releaseEnvId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("52ee2057-4b54-41a6-a18c-ed4375a00f8d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestTag>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestTag>> UpdateTestRunTagsAsync(
      TestTagsUpdateModel testTagsUpdateModel,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a5e2f411-2b43-45f3-989c-05b71339f5b8");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestTagsUpdateModel>(testTagsUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestTag>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestTag>> UpdateTestRunTagsAsync(
      TestTagsUpdateModel testTagsUpdateModel,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a5e2f411-2b43-45f3-989c-05b71339f5b8");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestTagsUpdateModel>(testTagsUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestTag>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestTagSummary> GetTestTagSummaryForBuildAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("655a8f6b-fec7-4b46-b672-68b44141b498");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TestTagSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestTagSummary> GetTestTagSummaryForBuildAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("655a8f6b-fec7-4b46-b672-68b44141b498");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TestTagSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestTagSummary> GetTestTagSummaryForReleaseAsync(
      string project,
      int releaseId,
      int releaseEnvId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("655a8f6b-fec7-4b46-b672-68b44141b498");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TestTagSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestTagSummary> GetTestTagSummaryForReleaseAsync(
      Guid project,
      int releaseId,
      int releaseEnvId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("655a8f6b-fec7-4b46-b672-68b44141b498");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TestTagSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task CreateBuildAttachmentInLogStoreAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6f747e16-18c2-435a-b4fb-fa05d6845fee");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAttachmentRequestModel>(attachmentRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TestResultsHttpClientBase resultsHttpClientBase2 = resultsHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await resultsHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task CreateBuildAttachmentInLogStoreAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6f747e16-18c2-435a-b4fb-fa05d6845fee");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAttachmentRequestModel>(attachmentRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TestResultsHttpClientBase resultsHttpClientBase2 = resultsHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await resultsHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<TestLogStoreAttachmentReference> CreateTestRunLogStoreAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1026d5de-4b0b-46ae-a31f-7c59b6af51ef");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAttachmentRequestModel>(attachmentRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestLogStoreAttachmentReference>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestLogStoreAttachmentReference> CreateTestRunLogStoreAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1026d5de-4b0b-46ae-a31f-7c59b6af51ef");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAttachmentRequestModel>(attachmentRequestModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestLogStoreAttachmentReference>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTestRunLogStoreAttachmentAsync(
      string project,
      int runId,
      string filename,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("1026d5de-4b0b-46ae-a31f-7c59b6af51ef");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (filename), filename);
      using (await resultsHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestRunLogStoreAttachmentAsync(
      Guid project,
      int runId,
      string filename,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("1026d5de-4b0b-46ae-a31f-7c59b6af51ef");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (filename), filename);
      using (await resultsHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task<Stream> GetTestRunLogStoreAttachmentContentAsync(
      string project,
      int runId,
      string filename,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1026d5de-4b0b-46ae-a31f-7c59b6af51ef");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (filename), filename);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestRunLogStoreAttachmentContentAsync(
      Guid project,
      int runId,
      string filename,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1026d5de-4b0b-46ae-a31f-7c59b6af51ef");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (filename), filename);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<TestLogStoreAttachment>> GetTestRunLogStoreAttachmentsAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestLogStoreAttachment>>(new HttpMethod("GET"), new Guid("1026d5de-4b0b-46ae-a31f-7c59b6af51ef"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestLogStoreAttachment>> GetTestRunLogStoreAttachmentsAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestLogStoreAttachment>>(new HttpMethod("GET"), new Guid("1026d5de-4b0b-46ae-a31f-7c59b6af51ef"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetTestRunLogStoreAttachmentZipAsync(
      string project,
      int runId,
      string filename,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1026d5de-4b0b-46ae-a31f-7c59b6af51ef");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (filename), filename);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestRunLogStoreAttachmentZipAsync(
      Guid project,
      int runId,
      string filename,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1026d5de-4b0b-46ae-a31f-7c59b6af51ef");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (filename), filename);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<TestResultFailureType> CreateFailureTypeAsync(
      TestResultFailureTypeRequestModel testResultFailureType,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c4ac0486-830c-4a2a-9ef9-e8a1791a70fd");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultFailureTypeRequestModel>(testResultFailureType, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultFailureType>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestResultFailureType> CreateFailureTypeAsync(
      TestResultFailureTypeRequestModel testResultFailureType,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c4ac0486-830c-4a2a-9ef9-e8a1791a70fd");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultFailureTypeRequestModel>(testResultFailureType, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultFailureType>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteFailureTypeAsync(
      string project,
      int failureTypeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("c4ac0486-830c-4a2a-9ef9-e8a1791a70fd"), (object) new
      {
        project = project,
        failureTypeId = failureTypeId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteFailureTypeAsync(
      Guid project,
      int failureTypeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("c4ac0486-830c-4a2a-9ef9-e8a1791a70fd"), (object) new
      {
        project = project,
        failureTypeId = failureTypeId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<TestResultFailureType>> GetFailureTypesAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestResultFailureType>>(new HttpMethod("GET"), new Guid("c4ac0486-830c-4a2a-9ef9-e8a1791a70fd"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestResultFailureType>> GetFailureTypesAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestResultFailureType>>(new HttpMethod("GET"), new Guid("c4ac0486-830c-4a2a-9ef9-e8a1791a70fd"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestHistoryQuery> QueryTestHistoryAsync(
      TestHistoryQuery filter,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2a41bd6a-8118-4403-b74e-5ba7492aed9d");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestHistoryQuery>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestHistoryQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestHistoryQuery> QueryTestHistoryAsync(
      TestHistoryQuery filter,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2a41bd6a-8118-4403-b74e-5ba7492aed9d");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestHistoryQuery>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestHistoryQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PagedList<TestLog>> GetTestLogsForBuildAsync(
      string project,
      int buildId,
      TestLogType type,
      string directoryPath = null,
      string fileNamePrefix = null,
      bool? fetchMetaData = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dff8ce3a-e539-4817-a405-d968491a88f1");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList1.Add(nameof (type), type.ToString());
      if (directoryPath != null)
        keyValuePairList1.Add(nameof (directoryPath), directoryPath);
      if (fileNamePrefix != null)
        keyValuePairList1.Add(nameof (fileNamePrefix), fileNamePrefix);
      if (fetchMetaData.HasValue)
        keyValuePairList1.Add(nameof (fetchMetaData), fetchMetaData.Value.ToString());
      if (top.HasValue)
        keyValuePairList1.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<PagedList<TestLog>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestLog>> GetTestLogsForBuildAsync(
      Guid project,
      int buildId,
      TestLogType type,
      string directoryPath = null,
      string fileNamePrefix = null,
      bool? fetchMetaData = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dff8ce3a-e539-4817-a405-d968491a88f1");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList1.Add(nameof (type), type.ToString());
      if (directoryPath != null)
        keyValuePairList1.Add(nameof (directoryPath), directoryPath);
      if (fileNamePrefix != null)
        keyValuePairList1.Add(nameof (fileNamePrefix), fileNamePrefix);
      if (fetchMetaData.HasValue)
        keyValuePairList1.Add(nameof (fetchMetaData), fetchMetaData.Value.ToString());
      if (top.HasValue)
        keyValuePairList1.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<PagedList<TestLog>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestLog>> GetTestResultLogsAsync(
      string project,
      int runId,
      int resultId,
      TestLogType type,
      string directoryPath = null,
      string fileNamePrefix = null,
      bool? fetchMetaData = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("714caaac-ae1e-4869-8323-9bc0f5120dbf");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        resultId = resultId
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (type), type.ToString());
      if (directoryPath != null)
        keyValuePairList1.Add(nameof (directoryPath), directoryPath);
      if (fileNamePrefix != null)
        keyValuePairList1.Add(nameof (fileNamePrefix), fileNamePrefix);
      if (fetchMetaData.HasValue)
        keyValuePairList1.Add(nameof (fetchMetaData), fetchMetaData.Value.ToString());
      if (top.HasValue)
        keyValuePairList1.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<PagedList<TestLog>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestLog>> GetTestResultLogsAsync(
      Guid project,
      int runId,
      int resultId,
      TestLogType type,
      string directoryPath = null,
      string fileNamePrefix = null,
      bool? fetchMetaData = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("714caaac-ae1e-4869-8323-9bc0f5120dbf");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        resultId = resultId
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (type), type.ToString());
      if (directoryPath != null)
        keyValuePairList1.Add(nameof (directoryPath), directoryPath);
      if (fileNamePrefix != null)
        keyValuePairList1.Add(nameof (fileNamePrefix), fileNamePrefix);
      if (fetchMetaData.HasValue)
        keyValuePairList1.Add(nameof (fetchMetaData), fetchMetaData.Value.ToString());
      if (top.HasValue)
        keyValuePairList1.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<PagedList<TestLog>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestLog>> GetTestSubResultLogsAsync(
      string project,
      int runId,
      int resultId,
      int subResultId,
      TestLogType type,
      string directoryPath = null,
      string fileNamePrefix = null,
      bool? fetchMetaData = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("714caaac-ae1e-4869-8323-9bc0f5120dbf");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        resultId = resultId
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (subResultId), subResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList1.Add(nameof (type), type.ToString());
      if (directoryPath != null)
        keyValuePairList1.Add(nameof (directoryPath), directoryPath);
      if (fileNamePrefix != null)
        keyValuePairList1.Add(nameof (fileNamePrefix), fileNamePrefix);
      if (fetchMetaData.HasValue)
        keyValuePairList1.Add(nameof (fetchMetaData), fetchMetaData.Value.ToString());
      if (top.HasValue)
        keyValuePairList1.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<PagedList<TestLog>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestLog>> GetTestSubResultLogsAsync(
      Guid project,
      int runId,
      int resultId,
      int subResultId,
      TestLogType type,
      string directoryPath = null,
      string fileNamePrefix = null,
      bool? fetchMetaData = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("714caaac-ae1e-4869-8323-9bc0f5120dbf");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        resultId = resultId
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (subResultId), subResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList1.Add(nameof (type), type.ToString());
      if (directoryPath != null)
        keyValuePairList1.Add(nameof (directoryPath), directoryPath);
      if (fileNamePrefix != null)
        keyValuePairList1.Add(nameof (fileNamePrefix), fileNamePrefix);
      if (fetchMetaData.HasValue)
        keyValuePairList1.Add(nameof (fetchMetaData), fetchMetaData.Value.ToString());
      if (top.HasValue)
        keyValuePairList1.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<PagedList<TestLog>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestLog>> GetTestRunLogsAsync(
      string project,
      int runId,
      TestLogType type,
      string directoryPath = null,
      string fileNamePrefix = null,
      bool? fetchMetaData = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5b47b946-e875-4c9a-acdc-2a20996caebe");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (type), type.ToString());
      if (directoryPath != null)
        keyValuePairList1.Add(nameof (directoryPath), directoryPath);
      if (fileNamePrefix != null)
        keyValuePairList1.Add(nameof (fileNamePrefix), fileNamePrefix);
      if (fetchMetaData.HasValue)
        keyValuePairList1.Add(nameof (fetchMetaData), fetchMetaData.Value.ToString());
      if (top.HasValue)
        keyValuePairList1.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<PagedList<TestLog>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestLog>> GetTestRunLogsAsync(
      Guid project,
      int runId,
      TestLogType type,
      string directoryPath = null,
      string fileNamePrefix = null,
      bool? fetchMetaData = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5b47b946-e875-4c9a-acdc-2a20996caebe");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (type), type.ToString());
      if (directoryPath != null)
        keyValuePairList1.Add(nameof (directoryPath), directoryPath);
      if (fileNamePrefix != null)
        keyValuePairList1.Add(nameof (fileNamePrefix), fileNamePrefix);
      if (fetchMetaData.HasValue)
        keyValuePairList1.Add(nameof (fetchMetaData), fetchMetaData.Value.ToString());
      if (top.HasValue)
        keyValuePairList1.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<PagedList<TestLog>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetailsForBuildLogAsync(
      string project,
      int build,
      TestLogType type,
      string filePath,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("39b09be7-f0c9-4a83-a513-9ae31b45c56f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (build), build.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (type), type.ToString());
      keyValuePairList.Add(nameof (filePath), filePath);
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetailsForBuildLogAsync(
      Guid project,
      int build,
      TestLogType type,
      string filePath,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("39b09be7-f0c9-4a83-a513-9ae31b45c56f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (build), build.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (type), type.ToString());
      keyValuePairList.Add(nameof (filePath), filePath);
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> TestLogStoreEndpointDetailsForBuildAsync(
      string project,
      int buildId,
      TestLogStoreOperationType testLogStoreOperationType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("39b09be7-f0c9-4a83-a513-9ae31b45c56f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (testLogStoreOperationType), testLogStoreOperationType.ToString());
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> TestLogStoreEndpointDetailsForBuildAsync(
      Guid project,
      int buildId,
      TestLogStoreOperationType testLogStoreOperationType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("39b09be7-f0c9-4a83-a513-9ae31b45c56f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (testLogStoreOperationType), testLogStoreOperationType.ToString());
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetailsForResultLogAsync(
      string project,
      int runId,
      int resultId,
      TestLogType type,
      string filePath,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("da630b37-1236-45b5-945e-1d7bdb673850");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        resultId = resultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (type), type.ToString());
      keyValuePairList.Add(nameof (filePath), filePath);
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetailsForResultLogAsync(
      Guid project,
      int runId,
      int resultId,
      TestLogType type,
      string filePath,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("da630b37-1236-45b5-945e-1d7bdb673850");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        resultId = resultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (type), type.ToString());
      keyValuePairList.Add(nameof (filePath), filePath);
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetailsForSubResultLogAsync(
      string project,
      int runId,
      int resultId,
      int subResultId,
      TestLogType type,
      string filePath,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("da630b37-1236-45b5-945e-1d7bdb673850");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        resultId = resultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (subResultId), subResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (type), type.ToString());
      keyValuePairList.Add(nameof (filePath), filePath);
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetailsForSubResultLogAsync(
      Guid project,
      int runId,
      int resultId,
      int subResultId,
      TestLogType type,
      string filePath,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("da630b37-1236-45b5-945e-1d7bdb673850");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        resultId = resultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (subResultId), subResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (type), type.ToString());
      keyValuePairList.Add(nameof (filePath), filePath);
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> TestLogStoreEndpointDetailsForResultAsync(
      string project,
      int runId,
      int resultId,
      int subResultId,
      string filePath,
      TestLogType type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("da630b37-1236-45b5-945e-1d7bdb673850");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        resultId = resultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (subResultId), subResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (filePath), filePath);
      keyValuePairList.Add(nameof (type), type.ToString());
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> TestLogStoreEndpointDetailsForResultAsync(
      Guid project,
      int runId,
      int resultId,
      int subResultId,
      string filePath,
      TestLogType type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("da630b37-1236-45b5-945e-1d7bdb673850");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        resultId = resultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (subResultId), subResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (filePath), filePath);
      keyValuePairList.Add(nameof (type), type.ToString());
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetailsForRunLogAsync(
      string project,
      int runId,
      TestLogType type,
      string filePath,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("67eb3f92-6c97-4fd9-8b63-6cbdc7e526ea");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (type), type.ToString());
      keyValuePairList.Add(nameof (filePath), filePath);
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> GetTestLogStoreEndpointDetailsForRunLogAsync(
      Guid project,
      int runId,
      TestLogType type,
      string filePath,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("67eb3f92-6c97-4fd9-8b63-6cbdc7e526ea");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (type), type.ToString());
      keyValuePairList.Add(nameof (filePath), filePath);
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> TestLogStoreEndpointDetailsForRunAsync(
      string project,
      int runId,
      TestLogStoreOperationType testLogStoreOperationType,
      string filePath = null,
      TestLogType? type = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("67eb3f92-6c97-4fd9-8b63-6cbdc7e526ea");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testLogStoreOperationType), testLogStoreOperationType.ToString());
      if (filePath != null)
        keyValuePairList.Add(nameof (filePath), filePath);
      if (type.HasValue)
        keyValuePairList.Add(nameof (type), type.Value.ToString());
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestLogStoreEndpointDetails> TestLogStoreEndpointDetailsForRunAsync(
      Guid project,
      int runId,
      TestLogStoreOperationType testLogStoreOperationType,
      string filePath = null,
      TestLogType? type = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("67eb3f92-6c97-4fd9-8b63-6cbdc7e526ea");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testLogStoreOperationType), testLogStoreOperationType.ToString());
      if (filePath != null)
        keyValuePairList.Add(nameof (filePath), filePath);
      if (type.HasValue)
        keyValuePairList.Add(nameof (type), type.Value.ToString());
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<long> CreateTestSessionAsync(
      TestResultsSession session,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("531e61ce-580d-4962-8591-0b2942b6bf78");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultsSession>(session, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<long>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<long> CreateTestSessionAsync(
      TestResultsSession session,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("531e61ce-580d-4962-8591-0b2942b6bf78");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultsSession>(session, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<long>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestResultsSession>> GetTestSessionAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("531e61ce-580d-4962-8591-0b2942b6bf78");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestResultsSession>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestResultsSession>> GetTestSessionAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("531e61ce-580d-4962-8591-0b2942b6bf78");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestResultsSession>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<Layout>> GetTestSessionLayoutAsync(
      string project,
      Guid sessionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("531e61ce-580d-4962-8591-0b2942b6bf78");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (sessionId), sessionId.ToString());
      return this.SendAsync<List<Layout>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<Layout>> GetTestSessionLayoutAsync(
      Guid project,
      Guid sessionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("531e61ce-580d-4962-8591-0b2942b6bf78");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (sessionId), sessionId.ToString());
      return this.SendAsync<List<Layout>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task CreateEnvironmentAsync(
      IEnumerable<TestSessionEnvironment> environments,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f9c2e9e4-9c9a-4c1d-9a7d-2b4c8a6f0d5f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestSessionEnvironment>>(environments, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TestResultsHttpClientBase resultsHttpClientBase2 = resultsHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await resultsHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task CreateEnvironmentAsync(
      IEnumerable<TestSessionEnvironment> environments,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f9c2e9e4-9c9a-4c1d-9a7d-2b4c8a6f0d5f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestSessionEnvironment>>(environments, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TestResultsHttpClientBase resultsHttpClientBase2 = resultsHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await resultsHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestCaseResult>> AddTestResultsToTestRunSessionAsync(
      TestCaseResult[] results,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ee6d95bf-7506-4c47-8100-9fed82cdc2f7");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestCaseResult[]>(results, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestCaseResult>> AddTestResultsToTestRunSessionAsync(
      TestCaseResult[] results,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ee6d95bf-7506-4c47-8100-9fed82cdc2f7");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestCaseResult[]>(results, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestCaseResult>> GetTestSessionResultsAsync(
      string project,
      int runId,
      ResultDetails? detailsToInclude = null,
      int? skip = null,
      int? top = null,
      IEnumerable<TestOutcome> outcomes = null,
      bool? newTestsOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ee6d95bf-7506-4c47-8100-9fed82cdc2f7");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        keyValuePairList.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (newTestsOnly.HasValue)
        keyValuePairList.Add("$newTestsOnly", newTestsOnly.Value.ToString());
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestCaseResult>> GetTestSessionResultsAsync(
      Guid project,
      int runId,
      ResultDetails? detailsToInclude = null,
      int? skip = null,
      int? top = null,
      IEnumerable<TestOutcome> outcomes = null,
      bool? newTestsOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ee6d95bf-7506-4c47-8100-9fed82cdc2f7");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        keyValuePairList.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (newTestsOnly.HasValue)
        keyValuePairList.Add("$newTestsOnly", newTestsOnly.Value.ToString());
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<int> CreateTestSettingsAsync(
      TestSettings testSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("930bad47-f826-4099-9597-f44d0a9c735c");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestSettings>(testSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<int>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<int> CreateTestSettingsAsync(
      TestSettings testSettings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("930bad47-f826-4099-9597-f44d0a9c735c");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestSettings>(testSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<int>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTestSettingsAsync(
      string project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("930bad47-f826-4099-9597-f44d0a9c735c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSettingsId), testSettingsId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await resultsHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestSettingsAsync(
      Guid project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClientBase resultsHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("930bad47-f826-4099-9597-f44d0a9c735c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSettingsId), testSettingsId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await resultsHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TestSettings> GetTestSettingsByIdAsync(
      string project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("930bad47-f826-4099-9597-f44d0a9c735c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSettingsId), testSettingsId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TestSettings>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestSettings> GetTestSettingsByIdAsync(
      Guid project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("930bad47-f826-4099-9597-f44d0a9c735c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSettingsId), testSettingsId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TestSettings>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemToTestLinks> AddWorkItemToTestLinksAsync(
      WorkItemToTestLinks workItemToTestLinks,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4e3abe63-ca46-4fe0-98b2-363f7ec7aa5f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemToTestLinks>(workItemToTestLinks, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemToTestLinks>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemToTestLinks> AddWorkItemToTestLinksAsync(
      WorkItemToTestLinks workItemToTestLinks,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4e3abe63-ca46-4fe0-98b2-363f7ec7aa5f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemToTestLinks>(workItemToTestLinks, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemToTestLinks>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<bool> DeleteTestMethodToWorkItemLinkAsync(
      string project,
      string testName,
      int workItemId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("cbd50bd7-f7ed-4e35-b127-4408ae6bfa2c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testName), testName);
      keyValuePairList.Add(nameof (workItemId), workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<bool>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<bool> DeleteTestMethodToWorkItemLinkAsync(
      Guid project,
      string testName,
      int workItemId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("cbd50bd7-f7ed-4e35-b127-4408ae6bfa2c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testName), testName);
      keyValuePairList.Add(nameof (workItemId), workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<bool>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestToWorkItemLinks> QueryTestMethodLinkedWorkItemsAsync(
      string project,
      string testName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("cbd50bd7-f7ed-4e35-b127-4408ae6bfa2c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testName), testName);
      return this.SendAsync<TestToWorkItemLinks>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestToWorkItemLinks> QueryTestMethodLinkedWorkItemsAsync(
      Guid project,
      string testName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("cbd50bd7-f7ed-4e35-b127-4408ae6bfa2c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testName), testName);
      return this.SendAsync<TestToWorkItemLinks>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemReference>> GetTestResultWorkItemsByIdAsync(
      string project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemReference>>(new HttpMethod("GET"), new Guid("3d032fd6-e7a0-468b-b105-75d206f99aad"), (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemReference>> GetTestResultWorkItemsByIdAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemReference>>(new HttpMethod("GET"), new Guid("3d032fd6-e7a0-468b-b105-75d206f99aad"), (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemReference>> QueryTestResultWorkItemsAsync(
      string project,
      string workItemCategory,
      string automatedTestName = null,
      int? testCaseId = null,
      DateTime? maxCompleteDate = null,
      int? days = null,
      int? workItemCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f7401a26-331b-44fe-a470-f7ed35138e4a");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (workItemCategory), workItemCategory);
      if (automatedTestName != null)
        keyValuePairList.Add(nameof (automatedTestName), automatedTestName);
      int num;
      if (testCaseId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = testCaseId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (testCaseId), str);
      }
      if (maxCompleteDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxCompleteDate), maxCompleteDate.Value);
      if (days.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = days.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (days), str);
      }
      if (workItemCount.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = workItemCount.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$workItemCount", str);
      }
      return this.SendAsync<List<WorkItemReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemReference>> QueryTestResultWorkItemsAsync(
      Guid project,
      string workItemCategory,
      string automatedTestName = null,
      int? testCaseId = null,
      DateTime? maxCompleteDate = null,
      int? days = null,
      int? workItemCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f7401a26-331b-44fe-a470-f7ed35138e4a");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (workItemCategory), workItemCategory);
      if (automatedTestName != null)
        keyValuePairList.Add(nameof (automatedTestName), automatedTestName);
      int num;
      if (testCaseId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = testCaseId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (testCaseId), str);
      }
      if (maxCompleteDate.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxCompleteDate), maxCompleteDate.Value);
      if (days.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = days.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (days), str);
      }
      if (workItemCount.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = workItemCount.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$workItemCount", str);
      }
      return this.SendAsync<List<WorkItemReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
