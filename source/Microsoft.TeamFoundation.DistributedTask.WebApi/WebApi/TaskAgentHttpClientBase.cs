// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentHttpClientBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [ResourceArea("A85B8835-C1A1-4AAC-AE97-1C3D0BA72DBD")]
  public abstract class TaskAgentHttpClientBase : TaskAgentHttpClientCompatBase
  {
    public TaskAgentHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TaskAgentHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TaskAgentHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TaskAgentHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TaskAgentHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<TaskAgentCloud> AddAgentCloudAsync(
      TaskAgentCloud agentCloud,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bfa72b3d-0fc6-43fb-932b-a7f6559f93b9");
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentCloud>(agentCloud, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentCloud>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskAgentCloud> DeleteAgentCloudAsync(
      int agentCloudId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TaskAgentCloud>(new HttpMethod("DELETE"), new Guid("bfa72b3d-0fc6-43fb-932b-a7f6559f93b9"), (object) new
      {
        agentCloudId = agentCloudId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskAgentCloud> GetAgentCloudAsync(
      int agentCloudId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TaskAgentCloud>(new HttpMethod("GET"), new Guid("bfa72b3d-0fc6-43fb-932b-a7f6559f93b9"), (object) new
      {
        agentCloudId = agentCloudId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentCloud>> GetAgentCloudsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TaskAgentCloud>>(new HttpMethod("GET"), new Guid("bfa72b3d-0fc6-43fb-932b-a7f6559f93b9"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskAgentCloud> UpdateAgentCloudAsync(
      int agentCloudId,
      TaskAgentCloud updatedCloud,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("bfa72b3d-0fc6-43fb-932b-a7f6559f93b9");
      object obj1 = (object) new
      {
        agentCloudId = agentCloudId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentCloud>(updatedCloud, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentCloud>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TaskAgentCloudType>> GetAgentCloudTypesAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TaskAgentCloudType>>(new HttpMethod("GET"), new Guid("5932e193-f376-469d-9c3e-e5588ce12cb5"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForQueueAsync(
      string project,
      int queueId,
      int top,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f5f81ffb-f396-498d-85b1-5ada145e648a");
      object routeValues = (object) new
      {
        project = project,
        queueId = queueId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$top", top.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForQueueAsync(
      Guid project,
      int queueId,
      int top,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f5f81ffb-f396-498d-85b1-5ada145e648a");
      object routeValues = (object) new
      {
        project = project,
        queueId = queueId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$top", top.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      string project,
      int queueId,
      TaskAgentJobRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f5f81ffb-f396-498d-85b1-5ada145e648a");
      object obj1 = (object) new
      {
        project = project,
        queueId = queueId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentJobRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentJobRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      Guid project,
      int queueId,
      TaskAgentJobRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f5f81ffb-f396-498d-85b1-5ada145e648a");
      object obj1 = (object) new
      {
        project = project,
        queueId = queueId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentJobRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentJobRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskAgent> AddAgentAsync(
      int poolId,
      TaskAgent agent,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e298ef32-5878-4cab-993c-043836571f42");
      object obj1 = (object) new{ poolId = poolId };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgent>(agent, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgent>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteAgentAsync(
      int poolId,
      int agentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("e298ef32-5878-4cab-993c-043836571f42"), (object) new
      {
        poolId = poolId,
        agentId = agentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TaskAgent> GetAgentAsync(
      int poolId,
      int agentId,
      bool? includeCapabilities = null,
      bool? includeAssignedRequest = null,
      bool? includeLastCompletedRequest = null,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e298ef32-5878-4cab-993c-043836571f42");
      object routeValues = (object) new
      {
        poolId = poolId,
        agentId = agentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeCapabilities.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeCapabilities.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeCapabilities), str);
      }
      if (includeAssignedRequest.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAssignedRequest.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAssignedRequest), str);
      }
      if (includeLastCompletedRequest.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLastCompletedRequest.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLastCompletedRequest), str);
      }
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<TaskAgent>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgent>> GetAgentsAsync(
      int poolId,
      string agentName = null,
      bool? includeCapabilities = null,
      bool? includeAssignedRequest = null,
      bool? includeLastCompletedRequest = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<string> demands = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e298ef32-5878-4cab-993c-043836571f42");
      object routeValues = (object) new{ poolId = poolId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (agentName != null)
        keyValuePairList.Add(nameof (agentName), agentName);
      bool flag;
      if (includeCapabilities.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeCapabilities.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeCapabilities), str);
      }
      if (includeAssignedRequest.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAssignedRequest.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAssignedRequest), str);
      }
      if (includeLastCompletedRequest.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLastCompletedRequest.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLastCompletedRequest), str);
      }
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (demands != null && demands.Any<string>())
        keyValuePairList.Add(nameof (demands), string.Join(",", demands));
      return this.SendAsync<List<TaskAgent>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskAgent> ReplaceAgentAsync(
      int poolId,
      int agentId,
      TaskAgent agent,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("e298ef32-5878-4cab-993c-043836571f42");
      object obj1 = (object) new
      {
        poolId = poolId,
        agentId = agentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgent>(agent, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgent>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskAgent> UpdateAgentAsync(
      int poolId,
      int agentId,
      TaskAgent agent,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e298ef32-5878-4cab-993c-043836571f42");
      object obj1 = (object) new
      {
        poolId = poolId,
        agentId = agentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgent>(agent, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgent>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<AzureManagementGroupQueryResult> GetAzureManagementGroupsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<AzureManagementGroupQueryResult>(new HttpMethod("GET"), new Guid("39fe3bf2-7ee0-4198-a469-4a29929afa9c"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<AzureSubscriptionQueryResult> GetAzureSubscriptionsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<AzureSubscriptionQueryResult>(new HttpMethod("GET"), new Guid("bcd6189c-0303-471f-a8e1-acb22b74d700"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<string> GenerateDeploymentGroupAccessTokenAsync(
      string project,
      int deploymentGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<string>(new HttpMethod("POST"), new Guid("3d197ba2-c3e9-4253-882f-0ee2440f8174"), (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<string> GenerateDeploymentGroupAccessTokenAsync(
      Guid project,
      int deploymentGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<string>(new HttpMethod("POST"), new Guid("3d197ba2-c3e9-4253-882f-0ee2440f8174"), (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DeploymentGroup> AddDeploymentGroupAsync(
      string project,
      DeploymentGroupCreateParameter deploymentGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("083c4d89-ab35-45af-aa11-7cf66895c53e");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentGroupCreateParameter>(deploymentGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DeploymentGroup> AddDeploymentGroupAsync(
      Guid project,
      DeploymentGroupCreateParameter deploymentGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("083c4d89-ab35-45af-aa11-7cf66895c53e");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentGroupCreateParameter>(deploymentGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteDeploymentGroupAsync(
      string project,
      int deploymentGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("083c4d89-ab35-45af-aa11-7cf66895c53e"), (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteDeploymentGroupAsync(
      Guid project,
      int deploymentGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("083c4d89-ab35-45af-aa11-7cf66895c53e"), (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<DeploymentGroup> GetDeploymentGroupAsync(
      string project,
      int deploymentGroupId,
      DeploymentGroupActionFilter? actionFilter = null,
      DeploymentGroupExpands? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("083c4d89-ab35-45af-aa11-7cf66895c53e");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<DeploymentGroup>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DeploymentGroup> GetDeploymentGroupAsync(
      Guid project,
      int deploymentGroupId,
      DeploymentGroupActionFilter? actionFilter = null,
      DeploymentGroupExpands? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("083c4d89-ab35-45af-aa11-7cf66895c53e");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<DeploymentGroup>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<DeploymentGroup>> GetDeploymentGroupsAsync(
      string project,
      string name = null,
      DeploymentGroupActionFilter? actionFilter = null,
      DeploymentGroupExpands? expand = null,
      string continuationToken = null,
      int? top = null,
      IEnumerable<int> ids = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("083c4d89-ab35-45af-aa11-7cf66895c53e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (ids != null && ids.Any<int>())
        keyValuePairList.Add(nameof (ids), string.Join<int>(",", ids));
      return this.SendAsync<List<DeploymentGroup>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<DeploymentGroup>> GetDeploymentGroupsAsync(
      Guid project,
      string name = null,
      DeploymentGroupActionFilter? actionFilter = null,
      DeploymentGroupExpands? expand = null,
      string continuationToken = null,
      int? top = null,
      IEnumerable<int> ids = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("083c4d89-ab35-45af-aa11-7cf66895c53e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (ids != null && ids.Any<int>())
        keyValuePairList.Add(nameof (ids), string.Join<int>(",", ids));
      return this.SendAsync<List<DeploymentGroup>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DeploymentGroup> UpdateDeploymentGroupAsync(
      string project,
      int deploymentGroupId,
      DeploymentGroupUpdateParameter deploymentGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("083c4d89-ab35-45af-aa11-7cf66895c53e");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentGroupUpdateParameter>(deploymentGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DeploymentGroup> UpdateDeploymentGroupAsync(
      Guid project,
      int deploymentGroupId,
      DeploymentGroupUpdateParameter deploymentGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("083c4d89-ab35-45af-aa11-7cf66895c53e");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentGroupUpdateParameter>(deploymentGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentGroupMetrics>> GetDeploymentGroupsMetricsAsync(
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
      if (deploymentGroupName != null)
        keyValuePairList.Add(nameof (deploymentGroupName), deploymentGroupName);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<DeploymentGroupMetrics>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentGroupMetrics>> GetDeploymentGroupsMetricsAsync(
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
      if (deploymentGroupName != null)
        keyValuePairList.Add(nameof (deploymentGroupName), deploymentGroupName);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<DeploymentGroupMetrics>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForDeploymentMachineAsync(
      string project,
      int deploymentGroupId,
      int machineId,
      int? completedRequestCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a3540e5b-f0dc-4668-963b-b752459be545");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (machineId), machineId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (completedRequestCount.HasValue)
        keyValuePairList.Add(nameof (completedRequestCount), completedRequestCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForDeploymentMachineAsync(
      Guid project,
      int deploymentGroupId,
      int machineId,
      int? completedRequestCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a3540e5b-f0dc-4668-963b-b752459be545");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (machineId), machineId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (completedRequestCount.HasValue)
        keyValuePairList.Add(nameof (completedRequestCount), completedRequestCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForDeploymentMachinesAsync(
      string project,
      int deploymentGroupId,
      IEnumerable<int> machineIds = null,
      int? completedRequestCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a3540e5b-f0dc-4668-963b-b752459be545");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (machineIds != null && machineIds.Any<int>())
        keyValuePairList.Add(nameof (machineIds), string.Join<int>(",", machineIds));
      if (completedRequestCount.HasValue)
        keyValuePairList.Add(nameof (completedRequestCount), completedRequestCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForDeploymentMachinesAsync(
      Guid project,
      int deploymentGroupId,
      IEnumerable<int> machineIds = null,
      int? completedRequestCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a3540e5b-f0dc-4668-963b-b752459be545");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (machineIds != null && machineIds.Any<int>())
        keyValuePairList.Add(nameof (machineIds), string.Join<int>(",", machineIds));
      if (completedRequestCount.HasValue)
        keyValuePairList.Add(nameof (completedRequestCount), completedRequestCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task RefreshDeploymentMachinesAsync(
      string project,
      int deploymentGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("91006ac4-0f68-4d82-a2bc-540676bd73ce"), (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task RefreshDeploymentMachinesAsync(
      Guid project,
      int deploymentGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("91006ac4-0f68-4d82-a2bc-540676bd73ce"), (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<string> GenerateDeploymentPoolAccessTokenAsync(
      int poolId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<string>(new HttpMethod("POST"), new Guid("e077ee4a-399b-420b-841f-c43fbc058e0b"), (object) new
      {
        poolId = poolId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentPoolSummary>> GetDeploymentPoolsSummaryAsync(
      string poolName = null,
      DeploymentPoolSummaryExpands? expands = null,
      IEnumerable<int> poolIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6525d6c6-258f-40e0-a1a9-8a24a3957625");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (poolName != null)
        keyValuePairList.Add(nameof (poolName), poolName);
      if (expands.HasValue)
        keyValuePairList.Add(nameof (expands), expands.Value.ToString());
      if (poolIds != null && poolIds.Any<int>())
        keyValuePairList.Add(nameof (poolIds), string.Join<int>(",", poolIds));
      return this.SendAsync<List<DeploymentPoolSummary>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForDeploymentTargetAsync(
      string project,
      int deploymentGroupId,
      int targetId,
      int? completedRequestCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2fac0be3-8c8f-4473-ab93-c1389b08a2c9");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (targetId), targetId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (completedRequestCount.HasValue)
        keyValuePairList.Add(nameof (completedRequestCount), completedRequestCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForDeploymentTargetAsync(
      Guid project,
      int deploymentGroupId,
      int targetId,
      int? completedRequestCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2fac0be3-8c8f-4473-ab93-c1389b08a2c9");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (targetId), targetId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (completedRequestCount.HasValue)
        keyValuePairList.Add(nameof (completedRequestCount), completedRequestCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForDeploymentTargetsAsync(
      string project,
      int deploymentGroupId,
      IEnumerable<int> targetIds = null,
      int? ownerId = null,
      DateTime? completedOn = null,
      int? completedRequestCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2fac0be3-8c8f-4473-ab93-c1389b08a2c9");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (targetIds != null && targetIds.Any<int>())
        keyValuePairList.Add(nameof (targetIds), string.Join<int>(",", targetIds));
      int num;
      if (ownerId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = ownerId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (ownerId), str);
      }
      if (completedOn.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (completedOn), completedOn.Value);
      if (completedRequestCount.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = completedRequestCount.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (completedRequestCount), str);
      }
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForDeploymentTargetsAsync(
      Guid project,
      int deploymentGroupId,
      IEnumerable<int> targetIds = null,
      int? ownerId = null,
      DateTime? completedOn = null,
      int? completedRequestCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2fac0be3-8c8f-4473-ab93-c1389b08a2c9");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (targetIds != null && targetIds.Any<int>())
        keyValuePairList.Add(nameof (targetIds), string.Join<int>(",", targetIds));
      int num;
      if (ownerId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = ownerId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (ownerId), str);
      }
      if (completedOn.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (completedOn), completedOn.Value);
      if (completedRequestCount.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = completedRequestCount.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (completedRequestCount), str);
      }
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task RefreshDeploymentTargetsAsync(
      string project,
      int deploymentGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("1c1a817f-f23d-41c6-bf8d-14b638f64152"), (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task RefreshDeploymentTargetsAsync(
      Guid project,
      int deploymentGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("1c1a817f-f23d-41c6-bf8d-14b638f64152"), (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> QueryEndpointAsync(
      TaskDefinitionEndpoint endpoint,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f223b809-8c33-4b7d-b53f-07232569b5d6");
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskDefinitionEndpoint>(endpoint, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<EnvironmentDeploymentExecutionRecord>> GetEnvironmentDeploymentExecutionRecordsAsync(
      string project,
      int environmentId,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("51bb5d21-4305-4ea6-9dbb-b7488af73334");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<EnvironmentDeploymentExecutionRecord>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<EnvironmentDeploymentExecutionRecord>> GetEnvironmentDeploymentExecutionRecordsAsync(
      Guid project,
      int environmentId,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("51bb5d21-4305-4ea6-9dbb-b7488af73334");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<EnvironmentDeploymentExecutionRecord>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<EnvironmentInstance> AddEnvironmentAsync(
      string project,
      EnvironmentCreateParameter environmentCreateParameter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8572b1fc-2482-47fa-8f74-7e3ed53ee54b");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<EnvironmentCreateParameter>(environmentCreateParameter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<EnvironmentInstance>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<EnvironmentInstance> AddEnvironmentAsync(
      Guid project,
      EnvironmentCreateParameter environmentCreateParameter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8572b1fc-2482-47fa-8f74-7e3ed53ee54b");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<EnvironmentCreateParameter>(environmentCreateParameter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<EnvironmentInstance>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteEnvironmentAsync(
      string project,
      int environmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("8572b1fc-2482-47fa-8f74-7e3ed53ee54b"), (object) new
      {
        project = project,
        environmentId = environmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteEnvironmentAsync(
      Guid project,
      int environmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("8572b1fc-2482-47fa-8f74-7e3ed53ee54b"), (object) new
      {
        project = project,
        environmentId = environmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<EnvironmentInstance> GetEnvironmentByIdAsync(
      string project,
      int environmentId,
      EnvironmentExpands? expands = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8572b1fc-2482-47fa-8f74-7e3ed53ee54b");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expands.HasValue)
        keyValuePairList.Add(nameof (expands), expands.Value.ToString());
      return this.SendAsync<EnvironmentInstance>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<EnvironmentInstance> GetEnvironmentByIdAsync(
      Guid project,
      int environmentId,
      EnvironmentExpands? expands = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8572b1fc-2482-47fa-8f74-7e3ed53ee54b");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expands.HasValue)
        keyValuePairList.Add(nameof (expands), expands.Value.ToString());
      return this.SendAsync<EnvironmentInstance>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<EnvironmentInstance>> GetEnvironmentsAsync(
      string project,
      string name = null,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8572b1fc-2482-47fa-8f74-7e3ed53ee54b");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<EnvironmentInstance>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<EnvironmentInstance>> GetEnvironmentsAsync(
      Guid project,
      string name = null,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8572b1fc-2482-47fa-8f74-7e3ed53ee54b");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<EnvironmentInstance>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<EnvironmentInstance> UpdateEnvironmentAsync(
      string project,
      int environmentId,
      EnvironmentUpdateParameter environmentUpdateParameter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("8572b1fc-2482-47fa-8f74-7e3ed53ee54b");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<EnvironmentUpdateParameter>(environmentUpdateParameter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<EnvironmentInstance>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<EnvironmentInstance> UpdateEnvironmentAsync(
      Guid project,
      int environmentId,
      EnvironmentUpdateParameter environmentUpdateParameter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("8572b1fc-2482-47fa-8f74-7e3ed53ee54b");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<EnvironmentUpdateParameter>(environmentUpdateParameter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<EnvironmentInstance>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskHubLicenseDetails> GetTaskHubLicenseDetailsAsync(
      string hubName,
      bool? includeEnterpriseUsersCount = null,
      bool? includeHostedAgentMinutesCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f9f0f436-b8a1-4475-9041-1ccdbf8f0128");
      object routeValues = (object) new{ hubName = hubName };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeEnterpriseUsersCount.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeEnterpriseUsersCount.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeEnterpriseUsersCount), str);
      }
      if (includeHostedAgentMinutesCount.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeHostedAgentMinutesCount.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeHostedAgentMinutesCount), str);
      }
      return this.SendAsync<TaskHubLicenseDetails>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskHubLicenseDetails> UpdateTaskHubLicenseDetailsAsync(
      string hubName,
      TaskHubLicenseDetails taskHubLicenseDetails,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("f9f0f436-b8a1-4475-9041-1ccdbf8f0128");
      object obj1 = (object) new{ hubName = hubName };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskHubLicenseDetails>(taskHubLicenseDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskHubLicenseDetails>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<StreamContent> GetTaskIconAsync(
      Guid taskId,
      string versionString,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<StreamContent>(new HttpMethod("GET"), new Guid("63463108-174d-49d4-b8cb-235eea42a5e1"), (object) new
      {
        taskId = taskId,
        versionString = versionString
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<InputValidationRequest> ValidateInputsAsync(
      InputValidationRequest inputValidationRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("58475b1e-adaf-4155-9bc1-e04bf1fff4c2");
      HttpContent httpContent = (HttpContent) new ObjectContent<InputValidationRequest>(inputValidationRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<InputValidationRequest>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteAgentRequestAsync(
      int poolId,
      long requestId,
      Guid lockToken,
      TaskResult? result = null,
      bool? agentShuttingDown = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("fc825784-c92a-4299-9221-998a02d1b54f");
      object routeValues = (object) new
      {
        poolId = poolId,
        requestId = requestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (lockToken), lockToken.ToString());
      if (result.HasValue)
        keyValuePairList.Add(nameof (result), result.Value.ToString());
      if (agentShuttingDown.HasValue)
        keyValuePairList.Add(nameof (agentShuttingDown), agentShuttingDown.Value.ToString());
      using (await agentHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentJobRequest> GetAgentRequestAsync(
      int poolId,
      long requestId,
      bool? includeStatus = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fc825784-c92a-4299-9221-998a02d1b54f");
      object routeValues = (object) new
      {
        poolId = poolId,
        requestId = requestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeStatus.HasValue)
        keyValuePairList.Add(nameof (includeStatus), includeStatus.Value.ToString());
      return this.SendAsync<TaskAgentJobRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsAsync(
      int poolId,
      int top,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fc825784-c92a-4299-9221-998a02d1b54f");
      object routeValues = (object) new{ poolId = poolId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$top", top.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForAgentAsync(
      int poolId,
      int agentId,
      int? completedRequestCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fc825784-c92a-4299-9221-998a02d1b54f");
      object routeValues = (object) new{ poolId = poolId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (agentId), agentId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (completedRequestCount.HasValue)
        keyValuePairList.Add(nameof (completedRequestCount), completedRequestCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForAgentsAsync(
      int poolId,
      IEnumerable<int> agentIds = null,
      int? completedRequestCount = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fc825784-c92a-4299-9221-998a02d1b54f");
      object routeValues = (object) new{ poolId = poolId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (agentIds != null && agentIds.Any<int>())
        keyValuePairList.Add(nameof (agentIds), string.Join<int>(",", agentIds));
      if (completedRequestCount.HasValue)
        keyValuePairList.Add(nameof (completedRequestCount), completedRequestCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentJobRequest>> GetAgentRequestsForPlanAsync(
      int poolId,
      Guid planId,
      Guid? jobId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fc825784-c92a-4299-9221-998a02d1b54f");
      object routeValues = (object) new{ poolId = poolId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (planId), planId.ToString());
      if (jobId.HasValue)
        keyValuePairList.Add(nameof (jobId), jobId.Value.ToString());
      return this.SendAsync<List<TaskAgentJobRequest>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentJobRequest> QueueAgentRequestByPoolAsync(
      int poolId,
      TaskAgentJobRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("fc825784-c92a-4299-9221-998a02d1b54f");
      object obj1 = (object) new{ poolId = poolId };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentJobRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentJobRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentJobRequest> UpdateAgentRequestAsync(
      int poolId,
      long requestId,
      Guid lockToken,
      TaskAgentJobRequest request,
      TaskAgentRequestUpdateOptions? updateOptions = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("fc825784-c92a-4299-9221-998a02d1b54f");
      object obj1 = (object) new
      {
        poolId = poolId,
        requestId = requestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentJobRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (lockToken), lockToken.ToString());
      if (updateOptions.HasValue)
        collection.Add(nameof (updateOptions), updateOptions.Value.ToString());
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
      return this.SendAsync<TaskAgentJobRequest>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<KubernetesResource> AddKubernetesResourceAsync(
      string project,
      int environmentId,
      KubernetesResourceCreateParameters createParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("73fba52f-15ab-42b3-a538-ce67a9223a04");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<KubernetesResourceCreateParameters>(createParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<KubernetesResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<KubernetesResource> AddKubernetesResourceAsync(
      Guid project,
      int environmentId,
      KubernetesResourceCreateParameters createParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("73fba52f-15ab-42b3-a538-ce67a9223a04");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<KubernetesResourceCreateParameters>(createParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<KubernetesResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteKubernetesResourceAsync(
      string project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("73fba52f-15ab-42b3-a538-ce67a9223a04"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteKubernetesResourceAsync(
      Guid project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("73fba52f-15ab-42b3-a538-ce67a9223a04"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<KubernetesResource> GetKubernetesResourceAsync(
      string project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<KubernetesResource>(new HttpMethod("GET"), new Guid("73fba52f-15ab-42b3-a538-ce67a9223a04"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<KubernetesResource> GetKubernetesResourceAsync(
      Guid project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<KubernetesResource>(new HttpMethod("GET"), new Guid("73fba52f-15ab-42b3-a538-ce67a9223a04"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<string> GenerateDeploymentMachineGroupAccessTokenAsync(
      string project,
      int machineGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<string>(new HttpMethod("POST"), new Guid("f8c7c0de-ac0d-469b-9cb1-c21f72d67693"), (object) new
      {
        project = project,
        machineGroupId = machineGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<string> GenerateDeploymentMachineGroupAccessTokenAsync(
      Guid project,
      int machineGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<string>(new HttpMethod("POST"), new Guid("f8c7c0de-ac0d-469b-9cb1-c21f72d67693"), (object) new
      {
        project = project,
        machineGroupId = machineGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachineGroup> AddDeploymentMachineGroupAsync(
      string project,
      DeploymentMachineGroup machineGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d4adf50f-80c6-4ac8-9ca1-6e4e544286e9");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachineGroup>(machineGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachineGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachineGroup> AddDeploymentMachineGroupAsync(
      Guid project,
      DeploymentMachineGroup machineGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d4adf50f-80c6-4ac8-9ca1-6e4e544286e9");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachineGroup>(machineGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachineGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteDeploymentMachineGroupAsync(
      string project,
      int machineGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("d4adf50f-80c6-4ac8-9ca1-6e4e544286e9"), (object) new
      {
        project = project,
        machineGroupId = machineGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteDeploymentMachineGroupAsync(
      Guid project,
      int machineGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("d4adf50f-80c6-4ac8-9ca1-6e4e544286e9"), (object) new
      {
        project = project,
        machineGroupId = machineGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachineGroup> GetDeploymentMachineGroupAsync(
      string project,
      int machineGroupId,
      MachineGroupActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d4adf50f-80c6-4ac8-9ca1-6e4e544286e9");
      object routeValues = (object) new
      {
        project = project,
        machineGroupId = machineGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<DeploymentMachineGroup>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachineGroup> GetDeploymentMachineGroupAsync(
      Guid project,
      int machineGroupId,
      MachineGroupActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d4adf50f-80c6-4ac8-9ca1-6e4e544286e9");
      object routeValues = (object) new
      {
        project = project,
        machineGroupId = machineGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<DeploymentMachineGroup>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentMachineGroup>> GetDeploymentMachineGroupsAsync(
      string project,
      string machineGroupName = null,
      MachineGroupActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d4adf50f-80c6-4ac8-9ca1-6e4e544286e9");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (machineGroupName != null)
        keyValuePairList.Add(nameof (machineGroupName), machineGroupName);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<DeploymentMachineGroup>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentMachineGroup>> GetDeploymentMachineGroupsAsync(
      Guid project,
      string machineGroupName = null,
      MachineGroupActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d4adf50f-80c6-4ac8-9ca1-6e4e544286e9");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (machineGroupName != null)
        keyValuePairList.Add(nameof (machineGroupName), machineGroupName);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<DeploymentMachineGroup>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachineGroup> UpdateDeploymentMachineGroupAsync(
      string project,
      int machineGroupId,
      DeploymentMachineGroup machineGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d4adf50f-80c6-4ac8-9ca1-6e4e544286e9");
      object obj1 = (object) new
      {
        project = project,
        machineGroupId = machineGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachineGroup>(machineGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachineGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachineGroup> UpdateDeploymentMachineGroupAsync(
      Guid project,
      int machineGroupId,
      DeploymentMachineGroup machineGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d4adf50f-80c6-4ac8-9ca1-6e4e544286e9");
      object obj1 = (object) new
      {
        project = project,
        machineGroupId = machineGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachineGroup>(machineGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachineGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentMachine>> GetDeploymentMachineGroupMachinesAsync(
      string project,
      int machineGroupId,
      IEnumerable<string> tagFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("966c3874-c347-4b18-a90c-d509116717fd");
      object routeValues = (object) new
      {
        project = project,
        machineGroupId = machineGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (tagFilters != null && tagFilters.Any<string>())
        keyValuePairList.Add(nameof (tagFilters), string.Join(",", tagFilters));
      return this.SendAsync<List<DeploymentMachine>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentMachine>> GetDeploymentMachineGroupMachinesAsync(
      Guid project,
      int machineGroupId,
      IEnumerable<string> tagFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("966c3874-c347-4b18-a90c-d509116717fd");
      object routeValues = (object) new
      {
        project = project,
        machineGroupId = machineGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (tagFilters != null && tagFilters.Any<string>())
        keyValuePairList.Add(nameof (tagFilters), string.Join(",", tagFilters));
      return this.SendAsync<List<DeploymentMachine>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentMachine>> UpdateDeploymentMachineGroupMachinesAsync(
      string project,
      int machineGroupId,
      IEnumerable<DeploymentMachine> deploymentMachines,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("966c3874-c347-4b18-a90c-d509116717fd");
      object obj1 = (object) new
      {
        project = project,
        machineGroupId = machineGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<DeploymentMachine>>(deploymentMachines, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<DeploymentMachine>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentMachine>> UpdateDeploymentMachineGroupMachinesAsync(
      Guid project,
      int machineGroupId,
      IEnumerable<DeploymentMachine> deploymentMachines,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("966c3874-c347-4b18-a90c-d509116717fd");
      object obj1 = (object) new
      {
        project = project,
        machineGroupId = machineGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<DeploymentMachine>>(deploymentMachines, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<DeploymentMachine>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> AddDeploymentMachineAsync(
      string project,
      int deploymentGroupId,
      DeploymentMachine machine,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6f6d406f-cfe6-409c-9327-7009928077e7");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachine>(machine, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> AddDeploymentMachineAsync(
      Guid project,
      int deploymentGroupId,
      DeploymentMachine machine,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6f6d406f-cfe6-409c-9327-7009928077e7");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachine>(machine, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteDeploymentMachineAsync(
      string project,
      int deploymentGroupId,
      int machineId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("6f6d406f-cfe6-409c-9327-7009928077e7"), (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        machineId = machineId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteDeploymentMachineAsync(
      Guid project,
      int deploymentGroupId,
      int machineId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("6f6d406f-cfe6-409c-9327-7009928077e7"), (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        machineId = machineId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> GetDeploymentMachineAsync(
      string project,
      int deploymentGroupId,
      int machineId,
      DeploymentMachineExpands? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6f6d406f-cfe6-409c-9327-7009928077e7");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        machineId = machineId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> GetDeploymentMachineAsync(
      Guid project,
      int deploymentGroupId,
      int machineId,
      DeploymentMachineExpands? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6f6d406f-cfe6-409c-9327-7009928077e7");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        machineId = machineId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentMachine>> GetDeploymentMachinesAsync(
      string project,
      int deploymentGroupId,
      IEnumerable<string> tags = null,
      string name = null,
      DeploymentMachineExpands? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6f6d406f-cfe6-409c-9327-7009928077e7");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (tags != null && tags.Any<string>())
        keyValuePairList.Add(nameof (tags), string.Join(",", tags));
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<DeploymentMachine>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentMachine>> GetDeploymentMachinesAsync(
      Guid project,
      int deploymentGroupId,
      IEnumerable<string> tags = null,
      string name = null,
      DeploymentMachineExpands? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6f6d406f-cfe6-409c-9327-7009928077e7");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (tags != null && tags.Any<string>())
        keyValuePairList.Add(nameof (tags), string.Join(",", tags));
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<DeploymentMachine>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> ReplaceDeploymentMachineAsync(
      string project,
      int deploymentGroupId,
      int machineId,
      DeploymentMachine machine,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("6f6d406f-cfe6-409c-9327-7009928077e7");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        machineId = machineId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachine>(machine, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> ReplaceDeploymentMachineAsync(
      Guid project,
      int deploymentGroupId,
      int machineId,
      DeploymentMachine machine,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("6f6d406f-cfe6-409c-9327-7009928077e7");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        machineId = machineId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachine>(machine, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> UpdateDeploymentMachineAsync(
      string project,
      int deploymentGroupId,
      int machineId,
      DeploymentMachine machine,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6f6d406f-cfe6-409c-9327-7009928077e7");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        machineId = machineId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachine>(machine, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> UpdateDeploymentMachineAsync(
      Guid project,
      int deploymentGroupId,
      int machineId,
      DeploymentMachine machine,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6f6d406f-cfe6-409c-9327-7009928077e7");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        machineId = machineId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachine>(machine, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentMachine>> UpdateDeploymentMachinesAsync(
      string project,
      int deploymentGroupId,
      IEnumerable<DeploymentMachine> machines,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6f6d406f-cfe6-409c-9327-7009928077e7");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<DeploymentMachine>>(machines, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<DeploymentMachine>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DeploymentMachine>> UpdateDeploymentMachinesAsync(
      Guid project,
      int deploymentGroupId,
      IEnumerable<DeploymentMachine> machines,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6f6d406f-cfe6-409c-9327-7009928077e7");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<DeploymentMachine>>(machines, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<DeploymentMachine>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentPoolMaintenanceDefinition> CreateAgentPoolMaintenanceDefinitionAsync(
      int poolId,
      TaskAgentPoolMaintenanceDefinition definition,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("80572e16-58f0-4419-ac07-d19fde32195c");
      object obj1 = (object) new{ poolId = poolId };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentPoolMaintenanceDefinition>(definition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentPoolMaintenanceDefinition>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteAgentPoolMaintenanceDefinitionAsync(
      int poolId,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("80572e16-58f0-4419-ac07-d19fde32195c"), (object) new
      {
        poolId = poolId,
        definitionId = definitionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentPoolMaintenanceDefinition> GetAgentPoolMaintenanceDefinitionAsync(
      int poolId,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TaskAgentPoolMaintenanceDefinition>(new HttpMethod("GET"), new Guid("80572e16-58f0-4419-ac07-d19fde32195c"), (object) new
      {
        poolId = poolId,
        definitionId = definitionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentPoolMaintenanceDefinition>> GetAgentPoolMaintenanceDefinitionsAsync(
      int poolId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TaskAgentPoolMaintenanceDefinition>>(new HttpMethod("GET"), new Guid("80572e16-58f0-4419-ac07-d19fde32195c"), (object) new
      {
        poolId = poolId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentPoolMaintenanceDefinition> UpdateAgentPoolMaintenanceDefinitionAsync(
      int poolId,
      int definitionId,
      TaskAgentPoolMaintenanceDefinition definition,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("80572e16-58f0-4419-ac07-d19fde32195c");
      object obj1 = (object) new
      {
        poolId = poolId,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentPoolMaintenanceDefinition>(definition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentPoolMaintenanceDefinition>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteAgentPoolMaintenanceJobAsync(
      int poolId,
      int jobId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("15e7ab6e-abce-4601-a6d8-e111fe148f46"), (object) new
      {
        poolId = poolId,
        jobId = jobId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentPoolMaintenanceJob> GetAgentPoolMaintenanceJobAsync(
      int poolId,
      int jobId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TaskAgentPoolMaintenanceJob>(new HttpMethod("GET"), new Guid("15e7ab6e-abce-4601-a6d8-e111fe148f46"), (object) new
      {
        poolId = poolId,
        jobId = jobId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetAgentPoolMaintenanceJobLogsAsync(
      int poolId,
      int jobId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("15e7ab6e-abce-4601-a6d8-e111fe148f46");
      object routeValues = (object) new
      {
        poolId = poolId,
        jobId = jobId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await agentHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await agentHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskAgentPoolMaintenanceJob>> GetAgentPoolMaintenanceJobsAsync(
      int poolId,
      int? definitionId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("15e7ab6e-abce-4601-a6d8-e111fe148f46");
      object routeValues = (object) new{ poolId = poolId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (definitionId.HasValue)
        keyValuePairList.Add(nameof (definitionId), definitionId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TaskAgentPoolMaintenanceJob>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentPoolMaintenanceJob> QueueAgentPoolMaintenanceJobAsync(
      int poolId,
      TaskAgentPoolMaintenanceJob job,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("15e7ab6e-abce-4601-a6d8-e111fe148f46");
      object obj1 = (object) new{ poolId = poolId };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentPoolMaintenanceJob>(job, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentPoolMaintenanceJob>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentPoolMaintenanceJob> UpdateAgentPoolMaintenanceJobAsync(
      int poolId,
      int jobId,
      TaskAgentPoolMaintenanceJob job,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("15e7ab6e-abce-4601-a6d8-e111fe148f46");
      object obj1 = (object) new
      {
        poolId = poolId,
        jobId = jobId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentPoolMaintenanceJob>(job, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentPoolMaintenanceJob>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteMessageAsync(
      int poolId,
      long messageId,
      Guid sessionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("c3a054f6-7a8a-49c0-944e-3a8e5d7adfd7");
      object routeValues = (object) new
      {
        poolId = poolId,
        messageId = messageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (sessionId), sessionId.ToString());
      using (await agentHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentMessage> GetMessageAsync(
      int poolId,
      Guid sessionId,
      long? lastMessageId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c3a054f6-7a8a-49c0-944e-3a8e5d7adfd7");
      object routeValues = (object) new{ poolId = poolId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (sessionId), sessionId.ToString());
      if (lastMessageId.HasValue)
        keyValuePairList.Add(nameof (lastMessageId), lastMessageId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TaskAgentMessage>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task RefreshAgentAsync(
      int poolId,
      int agentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("c3a054f6-7a8a-49c0-944e-3a8e5d7adfd7");
      object routeValues = (object) new{ poolId = poolId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (agentId), agentId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await agentHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task RefreshAgentsAsync(
      int poolId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("c3a054f6-7a8a-49c0-944e-3a8e5d7adfd7"), (object) new
      {
        poolId = poolId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task SendMessageAsync(
      int poolId,
      long requestId,
      TaskAgentMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c3a054f6-7a8a-49c0-944e-3a8e5d7adfd7");
      object obj1 = (object) new{ poolId = poolId };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentMessage>(message, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (requestId), requestId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      TaskAgentHttpClientBase agentHttpClientBase2 = agentHttpClientBase1;
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
      using (await agentHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<PackageMetadata> GetPackageAsync(
      string packageType,
      string platform,
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PackageMetadata>(new HttpMethod("GET"), new Guid("8ffcd551-079c-493a-9c02-54346299d144"), (object) new
      {
        packageType = packageType,
        platform = platform,
        version = version
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<PackageMetadata>> GetPackagesAsync(
      string packageType,
      string platform = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8ffcd551-079c-493a-9c02-54346299d144");
      object routeValues = (object) new
      {
        packageType = packageType,
        platform = platform
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<PackageMetadata>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetAgentPoolMetadataAsync(
      int poolId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0d62f887-9f53-48b9-9161-4c35d5735b0f");
      object routeValues = (object) new{ poolId = poolId };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await agentHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await agentHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task SetAgentPoolMetadataAsync(
      int poolId,
      Stream agentPoolMetadata,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("0d62f887-9f53-48b9-9161-4c35d5735b0f");
      object obj1 = (object) new{ poolId = poolId };
      HttpContent httpContent = (HttpContent) new ObjectContent<Stream>(agentPoolMetadata, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/octet-stream");
      TaskAgentHttpClientBase agentHttpClientBase2 = agentHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await agentHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<bool> HasPoolPermissionsAsync(
      int poolId,
      int permissions,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<bool>(new HttpMethod("GET"), new Guid("162778f3-4b48-48f3-9d58-436fb9c407bc"), (object) new
      {
        poolId = poolId,
        permissions = permissions
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskAgentPool> AddAgentPoolAsync(
      TaskAgentPool pool,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a8c47e17-4d56-4a56-92bb-de7ea7dc65be");
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentPool>(pool, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentPool>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteAgentPoolAsync(
      int poolId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("a8c47e17-4d56-4a56-92bb-de7ea7dc65be"), (object) new
      {
        poolId = poolId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TaskAgentPool> GetAgentPoolAsync(
      int poolId,
      IEnumerable<string> properties = null,
      TaskAgentPoolActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a8c47e17-4d56-4a56-92bb-de7ea7dc65be");
      object routeValues = (object) new{ poolId = poolId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (properties != null && properties.Any<string>())
        keyValuePairList.Add(nameof (properties), string.Join(",", properties));
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<TaskAgentPool>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentPool>> GetAgentPoolsAsync(
      string poolName = null,
      IEnumerable<string> properties = null,
      TaskAgentPoolType? poolType = null,
      TaskAgentPoolActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a8c47e17-4d56-4a56-92bb-de7ea7dc65be");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (poolName != null)
        keyValuePairList.Add(nameof (poolName), poolName);
      if (properties != null && properties.Any<string>())
        keyValuePairList.Add(nameof (properties), string.Join(",", properties));
      if (poolType.HasValue)
        keyValuePairList.Add(nameof (poolType), poolType.Value.ToString());
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentPool>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentPool>> GetAgentPoolsByIdsAsync(
      IEnumerable<int> poolIds,
      TaskAgentPoolActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a8c47e17-4d56-4a56-92bb-de7ea7dc65be");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (poolIds != null)
        str = string.Join<int>(",", poolIds);
      keyValuePairList.Add(nameof (poolIds), str);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentPool>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskAgentPool> UpdateAgentPoolAsync(
      int poolId,
      TaskAgentPool pool,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a8c47e17-4d56-4a56-92bb-de7ea7dc65be");
      object obj1 = (object) new{ poolId = poolId };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentPool>(pool, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentPool>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskAgentQueue> AddAgentQueueAsync(
      TaskAgentQueue queue,
      bool? authorizePipelines = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentQueue>(queue, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (authorizePipelines.HasValue)
        collection.Add(nameof (authorizePipelines), authorizePipelines.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentQueue>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskAgentQueue> AddAgentQueueAsync(
      string project,
      TaskAgentQueue queue,
      bool? authorizePipelines = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentQueue>(queue, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (authorizePipelines.HasValue)
        collection.Add(nameof (authorizePipelines), authorizePipelines.Value.ToString());
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
      return this.SendAsync<TaskAgentQueue>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<TaskAgentQueue> AddAgentQueueAsync(
      Guid project,
      TaskAgentQueue queue,
      bool? authorizePipelines = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentQueue>(queue, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (authorizePipelines.HasValue)
        collection.Add(nameof (authorizePipelines), authorizePipelines.Value.ToString());
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
      return this.SendAsync<TaskAgentQueue>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task CreateTeamProjectAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("PUT"), new Guid("900fa995-c559-4923-aae7-f8424fe4fbea"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task CreateTeamProjectAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("PUT"), new Guid("900fa995-c559-4923-aae7-f8424fe4fbea"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task CreateTeamProjectAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("PUT"), new Guid("900fa995-c559-4923-aae7-f8424fe4fbea"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteAgentQueueAsync(
      int queueId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("900fa995-c559-4923-aae7-f8424fe4fbea"), (object) new
      {
        queueId = queueId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteAgentQueueAsync(
      string project,
      int queueId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("900fa995-c559-4923-aae7-f8424fe4fbea"), (object) new
      {
        project = project,
        queueId = queueId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteAgentQueueAsync(
      Guid project,
      int queueId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("900fa995-c559-4923-aae7-f8424fe4fbea"), (object) new
      {
        project = project,
        queueId = queueId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TaskAgentQueue> GetAgentQueueAsync(
      string project,
      int queueId,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object routeValues = (object) new
      {
        project = project,
        queueId = queueId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<TaskAgentQueue>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskAgentQueue> GetAgentQueueAsync(
      Guid project,
      int queueId,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object routeValues = (object) new
      {
        project = project,
        queueId = queueId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<TaskAgentQueue>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskAgentQueue> GetAgentQueueAsync(
      int queueId,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object routeValues = (object) new{ queueId = queueId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<TaskAgentQueue>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentQueue>> GetAgentQueuesAsync(
      string project,
      string queueName = null,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (queueName != null)
        keyValuePairList.Add(nameof (queueName), queueName);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentQueue>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentQueue>> GetAgentQueuesAsync(
      Guid project,
      string queueName = null,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (queueName != null)
        keyValuePairList.Add(nameof (queueName), queueName);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentQueue>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentQueue>> GetAgentQueuesAsync(
      string queueName = null,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (queueName != null)
        keyValuePairList.Add(nameof (queueName), queueName);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentQueue>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentQueue>> GetAgentQueuesByIdsAsync(
      string project,
      IEnumerable<int> queueIds,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (queueIds != null)
        str = string.Join<int>(",", queueIds);
      keyValuePairList.Add(nameof (queueIds), str);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentQueue>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentQueue>> GetAgentQueuesByIdsAsync(
      Guid project,
      IEnumerable<int> queueIds,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (queueIds != null)
        str = string.Join<int>(",", queueIds);
      keyValuePairList.Add(nameof (queueIds), str);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentQueue>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentQueue>> GetAgentQueuesByIdsAsync(
      IEnumerable<int> queueIds,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (queueIds != null)
        str = string.Join<int>(",", queueIds);
      keyValuePairList.Add(nameof (queueIds), str);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentQueue>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentQueue>> GetAgentQueuesByNamesAsync(
      string project,
      IEnumerable<string> queueNames,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (queueNames != null)
        str = string.Join(",", queueNames);
      keyValuePairList.Add(nameof (queueNames), str);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentQueue>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentQueue>> GetAgentQueuesByNamesAsync(
      Guid project,
      IEnumerable<string> queueNames,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (queueNames != null)
        str = string.Join(",", queueNames);
      keyValuePairList.Add(nameof (queueNames), str);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentQueue>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentQueue>> GetAgentQueuesByNamesAsync(
      IEnumerable<string> queueNames,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (queueNames != null)
        str = string.Join(",", queueNames);
      keyValuePairList.Add(nameof (queueNames), str);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentQueue>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentQueue>> GetAgentQueuesForPoolsAsync(
      string project,
      IEnumerable<int> poolIds,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (poolIds != null)
        str = string.Join<int>(",", poolIds);
      keyValuePairList.Add(nameof (poolIds), str);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentQueue>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentQueue>> GetAgentQueuesForPoolsAsync(
      Guid project,
      IEnumerable<int> poolIds,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (poolIds != null)
        str = string.Join<int>(",", poolIds);
      keyValuePairList.Add(nameof (poolIds), str);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentQueue>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentQueue>> GetAgentQueuesForPoolsAsync(
      IEnumerable<int> poolIds,
      TaskAgentQueueActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("900fa995-c559-4923-aae7-f8424fe4fbea");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (poolIds != null)
        str = string.Join<int>(",", poolIds);
      keyValuePairList.Add(nameof (poolIds), str);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<TaskAgentQueue>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskAgentCloudRequest>> GetAgentCloudRequestsAsync(
      int agentCloudId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TaskAgentCloudRequest>>(new HttpMethod("GET"), new Guid("20189bd7-5134-49c2-b8e9-f9e856eea2b2"), (object) new
      {
        agentCloudId = agentCloudId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ResourceLimit>> GetResourceLimitsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ResourceLimit>>(new HttpMethod("GET"), new Guid("1f1f0557-c445-42a6-b4a0-0df605a3a0f8"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ResourceUsage> GetResourceUsageAsync(
      string parallelismTag = null,
      bool? poolIsHosted = null,
      bool? includeRunningRequests = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("eae1d376-a8b1-4475-9041-1dfdbe8f0143");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (parallelismTag != null)
        keyValuePairList.Add(nameof (parallelismTag), parallelismTag);
      bool flag;
      if (poolIsHosted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = poolIsHosted.Value;
        string str = flag.ToString();
        collection.Add(nameof (poolIsHosted), str);
      }
      if (includeRunningRequests.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeRunningRequests.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeRunningRequests), str);
      }
      return this.SendAsync<ResourceUsage>(method, locationId, version: new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskGroupRevision>> GetTaskGroupHistoryAsync(
      string project,
      Guid taskGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TaskGroupRevision>>(new HttpMethod("GET"), new Guid("100cc92a-b255-47fa-9ab3-e44a2985a3ac"), (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskGroupRevision>> GetTaskGroupHistoryAsync(
      Guid project,
      Guid taskGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TaskGroupRevision>>(new HttpMethod("GET"), new Guid("100cc92a-b255-47fa-9ab3-e44a2985a3ac"), (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteSecureFileAsync(
      string project,
      Guid secureFileId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421"), (object) new
      {
        project = project,
        secureFileId = secureFileId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteSecureFileAsync(
      Guid project,
      Guid secureFileId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421"), (object) new
      {
        project = project,
        secureFileId = secureFileId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> DownloadSecureFileAsync(
      string project,
      Guid secureFileId,
      string ticket,
      bool? download = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object routeValues = (object) new
      {
        project = project,
        secureFileId = secureFileId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (ticket), ticket);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await agentHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await agentHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> DownloadSecureFileAsync(
      Guid project,
      Guid secureFileId,
      string ticket,
      bool? download = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object routeValues = (object) new
      {
        project = project,
        secureFileId = secureFileId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (ticket), ticket);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await agentHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await agentHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<SecureFile> GetSecureFileAsync(
      string project,
      Guid secureFileId,
      bool? includeDownloadTicket = null,
      SecureFileActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object routeValues = (object) new
      {
        project = project,
        secureFileId = secureFileId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDownloadTicket.HasValue)
        keyValuePairList.Add(nameof (includeDownloadTicket), includeDownloadTicket.Value.ToString());
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<SecureFile>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<SecureFile> GetSecureFileAsync(
      Guid project,
      Guid secureFileId,
      bool? includeDownloadTicket = null,
      SecureFileActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object routeValues = (object) new
      {
        project = project,
        secureFileId = secureFileId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDownloadTicket.HasValue)
        keyValuePairList.Add(nameof (includeDownloadTicket), includeDownloadTicket.Value.ToString());
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<SecureFile>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<SecureFile>> GetSecureFilesAsync(
      string project,
      string namePattern = null,
      bool? includeDownloadTickets = null,
      SecureFileActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (namePattern != null)
        keyValuePairList.Add(nameof (namePattern), namePattern);
      if (includeDownloadTickets.HasValue)
        keyValuePairList.Add(nameof (includeDownloadTickets), includeDownloadTickets.Value.ToString());
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<SecureFile>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<SecureFile>> GetSecureFilesAsync(
      Guid project,
      string namePattern = null,
      bool? includeDownloadTickets = null,
      SecureFileActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (namePattern != null)
        keyValuePairList.Add(nameof (namePattern), namePattern);
      if (includeDownloadTickets.HasValue)
        keyValuePairList.Add(nameof (includeDownloadTickets), includeDownloadTickets.Value.ToString());
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<SecureFile>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<SecureFile>> GetSecureFilesByIdsAsync(
      string project,
      IEnumerable<Guid> secureFileIds,
      bool? includeDownloadTickets = null,
      SecureFileActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (secureFileIds != null)
        str = string.Join<Guid>(",", secureFileIds);
      keyValuePairList.Add(nameof (secureFileIds), str);
      if (includeDownloadTickets.HasValue)
        keyValuePairList.Add(nameof (includeDownloadTickets), includeDownloadTickets.Value.ToString());
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<SecureFile>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<SecureFile>> GetSecureFilesByIdsAsync(
      Guid project,
      IEnumerable<Guid> secureFileIds,
      bool? includeDownloadTickets = null,
      SecureFileActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (secureFileIds != null)
        str = string.Join<Guid>(",", secureFileIds);
      keyValuePairList.Add(nameof (secureFileIds), str);
      if (includeDownloadTickets.HasValue)
        keyValuePairList.Add(nameof (includeDownloadTickets), includeDownloadTickets.Value.ToString());
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<SecureFile>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<SecureFile>> GetSecureFilesByNamesAsync(
      string project,
      IEnumerable<string> secureFileNames,
      bool? includeDownloadTickets = null,
      SecureFileActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (secureFileNames != null)
        str = string.Join(",", secureFileNames);
      keyValuePairList.Add(nameof (secureFileNames), str);
      if (includeDownloadTickets.HasValue)
        keyValuePairList.Add(nameof (includeDownloadTickets), includeDownloadTickets.Value.ToString());
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<SecureFile>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<SecureFile>> GetSecureFilesByNamesAsync(
      Guid project,
      IEnumerable<string> secureFileNames,
      bool? includeDownloadTickets = null,
      SecureFileActionFilter? actionFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (secureFileNames != null)
        str = string.Join(",", secureFileNames);
      keyValuePairList.Add(nameof (secureFileNames), str);
      if (includeDownloadTickets.HasValue)
        keyValuePairList.Add(nameof (includeDownloadTickets), includeDownloadTickets.Value.ToString());
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      return this.SendAsync<List<SecureFile>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<SecureFile>> QuerySecureFilesByPropertiesAsync(
      string project,
      string condition,
      string namePattern = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<string>(condition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (namePattern != null)
        collection.Add(nameof (namePattern), namePattern);
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
      return this.SendAsync<List<SecureFile>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<SecureFile>> QuerySecureFilesByPropertiesAsync(
      Guid project,
      string condition,
      string namePattern = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<string>(condition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (namePattern != null)
        collection.Add(nameof (namePattern), namePattern);
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
      return this.SendAsync<List<SecureFile>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<SecureFile> UpdateSecureFileAsync(
      string project,
      Guid secureFileId,
      SecureFile secureFile,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object obj1 = (object) new
      {
        project = project,
        secureFileId = secureFileId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<SecureFile>(secureFile, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<SecureFile>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<SecureFile> UpdateSecureFileAsync(
      Guid project,
      Guid secureFileId,
      SecureFile secureFile,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object obj1 = (object) new
      {
        project = project,
        secureFileId = secureFileId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<SecureFile>(secureFile, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<SecureFile>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<SecureFile>> UpdateSecureFilesAsync(
      string project,
      IEnumerable<SecureFile> secureFiles,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<SecureFile>>(secureFiles, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<SecureFile>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<SecureFile>> UpdateSecureFilesAsync(
      Guid project,
      IEnumerable<SecureFile> secureFiles,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<SecureFile>>(secureFiles, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<SecureFile>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<SecureFile> UploadSecureFileAsync(
      string project,
      Stream uploadStream,
      string name,
      bool? authorizePipelines = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (name), name);
      if (authorizePipelines.HasValue)
        collection.Add(nameof (authorizePipelines), authorizePipelines.Value.ToString());
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
      return this.SendAsync<SecureFile>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<SecureFile> UploadSecureFileAsync(
      Guid project,
      Stream uploadStream,
      string name,
      bool? authorizePipelines = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("adcfd8bc-b184-43ba-bd84-7c8c6a2ff421");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (name), name);
      if (authorizePipelines.HasValue)
        collection.Add(nameof (authorizePipelines), authorizePipelines.Value.ToString());
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
      return this.SendAsync<SecureFile>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentSession> CreateAgentSessionAsync(
      int poolId,
      TaskAgentSession session,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("134e239e-2df3-4794-a6f6-24f1f19ec8dc");
      object obj1 = (object) new{ poolId = poolId };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskAgentSession>(session, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgentSession>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteAgentSessionAsync(
      int poolId,
      Guid sessionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("134e239e-2df3-4794-a6f6-24f1f19ec8dc"), (object) new
      {
        poolId = poolId,
        sessionId = sessionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> AddDeploymentTargetAsync(
      string project,
      int deploymentGroupId,
      DeploymentMachine machine,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachine>(machine, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> AddDeploymentTargetAsync(
      Guid project,
      int deploymentGroupId,
      DeploymentMachine machine,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachine>(machine, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteDeploymentTargetAsync(
      string project,
      int deploymentGroupId,
      int targetId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6"), (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        targetId = targetId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteDeploymentTargetAsync(
      Guid project,
      int deploymentGroupId,
      int targetId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6"), (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        targetId = targetId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<DeploymentMachine> GetDeploymentTargetAsync(
      string project,
      int deploymentGroupId,
      int targetId,
      DeploymentTargetExpands? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        targetId = targetId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DeploymentMachine> GetDeploymentTargetAsync(
      Guid project,
      int deploymentGroupId,
      int targetId,
      DeploymentTargetExpands? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6");
      object routeValues = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        targetId = targetId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<DeploymentMachine>> GetDeploymentTargetsAsync(
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
      bool? enabled = null,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
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
      if (name != null)
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
      if (continuationToken != null)
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
      return this.SendAsync<List<DeploymentMachine>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<DeploymentMachine>> GetDeploymentTargetsAsync(
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
      bool? enabled = null,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
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
      if (name != null)
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
      if (continuationToken != null)
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
      return this.SendAsync<List<DeploymentMachine>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> ReplaceDeploymentTargetAsync(
      string project,
      int deploymentGroupId,
      int targetId,
      DeploymentMachine machine,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        targetId = targetId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachine>(machine, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> ReplaceDeploymentTargetAsync(
      Guid project,
      int deploymentGroupId,
      int targetId,
      DeploymentMachine machine,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        targetId = targetId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachine>(machine, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> UpdateDeploymentTargetAsync(
      string project,
      int deploymentGroupId,
      int targetId,
      DeploymentMachine machine,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        targetId = targetId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachine>(machine, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DeploymentMachine> UpdateDeploymentTargetAsync(
      Guid project,
      int deploymentGroupId,
      int targetId,
      DeploymentMachine machine,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId,
        targetId = targetId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentMachine>(machine, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentMachine>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<DeploymentMachine>> UpdateDeploymentTargetsAsync(
      string project,
      int deploymentGroupId,
      IEnumerable<DeploymentTargetUpdateParameter> machines,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<DeploymentTargetUpdateParameter>>(machines, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<DeploymentMachine>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<DeploymentMachine>> UpdateDeploymentTargetsAsync(
      Guid project,
      int deploymentGroupId,
      IEnumerable<DeploymentTargetUpdateParameter> machines,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2f0aa599-c121-4256-a5fd-ba370e0ae7b6");
      object obj1 = (object) new
      {
        project = project,
        deploymentGroupId = deploymentGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<DeploymentTargetUpdateParameter>>(machines, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<DeploymentMachine>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskGroup> AddTaskGroupAsync(
      string project,
      TaskGroupCreateParameter taskGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskGroupCreateParameter>(taskGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskGroup> AddTaskGroupAsync(
      Guid project,
      TaskGroupCreateParameter taskGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskGroupCreateParameter>(taskGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTaskGroupAsync(
      string project,
      Guid taskGroupId,
      string comment = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object routeValues = (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      using (await agentHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTaskGroupAsync(
      Guid project,
      Guid taskGroupId,
      string comment = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object routeValues = (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      using (await agentHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskGroup> GetTaskGroupAsync(
      string project,
      Guid taskGroupId,
      string versionSpec,
      TaskGroupExpands? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object routeValues = (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (versionSpec), versionSpec);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<TaskGroup>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskGroup> GetTaskGroupAsync(
      Guid project,
      Guid taskGroupId,
      string versionSpec,
      TaskGroupExpands? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object routeValues = (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (versionSpec), versionSpec);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<TaskGroup>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetTaskGroupRevisionAsync(
      string project,
      Guid taskGroupId,
      int revision,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object routeValues = (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (revision), revision.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await agentHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await agentHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetTaskGroupRevisionAsync(
      Guid project,
      Guid taskGroupId,
      int revision,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object routeValues = (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (revision), revision.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await agentHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await agentHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<TaskGroup>> GetTaskGroupsAsync(
      string project,
      Guid? taskGroupId = null,
      bool? expanded = null,
      Guid? taskIdFilter = null,
      bool? deleted = null,
      int? top = null,
      DateTime? continuationToken = null,
      TaskGroupQueryOrder? queryOrder = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object routeValues = (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (expanded.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = expanded.Value;
        string str = flag.ToString();
        collection.Add(nameof (expanded), str);
      }
      if (taskIdFilter.HasValue)
        keyValuePairList.Add(nameof (taskIdFilter), taskIdFilter.Value.ToString());
      if (deleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = deleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (deleted), str);
      }
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (continuationToken), continuationToken.Value);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      return this.SendAsync<List<TaskGroup>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskGroup>> GetTaskGroupsAsync(
      Guid project,
      Guid? taskGroupId = null,
      bool? expanded = null,
      Guid? taskIdFilter = null,
      bool? deleted = null,
      int? top = null,
      DateTime? continuationToken = null,
      TaskGroupQueryOrder? queryOrder = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object routeValues = (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (expanded.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = expanded.Value;
        string str = flag.ToString();
        collection.Add(nameof (expanded), str);
      }
      if (taskIdFilter.HasValue)
        keyValuePairList.Add(nameof (taskIdFilter), taskIdFilter.Value.ToString());
      if (deleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = deleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (deleted), str);
      }
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (continuationToken), continuationToken.Value);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      return this.SendAsync<List<TaskGroup>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskGroup>> PublishTaskGroupAsync(
      string project,
      Guid parentTaskGroupId,
      PublishTaskGroupMetadata taskGroupMetadata,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<PublishTaskGroupMetadata>(taskGroupMetadata, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (parentTaskGroupId), parentTaskGroupId.ToString());
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
      return this.SendAsync<List<TaskGroup>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskGroup>> PublishTaskGroupAsync(
      Guid project,
      Guid parentTaskGroupId,
      PublishTaskGroupMetadata taskGroupMetadata,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<PublishTaskGroupMetadata>(taskGroupMetadata, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (parentTaskGroupId), parentTaskGroupId.ToString());
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
      return this.SendAsync<List<TaskGroup>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskGroup>> UndeleteTaskGroupAsync(
      string project,
      TaskGroup taskGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskGroup>(taskGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TaskGroup>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskGroup>> UndeleteTaskGroupAsync(
      Guid project,
      TaskGroup taskGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskGroup>(taskGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TaskGroup>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use UpdateTaskGroup(Guid taskGroupId, [FromBody] TaskGroupUpdateParameter taskGroup) instead")]
    public virtual Task<TaskGroup> UpdateTaskGroupAsync(
      string project,
      TaskGroupUpdateParameter taskGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskGroupUpdateParameter>(taskGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use UpdateTaskGroup(Guid taskGroupId, [FromBody] TaskGroupUpdateParameter taskGroup) instead")]
    public virtual Task<TaskGroup> UpdateTaskGroupAsync(
      Guid project,
      TaskGroupUpdateParameter taskGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskGroupUpdateParameter>(taskGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskGroup> UpdateTaskGroupAsync(
      string project,
      Guid taskGroupId,
      TaskGroupUpdateParameter taskGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object obj1 = (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskGroupUpdateParameter>(taskGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskGroup> UpdateTaskGroupAsync(
      Guid project,
      Guid taskGroupId,
      TaskGroupUpdateParameter taskGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object obj1 = (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskGroupUpdateParameter>(taskGroup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskGroup>> UpdateTaskGroupPropertiesAsync(
      string project,
      Guid taskGroupId,
      TaskGroupUpdatePropertiesBase taskGroupUpdateProperties,
      bool? disablePriorVersions = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object obj1 = (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskGroupUpdatePropertiesBase>(taskGroupUpdateProperties, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (disablePriorVersions.HasValue)
        collection.Add(nameof (disablePriorVersions), disablePriorVersions.Value.ToString());
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
      return this.SendAsync<List<TaskGroup>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskGroup>> UpdateTaskGroupPropertiesAsync(
      Guid project,
      Guid taskGroupId,
      TaskGroupUpdatePropertiesBase taskGroupUpdateProperties,
      bool? disablePriorVersions = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7");
      object obj1 = (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskGroupUpdatePropertiesBase>(taskGroupUpdateProperties, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (disablePriorVersions.HasValue)
        collection.Add(nameof (disablePriorVersions), disablePriorVersions.Value.ToString());
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
      return this.SendAsync<List<TaskGroup>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteTaskDefinitionAsync(
      Guid taskId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("60aac929-f0cd-4bc8-9ce4-6b30e8f1b1bd"), (object) new
      {
        taskId = taskId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetTaskContentZipAsync(
      Guid taskId,
      string versionString,
      IEnumerable<string> visibility = null,
      bool? scopeLocal = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("60aac929-f0cd-4bc8-9ce4-6b30e8f1b1bd");
      object routeValues = (object) new
      {
        taskId = taskId,
        versionString = versionString
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (visibility != null)
        agentHttpClientBase.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (visibility), (object) visibility);
      if (scopeLocal.HasValue)
        keyValuePairList.Add(nameof (scopeLocal), scopeLocal.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await agentHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await agentHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskDefinition> GetTaskDefinitionAsync(
      Guid taskId,
      string versionString,
      IEnumerable<string> visibility = null,
      bool? scopeLocal = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("60aac929-f0cd-4bc8-9ce4-6b30e8f1b1bd");
      object routeValues = (object) new
      {
        taskId = taskId,
        versionString = versionString
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (visibility != null)
        this.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (visibility), (object) visibility);
      if (scopeLocal.HasValue)
        keyValuePairList.Add(nameof (scopeLocal), scopeLocal.Value.ToString());
      return this.SendAsync<TaskDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskDefinition>> GetTaskDefinitionsAsync(
      Guid? taskId = null,
      IEnumerable<string> visibility = null,
      bool? scopeLocal = null,
      bool? allVersions = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("60aac929-f0cd-4bc8-9ce4-6b30e8f1b1bd");
      object routeValues = (object) new{ taskId = taskId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (visibility != null)
        this.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (visibility), (object) visibility);
      bool flag;
      if (scopeLocal.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = scopeLocal.Value;
        string str = flag.ToString();
        collection.Add(nameof (scopeLocal), str);
      }
      if (allVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = allVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (allVersions), str);
      }
      return this.SendAsync<List<TaskDefinition>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgent> UpdateAgentUpdateStateAsync(
      int poolId,
      int agentId,
      string currentState,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("8cc1b02b-ae49-4516-b5ad-4f9b29967c30");
      object routeValues = (object) new
      {
        poolId = poolId,
        agentId = agentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (currentState), currentState);
      return this.SendAsync<TaskAgent>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgent> UpdateAgentUserCapabilitiesAsync(
      int poolId,
      int agentId,
      IDictionary<string, string> userCapabilities,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("30ba3ada-fedf-4da8-bbb5-dacf2f82e176");
      object obj1 = (object) new
      {
        poolId = poolId,
        agentId = agentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IDictionary<string, string>>(userCapabilities, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAgent>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<VariableGroup> AddVariableGroupAsync(
      VariableGroupParameters variableGroupParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ef5b7057-ffc3-4c77-bbad-c10b4a4abcc7");
      HttpContent httpContent = (HttpContent) new ObjectContent<VariableGroupParameters>(variableGroupParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VariableGroup>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteVariableGroupAsync(
      int groupId,
      IEnumerable<string> projectIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("ef5b7057-ffc3-4c77-bbad-c10b4a4abcc7");
      object routeValues = (object) new{ groupId = groupId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (projectIds != null)
        str = string.Join(",", projectIds);
      keyValuePairList.Add(nameof (projectIds), str);
      using (await agentHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task ShareVariableGroupAsync(
      int variableGroupId,
      IEnumerable<VariableGroupProjectReference> variableGroupProjectReferences,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskAgentHttpClientBase agentHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ef5b7057-ffc3-4c77-bbad-c10b4a4abcc7");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<VariableGroupProjectReference>>(variableGroupProjectReferences, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (variableGroupId), variableGroupId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      TaskAgentHttpClientBase agentHttpClientBase2 = agentHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await agentHttpClientBase2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<VariableGroup> UpdateVariableGroupAsync(
      int groupId,
      VariableGroupParameters variableGroupParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("ef5b7057-ffc3-4c77-bbad-c10b4a4abcc7");
      object obj1 = (object) new{ groupId = groupId };
      HttpContent httpContent = (HttpContent) new ObjectContent<VariableGroupParameters>(variableGroupParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VariableGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<VariableGroup> GetVariableGroupAsync(
      string project,
      int groupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<VariableGroup>(new HttpMethod("GET"), new Guid("f5b09dd5-9d54-45a1-8b5a-1c8287d634cc"), (object) new
      {
        project = project,
        groupId = groupId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<VariableGroup> GetVariableGroupAsync(
      Guid project,
      int groupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<VariableGroup>(new HttpMethod("GET"), new Guid("f5b09dd5-9d54-45a1-8b5a-1c8287d634cc"), (object) new
      {
        project = project,
        groupId = groupId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<VariableGroup>> GetVariableGroupsAsync(
      string project,
      string groupName = null,
      VariableGroupActionFilter? actionFilter = null,
      int? top = null,
      int? continuationToken = null,
      VariableGroupQueryOrder? queryOrder = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f5b09dd5-9d54-45a1-8b5a-1c8287d634cc");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (groupName != null)
        keyValuePairList.Add(nameof (groupName), groupName);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      return this.SendAsync<List<VariableGroup>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<VariableGroup>> GetVariableGroupsAsync(
      Guid project,
      string groupName = null,
      VariableGroupActionFilter? actionFilter = null,
      int? top = null,
      int? continuationToken = null,
      VariableGroupQueryOrder? queryOrder = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f5b09dd5-9d54-45a1-8b5a-1c8287d634cc");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (groupName != null)
        keyValuePairList.Add(nameof (groupName), groupName);
      if (actionFilter.HasValue)
        keyValuePairList.Add(nameof (actionFilter), actionFilter.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      return this.SendAsync<List<VariableGroup>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<VariableGroup>> GetVariableGroupsByIdAsync(
      string project,
      IEnumerable<int> groupIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f5b09dd5-9d54-45a1-8b5a-1c8287d634cc");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (groupIds != null)
        str = string.Join<int>(",", groupIds);
      keyValuePairList.Add(nameof (groupIds), str);
      return this.SendAsync<List<VariableGroup>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<VariableGroup>> GetVariableGroupsByIdAsync(
      Guid project,
      IEnumerable<int> groupIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f5b09dd5-9d54-45a1-8b5a-1c8287d634cc");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (groupIds != null)
        str = string.Join<int>(",", groupIds);
      keyValuePairList.Add(nameof (groupIds), str);
      return this.SendAsync<List<VariableGroup>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<VirtualMachineGroup> AddVirtualMachineGroupAsync(
      string project,
      int environmentId,
      VirtualMachineGroupCreateParameters createParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9e597901-4af7-4cc3-8d92-47d54db8ebfb");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineGroupCreateParameters>(createParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<VirtualMachineGroup> AddVirtualMachineGroupAsync(
      Guid project,
      int environmentId,
      VirtualMachineGroupCreateParameters createParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9e597901-4af7-4cc3-8d92-47d54db8ebfb");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineGroupCreateParameters>(createParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task DeleteVirtualMachineGroupAsync(
      string project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9e597901-4af7-4cc3-8d92-47d54db8ebfb"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task DeleteVirtualMachineGroupAsync(
      Guid project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9e597901-4af7-4cc3-8d92-47d54db8ebfb"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<VirtualMachineGroup> GetVirtualMachineGroupAsync(
      string project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<VirtualMachineGroup>(new HttpMethod("GET"), new Guid("9e597901-4af7-4cc3-8d92-47d54db8ebfb"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<VirtualMachineGroup> GetVirtualMachineGroupAsync(
      Guid project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<VirtualMachineGroup>(new HttpMethod("GET"), new Guid("9e597901-4af7-4cc3-8d92-47d54db8ebfb"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<VirtualMachineGroup> UpdateVirtualMachineGroupAsync(
      string project,
      int environmentId,
      VirtualMachineGroup resource,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9e597901-4af7-4cc3-8d92-47d54db8ebfb");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineGroup>(resource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<VirtualMachineGroup> UpdateVirtualMachineGroupAsync(
      Guid project,
      int environmentId,
      VirtualMachineGroup resource,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9e597901-4af7-4cc3-8d92-47d54db8ebfb");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineGroup>(resource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<VirtualMachine>> GetVirtualMachinesAsync(
      string project,
      int environmentId,
      int resourceId,
      string continuationToken = null,
      string name = null,
      bool? partialNameMatch = null,
      IEnumerable<string> tags = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("48700676-2ba5-4282-8ec8-083280d169c7");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (partialNameMatch.HasValue)
        keyValuePairList.Add(nameof (partialNameMatch), partialNameMatch.Value.ToString());
      if (tags != null && tags.Any<string>())
        keyValuePairList.Add(nameof (tags), string.Join(",", tags));
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<VirtualMachine>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<VirtualMachine>> GetVirtualMachinesAsync(
      Guid project,
      int environmentId,
      int resourceId,
      string continuationToken = null,
      string name = null,
      bool? partialNameMatch = null,
      IEnumerable<string> tags = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("48700676-2ba5-4282-8ec8-083280d169c7");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (partialNameMatch.HasValue)
        keyValuePairList.Add(nameof (partialNameMatch), partialNameMatch.Value.ToString());
      if (tags != null && tags.Any<string>())
        keyValuePairList.Add(nameof (tags), string.Join(",", tags));
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<VirtualMachine>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<VirtualMachine>> UpdateVirtualMachinesAsync(
      string project,
      int environmentId,
      int resourceId,
      IEnumerable<VirtualMachine> machines,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("48700676-2ba5-4282-8ec8-083280d169c7");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<VirtualMachine>>(machines, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<VirtualMachine>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<VirtualMachine>> UpdateVirtualMachinesAsync(
      Guid project,
      int environmentId,
      int resourceId,
      IEnumerable<VirtualMachine> machines,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("48700676-2ba5-4282-8ec8-083280d169c7");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<VirtualMachine>>(machines, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<VirtualMachine>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    public virtual Task<string> CreateAadOAuthRequestAsync(
      string tenantId,
      string redirectUri,
      AadLoginPromptOption? promptOption = null,
      string completeCallbackPayload = null,
      bool? completeCallbackByAuthCode = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("9c63205e-3a0f-42a0-ad88-095200f13607");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (tenantId), tenantId);
      keyValuePairList.Add(nameof (redirectUri), redirectUri);
      if (promptOption.HasValue)
        keyValuePairList.Add(nameof (promptOption), promptOption.Value.ToString());
      if (completeCallbackPayload != null)
        keyValuePairList.Add(nameof (completeCallbackPayload), completeCallbackPayload);
      if (completeCallbackByAuthCode.HasValue)
        keyValuePairList.Add(nameof (completeCallbackByAuthCode), completeCallbackByAuthCode.Value.ToString());
      return this.SendAsync<string>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<string> GetVstsAadTenantIdAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<string>(new HttpMethod("GET"), new Guid("9c63205e-3a0f-42a0-ad88-095200f13607"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<object> GetYamlSchemaAsync(
      bool? validateTaskNames = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1f9990b9-1dba-441f-9c2e-6485888c42b6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (validateTaskNames.HasValue)
        keyValuePairList.Add(nameof (validateTaskNames), validateTaskNames.Value.ToString());
      return this.SendAsync<object>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
