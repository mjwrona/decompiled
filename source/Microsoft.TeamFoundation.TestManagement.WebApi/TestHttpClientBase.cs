// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestHttpClientBase
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi.Types;
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

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [ResourceArea("C2AA639C-3CCC-4740-B3B6-CE2A1E1D984E")]
  public abstract class TestHttpClientBase : TestCompatHttpClientBase
  {
    public TestHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid guid = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid guid = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
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
      Guid guid = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
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
      Guid guid = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
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
      Guid guid = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
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
      Guid guid = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
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

    public virtual async Task<Stream> GetTestResultAttachmentContentAsync(
      string project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestHttpClientBase testHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await testHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await testHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TestHttpClientBase testHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await testHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await testHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      return this.SendAsync<List<TestAttachment>>(new HttpMethod("GET"), new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d"), (object) new
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
      return this.SendAsync<List<TestAttachment>>(new HttpMethod("GET"), new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d"), (object) new
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
      TestHttpClientBase testHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await testHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await testHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TestHttpClientBase testHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await testHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await testHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TestHttpClientBase testHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
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
      using (HttpRequestMessage requestMessage = await testHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await testHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TestHttpClientBase testHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
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
      using (HttpRequestMessage requestMessage = await testHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await testHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      Guid locationId = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
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
      Guid locationId = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
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
      TestHttpClientBase testHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
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
      using (HttpRequestMessage requestMessage = await testHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await testHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TestHttpClientBase testHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2bffebe9-2f0f-4639-9af8-56129e9fed2d");
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
      using (HttpRequestMessage requestMessage = await testHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await testHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      Guid guid = new Guid("4f004af4-a507-489c-9b13-cb62060beb11");
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
      Guid guid = new Guid("4f004af4-a507-489c-9b13-cb62060beb11");
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

    public virtual async Task<Stream> GetTestRunAttachmentContentAsync(
      string project,
      int runId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestHttpClientBase testHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4f004af4-a507-489c-9b13-cb62060beb11");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await testHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await testHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TestHttpClientBase testHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4f004af4-a507-489c-9b13-cb62060beb11");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await testHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await testHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<TestAttachment>> GetTestRunAttachmentsAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestAttachment>>(new HttpMethod("GET"), new Guid("4f004af4-a507-489c-9b13-cb62060beb11"), (object) new
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
      return this.SendAsync<List<TestAttachment>>(new HttpMethod("GET"), new Guid("4f004af4-a507-489c-9b13-cb62060beb11"), (object) new
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
      TestHttpClientBase testHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4f004af4-a507-489c-9b13-cb62060beb11");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await testHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await testHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TestHttpClientBase testHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4f004af4-a507-489c-9b13-cb62060beb11");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await testHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await testHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<WorkItemReference>> GetBugsLinkedToTestResultAsync(
      string project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemReference>>(new HttpMethod("GET"), new Guid("6de20ca2-67de-4faf-97fa-38c5d585eb00"), (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<WorkItemReference>> GetBugsLinkedToTestResultAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemReference>>(new HttpMethod("GET"), new Guid("6de20ca2-67de-4faf-97fa-38c5d585eb00"), (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BuildCoverage>> GetBuildCodeCoverageAsync(
      string project,
      int buildId,
      int flags,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("77560e8a-4e8c-4d59-894e-a5f264c24444");
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
      Guid locationId = new Guid("77560e8a-4e8c-4d59-894e-a5f264c24444");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (flags), flags.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<BuildCoverage>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<CodeCoverageSummary> GetCodeCoverageSummaryAsync(
      string project,
      int buildId,
      int? deltaBuildId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("77560e8a-4e8c-4d59-894e-a5f264c24444");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (deltaBuildId.HasValue)
        keyValuePairList.Add(nameof (deltaBuildId), deltaBuildId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<CodeCoverageSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<CodeCoverageSummary> GetCodeCoverageSummaryAsync(
      Guid project,
      int buildId,
      int? deltaBuildId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("77560e8a-4e8c-4d59-894e-a5f264c24444");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (deltaBuildId.HasValue)
        keyValuePairList.Add(nameof (deltaBuildId), deltaBuildId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<CodeCoverageSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdateCodeCoverageSummaryAsync(
      CodeCoverageData coverageData,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestHttpClientBase testHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("77560e8a-4e8c-4d59-894e-a5f264c24444");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CodeCoverageData>(coverageData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      TestHttpClientBase testHttpClientBase2 = testHttpClientBase1;
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
      using (await testHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdateCodeCoverageSummaryAsync(
      CodeCoverageData coverageData,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestHttpClientBase testHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("77560e8a-4e8c-4d59-894e-a5f264c24444");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CodeCoverageData>(coverageData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      TestHttpClientBase testHttpClientBase2 = testHttpClientBase1;
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
      using (await testHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2).ConfigureAwait(false))
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
      Guid locationId = new Guid("9629116f-3b89-4ed8-b358-d4694efda160");
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
      Guid locationId = new Guid("9629116f-3b89-4ed8-b358-d4694efda160");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (flags), flags.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestRunCoverage>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<CustomTestFieldDefinition>> AddCustomFieldsAsync(
      CustomTestFieldDefinition[] newFields,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8ce1923b-f4c7-4e22-b93b-f6284e525ec2");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CustomTestFieldDefinition[]>(newFields, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<CustomTestFieldDefinition>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<CustomTestFieldDefinition>> AddCustomFieldsAsync(
      CustomTestFieldDefinition[] newFields,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8ce1923b-f4c7-4e22-b93b-f6284e525ec2");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CustomTestFieldDefinition[]>(newFields, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<CustomTestFieldDefinition>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<CustomTestFieldDefinition>> QueryCustomFieldsAsync(
      string project,
      CustomTestFieldScope scopeFilter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8ce1923b-f4c7-4e22-b93b-f6284e525ec2");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (scopeFilter), scopeFilter.ToString());
      return this.SendAsync<List<CustomTestFieldDefinition>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<CustomTestFieldDefinition>> QueryCustomFieldsAsync(
      Guid project,
      CustomTestFieldScope scopeFilter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8ce1923b-f4c7-4e22-b93b-f6284e525ec2");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (scopeFilter), scopeFilter.ToString());
      return this.SendAsync<List<CustomTestFieldDefinition>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TestResultHistory> QueryTestResultHistoryAsync(
      ResultsFilter filter,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("234616f5-429c-4e7b-9192-affd76731dfd");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TestResultHistory> QueryTestResultHistoryAsync(
      ResultsFilter filter,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("234616f5-429c-4e7b-9192-affd76731dfd");
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

    public virtual Task<TestIterationDetailsModel> GetTestIterationAsync(
      string project,
      int runId,
      int testCaseResultId,
      int iterationId,
      bool? includeActionResults = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("73eb9074-3446-4c44-8296-2f811950ff8d");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        iterationId = iterationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeActionResults.HasValue)
        keyValuePairList.Add(nameof (includeActionResults), includeActionResults.Value.ToString());
      return this.SendAsync<TestIterationDetailsModel>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestIterationDetailsModel> GetTestIterationAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int iterationId,
      bool? includeActionResults = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("73eb9074-3446-4c44-8296-2f811950ff8d");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        iterationId = iterationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeActionResults.HasValue)
        keyValuePairList.Add(nameof (includeActionResults), includeActionResults.Value.ToString());
      return this.SendAsync<TestIterationDetailsModel>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestIterationDetailsModel>> GetTestIterationsAsync(
      string project,
      int runId,
      int testCaseResultId,
      bool? includeActionResults = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("73eb9074-3446-4c44-8296-2f811950ff8d");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeActionResults.HasValue)
        keyValuePairList.Add(nameof (includeActionResults), includeActionResults.Value.ToString());
      return this.SendAsync<List<TestIterationDetailsModel>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestIterationDetailsModel>> GetTestIterationsAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      bool? includeActionResults = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("73eb9074-3446-4c44-8296-2f811950ff8d");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeActionResults.HasValue)
        keyValuePairList.Add(nameof (includeActionResults), includeActionResults.Value.ToString());
      return this.SendAsync<List<TestIterationDetailsModel>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<LinkedWorkItemsQueryResult>> GetLinkedWorkItemsByQueryAsync(
      LinkedWorkItemsQuery workItemQuery,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a4dcb25b-9878-49ea-abfd-e440bd9b1dcd");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<LinkedWorkItemsQuery>(workItemQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<LinkedWorkItemsQueryResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<LinkedWorkItemsQueryResult>> GetLinkedWorkItemsByQueryAsync(
      LinkedWorkItemsQuery workItemQuery,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a4dcb25b-9878-49ea-abfd-e440bd9b1dcd");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<LinkedWorkItemsQuery>(workItemQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<LinkedWorkItemsQueryResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestMessageLogDetails>> GetTestRunLogsAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestMessageLogDetails>>(new HttpMethod("GET"), new Guid("a1e55200-637e-42e9-a7c0-7e5bfdedb1b3"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestMessageLogDetails>> GetTestRunLogsAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestMessageLogDetails>>(new HttpMethod("GET"), new Guid("a1e55200-637e-42e9-a7c0-7e5bfdedb1b3"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestPoint> GetPointAsync(
      string project,
      int planId,
      int suiteId,
      int pointIds,
      string witFields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3bcfd5c8-be62-488e-b1da-b8289ce9299c");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId,
        pointIds = pointIds
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (witFields != null)
        keyValuePairList.Add(nameof (witFields), witFields);
      return this.SendAsync<TestPoint>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestPoint> GetPointAsync(
      Guid project,
      int planId,
      int suiteId,
      int pointIds,
      string witFields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3bcfd5c8-be62-488e-b1da-b8289ce9299c");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId,
        pointIds = pointIds
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (witFields != null)
        keyValuePairList.Add(nameof (witFields), witFields);
      return this.SendAsync<TestPoint>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestPoint>> GetPointsAsync(
      string project,
      int planId,
      int suiteId,
      string witFields = null,
      string configurationId = null,
      string testCaseId = null,
      string testPointIds = null,
      bool? includePointDetails = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3bcfd5c8-be62-488e-b1da-b8289ce9299c");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (witFields != null)
        keyValuePairList.Add(nameof (witFields), witFields);
      if (configurationId != null)
        keyValuePairList.Add(nameof (configurationId), configurationId);
      if (testCaseId != null)
        keyValuePairList.Add(nameof (testCaseId), testCaseId);
      if (testPointIds != null)
        keyValuePairList.Add(nameof (testPointIds), testPointIds);
      if (includePointDetails.HasValue)
        keyValuePairList.Add(nameof (includePointDetails), includePointDetails.Value.ToString());
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
      return this.SendAsync<List<TestPoint>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestPoint>> GetPointsAsync(
      Guid project,
      int planId,
      int suiteId,
      string witFields = null,
      string configurationId = null,
      string testCaseId = null,
      string testPointIds = null,
      bool? includePointDetails = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3bcfd5c8-be62-488e-b1da-b8289ce9299c");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (witFields != null)
        keyValuePairList.Add(nameof (witFields), witFields);
      if (configurationId != null)
        keyValuePairList.Add(nameof (configurationId), configurationId);
      if (testCaseId != null)
        keyValuePairList.Add(nameof (testCaseId), testCaseId);
      if (testPointIds != null)
        keyValuePairList.Add(nameof (testPointIds), testPointIds);
      if (includePointDetails.HasValue)
        keyValuePairList.Add(nameof (includePointDetails), includePointDetails.Value.ToString());
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
      return this.SendAsync<List<TestPoint>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestPoint>> UpdateTestPointsAsync(
      PointUpdateModel pointUpdateModel,
      string project,
      int planId,
      int suiteId,
      string pointIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("3bcfd5c8-be62-488e-b1da-b8289ce9299c");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId,
        pointIds = pointIds
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PointUpdateModel>(pointUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestPoint>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestPoint>> UpdateTestPointsAsync(
      PointUpdateModel pointUpdateModel,
      Guid project,
      int planId,
      int suiteId,
      string pointIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("3bcfd5c8-be62-488e-b1da-b8289ce9299c");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId,
        pointIds = pointIds
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PointUpdateModel>(pointUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestPoint>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestPointsQuery> GetPointsByQueryAsync(
      TestPointsQuery query,
      string project,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b4264fd0-a5d1-43e2-82a5-b9c46b7da9ce");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestPointsQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
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
      return this.SendAsync<TestPointsQuery>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<TestPointsQuery> GetPointsByQueryAsync(
      TestPointsQuery query,
      Guid project,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b4264fd0-a5d1-43e2-82a5-b9c46b7da9ce");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestPointsQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
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
      return this.SendAsync<TestPointsQuery>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("efb387b0-10d5-42e7-be40-95e06ee9430f");
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
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("efb387b0-10d5-42e7-be40-95e06ee9430f");
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
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("b834ec7e-35bb-450f-a3c8-802e70ca40dd");
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
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("b834ec7e-35bb-450f-a3c8-802e70ca40dd");
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
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TestResultDocument> PublishTestResultDocumentAsync(
      TestResultDocument document,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("370ca04b-8eec-4ca8-8ba3-d24dca228791");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TestResultDocument> PublishTestResultDocumentAsync(
      TestResultDocument document,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("370ca04b-8eec-4ca8-8ba3-d24dca228791");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("d279d052-c55a-4204-b913-42f733b52958");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("d279d052-c55a-4204-b913-42f733b52958");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("ef5ce5d4-a4e5-47ee-804c-354518f8d03f");
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
      return this.SendAsync<PagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("ef5ce5d4-a4e5-47ee-804c-354518f8d03f");
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
      return this.SendAsync<PagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestResultMetaData>> QueryTestResultsMetaDataAsync(
      IEnumerable<string> testReferenceIds,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("afa7830e-67a7-4336-8090-2b448ca80295");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(testReferenceIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestResultMetaData>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestResultMetaData>> QueryTestResultsMetaDataAsync(
      IEnumerable<string> testReferenceIds,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("afa7830e-67a7-4336-8090-2b448ca80295");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(testReferenceIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestResultMetaData>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ResultRetentionSettings> GetResultRetentionSettingsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ResultRetentionSettings>(new HttpMethod("GET"), new Guid("a3206d9e-fa8d-42d3-88cb-f75c51e69cde"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ResultRetentionSettings> GetResultRetentionSettingsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ResultRetentionSettings>(new HttpMethod("GET"), new Guid("a3206d9e-fa8d-42d3-88cb-f75c51e69cde"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ResultRetentionSettings> UpdateResultRetentionSettingsAsync(
      ResultRetentionSettings retentionSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a3206d9e-fa8d-42d3-88cb-f75c51e69cde");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultRetentionSettings>(retentionSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ResultRetentionSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ResultRetentionSettings> UpdateResultRetentionSettingsAsync(
      ResultRetentionSettings retentionSettings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a3206d9e-fa8d-42d3-88cb-f75c51e69cde");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultRetentionSettings>(retentionSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ResultRetentionSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestCaseResult>> AddTestResultsToTestRunAsync(
      TestCaseResult[] results,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4637d869-3a76-4468-8057-0bb02aa385cf");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestCaseResult[]>(results, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 6);
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
      Guid guid = new Guid("4637d869-3a76-4468-8057-0bb02aa385cf");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestCaseResult[]>(results, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 6);
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
      int testCaseResultId,
      ResultDetails? detailsToInclude = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4637d869-3a76-4468-8057-0bb02aa385cf");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        keyValuePairList.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      return this.SendAsync<TestCaseResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 6), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestCaseResult> GetTestResultByIdAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      ResultDetails? detailsToInclude = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4637d869-3a76-4468-8057-0bb02aa385cf");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        keyValuePairList.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      return this.SendAsync<TestCaseResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 6), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestCaseResult>> GetTestResultsAsync(
      string project,
      int runId,
      ResultDetails? detailsToInclude = null,
      int? skip = null,
      int? top = null,
      IEnumerable<TestOutcome> outcomes = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4637d869-3a76-4468-8057-0bb02aa385cf");
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
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 6), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestCaseResult>> GetTestResultsAsync(
      Guid project,
      int runId,
      ResultDetails? detailsToInclude = null,
      int? skip = null,
      int? top = null,
      IEnumerable<TestOutcome> outcomes = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4637d869-3a76-4468-8057-0bb02aa385cf");
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
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 6), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestCaseResult>> UpdateTestResultsAsync(
      TestCaseResult[] results,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4637d869-3a76-4468-8057-0bb02aa385cf");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestCaseResult[]>(results, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 6);
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
      Guid guid = new Guid("4637d869-3a76-4468-8057-0bb02aa385cf");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestCaseResult[]>(results, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 6);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TestResultsQuery> GetTestResultsByQueryAsync(
      TestResultsQuery query,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6711da49-8e6f-4d35-9f73-cef7a3c81a5b");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultsQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 6);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultsQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TestResultsQuery> GetTestResultsByQueryAsync(
      TestResultsQuery query,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6711da49-8e6f-4d35-9f73-cef7a3c81a5b");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultsQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 6);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultsQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("3c191b88-615b-4be2-b7d9-5ff9141e91d4");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("3c191b88-615b-4be2-b7d9-5ff9141e91d4");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("ce01820b-83f3-4c15-a583-697a43292c4e");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("ce01820b-83f3-4c15-a583-697a43292c4e");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("000ef77b-fea2-498d-a10d-ad1a037f559f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (includeFailureDetails.HasValue)
        keyValuePairList.Add(nameof (includeFailureDetails), includeFailureDetails.Value.ToString());
      if (buildToCompare != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (buildToCompare), (object) buildToCompare);
      return this.SendAsync<TestResultSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("000ef77b-fea2-498d-a10d-ad1a037f559f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (includeFailureDetails.HasValue)
        keyValuePairList.Add(nameof (includeFailureDetails), includeFailureDetails.Value.ToString());
      if (buildToCompare != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (buildToCompare), (object) buildToCompare);
      return this.SendAsync<TestResultSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("85765790-ac68-494e-b268-af36c3929744");
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
      return this.SendAsync<TestResultSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("85765790-ac68-494e-b268-af36c3929744");
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
      return this.SendAsync<TestResultSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestResultSummary>> QueryTestResultsSummaryForReleasesAsync(
      IEnumerable<ReleaseReference> releases,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("85765790-ac68-494e-b268-af36c3929744");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ReleaseReference>>(releases, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestResultSummary>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestResultSummary>> QueryTestResultsSummaryForReleasesAsync(
      IEnumerable<ReleaseReference> releases,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("85765790-ac68-494e-b268-af36c3929744");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ReleaseReference>>(releases, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestResultSummary>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestSummaryForWorkItem>> QueryTestSummaryByRequirementAsync(
      TestResultsContext resultsContext,
      string project,
      IEnumerable<int> workItemIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("cd08294e-308d-4460-a46e-4cfdefba0b4b");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestSummaryForWorkItem>> QueryTestSummaryByRequirementAsync(
      TestResultsContext resultsContext,
      Guid project,
      IEnumerable<int> workItemIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("cd08294e-308d-4460-a46e-4cfdefba0b4b");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AggregatedDataForResultTrend>> QueryResultTrendForBuildAsync(
      TestResultTrendFilter filter,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("fbc82a85-0786-4442-88bb-eb0fda6b01b0");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AggregatedDataForResultTrend>> QueryResultTrendForBuildAsync(
      TestResultTrendFilter filter,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("fbc82a85-0786-4442-88bb-eb0fda6b01b0");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AggregatedDataForResultTrend>> QueryResultTrendForReleaseAsync(
      TestResultTrendFilter filter,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("dd178e93-d8dd-4887-9635-d6b9560b7b6e");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AggregatedDataForResultTrend>> QueryResultTrendForReleaseAsync(
      TestResultTrendFilter filter,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("dd178e93-d8dd-4887-9635-d6b9560b7b6e");
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

    public virtual Task<TestRunStatistic> GetTestRunStatisticsAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestRunStatistic>(new HttpMethod("GET"), new Guid("0a42c424-d764-4a16-a2d5-5c85f87d0ae8"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestRunStatistic> GetTestRunStatisticsAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestRunStatistic>(new HttpMethod("GET"), new Guid("0a42c424-d764-4a16-a2d5-5c85f87d0ae8"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestRun> CreateTestRunAsync(
      RunCreateModel testRun,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<RunCreateModel>(testRun, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
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
      Guid guid = new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<RunCreateModel>(testRun, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
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
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestRunAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TestRun> GetTestRunByIdAsync(
      string project,
      int runId,
      bool? includeDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDetails.HasValue)
        keyValuePairList.Add(nameof (includeDetails), includeDetails.Value.ToString());
      return this.SendAsync<TestRun>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestRun> GetTestRunByIdAsync(
      Guid project,
      int runId,
      bool? includeDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDetails.HasValue)
        keyValuePairList.Add(nameof (includeDetails), includeDetails.Value.ToString());
      return this.SendAsync<TestRun>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
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
      Guid locationId = new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138");
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
      return this.SendAsync<List<TestRun>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
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
      Guid locationId = new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138");
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
      return this.SendAsync<List<TestRun>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
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
      Guid locationId = new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138");
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
      return this.SendAsync<PagedList<TestRun>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
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
      Guid locationId = new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138");
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
      return this.SendAsync<PagedList<TestRun>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestRun> UpdateTestRunAsync(
      RunUpdateModel runUpdateModel,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<RunUpdateModel>(runUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
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
      Guid guid = new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138");
      object obj1 = (object) new
      {
        project = project,
        runId = runId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<RunUpdateModel>(runUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestRun>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestSession> CreateTestSessionAsync(
      TestSession testSession,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1500b4b4-6c69-4ca6-9b18-35e9e97fe2ac");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestSession>(testSession, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestSession>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestSession>> GetTestSessionsAsync(
      TeamContext teamContext,
      int? period = null,
      bool? allSessions = null,
      bool? includeAllProperties = null,
      TestSessionSource? source = null,
      bool? includeOnlyCompletedSessions = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1500b4b4-6c69-4ca6-9b18-35e9e97fe2ac");
      string str3 = str2;
      object routeValues = (object) new
      {
        project = str1,
        team = str3
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (period.HasValue)
        keyValuePairList.Add(nameof (period), period.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (allSessions.HasValue)
        keyValuePairList.Add(nameof (allSessions), allSessions.Value.ToString());
      if (includeAllProperties.HasValue)
        keyValuePairList.Add(nameof (includeAllProperties), includeAllProperties.Value.ToString());
      if (source.HasValue)
        keyValuePairList.Add(nameof (source), source.Value.ToString());
      if (includeOnlyCompletedSessions.HasValue)
        keyValuePairList.Add(nameof (includeOnlyCompletedSessions), includeOnlyCompletedSessions.Value.ToString());
      return this.SendAsync<List<TestSession>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestSession> UpdateTestSessionAsync(
      TestSession testSession,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1500b4b4-6c69-4ca6-9b18-35e9e97fe2ac");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestSession>(testSession, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestSession>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteSharedParameterAsync(
      string project,
      int sharedParameterId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("8300eeca-0f8c-4eff-a089-d2dda409c41f"), (object) new
      {
        project = project,
        sharedParameterId = sharedParameterId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteSharedParameterAsync(
      Guid project,
      int sharedParameterId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("8300eeca-0f8c-4eff-a089-d2dda409c41f"), (object) new
      {
        project = project,
        sharedParameterId = sharedParameterId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteSharedStepAsync(
      string project,
      int sharedStepId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("fabb3cc9-e3f8-40b7-8b62-24cc4b73fccf"), (object) new
      {
        project = project,
        sharedStepId = sharedStepId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteSharedStepAsync(
      Guid project,
      int sharedStepId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("fabb3cc9-e3f8-40b7-8b62-24cc4b73fccf"), (object) new
      {
        project = project,
        sharedStepId = sharedStepId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<SuiteTestCase>> AddTestCasesToSuiteAsync(
      string project,
      int planId,
      int suiteId,
      string testCaseIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<SuiteTestCase>>(new HttpMethod("POST"), new Guid("a4a1ec1c-b03f-41ca-8857-704594ecf58e"), (object) new
      {
        action = "TestCases",
        project = project,
        planId = planId,
        suiteId = suiteId,
        testCaseIds = testCaseIds
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SuiteTestCase>> AddTestCasesToSuiteAsync(
      Guid project,
      int planId,
      int suiteId,
      string testCaseIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<SuiteTestCase>>(new HttpMethod("POST"), new Guid("a4a1ec1c-b03f-41ca-8857-704594ecf58e"), (object) new
      {
        action = "TestCases",
        project = project,
        planId = planId,
        suiteId = suiteId,
        testCaseIds = testCaseIds
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<SuiteTestCase> GetTestCaseByIdAsync(
      string project,
      int planId,
      int suiteId,
      int testCaseIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<SuiteTestCase>(new HttpMethod("GET"), new Guid("a4a1ec1c-b03f-41ca-8857-704594ecf58e"), (object) new
      {
        action = "TestCases",
        project = project,
        planId = planId,
        suiteId = suiteId,
        testCaseIds = testCaseIds
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<SuiteTestCase> GetTestCaseByIdAsync(
      Guid project,
      int planId,
      int suiteId,
      int testCaseIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<SuiteTestCase>(new HttpMethod("GET"), new Guid("a4a1ec1c-b03f-41ca-8857-704594ecf58e"), (object) new
      {
        action = "TestCases",
        project = project,
        planId = planId,
        suiteId = suiteId,
        testCaseIds = testCaseIds
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SuiteTestCase>> GetTestCasesAsync(
      string project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<SuiteTestCase>>(new HttpMethod("GET"), new Guid("a4a1ec1c-b03f-41ca-8857-704594ecf58e"), (object) new
      {
        action = "TestCases",
        project = project,
        planId = planId,
        suiteId = suiteId
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SuiteTestCase>> GetTestCasesAsync(
      Guid project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<SuiteTestCase>>(new HttpMethod("GET"), new Guid("a4a1ec1c-b03f-41ca-8857-704594ecf58e"), (object) new
      {
        action = "TestCases",
        project = project,
        planId = planId,
        suiteId = suiteId
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task RemoveTestCasesFromSuiteUrlAsync(
      string project,
      int planId,
      int suiteId,
      string testCaseIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("a4a1ec1c-b03f-41ca-8857-704594ecf58e"), (object) new
      {
        action = "TestCases",
        project = project,
        planId = planId,
        suiteId = suiteId,
        testCaseIds = testCaseIds
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task RemoveTestCasesFromSuiteUrlAsync(
      Guid project,
      int planId,
      int suiteId,
      string testCaseIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("a4a1ec1c-b03f-41ca-8857-704594ecf58e"), (object) new
      {
        action = "TestCases",
        project = project,
        planId = planId,
        suiteId = suiteId,
        testCaseIds = testCaseIds
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<SuiteTestCase>> UpdateSuiteTestCasesAsync(
      SuiteTestCaseUpdateModel suiteTestCaseUpdateModel,
      string project,
      int planId,
      int suiteId,
      string testCaseIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a4a1ec1c-b03f-41ca-8857-704594ecf58e");
      object obj1 = (object) new
      {
        action = "TestCases",
        project = project,
        planId = planId,
        suiteId = suiteId,
        testCaseIds = testCaseIds
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<SuiteTestCaseUpdateModel>(suiteTestCaseUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<SuiteTestCase>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<SuiteTestCase>> UpdateSuiteTestCasesAsync(
      SuiteTestCaseUpdateModel suiteTestCaseUpdateModel,
      Guid project,
      int planId,
      int suiteId,
      string testCaseIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a4a1ec1c-b03f-41ca-8857-704594ecf58e");
      object obj1 = (object) new
      {
        action = "TestCases",
        project = project,
        planId = planId,
        suiteId = suiteId,
        testCaseIds = testCaseIds
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<SuiteTestCaseUpdateModel>(suiteTestCaseUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<SuiteTestCase>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTestCaseAsync(
      string project,
      int testCaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("4d472e0f-e32c-4ef8-adf4-a4078772889c"), (object) new
      {
        project = project,
        testCaseId = testCaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestCaseAsync(
      Guid project,
      int testCaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("4d472e0f-e32c-4ef8-adf4-a4078772889c"), (object) new
      {
        project = project,
        testCaseId = testCaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TestHistoryQuery> QueryTestHistoryAsync(
      TestHistoryQuery filter,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("929fd86c-3e38-4d8c-b4b6-90df256e5971");
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
      Guid guid = new Guid("929fd86c-3e38-4d8c-b4b6-90df256e5971");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<int> CreateTestSettingsAsync(
      TestSettings testSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8133ce14-962f-42af-a5f9-6aa9defcb9c8");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<int> CreateTestSettingsAsync(
      TestSettings testSettings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8133ce14-962f-42af-a5f9-6aa9defcb9c8");
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteTestSettingsAsync(
      string project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("8133ce14-962f-42af-a5f9-6aa9defcb9c8"), (object) new
      {
        project = project,
        testSettingsId = testSettingsId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteTestSettingsAsync(
      Guid project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("8133ce14-962f-42af-a5f9-6aa9defcb9c8"), (object) new
      {
        project = project,
        testSettingsId = testSettingsId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TestSettings> GetTestSettingsByIdAsync(
      string project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestSettings>(new HttpMethod("GET"), new Guid("8133ce14-962f-42af-a5f9-6aa9defcb9c8"), (object) new
      {
        project = project,
        testSettingsId = testSettingsId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TestSettings> GetTestSettingsByIdAsync(
      Guid project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestSettings>(new HttpMethod("GET"), new Guid("8133ce14-962f-42af-a5f9-6aa9defcb9c8"), (object) new
      {
        project = project,
        testSettingsId = testSettingsId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemToTestLinks> AddWorkItemToTestLinksAsync(
      WorkItemToTestLinks workItemToTestLinks,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("371b1655-ce05-412e-a113-64cc77bb78d2");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemToTestLinks>(workItemToTestLinks, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemToTestLinks>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemToTestLinks> AddWorkItemToTestLinksAsync(
      WorkItemToTestLinks workItemToTestLinks,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("371b1655-ce05-412e-a113-64cc77bb78d2");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemToTestLinks>(workItemToTestLinks, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemToTestLinks>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<bool> DeleteTestMethodToWorkItemLinkAsync(
      string project,
      string testName,
      int workItemId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("7b0bdee3-a354-47f9-a42c-89018d7808d5");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testName), testName);
      keyValuePairList.Add(nameof (workItemId), workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<bool>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<bool> DeleteTestMethodToWorkItemLinkAsync(
      Guid project,
      string testName,
      int workItemId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("7b0bdee3-a354-47f9-a42c-89018d7808d5");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testName), testName);
      keyValuePairList.Add(nameof (workItemId), workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<bool>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TestToWorkItemLinks> QueryTestMethodLinkedWorkItemsAsync(
      string project,
      string testName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("7b0bdee3-a354-47f9-a42c-89018d7808d5");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testName), testName);
      return this.SendAsync<TestToWorkItemLinks>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TestToWorkItemLinks> QueryTestMethodLinkedWorkItemsAsync(
      Guid project,
      string testName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("7b0bdee3-a354-47f9-a42c-89018d7808d5");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testName), testName);
      return this.SendAsync<TestToWorkItemLinks>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("926ff5dc-137f-45f0-bd51-9412fa9810ce");
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
      return this.SendAsync<List<WorkItemReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
      Guid locationId = new Guid("926ff5dc-137f-45f0-bd51-9412fa9810ce");
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
      return this.SendAsync<List<WorkItemReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
