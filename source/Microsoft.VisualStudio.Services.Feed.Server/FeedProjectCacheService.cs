// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedProjectCacheService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Utils;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedProjectCacheService : 
    VssMemoryCacheService<ProjectCacheKey, TeamProject>,
    IFeedProjectCacheService,
    IVssFrameworkService
  {
    private const int MaxElementsDefault = 10000;
    private static readonly MemoryCacheConfiguration<ProjectCacheKey, TeamProject> DefaultMemoryCacheConfiguration = new MemoryCacheConfiguration<ProjectCacheKey, TeamProject>().WithCleanupInterval(TimeSpan.FromMinutes(60.0)).WithExpiryInterval(TimeSpan.FromMinutes(30.0)).WithMaxElements(10000);
    private readonly ConcurrencyConsolidator<ProjectCacheKey, TeamProject> fetchProjectByNameOrIdConsolidator = new ConcurrencyConsolidator<ProjectCacheKey, TeamProject>(false, 2);

    public FeedProjectCacheService()
      : base((IEqualityComparer<ProjectCacheKey>) EqualityComparer<ProjectCacheKey>.Default, FeedProjectCacheService.DefaultMemoryCacheConfiguration)
    {
    }

    public FeedProjectCacheService(double evictionTimeInMinutes, double cleanupTimeInMinutes)
      : base(TimeSpan.FromMinutes(cleanupTimeInMinutes))
    {
      this.InactivityInterval.Value = (TimeSpan) Capture.Create<TimeSpan>(TimeSpan.FromMinutes(evictionTimeInMinutes));
    }

    public async Task<TeamProject> GetProject(
      IVssRequestContext requestContext,
      string projectNameOrId,
      Guid hostId,
      bool shouldElevate = false)
    {
      requestContext.CheckDeploymentRequestContext();
      if (!requestContext.IsFeatureEnabled("Packaging.Feed.UseProjectServiceCache"))
        return await this.GetProjectFromProjectServiceAsync(requestContext, hostId, projectNameOrId, shouldElevate);
      TeamProject projectInfo;
      if (!this.TryGetValueWrapper(requestContext, new ProjectCacheKey(hostId, projectNameOrId), out projectInfo) || !shouldElevate && projectInfo.Visibility != ProjectVisibility.Public)
        projectInfo = await this.AddProjectToCacheFromServiceAsync(requestContext, hostId, projectNameOrId, shouldElevate);
      return projectInfo;
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      base.ServiceStart(requestContext);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext) => base.ServiceEnd(requestContext);

    private bool TryGetValueWrapper(
      IVssRequestContext requestContext,
      ProjectCacheKey projectCacheKey,
      out TeamProject projectInfo)
    {
      bool valueWrapper = this.TryGetValue(requestContext, projectCacheKey, out projectInfo);
      requestContext.Trace(10019152, TraceLevel.Info, "Feed", "CacheService", valueWrapper ? "Cache hit: Project= " + projectCacheKey.ProjectNameOrId : "Cache miss for project: Project= " + projectCacheKey.ProjectNameOrId);
      return valueWrapper;
    }

    private async Task<TeamProject> AddProjectToCacheFromServiceAsync(
      IVssRequestContext requestContext,
      Guid hostId,
      string projectNameOrId,
      bool shouldElevate)
    {
      FeedProjectCacheService projectCacheService = this;
      TeamProject projectServiceAsync = await projectCacheService.GetProjectFromProjectServiceAsync(requestContext, hostId, projectNameOrId, shouldElevate);
      if (shouldElevate || projectServiceAsync.Visibility == ProjectVisibility.Public)
      {
        projectCacheService.Set(requestContext, new ProjectCacheKey(hostId, projectNameOrId), projectServiceAsync);
        requestContext.Trace(10019153, TraceLevel.Info, "Feed", "CacheService", string.Format("Project added: Project= {0}, Name= {1}", (object) projectServiceAsync.Id, (object) projectServiceAsync.Name));
      }
      return projectServiceAsync;
    }

    private async Task<TeamProject> GetProjectFromProjectServiceAsync(
      IVssRequestContext requestContext,
      Guid hostId,
      string projectNameOrId,
      bool shouldElevate)
    {
      requestContext.TraceEnter(10019150, "Feed", "CacheService", nameof (GetProjectFromProjectServiceAsync));
      TeamProject projectServiceAsync;
      try
      {
        projectServiceAsync = await this.fetchProjectByNameOrIdConsolidator.RunOnceAsync(new ProjectCacheKey(hostId, projectNameOrId), (Func<Task<TeamProject>>) (async () =>
        {
          TeamProject project;
          try
          {
            project = await (await this.GetProjectClient(requestContext, hostId, shouldElevate)).GetProject(projectNameOrId);
          }
          catch (VssServiceResponseException ex) when (ex.HttpStatusCode == HttpStatusCode.Forbidden)
          {
            throw new UnauthorizedAccessException("GetProject returned status code Forbidden", (Exception) ex);
          }
          return project;
        }));
      }
      finally
      {
        requestContext.TraceLeave(10019150, "Feed", "CacheService", nameof (GetProjectFromProjectServiceAsync));
      }
      return projectServiceAsync;
    }

    private async Task<ProjectHttpClient> GetProjectClient(
      IVssRequestContext requestContext,
      Guid hostId,
      bool shouldElevate)
    {
      IVssRequestContext vssRequestContext = shouldElevate ? requestContext.Elevate() : requestContext;
      ICrossCollectionClientCreatorService service = vssRequestContext.GetService<ICrossCollectionClientCreatorService>();
      ProjectHttpClient clientAsync;
      try
      {
        clientAsync = await service.CreateClientAsync<ProjectHttpClient>(vssRequestContext, hostId, ServiceInstanceTypes.TFS);
      }
      catch (ServiceHostDoesNotExistException ex)
      {
        throw new InvalidUpstreamSourceException(Resources.Error_InternalUpstreamsHostCouldNotBeResolved((object) hostId), (Exception) ex);
      }
      return clientAsync;
    }
  }
}
