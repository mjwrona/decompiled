// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.Subscription
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class Subscription : IDisposable, ICloneable
  {
    private string m_matcher;
    private string m_eventTypeName;
    private string m_subscriptionId;
    private string[] m_traceTag;
    private EventSerializerType? m_eventSerializerType;
    private const string s_area = "Notifications";
    private const string s_layer = "Subscription";
    private ISubscriptionAdapter m_defaultAdapter;
    private SubscriptionFieldProvider m_fieldProvider;
    private ISubscriptionFilter m_subscriptionFilter;

    public Subscription()
    {
      this.SubsStats = new SubscriptionEvaluationStats();
      this.Diagnostics = new SubscriptionDiagnostics();
    }

    public Subscription(ISubscriptionAdapter adapter)
      : this()
    {
      this.m_defaultAdapter = adapter;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int ID { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public Guid UniqueId { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string ContributionId { get; set; }

    [XmlIgnore]
    public string SubscriptionId
    {
      get
      {
        if (this.m_subscriptionId == null)
          this.m_subscriptionId = string.IsNullOrEmpty(this.ContributionId) ? this.ID.ToString() : this.ContributionId;
        return this.m_subscriptionId;
      }
    }

    internal string EventTypeName
    {
      get => this.m_eventTypeName;
      set
      {
        this.m_eventTypeName = value;
        if (this.SubscriptionFilter == null)
          return;
        this.SubscriptionFilter.EventType = this.m_eventTypeName;
      }
    }

    public string Matcher
    {
      get => this.m_matcher;
      set
      {
        if (this.m_matcher != null && this.m_matcher != value)
          this.m_defaultAdapter = (ISubscriptionAdapter) null;
        this.m_matcher = value;
      }
    }

    public string Tag { get; set; }

    public Guid ProjectId { get; set; }

    public Guid ScopeId => this.SubscriptionScope == null ? Guid.Empty : this.SubscriptionScope.Id;

    internal SubscriptionDiagnostics Diagnostics { get; set; }

    internal SubscriptionTraceEventProcessingLogInternal TraceLog { get; set; }

    internal bool SubscriptionTracingEnabled
    {
      get
      {
        if (this.TraceLog == null)
          return false;
        SubscriptionTracing evaluationTracing = this.Diagnostics.EvaluationTracing;
        return evaluationTracing != null && evaluationTracing.Enabled;
      }
    }

    [XmlIgnore]
    public ISubscriptionFilter SubscriptionFilter
    {
      get => this.m_subscriptionFilter;
      set
      {
        this.m_subscriptionFilter = value;
        if (string.IsNullOrEmpty(this.SubscriptionFilter?.EventType))
          return;
        this.EventTypeName = this.SubscriptionFilter.EventType;
      }
    }

    public SubscriptionScope SubscriptionScope { get; set; }

    public Guid SubscriberId { get; set; }

    [XmlIgnore]
    public Microsoft.VisualStudio.Services.Identity.Identity SubscriberIdentity { get; internal set; }

    [XmlIgnore]
    public Guid LastModifiedBy { get; set; }

    [XmlIgnore]
    public Microsoft.VisualStudio.Services.Identity.Identity LastModifiedByIdentity { get; internal set; }

    [XmlIgnore]
    public DateTime ModifiedTime { get; internal set; }

    public string Metadata { get; set; }

    public string Description { get; set; }

    [XmlIgnore]
    public Dictionary<string, string> ExtendedProperties { get; set; }

    public string IndexedExpression { get; set; }

    [XmlIgnore]
    internal string ProjectName { get; set; }

    [XmlIgnore]
    public SubscriptionFlags Flags { get; internal set; }

    public string DeliveryAddress { get; set; }

    public string Channel { get; set; }

    internal string Warning { get; set; }

    [XmlIgnore]
    internal SubscriptionEvaluationStats SubsStats { get; set; }

    [XmlIgnore]
    internal bool UserSettingsOptedOut { get; set; }

    [XmlIgnore]
    internal bool AdminSettingsBlockUserOptOut { get; set; }

    [XmlIgnore]
    internal bool PostBindSubscriptionCompleted { get; set; }

    [XmlIgnore]
    internal bool PreBindSubscriptionCompleted { get; set; }

    [XmlIgnore]
    internal string[] TraceTags
    {
      get
      {
        if (this.m_traceTag == null)
          this.m_traceTag = new string[1]
          {
            string.Format("Subscription{0}", (object) this.SubscriptionId)
          };
        return this.m_traceTag;
      }
    }

    public string ConditionString { get; set; }

    internal string Expression { get; set; }

    public SubscriptionStatus Status { get; set; }

    public string StatusMessage { get; set; }

    public SubscriptionPermissions Permissions { get; set; }

    [XmlIgnore]
    internal bool ExceptionLoggedAlready { get; set; }

    [XmlIgnore]
    internal bool IsContributed => this.Flags.HasFlag((Enum) SubscriptionFlags.ContributedSubscription);

    [XmlIgnore]
    internal bool IsSystem { get; set; }

    [XmlIgnore]
    internal bool IsServiceHooksDelivery => this.Channel == "ServiceHooks";

    [XmlIgnore]
    internal bool IsSoapDelivery => this.Channel == "Soap";

    [XmlIgnore]
    public bool IsGroup => this.Flags.HasFlag((Enum) SubscriptionFlags.GroupSubscription);

    [XmlIgnore]
    public bool IsEmailDelivery => NotificationFrameworkConstants.EmailTargetDeliveryChannels.Contains(this.Channel);

    [XmlIgnore]
    internal bool IsHtmlDelivery => NotificationFrameworkConstants.HtmlEmailTargetDeliveryChannels.Contains(this.Channel);

    [XmlIgnore]
    internal bool IsLocalServiceHooksDelivery => NotificationFrameworkConstants.LocalServiceHooksDeliveryChannels.Contains(this.Channel);

    [XmlIgnore]
    internal bool TraceDeliveryResultsAllowed => this.IsLocalServiceHooksDelivery;

    [XmlIgnore]
    internal bool IsExpandedTeamEmailSubscription => this.SubscriberIdentity.IsContainer && this.IsEmailDelivery && !this.HasAddress;

    [XmlIgnore]
    public bool IsTeam => this.Flags.HasFlag((Enum) SubscriptionFlags.TeamSubscription);

    [XmlIgnore]
    public bool HasAddress => !string.IsNullOrEmpty(this.DeliveryAddress);

    [XmlIgnore]
    internal bool HasPeopleMacros { get; set; }

    internal bool LastModifiedByWillBeUsedForAuth()
    {
      bool flag = false;
      if ((this.IsServiceHooksDelivery || this.SubscriberIdentity.IsContainer) && (this.IsServiceHooksDelivery || this.IsSoapDelivery || this.IsEmailDelivery && this.HasAddress))
        flag = true;
      return flag;
    }

    internal bool ShouldPersistStatus() => this.Status < SubscriptionStatus.Enabled && this.Status != SubscriptionStatusInternal.DisabledForBatch;

    [XmlIgnore]
    internal Condition Condition { get; set; }

    public EventSerializerType GetEventSerializerType(IVssRequestContext requestContext)
    {
      if (!this.m_eventSerializerType.HasValue)
        this.m_eventSerializerType = new EventSerializerType?(requestContext.GetService<INotificationEventService>().GetSerializationFormatForEvent(requestContext, this.EventTypeName));
      return this.m_eventSerializerType.Value;
    }

    internal void PostBindSubscription(
      IVssRequestContext requestContext,
      SubscriptionQueryFlags queryFlags)
    {
      MatcherFilterFactory.GetMatcherFilter(requestContext, this.Matcher).PostBindSubscription(requestContext, this, queryFlags);
    }

    internal void PreBindSubscription(IVssRequestContext requestContext) => MatcherFilterFactory.GetMatcherFilter(requestContext, this.Matcher).PreBindSubscription(requestContext, this);

    public object Clone()
    {
      Subscription subscription = new Subscription();
      subscription.Expression = this.Expression;
      subscription.EventTypeName = this.EventTypeName;
      subscription.ConditionString = this.ConditionString;
      subscription.DeliveryAddress = this.DeliveryAddress;
      subscription.Description = this.Description;
      subscription.ExtendedProperties = this.ExtendedProperties;
      subscription.Flags = this.Flags;
      subscription.Status = this.Status;
      subscription.StatusMessage = this.StatusMessage;
      subscription.ID = this.ID;
      subscription.ContributionId = this.ContributionId;
      subscription.SubscriberId = this.SubscriberId;
      subscription.SubscriberIdentity = this.SubscriberIdentity;
      subscription.LastModifiedBy = this.LastModifiedBy;
      subscription.ProjectId = this.ProjectId;
      subscription.SubscriptionScope = this.SubscriptionScope;
      subscription.Tag = this.Tag;
      subscription.Condition = this.Condition;
      subscription.Warning = this.Warning;
      subscription.SubsStats = this.SubsStats;
      subscription.m_matcher = this.m_matcher;
      subscription.Channel = this.Channel;
      subscription.HasPeopleMacros = this.HasPeopleMacros;
      subscription.IndexedExpression = this.IndexedExpression;
      subscription.UserSettingsOptedOut = this.UserSettingsOptedOut;
      subscription.AdminSettingsBlockUserOptOut = this.AdminSettingsBlockUserOptOut;
      subscription.ModifiedTime = this.ModifiedTime;
      subscription.Metadata = this.Metadata;
      subscription.ProjectName = this.ProjectName;
      subscription.ExceptionLoggedAlready = this.ExceptionLoggedAlready;
      subscription.Permissions = this.Permissions;
      subscription.SubscriptionFilter = this.SubscriptionFilter;
      subscription.UniqueId = this.UniqueId;
      SubscriptionDiagnostics diagnostics = this.Diagnostics;
      subscription.Diagnostics = diagnostics != null ? diagnostics.Clone() : (SubscriptionDiagnostics) null;
      subscription.Messages = this.Messages;
      subscription.IsSystem = this.IsSystem;
      return (object) subscription;
    }

    internal Subscription CloneBasic()
    {
      Subscription subscription = new Subscription();
      subscription.EventTypeName = this.EventTypeName;
      subscription.Matcher = this.Matcher;
      subscription.ID = this.ID;
      subscription.ContributionId = this.ContributionId;
      subscription.SubscriberId = this.SubscriberId;
      subscription.SubscriberIdentity = this.SubscriberIdentity;
      subscription.LastModifiedBy = this.LastModifiedBy;
      subscription.Status = this.Status;
      subscription.ProjectId = this.ProjectId;
      subscription.SubscriptionScope = this.SubscriptionScope;
      subscription.Channel = this.Channel;
      subscription.ModifiedTime = this.ModifiedTime;
      subscription.Permissions = this.Permissions;
      subscription.UniqueId = this.UniqueId;
      SubscriptionDiagnostics diagnostics = this.Diagnostics;
      subscription.Diagnostics = diagnostics != null ? diagnostics.Clone() : (SubscriptionDiagnostics) null;
      return subscription;
    }

    public ISubscriptionAdapter GetDefaultAdapter(
      IVssRequestContext requestContext,
      bool throwIfNotFound = true)
    {
      if (this.m_defaultAdapter == null)
        this.m_defaultAdapter = SubscriptionAdapterFactory.CreateAdapter(requestContext, this.EventTypeName, this.Matcher, this.SubscriptionScope, this.Metadata, throwIfNotFound);
      return this.m_defaultAdapter;
    }

    private void SetStatusFromException(IVssRequestContext requestContext, Exception ex)
    {
      switch (ex)
      {
        case XPathException _:
        case InvalidFieldValueException _:
        case ParseException _:
        case DynamicEventPredicateException _:
          this.Status = SubscriptionStatus.DisabledInvalidPathClause;
          break;
        case InvalidRoleBasedExpressionException _:
          this.Status = SubscriptionStatus.DisabledInvalidRoleExpression;
          break;
        case IdentityNotFoundException _:
          this.Status = SubscriptionStatus.DisabledMissingIdentity;
          break;
        case IdentityInactiveException _:
          this.Status = SubscriptionStatus.DisabledInactiveIdentity;
          break;
        case SubscriptionMemberMissingPermissionsException _:
          this.Status = SubscriptionStatus.DisabledMissingPermissions;
          break;
        case SubscriptionProjectInvalidException _:
          this.Status = SubscriptionStatus.DisabledProjectInvalid;
          break;
        case ArgumentException _:
          this.Status = SubscriptionStatus.DisabledArgumentException;
          break;
        default:
          this.Status = SubscriptionStatusInternal.DisabledForBatch;
          break;
      }
    }

    internal void UpdateDisabledStatusFromException(IVssRequestContext requestContext, Exception ex)
    {
      this.SetStatusFromException(requestContext, ex);
      this.StatusMessage = CoreRes.DisablingSubscription((object) ex.GetType().Name, (object) ex.Message);
    }

    internal List<NotificationDiagnosticLogMessage> Messages { get; private set; }

    internal void AddMessage(TraceLevel level, string message)
    {
      if (this.Messages == null)
        this.Messages = new List<NotificationDiagnosticLogMessage>();
      this.Messages.Append(level, message);
    }

    void IDisposable.Dispose() => GC.SuppressFinalize((object) this);

    internal SubscriptionFieldProvider GetFieldProvider(IVssRequestContext requestContext)
    {
      if (this.m_fieldProvider == null)
        this.m_fieldProvider = new SubscriptionFieldProvider(requestContext, this.SubscriptionFilter.EventType, this.SubscriptionScope);
      return this.m_fieldProvider;
    }

    internal void ToLegacyEventType(IVssRequestContext requestContext)
    {
      this.EventTypeName = EventTypeMapper.ToLegacy(requestContext, this.EventTypeName);
      if (this.SubscriptionFilter == null)
        return;
      this.SubscriptionFilter.EventType = EventTypeMapper.ToLegacy(requestContext, this.SubscriptionFilter.EventType);
    }

    internal void ToContributedEventType(IVssRequestContext requestContext)
    {
      this.EventTypeName = EventTypeMapper.ToContributed(requestContext, this.EventTypeName);
      if (this.SubscriptionFilter == null)
        return;
      this.SubscriptionFilter.EventType = EventTypeMapper.ToContributed(requestContext, this.SubscriptionFilter.EventType);
    }
  }
}
