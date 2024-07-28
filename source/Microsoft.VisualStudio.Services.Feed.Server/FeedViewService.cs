// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedViewService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.MessageBus;
using Microsoft.VisualStudio.Services.Feed.Server.Telemetry;
using Microsoft.VisualStudio.Services.Feed.Server.Utils;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedViewService : IFeedViewService, IVssFrameworkService
  {
    public static readonly string[] ReservedViewNames = new string[1]
    {
      "Local"
    };
    private readonly FeedLocalSecurityInvalidationFacade localSecurityInvalidationFacade;

    public FeedViewService()
      : this(new FeedLocalSecurityInvalidationFacade())
    {
    }

    public FeedViewService(
      FeedLocalSecurityInvalidationFacade localSecurityInvalidationFacade)
    {
      this.localSecurityInvalidationFacade = localSecurityInvalidationFacade;
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.TraceBlock(10019000, 10019001, 10019002, "Feed", "Service", (Action) (() =>
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }), "ServiceStart");

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => requestContext.TraceBlock(10019003, 10019004, 10019005, "Feed", "Service", (Action) (() => ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext))), "ServiceEnd");

    public FeedView CreateView(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, FeedView view) => requestContext.TraceBlock<FeedView>(10019066, 10019067, 10019068, "Feed", "Service", (Func<FeedView>) (() =>
    {
      FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, feed.Project);
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      view.Name.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create("Name")));
      FeedViewService.ValidateViewName(view.Name);
      FeedViewService.ValidateViewType(view.Type);
      this.ValidateUserOwned(view, Resources.Error_ImplicitViewCannotBeCreated());
      if (!view.Visibility.HasValue)
        view.Visibility = new FeedVisibility?(FeedVisibility.Private);
      this.ValidateViewVisibilityForProjectFeed(requestContext, view.Visibility, feed.Project);
      FeedSecurityHelper.CheckEditFeedPermissions(requestContext, (FeedCore) feed);
      FeedView feedViewInternal = this.CreateFeedViewInternal(requestContext, feed, view);
      FeedCiPublisher.PublishCreateFeedViewEvent(requestContext, feed, view);
      FeedChangedPublisher.CreatedView(requestContext, feed, feedViewInternal);
      this.InvalidateAllViewsFromFeedInFeedViewCache(requestContext, feed.Id, feed.Project?.Id);
      this.InvalidateSystemStore(requestContext);
      if (view.Type == FeedViewType.Release)
        FeedAuditHelper.AuditCreateFeedView(requestContext, feed, feedViewInternal);
      return feedViewInternal;
    }), nameof (CreateView));

    public FeedView GetView(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, string viewId) => requestContext.TraceBlock<FeedView>(10019072, 10019073, 10019074, "Feed", "Service", (Func<FeedView>) (() =>
    {
      FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, feed.Project);
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      viewId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (viewId))));
      FeedView feedViewInternal = this.GetFeedViewInternal(requestContext, feed, viewId);
      if (feedViewInternal == null)
        throw FeedViewNotFoundException.Create(viewId);
      FeedCore withoutCallingGetFeed = ProjectFeedsConversionHelper.ExplicitlyCreateFeedCoreWithoutCallingGetFeed(feed.GetIdentity());
      withoutCallingGetFeed.View = feedViewInternal;
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, withoutCallingGetFeed);
      return FeedViewService.AdjustViewVisibility(requestContext, feedViewInternal);
    }), nameof (GetView));

    public IEnumerable<FeedView> GetViews(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed) => requestContext.TraceBlock<IEnumerable<FeedView>>(10019069, 10019070, 10019071, "Feed", "Service", (Func<IEnumerable<FeedView>>) (() =>
    {
      FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, feed.Project);
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      feed.View = (FeedView) null;
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed, true);
      return this.GetViewsInternal(requestContext, feed).Select<FeedView, FeedView>((Func<FeedView, FeedView>) (view => FeedViewService.AdjustViewVisibility(requestContext, view)));
    }), nameof (GetViews));

    public FeedView UpdateView(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string viewId,
      FeedView updatedView)
    {
      return requestContext.TraceBlock<FeedView>(10019075, 10019076, 10019077, "Feed", "Service", (Func<FeedView>) (() =>
      {
        FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, feed.Project);
        feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
        updatedView.ThrowIfNull<FeedView>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (updatedView))));
        FeedSecurityHelper.CheckEditFeedPermissions(requestContext, (FeedCore) feed);
        FeedView feedViewInternal = this.GetFeedViewInternal(requestContext, feed, viewId);
        if (feedViewInternal == null)
          throw FeedViewNotFoundException.Create(viewId);
        this.ValidateViewVisibilityForProjectFeed(requestContext, feedViewInternal.Visibility, feed.Project);
        FeedView updatedView1 = feedViewInternal;
        if (feedViewInternal.Type.Equals((object) FeedViewType.Implicit) && updatedView.Name != null)
          throw InvalidUserInputException.Create(Resources.Error_ImplicitViewCannotBeRenamed());
        if (!feedViewInternal.Type.Equals((object) FeedViewType.Implicit) && updatedView.Name != null)
        {
          FeedViewService.ValidateViewName(updatedView.Name);
          using (FeedViewSqlResourceComponent component = requestContext.CreateComponent<FeedViewSqlResourceComponent>())
            updatedView1 = component.RenameFeedView(feed.GetIdentity(), feedViewInternal.Id, updatedView.Name);
        }
        this.InvalidateAllViewsFromFeedInFeedViewCache(requestContext, feed.Id, feed.Project?.Id);
        using (FeedViewSqlResourceComponent component = requestContext.CreateComponent<FeedViewSqlResourceComponent>())
          updatedView1 = component.UpdateFeedView(feed.GetIdentity(), feedViewInternal.Id, updatedView);
        this.InvalidateSystemStore(requestContext);
        FeedChangedPublisher.UpdatedView(requestContext, feed, updatedView1);
        FeedAuditHelper.AuditModifyFeedView(requestContext, feed, feedViewInternal, updatedView1);
        return updatedView1;
      }), nameof (UpdateView));
    }

    public void DeleteView(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, string viewId) => requestContext.TraceBlock(10019078, 10019079, 10019080, "Feed", "Service", (Action) (() =>
    {
      FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, feed.Project);
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      viewId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (viewId))));
      FeedSecurityHelper.CheckEditFeedPermissions(requestContext, (FeedCore) feed);
      FeedView userOwnedView = this.GetUserOwnedView(requestContext, feed, viewId, Resources.Error_ImplicitViewCannotBeDeleted());
      bool removedAtleastOneACL = FeedSecurityHelper.RemoveAllPermissionsForFeedView(requestContext, new FeedCore()
      {
        Id = feed.Id,
        Project = feed.Project,
        View = userOwnedView
      });
      FeedCiPublisher.PublishDeletedFeedViewPermissionsEvent(requestContext, feed, userOwnedView, removedAtleastOneACL);
      using (FeedViewSqlResourceComponent component = requestContext.CreateComponent<FeedViewSqlResourceComponent>())
        component.DeleteFeedView(feed.GetIdentity(), userOwnedView.Id);
      this.InvalidateAllViewsFromFeedInFeedViewCache(requestContext, feed.Id, feed.Project?.Id);
      this.InvalidateSystemStore(requestContext);
      FeedChangedPublisher.DeletedView(requestContext, feed, userOwnedView);
      FeedCiPublisher.PublishDeleteFeedViewEvent(requestContext, feed, userOwnedView);
      if (userOwnedView.Type != FeedViewType.Release)
        return;
      FeedAuditHelper.AuditDeleteFeedView(requestContext, feed, userOwnedView);
    }), nameof (DeleteView));

    private static void ValidateViewName(string name)
    {
      if (name.Length > 64)
        throw new InvalidFeedViewNameException(Resources.Error_FeedNameExceedsMaximumLength((object) name, (object) 64));
      if (Guid.TryParse(name, out Guid _))
        throw new InvalidFeedViewNameException(Resources.Error_FeedNameIsGuid((object) name));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (name.Any<char>(FeedViewService.\u003C\u003EO.\u003C0\u003E__IsWhiteSpace ?? (FeedViewService.\u003C\u003EO.\u003C0\u003E__IsWhiteSpace = new Func<char, bool>(char.IsWhiteSpace))))
        throw new InvalidFeedViewNameException(Resources.Error_FeedViewNameContainsWhitespace((object) name));
      string errorMessage;
      if (!new NameValidator().IsValidName(name, out errorMessage))
        throw new InvalidFeedViewNameException(Resources.Error_FeedViewNameIsInvalid((object) name, (object) errorMessage));
      if (FeedViewService.IsReservedViewName(name))
        throw new InvalidFeedViewNameException(Resources.Error_FeedViewNameIsInvalidReserved((object) name, (object) string.Join(", ", FeedViewService.ReservedViewNames)));
    }

    private static void ValidateViewType(FeedViewType viewType)
    {
      if (!Enum.IsDefined(typeof (FeedViewType), (object) viewType) || viewType.Equals((object) FeedViewType.None))
        throw InvalidUserInputException.Create("Type");
    }

    private static bool IsReservedViewName(string name) => ((IEnumerable<string>) FeedViewService.ReservedViewNames).Any<string>((Func<string, bool>) (x => x.Equals(name, StringComparison.InvariantCultureIgnoreCase)));

    private FeedView GetFeedViewInternal(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string viewId)
    {
      IFeedServiceFeedViewCache2 feedViewCache = FeedViewService.GetFeedViewCache(requestContext);
      Guid result;
      return Guid.TryParse(viewId, out result) ? feedViewCache.GetFeedView(requestContext, feed, result) : feedViewCache.GetFeedView(requestContext, feed, viewId);
    }

    private FeedView CreateFeedViewInternal(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedView view)
    {
      Guid viewId = Guid.NewGuid();
      using (FeedViewSqlResourceComponent component = requestContext.CreateComponent<FeedViewSqlResourceComponent>())
        return component.CreateFeedView(feed.GetIdentity(), viewId, view.Name, view.Type, view.Visibility.Value);
    }

    private FeedView GetUserOwnedView(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string viewId,
      string errorMessage)
    {
      FeedView feedViewInternal = this.GetFeedViewInternal(requestContext, feed, viewId);
      if (feedViewInternal == null)
        throw FeedViewNotFoundException.Create(viewId);
      this.ValidateUserOwned(feedViewInternal, errorMessage);
      return feedViewInternal;
    }

    private void ValidateUserOwned(FeedView view, string errorMessage)
    {
      if (FeedViewType.Implicit.Equals((object) view.Type))
        throw InvalidUserInputException.Create(errorMessage);
    }

    private FeedView GetFeedViewWithPermissions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedView feedView,
      int permissions)
    {
      FeedCore withoutCallingGetFeed = ProjectFeedsConversionHelper.ExplicitlyCreateFeedCoreWithoutCallingGetFeed(feed.GetIdentity());
      withoutCallingGetFeed.View = feedView;
      return FeedSecurityHelper.HasReadFeedPermissions(requestContext, withoutCallingGetFeed) ? feedView : (FeedView) null;
    }

    private void InvalidateSystemStore(IVssRequestContext requestContext) => this.localSecurityInvalidationFacade.InvalidateSystemStore(requestContext, nameof (FeedViewService));

    private void InvalidateAllViewsFromFeedInFeedViewCache(
      IVssRequestContext requestContext,
      Guid feedId,
      Guid? projectId)
    {
      requestContext.GetService<IFeedServiceFeedViewCache2>().InvalidateFeedViews(requestContext, feedId);
      requestContext.GetService<IFeedServiceFeedCache>().Invalidate(requestContext, feedId);
    }

    public IReadOnlyDictionary<Guid, IReadOnlyList<FeedView>> GetAllNonDeletedFeedViewsForCollection(
      IVssRequestContext requestContext)
    {
      requestContext.CheckSystemRequestContext();
      return FeedViewService.GetFeedViewCache(requestContext).GetAllNonDeletedViewsByFeedForCollection(requestContext);
    }

    public IReadOnlyDictionary<Guid, IReadOnlyList<FeedView>> GetFeedViewsNoSecurityCheck(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds)
    {
      requestContext.CheckSystemRequestContext();
      return FeedViewService.GetFeedViewCache(requestContext).GetFeedViews(requestContext, feeds);
    }

    private IEnumerable<FeedView> GetViewsInternal(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      IEnumerable<FeedView> feedViews = FeedViewService.GetFeedViewCache(requestContext).GetFeedViews(requestContext, feed);
      List<FeedView> viewsInternal = new List<FeedView>();
      foreach (FeedView feedView in feedViews)
      {
        FeedView viewWithPermissions = this.GetFeedViewWithPermissions(requestContext, feed, feedView, 32);
        if (viewWithPermissions != null)
          viewsInternal.Add(viewWithPermissions);
      }
      return (IEnumerable<FeedView>) viewsInternal;
    }

    private static IFeedServiceFeedViewCache2 GetFeedViewCache(IVssRequestContext requestContext) => requestContext.GetService<IFeedServiceFeedViewCache2>();

    private static FeedView AdjustViewVisibility(IVssRequestContext requestContext, FeedView view)
    {
      if (requestContext.IsFeatureEnabled("Packaging.Feed.DisableInboundInternalAadUpstreams"))
      {
        FeedVisibility? visibility = view.Visibility;
        FeedVisibility feedVisibility = FeedVisibility.Organization;
        if (visibility.GetValueOrDefault() == feedVisibility & visibility.HasValue)
          view.Visibility = new FeedVisibility?(FeedVisibility.Collection);
      }
      return view;
    }

    private void ValidateViewVisibilityForProjectFeed(
      IVssRequestContext requestContext,
      FeedVisibility? visibility,
      ProjectReference project)
    {
      if (requestContext.IsFeatureEnabled("Packaging.Feed.ProjectScopedUpstreams"))
        return;
      FeedVisibility? nullable = visibility;
      FeedVisibility feedVisibility = FeedVisibility.Private;
      if (!(nullable.GetValueOrDefault() == feedVisibility & nullable.HasValue) && project != (ProjectReference) null)
        throw new InvalidUserInputException(Resources.Error_NonPrivateViewsNotSupportedForProjectScopedFeeds());
    }
  }
}
