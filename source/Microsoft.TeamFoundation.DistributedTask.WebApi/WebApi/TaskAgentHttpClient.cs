// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentHttpClient
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
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

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [ResourceArea("A85B8835-C1A1-4AAC-AE97-1C3D0BA72DBD")]
  public class TaskAgentHttpClient : TaskAgentHttpClientBase
  {
    private readonly ApiResourceVersion m_currentApiVersion = new ApiResourceVersion(3.0, 1);

    public TaskAgentHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TaskAgentHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TaskAgentHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TaskAgentHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TaskAgentHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<TaskAgentJobRequest> FinishAgentRequestAsync(
      int poolId,
      long requestId,
      Guid lockToken,
      DateTime finishTime,
      TaskResult result,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentJobRequest request = new TaskAgentJobRequest()
      {
        RequestId = requestId,
        FinishTime = new DateTime?(finishTime),
        Result = new TaskResult?(result)
      };
      return this.UpdateAgentRequestAsync(poolId, requestId, lockToken, request, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<TaskAgent>> GetAgentsAsync(
      int poolId,
      string agentName = null,
      bool? includeCapabilities = null,
      bool? includeAssignedRequest = null,
      bool? includeLastCompletedRequest = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<Demand> demands = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IEnumerable<string> demands1 = (IEnumerable<string>) null;
      if (demands != null)
        demands1 = demands.Select<Demand, string>((Func<Demand, string>) (d => d.ToString()));
      return this.GetAgentsAsync(poolId, agentName, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest, propertyFilters, demands1, userState, cancellationToken);
    }

    public virtual Task<SecureFile> GetSecureFileAsync(
      Guid project,
      Guid secureFileId,
      bool? includeDownloadTicket = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetSecureFileAsync(project, secureFileId, includeDownloadTicket, new SecureFileActionFilter?(), userState, cancellationToken);
    }

    public virtual Task<List<SecureFile>> GetSecureFilesAsync(
      string project,
      string namePattern = null,
      bool? includeDownloadTickets = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetSecureFilesAsync(project, namePattern, includeDownloadTickets, new SecureFileActionFilter?(), userState, cancellationToken);
    }

    public virtual Task<List<SecureFile>> GetSecureFilesAsync(
      Guid project,
      string namePattern = null,
      bool? includeDownloadTickets = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetSecureFilesAsync(project, namePattern, includeDownloadTickets, new SecureFileActionFilter?(), userState, cancellationToken);
    }

    public virtual Task<List<SecureFile>> GetSecureFilesByIdsAsync(
      string project,
      IEnumerable<Guid> secureFileIds,
      bool? includeDownloadTickets = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetSecureFilesByIdsAsync(project, secureFileIds, includeDownloadTickets, new SecureFileActionFilter?(), userState, cancellationToken);
    }

    public virtual Task<List<SecureFile>> GetSecureFilesByIdsAsync(
      Guid project,
      IEnumerable<Guid> secureFileIds,
      bool? includeDownloadTickets = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetSecureFilesByIdsAsync(project, secureFileIds, includeDownloadTickets, new SecureFileActionFilter?(), userState, cancellationToken);
    }

    public async Task<Stream> GetTaskContentZipAsync(
      Guid taskId,
      TaskVersion version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClient taskAgentHttpClient = this;
      var routeValues = new
      {
        taskId = taskId,
        versionString = version.ToString()
      };
      HttpRequestMessage message = await taskAgentHttpClient.CreateRequestMessageAsync(HttpMethod.Get, TaskResourceIds.Tasks, (object) routeValues, taskAgentHttpClient.m_currentApiVersion).ConfigureAwait(false);
      message.Headers.Accept.Clear();
      MediaTypeWithQualityHeaderValue qualityHeaderValue = new MediaTypeWithQualityHeaderValue("application/zip");
      qualityHeaderValue.Parameters.Add(new NameValueHeaderValue("api-version", taskAgentHttpClient.m_currentApiVersion.ApiVersionString));
      qualityHeaderValue.Parameters.Add(new NameValueHeaderValue("res-version", "1"));
      message.Headers.Accept.Add(qualityHeaderValue);
      HttpResponseMessage httpResponseMessage = await taskAgentHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
        throw new Exception("no content");
      if (!VssStringComparer.ContentType.Equals(httpResponseMessage.Content.Headers.ContentType.MediaType, "application/zip"))
        throw new Exception("bad content type");
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public Task<TaskAgentJobRequest> QueueAgentRequestByPoolAsync(
      int poolId,
      IList<Demand> demands,
      Guid serviceOwner,
      Guid hostId,
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid jobId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentJobRequest request = new TaskAgentJobRequest()
      {
        ServiceOwner = serviceOwner,
        HostId = hostId,
        PlanType = hubName,
        ScopeId = scopeIdentifier,
        PlanId = planId,
        JobId = jobId,
        Demands = demands
      };
      return this.QueueAgentRequestByPoolAsync(poolId, request, userState, cancellationToken);
    }

    public Task<TaskAgentJobRequest> RenewAgentRequestAsync(
      int poolId,
      long requestId,
      Guid lockToken,
      DateTime? expiresOn = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentJobRequest request = new TaskAgentJobRequest()
      {
        RequestId = requestId,
        LockedUntil = expiresOn
      };
      return this.UpdateAgentRequestAsync(poolId, requestId, lockToken, request, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<TaskAgent> ReplaceAgentAsync(
      int poolId,
      TaskAgent agent,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<TaskAgent>(agent, nameof (agent));
      return this.ReplaceAgentAsync(poolId, agent.Id, agent, userState, cancellationToken);
    }

    public Task SendMessageAsync(
      int poolId,
      long requestId,
      AgentJobRequestMessage request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentMessage message = new TaskAgentMessage()
      {
        Body = JsonUtility.ToString((object) request),
        MessageType = request.MessageType
      };
      return this.SendMessageAsync(poolId, requestId, message, userState, cancellationToken);
    }

    public Task SendMessageAsync(
      int poolId,
      long requestId,
      JobCancelMessage cancel,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentMessage message = new TaskAgentMessage()
      {
        Body = JsonUtility.ToString((object) cancel),
        MessageType = JobCancelMessage.MessageType
      };
      return this.SendMessageAsync(poolId, requestId, message, userState, cancellationToken);
    }

    public async Task<HttpResponseMessage> UploadTaskZipAsync(
      Guid taskId,
      Stream fileStream,
      bool overwrite = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClient taskAgentHttpClient = this;
      ArgumentUtility.CheckForNull<Stream>(fileStream, nameof (fileStream));
      if (fileStream.Length == 0L)
        throw new Exception("file stream of length 0 not allowed.");
      byte[] dataToSend = fileStream.Length <= 16777216L ? new byte[fileStream.Length] : throw new Exception("file stream too big");
      List<KeyValuePair<string, string>> keyValuePairList = (List<KeyValuePair<string, string>>) null;
      if (overwrite)
      {
        keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (overwrite), "true");
      }
      var routeValues = new{ taskId = taskId };
      HttpRequestMessage requestMessage = await taskAgentHttpClient.CreateRequestMessageAsync(HttpMethod.Put, TaskResourceIds.Tasks, (object) routeValues, taskAgentHttpClient.m_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      int bytesToCopy = (int) fileStream.Length;
      using (MemoryStream ms = new MemoryStream(dataToSend))
        await fileStream.CopyToAsync((Stream) ms, bytesToCopy, cancellationToken).ConfigureAwait(false);
      HttpContent httpContent = (HttpContent) new ByteArrayContent(dataToSend, 0, bytesToCopy);
      httpContent.Headers.ContentLength = new long?(fileStream.Length);
      httpContent.Headers.ContentRange = new ContentRangeHeaderValue(0L, fileStream.Length - 1L, fileStream.Length);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      requestMessage.Content = httpContent;
      HttpResponseMessage httpResponseMessage = await taskAgentHttpClient.SendAsync(requestMessage, userState, cancellationToken).ConfigureAwait(false);
      requestMessage = (HttpRequestMessage) null;
      dataToSend = (byte[]) null;
      return httpResponseMessage;
    }

    public virtual Task<IPagedList<DeploymentGroupMetrics>> GetDeploymentGroupsMetricsAsync2(
      string project,
      string deploymentGroupName = null,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("281c6308-427a-49e1-b83a-dac0f4862189");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(deploymentGroupName))
        keyValuePairList.Add(nameof (deploymentGroupName), deploymentGroupName);
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<IPagedList<DeploymentGroupMetrics>>(method, locationId, routeValues, new ApiResourceVersion("4.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<DeploymentGroupMetrics>>>(this.GetPagedList<DeploymentGroupMetrics>));
    }

    public virtual Task<IPagedList<DeploymentGroupMetrics>> GetDeploymentGroupsMetricsAsync2(
      Guid project,
      string deploymentGroupName = null,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("281c6308-427a-49e1-b83a-dac0f4862189");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(deploymentGroupName))
        keyValuePairList.Add(nameof (deploymentGroupName), deploymentGroupName);
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<IPagedList<DeploymentGroupMetrics>>(method, locationId, routeValues, new ApiResourceVersion("4.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<DeploymentGroupMetrics>>>(this.GetPagedList<DeploymentGroupMetrics>));
    }

    public virtual Task<IPagedList<DeploymentMachine>> GetDeploymentTargetsAsyncWithContinuationToken(
      string project,
      int deploymentGroupId,
      IEnumerable<string> tags = null,
      string name = null,
      bool? partialNameMatch = null,
      DeploymentTargetExpands? expand = null,
      TaskAgentStatusFilter? agentStatus = null,
      TaskAgentJobResultFilter? agentJobResult = null,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      bool? enabled = null,
      IEnumerable<string> propertyFilters = null)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (tags != null && tags.Any<string>())
        keyValuePairList.Add(nameof (tags), string.Join(",", tags));
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      bool flag;
      if (partialNameMatch.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = partialNameMatch.Value;
        string str = flag.ToString();
        collection.Add(nameof (partialNameMatch), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (agentStatus.HasValue)
        keyValuePairList.Add(nameof (agentStatus), agentStatus.Value.ToString());
      if (agentJobResult.HasValue)
        keyValuePairList.Add(nameof (agentJobResult), agentJobResult.Value.ToString());
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (enabled.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = enabled.Value;
        string str = flag.ToString();
        collection.Add(nameof (enabled), str);
      }
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<IPagedList<DeploymentMachine>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<DeploymentMachine>>>(this.GetPagedList<DeploymentMachine>));
    }

    public virtual Task<IPagedList<DeploymentMachine>> GetDeploymentTargetsAsyncWithContinuationToken(
      Guid project,
      int deploymentGroupId,
      IEnumerable<string> tags = null,
      string name = null,
      bool? partialNameMatch = null,
      DeploymentTargetExpands? expand = null,
      TaskAgentStatusFilter? agentStatus = null,
      TaskAgentJobResultFilter? agentJobResult = null,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      bool? enabled = null,
      IEnumerable<string> propertyFilters = null)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (tags != null && tags.Any<string>())
        keyValuePairList.Add(nameof (tags), string.Join(",", tags));
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      bool flag;
      if (partialNameMatch.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = partialNameMatch.Value;
        string str = flag.ToString();
        collection.Add(nameof (partialNameMatch), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (agentStatus.HasValue)
        keyValuePairList.Add(nameof (agentStatus), agentStatus.Value.ToString());
      if (agentJobResult.HasValue)
        keyValuePairList.Add(nameof (agentJobResult), agentJobResult.Value.ToString());
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (enabled.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = enabled.Value;
        string str = flag.ToString();
        collection.Add(nameof (enabled), str);
      }
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<IPagedList<DeploymentMachine>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<DeploymentMachine>>>(this.GetPagedList<DeploymentMachine>));
    }

    protected async Task<IPagedList<T>> GetPagedList<T>(
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      TaskAgentHttpClient taskAgentHttpClient = this;
      string continuationToken = taskAgentHttpClient.GetContinuationToken(responseMessage);
      IPagedList<T> pagedList = (IPagedList<T>) new PagedList<T>((IEnumerable<T>) await taskAgentHttpClient.ReadContentAsAsync<List<T>>(responseMessage, cancellationToken).ConfigureAwait(false), continuationToken);
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
      TaskAgentHttpClient taskAgentHttpClient = this;
      T obj;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await taskAgentHttpClient.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          obj = await taskAgentHttpClient.SendAsync<T>(requestMessage, userState, cancellationToken, processResponse).ConfigureAwait(false);
      }
      return obj;
    }

    protected async Task<T> SendAsync<T>(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      TaskAgentHttpClient taskAgentHttpClient = this;
      if (processResponse == null)
        processResponse = new Func<HttpResponseMessage, CancellationToken, Task<T>>(((VssHttpClientBase) taskAgentHttpClient).ReadContentAsAsync<T>);
      T obj;
      using (HttpResponseMessage response = await taskAgentHttpClient.SendAsync(message, userState, cancellationToken).ConfigureAwait(false))
        obj = await processResponse(response, cancellationToken).ConfigureAwait(false);
      return obj;
    }
  }
}
