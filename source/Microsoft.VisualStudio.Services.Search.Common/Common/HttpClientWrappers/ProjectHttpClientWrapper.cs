// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.ProjectHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class ProjectHttpClientWrapper
  {
    private readonly ProjectHttpClient m_projectHttpClient;
    private readonly WikiHttpClient m_wikiHttpClient;
    private readonly ExponentialBackoffRetryInvoker m_expRetryInvoker;
    private readonly TraceMetaData m_traceMetaData;
    private readonly IIndexerFaultService m_faultService;
    private readonly int m_waitTimeInMs;
    private readonly int m_retryLimit;
    public const string ProjectTagsPropertyPrefix = "Microsoft.TeamFoundation.Project.Tag.";
    internal const string Asterisk = "*";
    private readonly int m_projectBatchSize;
    private readonly IVssRequestContext m_requestContext;

    public int ProjectBatchSize => this.m_projectBatchSize;

    public ProjectHttpClientWrapper() => this.m_projectBatchSize = 100;

    public ProjectHttpClientWrapper(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext, TraceMetaData traceMetadata)
    {
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      if (executionContext.RequestContext.IsFeatureEnabled("Search.Server.Code.EnableFrameworkService"))
        this.m_requestContext = executionContext.RequestContext;
      this.m_projectHttpClient = executionContext.RequestContext.GetRedirectedClientIfNeeded<ProjectHttpClient>(executionContext.RequestContext.GetHttpClientOptionsForEventualReadConsistencyLevel("Search.Server.Project.EnableReadReplica"));
      this.m_wikiHttpClient = executionContext.RequestContext.GetClient<WikiHttpClient>();
      this.m_faultService = executionContext.FaultService;
      this.m_expRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.m_traceMetaData = traceMetadata;
      this.m_waitTimeInMs = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryIntervalInSec * 1000;
      this.m_retryLimit = executionContext.ServiceSettings.PipelineSettings.CrawlOperationRetryLimit;
      this.m_projectBatchSize = executionContext.RequestContext.GetService<IVssRegistryService>().GetValue<int>(executionContext.RequestContext, (RegistryQuery) "/Service/ALMSearch/Settings/MaxProjectBatchSize", true, 100);
    }

    public ProjectHttpClientWrapper(IVssRequestContext requestContext, TraceMetaData traceMetadata)
    {
      if (requestContext.IsFeatureEnabled("Search.Server.Code.EnableFrameworkService"))
        this.m_requestContext = requestContext;
      this.m_projectHttpClient = requestContext.GetRedirectedClientIfNeeded<ProjectHttpClient>();
      this.m_expRetryInvoker = ExponentialBackoffRetryInvoker.Instance;
      this.m_traceMetaData = traceMetadata;
      this.m_faultService = (IIndexerFaultService) new IndexerV1FaultService(requestContext, (IFaultStore) new RegistryServiceFaultStore(requestContext));
      this.m_waitTimeInMs = 60000;
      this.m_retryLimit = 3;
      this.m_projectBatchSize = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/ALMSearch/Settings/MaxProjectBatchSize", true, 100);
    }

    public virtual IEnumerable<TeamProjectReference> GetProjectBatch(
      int top,
      int skip = 0,
      ProjectState? stateFilter = null)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<IEnumerable<TeamProjectReference>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<IPagedList<TeamProjectReference>>((Func<CancellationTokenSource, Task<IPagedList<TeamProjectReference>>>) (tokenSource => this.m_projectHttpClient.GetProjects(stateFilter, new int?(top), new int?(skip))), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    public virtual IEnumerable<TeamProjectReference> GetProjects(ProjectState? stateFilter = null)
    {
      IVssRequestContext requestContext = this.m_requestContext;
      if ((requestContext != null ? (requestContext.IsFeatureEnabled("Search.Server.Code.EnableFrameworkService") ? 1 : 0) : 0) != 0)
      {
        List<TeamProjectReference> referenceEnumerable = ProjectHttpClientWrapper.ToTeamProjectReferenceEnumerable(this.m_requestContext.GetService<IProjectService>().GetProjects(this.m_requestContext, (ProjectState) ((int) stateFilter ?? -1)), this.m_requestContext);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082635, "Common", "Diagnostics", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetProjects return {0} project(s)", (object) referenceEnumerable.Count));
        return (IEnumerable<TeamProjectReference>) referenceEnumerable;
      }
      FriendlyDictionary<Guid, TeamProjectReference> friendlyDictionary = new FriendlyDictionary<Guid, TeamProjectReference>();
      int num;
      IEnumerable<TeamProjectReference> projectBatch;
      for (int skip = 0; (projectBatch = this.GetProjectBatch(this.ProjectBatchSize, skip, stateFilter)) != null && projectBatch.Any<TeamProjectReference>(); skip += num)
      {
        num = 0;
        foreach (TeamProjectReference projectReference1 in projectBatch)
        {
          ++num;
          Guid id = projectReference1.Id;
          TeamProjectReference projectReference2;
          if (friendlyDictionary.TryGetValue(id, out projectReference2))
          {
            if (projectReference1.Revision > projectReference2.Revision)
              friendlyDictionary[id] = projectReference1;
          }
          else
            friendlyDictionary[id] = projectReference1;
        }
        if (num < this.ProjectBatchSize)
          break;
      }
      return (IEnumerable<TeamProjectReference>) friendlyDictionary.Values;
    }

    public virtual TeamProject GetProjectInGivenState(string projectId, ProjectState projectState = ProjectState.WellFormed)
    {
      TeamProject projectInGivenState = (TeamProject) null;
      try
      {
        TeamProject withCapabilities = this.GetTeamProjectWithCapabilities(projectId, false);
        if (withCapabilities != null)
        {
          if (projectState != ProjectState.All)
          {
            if (projectState != withCapabilities.State)
              goto label_7;
          }
          projectInGivenState = withCapabilities;
        }
      }
      catch (Exception ex)
      {
        if (!IndexFaultMapManager.GetFaultMapper(typeof (ProjectNotFoundFaultMapper)).IsMatch(ex))
          ExceptionDispatchInfo.Capture(ex).Throw();
      }
label_7:
      return projectInGivenState;
    }

    public virtual TeamProject GetTeamProjectWithCapabilities(
      string projectId,
      bool includeCapabilities = true)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<TeamProject>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<TeamProject>((Func<CancellationTokenSource, Task<TeamProject>>) (tokenSource => this.m_projectHttpClient.GetProject(projectId, new bool?(includeCapabilities))), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    public virtual IEnumerable<TeamProject> GetTeamProjectsWithCapabilities()
    {
      List<TeamProject> withCapabilities = new List<TeamProject>();
      IEnumerable<TeamProjectReference> projects = this.GetProjects();
      if (projects != null)
      {
        foreach (TeamProjectReference projectReference in projects)
        {
          try
          {
            withCapabilities.Add(this.GetTeamProjectWithCapabilities(projectReference.Id.ToString()));
          }
          catch (Exception ex)
          {
            if (!IndexFaultMapManager.GetFaultMapper(typeof (ProjectNotFoundFaultMapper)).IsMatch(ex))
              ExceptionDispatchInfo.Capture(ex).Throw();
          }
        }
      }
      return (IEnumerable<TeamProject>) withCapabilities;
    }

    public virtual List<ProjectProperty> GetProjectProperties(
      Guid projectId,
      List<string> propertyKeys)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<List<ProjectProperty>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<ProjectProperty>>((Func<CancellationTokenSource, Task<List<ProjectProperty>>>) (tokenSource => this.m_projectHttpClient.GetProjectPropertiesAsync(projectId, (IEnumerable<string>) propertyKeys)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    public virtual bool TryIsWikiRepo(Guid projectId, Guid repositoryId)
    {
      IEnumerable<Microsoft.TeamFoundation.Wiki.WebApi.Wiki> wikisInProject = this.GetWikisInProject(projectId);
      Guid empty = Guid.Empty;
      return wikisInProject != null && wikisInProject.Any<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>() && wikisInProject.Where<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>((Func<Microsoft.TeamFoundation.Wiki.WebApi.Wiki, bool>) (wiki => wiki.Repository.Id.Equals(repositoryId))).Any<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>();
    }

    public virtual List<string> GetProjectTags(Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, "ProjectId");
      List<string> propertyKeys = new List<string>()
      {
        "Microsoft.TeamFoundation.Project.Tag.*"
      };
      List<string> projectTags = new List<string>();
      List<ProjectProperty> projectProperties = this.GetProjectProperties(projectId, propertyKeys);
      if (projectProperties != null)
      {
        foreach (ProjectProperty projectProperty in projectProperties)
        {
          string str = projectProperty?.Name?.ToString().Substring("Microsoft.TeamFoundation.Project.Tag.".Length);
          if (!string.IsNullOrEmpty(str))
            projectTags.Add(str.Trim());
        }
      }
      return projectTags;
    }

    public virtual IEnumerable<Microsoft.TeamFoundation.Wiki.WebApi.Wiki> GetWikisInProject(
      Guid teamProjectId)
    {
      return this.m_expRetryInvoker.InvokeWithFaultCheck<IEnumerable<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>>((Func<object>) (() => (object) AsyncInvoker.InvokeAsyncWait<List<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>>((Func<CancellationTokenSource, Task<List<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>>>) (tokenSource => this.m_wikiHttpClient.GetWikisAsync(teamProjectId, (object) null, tokenSource.Token)), this.m_waitTimeInMs, this.m_traceMetaData)), this.m_faultService, this.m_retryLimit, this.m_waitTimeInMs, true, this.m_traceMetaData);
    }

    private static TeamProjectReference ToTeamProjectReference(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      ILocationService locationService)
    {
      var routeValues = new{ projectId = projectInfo.Id };
      Uri resourceUri;
      try
      {
        resourceUri = locationService.GetResourceUri(requestContext, "core", CoreConstants.ProjectsLocationId, (object) routeValues);
      }
      catch (VssResourceNotFoundException ex)
      {
        resourceUri = locationService.GetLocationData(requestContext, Guid.Parse("79134C72-4A58-4B42-976C-04E7115F32BF")).GetResourceUri(requestContext, "core", CoreConstants.ProjectsLocationId, (object) routeValues);
      }
      return new TeamProjectReference()
      {
        Id = projectInfo.Id,
        Abbreviation = projectInfo.Abbreviation,
        Name = projectInfo.Name,
        Url = resourceUri?.ToString(),
        State = projectInfo.State,
        Description = projectInfo.Description,
        Revision = projectInfo.Revision,
        Visibility = projectInfo.Visibility,
        LastUpdateTime = projectInfo.LastUpdateTime
      };
    }

    private static List<TeamProjectReference> ToTeamProjectReferenceEnumerable(
      IEnumerable<ProjectInfo> projects,
      IVssRequestContext requestContext)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      List<TeamProjectReference> referenceEnumerable = new List<TeamProjectReference>();
      foreach (ProjectInfo project in projects)
        referenceEnumerable.Add(ProjectHttpClientWrapper.ToTeamProjectReference(requestContext, project, service));
      return referenceEnumerable;
    }
  }
}
