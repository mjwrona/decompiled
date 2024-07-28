// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.WebApi.TcmHttpClient
// Assembly: Microsoft.VisualStudio.Services.Tcm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DCD48481-6B90-4012-9254-BC9E7077DAC8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.WebApi.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
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

namespace Microsoft.VisualStudio.Services.Tcm.WebApi
{
  [ResourceArea("C08C062A-B973-4754-B339-8DE3B6FE53EC")]
  public class TcmHttpClient : TcmHttpClientBase, ITestResultsHttpClient
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();

    public TcmHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TcmHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TcmHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TcmHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TcmHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    static TcmHttpClient()
    {
      TcmHttpClient.s_translatedExceptions.Add("AccessDeniedException", typeof (AccessDeniedException));
      TcmHttpClient.s_translatedExceptions.Add("TestObjectNotFoundException", typeof (TestObjectNotFoundException));
      TcmHttpClient.s_translatedExceptions.Add("TeamProjectNotFoundException", typeof (TeamProjectNotFoundException));
      TcmHttpClient.s_translatedExceptions.Add("InvalidPropertyException", typeof (InvalidPropertyException));
      TcmHttpClient.s_translatedExceptions.Add("TestObjectInUseException", typeof (TestObjectInUseException));
      TcmHttpClient.s_translatedExceptions.Add("TestObjectUpdatedException", typeof (TestObjectUpdatedException));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<FieldDetailsForTestResults>> GetResultGroupsByBuildWithContinuationTokenAsync(
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
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<FieldDetailsForTestResults>>>(this.GetPagedList<FieldDetailsForTestResults>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<FieldDetailsForTestResults>> GetResultGroupsByBuildWithContinuationTokenAsync(
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
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<FieldDetailsForTestResults>>>(this.GetPagedList<FieldDetailsForTestResults>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<FieldDetailsForTestResults>> GetResultGroupsByReleaseWithContinuationTokenAsync(
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
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (releaseEnvId.HasValue)
        keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<FieldDetailsForTestResults>>>(this.GetPagedList<FieldDetailsForTestResults>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<FieldDetailsForTestResults>> GetResultGroupsByReleaseWithContinuationTokenAsync(
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
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (releaseEnvId.HasValue)
        keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<FieldDetailsForTestResults>>>(this.GetPagedList<FieldDetailsForTestResults>));
    }

    public virtual Task<TestResultsGroupsForBuild> GetResultGroupsByBuildV1Async(
      string project,
      int buildId,
      string publishContext,
      IEnumerable<string> fields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("af70663f-e385-4d73-9d59-3f44a5d9e066");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      return this.SendAsync<TestResultsGroupsForBuild>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<TestResultsGroupsForBuild>>) null);
    }

    public virtual Task<TestResultsGroupsForBuild> GetResultGroupsByBuildV1Async(
      Guid project,
      int buildId,
      string publishContext,
      IEnumerable<string> fields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("af70663f-e385-4d73-9d59-3f44a5d9e066");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      return this.SendAsync<TestResultsGroupsForBuild>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<TestResultsGroupsForBuild>>) null);
    }

    public virtual Task<TestResultsGroupsForRelease> GetResultGroupsByReleaseV1Async(
      string project,
      int releaseId,
      string publishContext,
      int? releaseEnvId = null,
      IEnumerable<string> fields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5e746c5c-4fb7-46f7-bc6c-913110a98fbf");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (releaseEnvId.HasValue)
        keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      return this.SendAsync<TestResultsGroupsForRelease>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<TestResultsGroupsForRelease>>) null);
    }

    public virtual Task<TestResultsGroupsForRelease> GetResultGroupsByReleaseV1Async(
      Guid project,
      int releaseId,
      string publishContext,
      int? releaseEnvId = null,
      IEnumerable<string> fields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5e746c5c-4fb7-46f7-bc6c-913110a98fbf");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (releaseEnvId.HasValue)
        keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      return this.SendAsync<TestResultsGroupsForRelease>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<TestResultsGroupsForRelease>>) null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<TcmAttachment> DownloadAttachmentsLegacyAsync(
      DownloadAttachmentsRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClient tcmHttpClient = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("f16ee7c3-199b-419f-a8ba-d4a76661e5b3");
      HttpContent content = (HttpContent) new ObjectContent<DownloadAttachmentsRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClient.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("5.0-preview.1"), content: content, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      TcmAttachment tcmAttachment1 = new TcmAttachment();
      TcmAttachment tcmAttachment3 = tcmAttachment1;
      string str;
      if (httpResponseMessage.Content.Headers.ContentDisposition.FileName == null)
        str = string.Empty;
      else
        str = httpResponseMessage.Content.Headers.ContentDisposition.FileName.Trim('"');
      tcmAttachment3.FileName = str;
      tcmAttachment1.ContentLength = httpResponseMessage.Content.Headers.ContentLength.GetValueOrDefault();
      tcmAttachment1.ContentType = httpResponseMessage.Content.Headers.ContentType != null ? httpResponseMessage.Content.Headers.ContentType.MediaType : string.Empty;
      if (httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        tcmAttachment1.Stream = (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        TcmAttachment tcmAttachment2 = tcmAttachment1;
        tcmAttachment2.Stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        tcmAttachment2 = (TcmAttachment) null;
      }
      TcmAttachment tcmAttachment4 = tcmAttachment1;
      tcmAttachment1 = (TcmAttachment) null;
      return tcmAttachment4;
    }

    public virtual async Task<TcmAttachment> GetTcmResultAttachmentContentAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClient tcmHttpClient = this;
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
      using (HttpRequestMessage requestMessage = await tcmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      TcmAttachment tcmAttachment1 = new TcmAttachment();
      tcmAttachment1.FileName = httpResponseMessage.Content.Headers.ContentDisposition.FileName.Trim('"');
      if (httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        tcmAttachment1.CompressionType = "gzip";
        tcmAttachment1.Stream = (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        TcmAttachment tcmAttachment2 = tcmAttachment1;
        tcmAttachment2.Stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        tcmAttachment2 = (TcmAttachment) null;
      }
      TcmAttachment attachmentContentAsync = tcmAttachment1;
      tcmAttachment1 = (TcmAttachment) null;
      return attachmentContentAsync;
    }

    public virtual async Task<TcmAttachment> GetTcmRunAttachmentContentAsync(
      Guid project,
      int runId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClient tcmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e78951d9-d80e-483d-ba86-9fe5e53a750e");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      TcmAttachment tcmAttachment1 = new TcmAttachment();
      tcmAttachment1.FileName = httpResponseMessage.Content.Headers.ContentDisposition.FileName.Trim('"');
      if (httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        tcmAttachment1.CompressionType = "gzip";
        tcmAttachment1.Stream = (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        TcmAttachment tcmAttachment2 = tcmAttachment1;
        tcmAttachment2.Stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        tcmAttachment2 = (TcmAttachment) null;
      }
      TcmAttachment attachmentContentAsync = tcmAttachment1;
      tcmAttachment1 = (TcmAttachment) null;
      return attachmentContentAsync;
    }

    public virtual async Task<TcmAttachment> GetTcmSubResultAttachmentContentAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int testSubResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClient tcmHttpClient = this;
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
      using (HttpRequestMessage requestMessage = await tcmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      TcmAttachment tcmAttachment1 = new TcmAttachment();
      tcmAttachment1.FileName = httpResponseMessage.Content.Headers.ContentDisposition.FileName.Trim('"');
      if (httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        tcmAttachment1.CompressionType = "gzip";
        tcmAttachment1.Stream = (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        TcmAttachment tcmAttachment2 = tcmAttachment1;
        tcmAttachment2.Stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        tcmAttachment2 = (TcmAttachment) null;
      }
      TcmAttachment attachmentContentAsync = tcmAttachment1;
      tcmAttachment1 = (TcmAttachment) null;
      return attachmentContentAsync;
    }

    public virtual async Task<TcmAttachment> GetTestResultAttachmentContentAsync(
      string project,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TcmHttpClient tcmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9a941c36-fa09-446c-b15c-6c1dc03efce9");
      object routeValues = (object) new
      {
        project = project,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tcmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tcmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      TcmAttachment tcmAttachment1 = new TcmAttachment();
      string stringToUnescape;
      if (httpResponseMessage.Content.Headers.ContentDisposition.FileName == null)
        stringToUnescape = string.Empty;
      else
        stringToUnescape = httpResponseMessage.Content.Headers.ContentDisposition.FileName.Trim('"');
      tcmAttachment1.FileName = Uri.UnescapeDataString(stringToUnescape);
      if (httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        tcmAttachment1.Stream = (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        TcmAttachment tcmAttachment2 = tcmAttachment1;
        tcmAttachment2.Stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        tcmAttachment2 = (TcmAttachment) null;
      }
      TcmAttachment attachmentContentAsync = tcmAttachment1;
      tcmAttachment1 = (TcmAttachment) null;
      return attachmentContentAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<ShallowTestCaseResult>> GetTestResultsByBuildAsync2(
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
      Guid locationId = new Guid("DC342339-9BF3-480F-8C41-A867CAF3CD1E");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<ShallowTestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<ShallowTestCaseResult>>>(this.GetPagedList<ShallowTestCaseResult>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<ShallowTestCaseResult>> GetTestResultsByBuildAsync2(
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
      Guid locationId = new Guid("DC342339-9BF3-480F-8C41-A867CAF3CD1E");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<ShallowTestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<ShallowTestCaseResult>>>(this.GetPagedList<ShallowTestCaseResult>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<ShallowTestCaseResult>> GetTestResultsByReleaseAsync2(
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
      Guid locationId = new Guid("8FC27F1B-12DB-4C69-BF06-7EC026688C2D");
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
      if (!string.IsNullOrEmpty(publishContext))
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
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<ShallowTestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<ShallowTestCaseResult>>>(this.GetPagedList<ShallowTestCaseResult>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<ShallowTestCaseResult>> GetTestResultsByReleaseAsync2(
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
      Guid locationId = new Guid("8FC27F1B-12DB-4C69-BF06-7EC026688C2D");
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
      if (!string.IsNullOrEmpty(publishContext))
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
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<ShallowTestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<ShallowTestCaseResult>>>(this.GetPagedList<ShallowTestCaseResult>));
    }

    public virtual Task<IPagedList<TestRun>> QueryTestRunsAsync2(
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
      Guid locationId = new Guid("86337575-7EF2-4612-A60E-03CFB8C455B7");
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
      if (!string.IsNullOrEmpty(branchName))
        keyValuePairList.Add(nameof (branchName), branchName);
      if (releaseIds != null && releaseIds.Any<int>())
        keyValuePairList.Add(nameof (releaseIds), string.Join<int>(",", releaseIds));
      if (releaseDefIds != null && releaseDefIds.Any<int>())
        keyValuePairList.Add(nameof (releaseDefIds), string.Join<int>(",", releaseDefIds));
      if (releaseEnvIds != null && releaseEnvIds.Any<int>())
        keyValuePairList.Add(nameof (releaseEnvIds), string.Join<int>(",", releaseEnvIds));
      if (releaseEnvDefIds != null && releaseEnvDefIds.Any<int>())
        keyValuePairList.Add(nameof (releaseEnvDefIds), string.Join<int>(",", releaseEnvDefIds));
      if (!string.IsNullOrEmpty(runTitle))
        keyValuePairList.Add(nameof (runTitle), runTitle);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<TestRun>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<TestRun>>>(this.GetPagedList<TestRun>));
    }

    public virtual Task<IPagedList<TestRun>> QueryTestRunsAsync2(
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
      Guid locationId = new Guid("86337575-7EF2-4612-A60E-03CFB8C455B7");
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
      if (!string.IsNullOrEmpty(branchName))
        keyValuePairList.Add(nameof (branchName), branchName);
      if (releaseIds != null && releaseIds.Any<int>())
        keyValuePairList.Add(nameof (releaseIds), string.Join<int>(",", releaseIds));
      if (releaseDefIds != null && releaseDefIds.Any<int>())
        keyValuePairList.Add(nameof (releaseDefIds), string.Join<int>(",", releaseDefIds));
      if (releaseEnvIds != null && releaseEnvIds.Any<int>())
        keyValuePairList.Add(nameof (releaseEnvIds), string.Join<int>(",", releaseEnvIds));
      if (releaseEnvDefIds != null && releaseEnvDefIds.Any<int>())
        keyValuePairList.Add(nameof (releaseEnvDefIds), string.Join<int>(",", releaseEnvDefIds));
      if (!string.IsNullOrEmpty(runTitle))
        keyValuePairList.Add(nameof (runTitle), runTitle);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<TestRun>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<TestRun>>>(this.GetPagedList<TestRun>));
    }

    public virtual Task<TestTagSummary> GetTestTagSummaryForBuildAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException();
    }

    public virtual Task<TestTagSummary> GetTestTagSummaryForBuildAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException();
    }

    public virtual Task<TestTagSummary> GetTestTagSummaryForReleaseAsync(
      string project,
      int releaseId,
      int releaseEnvId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException();
    }

    public virtual Task<TestTagSummary> GetTestTagSummaryForReleaseAsync(
      Guid project,
      int releaseId,
      int releaseEnvId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException();
    }

    public virtual Task<List<TestTag>> GetTestTagsForBuildAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException();
    }

    public virtual Task<List<TestTag>> GetTestTagsForBuildAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException();
    }

    public virtual Task<List<TestTag>> GetTestTagsForReleaseAsync(
      string project,
      int releaseId,
      int releaseEnvId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException();
    }

    public virtual Task<List<TestTag>> GetTestTagsForReleaseAsync(
      Guid project,
      int releaseId,
      int releaseEnvId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException();
    }

    public virtual Task<TestRunStatistic> GetTestRunSummaryByOutcomeAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException();
    }

    public virtual Task<TestResultsSettings> GetTestResultsSettingsAsync(
      string project,
      TestResultsSettingsType? settingsType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException();
    }

    public HttpClient HttpClient => this.Client;

    protected async Task<IPagedList<T>> GetPagedList<T>(
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      TcmHttpClient tcmHttpClient = this;
      string continuationToken = tcmHttpClient.GetContinuationToken(responseMessage);
      IPagedList<T> pagedList = (IPagedList<T>) new PagedList<T>((IEnumerable<T>) await tcmHttpClient.ReadContentAsAsync<List<T>>(responseMessage, cancellationToken).ConfigureAwait(false), continuationToken);
      continuationToken = (string) null;
      return pagedList;
    }

    protected string GetContinuationToken(HttpResponseMessage responseMessage)
    {
      string continuationToken = (string) null;
      IEnumerable<string> values = (IEnumerable<string>) null;
      if (responseMessage.Headers.TryGetValues("x-ms-continuationtoken", out values))
        continuationToken = values.FirstOrDefault<string>();
      return continuationToken;
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
      TcmHttpClient tcmHttpClient = this;
      T obj;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await tcmHttpClient.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          obj = await tcmHttpClient.SendAsync<T>(requestMessage, userState, cancellationToken, processResponse).ConfigureAwait(false);
      }
      return obj;
    }

    protected async Task<T> SendAsync<T>(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      TcmHttpClient tcmHttpClient = this;
      if (processResponse == null)
        processResponse = new Func<HttpResponseMessage, CancellationToken, Task<T>>(((VssHttpClientBase) tcmHttpClient).ReadContentAsAsync<T>);
      T obj;
      using (HttpResponseMessage response = await tcmHttpClient.SendAsync(message, userState, cancellationToken).ConfigureAwait(false))
        obj = await processResponse(response, cancellationToken).ConfigureAwait(false);
      return obj;
    }

    public virtual Task<TestRun> GetTestRunByIdAsync(
      Guid project,
      int runId,
      bool? includeDetails = null,
      bool? includeTags = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetTestRunByIdAsync(project, runId, includeDetails, userState, cancellationToken);
    }

    public virtual Task<TestRun> GetTestRunByIdAsync(
      string project,
      int runId,
      bool? includeDetails = null,
      bool? includeTags = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetTestRunByIdAsync(project, runId, includeDetails, userState, cancellationToken);
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) TcmHttpClient.s_translatedExceptions;
  }
}
