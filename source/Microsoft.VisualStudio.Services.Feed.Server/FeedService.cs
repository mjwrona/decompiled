// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.MessageBus;
using Microsoft.VisualStudio.Services.Feed.Server.Telemetry;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.Server.Utils;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedService : IFeedService, IVssFrameworkService
  {
    public static readonly string[] ReservedFeedNames = new string[4]
    {
      "feeds",
      "npm",
      "nuget",
      "pypi"
    };

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.TraceBlock(10019000, 10019001, 10019002, "Feed", "Service", (Action) (() =>
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", ProjectSqlNotificationEventClasses.ProjectUpdated, new SqlNotificationHandler(this.OnProjectDeleted), false);
    }), "ServiceStart");

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => requestContext.TraceBlock(10019003, 10019004, 10019005, "Feed", "Service", (Action) (() =>
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", ProjectSqlNotificationEventClasses.ProjectUpdated, new SqlNotificationHandler(this.OnProjectDeleted), false);
    }), "ServiceEnd");

    public async Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> CreateFeed(
      IVssRequestContext requestContext,
      string feedName,
      string feedDescription,
      bool upstreamEnabled = false,
      bool allowUpstreamNameConflict = false,
      bool? hideDeletedPackageVersions = true,
      bool? badgesEnabled = false,
      IEnumerable<FeedPermission> feedPermission = null,
      IList<UpstreamSource> upstreamSources = null,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project = null)
    {
      return await requestContext.TraceBlock<Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>(10019006, 10019007, 10019008, "Feed", "Service", (Func<Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>) (async () =>
      {
        feedName.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedName))));
        FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, project);
        FeedSecurityHelper.CheckCreateFeedPermissions(requestContext);
        FeedService.ValidateFeedName(feedName);
        FeedService.ValidateFeedDescription(feedDescription);
        this.ValidateAllowUpstreamForProjectFeed(requestContext, upstreamEnabled, project);
        await this.ValidateServiceEndpointIds(requestContext, project, upstreamSources);
        List<AccessControlEntry> defaultPermissions = new List<AccessControlEntry>();
        IdentityDescriptor authenticatedUser = requestContext.GetAuthenticatedDescriptor();
        if (authenticatedUser.IdentityType != "System:ServicePrincipal")
          defaultPermissions.Add(new AccessControlEntry(authenticatedUser, 3575, 0));
        FeedIdentity feedId = new FeedIdentity(project?.Id, Guid.NewGuid());
        if (feedPermission == null)
        {
          defaultPermissions.Add(new AccessControlEntry(IdentityHelper.CreateDescriptorFromSid(GroupWellKnownSidConstants.EveryoneGroupSid), 32, 0));
        }
        else
        {
          FeedPermissionsController.ValidatePermissions(feedPermission, false);
          defaultPermissions.AddRange(SecurityConverter.Convert(FeedPermissionsController.GetAccessControlEntriesToUpdate(feedPermission)));
        }
        this.ValidateAllowUpstreamNameConflictValue(requestContext, allowUpstreamNameConflict);
        if (allowUpstreamNameConflict && !upstreamEnabled)
          throw new ArgumentException(Resources.Error_FeedUpstreamConflictsEnabledWithoutUpstreamEnabled((object) upstreamEnabled, (object) allowUpstreamNameConflict));
        if (upstreamSources != null)
        {
          foreach (UpstreamSource upstreamSource in upstreamSources.Where<UpstreamSource>((Func<UpstreamSource, bool>) (x => x.Id == Guid.Empty)))
            upstreamSource.Id = Guid.NewGuid();
        }
        await new FeedSourcesValidator(requestContext, (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null, project).ValidateSourcesAsync(upstreamSources);
        FeedCapabilities capabilities = FeedCapabilities.UpstreamV2;
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
        using (FeedSqlResourceComponent component = requestContext.CreateComponent<FeedSqlResourceComponent>())
          feed1 = component.CreateFeed(feedId, feedName, feedDescription, upstreamEnabled, allowUpstreamNameConflict, hideDeletedPackageVersions.GetValueOrDefault(), upstreamSources, capabilities, badgesEnabled.GetValueOrDefault());
        FeedCore withoutCallingGetFeed = ProjectFeedsConversionHelper.ExplicitlyCreateFeedCoreWithoutCallingGetFeed(feedId);
        FeedSecurityHelper.SetFeedPermissions(requestContext, withoutCallingGetFeed, (IEnumerable<AccessControlEntry>) defaultPermissions);
        FeedChangedPublisher.CreatedFeed(requestContext, feed1);
        FeedView feedView = this.CreateDefaultReleaseViews(requestContext, feed1).Single<FeedView>((Func<FeedView, bool>) (v => v.Name.Equals("Local", StringComparison.OrdinalIgnoreCase)));
        using (FeedSqlResourceComponent component = requestContext.CreateComponent<FeedSqlResourceComponent>())
        {
          FeedSqlResourceComponent resourceComponent = component;
          FeedIdentity feedId1 = feedId;
          Guid? nullable = new Guid?(feedView.Id);
          bool? upstreamEnabled1 = new bool?();
          bool? allowUpstreamNameConflict1 = new bool?();
          bool? hideDeletedPackageVersions1 = new bool?();
          Guid? defaultReaderViewId = nullable;
          bool? badgesEnabled1 = new bool?();
          feed1 = resourceComponent.UpdateFeed(feedId1, (string) null, upstreamEnabled1, allowUpstreamNameConflict1, hideDeletedPackageVersions1, defaultReaderViewId, (IList<UpstreamSource>) null, badgesEnabled1);
        }
        FeedChangedPublisher.UpdatedFeed(requestContext, feed1);
        ProjectHelper.HydrateProjectReference(feed1, project);
        List<FeedPermission> permissions = new List<FeedPermission>();
        if (authenticatedUser.IdentityType != "System:ServicePrincipal")
          permissions.Add(new FeedPermission()
          {
            Role = FeedRole.Administrator,
            IdentityDescriptor = authenticatedUser
          });
        if (feedPermission != null)
          permissions.AddRange(feedPermission);
        this.InvalidateMemoryCache(requestContext);
        FeedChangedPublisher.FeedPermissionChanged(requestContext, feed1, permissions);
        FeedCiPublisher.PublishCreateFeedPermissionEvent(requestContext, feedId.Id, (IEnumerable<FeedPermission>) permissions);
        this.ClearAllowUpstreamNameConflictIfFeatureDisabled(requestContext, feed1);
        FeedCiPublisher.PublishCreateFeedEvent(requestContext, feed1);
        FeedAuditHelper.AuditCreateFeed(requestContext, feed1);
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed2 = feed1;
        defaultPermissions = (List<AccessControlEntry>) null;
        authenticatedUser = (IdentityDescriptor) null;
        feedId = (FeedIdentity) null;
        return feed2;
      }), nameof (CreateFeed));
    }

    private async Task ValidateServiceEndpointIds(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project,
      IList<UpstreamSource> upstreamSources)
    {
      IServiceEndpointService2 serviceEndpointService;
      List<Guid> endpointIds;
      if (upstreamSources == null)
      {
        serviceEndpointService = (IServiceEndpointService2) null;
        endpointIds = (List<Guid>) null;
      }
      else if (!upstreamSources.Any<UpstreamSource>())
      {
        serviceEndpointService = (IServiceEndpointService2) null;
        endpointIds = (List<Guid>) null;
      }
      else
      {
        IEnumerable<UpstreamSource> source = upstreamSources.Where<UpstreamSource>((Func<UpstreamSource, bool>) (u => u.ServiceEndpointId.HasValue));
        if (!source.Any<UpstreamSource>())
        {
          serviceEndpointService = (IServiceEndpointService2) null;
          endpointIds = (List<Guid>) null;
        }
        else
        {
          serviceEndpointService = requestContext.IsFeatureEnabled("Packaging.Feed.Upstreams.SupportServiceEndpoint") ? requestContext.GetService<IServiceEndpointService2>() : throw new ServiceConnectionNotSupportedException(Resources.ServiceEndpointNotSupportedText());
          endpointIds = source.Select<UpstreamSource, Guid>((Func<UpstreamSource, Guid>) (x => x.ServiceEndpointId.Value)).ToList<Guid>();
          foreach (UpstreamSource upstreamSource in source)
          {
            UpstreamSource upstream = upstreamSource;
            Guid? endpointProjectId = upstream.ServiceEndpointProjectId;
            if (!endpointProjectId.HasValue)
              throw new ServiceConnectionNotSupportedException(Resources.UpstreamServiceEndpointProjectMissing((object) upstream.Location));
            IServiceEndpointService2 endpointService2_1 = serviceEndpointService;
            IVssRequestContext requestContext1 = requestContext;
            endpointProjectId = upstream.ServiceEndpointProjectId;
            Guid scopeIdentifier1 = endpointProjectId.Value;
            List<Guid> endpointIds1 = endpointIds;
            ServiceEndpoint serviceEndpoint1 = endpointService2_1.QueryServiceEndpoints(requestContext1, scopeIdentifier1, "artifacts-upstream", (IEnumerable<string>) null, (IEnumerable<Guid>) endpointIds1, (string) null, true, true).Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => x.Id == upstream.ServiceEndpointId.Value)).FirstOrDefault<ServiceEndpoint>();
            if (serviceEndpoint1 == null)
              throw new ServiceConnectionNotSupportedException(Resources.ServiceEndpointNotFoundText());
            IVssRequestContext vssRequestContext = requestContext.Elevate();
            IServiceEndpointService2 endpointService2_2 = serviceEndpointService;
            IVssRequestContext requestContext2 = vssRequestContext;
            endpointProjectId = upstream.ServiceEndpointProjectId;
            Guid scopeIdentifier2 = endpointProjectId.Value;
            ServiceEndpoint serviceEndpoint2 = endpointService2_2.QueryServiceEndpoints(requestContext2, scopeIdentifier2, "artifacts-upstream", (IEnumerable<string>) null, (IEnumerable<Guid>) new List<Guid>()
            {
              serviceEndpoint1.Id
            }, (string) null, true, true).FirstOrDefault<ServiceEndpoint>();
            if (serviceEndpoint2 == null)
              throw new ServiceConnectionNotSupportedException(Resources.ServiceEndpointNotFoundText());
            await InternalSourceValidator.ValidateServiceEndpointMatchesUpstream(requestContext, upstream, serviceEndpoint2);
          }
          serviceEndpointService = (IServiceEndpointService2) null;
          endpointIds = (List<Guid>) null;
        }
      }
    }

    public IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeeds(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project,
      FeedRole feedRole = FeedRole.Reader,
      bool includeDeletedUpstreams = false)
    {
      return (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) requestContext.TraceBlock<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>(10019012, 10019013, 10019014, "Feed", "Service", (Func<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>) (() =>
      {
        FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, project);
        IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feedsInternal = this.GetFeedsInternal(requestContext, project, includeDeletedUpstreams);
        int permissions;
        switch (feedRole)
        {
          case FeedRole.Contributor:
            permissions = 3296;
            break;
          case FeedRole.Administrator:
            permissions = 3575;
            break;
          case FeedRole.Collaborator:
            permissions = 2080;
            break;
          default:
            permissions = 32;
            break;
        }
        List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> first = new List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();
        foreach (Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed in feedsInternal)
        {
          if (FeedSecurityHelper.HasFeedPermissions(requestContext, (FeedCore) feed, permissions, true))
            first.Add(feed);
        }
        if (project != (Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference) null)
        {
          first.ForEach((Action<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (x => ProjectHelper.HydrateProjectReference(x, project)));
        }
        else
        {
          IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> filteredOutFeeds;
          ProjectHelper.HydrateProjectReferences(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) first, out filteredOutFeeds);
          first = first.Except<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(filteredOutFeeds).ToList<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();
        }
        first.ForEach((Action<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (f => this.ClearAllowUpstreamNameConflictIfFeatureDisabled(requestContext, f)));
        first.ForEach((Action<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (f => this.DisableInvalidWellKnownUpstreams((FeedCore) f)));
        first.ForEach((Action<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (f => this.DisableUpstreamSourcesForPublicFeeds(requestContext, f)));
        first.Sort((Comparison<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) ((list1, list2) => list1.Name.CompareTo(list2.Name)));
        return first;
      }), nameof (GetFeeds));
    }

    public Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      IVssRequestContext requestContext,
      string feedId,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project = null,
      bool includeDeleted = false,
      bool includeDeletedUpstreams = false)
    {
      return requestContext.TraceBlock<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(10019012, 10019013, 10019014, "Feed", "Service", (Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (() =>
      {
        feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
        FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, project);
        string viewName;
        feedId = FeedViewsHelper.ParseFeedNameParameter(feedId, out viewName);
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedInternal;
        try
        {
          feedInternal = this.GetFeedInternal(requestContext, feedId, project, includeDeleted, includeDeletedUpstreams);
        }
        catch (DataspaceNotFoundException ex)
        {
          requestContext.TraceException(10019014, "Feed", "Service", (Exception) ex);
          throw FeedIdNotFoundException.Create(feedId);
        }
        if (feedInternal == null)
          throw FeedIdNotFoundException.Create(feedId);
        if (viewName != null)
          feedInternal.View = requestContext.GetService<IFeedViewService>().GetView(requestContext, feedInternal, viewName);
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feedInternal, true);
        ProjectHelper.HydrateProjectReference(feedInternal, project);
        this.ClearAllowUpstreamNameConflictIfFeatureDisabled(requestContext, feedInternal);
        this.DisableUpstreamSourcesForPublicFeeds(requestContext, feedInternal);
        return feedInternal;
      }), nameof (GetFeed));
    }

    public Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedById(
      IVssRequestContext requestContext,
      Guid feedId,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project,
      bool includeDeletedUpstreams = false)
    {
      return requestContext.TraceBlock<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(10019109, 10019110, 10019111, "Feed", "Service", (Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (() =>
      {
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedByFeedGuid = this.GetFeedByFeedGuid(requestContext, feedId, project, includeDeletedUpstreams);
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feedByFeedGuid, true);
        this.ClearAllowUpstreamNameConflictIfFeatureDisabled(requestContext, feedByFeedGuid);
        this.DisableUpstreamSourcesForPublicFeeds(requestContext, feedByFeedGuid);
        return feedByFeedGuid;
      }), nameof (GetFeedById));
    }

    public Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedByIdForAnyScope(
      IVssRequestContext requestContext,
      Guid feedId,
      bool includeSoftDeletedFeeds = false)
    {
      return requestContext.TraceBlock<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(10019121, 10019122, 10019123, "Feed", "Service", (Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (() =>
      {
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedByIdForAnyScope = requestContext.GetService<IFeedServiceFeedCache>().GetFeedByIdForAnyScope(requestContext, feedId);
        if (feedByIdForAnyScope == null & includeSoftDeletedFeeds)
        {
          using (FeedSqlResourceComponent component = requestContext.CreateComponent<FeedSqlResourceComponent>())
            feedByIdForAnyScope = component.GetFeedByIdForAnyScope(feedId, true);
        }
        if (feedByIdForAnyScope == null)
          throw FeedIdNotFoundException.Create(feedId.ToString());
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feedByIdForAnyScope, true);
        IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> filteredOutFeeds;
        ProjectHelper.HydrateProjectReferences(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) new Microsoft.VisualStudio.Services.Feed.WebApi.Feed[1]
        {
          feedByIdForAnyScope
        }, out filteredOutFeeds);
        if (filteredOutFeeds.Any<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>())
          throw FeedIdNotFoundException.Create(feedId.ToString());
        this.ClearAllowUpstreamNameConflictIfFeatureDisabled(requestContext, feedByIdForAnyScope);
        this.DisableUpstreamSourcesForPublicFeeds(requestContext, feedByIdForAnyScope);
        return feedByIdForAnyScope;
      }), nameof (GetFeedByIdForAnyScope));
    }

    public FeedInternalState GetFeedInternalState(
      IVssRequestContext requestContext,
      Guid feedId,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project = null)
    {
      return requestContext.TraceBlock<FeedInternalState>(10019099, 10019100, 10019101, "Feed", "Service", (Func<FeedInternalState>) (() =>
      {
        FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, project);
        FeedInternalState feedInternalState = (FeedInternalState) null;
        using (FeedSqlResourceComponent component = requestContext.CreateComponent<FeedSqlResourceComponent>())
          feedInternalState = component.GetInternalState(feedId, project?.Id);
        if (feedInternalState != null)
          FeedSecurityHelper.CheckReadFeedPermissions(requestContext, new FeedCore()
          {
            Id = feedInternalState.FeedId,
            Project = project
          }, true);
        return feedInternalState;
      }), nameof (GetFeedInternalState));
    }

    public async Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> UpdateFeed(
      IVssRequestContext requestContext,
      string feedId,
      FeedUpdate updatedFeed,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project = null)
    {
      FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, project);
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = await this.UpdateFeed(requestContext, feedId, updatedFeed, (IFeedMessageBus) new FeedMessageBus(), project);
      ProjectHelper.HydrateProjectReference(feed, project);
      this.ClearAllowUpstreamNameConflictIfFeatureDisabled(requestContext, feed);
      return feed;
    }

    public void DeleteFeed(
      IVssRequestContext requestContext,
      string feedId,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project)
    {
      this.DeleteFeed(requestContext, feedId, project, (IFeedMessageBus) new FeedMessageBus());
    }

    internal async Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> UpdateFeed(
      IVssRequestContext requestContext,
      string feedId,
      FeedUpdate updatedFeed,
      IFeedMessageBus messageBus,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project = null)
    {
      return await requestContext.TraceBlock<Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>(10019018, 10019019, 10019020, "Feed", "Service", (Func<Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>) (async () =>
      {
        feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed currentFeed = this.GetFeedInternal(requestContext, feedId, project);
        if (currentFeed == null)
          throw FeedIdNotFoundException.Create(feedId);
        if (updatedFeed.Id != Guid.Empty && updatedFeed.Id != currentFeed.Id)
          throw new ArgumentException(Resources.Error_FeedIdMismatch((object) updatedFeed.Id));
        FeedIdentity currentFeedId = currentFeed.GetIdentity();
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedToReturn = (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
        bool invalidateCrossServiceCache = false;
        bool invalidateFeedFromMemoryCache = false;
        if (updatedFeed.Name != null)
        {
          FeedSecurityHelper.CheckEditFeedPermissions(requestContext, (FeedCore) currentFeed);
          FeedService.ValidateFeedName(updatedFeed.Name);
          using (FeedSqlResourceComponent component = requestContext.CreateComponent<FeedSqlResourceComponent>())
            feedToReturn = component.RenameFeed(currentFeedId, updatedFeed.Name);
          invalidateCrossServiceCache = true;
        }
        this.ValidateAllowUpstreamNameConflictValue(requestContext, updatedFeed.AllowUpstreamNameConflict.GetValueOrDefault());
        bool? nullable = updatedFeed.UpstreamEnabled;
        bool upstreamEnabled = ((int) nullable ?? (currentFeed.UpstreamEnabled ? 1 : 0)) != 0;
        this.ValidateAllowUpstreamForProjectFeed(requestContext, upstreamEnabled, project);
        nullable = updatedFeed.AllowUpstreamNameConflict;
        if (nullable.GetValueOrDefault() && !upstreamEnabled)
          throw new ArgumentException(Resources.Error_FeedUpstreamConflictsEnabledWithoutUpstreamEnabled((object) upstreamEnabled, (object) updatedFeed.AllowUpstreamNameConflict));
        if (updatedFeed.UpstreamSources != null)
        {
          foreach (UpstreamSource upstreamSource in updatedFeed.UpstreamSources.Where<UpstreamSource>((Func<UpstreamSource, bool>) (x => x.Id == Guid.Empty)))
            upstreamSource.Id = Guid.NewGuid();
          await this.ValidateServiceEndpointIds(requestContext, project, updatedFeed.UpstreamSources);
          await new FeedSourcesValidator(requestContext, currentFeed, project).ValidateSourcesAsync(updatedFeed.UpstreamSources);
        }
        if (updatedFeed.Description == null)
        {
          nullable = updatedFeed.UpstreamEnabled;
          if (!nullable.HasValue)
          {
            nullable = updatedFeed.AllowUpstreamNameConflict;
            if (!nullable.HasValue)
            {
              nullable = updatedFeed.HideDeletedPackageVersions;
              if (!nullable.HasValue && !updatedFeed.DefaultViewId.HasValue && updatedFeed.UpstreamSources == null)
              {
                nullable = updatedFeed.BadgesEnabled;
                if (!nullable.HasValue)
                  goto label_35;
              }
            }
          }
        }
        FeedSecurityHelper.CheckEditFeedPermissions(requestContext, (FeedCore) currentFeed);
        FeedService.ValidateFeedDescription(updatedFeed.Description);
        using (FeedSqlResourceComponent component = requestContext.CreateComponent<FeedSqlResourceComponent>())
          feedToReturn = component.UpdateFeed(currentFeedId, updatedFeed.Description, updatedFeed.UpstreamEnabled, updatedFeed.AllowUpstreamNameConflict, updatedFeed.HideDeletedPackageVersions, updatedFeed.DefaultViewId, updatedFeed.UpstreamSources, updatedFeed.BadgesEnabled);
        if (updatedFeed.UpstreamEnabled.HasValue || updatedFeed.AllowUpstreamNameConflict.HasValue || updatedFeed.UpstreamSources != null)
        {
          invalidateCrossServiceCache = true;
          invalidateFeedFromMemoryCache = true;
        }
label_35:
        FeedChangedPublisher.UpdatedFeed(requestContext, feedToReturn);
        this.InvalidateCaches(requestContext, invalidateCrossServiceCache, currentFeedId.Id, messageBus, invalidateFeedFromMemoryCache);
        if (feedToReturn == null)
          return this.GetFeed(requestContext, feedId, project, false, false);
        FeedCiPublisher.PublishUpdateFeedEvent(requestContext, feedToReturn);
        FeedAuditHelper.AuditModifyFeed(requestContext, currentFeed, feedToReturn);
        return feedToReturn;
      }), nameof (UpdateFeed));
    }

    internal void DeleteFeed(
      IVssRequestContext requestContext,
      string feedId,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project,
      IFeedMessageBus messageBus)
    {
      requestContext.TraceBlock(10019021, 10019022, 10019023, "Feed", "Service", (Action) (() =>
      {
        FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, project);
        feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = this.GetFeed(requestContext, feedId, project, false, false);
        FeedSecurityHelper.CheckDeleteFeedPermissions(requestContext, (FeedCore) feed);
        using (FeedSqlResourceComponent component = requestContext.CreateComponent<FeedSqlResourceComponent>())
          component.DeleteFeed(feed.GetIdentity());
        FeedChangedPublisher.DeletedFeed(requestContext, feed);
        this.InvalidateCaches(requestContext, true, feed.Id, messageBus);
        FeedCiPublisher.PublishDeleteFeedEvent(requestContext, feed.Id);
        FeedAuditHelper.AuditSoftDeleteFeed(requestContext, feed);
      }), nameof (DeleteFeed));
    }

    public void DeleteAllFeedsInProject(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project) => requestContext.TraceBlock(10019028, 10019029, 10019030, "Feed", "Service", (Action) (() =>
    {
      IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> collection = requestContext.IsSystemContext ? this.GetFeedsInternal(requestContext, project) : throw new UnauthorizedRequestException(requestContext.GetUserId());
      IFeedRecycleBinService recycleBinService = requestContext.GetService<IFeedRecycleBinService>();
      Action<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> action = (Action<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (orphanedFeed =>
      {
        try
        {
          this.DeleteFeed(requestContext, orphanedFeed.Id.ToString(), project);
          recycleBinService.PermanentDeleteFeed(requestContext, project, orphanedFeed.Id.ToString());
        }
        catch
        {
        }
      });
      collection.ForEach<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(action);
    }), nameof (DeleteAllFeedsInProject));

    private static void ValidateFeedDescription(string feedDescription)
    {
      if (feedDescription != null && feedDescription.Length > (int) byte.MaxValue)
        throw new InvalidFeedDescriptionException(Resources.Error_FeedDescriptionExcedesMaximumLength((object) feedDescription, (object) (int) byte.MaxValue));
    }

    private static void ValidateFeedName(string name)
    {
      if (name.Length > 64)
        throw new InvalidFeedNameException(Resources.Error_FeedNameExceedsMaximumLength((object) name, (object) 64));
      if (Guid.TryParse(name, out Guid _))
        throw new InvalidFeedNameException(Resources.Error_FeedNameIsGuid((object) name));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (name.Any<char>(FeedService.\u003C\u003EO.\u003C0\u003E__IsWhiteSpace ?? (FeedService.\u003C\u003EO.\u003C0\u003E__IsWhiteSpace = new Func<char, bool>(char.IsWhiteSpace))))
        throw new InvalidFeedNameException(Resources.Error_FeedNameContainsWhitespace((object) name));
      if (FeedService.IsReservedFeedName(name))
        throw new InvalidFeedNameException(Resources.Error_FeedNameIsReserved((object) name));
      string errorMessage;
      if (!new NameValidator().IsValidName(name, out errorMessage))
        throw new InvalidFeedNameException(Resources.Error_FeedNameIsInvalid((object) name, (object) errorMessage));
    }

    private static bool IsReservedFeedName(string name) => ((IEnumerable<string>) FeedService.ReservedFeedNames).Any<string>((Func<string, bool>) (x => x.Equals(name, StringComparison.InvariantCultureIgnoreCase)));

    private static Microsoft.VisualStudio.Services.Feed.WebApi.Feed TryLoadingDeletedFeedByFeedName(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project,
      string feedName)
    {
      using (FeedSqlResourceComponent component = requestContext.CreateComponent<FeedSqlResourceComponent>())
        return component.GetFeed(feedName, project?.Id, true);
    }

    private Guid GetFeedGuidFromFeedId(IVssRequestContext requestContext, string feedId)
    {
      Guid result;
      if (Guid.TryParse(feedId, out result))
        return result;
      return (this.GetFeedByFeedName(requestContext, feedId, (Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference) null) ?? throw FeedIdNotFoundException.Create(feedId)).Id;
    }

    private Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedInternal(
      IVssRequestContext requestContext,
      string feedId,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project,
      bool includeDeleted = false,
      bool includeDeletedUpstreams = false)
    {
      Guid result;
      return Guid.TryParse(feedId, out result) ? this.GetFeedByFeedGuid(requestContext, result, project, includeDeletedUpstreams) : this.GetFeedByFeedName(requestContext, feedId, project, includeDeleted, includeDeletedUpstreams);
    }

    private IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedsInternal(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project,
      bool includeDeletedUpstreams = false)
    {
      return requestContext.GetService<IFeedServiceFeedCache>().GetFeeds(requestContext, project, includeDeletedUpstreams);
    }

    private Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedByFeedGuid(
      IVssRequestContext requestContext,
      Guid feedId,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project,
      bool includeDeletedUpstreams = false)
    {
      return requestContext.GetService<IFeedServiceFeedCache>().GetFeed(requestContext, feedId, project, includeDeletedUpstreams);
    }

    private Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedByFeedName(
      IVssRequestContext requestContext,
      string feedName,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project,
      bool includeDeleted = false,
      bool includeDeletedUpstreams = false)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedByFeedName = requestContext.GetService<IFeedServiceFeedCache>().GetFeed(requestContext, feedName, project, includeDeletedUpstreams);
      if (feedByFeedName == null & includeDeleted)
        feedByFeedName = FeedService.TryLoadingDeletedFeedByFeedName(requestContext, project, feedName);
      return feedByFeedName;
    }

    private void InvalidateMemoryCache(IVssRequestContext requestContext) => requestContext.GetService<IFeedServiceFeedCache>().Invalidate(requestContext);

    private void InvalidateFeedFromMemoryCache(IVssRequestContext requestContext, Guid feedId) => requestContext.GetService<IFeedServiceFeedCache>().Invalidate(requestContext, feedId);

    private void ClearAllowUpstreamNameConflictIfFeatureDisabled(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (requestContext.IsFeatureEnabled("Packaging.Feed.Npm.AllowUpstreamNameConflict"))
        return;
      feed.AllowUpstreamNameConflict = false;
    }

    private void DisableUpstreamSourcesForPublicFeeds(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (feed.Project?.Visibility != "Public" || feed.UpstreamSources == null)
        return;
      bool flag1 = (requestContext.IsFeatureEnabled("Packaging.Feed.UpstreamsAllowedForPublicFeeds") ? 1 : (!requestContext.IsFeatureEnabled("Packaging.Feed.UpstreamsAllowedForPublicFeeds.MSFT") ? 0 : (requestContext.IsMicrosoftTenant() ? 1 : 0))) == 0;
      foreach (UpstreamSource upstreamSource in (IEnumerable<UpstreamSource>) feed.UpstreamSources)
      {
        bool flag2 = upstreamSource.UpstreamSourceType == UpstreamSourceType.Public && (object) WellKnownSourceProvider.Instance.GetWellKnownSourceOrDefault(upstreamSource.Location) == null;
        if (flag1 | flag2)
        {
          upstreamSource.Status = UpstreamStatus.Disabled;
          IEnumerable<UpstreamStatusDetail> statusDetails = upstreamSource.StatusDetails;
          List<UpstreamStatusDetail> upstreamStatusDetailList = (statusDetails != null ? statusDetails.ToList<UpstreamStatusDetail>() : (List<UpstreamStatusDetail>) null) ?? new List<UpstreamStatusDetail>();
          upstreamStatusDetailList.Add(new UpstreamStatusDetail()
          {
            Reason = Resources.Error_UpstreamFeedNotAllowedForPublicFeed()
          });
          upstreamSource.StatusDetails = (IEnumerable<UpstreamStatusDetail>) upstreamStatusDetailList;
        }
      }
    }

    private void DisableInvalidWellKnownUpstreams(FeedCore feed)
    {
      if (feed.UpstreamSources == null)
        return;
      foreach (UpstreamSource upstreamSource1 in (IEnumerable<UpstreamSource>) feed.UpstreamSources)
      {
        WellKnownUpstreamSource knownSourceOrDefault = WellKnownSourceProvider.Instance.GetWellKnownSourceOrDefault(upstreamSource1.Location);
        if ((object) knownSourceOrDefault != null)
        {
          (UpstreamStatus Status, IEnumerable<UpstreamStatusDetail> StatusDetails) = knownSourceOrDefault.ValidateUpstreamSourceMatches(upstreamSource1);
          IEnumerable<UpstreamStatusDetail> second = StatusDetails;
          if (Status != UpstreamStatus.Ok)
          {
            upstreamSource1.Status = Status;
            UpstreamSource upstreamSource2 = upstreamSource1;
            IEnumerable<UpstreamStatusDetail> statusDetails = upstreamSource1.StatusDetails;
            IEnumerable<UpstreamStatusDetail> upstreamStatusDetails = statusDetails != null ? statusDetails.Concat<UpstreamStatusDetail>(second) : (IEnumerable<UpstreamStatusDetail>) null;
            if (upstreamStatusDetails == null)
            {
              StatusDetails = second;
              upstreamStatusDetails = StatusDetails;
            }
            upstreamSource2.StatusDetails = upstreamStatusDetails;
          }
        }
      }
    }

    private void InvalidateCaches(
      IVssRequestContext requestContext,
      bool invalidateCrossServiceCache,
      Guid feedId = default (Guid),
      IFeedMessageBus messageBus = null,
      bool invalidateFeedFromMemoryCache = false)
    {
      if (invalidateCrossServiceCache)
      {
        ArgumentUtility.CheckForNull<IFeedMessageBus>(messageBus, nameof (messageBus));
        ArgumentUtility.CheckForEmptyGuid(feedId, nameof (feedId));
        messageBus.SendCachedFeedNoLongerValidNotification(requestContext, feedId);
      }
      this.InvalidateMemoryCache(requestContext);
      if (!invalidateFeedFromMemoryCache)
        return;
      this.InvalidateFeedFromMemoryCache(requestContext, feedId);
    }

    private FeedView CreateFeedViewInternal(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedView view)
    {
      Guid viewId = Guid.NewGuid();
      FeedView feedView;
      using (FeedViewSqlResourceComponent component = requestContext.CreateComponent<FeedViewSqlResourceComponent>())
        feedView = component.CreateFeedView(feed.GetIdentity(), viewId, view.Name, view.Type, FeedVisibility.Private);
      FeedChangedPublisher.CreatedView(requestContext, feed, feedView);
      requestContext.GetService<IFeedServiceFeedViewCache2>().GetFeedView(requestContext, feed, feedView.Id);
      return feedView;
    }

    private IEnumerable<FeedView> CreateDefaultReleaseViews(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      return (IEnumerable<FeedView>) ((IEnumerable<FeedView>) new FeedView[3]
      {
        this.CreateFeedViewInternal(requestContext, feed, new FeedView()
        {
          Name = "Prerelease",
          Type = FeedViewType.Release
        }),
        this.CreateFeedViewInternal(requestContext, feed, new FeedView()
        {
          Name = "Release",
          Type = FeedViewType.Release
        }),
        this.CreateFeedViewInternal(requestContext, feed, new FeedView()
        {
          Name = "Local",
          Type = FeedViewType.Implicit
        })
      }).OrderBy<FeedView, string>((Func<FeedView, string>) (v => v.Name)).ToList<FeedView>();
    }

    private void ValidateAllowUpstreamNameConflictValue(
      IVssRequestContext requestContext,
      bool allowUpstreamNameConflict)
    {
      if (allowUpstreamNameConflict && !requestContext.IsFeatureEnabled("Packaging.Feed.Npm.AllowUpstreamNameConflict"))
        throw new FeatureDisabledException("Packaging.Feed.Npm.AllowUpstreamNameConflict");
    }

    private void ValidateAllowUpstreamForProjectFeed(
      IVssRequestContext requestContext,
      bool upstreamEnabled,
      Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference project)
    {
      if (!upstreamEnabled || !(project != (Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference) null))
        return;
      if (!requestContext.IsFeatureEnabled("Packaging.Feed.ProjectScopedUpstreams"))
        throw new InvalidUserInputException(Resources.Error_UpstreamFeedNotAllowedForProjectScopedFeed());
      bool flag = requestContext.IsFeatureEnabled("Packaging.Feed.UpstreamsAllowedForPublicFeeds.MSFT") && requestContext.IsMicrosoftTenant();
      if (project?.Visibility == "Public" && !(requestContext.IsFeatureEnabled("Packaging.Feed.UpstreamsAllowedForPublicFeeds") | flag))
        throw new InvalidUserInputException(Resources.Error_UpstreamFeedNotAllowedForPublicFeed());
    }

    private void OnProjectDeleted(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      ProjectInfo projectInfo;
      if (!JsonUtilities.TryDeserialize<ProjectInfo>(args.Data, out projectInfo, true))
      {
        requestContext.Trace(10019124, TraceLevel.Error, "Feed", "CacheService", "Invalid ProjectInfo: " + (args.Data ?? string.Empty));
      }
      else
      {
        if (projectInfo.State != ProjectState.Deleted)
          return;
        this.DeleteAllFeedsInProject(requestContext, new Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference()
        {
          Id = projectInfo.Id,
          Name = projectInfo.Name,
          Visibility = projectInfo.Visibility.ToString()
        });
      }
    }
  }
}
