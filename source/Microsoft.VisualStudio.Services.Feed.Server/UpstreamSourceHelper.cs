// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.UpstreamSourceHelper
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
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public static class UpstreamSourceHelper
  {
    public static IEqualityComparer<UpstreamIdentifier> UpstreamIdentifierComparer { get; } = (IEqualityComparer<UpstreamIdentifier>) new Microsoft.VisualStudio.Services.Feed.Server.UpstreamIdentifierComparer();

    public static async Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetExternalCollectionFeedWithPatAsync(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid? projectId,
      string feedId,
      string viewId,
      VssCredentials overrideCredentials)
    {
      if (requestContext.IsSystemContext)
        throw new UnauthorizedRequestException(requestContext.GetUserId());
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedInternalAsync = await UpstreamSourceHelper.GetFeedInternalAsync(UpstreamSourceHelper.GetFeedClientWithExcludeUrlsHeader(requestContext, hostId), projectId, feedId, viewId);
      if (feedInternalAsync == null || feedInternalAsync.Visibility == FeedVisibility.Private)
        return (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
      FeedHttpClient excludeUrlsHeader = UpstreamSourceHelper.GetFeedClientWithExcludeUrlsHeader(requestContext, hostId, overrideCredentials);
      try
      {
        return await UpstreamSourceHelper.GetFeedInternalAsync(excludeUrlsHeader, projectId, feedId, viewId);
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(10019142, "Feed", "Model", ex);
      }
      return (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
    }

    private static async Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedInternalAsync(
      FeedHttpClient client,
      Guid? projectId,
      string feedId,
      string viewId)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedAsync;
      if (projectId.HasValue)
        feedAsync = await client.GetFeedAsync(projectId.Value, feedId + "@" + viewId);
      else
        feedAsync = await client.GetFeedAsync(feedId + "@" + viewId, (object) null, new CancellationToken());
      return feedAsync;
    }

    public static async Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetExternalCollectionFeedAsync(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid? projectId,
      string feedId,
      string viewId)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedInternalAsync = await UpstreamSourceHelper.GetFeedInternalAsync(UpstreamSourceHelper.GetFeedClientWithExcludeUrlsHeader(requestContext.Elevate(), hostId), projectId, feedId, viewId);
      return feedInternalAsync.Project?.Visibility == "Public" || feedInternalAsync.Visibility == FeedVisibility.Organization ? feedInternalAsync : (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
    }

    public static FeedHttpClient GetFeedClientWithExcludeUrlsHeader(
      IVssRequestContext requestContext,
      Guid hostId,
      VssCredentials overrideCredentials = null)
    {
      ICrossCollectionClientCreatorService clientCreatorService = requestContext.GetService<ICrossCollectionClientCreatorService>();
      try
      {
        FeedHttpClient excludeUrlsHeader = overrideCredentials == null ? AsyncPump.Run<FeedHttpClient>((Func<Task<FeedHttpClient>>) (() => clientCreatorService.CreateClientAsync<FeedHttpClient>(requestContext, hostId, Guid.Parse("00000036-0000-8888-8000-000000000000")))) : AsyncPump.Run<FeedHttpClient>((Func<Task<FeedHttpClient>>) (() => clientCreatorService.CreateClientAsync<FeedHttpClient>(requestContext, hostId, Guid.Parse("00000036-0000-8888-8000-000000000000"), (Func<Uri, FeedHttpClient>) (baseUri => new FeedHttpClient(baseUri, overrideCredentials)))));
        excludeUrlsHeader.ExcludeUrlsHeader = true;
        return excludeUrlsHeader;
      }
      catch (ServiceHostDoesNotExistException ex)
      {
        throw new InvalidUpstreamSourceException(Resources.Error_InternalUpstreamsHostCouldNotBeResolved((object) hostId), (Exception) ex);
      }
    }

    public static string CreateInternalUpstreamLocator(
      string collection,
      string project,
      string feed,
      string view)
    {
      return string.IsNullOrEmpty(project) ? "azure-feed://" + collection + "/" + feed + "@" + view : "azure-feed://" + collection + "/" + project + "/" + feed + "@" + view;
    }

    public static async Task<string> GetExternalProjectNameAsync(
      IVssRequestContext requestContext,
      Guid collectionId,
      Guid projectId)
    {
      TeamProject teamProject = (TeamProject) null;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        teamProject = await vssRequestContext.GetService<IFeedProjectCacheService>().GetProject(vssRequestContext, projectId.ToString(), collectionId);
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(10019103, "Feed", "Model", ex);
      }
      string name = teamProject?.Name;
      teamProject = (TeamProject) null;
      return name;
    }

    public static async Task<Guid?> GetExternalProjectIdAsync(
      IVssRequestContext requestContext,
      Guid collectionId,
      string projectName,
      bool shouldElevate)
    {
      TeamProject teamProject = (TeamProject) null;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        teamProject = await vssRequestContext.GetService<IFeedProjectCacheService>().GetProject(vssRequestContext, projectName, collectionId, shouldElevate);
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(10019103, "Feed", "Model", ex);
      }
      Guid? id = teamProject?.Id;
      teamProject = (TeamProject) null;
      return id;
    }

    public static async Task<bool> IsExternalProjectPublicAsync(
      IVssRequestContext requestContext,
      Guid collectionId,
      Guid projectName,
      bool shouldElevate)
    {
      TeamProject teamProject = (TeamProject) null;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        teamProject = await vssRequestContext.GetService<IFeedProjectCacheService>().GetProject(vssRequestContext, projectName.ToString(), collectionId, shouldElevate);
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(10019103, "Feed", "Model", ex);
      }
      if (teamProject == null)
        return false;
      TeamProject teamProject1 = teamProject;
      return teamProject1 != null && teamProject1.Visibility == ProjectVisibility.Public;
    }
  }
}
