// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Common.NotificationClientConstants
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Notifications.Common
{
  public static class NotificationClientConstants
  {
    public static readonly string EventPublisherContribution = "ms.vss-notifications.event-publishers";
    public static readonly string CategoryContributionRoot = "ms.vss-notifications.event-categories";
    public const string EventContribution = "ms.vss-notifications.event-type";
    public const string LegacyEventContribution = "ms.vss-notifications.legacy-event-type";
    public const string FieldContribution = "ms.vss-notifications.event-field";
    public const string CategoryContribution = "ms.vss-notifications.event-category";
    public const string PublisherContributionType = "ms.vss-notifications.event-publisher";
    public const string UserSubscriptionTemplateContribution = "ms.vss-notifications.user-subscription-templates";
    public const string TeamSubscriptionTemplateContribution = "ms.vss-notifications.team-subscription-templates";
    public const string SubscriptionTemplateContributionType = "ms.vss-notifications.subscription-template";
    public static readonly string SubscriptionContribution = "ms.vss-notifications.subscription";
    public static readonly string DefaultSubscriptionContributionTarget = "ms.vss-notifications.default-subscription-collection";
    public static readonly string DefaultSystemSubscriptionContributionTarget = "ms.vss-notifications.default-system-subscription-collection";
    public static readonly string NotificationsOverviewHubId = "ms.vss-notifications-web.collection-admin-hub";
    public static readonly string CollectionSettingsHubId = "ms.vss-admin-web.collection-admin-hub";
    public static readonly string NotificationsTeamHubId = "ms.vss-notifications-web.team-admin-hub";
    public static readonly string FieldTypeProperty = "fieldType";
    public static readonly string DisplayNameProperty = "displayName";
    public static readonly string NameProperty = "name";
    public static readonly string AliasProperty = "alias";
    public static readonly string RolesProperty = "roles";
    public static readonly string ScopesProperty = "supportedScopes";
    public static readonly string HasInitiatorProperty = "hasInitiator";
    public static readonly string HasDynamicRolesProperty = "hasDynamicRoles";
    public static readonly string IconProperty = "icon";
    public static readonly string ColorProperty = "color";
    public static readonly string CustomSubscriptionsAllowed = "customSubscriptionsAllowed";
    public static readonly string PathProperty = "path";
    public static readonly string OperatorsProperty = "operators";
    public static readonly string ValuesProperty = "values";
    public static readonly string IsComplexProperty = "isComplex";
    public static readonly string ServiceInstanceTypeProperty = "serviceInstanceType";
    public static readonly string OperatorsConstraintProperty = "operatorsConstraints";
    public static readonly string IsRoleProperty = "isRole";
    public static readonly string DetailedDescriptionProperty = "detailedDescription";
    public const string UserChannel = "User";
    public const string UserSystemChannel = "UserSystem";
    public const string EmailHtmlChannel = "EmailHtml";
    public const string EmailPlaintextChannel = "EmailPlaintext";
    public const string SoapChannel = "Soap";
    public const string MessageQueueChannel = "MessageQueue";
    public const string ServiceHooksChannel = "ServiceHooks";
    public const string ServiceBusChannel = "ServiceBus";
    public const string GroupChannel = "Group";
    public const string PersistedNotificationChannel = "PersistedNotification";
    public const string BlockChannel = "Block";
    public const string AuditChannel = "Audit";
    public const string PathMatcher = "PathMatcher";
    public const string JsonPathMatcher = "JsonPathMatcher";
    public const string XPathMatcher = "XPathMatcher";
    public const string FollowsMatcher = "FollowsMatcher";
    public const string ActorMatcher = "ActorMatcher";
    public const string BlockMatcher = "BlockMatcher";
    public const string PathExpressionMatcher = "PathExpressionMatcher";
    public const string ActorExpressionMatcher = "ActorExpressionMatcher";
    public const string BlockExpressionMatcher = "BlockExpressionMatcher";
    public const string AuditExpressionMatcher = "AuditExpressionMatcher";
    public const string ArtifactFilter = "Artifact";
    public const string ExpressionFilter = "Expression";
    public const string BlockFilter = "Block";
    public const string ActorFilter = "Actor";
    public const string DiagnosticLogEventProcessing = "EventProcessing";
    public const string DiagnosticLogNotificationDelivery = "NotificationDelivery";
    public const string DiagnosticLogSubscriptionTraceEventProcessing = "SubscriptionTraceEventProcessing";
    public const string DiagnosticLogSubscriptionTraceNotificationDelivery = "SubscriptionTraceNotificationDelivery";
    public const string MyProjectNameMacro = "@@MyProjectName@@";
    public const string MyDisplayNameMacro = "@@MyDisplayName@@";
    public const string MyUniqueNameMacro = "@@MyUniqueName@@";
    public const string SingleQuoteNameMacro = "@@SQBDQ@@";
    public const string DoubleQuoteNameMacro = "@@DQBSQ@@";
    public const string SingleQuoteCharMacro = "@@SingleQuote@@";
    public const string DoubleQuoteCharMacro = "@@DoubleQuote@@";
    public const char ProcessedFlagCharacter = '\a';
    public const char ProcessedTfIdFlagCharacter = '\v';
    public const char DisplayNameFlagCharacter = '|';
    public const char TfIdFlagCharacter = '%';
    public const string DefaultSubscriptionScope = "none";
    public static Guid CollectionScope = new Guid("00000000-0000-636f-6c6c-656374696f6e");
    public const string CollectionScopeName = "collection";
    public const string ProjectScopeName = "project";
    public static readonly string GitCommentsProcessQueueId = "ms.vss-code.git-comments-event-publisher";
  }
}
