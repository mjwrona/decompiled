// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Resources
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Agile.Web
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.TeamFoundation.Agile.Web.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.TeamFoundation.Agile.Web.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.TeamFoundation.Agile.Web.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Agile.Web.Resources.Get(resourceName) : Microsoft.TeamFoundation.Agile.Web.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.TeamFoundation.Agile.Web.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Agile.Web.Resources.GetInt(resourceName) : (int) Microsoft.TeamFoundation.Agile.Web.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.TeamFoundation.Agile.Web.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Agile.Web.Resources.GetBool(resourceName) : (bool) Microsoft.TeamFoundation.Agile.Web.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.TeamFoundation.Agile.Web.Resources.Get(resourceName, culture);
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

    public static string MissingTimelineDateMessage() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MissingTimelineDateMessage));

    public static string MissingTimelineDateMessage(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MissingTimelineDateMessage), culture);

    public static string CanNotGroup() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CanNotGroup));

    public static string CanNotGroup(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CanNotGroup), culture);

    public static string UnexpectedNode(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (UnexpectedNode), arg0);

    public static string UnexpectedNode(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (UnexpectedNode), culture, arg0);

    public static string MissingGroupingFirst() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MissingGroupingFirst));

    public static string MissingGroupingFirst(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MissingGroupingFirst), culture);

    public static string MissingGroupingIndices() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MissingGroupingIndices));

    public static string MissingGroupingIndices(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MissingGroupingIndices), culture);

    public static string MissingGroupingLast() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MissingGroupingLast));

    public static string MissingGroupingLast(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MissingGroupingLast), culture);

    public static string UnableToProcessValueNode(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (UnableToProcessValueNode), arg0);

    public static string UnableToProcessValueNode(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (UnableToProcessValueNode), culture, arg0);

    public static string QueryEditorInvalidOperatorForSpecialValue(object arg0, object arg1) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (QueryEditorInvalidOperatorForSpecialValue), arg0, arg1);

    public static string QueryEditorInvalidOperatorForSpecialValue(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (QueryEditorInvalidOperatorForSpecialValue), culture, arg0, arg1);
    }

    public static string QueryFilterInvalidMacro(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (QueryFilterInvalidMacro), arg0);

    public static string QueryFilterInvalidMacro(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (QueryFilterInvalidMacro), culture, arg0);

    public static string CardRules_InvalidType(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_InvalidType), arg0);

    public static string CardRules_InvalidType(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_InvalidType), culture, arg0);

    public static string CardRules_DuplicateName(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_DuplicateName), arg0);

    public static string CardRules_DuplicateName(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_DuplicateName), culture, arg0);

    public static string CardRuleAttribute_InvalidSettings(object arg0, object arg1) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRuleAttribute_InvalidSettings), arg0, arg1);

    public static string CardRuleAttribute_InvalidSettings(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRuleAttribute_InvalidSettings), culture, arg0, arg1);
    }

    public static string CardRuleAttribute_SettingValueExceedsLimit(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRuleAttribute_SettingValueExceedsLimit), arg0);

    public static string CardRuleAttribute_SettingValueExceedsLimit(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRuleAttribute_SettingValueExceedsLimit), culture, arg0);
    }

    public static string CardRuleAttribute_SettingNameExceedsLimit(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRuleAttribute_SettingNameExceedsLimit), arg0);

    public static string CardRuleAttribute_SettingNameExceedsLimit(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRuleAttribute_SettingNameExceedsLimit), culture, arg0);

    public static string CardRuleAttribute_MissingName() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardRuleAttribute_MissingName));

    public static string CardRuleAttribute_MissingName(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardRuleAttribute_MissingName), culture);

    public static string CardRuleAttribute_MissingValue() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardRuleAttribute_MissingValue));

    public static string CardRuleAttribute_MissingValue(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardRuleAttribute_MissingValue), culture);

    public static string CardRule_NoAttributesSupplied(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRule_NoAttributesSupplied), arg0);

    public static string CardRule_NoAttributesSupplied(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRule_NoAttributesSupplied), culture, arg0);

    public static string CardRules_InvalidLogicalOperator(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_InvalidLogicalOperator), arg0);

    public static string CardRules_InvalidLogicalOperator(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_InvalidLogicalOperator), culture, arg0);

    public static string CardRules_EmptyOrNullWiql() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardRules_EmptyOrNullWiql));

    public static string CardRules_EmptyOrNullWiql(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardRules_EmptyOrNullWiql), culture);

    public static string CardRules_InvalidOperator(object arg0, object arg1) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_InvalidOperator), arg0, arg1);

    public static string CardRules_InvalidOperator(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_InvalidOperator), culture, arg0, arg1);

    public static string CardRules_UnSupportedType(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_UnSupportedType), arg0);

    public static string CardRules_UnSupportedType(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_UnSupportedType), culture, arg0);

    public static string CardRules_InvalidField(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_InvalidField), arg0);

    public static string CardRules_InvalidField(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_InvalidField), culture, arg0);

    public static string CardRules_WIQLGroups() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardRules_WIQLGroups));

    public static string CardRules_WIQLGroups(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardRules_WIQLGroups), culture);

    public static string CardRules_MaxLimitForConditions(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_MaxLimitForConditions), arg0);

    public static string CardRules_MaxLimitForConditions(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_MaxLimitForConditions), culture, arg0);

    public static string CardRules_MaxLimitForNumberOfRules(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_MaxLimitForNumberOfRules), arg0);

    public static string CardRules_MaxLimitForNumberOfRules(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_MaxLimitForNumberOfRules), culture, arg0);

    public static string CardRules_RuleNameExceedsLimit(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_RuleNameExceedsLimit), arg0);

    public static string CardRules_RuleNameExceedsLimit(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRules_RuleNameExceedsLimit), culture, arg0);

    public static string CardRules_RuleNameNullOrEmpty() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardRules_RuleNameNullOrEmpty));

    public static string CardRules_RuleNameNullOrEmpty(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardRules_RuleNameNullOrEmpty), culture);

    public static string EmptyBoardId() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (EmptyBoardId));

    public static string EmptyBoardId(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (EmptyBoardId), culture);

    public static string CardCustomization_BadBoardType() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardCustomization_BadBoardType));

    public static string CardCustomization_BadBoardType(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardCustomization_BadBoardType), culture);

    public static string CardCustomization_SettingsNULL() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardCustomization_SettingsNULL));

    public static string CardCustomization_SettingsNULL(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardCustomization_SettingsNULL), culture);

    public static string CardCustomization_CardKeyCannotBeNull() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardCustomization_CardKeyCannotBeNull));

    public static string CardCustomization_CardKeyCannotBeNull(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardCustomization_CardKeyCannotBeNull), culture);

    public static string CardCustomization_CardValueCannotBeNull(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_CardValueCannotBeNull), arg0);

    public static string CardCustomization_CardValueCannotBeNull(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_CardValueCannotBeNull), culture, arg0);

    public static string CardCustomization_RequiredFieldMissing(object arg0, object arg1) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_RequiredFieldMissing), arg0, arg1);

    public static string CardCustomization_RequiredFieldMissing(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_RequiredFieldMissing), culture, arg0, arg1);
    }

    public static string CardCustomization_DuplicateCards(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_DuplicateCards), arg0);

    public static string CardCustomization_DuplicateCards(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_DuplicateCards), culture, arg0);

    public static string CardCustomization_DuplicateFields(object arg0, object arg1) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_DuplicateFields), arg0, arg1);

    public static string CardCustomization_DuplicateFields(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_DuplicateFields), culture, arg0, arg1);
    }

    public static string CardCustomization_FieldIdentifierMissing(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_FieldIdentifierMissing), arg0);

    public static string CardCustomization_FieldIdentifierMissing(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_FieldIdentifierMissing), culture, arg0);

    public static string CardCustomization_InvalidCardType(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_InvalidCardType), arg0);

    public static string CardCustomization_InvalidCardType(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_InvalidCardType), culture, arg0);

    public static string CardCustomization_InvalidFieldType(object arg0, object arg1) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_InvalidFieldType), arg0, arg1);

    public static string CardCustomization_InvalidFieldType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_InvalidFieldType), culture, arg0, arg1);
    }

    public static string CardCustomization_NoCardsFound() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardCustomization_NoCardsFound));

    public static string CardCustomization_NoCardsFound(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardCustomization_NoCardsFound), culture);

    public static string CardCustomization_UnknownBoardId(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_UnknownBoardId), arg0);

    public static string CardCustomization_UnknownBoardId(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_UnknownBoardId), culture, arg0);

    public static string CardCustomization_MaxAdditionalFieldsExceeded(object arg0, object arg1) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_MaxAdditionalFieldsExceeded), arg0, arg1);

    public static string CardCustomization_MaxAdditionalFieldsExceeded(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardCustomization_MaxAdditionalFieldsExceeded), culture, arg0, arg1);
    }

    public static string PlanPermissionEditMessage() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (PlanPermissionEditMessage));

    public static string PlanPermissionEditMessage(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (PlanPermissionEditMessage), culture);

    public static string PlanPermissionDeleteMessage() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (PlanPermissionDeleteMessage));

    public static string PlanPermissionDeleteMessage(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (PlanPermissionDeleteMessage), culture);

    public static string PlanNameTooLongMessage(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (PlanNameTooLongMessage), arg0);

    public static string PlanNameTooLongMessage(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (PlanNameTooLongMessage), culture, arg0);

    public static string PlanNameContainsInvalidCharacters() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (PlanNameContainsInvalidCharacters));

    public static string PlanNameContainsInvalidCharacters(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (PlanNameContainsInvalidCharacters), culture);

    public static string PlanDescriptionTooLongMessage(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (PlanDescriptionTooLongMessage), arg0);

    public static string PlanDescriptionTooLongMessage(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (PlanDescriptionTooLongMessage), culture, arg0);

    public static string TeamMissingOrAccessDenied() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (TeamMissingOrAccessDenied));

    public static string TeamMissingOrAccessDenied(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (TeamMissingOrAccessDenied), culture);

    public static string MaximumExpandedTeamCountExceeded() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MaximumExpandedTeamCountExceeded));

    public static string MaximumExpandedTeamCountExceeded(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MaximumExpandedTeamCountExceeded), culture);

    public static string MaximumTeamFieldCountExceeded() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MaximumTeamFieldCountExceeded));

    public static string MaximumTeamFieldCountExceeded(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MaximumTeamFieldCountExceeded), culture);

    public static string IterationOverlap() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (IterationOverlap));

    public static string IterationOverlap(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (IterationOverlap), culture);

    public static string BacklogInError() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (BacklogInError));

    public static string BacklogInError(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (BacklogInError), culture);

    public static string MissingTeamFieldValue() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MissingTeamFieldValue));

    public static string MissingTeamFieldValue(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MissingTeamFieldValue), culture);

    public static string NoIterationsForTeam() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (NoIterationsForTeam));

    public static string NoIterationsForTeam(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (NoIterationsForTeam), culture);

    public static string FilterCriteriaTooLongMessage(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (FilterCriteriaTooLongMessage), arg0);

    public static string FilterCriteriaTooLongMessage(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (FilterCriteriaTooLongMessage), culture, arg0);

    public static string DTLCriteriaMaxLimitForConditions(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaMaxLimitForConditions), arg0);

    public static string DTLCriteriaMaxLimitForConditions(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaMaxLimitForConditions), culture, arg0);

    public static string DTLCriteriaWIQLGroups() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (DTLCriteriaWIQLGroups));

    public static string DTLCriteriaWIQLGroups(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (DTLCriteriaWIQLGroups), culture);

    public static string DTLCriteriaMissingField(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaMissingField), arg0);

    public static string DTLCriteriaMissingField(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaMissingField), culture, arg0);

    public static string MarkerIncorrectFormat() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MarkerIncorrectFormat));

    public static string MarkerIncorrectFormat(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MarkerIncorrectFormat), culture);

    public static string DateRangeExceedLimitMessage(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DateRangeExceedLimitMessage), arg0);

    public static string DateRangeExceedLimitMessage(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DateRangeExceedLimitMessage), culture, arg0);

    public static string MarkerLabelValueOutOfRange(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (MarkerLabelValueOutOfRange), arg0);

    public static string MarkerLabelValueOutOfRange(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (MarkerLabelValueOutOfRange), culture, arg0);

    public static string UnexpandedTeamLimitExceededMessage(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (UnexpandedTeamLimitExceededMessage), arg0);

    public static string UnexpandedTeamLimitExceededMessage(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (UnexpandedTeamLimitExceededMessage), culture, arg0);

    public static string ExceededMaximumNumberOfMarkers(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (ExceededMaximumNumberOfMarkers), arg0);

    public static string ExceededMaximumNumberOfMarkers(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (ExceededMaximumNumberOfMarkers), culture, arg0);

    public static string DTLCriteriaInvalidField(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaInvalidField), arg0);

    public static string DTLCriteriaInvalidField(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaInvalidField), culture, arg0);

    public static string DTLCriteriaNotSupportedField(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaNotSupportedField), arg0);

    public static string DTLCriteriaNotSupportedField(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaNotSupportedField), culture, arg0);

    public static string DTLCriteriaInvalidLogicalOperator(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaInvalidLogicalOperator), arg0);

    public static string DTLCriteriaInvalidLogicalOperator(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaInvalidLogicalOperator), culture, arg0);

    public static string DTLCriteriaInvalidOperator(object arg0, object arg1) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaInvalidOperator), arg0, arg1);

    public static string DTLCriteriaInvalidOperator(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaInvalidOperator), culture, arg0, arg1);

    public static string DTLCriteriaNotSupportedFieldValue(object arg0, object arg1) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaNotSupportedFieldValue), arg0, arg1);

    public static string DTLCriteriaNotSupportedFieldValue(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (DTLCriteriaNotSupportedFieldValue), culture, arg0, arg1);
    }

    public static string MarkerColorValueOutOfRange() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MarkerColorValueOutOfRange));

    public static string MarkerColorValueOutOfRange(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (MarkerColorValueOutOfRange), culture);

    public static string CardRuleAttribute_InvalidRowIdAttribute(object arg0) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRuleAttribute_InvalidRowIdAttribute), arg0);

    public static string CardRuleAttribute_InvalidRowIdAttribute(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Format(nameof (CardRuleAttribute_InvalidRowIdAttribute), culture, arg0);

    public static string CardRules_MeMacroNotSupported() => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardRules_MeMacroNotSupported));

    public static string CardRules_MeMacroNotSupported(CultureInfo culture) => Microsoft.TeamFoundation.Agile.Web.Resources.Get(nameof (CardRules_MeMacroNotSupported), culture);
  }
}
