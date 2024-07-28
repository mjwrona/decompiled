// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationDeliveryDetails
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class NotificationDeliveryDetails : ICloneable
  {
    public NotificationDeliveryDetails() => this.Recipients = new List<NotificationRecipient>();

    public string NotificationSource { get; set; }

    public bool IsOptOutable { get; set; }

    public bool IsSystem { get; set; }

    public Guid SourceId { get; set; }

    [JsonIgnore]
    public Microsoft.VisualStudio.Services.Identity.Identity SourceIdentity { get; set; }

    public string SourceUrl { get; set; }

    public string Matcher { get; set; }

    public Guid AuthId { get; set; }

    [JsonIgnore]
    public Microsoft.VisualStudio.Services.Identity.Identity AuthIdentity { get; set; }

    public string DeliveryAddress { get; set; }

    public string Tag { get; set; }

    public string Description { get; set; }

    public List<NotificationRecipient> Recipients { get; }

    public string ChannelMetadata { get; set; }

    public bool ShouldPersistResultDetail { get; set; }

    public bool DeliveryTracing { get; set; }

    public object Clone()
    {
      NotificationDeliveryDetails ndd = new NotificationDeliveryDetails()
      {
        NotificationSource = this.NotificationSource,
        SourceId = this.SourceId,
        SourceIdentity = this.SourceIdentity?.Clone(),
        SourceUrl = this.SourceUrl,
        Matcher = this.Matcher,
        AuthId = this.AuthId,
        AuthIdentity = this.AuthIdentity?.Clone(),
        DeliveryAddress = this.DeliveryAddress,
        Tag = this.Tag,
        Description = this.Description,
        IsOptOutable = this.IsOptOutable,
        IsSystem = this.IsSystem,
        ChannelMetadata = this.ChannelMetadata,
        ShouldPersistResultDetail = this.ShouldPersistResultDetail,
        DeliveryTracing = this.DeliveryTracing
      };
      this.Recipients.ForEach((Action<NotificationRecipient>) (r => ndd.Recipients.Add((NotificationRecipient) r.Clone())));
      return (object) ndd;
    }

    private Microsoft.VisualStudio.Services.Identity.Identity LookupIdentity(
      Guid id,
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityCache)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      return id.Equals(Guid.Empty) || !identityCache.TryGetValue(id, out identity) ? (Microsoft.VisualStudio.Services.Identity.Identity) null : identity;
    }

    public void PopulateIdentities(Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityCache)
    {
      this.SourceIdentity = this.LookupIdentity(this.SourceId, identityCache);
      this.AuthIdentity = this.LookupIdentity(this.AuthId, identityCache);
      foreach (NotificationRecipient recipient in this.Recipients)
        recipient.Identity = this.LookupIdentity(recipient.Id, identityCache);
    }

    public void PopulateFromSubscription(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      this.NotificationSource = subscription.UniqueId != Guid.Empty ? subscription.UniqueId.ToString("D") : subscription.SubscriptionId;
      this.SourceId = subscription.SubscriberId;
      this.AuthId = subscription.LastModifiedBy == Guid.Empty ? subscription.SubscriberId : subscription.LastModifiedBy;
      this.Matcher = subscription.Matcher;
      this.DeliveryAddress = subscription.DeliveryAddress;
      this.Tag = subscription.Tag;
      this.Description = subscription.Description;
      this.IsOptOutable = subscription.IsOptOutable();
      this.IsSystem = subscription.IsSystem;
      this.ChannelMetadata = subscription.Metadata;
      if (requestContext == null || subscription.IsSystem)
        return;
      this.SourceUrl = NotificationSubscriptionService.GetMyNotificationsUrl(requestContext, subscription);
    }

    public static NotificationDeliveryDetails CreateFromSubscription(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      NotificationDeliveryDetails fromSubscription = new NotificationDeliveryDetails();
      fromSubscription.PopulateFromSubscription(requestContext, subscription);
      return fromSubscription;
    }
  }
}
