// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Coverage.WebApi.CoverageHttpClient
// Assembly: Microsoft.TeamFoundation.Coverage.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 18FFD1B8-3EB9-46CC-8BE4-74DD890A1980
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Coverage.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Coverage.WebApi
{
  [ResourceArea("F86B1517-B514-464E-967E-07567F868756")]
  public class CoverageHttpClient : VssHttpClientBase
  {
    public CoverageHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public CoverageHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public CoverageHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public CoverageHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public CoverageHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<CoverageChangeSummaryResponse> GetChangesAsync(
      string project,
      int pipelineRunId,
      string scope = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CoverageHttpClient coverageHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("73c802cf-b97d-4d2f-a8f6-fe0a8bd8b376");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pipelineRunId), pipelineRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (scope != null)
        keyValuePairList.Add(nameof (scope), scope);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      CoverageChangeSummaryResponse changesAsync;
      using (HttpRequestMessage requestMessage = await coverageHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        CoverageChangeSummaryResponse returnObject = new CoverageChangeSummaryResponse();
        using (HttpResponseMessage response = await coverageHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          CoverageChangeSummaryResponse changeSummaryResponse1 = returnObject;
          IEnumerable<string> headerValue = coverageHttpClient.GetHeaderValue(response, "x-ms-continuationtoken");
          string str = headerValue != null ? headerValue.FirstOrDefault<string>() : (string) null;
          changeSummaryResponse1.ContinuationToken = str;
          CoverageChangeSummaryResponse changeSummaryResponse = returnObject;
          changeSummaryResponse.CoverageChangeSummary = await coverageHttpClient.ReadContentAsAsync<CoverageChangeSummary>(response, cancellationToken).ConfigureAwait(false);
          changeSummaryResponse = (CoverageChangeSummaryResponse) null;
        }
        changesAsync = returnObject;
      }
      return changesAsync;
    }

    public async Task<CoverageChangeSummaryResponse> GetChangesAsync(
      Guid project,
      int pipelineRunId,
      string scope = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CoverageHttpClient coverageHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("73c802cf-b97d-4d2f-a8f6-fe0a8bd8b376");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pipelineRunId), pipelineRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (scope != null)
        keyValuePairList.Add(nameof (scope), scope);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      CoverageChangeSummaryResponse changesAsync;
      using (HttpRequestMessage requestMessage = await coverageHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        CoverageChangeSummaryResponse returnObject = new CoverageChangeSummaryResponse();
        using (HttpResponseMessage response = await coverageHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          CoverageChangeSummaryResponse changeSummaryResponse1 = returnObject;
          IEnumerable<string> headerValue = coverageHttpClient.GetHeaderValue(response, "x-ms-continuationtoken");
          string str = headerValue != null ? headerValue.FirstOrDefault<string>() : (string) null;
          changeSummaryResponse1.ContinuationToken = str;
          CoverageChangeSummaryResponse changeSummaryResponse = returnObject;
          changeSummaryResponse.CoverageChangeSummary = await coverageHttpClient.ReadContentAsAsync<CoverageChangeSummary>(response, cancellationToken).ConfigureAwait(false);
          changeSummaryResponse = (CoverageChangeSummaryResponse) null;
        }
        changesAsync = returnObject;
      }
      return changesAsync;
    }

    public Task<DirectoryCoverageSummary> GetDirectoryCoverageSummaryAsync(
      DirectoryCoverageSummaryRequest directoryCoverageSummaryRequest,
      string project,
      int pipelineRunId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6e2f6ddf-76f0-4fa8-b3df-12130403fa14");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DirectoryCoverageSummaryRequest>(directoryCoverageSummaryRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (pipelineRunId), pipelineRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
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
      return this.SendAsync<DirectoryCoverageSummary>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<DirectoryCoverageSummary> GetDirectoryCoverageSummaryAsync(
      DirectoryCoverageSummaryRequest directoryCoverageSummaryRequest,
      Guid project,
      int pipelineRunId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6e2f6ddf-76f0-4fa8-b3df-12130403fa14");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DirectoryCoverageSummaryRequest>(directoryCoverageSummaryRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (pipelineRunId), pipelineRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
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
      return this.SendAsync<DirectoryCoverageSummary>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<FileCoverageDetails> GetFileCoverageDetailsAsync(
      FileCoverageDetailsRequest fileCoverageDetailsRequest,
      string project,
      int pipelineRunId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("195d4e37-6c72-4b74-94c3-733f4158e0ae");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<FileCoverageDetailsRequest>(fileCoverageDetailsRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (pipelineRunId), pipelineRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
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
      return this.SendAsync<FileCoverageDetails>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<FileCoverageDetails> GetFileCoverageDetailsAsync(
      FileCoverageDetailsRequest fileCoverageDetailsRequest,
      Guid project,
      int pipelineRunId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("195d4e37-6c72-4b74-94c3-733f4158e0ae");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<FileCoverageDetailsRequest>(fileCoverageDetailsRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (pipelineRunId), pipelineRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
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
      return this.SendAsync<FileCoverageDetails>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<List<string>> GetScopesAsync(
      string project,
      int pipelineRunId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("df578f76-d200-4a52-b5c7-16629b790bfd");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pipelineRunId), pipelineRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<string>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<string>> GetScopesAsync(
      Guid project,
      int pipelineRunId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("df578f76-d200-4a52-b5c7-16629b790bfd");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pipelineRunId), pipelineRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<string>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PipelineCoverageSummary> GetPipelineCoverageSummaryAsync(
      string project,
      int pipelineRunId,
      string scope = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e1a18b6a-b798-4e98-82e4-53b4b086390d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pipelineRunId), pipelineRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (scope != null)
        keyValuePairList.Add(nameof (scope), scope);
      return this.SendAsync<PipelineCoverageSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PipelineCoverageSummary> GetPipelineCoverageSummaryAsync(
      Guid project,
      int pipelineRunId,
      string scope = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e1a18b6a-b798-4e98-82e4-53b4b086390d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (pipelineRunId), pipelineRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (scope != null)
        keyValuePairList.Add(nameof (scope), scope);
      return this.SendAsync<PipelineCoverageSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
