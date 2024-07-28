// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.FollowsSubscriptionAdapter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class FollowsSubscriptionAdapter : BaseSubscriptionAdapter
  {
    private const string CustomerIntelligenceLayer = "Service.FollowsAdapter";

    public override string FilterType => "Artifact";

    public override string GetMatcher(IVssRequestContext requestContext, string eventType) => "FollowsMatcher";

    protected abstract string GetEditArtifactUrl(
      IVssRequestContext requestContext,
      Subscription subscription);

    protected abstract string GetArtifactUrlFromInfo(
      IVssRequestContext requestContext,
      string artifactType,
      string artifactId);

    protected abstract void GetArtifactInfoFromUrl(
      string artifactUri,
      ref string artifactType,
      ref string artifactId);

    public override void ApplyFilterUpdatesToSubscription(
      IVssRequestContext requestContext,
      Subscription subscriptionToPatch,
      NotificationSubscriptionUpdateParameters updateParameters)
    {
      int id = subscriptionToPatch.ID;
      this.VerifyFieldIsNull((object) updateParameters.Filter, "Filter", id);
    }

    public override Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscription notificationSubscription)
    {
      Subscription subscription = base.ToSubscription(requestContext, notificationSubscription);
      this.SetSubscriptionFilter(requestContext, subscription, notificationSubscription.Filter);
      if (notificationSubscription.Channel == null)
        subscription.Channel = "User";
      return subscription;
    }

    public override Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters)
    {
      Subscription subscription = base.ToSubscription(requestContext, createParameters);
      this.SetSubscriptionFilter(requestContext, subscription, createParameters.Filter);
      if (createParameters.Channel == null)
        subscription.Channel = "User";
      return subscription;
    }

    private void SetSubscriptionFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      ISubscriptionFilter filter)
    {
      ArtifactFilter subsbscriptionFilter = this.ValidateAndParseSubsbscriptionFilter(requestContext, filter);
      if (!this.CheckCanFollowArtifact(requestContext, subscription.SubscriberIdentity, subsbscriptionFilter.ArtifactId))
        throw new UnauthorizedAccessException(CoreRes.UnauthorizedFollow((object) subscription.SubscriberIdentity.Id, (object) subsbscriptionFilter.ArtifactUri, (object) string.Empty));
      if (string.IsNullOrEmpty(subscription.Description))
        subscription.Description = CoreRes.FollowingArtifact((object) subsbscriptionFilter.ArtifactType, (object) subsbscriptionFilter.ArtifactId);
      subscription.IndexedExpression = subsbscriptionFilter.ArtifactUri;
      subscription.Metadata = subsbscriptionFilter.ArtifactType;
      subscription.Matcher = "FollowsMatcher";
    }

    public override NotificationSubscription ToNotificationSubscription(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Subscription>(subscription, nameof (subscription));
      NotificationSubscription notificationSubscription = base.ToNotificationSubscription(requestContext, subscription, queryFlags);
      if (!string.IsNullOrEmpty(subscription.IndexedExpression) || !queryFlags.HasFlag((Enum) SubscriptionQueryFlags.AlwaysReturnBasicInformation))
        notificationSubscription.Links.AddLink("edit", this.GetEditArtifactUrl(requestContext, subscription));
      return notificationSubscription;
    }

    public override ISubscriptionFilter CreateSubscriptionFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      this.GetArtifactInfoFromUrl(subscription.IndexedExpression, ref empty1, ref empty2);
      ArtifactFilter subscriptionFilter = new ArtifactFilter(subscription.IndexedExpression);
      subscriptionFilter.EventType = subscription.EventTypeName;
      subscriptionFilter.ArtifactId = empty2;
      subscriptionFilter.ArtifactType = empty1;
      return (ISubscriptionFilter) subscriptionFilter;
    }

    public override void ApplyToSubscriptionLookup(
      IVssRequestContext requestContext,
      SubscriptionLookup lookup,
      ISubscriptionFilter filter)
    {
      ArgumentUtility.CheckForNull<SubscriptionLookup>(lookup, nameof (lookup));
      ArgumentUtility.CheckForNull<ISubscriptionFilter>(filter, nameof (filter));
      ArtifactFilter artifactFilter = filter as ArtifactFilter;
      if (artifactFilter.ArtifactType != null && artifactFilter.ArtifactId != null)
        artifactFilter = this.ValidateAndParseSubsbscriptionFilter(requestContext, filter);
      if (artifactFilter == null)
        return;
      lookup.Matcher = "FollowsMatcher";
      lookup.IndexedExpression = artifactFilter.ArtifactUri;
      lookup.Metadata = artifactFilter.ArtifactType;
    }

    private ArtifactFilter ValidateAndParseSubsbscriptionFilter(
      IVssRequestContext requestContext,
      ISubscriptionFilter filter)
    {
      ArtifactFilter subsbscriptionFilter = filter as ArtifactFilter;
      if (filter == null)
        throw new InvalidSubscriptionException(CoreRes.InvalidSubscriptionFilter());
      if (string.IsNullOrEmpty(subsbscriptionFilter.ArtifactUri))
      {
        if (string.IsNullOrEmpty(subsbscriptionFilter.ArtifactType) || string.IsNullOrEmpty(subsbscriptionFilter.ArtifactId))
          throw new InvalidSubscriptionException(CoreRes.ArtifactIsRequired());
        subsbscriptionFilter.ArtifactUri = this.GetArtifactUrlFromInfo(requestContext, subsbscriptionFilter.ArtifactType, subsbscriptionFilter.ArtifactId);
      }
      else
      {
        if (!string.IsNullOrEmpty(subsbscriptionFilter.ArtifactType) || !string.IsNullOrEmpty(subsbscriptionFilter.ArtifactId))
          throw new InvalidSubscriptionException(CoreRes.ArtifactTypeOrUri());
        string artifactType = (string) null;
        string artifactId = (string) null;
        this.GetArtifactInfoFromUrl(subsbscriptionFilter.ArtifactUri, ref artifactType, ref artifactId);
        subsbscriptionFilter.ArtifactId = artifactId;
        subsbscriptionFilter.ArtifactType = artifactType;
      }
      return subsbscriptionFilter;
    }

    protected abstract bool CheckCanFollowArtifact(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string artifact);
  }
}
