// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Alerts.SubscriptionModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Alerts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0FF2CB39-6514-430A-A4E9-A45535A580D6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Alerts.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Server.WebAccess.Alerts
{
  [DataContract]
  internal class SubscriptionModel
  {
    private static Regex s_subscriptionNameRegEx = new Regex("(?<prename>.*<PT [^>]*N=\\\")(?<name>[^\\\"]+)(?<postname>\\\".*)", RegexOptions.Compiled);
    private static string s_subscriptionNameFormat = "<PT N=\"{0}\" />";

    public SubscriptionModel()
    {
    }

    public SubscriptionModel(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription,
      TfsSubscriptionAdapter adapter)
    {
      bool isBasicAlert = false;
      this.Id = subscription.ID;
      this.SubscriberId = subscription.SubscriberId;
      this.Tag = subscription.Tag;
      this.Name = adapter.ParseTagData(requestContext, subscription, ref isBasicAlert);
      this.IsBasicAlert = isBasicAlert;
      this.DeliveryPreference = new DeliveryPreferenceModel(new Microsoft.VisualStudio.Services.Notifications.Common.DeliveryPreference()
      {
        Address = subscription.DeliveryAddress,
        Type = DeliveryTypeChannelMapper.GetDeliveryType(subscription.Channel)
      });
      this.EventTypeName = subscription.SubscriptionFilter.EventType;
      this.SubscriptionType = adapter.SubscriptionType;
      this.Filter = this.GetFilter(requestContext, subscription, (PathSubscriptionAdapter) adapter);
      this.ProjectId = subscription.ProjectId;
    }

    private ExpressionFilterModel GetFilter(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription,
      PathSubscriptionAdapter adapter)
    {
      ExpressionFilterModel filter = new ExpressionFilterModel();
      try
      {
        filter = adapter.ParseCondition(requestContext, subscription.ConditionString, subscription.Matcher);
      }
      catch (Exception ex)
      {
        this.LastError = string.Format("Failed to parse subscription condition string '{0}'. Error: {1}", (object) subscription.ConditionString, (object) ex);
        TeamFoundationTrace.Warning(this.LastError);
      }
      return filter;
    }

    internal string GetTagForSave()
    {
      if (this.IsBasicAlert && !string.IsNullOrWhiteSpace(this.Tag))
        return this.Tag;
      return string.IsNullOrEmpty(this.Name) ? string.Empty : string.Format(SubscriptionModel.s_subscriptionNameFormat, (object) this.Name.Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;"));
    }

    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "subscriberId")]
    public Guid SubscriberId { get; set; }

    [DataMember(Name = "ownerId")]
    public Guid OwnerId { get; set; }

    [DataMember(Name = "eventTypeName")]
    public string EventTypeName { get; set; }

    [DataMember(Name = "subscriptionType")]
    public SubscriptionType SubscriptionType { get; set; }

    [DataMember(Name = "tag", EmitDefaultValue = false)]
    public string Tag { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "isBasicAlert", EmitDefaultValue = false)]
    public bool IsBasicAlert { get; set; }

    [DataMember(Name = "deliveryPreference")]
    public DeliveryPreferenceModel DeliveryPreference { get; set; }

    [DataMember(Name = "filter")]
    public ExpressionFilterModel Filter { get; set; }

    [DataMember(Name = "lastError", EmitDefaultValue = false)]
    public string LastError { get; set; }

    [DataMember(Name = "lastWarning", EmitDefaultValue = false)]
    public string LastWarning { get; set; }

    [DataMember(Name = "projectId")]
    public Guid ProjectId { get; set; }

    [DataMember(Name = "matcher")]
    public string Matcher { get; set; }

    [DataMember(Name = "status", EmitDefaultValue = false)]
    public SubscriptionStatus Status { get; set; }
  }
}
