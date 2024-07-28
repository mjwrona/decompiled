// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ProjectHttpClient
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [ResourceArea("79134C72-4A58-4B42-976C-04E7115F32BF")]
  public class ProjectHttpClient : ProjectHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions;
    private static readonly ApiResourceVersion s_resourceVersion = new ApiResourceVersion("2.0-preview.3");

    internal HttpRequestHeaders DefaultRequestHeaders => this.Client.DefaultRequestHeaders;

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) ProjectHttpClient.s_translatedExceptions;

    static ProjectHttpClient() => ProjectHttpClient.s_translatedExceptions = new Dictionary<string, Type>();

    public ProjectHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ProjectHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ProjectHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ProjectHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ProjectHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<TeamProject> GetProject(
      string id,
      bool? includeCapabilities = null,
      bool includeHistory = false,
      object userState = null)
    {
      List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
      QueryParamHelper.AddNonNullParam(queryParams, nameof (includeCapabilities), (object) includeCapabilities);
      QueryParamHelper.AddNonNullParam(queryParams, nameof (includeHistory), (object) includeHistory);
      Guid projectsLocationId = CoreConstants.ProjectsLocationId;
      ApiResourceVersion resourceVersion = ProjectHttpClient.s_resourceVersion;
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) queryParams;
      var routeValues = new{ projectId = id };
      ApiResourceVersion version = resourceVersion;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = userState;
      CancellationToken cancellationToken = new CancellationToken();
      return this.GetAsync<TeamProject>(projectsLocationId, (object) routeValues, version, queryParameters, userState1, cancellationToken);
    }

    public virtual Task<IPagedList<TeamProjectReference>> GetProjects(
      ProjectState? stateFilter,
      int? top,
      int? skip,
      object userState,
      string continuationToken)
    {
      return this.GetProjects(stateFilter, top, skip, userState, continuationToken, new bool?());
    }

    public virtual Task<IPagedList<TeamProjectReference>> GetProjects(
      ProjectState? stateFilter = null,
      int? top = null,
      int? skip = null,
      object userState = null,
      string continuationToken = null,
      bool? getDefaultTeamImageUrl = null)
    {
      List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
      QueryParamHelper.AddNonNullParam(queryParams, nameof (stateFilter), (object) stateFilter);
      QueryParamHelper.AddNonNullParam(queryParams, "$top", (object) top);
      QueryParamHelper.AddNonNullParam(queryParams, "$skip", (object) skip);
      QueryParamHelper.AddNonNullParam(queryParams, nameof (continuationToken), (object) continuationToken);
      QueryParamHelper.AddNonNullParam(queryParams, nameof (getDefaultTeamImageUrl), (object) getDefaultTeamImageUrl);
      return this.GetProjectsAsync((IEnumerable<KeyValuePair<string, string>>) queryParams, userState);
    }

    private async Task<IPagedList<TeamProjectReference>> GetProjectsAsync(
      IEnumerable<KeyValuePair<string, string>> queryParams = null,
      object userState = null)
    {
      ProjectHttpClient projectHttpClient = this;
      IPagedList<TeamProjectReference> projectsAsync;
      using (HttpRequestMessage requestMessage = await projectHttpClient.CreateRequestMessageAsync(HttpMethod.Get, CoreConstants.ProjectsLocationId, version: ProjectHttpClient.s_resourceVersion, queryParameters: queryParams, userState: userState).ConfigureAwait(false))
      {
        string nextToken = (string) null;
        IEnumerable<TeamProjectReference> list;
        using (HttpResponseMessage response = await projectHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          IEnumerable<string> headerValue = projectHttpClient.GetHeaderValue(response, "x-ms-continuationtoken");
          nextToken = headerValue != null ? headerValue.FirstOrDefault<string>() : (string) null;
          list = (IEnumerable<TeamProjectReference>) await projectHttpClient.ReadContentAsAsync<List<TeamProjectReference>>(response).ConfigureAwait(false);
        }
        projectsAsync = (IPagedList<TeamProjectReference>) new PagedList<TeamProjectReference>(list, nextToken);
      }
      return projectsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use GetProjectHistoryEntriesAsync instead")]
    public Task<IEnumerable<TeamProjectReference>> GetProjectHistory(
      long minRevision,
      object userState = null)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      QueryParamHelper.AddNonNullParam(keyValuePairList, nameof (minRevision), (object) minRevision);
      return this.GetAsync<IEnumerable<TeamProjectReference>>(CoreConstants.ProjectHistoryLocationId, version: ProjectHttpClient.s_resourceVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState);
    }

    public Task<OperationReference> QueueCreateProject(
      TeamProject projectToCreate,
      object userState = null)
    {
      return this.PostAsync<TeamProject, OperationReference>(projectToCreate, CoreConstants.ProjectsLocationId, version: ProjectHttpClient.s_resourceVersion, userState: userState);
    }

    public Task<OperationReference> QueueDeleteProject(Guid projectId, object userState = null) => this.QueueDeleteProject(projectId, false, userState);

    public Task<OperationReference> QueueDeleteProject(
      Guid projectId,
      bool hardDelete,
      object userState = null)
    {
      HttpMethod delete = HttpMethod.Delete;
      Guid projectsLocationId = CoreConstants.ProjectsLocationId;
      ApiResourceVersion resourceVersion = ProjectHttpClient.s_resourceVersion;
      var routeValues = new
      {
        projectId = projectId.ToString("D")
      };
      ApiResourceVersion version = resourceVersion;
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      queryParameters.Add(new KeyValuePair<string, string>(nameof (hardDelete), hardDelete.ToString()));
      object userState1 = userState;
      CancellationToken cancellationToken = new CancellationToken();
      return this.SendAsync<OperationReference>(delete, projectsLocationId, (object) routeValues, version, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken);
    }

    public Task<OperationReference> UpdateProject(
      Guid projectToUpdateId,
      TeamProject projectUpdate,
      object userState = null)
    {
      TeamProject teamProject = projectUpdate;
      Guid projectsLocationId = CoreConstants.ProjectsLocationId;
      ApiResourceVersion resourceVersion = ProjectHttpClient.s_resourceVersion;
      var routeValues = new
      {
        projectId = projectToUpdateId.ToString("D")
      };
      ApiResourceVersion version = resourceVersion;
      object userState1 = userState;
      CancellationToken cancellationToken = new CancellationToken();
      return this.PatchAsync<TeamProject, OperationReference>(teamProject, projectsLocationId, (object) routeValues, version, userState: userState1, cancellationToken: cancellationToken);
    }

    public Task<ApiResourceLocation> Options() => this.GetResourceLocationAsync(CoreConstants.ProjectsLocationId);
  }
}
