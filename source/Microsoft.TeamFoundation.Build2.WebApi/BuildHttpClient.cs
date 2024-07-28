// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildHttpClient
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [ReactiveClientToThrottling(true)]
  public class BuildHttpClient : BuildHttpClientBase
  {
    private static readonly ApiResourceVersion s_ChangesApiVersion = new ApiResourceVersion("4.1-preview.2");
    private static readonly Guid s_getBuildsLocationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");

    public BuildHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public BuildHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public BuildHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public BuildHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public BuildHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<BuildDefinition> CreateDefinitionAsync(
      BuildDefinition definition,
      int? definitionToCloneId = null,
      int? definitionToCloneRevision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckForNull<TeamProjectReference>(definition.Project, "definition.Project");
      return this.CreateDefinitionAsync(definition, definition.Project.Id, definitionToCloneId, definitionToCloneRevision, userState, cancellationToken);
    }

    public Task<BuildDefinition> UpdateDefinitionAsync(
      BuildDefinition definition,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.UpdateDefinitionAsync(definition, definition.Project.Id, definition.Id, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> QueueBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      bool? ignoreWarnings = null,
      string checkInTicket = null,
      int? sourceBuildId = null,
      int? definitionId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.Build>(build, nameof (build));
      ArgumentUtility.CheckForNull<TeamProjectReference>(build.Project, "build.Project");
      return this.QueueBuildAsync(build, build.Project.Id, ignoreWarnings, checkInTicket, sourceBuildId, definitionId, userState, cancellationToken);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> UpdateBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      bool? retry = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.Build>(build, nameof (build));
      ArgumentUtility.CheckForNull<TeamProjectReference>(build.Project, "build.Project");
      return this.UpdateBuildAsync(build, build.Project.Id, build.Id, retry, userState, cancellationToken);
    }

    public virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      string project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetDefinitionsAsync(project, name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(false), includeLatestBuilds, taskIdFilter, processType, yamlFilename, userState, cancellationToken);
    }

    public virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      Guid project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetDefinitionsAsync(project, name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(false), includeLatestBuilds, taskIdFilter, processType, yamlFilename, userState, cancellationToken);
    }

    public virtual Task<IPagedList<BuildDefinitionReference>> GetDefinitionsAsync2(
      string project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(false), includeLatestBuilds, taskIdFilter, processType, yamlFilename);
      return this.SendAsync<IPagedList<BuildDefinitionReference>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<BuildDefinitionReference>>>(((BuildHttpClientCompatBase) this).GetPagedList<BuildDefinitionReference>));
    }

    public virtual Task<IPagedList<BuildDefinitionReference>> GetDefinitionsAsync2(
      Guid project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(false), includeLatestBuilds, taskIdFilter, processType, yamlFilename);
      return this.SendAsync<IPagedList<BuildDefinitionReference>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<BuildDefinitionReference>>>(((BuildHttpClientCompatBase) this).GetPagedList<BuildDefinitionReference>));
    }

    [Obsolete("Use GetDefinitionsAsync2(string) instead.")]
    public virtual Task<IPagedList<BuildDefinitionReference>> GetDefinitionsAsync2(
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(false), includeLatestBuilds, taskIdFilter, processType, yamlFilename);
      return this.SendAsync<IPagedList<BuildDefinitionReference>>(method, locationId, version: BuildHttpClientCompatBase.s_DefinitionsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<BuildDefinitionReference>>>(((BuildHttpClientCompatBase) this).GetPagedList<BuildDefinitionReference>));
    }

    public virtual Task<List<BuildDefinition>> GetFullDefinitionsAsync(
      string project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(true), includeLatestBuilds, taskIdFilter, processType, yamlFilename);
      return this.SendAsync<List<BuildDefinition>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinition>>>) null);
    }

    public virtual Task<List<BuildDefinition>> GetFullDefinitionsAsync(
      Guid project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(true), includeLatestBuilds, taskIdFilter, processType, yamlFilename);
      return this.SendAsync<List<BuildDefinition>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinition>>>) null);
    }

    [Obsolete("Use GetFullDefinitionsAsync(string) instead.")]
    public virtual Task<List<BuildDefinition>> GetFullDefinitionsAsync(
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(true), includeLatestBuilds, taskIdFilter, processType, yamlFilename);
      return this.SendAsync<List<BuildDefinition>>(method, locationId, (object) null, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinition>>>) null);
    }

    public virtual Task<IPagedList<BuildDefinition>> GetFullDefinitionsAsync2(
      string project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(true), includeLatestBuilds, taskIdFilter, processType, yamlFilename);
      return this.SendAsync<IPagedList<BuildDefinition>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<BuildDefinition>>>(((BuildHttpClientCompatBase) this).GetPagedList<BuildDefinition>));
    }

    public virtual Task<IPagedList<BuildDefinition>> GetFullDefinitionsAsync2(
      Guid project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(true), includeLatestBuilds, taskIdFilter, processType, yamlFilename);
      return this.SendAsync<IPagedList<BuildDefinition>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<BuildDefinition>>>(((BuildHttpClientCompatBase) this).GetPagedList<BuildDefinition>));
    }

    [Obsolete("Use GetFullDefinitionsAsync2(string) instead.")]
    public virtual Task<IPagedList<BuildDefinition>> GetFullDefinitionsAsync2(
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(true), includeLatestBuilds, taskIdFilter, processType, yamlFilename);
      return this.SendAsync<IPagedList<BuildDefinition>>(method, locationId, version: BuildHttpClientCompatBase.s_DefinitionsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<BuildDefinition>>>(((BuildHttpClientCompatBase) this).GetPagedList<BuildDefinition>));
    }

    public override Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      Guid project,
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      IEnumerable<string> properties = null,
      int? top = null,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption? deletedFilter = null,
      BuildQueryOrder? queryOrder = null,
      string branchName = null,
      IEnumerable<int> buildIds = null,
      string repositoryId = null,
      string repositoryType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> buildsQueryParams = this.GetBuildsQueryParams(definitions, queues, buildNumber, minFinishTime, maxFinishTime, requestedFor, reasonFilter, statusFilter, resultFilter, tagFilters, properties, top, continuationToken, maxBuildsPerDefinition, deletedFilter, queryOrder, branchName, buildIds, repositoryId, repositoryType, userState, cancellationToken);
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, BuildHttpClient.s_getBuildsLocationId, routeValues, BuildHttpClientCompatBase.s_BuildsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) buildsQueryParams, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) null);
    }

    public override Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      string project,
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      IEnumerable<string> properties = null,
      int? top = null,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption? deletedFilter = null,
      BuildQueryOrder? queryOrder = null,
      string branchName = null,
      IEnumerable<int> buildIds = null,
      string repositoryId = null,
      string repositoryType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> buildsQueryParams = this.GetBuildsQueryParams(definitions, queues, buildNumber, minFinishTime, maxFinishTime, requestedFor, reasonFilter, statusFilter, resultFilter, tagFilters, properties, top, continuationToken, maxBuildsPerDefinition, deletedFilter, queryOrder, branchName, buildIds, repositoryId, repositoryType, userState, cancellationToken);
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, BuildHttpClient.s_getBuildsLocationId, routeValues, BuildHttpClientCompatBase.s_BuildsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) buildsQueryParams, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) null);
    }

    public virtual Task<IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync2(
      Guid project,
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      IEnumerable<string> properties = null,
      int? top = null,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption? deletedFilter = null,
      BuildQueryOrder? queryOrder = null,
      string branchName = null,
      IEnumerable<int> buildIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> buildsQueryParams = this.GetBuildsQueryParams(definitions, queues, buildNumber, minFinishTime, maxFinishTime, requestedFor, reasonFilter, statusFilter, resultFilter, tagFilters, properties, top, continuationToken, maxBuildsPerDefinition, deletedFilter, queryOrder, branchName, buildIds, userState: userState, cancellationToken: cancellationToken);
      return this.SendAsync<IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, BuildHttpClient.s_getBuildsLocationId, routeValues, BuildHttpClientCompatBase.s_BuildsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) buildsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>>>(((BuildHttpClientCompatBase) this).GetPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>));
    }

    public virtual Task<IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync2(
      string project,
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      IEnumerable<string> properties = null,
      int? top = null,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption? deletedFilter = null,
      BuildQueryOrder? queryOrder = null,
      string branchName = null,
      IEnumerable<int> buildIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> buildsQueryParams = this.GetBuildsQueryParams(definitions, queues, buildNumber, minFinishTime, maxFinishTime, requestedFor, reasonFilter, statusFilter, resultFilter, tagFilters, properties, top, continuationToken, maxBuildsPerDefinition, deletedFilter, queryOrder, branchName, buildIds, userState: userState, cancellationToken: cancellationToken);
      return this.SendAsync<IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, BuildHttpClient.s_getBuildsLocationId, routeValues, BuildHttpClientCompatBase.s_BuildsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) buildsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>>>(((BuildHttpClientCompatBase) this).GetPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>));
    }

    public virtual Task<IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync2(
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      IEnumerable<string> properties = null,
      int? top = null,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption? deletedFilter = null,
      BuildQueryOrder? queryOrder = null,
      string branchName = null,
      IEnumerable<int> buildIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      List<KeyValuePair<string, string>> buildsQueryParams = this.GetBuildsQueryParams(definitions, queues, buildNumber, minFinishTime, maxFinishTime, requestedFor, reasonFilter, statusFilter, resultFilter, tagFilters, properties, top, continuationToken, maxBuildsPerDefinition, deletedFilter, queryOrder, branchName, buildIds, userState: userState, cancellationToken: cancellationToken);
      return this.SendAsync<IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, BuildHttpClient.s_getBuildsLocationId, version: BuildHttpClientCompatBase.s_BuildsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) buildsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>>>(((BuildHttpClientCompatBase) this).GetPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>));
    }

    public virtual Task<IPagedList<Change>> GetBuildChangesAsync2(
      string project,
      int buildId,
      string continuationToken = null,
      int? top = null,
      bool? includeSourceChange = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("54572c7b-bbd3-45d4-80dc-28be08941620");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeSourceChange.HasValue)
        keyValuePairList.Add(nameof (includeSourceChange), includeSourceChange.Value.ToString());
      return this.SendAsync<IPagedList<Change>>(method, locationId, routeValues, BuildHttpClient.s_ChangesApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<Change>>>(((BuildHttpClientCompatBase) this).GetPagedList<Change>));
    }

    public virtual Task<IPagedList<Change>> GetBuildChangesAsync2(
      Guid project,
      int buildId,
      string continuationToken = null,
      int? top = null,
      bool? includeSourceChange = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("54572c7b-bbd3-45d4-80dc-28be08941620");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeSourceChange.HasValue)
        keyValuePairList.Add(nameof (includeSourceChange), includeSourceChange.Value.ToString());
      return this.SendAsync<IPagedList<Change>>(method, locationId, routeValues, BuildHttpClient.s_ChangesApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<Change>>>(((BuildHttpClientCompatBase) this).GetPagedList<Change>));
    }

    public virtual async Task<PropertiesCollection> UpdateDefinitionPropertiesAsync(
      PropertiesCollection properties,
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClient buildHttpClient = this;
      return await buildHttpClient.UpdateDefinitionPropertiesAsync(buildHttpClient.CreateJsonPatchDocument(properties, Operation.Replace), project, definitionId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> UpdateDefinitionPropertiesAsync(
      PropertiesCollection properties,
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClient buildHttpClient = this;
      return await buildHttpClient.UpdateDefinitionPropertiesAsync(buildHttpClient.CreateJsonPatchDocument(properties, Operation.Replace), project, definitionId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> UpdateBuildPropertiesAsync(
      PropertiesCollection properties,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClient buildHttpClient = this;
      return await buildHttpClient.UpdateBuildPropertiesAsync(buildHttpClient.CreateJsonPatchDocument(properties, Operation.Replace), project, buildId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> UpdateBuildPropertiesAsync(
      PropertiesCollection properties,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClient buildHttpClient = this;
      return await buildHttpClient.UpdateBuildPropertiesAsync(buildHttpClient.CreateJsonPatchDocument(properties, Operation.Replace), project, buildId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> RemoveDefinitionPropertiesAsync(
      PropertiesCollection properties,
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClient buildHttpClient = this;
      return await buildHttpClient.UpdateDefinitionPropertiesAsync(buildHttpClient.CreateJsonPatchDocument(properties, Operation.Remove), project, definitionId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> RemoveDefinitionPropertiesAsync(
      PropertiesCollection properties,
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClient buildHttpClient = this;
      return await buildHttpClient.UpdateDefinitionPropertiesAsync(buildHttpClient.CreateJsonPatchDocument(properties, Operation.Remove), project, definitionId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> RemoveBuildPropertiesAsync(
      PropertiesCollection properties,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClient buildHttpClient = this;
      return await buildHttpClient.UpdateBuildPropertiesAsync(buildHttpClient.CreateJsonPatchDocument(properties, Operation.Remove), project, buildId, userState, cancellationToken);
    }

    public virtual async Task<PropertiesCollection> RemoveBuildPropertiesAsync(
      PropertiesCollection properties,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClient buildHttpClient = this;
      return await buildHttpClient.UpdateBuildPropertiesAsync(buildHttpClient.CreateJsonPatchDocument(properties, Operation.Remove), project, buildId, userState, cancellationToken);
    }

    private List<KeyValuePair<string, string>> GetBuildsQueryParams(
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minTime = null,
      DateTime? maxTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      IEnumerable<string> properties = null,
      int? top = null,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption? deletedFilter = null,
      BuildQueryOrder? queryOrder = null,
      string branchName = null,
      IEnumerable<int> buildIds = null,
      string repositoryId = null,
      string repositoryType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      List<KeyValuePair<string, string>> buildsQueryParams = new List<KeyValuePair<string, string>>();
      bool flag = true;
      ApiResourceLocation result = this.GetResourceLocationAsync(BuildHttpClient.s_getBuildsLocationId, userState, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
      if (result != null && result.MaxVersion >= BuildHttpClientCompatBase.s_BuildsApiVersion.ApiVersion)
        flag = false;
      buildsQueryParams.Add("type", "Build");
      if (definitions != null && definitions.Any<int>())
        buildsQueryParams.Add(nameof (definitions), string.Join<int>(",", definitions));
      if (queues != null && queues.Any<int>())
        buildsQueryParams.Add(nameof (queues), string.Join<int>(",", queues));
      if (!string.IsNullOrEmpty(buildNumber))
        buildsQueryParams.Add(nameof (buildNumber), buildNumber);
      if (flag)
      {
        if (minTime.HasValue)
          this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) buildsQueryParams, "minFinishTime", minTime.Value);
        if (maxTime.HasValue)
          this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) buildsQueryParams, "maxFinishTime", maxTime.Value);
      }
      else
      {
        if (minTime.HasValue)
          this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) buildsQueryParams, nameof (minTime), minTime.Value);
        if (maxTime.HasValue)
          this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) buildsQueryParams, nameof (maxTime), maxTime.Value);
      }
      if (!string.IsNullOrEmpty(requestedFor))
        buildsQueryParams.Add(nameof (requestedFor), requestedFor);
      if (reasonFilter.HasValue)
        buildsQueryParams.Add(nameof (reasonFilter), reasonFilter.Value.ToString());
      if (statusFilter.HasValue)
        buildsQueryParams.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (resultFilter.HasValue)
        buildsQueryParams.Add(nameof (resultFilter), resultFilter.Value.ToString());
      if (tagFilters != null && tagFilters.Any<string>())
        buildsQueryParams.Add(nameof (tagFilters), string.Join(",", tagFilters));
      if (properties != null && properties.Any<string>())
        buildsQueryParams.Add(nameof (properties), string.Join(",", properties));
      if (top.HasValue)
        buildsQueryParams.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        buildsQueryParams.Add(nameof (continuationToken), continuationToken);
      if (maxBuildsPerDefinition.HasValue)
        buildsQueryParams.Add(nameof (maxBuildsPerDefinition), maxBuildsPerDefinition.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (deletedFilter.HasValue)
        buildsQueryParams.Add(nameof (deletedFilter), deletedFilter.Value.ToString());
      if (queryOrder.HasValue)
        buildsQueryParams.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (!string.IsNullOrEmpty(branchName))
        buildsQueryParams.Add(nameof (branchName), branchName);
      if (buildIds != null && buildIds.Any<int>())
        buildsQueryParams.Add(nameof (buildIds), string.Join<int>(",", buildIds));
      if (!string.IsNullOrEmpty(repositoryId))
        buildsQueryParams.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        buildsQueryParams.Add(nameof (repositoryType), repositoryType);
      return buildsQueryParams;
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
