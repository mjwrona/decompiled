// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.PipelinesHttpClientBase
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [ResourceArea("2e0bf237-8973-4ec9-a581-9c3d679d1776")]
  public abstract class PipelinesHttpClientBase : VssHttpClientBase
  {
    public PipelinesHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PipelinesHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PipelinesHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PipelinesHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PipelinesHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<Artifact> GetArtifactAsync(
      string project,
      int pipelineId,
      int runId,
      string artifactName,
      GetArtifactExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("85023071-bd5e-4438-89b0-2a5bf362a19d");
      object routeValues = (object) new
      {
        project = project,
        pipelineId = pipelineId,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactName), artifactName);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<Artifact>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Artifact> GetArtifactAsync(
      Guid project,
      int pipelineId,
      int runId,
      string artifactName,
      GetArtifactExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("85023071-bd5e-4438-89b0-2a5bf362a19d");
      object routeValues = (object) new
      {
        project = project,
        pipelineId = pipelineId,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactName), artifactName);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<Artifact>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Log> GetLogAsync(
      string project,
      int pipelineId,
      int runId,
      int logId,
      GetLogExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb1b6d27-3957-43d5-a14b-a2d70403e545");
      object routeValues = (object) new
      {
        project = project,
        pipelineId = pipelineId,
        runId = runId,
        logId = logId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<Log>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Log> GetLogAsync(
      Guid project,
      int pipelineId,
      int runId,
      int logId,
      GetLogExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb1b6d27-3957-43d5-a14b-a2d70403e545");
      object routeValues = (object) new
      {
        project = project,
        pipelineId = pipelineId,
        runId = runId,
        logId = logId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<Log>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<LogCollection> ListLogsAsync(
      string project,
      int pipelineId,
      int runId,
      GetLogExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb1b6d27-3957-43d5-a14b-a2d70403e545");
      object routeValues = (object) new
      {
        project = project,
        pipelineId = pipelineId,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<LogCollection>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<LogCollection> ListLogsAsync(
      Guid project,
      int pipelineId,
      int runId,
      GetLogExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb1b6d27-3957-43d5-a14b-a2d70403e545");
      object routeValues = (object) new
      {
        project = project,
        pipelineId = pipelineId,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<LogCollection>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Pipeline> CreatePipelineAsync(
      CreatePipelineParameters inputParameters,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("28e1305e-2afe-47bf-abaf-cbb0e6a91988");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CreatePipelineParameters>(inputParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Pipeline>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Pipeline> CreatePipelineAsync(
      CreatePipelineParameters inputParameters,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("28e1305e-2afe-47bf-abaf-cbb0e6a91988");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CreatePipelineParameters>(inputParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Pipeline>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Pipeline> GetPipelineAsync(
      string project,
      int pipelineId,
      int? pipelineVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("28e1305e-2afe-47bf-abaf-cbb0e6a91988");
      object routeValues = (object) new
      {
        project = project,
        pipelineId = pipelineId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (pipelineVersion.HasValue)
        keyValuePairList.Add(nameof (pipelineVersion), pipelineVersion.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<Pipeline>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Pipeline> GetPipelineAsync(
      Guid project,
      int pipelineId,
      int? pipelineVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("28e1305e-2afe-47bf-abaf-cbb0e6a91988");
      object routeValues = (object) new
      {
        project = project,
        pipelineId = pipelineId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (pipelineVersion.HasValue)
        keyValuePairList.Add(nameof (pipelineVersion), pipelineVersion.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<Pipeline>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Pipeline>> ListPipelinesAsync(
      string project,
      string orderBy = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("28e1305e-2afe-47bf-abaf-cbb0e6a91988");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (orderBy != null)
        keyValuePairList.Add(nameof (orderBy), orderBy);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<List<Pipeline>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Pipeline>> ListPipelinesAsync(
      Guid project,
      string orderBy = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("28e1305e-2afe-47bf-abaf-cbb0e6a91988");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (orderBy != null)
        keyValuePairList.Add(nameof (orderBy), orderBy);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<List<Pipeline>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PreviewRun> PreviewAsync(
      RunPipelineParameters runParameters,
      string project,
      int pipelineId,
      int? pipelineVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("53df2d18-29ea-46a9-bee0-933540f80abf");
      object obj1 = (object) new
      {
        project = project,
        pipelineId = pipelineId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<RunPipelineParameters>(runParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (pipelineVersion.HasValue)
        collection.Add(nameof (pipelineVersion), pipelineVersion.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
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
      return this.SendAsync<PreviewRun>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<PreviewRun> PreviewAsync(
      RunPipelineParameters runParameters,
      Guid project,
      int pipelineId,
      int? pipelineVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("53df2d18-29ea-46a9-bee0-933540f80abf");
      object obj1 = (object) new
      {
        project = project,
        pipelineId = pipelineId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<RunPipelineParameters>(runParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (pipelineVersion.HasValue)
        collection.Add(nameof (pipelineVersion), pipelineVersion.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
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
      return this.SendAsync<PreviewRun>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<Run> GetRunAsync(
      string project,
      int pipelineId,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Run>(new HttpMethod("GET"), new Guid("7859261e-d2e9-4a68-b820-a5d84cc5bb3d"), (object) new
      {
        project = project,
        pipelineId = pipelineId,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Run> GetRunAsync(
      Guid project,
      int pipelineId,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Run>(new HttpMethod("GET"), new Guid("7859261e-d2e9-4a68-b820-a5d84cc5bb3d"), (object) new
      {
        project = project,
        pipelineId = pipelineId,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Run>> ListRunsAsync(
      string project,
      int pipelineId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Run>>(new HttpMethod("GET"), new Guid("7859261e-d2e9-4a68-b820-a5d84cc5bb3d"), (object) new
      {
        project = project,
        pipelineId = pipelineId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Run>> ListRunsAsync(
      Guid project,
      int pipelineId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Run>>(new HttpMethod("GET"), new Guid("7859261e-d2e9-4a68-b820-a5d84cc5bb3d"), (object) new
      {
        project = project,
        pipelineId = pipelineId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Run> RunPipelineAsync(
      RunPipelineParameters runParameters,
      string project,
      int pipelineId,
      int? pipelineVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7859261e-d2e9-4a68-b820-a5d84cc5bb3d");
      object obj1 = (object) new
      {
        project = project,
        pipelineId = pipelineId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<RunPipelineParameters>(runParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (pipelineVersion.HasValue)
        collection.Add(nameof (pipelineVersion), pipelineVersion.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
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
      return this.SendAsync<Run>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<Run> RunPipelineAsync(
      RunPipelineParameters runParameters,
      Guid project,
      int pipelineId,
      int? pipelineVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7859261e-d2e9-4a68-b820-a5d84cc5bb3d");
      object obj1 = (object) new
      {
        project = project,
        pipelineId = pipelineId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<RunPipelineParameters>(runParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (pipelineVersion.HasValue)
        collection.Add(nameof (pipelineVersion), pipelineVersion.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
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
      return this.SendAsync<Run>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<SignalRConnection> GetSignedSignalRUrlAsync(
      string project,
      int pipelineId,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<SignalRConnection>(new HttpMethod("GET"), new Guid("1ffe4916-ac72-4566-add0-9bab31e44fcf"), (object) new
      {
        project = project,
        pipelineId = pipelineId,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<SignalRConnection> GetSignedSignalRUrlAsync(
      Guid project,
      int pipelineId,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<SignalRConnection>(new HttpMethod("GET"), new Guid("1ffe4916-ac72-4566-add0-9bab31e44fcf"), (object) new
      {
        project = project,
        pipelineId = pipelineId,
        runId = runId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
