// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.InternalSourceValidator
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  public class InternalSourceValidator : UpstreamSourceValidator
  {
    public static readonly IReadOnlyList<string> SupportedInternalUpstreamLocationScheme = (IReadOnlyList<string>) new string[2]
    {
      "azure-feed",
      "vsts-feed"
    };
    public const string InternalUpstreamLocationScheme = "azure-feed";

    internal InternalSourceValidator(IVssRequestContext requestContext, UpstreamSource source)
      : base(requestContext, source)
    {
    }

    public override string GetNormalizedLocation()
    {
      Guid? nullable = this.Source.InternalUpstreamCollectionId;
      string collection = nullable.ToString();
      nullable = this.Source.InternalUpstreamProjectId;
      ref Guid? local = ref nullable;
      string project = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
      nullable = this.Source.InternalUpstreamFeedId;
      string feed = nullable.ToString();
      nullable = this.Source.InternalUpstreamViewId;
      string view = nullable.ToString();
      return UpstreamSourceHelper.CreateInternalUpstreamLocator(collection, project, feed, view);
    }

    protected override async Task ValidateUpstreamLocationAsync()
    {
      InternalSourceValidator internalSourceValidator = this;
      bool upstreamHasIds = internalSourceValidator.Source.InternalUpstreamCollectionId.HasValue && internalSourceValidator.Source.InternalUpstreamFeedId.HasValue && internalSourceValidator.Source.InternalUpstreamViewId.HasValue;
      bool flag = !string.IsNullOrWhiteSpace(internalSourceValidator.Source.Location);
      if (!upstreamHasIds && !flag)
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_IncompleteInternalUpstreamSourceFields());
      if (((!internalSourceValidator.requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? 0 : (!upstreamHasIds ? 1 : 0)) & (flag ? 1 : 0)) != 0)
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_UpstreamConfigurationFeedLocatorNotAllowedOnPrem());
      await internalSourceValidator.ValidateInternalUpstreamSourceAccess(upstreamHasIds);
    }

    private async Task ValidateInternalUpstreamSourceAccess(bool upstreamHasIds)
    {
      InternalSourceValidator internalSourceValidator = this;
      string feedName = (string) null;
      string viewName = (string) null;
      if (!upstreamHasIds)
      {
        string hostName;
        string projectName;
        (hostName, projectName, feedName, viewName) = InternalSourceValidator.ParseAndValidateInternalUpstreamLocationString(internalSourceValidator.Source.Location);
        internalSourceValidator.Source.InternalUpstreamCollectionId = new Guid?(InternalSourceValidator.GetCollectionHostId(internalSourceValidator.requestContext, hostName));
        if (!string.IsNullOrEmpty(projectName))
        {
          UpstreamSource upstreamSource = internalSourceValidator.Source;
          upstreamSource.InternalUpstreamProjectId = await UpstreamSourceHelper.GetExternalProjectIdAsync(internalSourceValidator.requestContext, internalSourceValidator.Source.InternalUpstreamCollectionId.Value, projectName, true);
          upstreamSource = (UpstreamSource) null;
          if (internalSourceValidator.Source.InternalUpstreamProjectId.HasValue)
          {
            Guid? upstreamProjectId = internalSourceValidator.Source.InternalUpstreamProjectId;
            Guid empty = Guid.Empty;
            if ((upstreamProjectId.HasValue ? (upstreamProjectId.HasValue ? (upstreamProjectId.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
              goto label_6;
          }
          throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InternalUpstreamsProjectCouldNotBeResolved((object) hostName, (object) projectName));
        }
label_6:
        hostName = (string) null;
        projectName = (string) null;
      }
      if (internalSourceValidator.Source.InternalUpstreamProjectId.HasValue && !internalSourceValidator.requestContext.IsFeatureEnabled("Packaging.Feed.ProjectScopedUpstreams"))
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_ProjectScopedUpstreamsNotEnabled());
      FeedHttpClient excludeUrlsHeader = UpstreamSourceHelper.GetFeedClientWithExcludeUrlsHeader(internalSourceValidator.requestContext.IsFeatureEnabled("Packaging.Feed.Upstreams.UseCallerIdentityForCrossOrgFeedRetrieval") ? internalSourceValidator.requestContext : internalSourceValidator.requestContext.Elevate(), internalSourceValidator.Source.InternalUpstreamCollectionId.Value);
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedResult;
      Guid? nullable;
      try
      {
        if (internalSourceValidator.Source.InternalUpstreamProjectId.HasValue)
        {
          Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedAsync;
          if (upstreamHasIds)
          {
            FeedHttpClient feedHttpClient = excludeUrlsHeader;
            nullable = internalSourceValidator.Source.InternalUpstreamProjectId;
            string project = nullable.ToString();
            nullable = internalSourceValidator.Source.InternalUpstreamFeedId;
            string str1 = nullable.ToString();
            nullable = internalSourceValidator.Source.InternalUpstreamViewId;
            string str2 = nullable.ToString();
            string feedId = str1 + "@" + str2;
            bool? includeDeletedUpstreams = new bool?();
            CancellationToken cancellationToken = new CancellationToken();
            feedAsync = await feedHttpClient.GetFeedAsync(project, feedId, includeDeletedUpstreams, cancellationToken: cancellationToken);
          }
          else
          {
            FeedHttpClient feedHttpClient = excludeUrlsHeader;
            nullable = internalSourceValidator.Source.InternalUpstreamProjectId;
            string project = nullable.ToString();
            string feedId = feedName + "@" + viewName;
            bool? includeDeletedUpstreams = new bool?();
            CancellationToken cancellationToken = new CancellationToken();
            feedAsync = await feedHttpClient.GetFeedAsync(project, feedId, includeDeletedUpstreams, cancellationToken: cancellationToken);
          }
          feedResult = feedAsync;
        }
        else
        {
          Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedAsync;
          if (upstreamHasIds)
          {
            FeedHttpClient feedHttpClient = excludeUrlsHeader;
            nullable = internalSourceValidator.Source.InternalUpstreamFeedId;
            string str3 = nullable.ToString();
            nullable = internalSourceValidator.Source.InternalUpstreamViewId;
            string str4 = nullable.ToString();
            string feedId = str3 + "@" + str4;
            CancellationToken cancellationToken = new CancellationToken();
            feedAsync = await feedHttpClient.GetFeedAsync(feedId, (object) null, cancellationToken);
          }
          else
            feedAsync = await excludeUrlsHeader.GetFeedAsync(feedName + "@" + viewName, (object) null, new CancellationToken());
          feedResult = feedAsync;
        }
      }
      catch (VssServiceException ex)
      {
        internalSourceValidator.requestContext.TraceException(10019020, "Feed", "Service", (Exception) ex);
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InvalidUpstream((object) internalSourceValidator.Source.Location));
      }
      nullable = internalSourceValidator.Source.InternalUpstreamCollectionId;
      Guid instanceId = internalSourceValidator.requestContext.ServiceHost.InstanceId;
      if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != instanceId ? 1 : 0) : 0) : 1) != 0 && !feedResult.IsPublicFeed())
      {
        Guid tenantId = internalSourceValidator.requestContext.IsOrganizationAadBacked() ? internalSourceValidator.requestContext.GetOrganizationAadTenantId() : throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InternalUpstreamsNotEnabledForNonAad((object) internalSourceValidator.Source.Location));
        IVssRequestContext requestContext = internalSourceValidator.requestContext;
        nullable = internalSourceValidator.Source.InternalUpstreamCollectionId;
        Guid upstreamHostId = nullable.Value;
        if (tenantId != await InternalSourceValidator.GetUpstreamTenantId(requestContext, upstreamHostId))
          throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InvalidUpstream((object) internalSourceValidator.Source.Location));
        if (internalSourceValidator.requestContext.IsFeatureEnabled("Packaging.Feed.Upstreams.ValidateCrossOrgMembership"))
        {
          try
          {
            await internalSourceValidator.ValidateAccessToSourceCollection();
          }
          catch (Exception ex)
          {
            throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InvalidUpstream((object) internalSourceValidator.Source.Location));
          }
        }
        tenantId = new Guid();
      }
      if (!upstreamHasIds)
      {
        internalSourceValidator.Source.InternalUpstreamFeedId = new Guid?(feedResult.Id);
        internalSourceValidator.Source.InternalUpstreamViewId = new Guid?(feedResult.View.Id);
      }
      string location = internalSourceValidator.Source.Location;
      internalSourceValidator.Source.Location = internalSourceValidator.GetNormalizedLocation();
      InternalSourceValidator.ValidateFeedVisibility(internalSourceValidator.requestContext, feedResult, internalSourceValidator.Source, location);
      feedName = (string) null;
      viewName = (string) null;
      feedResult = (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
    }

    private async Task ValidateAccessToSourceCollection()
    {
      ICrossCollectionClientCreatorService clientCreatorService = this.requestContext.GetService<ICrossCollectionClientCreatorService>();
      ProjectHttpClient projectHttpClient = AsyncPump.Run<ProjectHttpClient>((Func<Task<ProjectHttpClient>>) (() => clientCreatorService.CreateClientAsync<ProjectHttpClient>(this.requestContext, this.Source.InternalUpstreamCollectionId.Value, ServiceInstanceTypes.TFS)));
      int? nullable = new int?(1);
      ProjectState? stateFilter = new ProjectState?();
      int? top = nullable;
      int? skip = new int?();
      bool? getDefaultTeamImageUrl = new bool?();
      IPagedList<TeamProjectReference> projects = await projectHttpClient.GetProjects(stateFilter, top, skip, (object) null, (string) null, getDefaultTeamImageUrl);
    }

    private static (string, string, string, string) ParseAndValidateInternalUpstreamLocationString(
      string location)
    {
      try
      {
        Uri uri = new Uri(location);
        if (!uri.IsAbsoluteUri)
          throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InternalUpstreamLocationFormatIsIncorrect((object) location));
        if (!InternalSourceValidator.SupportedInternalUpstreamLocationScheme.Any<string>((Func<string, bool>) (s => s.Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))))
          throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InternalUpstreamContainsIncorrectScheme((object) uri.Scheme));
        string host = uri.Host;
        string str1;
        string segment;
        if (uri.Segments.Length == 2)
        {
          str1 = (string) null;
          segment = uri.Segments[1];
        }
        else
        {
          if (uri.Segments.Length != 3)
            throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InternalUpstreamLocationFormatIsIncorrect((object) location));
          str1 = uri.Segments[1].TrimEnd('/');
          segment = uri.Segments[2];
        }
        (string str2, string str3) = FeedViewsHelper.ParseFeedNameParameter(segment);
        if (string.IsNullOrWhiteSpace(str2) || string.IsNullOrWhiteSpace(str3))
          throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InternalUpstreamContainsIncorrectFeed((object) segment));
        return (host, str1, str2, str3);
      }
      catch (UriFormatException ex)
      {
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InternalUpstreamLocationFormatIsIncorrect((object) location));
      }
    }

    private static async Task<Guid> GetUpstreamTenantId(
      IVssRequestContext requestContext,
      Guid upstreamHostId)
    {
      IVssRequestContext elevatedRequestContext = requestContext.Elevate();
      ICrossCollectionClientCreatorService clientCreatorService = elevatedRequestContext.GetService<ICrossCollectionClientCreatorService>();
      OrganizationHttpClient organizationHttpClient;
      try
      {
        organizationHttpClient = AsyncPump.Run<OrganizationHttpClient>((Func<Task<OrganizationHttpClient>>) (() => clientCreatorService.CreateClientAsync<OrganizationHttpClient>(elevatedRequestContext, upstreamHostId, ServiceInstanceTypes.SPS)));
      }
      catch (ServiceHostDoesNotExistException ex)
      {
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InternalUpstreamsHostCouldNotBeResolved((object) upstreamHostId), (Exception) ex);
      }
      return (await organizationHttpClient.GetCollectionAsync("me")).TenantId;
    }

    private static void ValidateFeedVisibility(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      UpstreamSource upstreamSource,
      string originalLocator)
    {
      Guid guid = upstreamSource.InternalUpstreamCollectionId.Value;
      FeedVisibility target = requestContext.ServiceHost.InstanceId == guid ? FeedVisibility.Collection : (upstreamSource.ServiceEndpointId.HasValue ? FeedVisibility.Collection : FeedVisibility.Organization);
      if (!feed.IsPublicFeed() && feed.Visibility.CompareTo((object) target) < 0)
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InvalidUpstream(string.IsNullOrWhiteSpace(originalLocator) ? (object) upstreamSource.Location : (object) originalLocator));
    }

    public static async Task<bool> ElevatedIsPublicProjectScopedFeedAsync(
      IVssRequestContext requestContext,
      UpstreamSource upstreamSource)
    {
      Guid? nullable = upstreamSource.InternalUpstreamProjectId;
      if (!nullable.HasValue)
        return false;
      IVssRequestContext requestContext1 = requestContext;
      nullable = upstreamSource.InternalUpstreamCollectionId;
      Guid collectionId = nullable.Value;
      nullable = upstreamSource.InternalUpstreamProjectId;
      Guid projectName = nullable.Value;
      return await UpstreamSourceHelper.IsExternalProjectPublicAsync(requestContext1, collectionId, projectName, true);
    }

    private static Guid GetCollectionHostId(IVssRequestContext requestContext, string hostName)
    {
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      Guid collectionId;
      HostNameResolver.TryGetCollectionServiceHostId(requestContext.To(TeamFoundationHostType.Deployment), hostName, out collectionId);
      return !(collectionId == Guid.Empty) ? collectionId : throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InternalUpstreamsHostCouldNotBeResolved((object) hostName));
    }

    public static async Task ValidateServiceEndpointMatchesUpstream(
      IVssRequestContext requestContext,
      UpstreamSource upstream,
      ServiceEndpoint serviceEndpoint)
    {
      if (serviceEndpoint == null || serviceEndpoint.Url == (Uri) null)
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.ServiceEndpointUnset());
      (string str1, string str2, string feedId1, string viewId1) = InternalSourceValidator.ParseAndValidateInternalUpstreamLocationString(upstream.Location);
      (string str3, string str4, string feedId2, string viewId2) = InternalSourceValidator.ParseAndValidateInternalUpstreamLocationString(serviceEndpoint.Data["feedLocator"]);
      Guid upstreamHostId;
      if (!Guid.TryParse(str1, out upstreamHostId))
        upstreamHostId = InternalSourceValidator.GetCollectionHostId(requestContext, str1);
      Guid svcConnHostId;
      if (!Guid.TryParse(str3, out svcConnHostId))
        svcConnHostId = InternalSourceValidator.GetCollectionHostId(requestContext, str3);
      if (upstreamHostId != svcConnHostId)
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.InvalidServiceEndpointConfig((object) upstream.Location, (object) serviceEndpoint.Data["feedLocator"]));
      if (str2 == null && str4 != null || str2 != null && str4 == null)
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.InvalidServiceEndpointConfig((object) upstream.Location, (object) serviceEndpoint.Data["feedLocator"]));
      Guid? upstreamProjectId = new Guid?();
      Guid? svcConnProjectId = new Guid?();
      if (str2 != null)
      {
        Guid result;
        if (Guid.TryParse(str2, out result))
          upstreamProjectId = new Guid?(result);
        else
          upstreamProjectId = await UpstreamSourceHelper.GetExternalProjectIdAsync(requestContext, upstreamHostId, str2, true);
      }
      if (str4 != null)
      {
        Guid result;
        if (Guid.TryParse(str4, out result))
          svcConnProjectId = new Guid?(result);
        else
          svcConnProjectId = await UpstreamSourceHelper.GetExternalProjectIdAsync(requestContext, svcConnHostId, str4, true);
      }
      Guid? nullable1 = upstreamProjectId;
      Guid? nullable2 = svcConnProjectId;
      if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.InvalidServiceEndpointConfig((object) upstream.Location, (object) serviceEndpoint.Data["feedLocator"]));
      EndpointAuthorization authorization = serviceEndpoint.Authorization;
      VssCredentials credentials = (VssCredentials) null;
      string password;
      if (authorization != null && authorization.Parameters.TryGetValue("apiToken", out password))
        credentials = (VssCredentials) (FederatedCredential) new VssBasicCredential("userName", password);
      if (credentials == null)
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.InvalidServiceEndpointConfig((object) upstream.Location, (object) serviceEndpoint.Data["feedLocator"]));
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed foundUpstreamFeed = await UpstreamSourceHelper.GetExternalCollectionFeedWithPatAsync(requestContext, upstreamHostId, upstreamProjectId, feedId1, viewId1, credentials);
      nullable2 = foundUpstreamFeed != null ? foundUpstreamFeed.ViewId : throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InvalidUpstream((object) upstream.Location));
      if (nullable2.HasValue)
      {
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedWithPatAsync = await UpstreamSourceHelper.GetExternalCollectionFeedWithPatAsync(requestContext, svcConnHostId, svcConnProjectId, feedId2, viewId2, credentials);
        nullable2 = feedWithPatAsync != null ? feedWithPatAsync.ViewId : throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InvalidUpstream((object) serviceEndpoint.Data["feedLocator"]));
        if (nullable2.HasValue)
        {
          if (!(foundUpstreamFeed.Id != feedWithPatAsync.Id))
          {
            nullable2 = foundUpstreamFeed.ViewId;
            Guid guid1 = nullable2.Value;
            nullable2 = feedWithPatAsync.ViewId;
            Guid guid2 = nullable2.Value;
            if (!(guid1 != guid2))
            {
              feedId1 = (string) null;
              viewId1 = (string) null;
              str4 = (string) null;
              feedId2 = (string) null;
              viewId2 = (string) null;
              credentials = (VssCredentials) null;
              foundUpstreamFeed = (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
            }
          }
          throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.InvalidServiceEndpointConfig((object) upstream.Location, (object) serviceEndpoint.Data["feedLocator"]));
        }
      }
    }
  }
}
