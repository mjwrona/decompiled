// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.CoreRes
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal static class CoreRes
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (CoreRes).GetTypeInfo().Assembly);

    public static ResourceManager Manager => CoreRes.s_resMgr;

    private static string Get(string resourceName) => CoreRes.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? CoreRes.Get(resourceName) : CoreRes.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) CoreRes.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? CoreRes.GetInt(resourceName) : (int) CoreRes.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) CoreRes.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? CoreRes.GetBool(resourceName) : (bool) CoreRes.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => CoreRes.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = CoreRes.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string SubscriptionWarning() => CoreRes.Get(nameof (SubscriptionWarning));

    public static string SubscriptionWarning(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionWarning), culture);

    public static string InvalidEventSerializerAttributeSerializerType() => CoreRes.Get(nameof (InvalidEventSerializerAttributeSerializerType));

    public static string InvalidEventSerializerAttributeSerializerType(CultureInfo culture) => CoreRes.Get(nameof (InvalidEventSerializerAttributeSerializerType), culture);

    public static string InvalidArtifactType(object arg0, object arg1) => CoreRes.Format(nameof (InvalidArtifactType), arg0, arg1);

    public static string InvalidArtifactType(object arg0, object arg1, CultureInfo culture) => CoreRes.Format(nameof (InvalidArtifactType), culture, arg0, arg1);

    public static string MalformedArtifact() => CoreRes.Get(nameof (MalformedArtifact));

    public static string MalformedArtifact(CultureInfo culture) => CoreRes.Get(nameof (MalformedArtifact), culture);

    public static string UnauthorizedFollow(object arg0, object arg1, object arg2) => CoreRes.Format(nameof (UnauthorizedFollow), arg0, arg1, arg2);

    public static string UnauthorizedFollow(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (UnauthorizedFollow), culture, arg0, arg1, arg2);
    }

    public static string UnauthorizedUnfollow(object arg0, object arg1) => CoreRes.Format(nameof (UnauthorizedUnfollow), arg0, arg1);

    public static string UnauthorizedUnfollow(object arg0, object arg1, CultureInfo culture) => CoreRes.Format(nameof (UnauthorizedUnfollow), culture, arg0, arg1);

    public static string MeMacroDisplayString() => CoreRes.Get(nameof (MeMacroDisplayString));

    public static string MeMacroDisplayString(CultureInfo culture) => CoreRes.Get(nameof (MeMacroDisplayString), culture);

    public static string ProjectMacroDisplayString() => CoreRes.Get(nameof (ProjectMacroDisplayString));

    public static string ProjectMacroDisplayString(CultureInfo culture) => CoreRes.Get(nameof (ProjectMacroDisplayString), culture);

    public static string CouldNotReadIdentityForActee(object arg0, object arg1) => CoreRes.Format(nameof (CouldNotReadIdentityForActee), arg0, arg1);

    public static string CouldNotReadIdentityForActee(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (CouldNotReadIdentityForActee), culture, arg0, arg1);
    }

    public static string RoleExpansionExceeded(object arg0, object arg1, object arg2) => CoreRes.Format(nameof (RoleExpansionExceeded), arg0, arg1, arg2);

    public static string RoleExpansionExceeded(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (RoleExpansionExceeded), culture, arg0, arg1, arg2);
    }

    public static string NoEmailFormatterAssociatedWithType(object arg0) => CoreRes.Format(nameof (NoEmailFormatterAssociatedWithType), arg0);

    public static string NoEmailFormatterAssociatedWithType(object arg0, CultureInfo culture) => CoreRes.Format(nameof (NoEmailFormatterAssociatedWithType), culture, arg0);

    public static string NotificationEventHasToBeXml(object arg0) => CoreRes.Format(nameof (NotificationEventHasToBeXml), arg0);

    public static string NotificationEventHasToBeXml(object arg0, CultureInfo culture) => CoreRes.Format(nameof (NotificationEventHasToBeXml), culture, arg0);

    public static string EventConditionFieldNotFound(object arg0) => CoreRes.Format(nameof (EventConditionFieldNotFound), arg0);

    public static string EventConditionFieldNotFound(object arg0, CultureInfo culture) => CoreRes.Format(nameof (EventConditionFieldNotFound), culture, arg0);

    public static string EventConditionDynamicPredicateNotFound(object arg0) => CoreRes.Format(nameof (EventConditionDynamicPredicateNotFound), arg0);

    public static string EventConditionDynamicPredicateNotFound(object arg0, CultureInfo culture) => CoreRes.Format(nameof (EventConditionDynamicPredicateNotFound), culture, arg0);

    public static string EventConditionDynamicPredicateThrewException(object arg0) => CoreRes.Format(nameof (EventConditionDynamicPredicateThrewException), arg0);

    public static string EventConditionDynamicPredicateThrewException(
      object arg0,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (EventConditionDynamicPredicateThrewException), culture, arg0);
    }

    public static string EventTypeDoesNotExistException(object arg0) => CoreRes.Format(nameof (EventTypeDoesNotExistException), arg0);

    public static string EventTypeDoesNotExistException(object arg0, CultureInfo culture) => CoreRes.Format(nameof (EventTypeDoesNotExistException), culture, arg0);

    public static string EventSubscriptionNotFound() => CoreRes.Get(nameof (EventSubscriptionNotFound));

    public static string EventSubscriptionNotFound(CultureInfo culture) => CoreRes.Get(nameof (EventSubscriptionNotFound), culture);

    public static string InvalidEventData() => CoreRes.Get(nameof (InvalidEventData));

    public static string InvalidEventData(CultureInfo culture) => CoreRes.Get(nameof (InvalidEventData), culture);

    public static string NotificationFields(object arg0, object arg1) => CoreRes.Format(nameof (NotificationFields), arg0, arg1);

    public static string NotificationFields(object arg0, object arg1, CultureInfo culture) => CoreRes.Format(nameof (NotificationFields), culture, arg0, arg1);

    public static string EventConditionSyntaxError(object arg0) => CoreRes.Format(nameof (EventConditionSyntaxError), arg0);

    public static string EventConditionSyntaxError(object arg0, CultureInfo culture) => CoreRes.Format(nameof (EventConditionSyntaxError), culture, arg0);

    public static string EventConditionExpected(object arg0) => CoreRes.Format(nameof (EventConditionExpected), arg0);

    public static string EventConditionExpected(object arg0, CultureInfo culture) => CoreRes.Format(nameof (EventConditionExpected), culture, arg0);

    public static string EventConditionExpectedBoolean(object arg0) => CoreRes.Format(nameof (EventConditionExpectedBoolean), arg0);

    public static string EventConditionExpectedBoolean(object arg0, CultureInfo culture) => CoreRes.Format(nameof (EventConditionExpectedBoolean), culture, arg0);

    public static string EventConditionExpectedInt() => CoreRes.Get(nameof (EventConditionExpectedInt));

    public static string EventConditionExpectedInt(CultureInfo culture) => CoreRes.Get(nameof (EventConditionExpectedInt), culture);

    public static string EventConditionUnexpectedEndOfFile() => CoreRes.Get(nameof (EventConditionUnexpectedEndOfFile));

    public static string EventConditionUnexpectedEndOfFile(CultureInfo culture) => CoreRes.Get(nameof (EventConditionUnexpectedEndOfFile), culture);

    public static string EventConditionUnexpectedEndOfLine() => CoreRes.Get(nameof (EventConditionUnexpectedEndOfLine));

    public static string EventConditionUnexpectedEndOfLine(CultureInfo culture) => CoreRes.Get(nameof (EventConditionUnexpectedEndOfLine), culture);

    public static string EventSubscriptionInvalidEmailEmpty() => CoreRes.Get(nameof (EventSubscriptionInvalidEmailEmpty));

    public static string EventSubscriptionInvalidEmailEmpty(CultureInfo culture) => CoreRes.Get(nameof (EventSubscriptionInvalidEmailEmpty), culture);

    public static string EventSubscriptionInvalidUri(object arg0) => CoreRes.Format(nameof (EventSubscriptionInvalidUri), arg0);

    public static string EventSubscriptionInvalidUri(object arg0, CultureInfo culture) => CoreRes.Format(nameof (EventSubscriptionInvalidUri), culture, arg0);

    public static string EventSubscriptionInvalidEmail(object arg0) => CoreRes.Format(nameof (EventSubscriptionInvalidEmail), arg0);

    public static string EventSubscriptionInvalidEmail(object arg0, CultureInfo culture) => CoreRes.Format(nameof (EventSubscriptionInvalidEmail), culture, arg0);

    public static string InvalidSubscriptionFilter() => CoreRes.Get(nameof (InvalidSubscriptionFilter));

    public static string InvalidSubscriptionFilter(CultureInfo culture) => CoreRes.Get(nameof (InvalidSubscriptionFilter), culture);

    public static string InvalidIdentityException(object arg0) => CoreRes.Format(nameof (InvalidIdentityException), arg0);

    public static string InvalidIdentityException(object arg0, CultureInfo culture) => CoreRes.Format(nameof (InvalidIdentityException), culture, arg0);

    public static string InvalidSubscriptionExpression(object arg0) => CoreRes.Format(nameof (InvalidSubscriptionExpression), arg0);

    public static string InvalidSubscriptionExpression(object arg0, CultureInfo culture) => CoreRes.Format(nameof (InvalidSubscriptionExpression), culture, arg0);

    public static string SubscriptionChannelMustBeSpecified() => CoreRes.Get(nameof (SubscriptionChannelMustBeSpecified));

    public static string SubscriptionChannelMustBeSpecified(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionChannelMustBeSpecified), culture);

    public static string SubscriptionFilterMustBeSpecified() => CoreRes.Get(nameof (SubscriptionFilterMustBeSpecified));

    public static string SubscriptionFilterMustBeSpecified(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionFilterMustBeSpecified), culture);

    public static string ArtifactIsRequired() => CoreRes.Get(nameof (ArtifactIsRequired));

    public static string ArtifactIsRequired(CultureInfo culture) => CoreRes.Get(nameof (ArtifactIsRequired), culture);

    public static string ArtifactTypeOrUri() => CoreRes.Get(nameof (ArtifactTypeOrUri));

    public static string ArtifactTypeOrUri(CultureInfo culture) => CoreRes.Get(nameof (ArtifactTypeOrUri), culture);

    public static string ErrorLoadSubscriptionAdapterByFilter(object arg0) => CoreRes.Format(nameof (ErrorLoadSubscriptionAdapterByFilter), arg0);

    public static string ErrorLoadSubscriptionAdapterByFilter(object arg0, CultureInfo culture) => CoreRes.Format(nameof (ErrorLoadSubscriptionAdapterByFilter), culture, arg0);

    public static string ErrorLoadSubscriptionAdapterByMatcher(object arg0, object arg1) => CoreRes.Format(nameof (ErrorLoadSubscriptionAdapterByMatcher), arg0, arg1);

    public static string ErrorLoadSubscriptionAdapterByMatcher(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (ErrorLoadSubscriptionAdapterByMatcher), culture, arg0, arg1);
    }

    public static string InvalidConsumerProperty(object arg0) => CoreRes.Format(nameof (InvalidConsumerProperty), arg0);

    public static string InvalidConsumerProperty(object arg0, CultureInfo culture) => CoreRes.Format(nameof (InvalidConsumerProperty), culture, arg0);

    public static string MustSpecifySubscriptionId() => CoreRes.Get(nameof (MustSpecifySubscriptionId));

    public static string MustSpecifySubscriptionId(CultureInfo culture) => CoreRes.Get(nameof (MustSpecifySubscriptionId), culture);

    public static string SubscriptionFieldNotSupported(object arg0) => CoreRes.Format(nameof (SubscriptionFieldNotSupported), arg0);

    public static string SubscriptionFieldNotSupported(object arg0, CultureInfo culture) => CoreRes.Format(nameof (SubscriptionFieldNotSupported), culture, arg0);

    public static string FieldCannotBeUpdated(object arg0, object arg1) => CoreRes.Format(nameof (FieldCannotBeUpdated), arg0, arg1);

    public static string FieldCannotBeUpdated(object arg0, object arg1, CultureInfo culture) => CoreRes.Format(nameof (FieldCannotBeUpdated), culture, arg0, arg1);

    public static string FilterTypeCantBeChanged(object arg0, object arg1, object arg2) => CoreRes.Format(nameof (FilterTypeCantBeChanged), arg0, arg1, arg2);

    public static string FilterTypeCantBeChanged(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (FilterTypeCantBeChanged), culture, arg0, arg1, arg2);
    }

    public static string SubscriptionDoesNotSupportOperation(object arg0, object arg1) => CoreRes.Format(nameof (SubscriptionDoesNotSupportOperation), arg0, arg1);

    public static string SubscriptionDoesNotSupportOperation(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (SubscriptionDoesNotSupportOperation), culture, arg0, arg1);
    }

    public static string InvalidValueException(object arg0, object arg1) => CoreRes.Format(nameof (InvalidValueException), arg0, arg1);

    public static string InvalidValueException(object arg0, object arg1, CultureInfo culture) => CoreRes.Format(nameof (InvalidValueException), culture, arg0, arg1);

    public static string DefaultSubscriptionMustHaveInclusions() => CoreRes.Get(nameof (DefaultSubscriptionMustHaveInclusions));

    public static string DefaultSubscriptionMustHaveInclusions(CultureInfo culture) => CoreRes.Get(nameof (DefaultSubscriptionMustHaveInclusions), culture);

    public static string IgnoreSubscriptionsMustHaveInclusionsOrConditions() => CoreRes.Get(nameof (IgnoreSubscriptionsMustHaveInclusionsOrConditions));

    public static string IgnoreSubscriptionsMustHaveInclusionsOrConditions(CultureInfo culture) => CoreRes.Get(nameof (IgnoreSubscriptionsMustHaveInclusionsOrConditions), culture);

    public static string SubscriptionIdCannotBeEmpty() => CoreRes.Get(nameof (SubscriptionIdCannotBeEmpty));

    public static string SubscriptionIdCannotBeEmpty(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionIdCannotBeEmpty), culture);

    public static string ErrorEventTypeNotSupported(object arg0) => CoreRes.Format(nameof (ErrorEventTypeNotSupported), arg0);

    public static string ErrorEventTypeNotSupported(object arg0, CultureInfo culture) => CoreRes.Format(nameof (ErrorEventTypeNotSupported), culture, arg0);

    public static string CouldNotDeserializeRoleBasedExpression(object arg0, object arg1) => CoreRes.Format(nameof (CouldNotDeserializeRoleBasedExpression), arg0, arg1);

    public static string CouldNotDeserializeRoleBasedExpression(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (CouldNotDeserializeRoleBasedExpression), culture, arg0, arg1);
    }

    public static string DisablingSubscription(object arg0, object arg1) => CoreRes.Format(nameof (DisablingSubscription), arg0, arg1);

    public static string DisablingSubscription(object arg0, object arg1, CultureInfo culture) => CoreRes.Format(nameof (DisablingSubscription), culture, arg0, arg1);

    public static string InvalidRoleBasedExpression(object arg0) => CoreRes.Format(nameof (InvalidRoleBasedExpression), arg0);

    public static string InvalidRoleBasedExpression(object arg0, CultureInfo culture) => CoreRes.Format(nameof (InvalidRoleBasedExpression), culture, arg0);

    public static string DisableSubscriptionBlockedByAdmin(object arg0) => CoreRes.Format(nameof (DisableSubscriptionBlockedByAdmin), arg0);

    public static string DisableSubscriptionBlockedByAdmin(object arg0, CultureInfo culture) => CoreRes.Format(nameof (DisableSubscriptionBlockedByAdmin), culture, arg0);

    public static string SubscriptionOnlySupportsStatusUpdate(object arg0) => CoreRes.Format(nameof (SubscriptionOnlySupportsStatusUpdate), arg0);

    public static string SubscriptionOnlySupportsStatusUpdate(object arg0, CultureInfo culture) => CoreRes.Format(nameof (SubscriptionOnlySupportsStatusUpdate), culture, arg0);

    public static string TemplateNotFoundException(object arg0) => CoreRes.Format(nameof (TemplateNotFoundException), arg0);

    public static string TemplateNotFoundException(object arg0, CultureInfo culture) => CoreRes.Format(nameof (TemplateNotFoundException), culture, arg0);

    public static string NotificationEmailFormatError(object arg0, object arg1) => CoreRes.Format(nameof (NotificationEmailFormatError), arg0, arg1);

    public static string NotificationEmailFormatError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (NotificationEmailFormatError), culture, arg0, arg1);
    }

    public static string NotificationXslNotFound(object arg0) => CoreRes.Format(nameof (NotificationXslNotFound), arg0);

    public static string NotificationXslNotFound(object arg0, CultureInfo culture) => CoreRes.Format(nameof (NotificationXslNotFound), culture, arg0);

    public static string MustacheTemplateInvalidPartialReference(object arg0) => CoreRes.Format(nameof (MustacheTemplateInvalidPartialReference), arg0);

    public static string MustacheTemplateInvalidPartialReference(object arg0, CultureInfo culture) => CoreRes.Format(nameof (MustacheTemplateInvalidPartialReference), culture, arg0);

    public static string MustacheContributedTemplateUndefinedMessage(object arg0) => CoreRes.Format(nameof (MustacheContributedTemplateUndefinedMessage), arg0);

    public static string MustacheContributedTemplateUndefinedMessage(
      object arg0,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (MustacheContributedTemplateUndefinedMessage), culture, arg0);
    }

    public static string MustacheTemplateInvalidArgumentsMessage() => CoreRes.Get(nameof (MustacheTemplateInvalidArgumentsMessage));

    public static string MustacheTemplateInvalidArgumentsMessage(CultureInfo culture) => CoreRes.Get(nameof (MustacheTemplateInvalidArgumentsMessage), culture);

    public static string ContributedShouldBeGroup(object arg0) => CoreRes.Format(nameof (ContributedShouldBeGroup), arg0);

    public static string ContributedShouldBeGroup(object arg0, CultureInfo culture) => CoreRes.Format(nameof (ContributedShouldBeGroup), culture, arg0);

    public static string MustProvideAtLeastOneCondition() => CoreRes.Get(nameof (MustProvideAtLeastOneCondition));

    public static string MustProvideAtLeastOneCondition(CultureInfo culture) => CoreRes.Get(nameof (MustProvideAtLeastOneCondition), culture);

    public static string MatcherNotsupportedException(object arg0) => CoreRes.Format(nameof (MatcherNotsupportedException), arg0);

    public static string MatcherNotsupportedException(object arg0, CultureInfo culture) => CoreRes.Format(nameof (MatcherNotsupportedException), culture, arg0);

    public static string ContributedEventNameNotSupported(object arg0) => CoreRes.Format(nameof (ContributedEventNameNotSupported), arg0);

    public static string ContributedEventNameNotSupported(object arg0, CultureInfo culture) => CoreRes.Format(nameof (ContributedEventNameNotSupported), culture, arg0);

    public static string LegacyEventNameNotSupported(object arg0) => CoreRes.Format(nameof (LegacyEventNameNotSupported), arg0);

    public static string LegacyEventNameNotSupported(object arg0, CultureInfo culture) => CoreRes.Format(nameof (LegacyEventNameNotSupported), culture, arg0);

    public static string SubscriptionOptOutNotSupported(object arg0) => CoreRes.Format(nameof (SubscriptionOptOutNotSupported), arg0);

    public static string SubscriptionOptOutNotSupported(object arg0, CultureInfo culture) => CoreRes.Format(nameof (SubscriptionOptOutNotSupported), culture, arg0);

    public static string TypeHasNoDefaultConstructor(object arg0) => CoreRes.Format(nameof (TypeHasNoDefaultConstructor), arg0);

    public static string TypeHasNoDefaultConstructor(object arg0, CultureInfo culture) => CoreRes.Format(nameof (TypeHasNoDefaultConstructor), culture, arg0);

    public static string SubscriptionPendingDeletion() => CoreRes.Get(nameof (SubscriptionPendingDeletion));

    public static string SubscriptionPendingDeletion(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionPendingDeletion), culture);

    public static string EmailNotificationViewLink() => CoreRes.Get(nameof (EmailNotificationViewLink));

    public static string EmailNotificationViewLink(CultureInfo culture) => CoreRes.Get(nameof (EmailNotificationViewLink), culture);

    public static string IgnoreSubscriptionMustBeEveryoneGroup() => CoreRes.Get(nameof (IgnoreSubscriptionMustBeEveryoneGroup));

    public static string IgnoreSubscriptionMustBeEveryoneGroup(CultureInfo culture) => CoreRes.Get(nameof (IgnoreSubscriptionMustBeEveryoneGroup), culture);

    public static string IgnoreSubscriptionOwnerMustBeAdmin() => CoreRes.Get(nameof (IgnoreSubscriptionOwnerMustBeAdmin));

    public static string IgnoreSubscriptionOwnerMustBeAdmin(CultureInfo culture) => CoreRes.Get(nameof (IgnoreSubscriptionOwnerMustBeAdmin), culture);

    public static string InvalidNotificationKey() => CoreRes.Get(nameof (InvalidNotificationKey));

    public static string InvalidNotificationKey(CultureInfo culture) => CoreRes.Get(nameof (InvalidNotificationKey), culture);

    public static string UnauthorizedSuspend() => CoreRes.Get(nameof (UnauthorizedSuspend));

    public static string UnauthorizedSuspend(CultureInfo culture) => CoreRes.Get(nameof (UnauthorizedSuspend), culture);

    public static string SubscriptionJailedByNotificationsVolume() => CoreRes.Get(nameof (SubscriptionJailedByNotificationsVolume));

    public static string SubscriptionJailedByNotificationsVolume(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionJailedByNotificationsVolume), culture);

    public static string EventBacklogDelay(object arg0, object arg1, object arg2, object arg3) => CoreRes.Format(nameof (EventBacklogDelay), arg0, arg1, arg2, arg3);

    public static string EventBacklogDelay(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (EventBacklogDelay), culture, arg0, arg1, arg2, arg3);
    }

    public static string EventProcessingUnhealthy(object arg0) => CoreRes.Format(nameof (EventProcessingUnhealthy), arg0);

    public static string EventProcessingUnhealthy(object arg0, CultureInfo culture) => CoreRes.Format(nameof (EventProcessingUnhealthy), culture, arg0);

    public static string NotificationDeliveryUnhealthy(object arg0) => CoreRes.Format(nameof (NotificationDeliveryUnhealthy), arg0);

    public static string NotificationDeliveryUnhealthy(object arg0, CultureInfo culture) => CoreRes.Format(nameof (NotificationDeliveryUnhealthy), culture, arg0);

    public static string NotitifcationBacklogDelay(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return CoreRes.Format(nameof (NotitifcationBacklogDelay), arg0, arg1, arg2, arg3);
    }

    public static string NotitifcationBacklogDelay(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (NotitifcationBacklogDelay), culture, arg0, arg1, arg2, arg3);
    }

    public static string BlockSubscriptionsNoOptOut(object arg0) => CoreRes.Format(nameof (BlockSubscriptionsNoOptOut), arg0);

    public static string BlockSubscriptionsNoOptOut(object arg0, CultureInfo culture) => CoreRes.Format(nameof (BlockSubscriptionsNoOptOut), culture, arg0);

    public static string CantSubscribeToChannel(object arg0) => CoreRes.Format(nameof (CantSubscribeToChannel), arg0);

    public static string CantSubscribeToChannel(object arg0, CultureInfo culture) => CoreRes.Format(nameof (CantSubscribeToChannel), culture, arg0);

    public static string ErrorEventDoesntSupportUserSubscriptions(object arg0) => CoreRes.Format(nameof (ErrorEventDoesntSupportUserSubscriptions), arg0);

    public static string ErrorEventDoesntSupportUserSubscriptions(object arg0, CultureInfo culture) => CoreRes.Format(nameof (ErrorEventDoesntSupportUserSubscriptions), culture, arg0);

    public static string SubscriptionStatusDefaultMessage(object arg0) => CoreRes.Format(nameof (SubscriptionStatusDefaultMessage), arg0);

    public static string SubscriptionStatusDefaultMessage(object arg0, CultureInfo culture) => CoreRes.Format(nameof (SubscriptionStatusDefaultMessage), culture, arg0);

    public static string FollowingArtifact(object arg0, object arg1) => CoreRes.Format(nameof (FollowingArtifact), arg0, arg1);

    public static string FollowingArtifact(object arg0, object arg1, CultureInfo culture) => CoreRes.Format(nameof (FollowingArtifact), culture, arg0, arg1);

    public static string UnauthorizedCreateSubscription() => CoreRes.Get(nameof (UnauthorizedCreateSubscription));

    public static string UnauthorizedCreateSubscription(CultureInfo culture) => CoreRes.Get(nameof (UnauthorizedCreateSubscription), culture);

    public static string UnauthorizedEvaluateSubscription() => CoreRes.Get(nameof (UnauthorizedEvaluateSubscription));

    public static string UnauthorizedEvaluateSubscription(CultureInfo culture) => CoreRes.Get(nameof (UnauthorizedEvaluateSubscription), culture);

    public static string UnauthorizedUpdateSubscription() => CoreRes.Get(nameof (UnauthorizedUpdateSubscription));

    public static string UnauthorizedUpdateSubscription(CultureInfo culture) => CoreRes.Get(nameof (UnauthorizedUpdateSubscription), culture);

    public static string AllowedFieldUpdatesForContributedSubscription() => CoreRes.Get(nameof (AllowedFieldUpdatesForContributedSubscription));

    public static string AllowedFieldUpdatesForContributedSubscription(CultureInfo culture) => CoreRes.Get(nameof (AllowedFieldUpdatesForContributedSubscription), culture);

    public static string SubscriptionOptOutError() => CoreRes.Get(nameof (SubscriptionOptOutError));

    public static string SubscriptionOptOutError(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionOptOutError), culture);

    public static string SubscriptionNotOptOutable() => CoreRes.Get(nameof (SubscriptionNotOptOutable));

    public static string SubscriptionNotOptOutable(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionNotOptOutable), culture);

    public static string SubscriptionOptOutUnAuth() => CoreRes.Get(nameof (SubscriptionOptOutUnAuth));

    public static string SubscriptionOptOutUnAuth(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionOptOutUnAuth), culture);

    public static string SystemSubscriptionNotOptOutable() => CoreRes.Get(nameof (SystemSubscriptionNotOptOutable));

    public static string SystemSubscriptionNotOptOutable(CultureInfo culture) => CoreRes.Get(nameof (SystemSubscriptionNotOptOutable), culture);

    public static string TemplateCriteriaNotMetException(object arg0) => CoreRes.Format(nameof (TemplateCriteriaNotMetException), arg0);

    public static string TemplateCriteriaNotMetException(object arg0, CultureInfo culture) => CoreRes.Format(nameof (TemplateCriteriaNotMetException), culture, arg0);

    public static string SubscriptionReasonPersonal() => CoreRes.Get(nameof (SubscriptionReasonPersonal));

    public static string SubscriptionReasonPersonal(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionReasonPersonal), culture);

    public static string SubscriptionReasonSystem() => CoreRes.Get(nameof (SubscriptionReasonSystem));

    public static string SubscriptionReasonSystem(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionReasonSystem), culture);

    public static string SubscriptionReasonFollows(object arg0) => CoreRes.Format(nameof (SubscriptionReasonFollows), arg0);

    public static string SubscriptionReasonFollows(object arg0, CultureInfo culture) => CoreRes.Format(nameof (SubscriptionReasonFollows), culture, arg0);

    public static string SubscriptionUnsubscribeActionName() => CoreRes.Get(nameof (SubscriptionUnsubscribeActionName));

    public static string SubscriptionUnsubscribeActionName(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionUnsubscribeActionName), culture);

    public static string ArtifactUnfollowActionName() => CoreRes.Get(nameof (ArtifactUnfollowActionName));

    public static string ArtifactUnfollowActionName(CultureInfo culture) => CoreRes.Get(nameof (ArtifactUnfollowActionName), culture);

    public static string ErrorLastModifiedByInvalid(object arg0) => CoreRes.Format(nameof (ErrorLastModifiedByInvalid), arg0);

    public static string ErrorLastModifiedByInvalid(object arg0, CultureInfo culture) => CoreRes.Format(nameof (ErrorLastModifiedByInvalid), culture, arg0);

    public static string ErrorSubscriberInvalid(object arg0) => CoreRes.Format(nameof (ErrorSubscriberInvalid), arg0);

    public static string ErrorSubscriberInvalid(object arg0, CultureInfo culture) => CoreRes.Format(nameof (ErrorSubscriberInvalid), culture, arg0);

    public static string IdentityInactiveException(object arg0, object arg1) => CoreRes.Format(nameof (IdentityInactiveException), arg0, arg1);

    public static string IdentityInactiveException(object arg0, object arg1, CultureInfo culture) => CoreRes.Format(nameof (IdentityInactiveException), culture, arg0, arg1);

    public static string SubscriptionReasonGroup(object arg0) => CoreRes.Format(nameof (SubscriptionReasonGroup), arg0);

    public static string SubscriptionReasonGroup(object arg0, CultureInfo culture) => CoreRes.Format(nameof (SubscriptionReasonGroup), culture, arg0);

    public static string InvalidQueryFlag() => CoreRes.Get(nameof (InvalidQueryFlag));

    public static string InvalidQueryFlag(CultureInfo culture) => CoreRes.Get(nameof (InvalidQueryFlag), culture);

    public static string ErrorLoadFilterMatcher(object arg0) => CoreRes.Format(nameof (ErrorLoadFilterMatcher), arg0);

    public static string ErrorLoadFilterMatcher(object arg0, CultureInfo culture) => CoreRes.Format(nameof (ErrorLoadFilterMatcher), culture, arg0);

    public static string UnauthorizedChangeStatusSubscription() => CoreRes.Get(nameof (UnauthorizedChangeStatusSubscription));

    public static string UnauthorizedChangeStatusSubscription(CultureInfo culture) => CoreRes.Get(nameof (UnauthorizedChangeStatusSubscription), culture);

    public static string BlockFilterNotSupportException() => CoreRes.Get(nameof (BlockFilterNotSupportException));

    public static string BlockFilterNotSupportException(CultureInfo culture) => CoreRes.Get(nameof (BlockFilterNotSupportException), culture);

    public static string NotificationStatisticInvalidDate() => CoreRes.Get(nameof (NotificationStatisticInvalidDate));

    public static string NotificationStatisticInvalidDate(CultureInfo culture) => CoreRes.Get(nameof (NotificationStatisticInvalidDate), culture);

    public static string NotificationStatisticInvalidHitCount() => CoreRes.Get(nameof (NotificationStatisticInvalidHitCount));

    public static string NotificationStatisticInvalidHitCount(CultureInfo culture) => CoreRes.Get(nameof (NotificationStatisticInvalidHitCount), culture);

    public static string NotificationStatisticInvalidPath() => CoreRes.Get(nameof (NotificationStatisticInvalidPath));

    public static string NotificationStatisticInvalidPath(CultureInfo culture) => CoreRes.Get(nameof (NotificationStatisticInvalidPath), culture);

    public static string NotificationStatisticInvalidType() => CoreRes.Get(nameof (NotificationStatisticInvalidType));

    public static string NotificationStatisticInvalidType(CultureInfo culture) => CoreRes.Get(nameof (NotificationStatisticInvalidType), culture);

    public static string RequiredEmailAddressErrorMessage() => CoreRes.Get(nameof (RequiredEmailAddressErrorMessage));

    public static string RequiredEmailAddressErrorMessage(CultureInfo culture) => CoreRes.Get(nameof (RequiredEmailAddressErrorMessage), culture);

    public static string UnauthorizedSubscriberDeliveryPreferenceUpdate(object arg0) => CoreRes.Format(nameof (UnauthorizedSubscriberDeliveryPreferenceUpdate), arg0);

    public static string UnauthorizedSubscriberDeliveryPreferenceUpdate(
      object arg0,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (UnauthorizedSubscriberDeliveryPreferenceUpdate), culture, arg0);
    }

    public static string UnsupportedDeliveryPreference(object arg0) => CoreRes.Format(nameof (UnsupportedDeliveryPreference), arg0);

    public static string UnsupportedDeliveryPreference(object arg0, CultureInfo culture) => CoreRes.Format(nameof (UnsupportedDeliveryPreference), culture, arg0);

    public static string UnsupportedUpdateDeliveryPreference() => CoreRes.Get(nameof (UnsupportedUpdateDeliveryPreference));

    public static string UnsupportedUpdateDeliveryPreference(CultureInfo culture) => CoreRes.Get(nameof (UnsupportedUpdateDeliveryPreference), culture);

    public static string SubscriptionReasonDefault() => CoreRes.Get(nameof (SubscriptionReasonDefault));

    public static string SubscriptionReasonDefault(CultureInfo culture) => CoreRes.Get(nameof (SubscriptionReasonDefault), culture);

    public static string MatcherEventTypeCombinationNotSupported(object arg0, object arg1) => CoreRes.Format(nameof (MatcherEventTypeCombinationNotSupported), arg0, arg1);

    public static string MatcherEventTypeCombinationNotSupported(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (MatcherEventTypeCombinationNotSupported), culture, arg0, arg1);
    }

    public static string UnauthorizedPublishEvent() => CoreRes.Get(nameof (UnauthorizedPublishEvent));

    public static string UnauthorizedPublishEvent(CultureInfo culture) => CoreRes.Get(nameof (UnauthorizedPublishEvent), culture);

    public static string SubscriptionTraceErrorBadSource(object arg0) => CoreRes.Format(nameof (SubscriptionTraceErrorBadSource), arg0);

    public static string SubscriptionTraceErrorBadSource(object arg0, CultureInfo culture) => CoreRes.Format(nameof (SubscriptionTraceErrorBadSource), culture, arg0);

    public static string SubscriptionTraceErrorLoggerInvalid(object arg0, object arg1) => CoreRes.Format(nameof (SubscriptionTraceErrorLoggerInvalid), arg0, arg1);

    public static string SubscriptionTraceErrorLoggerInvalid(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (SubscriptionTraceErrorLoggerInvalid), culture, arg0, arg1);
    }

    public static string InvalidUpdateTracingParametersDeliveryResultsNotSupported() => CoreRes.Get(nameof (InvalidUpdateTracingParametersDeliveryResultsNotSupported));

    public static string InvalidUpdateTracingParametersDeliveryResultsNotSupported(
      CultureInfo culture)
    {
      return CoreRes.Get(nameof (InvalidUpdateTracingParametersDeliveryResultsNotSupported), culture);
    }

    public static string InvalidUpdateTracingParametersEmpty() => CoreRes.Get(nameof (InvalidUpdateTracingParametersEmpty));

    public static string InvalidUpdateTracingParametersEmpty(CultureInfo culture) => CoreRes.Get(nameof (InvalidUpdateTracingParametersEmpty), culture);

    public static string SubscriptionMemberMissingPermissionsException(object arg0) => CoreRes.Format(nameof (SubscriptionMemberMissingPermissionsException), arg0);

    public static string SubscriptionMemberMissingPermissionsException(
      object arg0,
      CultureInfo culture)
    {
      return CoreRes.Format(nameof (SubscriptionMemberMissingPermissionsException), culture, arg0);
    }

    public static string UnauthorizedAdminSettings() => CoreRes.Get(nameof (UnauthorizedAdminSettings));

    public static string UnauthorizedAdminSettings(CultureInfo culture) => CoreRes.Get(nameof (UnauthorizedAdminSettings), culture);

    public static string SubscriptionProjectInvalidException(object arg0) => CoreRes.Format(nameof (SubscriptionProjectInvalidException), arg0);

    public static string SubscriptionProjectInvalidException(object arg0, CultureInfo culture) => CoreRes.Format(nameof (SubscriptionProjectInvalidException), culture, arg0);

    public static string BlockUserOptOutAuditMessage(object arg0, object arg1) => CoreRes.Format(nameof (BlockUserOptOutAuditMessage), arg0, arg1);

    public static string BlockUserOptOutAuditMessage(object arg0, object arg1, CultureInfo culture) => CoreRes.Format(nameof (BlockUserOptOutAuditMessage), culture, arg0, arg1);

    public static string StatusChangeAuditMessage(object arg0, object arg1) => CoreRes.Format(nameof (StatusChangeAuditMessage), arg0, arg1);

    public static string StatusChangeAuditMessage(object arg0, object arg1, CultureInfo culture) => CoreRes.Format(nameof (StatusChangeAuditMessage), culture, arg0, arg1);

    public static string EventTransformTeplateNotFound(object arg0) => CoreRes.Format(nameof (EventTransformTeplateNotFound), arg0);

    public static string EventTransformTeplateNotFound(object arg0, CultureInfo culture) => CoreRes.Format(nameof (EventTransformTeplateNotFound), culture, arg0);
  }
}
