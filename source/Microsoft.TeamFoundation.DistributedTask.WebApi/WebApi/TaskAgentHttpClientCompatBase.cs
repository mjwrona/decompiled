// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentHttpClientCompatBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [ResourceArea("A85B8835-C1A1-4AAC-AE97-1C3D0BA72DBD")]
  public abstract class TaskAgentHttpClientCompatBase : VssHttpClientBase
  {
    public TaskAgentHttpClientCompatBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TaskAgentHttpClientCompatBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TaskAgentHttpClientCompatBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TaskAgentHttpClientCompatBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TaskAgentHttpClientCompatBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task DeleteTaskGroupAsync(
      string project,
      Guid taskGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7"), (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      }, new ApiResourceVersion("4.0-preview.1"), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTaskGroupAsync(
      Guid project,
      Guid taskGroupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("6c08ffbf-dbf1-4f9a-94e5-a1cbd47005e7"), (object) new
      {
        project = project,
        taskGroupId = taskGroupId
      }, new ApiResourceVersion("4.0-preview.1"), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<TaskGroup>> GetTaskGroupsAsync(
      string project,
      Guid? taskGroupId = null,
      bool? expanded = null,
      Guid? taskIdFilter = null,
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
      if (expanded.HasValue)
        keyValuePairList.Add(nameof (expanded), expanded.Value.ToString());
      if (taskIdFilter.HasValue)
        keyValuePairList.Add(nameof (taskIdFilter), taskIdFilter.Value.ToString());
      return this.SendAsync<List<TaskGroup>>(method, locationId, routeValues, new ApiResourceVersion("4.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskGroup>> GetTaskGroupsAsync(
      Guid project,
      Guid? taskGroupId = null,
      bool? expanded = null,
      Guid? taskIdFilter = null,
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
      if (expanded.HasValue)
        keyValuePairList.Add(nameof (expanded), expanded.Value.ToString());
      if (taskIdFilter.HasValue)
        keyValuePairList.Add(nameof (taskIdFilter), taskIdFilter.Value.ToString());
      return this.SendAsync<List<TaskGroup>>(method, locationId, routeValues, new ApiResourceVersion("4.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskGroup>> GetTaskGroupsAsync(
      string project,
      Guid? taskGroupId = null,
      bool? expanded = null,
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
      if (expanded.HasValue)
        keyValuePairList.Add(nameof (expanded), expanded.Value.ToString());
      return this.SendAsync<List<TaskGroup>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskGroup>> GetTaskGroupsAsync(
      Guid project,
      Guid? taskGroupId = null,
      bool? expanded = null,
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
      if (expanded.HasValue)
        keyValuePairList.Add(nameof (expanded), expanded.Value.ToString());
      return this.SendAsync<List<TaskGroup>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TaskAgent> GetAgentAsync(
      int poolId,
      int agentId,
      bool? includeCapabilities,
      bool? includeAssignedRequest,
      IEnumerable<string> propertyFilters,
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
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<TaskAgent>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TaskAgent>> GetAgentsAsync(
      int poolId,
      string agentName,
      bool? includeCapabilities,
      bool? includeAssignedRequest,
      IEnumerable<string> propertyFilters,
      IEnumerable<string> demands,
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
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (demands != null && demands.Any<string>())
        keyValuePairList.Add(nameof (demands), string.Join(",", demands));
      return this.SendAsync<List<TaskAgent>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
