// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.NotificationConstants
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class NotificationConstants
  {
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.MyProjectNameMacro in assembly MS.VS.Services.Notifications.WebApi")]
    public const string MyProjectNameMacro = "@@MyProjectName@@";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.MyDisplayNameMacro in assembly MS.VS.Services.Notifications.WebApi")]
    public const string MyDisplayNameMacro = "@@MyDisplayName@@";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.MyUniqueNameMacro in assembly MS.VS.Services.Notifications.WebApi")]
    public const string MyUniqueNameMacro = "@@MyUniqueName@@";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.SingleQuoteNameMacro in assembly MS.VS.Services.Notifications.WebApi")]
    public const string SingleQuoteNameMacro = "@@SQBDQ@@";
    [Obsolete]
    public const string SingleQuoteValue = "\"'\"";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.DoubleQuoteNameMacro in assembly MS.VS.Services.Notifications.WebApi")]
    public const string DoubleQuoteNameMacro = "@@DQBSQ@@";
    [Obsolete]
    public const string DoubleQuoteValue = "'\"'";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.SingleQuoteCharMacro in assembly MS.VS.Services.Notifications.WebApi")]
    public const string SingleQuoteCharMacro = "@@SingleQuote@@";
    [Obsolete]
    public const string SingleQuoteCharValue = "'";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.DoubleQuoteCharMacro in assembly MS.VS.Services.Notifications.WebApi")]
    public const string DoubleQuoteCharMacro = "@@DoubleQuote@@";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.DoubleQuoteCharValue in assembly MS.VS.Services.Notifications.WebApi")]
    public const string DoubleQuoteCharValue = "\"";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.DynamicTargetsToken in assembly MS.VS.Services.Notifications.WebApi")]
    public const string DynamicTargetsToken = "@@";
    public const string CustomNotificationAddressesIdentityProperty = "CustomNotificationAddresses";
    public const string ConfirmedNotificationAddressIdentityProperty = "ConfirmedNotificationAddress";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.LegacyNames.WorkItemChangedEvent in assembly MS.VS.Services.Notifications.WebApi")]
    public const string WorkItemChangedEventTypeName = "WorkItemChangedEvent";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.LegacyNames.BuildStatusChangeEvent in assembly MS.VS.Services.Notifications.WebApi")]
    public const string BuildStatusChangeEventName = "BuildStatusChangeEvent";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.LegacyNames.BuildCompletedEvent in assembly MS.VS.Services.Notifications.WebApi")]
    public const string BuildCompletedEventName = "BuildCompletedEvent";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.LegacyNames.CheckinEvent in assembly MS.VS.Services.Notifications.WebApi")]
    public const string CheckinEventName = "CheckinEvent";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.LegacyNames.CodeReviewChangedEvent in assembly MS.VS.Services.Notifications.WebApi")]
    public const string CodeReviewChangedEventName = "CodeReviewChangedEvent";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.LegacyNames.GitPushEvent in assembly MS.VS.Services.Notifications.WebApi")]
    public const string GitPushEventName = "GitPushEvent";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.LegacyNames.GitPullRequestEvent in assembly MS.VS.Services.Notifications.WebApi")]
    public const string GitPullRequestEventName = "GitPullRequestEvent";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationUrlConstants.AlertsPageRelativePath in assembly MS.VS.Services.Notifications.WebApi")]
    public const string AlertsPageRelativePath = "{0}#id={1}&showteams={2}";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationUrlConstants.AlertsPage in assembly MS.VS.Services.Notifications.WebApi")]
    public const string AlertsPage = "_Alerts";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationUrlConstants.AlertsAdminPage in assembly MS.VS.Services.Notifications.WebApi")]
    public const string AlertsAdminPage = "_admin/_Alerts";
    public const string EmailConfirmationSendDates = "EmailConfirmationSendDates";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.ProcessedFlagCharacter in assembly MS.VS.Services.Notifications.WebApi")]
    public const char ProcessedFlagCharacter = '\a';
    public const char ProcessedTfIdFlagCharacter = '\v';
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.DisplayNameFlagCharacter in assembly MS.VS.Services.Notifications.WebApi")]
    public const char DisplayNameFlagCharacter = '|';
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.TfIdFlagCharacter in assembly MS.VS.Services.Notifications.WebApi")]
    public const char TfIdFlagCharacter = '%';
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.FeatureFlags.AllowUserRegexInMatchConditionFeatureFlag in assembly MS.VS.Services.Notifications.WebApi")]
    public const string AllowUserRegexInMatchConditionFeatureFlag = "VisualStudio.Services.Notifications.AllowUserRegexInMatchCondition";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.MDMConstants.MDMNotificationJobScope in assembly MS.VS.Services.Notifications.WebApi")]
    public const string MDMNotificationJobScope = "NotificationJob";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.MDMConstants.EventProcessingDelayKPI in assembly MS.VS.Services.Notifications.WebApi")]
    public const string EventProcessingDelayKPI = "EventProcessingDelayInMs";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.MDMConstants.EventProcessingDelayKPIDesc in assembly MS.VS.Services.Notifications.WebApi")]
    public const string EventProcessingDelayKPIDesc = "Time taken to start processing an event";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.MDMConstants.MDMDeliveryJobscope in assembly MS.VS.Services.Notifications.WebApi")]
    public const string MDMDeliveryJobscope = "NotificationDeliveryJob";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.MDMConstants.DeliveryDelayKPI in assembly MS.VS.Services.Notifications.WebApi")]
    public const string DeliveryDelayKPI = "NotificationDeliveryDelayInMs";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.MDMConstants.DeliveryDelayWithRetriesKPI in assembly MS.VS.Services.Notifications.WebApi")]
    public const string DeliveryDelayWithRetriesKPI = "NotificationDeliveryDelayWithRetriesInMs";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.MDMConstants.TotalProcessingTimeKPI in assembly MS.VS.Services.Notifications.WebApi")]
    public const string TotalProcessingTimeKPI = "EventProcessingTimeInMs";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.MDMConstants.TotalProcessingTimeWithRetriesKPI in assembly MS.VS.Services.Notifications.WebApi")]
    public const string TotalProcessingTimeWithRetriesKPI = "EventProcessingTimeWithRetriesInMs";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.MDMConstants.DeliveryDelayKPIDesc in assembly MS.VS.Services.Notifications.WebApi")]
    public const string DeliveryDelayKPIDesc = "Time taken to start deliverying a notification";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.BridgeKey in assembly MS.VS.Services.Notifications.WebApi")]
    public const string BridgeKey = "@NotifBridge";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.RetryCountRegistryKey in assembly MS.VS.Services.Notifications.WebApi")]
    public const string RetryCountRegistryKey = "NotificationRetryCount";
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.RetryCountDefaultValue in assembly MS.VS.Services.Notifications.WebApi")]
    public const int RetryCountDefaultValue = 5;
    [Obsolete("Moved to Microsoft.VisualStudio.Services.Notifications.Common.NotificationFrameworkConstants.CollectionScope in assembly MS.VS.Services.Notifications.WebApi")]
    public static Guid CollectionScope = new Guid("00000000-0000-636f-6c6c-656374696f6e");
  }
}
