// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.WebApi.TcmHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.Tcm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DCD48481-6B90-4012-9254-BC9E7077DAC8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.WebApi.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Tcm.WebApi
{
  [ResourceArea("C08C062A-B973-4754-B339-8DE3B6FE53EC")]
  public abstract class TcmHttpClientBase : VssHttpClientBase
  {
    public TcmHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TcmHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TcmHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TcmHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TcmHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<AfnStrip> CreateAfnStripAsync(
      AfnStrip afnStrip,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e95b7842-152c-4a62-ad35-6c428cddde07");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<AfnStrip>(afnStrip, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AfnStrip>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<AfnStrip> CreateAfnStripAsync(
      AfnStrip afnStrip,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e95b7842-152c-4a62-ad35-6c428cddde07");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<AfnStrip>(afnStrip, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AfnStrip>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<AfnStrip>> GetAfnStripsAsync(
      string project,
      IEnumerable<int> testCaseIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e95b7842-152c-4a62-ad35-6c428cddde07");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (testCaseIds != null && testCaseIds.Any<int>())
        keyValuePairList.Add(nameof (testCaseIds), string.Join<int>(",", testCaseIds));
      return this.SendAsync<List<AfnStrip>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<AfnStrip>> GetAfnStripsAsync(
      Guid project,
      IEnumerable<int> testCaseIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e95b7842-152c-4a62-ad35-6c428cddde07");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (testCaseIds != null && testCaseIds.Any<int>())
        keyValuePairList.Add(nameof (testCaseIds), string.Join<int>(",", testCaseIds));
      return this.SendAsync<List<AfnStrip>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateDefaultAfnStripsAsync(
      IEnumerable<DefaultAfnStripBinding> bindings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClientBase tcmHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e95b7842-152c-4a62-ad35-6c428cddde07");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<DefaultAfnStripBinding>>(bindings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TcmHttpClientBase tcmHttpClientBase2 = tcmHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await tcmHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdateDefaultAfnStripsAsync(
      IEnumerable<DefaultAfnStripBinding> bindings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClientBase tcmHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e95b7842-152c-4a62-ad35-6c428cddde07");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<DefaultAfnStripBinding>>(bindings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TcmHttpClientBase tcmHttpClientBase2 = tcmHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await tcmHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<TestResultAttachment>> GetAttachmentsAsync(
      string project,
      int attachmentId,
      int testRunId,
      int testResultId,
      int subResultId,
      int sessionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9a941c36-fa09-446c-b15c-6c1dc03efce9");
      object routeValues = (object) new
      {
        project = project,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testRunId), testRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (testResultId), testResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (subResultId), subResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (sessionId), sessionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestResultAttachment>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestResultAttachment>> GetAttachmentsAsync(
      Guid project,
      int attachmentId,
      int testRunId,
      int testResultId,
      int subResultId,
      int sessionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9a941c36-fa09-446c-b15c-6c1dc03efce9");
      object routeValues = (object) new
      {
        project = project,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testRunId), testRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (testResultId), testResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (subResultId), subResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (sessionId), sessionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestResultAttachment>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestResultAttachment>> GetAttachments2Async(
      string project,
      int attachmentId,
      bool getSiblingAttachments,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9a941c36-fa09-446c-b15c-6c1dc03efce9");
      object routeValues = (object) new
      {
        project = project,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (getSiblingAttachments), getSiblingAttachments.ToString());
      return this.SendAsync<List<TestResultAttachment>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestResultAttachment>> GetAttachments2Async(
      Guid project,
      int attachmentId,
      bool getSiblingAttachments,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9a941c36-fa09-446c-b15c-6c1dc03efce9");
      object routeValues = (object) new
      {
        project = project,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (getSiblingAttachments), getSiblingAttachments.ToString());
      return this.SendAsync<List<TestResultAttachment>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestResultAttachment>> GetAttachmentsByQueryAsync(
      ResultsStoreQuery query,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9a941c36-fa09-446c-b15c-6c1dc03efce9");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultsStoreQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestResultAttachment>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestResultAttachment>> GetAttachmentsByQueryAsync(
      ResultsStoreQuery query,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9a941c36-fa09-446c-b15c-6c1dc03efce9");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultsStoreQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestResultAttachment>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task<Stream> GetTestResultAttachmentStreamAsync(
      string project,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9a941c36-fa09-446c-b15c-6c1dc03efce9");
      object routeValues = (object) new
      {
        project = project,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTestResultAttachmentStreamAsync(
      Guid project,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9a941c36-fa09-446c-b15c-6c1dc03efce9");
      object routeValues = (object) new
      {
        project = project,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
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
      Guid guid = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
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
      Guid guid = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
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
      Guid guid = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
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
      Guid guid = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
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
      Guid guid = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
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
      Guid guid = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      return this.SendAsync<List<TestAttachment>>(new HttpMethod("GET"), new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50"), (object) new
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
      return this.SendAsync<List<TestAttachment>>(new HttpMethod("GET"), new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50"), (object) new
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
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
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
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
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      Guid locationId = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
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
      Guid locationId = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
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
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("99a64084-7ef3-4cd6-8ba1-1d71891fcc50");
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
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      Guid guid = new Guid("e78951d9-d80e-483d-ba86-9fe5e53a750e");
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
      Guid guid = new Guid("e78951d9-d80e-483d-ba86-9fe5e53a750e");
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e78951d9-d80e-483d-ba86-9fe5e53a750e");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e78951d9-d80e-483d-ba86-9fe5e53a750e");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<TestAttachment>> GetTestRunAttachmentsAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TestAttachment>>(new HttpMethod("GET"), new Guid("e78951d9-d80e-483d-ba86-9fe5e53a750e"), (object) new
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
      return this.SendAsync<List<TestAttachment>>(new HttpMethod("GET"), new Guid("e78951d9-d80e-483d-ba86-9fe5e53a750e"), (object) new
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e78951d9-d80e-483d-ba86-9fe5e53a750e");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e78951d9-d80e-483d-ba86-9fe5e53a750e");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
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
      return this.SendAsync<List<WorkItemReference>>(new HttpMethod("GET"), new Guid("f337d599-48f4-41a9-a21d-02892899faaf"), (object) new
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
      return this.SendAsync<List<WorkItemReference>>(new HttpMethod("GET"), new Guid("f337d599-48f4-41a9-a21d-02892899faaf"), (object) new
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
      Guid locationId = new Guid("21dfff92-0917-41bb-a4af-e68922678713");
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
      Guid locationId = new Guid("21dfff92-0917-41bb-a4af-e68922678713");
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
      Guid locationId = new Guid("21dfff92-0917-41bb-a4af-e68922678713");
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
      Guid locationId = new Guid("21dfff92-0917-41bb-a4af-e68922678713");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (deltaBuildId.HasValue)
        keyValuePairList.Add(nameof (deltaBuildId), deltaBuildId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<CodeCoverageSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateCodeCoverageSummaryAsync(
      CodeCoverageData coverageData,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClientBase tcmHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("21dfff92-0917-41bb-a4af-e68922678713");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CodeCoverageData>(coverageData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      TcmHttpClientBase tcmHttpClientBase2 = tcmHttpClientBase1;
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
      using (await tcmHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdateCodeCoverageSummaryAsync(
      CodeCoverageData coverageData,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClientBase tcmHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("21dfff92-0917-41bb-a4af-e68922678713");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CodeCoverageData>(coverageData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      TcmHttpClientBase tcmHttpClientBase2 = tcmHttpClientBase1;
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
      using (await tcmHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2).ConfigureAwait(false))
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
      Guid locationId = new Guid("b82bc30a-7228-4679-901c-0a59ac4b1252");
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
      Guid locationId = new Guid("b82bc30a-7228-4679-901c-0a59ac4b1252");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (flags), flags.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestRunCoverage>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultHistory> QueryTestResultHistoryAsync(
      ResultsFilter filter,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("eb9d9b32-4ccb-4c70-a72a-c7ba31d92fd2");
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
      Guid guid = new Guid("eb9d9b32-4ccb-4c70-a72a-c7ba31d92fd2");
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

    public virtual Task<UpdatedProperties> AbortTestRunAsync(
      AbortTestRunRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("04a5f40b-a3aa-4c3a-82c6-89174ad0e235");
      HttpContent httpContent = (HttpContent) new ObjectContent<AbortTestRunRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<UpdatedProperties>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<int>> CreateTestMessageLogEntriesAsync(
      CreateTestMessageLogEntryRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2bcbb8a7-1496-464c-9a64-72cd191c35b1");
      HttpContent httpContent = (HttpContent) new ObjectContent<CreateTestMessageLogEntryRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<int>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task CreateTestResultsLegacyAsync(
      CreateTestResultsRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClientBase tcmHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("459dc66b-20fc-4b7d-a7ea-88cf57d07616");
      HttpContent httpContent = (HttpContent) new ObjectContent<CreateTestResultsRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TcmHttpClientBase tcmHttpClientBase2 = tcmHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await tcmHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<LegacyTestRun> CreateTestRunLegacyAsync(
      CreateTestRunRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1e0d28b8-ed74-4d97-83af-2e3cd024a8bf");
      HttpContent httpContent = (HttpContent) new ObjectContent<CreateTestRunRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<LegacyTestRun>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTestRunIdsAsync(
      DeleteTestRunRequest deleteTestRunRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClientBase tcmHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8959fe42-ae7f-4f24-aa13-a6fd356d9fca");
      HttpContent httpContent = (HttpContent) new ObjectContent<DeleteTestRunRequest>(deleteTestRunRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TcmHttpClientBase tcmHttpClientBase2 = tcmHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await tcmHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task<Stream> DownloadAttachmentsLegacyContentAsync(
      DownloadAttachmentsRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("f16ee7c3-199b-419f-a8ba-d4a76661e5b3");
      HttpContent content = (HttpContent) new ObjectContent<DownloadAttachmentsRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("7.2-preview.1"), content: content, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> DownloadAttachmentsLegacyZipAsync(
      DownloadAttachmentsRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("f16ee7c3-199b-419f-a8ba-d4a76661e5b3");
      HttpContent content = (HttpContent) new ObjectContent<DownloadAttachmentsRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClientBase.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("7.2-preview.1"), content: content, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<FetchTestResultsResponse> FetchTestResultsAsync(
      FetchTestResultsRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2daf90f7-fc98-4acc-881c-eabaf2244ac5");
      HttpContent httpContent = (HttpContent) new ObjectContent<FetchTestResultsRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FetchTestResultsResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestResultAcrossProjectResponse> GetTestResultsAcrossProjectsAsync(
      LegacyTestCaseResultIdentifier identifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("188287b2-7878-4f28-98bd-d53dd8351d19");
      HttpContent httpContent = (HttpContent) new ObjectContent<LegacyTestCaseResultIdentifier>(identifier, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultAcrossProjectResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ResultsByQueryResponse> GetTestResultsByQueryLegacyAsync(
      ResultsByQueryRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c3801e01-633b-42e0-b045-7d6577aa1da8");
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultsByQueryRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ResultsByQueryResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<LegacyTestRun> GetTestRunByTmiRunIdAsync(
      Guid tmiRunId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("414c81b9-e7ca-4814-869b-216f18cbf510");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (tmiRunId), tmiRunId.ToString());
      return this.SendAsync<LegacyTestRun>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<LegacyTestCaseResult>> QueryByPointAsync(
      QueryByPointRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ef28b894-f109-453b-bed9-a5a90c58264c");
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryByPointRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<LegacyTestCaseResult>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<FetchTestResultsResponse> QueryByRunAsync(
      QueryByRunRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bc2b04d6-441b-46d7-b3cd-3db127fcfc46");
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryByRunRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FetchTestResultsResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<FetchTestResultsResponse> QueryByRunAndOutcomeAsync(
      QueryByRunRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("3f690505-1d33-470c-a309-d279c015b7e8");
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryByRunRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FetchTestResultsResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<FetchTestResultsResponse> QueryByRunAndOwnerAsync(
      QueryByRunRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("980fe82f-bd10-4d3b-ad46-00b679f1b628");
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryByRunRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FetchTestResultsResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<FetchTestResultsResponse> QueryByRunAndStateAsync(
      QueryByRunRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6516def2-10dc-49c3-8557-1001af5ba0d0");
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryByRunRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FetchTestResultsResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<QueryTestActionResultResponse> QueryTestActionResultsAsync(
      QueryTestActionResultRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d8eb6ab5-0457-4ec0-8bf9-a2be683188b3");
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryTestActionResultRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<QueryTestActionResultResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestMessageLogEntry>> QueryTestMessageLogEntryAsync(
      QueryTestMessageLogEntryRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1b5d25b4-e83c-4400-b8da-c3c89f14cdde");
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryTestMessageLogEntryRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestMessageLogEntry>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<LegacyTestRun>> QueryTestRuns2Async(
      QueryTestRuns2Request request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9d897c0c-48a5-430b-a96a-6b033ee823a4");
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryTestRuns2Request>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<LegacyTestRun>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<LegacyTestRun>> QueryTestRunsAcrossMultipleProjectsAsync(
      ResultsStoreQuery query,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("466760db-95dd-413e-9030-bc109cc16c86");
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultsStoreQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<LegacyTestRun>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<int> QueryTestRunsCountAsync(
      ResultsStoreQuery query,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2fe7c059-cb0d-4239-a37b-5cf10e57c2b4");
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultsStoreQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<int>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<LegacyTestRun>> QueryTestRunsLegacyAsync(
      QueryTestRunsRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5cc14c9a-6782-4773-98f2-94846aa10c47");
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryTestRunsRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<LegacyTestRun>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<LegacyTestRunStatistic>> QueryTestRunStatsAsync(
      QueryTestRunStatsRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9b343f31-9e27-4d5f-9450-99341aa62954");
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryTestRunStatsRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<LegacyTestRunStatistic>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<LegacyTestCaseResult[]> ResetTestResultsAsync(
      ResetTestResultsRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("535f2f8b-67a7-4783-b304-71855008b15e");
      HttpContent httpContent = (HttpContent) new ObjectContent<ResetTestResultsRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<LegacyTestCaseResult[]>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ResultUpdateResponse[]> UpdateTestResultsLegacyAsync(
      BulkResultUpdateRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2d8bef90-cf8f-44b2-8cff-148925458c16");
      HttpContent httpContent = (HttpContent) new ObjectContent<BulkResultUpdateRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ResultUpdateResponse[]>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<UpdateTestRunResponse> UpdateTestRunLegacyAsync(
      UpdateTestRunRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6afb5d3c-3a6f-4598-b6cc-fcf187dd3e6e");
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateTestRunRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<UpdateTestRunResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task UploadAttachmentsLegacyAsync(
      UploadAttachmentsRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClientBase tcmHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d1add0dd-79c5-40b1-b8bf-ca37c79ba764");
      HttpContent httpContent = (HttpContent) new ObjectContent<UploadAttachmentsRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TcmHttpClientBase tcmHttpClientBase2 = tcmHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await tcmHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<PointLastResult>> FilterPointsAsync(
      FilterPointQuery request,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("905d7331-24be-4cbf-ac54-70c2c838ab7a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<FilterPointQuery>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<PointLastResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<PointLastResult>> FilterPointsAsync(
      FilterPointQuery request,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("905d7331-24be-4cbf-ac54-70c2c838ab7a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<FilterPointQuery>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<PointLastResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestResultsWithWatermark> GetManualTestResultsByUpdatedDateAsync(
      string project,
      DateTime updateDate,
      int fromRunId,
      int fromResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c1da87e8-68e7-46c4-9c64-bc449ab358dd");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (updateDate), updateDate);
      keyValuePairList.Add(nameof (fromRunId), fromRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (fromResultId), fromResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TestResultsWithWatermark>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultsWithWatermark> GetManualTestResultsByUpdatedDateAsync(
      Guid project,
      DateTime updateDate,
      int fromRunId,
      int fromResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c1da87e8-68e7-46c4-9c64-bc449ab358dd");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (updateDate), updateDate);
      keyValuePairList.Add(nameof (fromRunId), fromRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (fromResultId), fromResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TestResultsWithWatermark>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
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
      Guid locationId = new Guid("aae1bb55-a1b2-4951-86f5-0e72f1396ef9");
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
      Guid locationId = new Guid("aae1bb55-a1b2-4951-86f5-0e72f1396ef9");
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
      Guid locationId = new Guid("5ee72329-c86b-4e7e-be8e-0744c2301a84");
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
      Guid locationId = new Guid("5ee72329-c86b-4e7e-be8e-0744c2301a84");
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
      Guid locationId = new Guid("af70663f-e385-4d73-9d59-3f44a5d9e066");
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
      Guid locationId = new Guid("af70663f-e385-4d73-9d59-3f44a5d9e066");
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
      Guid locationId = new Guid("5e746c5c-4fb7-46f7-bc6c-913110a98fbf");
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
      Guid locationId = new Guid("5e746c5c-4fb7-46f7-bc6c-913110a98fbf");
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

    public virtual Task<TestResultsQuery> GetTestResultsByQueryAsync(
      TestResultsQuery query,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5645f3b7-c0be-4e21-9cbb-ca91db31a422");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultsQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
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
      Guid guid = new Guid("5645f3b7-c0be-4e21-9cbb-ca91db31a422");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultsQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultsQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestCaseResult>> AddTestResultsToTestRunAsync(
      TestCaseResult[] results,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0655a660-543a-4408-93d7-a28d79cf1560");
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

    public virtual Task<List<TestCaseResult>> AddTestResultsToTestRunAsync(
      TestCaseResult[] results,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0655a660-543a-4408-93d7-a28d79cf1560");
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

    public virtual Task<TestCaseResult> GetTestResultByIdAsync(
      string project,
      int runId,
      int testResultId,
      ResultDetails? detailsToInclude = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0655a660-543a-4408-93d7-a28d79cf1560");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testResultId = testResultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        keyValuePairList.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      return this.SendAsync<TestCaseResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
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
      Guid locationId = new Guid("0655a660-543a-4408-93d7-a28d79cf1560");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testResultId = testResultId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        keyValuePairList.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      return this.SendAsync<TestCaseResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
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
      Guid locationId = new Guid("0655a660-543a-4408-93d7-a28d79cf1560");
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
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
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
      Guid locationId = new Guid("0655a660-543a-4408-93d7-a28d79cf1560");
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
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestCaseResult>> UpdateTestResultsAsync(
      TestCaseResult[] results,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0655a660-543a-4408-93d7-a28d79cf1560");
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

    public virtual Task<List<TestCaseResult>> UpdateTestResultsAsync(
      TestCaseResult[] results,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0655a660-543a-4408-93d7-a28d79cf1560");
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

    public virtual Task<List<LegacyTestCaseResult>> CreateBatchedBlockedResultsAsync(
      IEnumerable<LegacyTestCaseResult> testCaseResults,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ce47a161-b387-4f55-b22e-2d3d5956fc1d");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<LegacyTestCaseResult>>(testCaseResults, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<LegacyTestCaseResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<LegacyTestCaseResult>> CreateBatchedBlockedResultsAsync(
      IEnumerable<LegacyTestCaseResult> testCaseResults,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ce47a161-b387-4f55-b22e-2d3d5956fc1d");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<LegacyTestCaseResult>>(testCaseResults, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<LegacyTestCaseResult>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
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
      Guid locationId = new Guid("dc342339-9bf3-480f-8c41-a867caf3cd1e");
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
      Guid locationId = new Guid("dc342339-9bf3-480f-8c41-a867caf3cd1e");
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
      Guid locationId = new Guid("8fc27f1b-12db-4c69-bf06-7ec026688c2d");
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
      Guid locationId = new Guid("8fc27f1b-12db-4c69-bf06-7ec026688c2d");
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
      Guid locationId = new Guid("2da0c8df-4ab9-4f4d-9aa8-e7859d63fdf1");
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
      Guid locationId = new Guid("2da0c8df-4ab9-4f4d-9aa8-e7859d63fdf1");
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
      Guid locationId = new Guid("46051c64-54d6-438f-9440-df9b635eb32d");
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
      Guid locationId = new Guid("46051c64-54d6-438f-9440-df9b635eb32d");
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
      Guid guid = new Guid("46051c64-54d6-438f-9440-df9b635eb32d");
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
      Guid guid = new Guid("46051c64-54d6-438f-9440-df9b635eb32d");
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
      Guid guid = new Guid("3f4bf032-936f-48ff-abc3-5bba67ec151e");
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
      Guid guid = new Guid("3f4bf032-936f-48ff-abc3-5bba67ec151e");
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
      Guid guid = new Guid("177e214e-1f56-4544-a68c-2b55496bd366");
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
      Guid guid = new Guid("177e214e-1f56-4544-a68c-2b55496bd366");
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
      Guid guid = new Guid("79376647-dd74-4b15-b0d4-243e332e169b");
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
      Guid guid = new Guid("79376647-dd74-4b15-b0d4-243e332e169b");
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
      return this.SendAsync<TestRunStatistic>(new HttpMethod("GET"), new Guid("ee95ff93-38aa-4f10-88f6-74323c933711"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestRunStatistic> GetTestRunStatisticsAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestRunStatistic>(new HttpMethod("GET"), new Guid("ee95ff93-38aa-4f10-88f6-74323c933711"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestRun> CreateTestRunAsync(
      RunCreateModel testRun,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7");
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
      Guid guid = new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7");
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
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7"), (object) new
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
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7"), (object) new
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
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDetails.HasValue)
        keyValuePairList.Add(nameof (includeDetails), includeDetails.Value.ToString());
      return this.SendAsync<TestRun>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestRun> GetTestRunByIdAsync(
      Guid project,
      int runId,
      bool? includeDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDetails.HasValue)
        keyValuePairList.Add(nameof (includeDetails), includeDetails.Value.ToString());
      return this.SendAsync<TestRun>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
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
      Guid locationId = new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7");
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
      Guid locationId = new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7");
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

    public virtual Task<List<TestRun>> GetTestRunsByQueryAsync(
      QueryModel query,
      string project,
      bool includeIdsOnly,
      bool? includeRunDetails = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryModel>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      collection1.Add(nameof (includeIdsOnly), includeIdsOnly.ToString());
      if (includeRunDetails.HasValue)
        collection1.Add(nameof (includeRunDetails), includeRunDetails.Value.ToString());
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection2.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection3.Add("$top", str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestRun>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<TestRun>> GetTestRunsByQueryAsync(
      QueryModel query,
      Guid project,
      bool includeIdsOnly,
      bool? includeRunDetails = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryModel>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      collection1.Add(nameof (includeIdsOnly), includeIdsOnly.ToString());
      if (includeRunDetails.HasValue)
        collection1.Add(nameof (includeRunDetails), includeRunDetails.Value.ToString());
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection2.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection3.Add("$top", str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestRun>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
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
      Guid locationId = new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7");
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
      Guid locationId = new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7");
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
      Guid guid = new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7");
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
      Guid guid = new Guid("86337575-7ef2-4612-a60e-03cfb8c455b7");
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

    public virtual Task<TestExecutionReportData> GetTestRunSummaryReportAsync(
      string project,
      int runId,
      IEnumerable<string> dimensionList = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("64ad9c84-aba3-4031-8547-3ccf2dce6804");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (dimensionList != null && dimensionList.Any<string>())
        keyValuePairList.Add(nameof (dimensionList), string.Join(",", dimensionList));
      return this.SendAsync<TestExecutionReportData>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestExecutionReportData> GetTestRunSummaryReportAsync(
      Guid project,
      int runId,
      IEnumerable<string> dimensionList = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("64ad9c84-aba3-4031-8547-3ccf2dce6804");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (dimensionList != null && dimensionList.Any<string>())
        keyValuePairList.Add(nameof (dimensionList), string.Join(",", dimensionList));
      return this.SendAsync<TestExecutionReportData>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestExecutionReportData> GetTestExecutionSummaryReportAsync(
      IEnumerable<TestAuthoringDetails> testAuthoringDetails,
      string project,
      int planId,
      IEnumerable<string> dimensionList = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c012317d-845c-4e0e-b93e-56c3fcedb95a");
      object obj1 = (object) new
      {
        project = project,
        planId = planId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestAuthoringDetails>>(testAuthoringDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (dimensionList != null && dimensionList.Any<string>())
        collection.Add(nameof (dimensionList), string.Join(",", dimensionList));
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
      return this.SendAsync<TestExecutionReportData>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<TestExecutionReportData> GetTestExecutionSummaryReportAsync(
      IEnumerable<TestAuthoringDetails> testAuthoringDetails,
      Guid project,
      int planId,
      IEnumerable<string> dimensionList = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c012317d-845c-4e0e-b93e-56c3fcedb95a");
      object obj1 = (object) new
      {
        project = project,
        planId = planId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestAuthoringDetails>>(testAuthoringDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (dimensionList != null && dimensionList.Any<string>())
        collection.Add(nameof (dimensionList), string.Join(",", dimensionList));
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
      return this.SendAsync<TestExecutionReportData>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<TestHistoryQuery> QueryTestHistoryAsync(
      TestHistoryQuery filter,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("98a1887b-e629-4744-a62e-99e8f0e17704");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestHistoryQuery>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
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
      Guid guid = new Guid("98a1887b-e629-4744-a62e-99e8f0e17704");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestHistoryQuery>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestHistoryQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<int> CreateTestSettingsAsync(
      TestSettings testSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5f0315cc-4d71-4ed8-b445-f7eed13a8399");
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
      Guid guid = new Guid("5f0315cc-4d71-4ed8-b445-f7eed13a8399");
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
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("5f0315cc-4d71-4ed8-b445-f7eed13a8399");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSettingsId), testSettingsId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await tcmHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestSettingsAsync(
      Guid project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClientBase tcmHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("5f0315cc-4d71-4ed8-b445-f7eed13a8399");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSettingsId), testSettingsId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await tcmHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TestSettings> GetTestSettingsByIdAsync(
      string project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5f0315cc-4d71-4ed8-b445-f7eed13a8399");
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
      Guid locationId = new Guid("5f0315cc-4d71-4ed8-b445-f7eed13a8399");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSettingsId), testSettingsId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TestSettings>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<LegacyTestSettings> GetTestSettingsCompatByIdAsync(
      string project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("06c36c34-dd95-4ffd-be7f-70135fcdf4ec");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSettingsId), testSettingsId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<LegacyTestSettings>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<LegacyTestSettings> GetTestSettingsCompatByIdAsync(
      Guid project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("06c36c34-dd95-4ffd-be7f-70135fcdf4ec");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSettingsId), testSettingsId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<LegacyTestSettings>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<UpdatedProperties> UpdateTestSettingsAsync(
      LegacyTestSettings legacyTestSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("06c36c34-dd95-4ffd-be7f-70135fcdf4ec");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<LegacyTestSettings>(legacyTestSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<UpdatedProperties>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<UpdatedProperties> UpdateTestSettingsAsync(
      LegacyTestSettings legacyTestSettings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("06c36c34-dd95-4ffd-be7f-70135fcdf4ec");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<LegacyTestSettings>(legacyTestSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<UpdatedProperties>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<UpdatedProperties> CreateTestSettingsCompatAsync(
      LegacyTestSettings legacyTestSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("898fbf7e-d847-4145-bcb8-f6e887a4ec67");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<LegacyTestSettings>(legacyTestSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<UpdatedProperties>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<UpdatedProperties> CreateTestSettingsCompatAsync(
      LegacyTestSettings legacyTestSettings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("898fbf7e-d847-4145-bcb8-f6e887a4ec67");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<LegacyTestSettings>(legacyTestSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<UpdatedProperties>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<LegacyTestSettings>> QueryTestSettingsAsync(
      ResultsStoreQuery query,
      string project,
      bool omitSettings,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e87c5c1a-1109-4f47-afc7-9205095f3b5a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultsStoreQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (omitSettings), omitSettings.ToString());
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
      return this.SendAsync<List<LegacyTestSettings>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<LegacyTestSettings>> QueryTestSettingsAsync(
      ResultsStoreQuery query,
      Guid project,
      bool omitSettings,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e87c5c1a-1109-4f47-afc7-9205095f3b5a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultsStoreQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (omitSettings), omitSettings.ToString());
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
      return this.SendAsync<List<LegacyTestSettings>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<int> QueryTestSettingsCountAsync(
      ResultsStoreQuery query,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6133ddc0-a54a-4eca-b0d7-b86fa87d48ba");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultsStoreQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
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

    public virtual Task<int> QueryTestSettingsCountAsync(
      ResultsStoreQuery query,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6133ddc0-a54a-4eca-b0d7-b86fa87d48ba");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResultsStoreQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
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

    public virtual Task<WorkItemToTestLinks> AddWorkItemToTestLinksAsync(
      WorkItemToTestLinks workItemToTestLinks,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("84ccb084-255b-483e-b7ea-9d39a273b5fe");
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

    public virtual Task<WorkItemToTestLinks> AddWorkItemToTestLinksAsync(
      WorkItemToTestLinks workItemToTestLinks,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("84ccb084-255b-483e-b7ea-9d39a273b5fe");
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

    public virtual Task<bool> DeleteTestMethodToWorkItemLinkAsync(
      string project,
      string testName,
      int workItemId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("f961577c-5122-4651-adfd-c9d500b15d41");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testName), testName);
      keyValuePairList.Add(nameof (workItemId), workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<bool>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<bool> DeleteTestMethodToWorkItemLinkAsync(
      Guid project,
      string testName,
      int workItemId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("f961577c-5122-4651-adfd-c9d500b15d41");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testName), testName);
      keyValuePairList.Add(nameof (workItemId), workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<bool>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestToWorkItemLinks> QueryTestMethodLinkedWorkItemsAsync(
      string project,
      string testName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("f961577c-5122-4651-adfd-c9d500b15d41");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testName), testName);
      return this.SendAsync<TestToWorkItemLinks>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestToWorkItemLinks> QueryTestMethodLinkedWorkItemsAsync(
      Guid project,
      string testName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("f961577c-5122-4651-adfd-c9d500b15d41");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testName), testName);
      return this.SendAsync<TestToWorkItemLinks>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
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
      Guid locationId = new Guid("c7d72725-0479-4c83-8dbc-2bf27010b78b");
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
      Guid locationId = new Guid("c7d72725-0479-4c83-8dbc-2bf27010b78b");
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
