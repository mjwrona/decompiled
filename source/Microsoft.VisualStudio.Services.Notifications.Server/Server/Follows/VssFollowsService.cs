// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.Follows.VssFollowsService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server.Follows
{
  public class VssFollowsService : IVssFollowsService, IVssFrameworkService
  {
    private readonly ServiceFactory<IEventNotificationComponent> m_componentFactory;
    private readonly ServiceFactory<IDisposableReadOnlyList<IArtifactUriProvider>> m_artifactProviderFactory;
    private readonly ServiceFactory<IDisposableReadOnlyList<IFollowSecurityProvider>> m_securityProviderFactory;
    private IDisposableReadOnlyList<IArtifactUriProvider> m_artifactProviders;
    private IDisposableReadOnlyList<IFollowSecurityProvider> m_securityProviders;

    internal VssFollowsService(
      IEventNotificationComponent component,
      IDisposableReadOnlyList<IArtifactUriProvider> metadataProviders,
      IDisposableReadOnlyList<IFollowSecurityProvider> securityProviders)
    {
      this.m_componentFactory = (ServiceFactory<IEventNotificationComponent>) (context => component);
      this.m_artifactProviders = metadataProviders;
      this.m_securityProviders = securityProviders;
    }

    public VssFollowsService()
    {
      this.m_componentFactory = (ServiceFactory<IEventNotificationComponent>) (context => (IEventNotificationComponent) context.CreateComponent<EventNotificationComponent>());
      this.m_artifactProviderFactory = (ServiceFactory<IDisposableReadOnlyList<IArtifactUriProvider>>) (context => context.GetExtensions<IArtifactUriProvider>());
      this.m_securityProviderFactory = (ServiceFactory<IDisposableReadOnlyList<IFollowSecurityProvider>>) (context => context.GetExtensions<IFollowSecurityProvider>());
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_artifactProviders != null)
      {
        this.m_artifactProviders.Dispose();
        this.m_artifactProviders = (IDisposableReadOnlyList<IArtifactUriProvider>) null;
      }
      if (this.m_securityProviders == null)
        return;
      this.m_securityProviders.Dispose();
      this.m_securityProviders = (IDisposableReadOnlyList<IFollowSecurityProvider>) null;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_artifactProviders = this.m_artifactProviderFactory(systemRequestContext);
      this.m_securityProviders = this.m_securityProviderFactory(systemRequestContext);
    }

    public ArtifactSubscription Follow(
      IVssRequestContext requestContext,
      ArtifactSubscription artifact,
      Guid identityId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ArtifactSubscription>(artifact, nameof (artifact));
      if (!this.IsArtifactValid(artifact))
        throw new InvalidOperationException(CoreRes.MalformedArtifact());
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      return requestContext.TraceBlock<ArtifactSubscription>(15091000, 15091001, 15091002, "Follows", "FollowsService", (Func<ArtifactSubscription>) (() =>
      {
        if (!this.CheckCanFollowArtifact(requestContext, artifact, identityId))
          throw new UnauthorizedAccessException(CoreRes.UnauthorizedFollow((object) identityId, (object) artifact.ArtifactType, (object) artifact.ArtifactId));
        string artifactUri = this.GetArtifactUri(requestContext, artifact.ArtifactType, artifact.ArtifactId);
        INotificationSubscriptionService service = requestContext.GetService<INotificationSubscriptionService>();
        string subscriptionMetadata = this.GetSubscriptionMetadata(artifact);
        Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription1 = new Microsoft.VisualStudio.Services.Notifications.Server.Subscription()
        {
          Channel = "User",
          SubscriberId = identityId,
          IndexedExpression = artifactUri,
          Metadata = subscriptionMetadata,
          Matcher = "FollowsMatcher"
        };
        IVssRequestContext requestContext1 = requestContext;
        Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription2 = subscription1;
        int subscription3 = service.CreateSubscription(requestContext1, subscription2);
        return new ArtifactSubscription()
        {
          SubscriptionId = subscription3,
          ArtifactId = artifact.ArtifactId,
          ArtifactType = artifact.ArtifactType
        };
      }), nameof (Follow));
    }

    public ArtifactSubscription Unfollow(
      IVssRequestContext requestContext,
      int subscriptionId,
      Guid identityId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(subscriptionId, nameof (subscriptionId), 0);
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      return requestContext.TraceBlock<ArtifactSubscription>(15091006, 15091007, 15091008, "Follows", "FollowsService", (Func<ArtifactSubscription>) (() =>
      {
        ArtifactSubscription artifactSubscription = this.GetArtifactSubscription(requestContext, subscriptionId);
        if (artifactSubscription == null || artifactSubscription.SubscriberId != identityId)
          throw new UnauthorizedAccessException(CoreRes.UnauthorizedUnfollow((object) requestContext.AuthenticatedUserName, (object) subscriptionId));
        using (IEventNotificationComponent notificationComponent = this.m_componentFactory(requestContext))
          notificationComponent.DeleteSubscription(subscriptionId);
        return artifactSubscription;
      }), nameof (Unfollow));
    }

    public void Unfollow(IVssRequestContext requestContext, IEnumerable<int> subscriptionIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(subscriptionIds, nameof (subscriptionIds));
      if (!subscriptionIds.Any<int>())
        return;
      int[] subscriptions = subscriptionIds.Where<int>((Func<int, bool>) (id => id > 0)).ToArray<int>();
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      requestContext.TraceBlock(15091009, 15091010, 15091011, "Follows", "FollowsService", (Action) (() =>
      {
        using (IEventNotificationComponent notificationComponent = this.m_componentFactory(requestContext))
          notificationComponent.DeleteSubscriptions((IEnumerable<int>) subscriptions);
      }), nameof (Unfollow));
    }

    public IEnumerable<ArtifactSubscription> GetArtifactSubscriptions(
      IVssRequestContext requestContext,
      string artifactType,
      string[] artifactIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactType, nameof (artifactType));
      ArgumentUtility.CheckForNull<string[]>(artifactIds, nameof (artifactIds));
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      return artifactIds.Length == 0 ? Enumerable.Empty<ArtifactSubscription>() : requestContext.TraceBlock<IEnumerable<ArtifactSubscription>>(15091018, 15091019, 15091020, "Follows", "FollowsService", (Func<IEnumerable<ArtifactSubscription>>) (() =>
      {
        List<SubscriptionLookup> keys = new List<SubscriptionLookup>();
        foreach (string artifactId in artifactIds)
        {
          string artifactUri = this.GetArtifactUri(requestContext, artifactType, artifactId);
          keys.Add(SubscriptionLookup.CreateFollowsMatcherProcessLookup(artifactUri));
        }
        return this.GetArtifactSubscriptionsInternal(requestContext, keys);
      }), nameof (GetArtifactSubscriptions));
    }

    private IEnumerable<ArtifactSubscription> GetArtifactSubscriptionsInternal(
      IVssRequestContext requestContext,
      List<SubscriptionLookup> keys)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> source;
      using (IEventNotificationComponent notificationComponent = this.m_componentFactory(requestContext))
        source = (IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>) notificationComponent.QuerySubscriptions((IEnumerable<SubscriptionLookup>) keys, false);
      return source == null || !source.Any<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>() ? Enumerable.Empty<ArtifactSubscription>() : source.Select<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, ArtifactSubscription>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, ArtifactSubscription>) (s => this.CreateArtifactFromSubscription(requestContext, s))).Where<ArtifactSubscription>((Func<ArtifactSubscription, bool>) (s => s != null));
    }

    public ArtifactSubscription GetArtifactSubscription(
      IVssRequestContext requestContext,
      string artifactType,
      string artifactId,
      Guid identityId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactType, nameof (artifactType));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      return requestContext.TraceBlock<ArtifactSubscription>(15091012, 15091013, 15091014, "Follows", "FollowsService", (Func<ArtifactSubscription>) (() => this.GetArtifactSubscriptionInternal(requestContext, SubscriptionLookup.CreateFollowArtifactForUserLookup(this.GetArtifactUri(requestContext, artifactType, artifactId), identityId))), nameof (GetArtifactSubscription));
    }

    private ArtifactSubscription GetArtifactSubscriptionInternal(
      IVssRequestContext requestContext,
      SubscriptionLookup key)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> source = (IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>) null;
      using (IEventNotificationComponent notificationComponent = this.m_componentFactory(requestContext))
        source = (IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>) notificationComponent.QuerySubscriptions((IEnumerable<SubscriptionLookup>) new List<SubscriptionLookup>()
        {
          key
        }, false);
      return source == null || !source.Any<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>() ? (ArtifactSubscription) null : this.CreateArtifactFromSubscription(requestContext, source.First<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>());
    }

    public ArtifactSubscription GetArtifactSubscription(
      IVssRequestContext requestContext,
      int subscriptionId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      return requestContext.TraceBlock<ArtifactSubscription>(15091015, 15091016, 15091017, "Follows", "FollowsService", (Func<ArtifactSubscription>) (() => this.GetArtifactSubscriptionInternal(requestContext, SubscriptionLookup.CreateSubscriptionIdLookup(subscriptionId))), nameof (GetArtifactSubscription));
    }

    private bool TryParseArtifact(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription,
      out ArtifactSubscription artifact)
    {
      artifact = (ArtifactSubscription) null;
      try
      {
        string artifactType = subscription.Metadata;
        IArtifactUriProvider artifactUriProvider = this.m_artifactProviders.Where<IArtifactUriProvider>((Func<IArtifactUriProvider, bool>) (p => p.ArtifactType == artifactType)).FirstOrDefault<IArtifactUriProvider>();
        if (artifactUriProvider == null)
          return false;
        artifact = artifactUriProvider.GetArtifact(subscription.IndexedExpression);
        artifact.SubscriptionId = subscription.ID;
        artifact.SubscriberId = subscription.SubscriberId;
        artifact.Description = subscription.Description;
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15091021, "Follows", "FollowsService", ex);
        return false;
      }
    }

    private string GetArtifactUri(
      IVssRequestContext requestContext,
      string artifactType,
      string artifactId)
    {
      IArtifactUriProvider artifactUriProvider = this.m_artifactProviders.Where<IArtifactUriProvider>((Func<IArtifactUriProvider, bool>) (p => p.ArtifactType == artifactType)).FirstOrDefault<IArtifactUriProvider>();
      if (artifactUriProvider == null)
      {
        string str = string.Join(" ,", this.m_artifactProviders.Select<IArtifactUriProvider, string>((Func<IArtifactUriProvider, string>) (p => p.ArtifactType)));
        throw new InvalidOperationException(CoreRes.InvalidArtifactType((object) artifactType, (object) str));
      }
      string artifactUri = artifactUriProvider.GetArtifactUri(artifactId);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactUri, "artifactUri");
      return artifactUri;
    }

    private string GetSubscriptionMetadata(ArtifactSubscription artifact) => artifact.ArtifactType;

    private bool IsArtifactValid(ArtifactSubscription artifact) => artifact != null && !string.IsNullOrWhiteSpace(artifact.ArtifactType) && !string.IsNullOrWhiteSpace(artifact.ArtifactId);

    private ArtifactSubscription CreateArtifactFromSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription)
    {
      ArtifactSubscription artifact;
      this.TryParseArtifact(requestContext, subscription, out artifact);
      return artifact;
    }

    private bool CheckCanFollowArtifact(
      IVssRequestContext requestContext,
      ArtifactSubscription artifact,
      Guid identityId)
    {
      IFollowSecurityProvider securityProvider = this.m_securityProviders.Where<IFollowSecurityProvider>((Func<IFollowSecurityProvider, bool>) (p => p.ArtifactType == artifact.ArtifactType)).FirstOrDefault<IFollowSecurityProvider>();
      if (securityProvider == null)
      {
        string str = string.Join(" ,", this.m_securityProviders.Select<IFollowSecurityProvider, string>((Func<IFollowSecurityProvider, string>) (p => p.ArtifactType)));
        throw new InvalidOperationException(CoreRes.InvalidArtifactType((object) artifact.ArtifactType, (object) str));
      }
      return securityProvider.CanFollow(requestContext, identityId, artifact.ArtifactId);
    }
  }
}
