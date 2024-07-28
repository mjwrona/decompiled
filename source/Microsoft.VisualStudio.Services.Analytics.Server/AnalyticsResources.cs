// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsResources
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal static class AnalyticsResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (AnalyticsResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AnalyticsResources.s_resMgr;

    private static string Get(string resourceName) => AnalyticsResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AnalyticsResources.Get(resourceName) : AnalyticsResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AnalyticsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AnalyticsResources.GetInt(resourceName) : (int) AnalyticsResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AnalyticsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AnalyticsResources.GetBool(resourceName) : (bool) AnalyticsResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AnalyticsResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AnalyticsResources.Get(resourceName, culture);
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

    public static string ENTITY_SET_NOT_FOUND(object arg0) => AnalyticsResources.Format(nameof (ENTITY_SET_NOT_FOUND), arg0);

    public static string ENTITY_SET_NOT_FOUND(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (ENTITY_SET_NOT_FOUND), culture, arg0);

    public static string QUERY_PARAMETER_NOT_SUPPORTED(object arg0) => AnalyticsResources.Format(nameof (QUERY_PARAMETER_NOT_SUPPORTED), arg0);

    public static string QUERY_PARAMETER_NOT_SUPPORTED(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (QUERY_PARAMETER_NOT_SUPPORTED), culture, arg0);

    public static string QUERY_TOO_WIDE() => AnalyticsResources.Get(nameof (QUERY_TOO_WIDE));

    public static string QUERY_TOO_WIDE(CultureInfo culture) => AnalyticsResources.Get(nameof (QUERY_TOO_WIDE), culture);

    public static string URI_QUERY_STRING_INVALID(object arg0) => AnalyticsResources.Format(nameof (URI_QUERY_STRING_INVALID), arg0);

    public static string URI_QUERY_STRING_INVALID(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (URI_QUERY_STRING_INVALID), culture, arg0);

    public static string TRANSFORM_FAILED(object arg0, object arg1) => AnalyticsResources.Format(nameof (TRANSFORM_FAILED), arg0, arg1);

    public static string TRANSFORM_FAILED(object arg0, object arg1, CultureInfo culture) => AnalyticsResources.Format(nameof (TRANSFORM_FAILED), culture, arg0, arg1);

    public static string TEAM_NOT_FOUND(object arg0) => AnalyticsResources.Format(nameof (TEAM_NOT_FOUND), arg0);

    public static string TEAM_NOT_FOUND(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (TEAM_NOT_FOUND), culture, arg0);

    public static string MAX_SIZE_HEADER_INVALID(object arg0) => AnalyticsResources.Format(nameof (MAX_SIZE_HEADER_INVALID), arg0);

    public static string MAX_SIZE_HEADER_INVALID(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (MAX_SIZE_HEADER_INVALID), culture, arg0);

    public static string QUERY_EXCEEDS_PREFERED_MAX_SIZE(object arg0, object arg1) => AnalyticsResources.Format(nameof (QUERY_EXCEEDS_PREFERED_MAX_SIZE), arg0, arg1);

    public static string QUERY_EXCEEDS_PREFERED_MAX_SIZE(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (QUERY_EXCEEDS_PREFERED_MAX_SIZE), culture, arg0, arg1);
    }

    public static string ODATA_SELECT_EXPAND_EMPTY() => AnalyticsResources.Get(nameof (ODATA_SELECT_EXPAND_EMPTY));

    public static string ODATA_SELECT_EXPAND_EMPTY(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_SELECT_EXPAND_EMPTY), culture);

    public static string ODATA_PATH_TEMPLATE_ARENT_SUPPORTED() => AnalyticsResources.Get(nameof (ODATA_PATH_TEMPLATE_ARENT_SUPPORTED));

    public static string ODATA_PATH_TEMPLATE_ARENT_SUPPORTED(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_PATH_TEMPLATE_ARENT_SUPPORTED), culture);

    public static string NO_VIEW_ANALYTICS_PERMISSION(object arg0) => AnalyticsResources.Format(nameof (NO_VIEW_ANALYTICS_PERMISSION), arg0);

    public static string NO_VIEW_ANALYTICS_PERMISSION(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (NO_VIEW_ANALYTICS_PERMISSION), culture, arg0);

    public static string STAKEHOLDERS_NOT_ALLOWED_ANALYTICS() => AnalyticsResources.Get(nameof (STAKEHOLDERS_NOT_ALLOWED_ANALYTICS));

    public static string STAKEHOLDERS_NOT_ALLOWED_ANALYTICS(CultureInfo culture) => AnalyticsResources.Get(nameof (STAKEHOLDERS_NOT_ALLOWED_ANALYTICS), culture);

    public static string MODEL_NOT_READY() => AnalyticsResources.Get(nameof (MODEL_NOT_READY));

    public static string MODEL_NOT_READY(CultureInfo culture) => AnalyticsResources.Get(nameof (MODEL_NOT_READY), culture);

    public static string MICROSERVICE_MODEL_READY() => AnalyticsResources.Get(nameof (MICROSERVICE_MODEL_READY));

    public static string MICROSERVICE_MODEL_READY(CultureInfo culture) => AnalyticsResources.Get(nameof (MICROSERVICE_MODEL_READY), culture);

    public static string DATE_FORMAT_ERROR() => AnalyticsResources.Get(nameof (DATE_FORMAT_ERROR));

    public static string DATE_FORMAT_ERROR(CultureInfo culture) => AnalyticsResources.Get(nameof (DATE_FORMAT_ERROR), culture);

    public static string QUERY_LEVELS_NOT_ALLOWED() => AnalyticsResources.Get(nameof (QUERY_LEVELS_NOT_ALLOWED));

    public static string QUERY_LEVELS_NOT_ALLOWED(CultureInfo culture) => AnalyticsResources.Get(nameof (QUERY_LEVELS_NOT_ALLOWED), culture);

    public static string QUERY_TOO_WIDE_EntityName(object arg0) => AnalyticsResources.Format(nameof (QUERY_TOO_WIDE_EntityName), arg0);

    public static string QUERY_TOO_WIDE_EntityName(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (QUERY_TOO_WIDE_EntityName), culture, arg0);

    public static string ANALYTICS_EXTENSION_NOT_FOUND() => AnalyticsResources.Get(nameof (ANALYTICS_EXTENSION_NOT_FOUND));

    public static string ANALYTICS_EXTENSION_NOT_FOUND(CultureInfo culture) => AnalyticsResources.Get(nameof (ANALYTICS_EXTENSION_NOT_FOUND), culture);

    public static string ODATA_BATCH_CHANGESET_INVALID() => AnalyticsResources.Get(nameof (ODATA_BATCH_CHANGESET_INVALID));

    public static string ODATA_BATCH_CHANGESET_INVALID(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_BATCH_CHANGESET_INVALID), culture);

    public static string ODATA_BATCH_QUERY_SIZE_INVALID() => AnalyticsResources.Get(nameof (ODATA_BATCH_QUERY_SIZE_INVALID));

    public static string ODATA_BATCH_QUERY_SIZE_INVALID(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_BATCH_QUERY_SIZE_INVALID), culture);

    public static string ODATA_NOT_SUPPORTED_VERSION(object arg0) => AnalyticsResources.Format(nameof (ODATA_NOT_SUPPORTED_VERSION), arg0);

    public static string ODATA_NOT_SUPPORTED_VERSION(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (ODATA_NOT_SUPPORTED_VERSION), culture, arg0);

    public static string ODATA_NOT_SUPPORTED_VERSION_PRIVATE_PREVIEW() => AnalyticsResources.Get(nameof (ODATA_NOT_SUPPORTED_VERSION_PRIVATE_PREVIEW));

    public static string ODATA_NOT_SUPPORTED_VERSION_PRIVATE_PREVIEW(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_NOT_SUPPORTED_VERSION_PRIVATE_PREVIEW), culture);

    public static string ODATA_NOT_SUPPORTED_VERSION_RELEASED(object arg0) => AnalyticsResources.Format(nameof (ODATA_NOT_SUPPORTED_VERSION_RELEASED), arg0);

    public static string ODATA_NOT_SUPPORTED_VERSION_RELEASED(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (ODATA_NOT_SUPPORTED_VERSION_RELEASED), culture, arg0);

    public static string ODATA_BAD_VERSION_SUFFIX_MESSAGE() => AnalyticsResources.Get(nameof (ODATA_BAD_VERSION_SUFFIX_MESSAGE));

    public static string ODATA_BAD_VERSION_SUFFIX_MESSAGE(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_BAD_VERSION_SUFFIX_MESSAGE), culture);

    public static string ODATA_VERSION_NOT_FOUND() => AnalyticsResources.Get(nameof (ODATA_VERSION_NOT_FOUND));

    public static string ODATA_VERSION_NOT_FOUND(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_VERSION_NOT_FOUND), culture);

    public static string ODATA_QUERY_DEPRECATED() => AnalyticsResources.Get(nameof (ODATA_QUERY_DEPRECATED));

    public static string ODATA_QUERY_DEPRECATED(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_QUERY_DEPRECATED), culture);

    public static string ODATA_QUERY_DISTINCT_COLUMNS_IN_LAST_GROUPBY() => AnalyticsResources.Get(nameof (ODATA_QUERY_DISTINCT_COLUMNS_IN_LAST_GROUPBY));

    public static string ODATA_QUERY_DISTINCT_COLUMNS_IN_LAST_GROUPBY(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_QUERY_DISTINCT_COLUMNS_IN_LAST_GROUPBY), culture);

    public static string ODATA_QUERY_NO_SELECT_OR_APPLY() => AnalyticsResources.Get(nameof (ODATA_QUERY_NO_SELECT_OR_APPLY));

    public static string ODATA_QUERY_NO_SELECT_OR_APPLY(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_QUERY_NO_SELECT_OR_APPLY), culture);

    public static string ODATA_QUERY_PARENT_CHILD_RELATIONS() => AnalyticsResources.Get(nameof (ODATA_QUERY_PARENT_CHILD_RELATIONS));

    public static string ODATA_QUERY_PARENT_CHILD_RELATIONS(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_QUERY_PARENT_CHILD_RELATIONS), culture);

    public static string ODATA_QUERY_TOO_WIDE(object arg0) => AnalyticsResources.Format(nameof (ODATA_QUERY_TOO_WIDE), arg0);

    public static string ODATA_QUERY_TOO_WIDE(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (ODATA_QUERY_TOO_WIDE), culture, arg0);

    public static string ODATA_SNAPSHOT_WITHOUT_AGGREGATION() => AnalyticsResources.Get(nameof (ODATA_SNAPSHOT_WITHOUT_AGGREGATION));

    public static string ODATA_SNAPSHOT_WITHOUT_AGGREGATION(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_SNAPSHOT_WITHOUT_AGGREGATION), culture);

    public static string ODATA_QUERY_NOT_SUPPORTED() => AnalyticsResources.Get(nameof (ODATA_QUERY_NOT_SUPPORTED));

    public static string ODATA_QUERY_NOT_SUPPORTED(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_QUERY_NOT_SUPPORTED), culture);

    public static string ODATA_QUERY_WITH_COUNTDISTINCT_NOT_SUPPORTED() => AnalyticsResources.Get(nameof (ODATA_QUERY_WITH_COUNTDISTINCT_NOT_SUPPORTED));

    public static string ODATA_QUERY_WITH_COUNTDISTINCT_NOT_SUPPORTED(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_QUERY_WITH_COUNTDISTINCT_NOT_SUPPORTED), culture);

    public static string ODATA_QUERY_PARENT_EXPAND_TOO_DEEP() => AnalyticsResources.Get(nameof (ODATA_QUERY_PARENT_EXPAND_TOO_DEEP));

    public static string ODATA_QUERY_PARENT_EXPAND_TOO_DEEP(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_QUERY_PARENT_EXPAND_TOO_DEEP), culture);

    public static string ANALYTICS_NOT_ENABLED() => AnalyticsResources.Get(nameof (ANALYTICS_NOT_ENABLED));

    public static string ANALYTICS_NOT_ENABLED(CultureInfo culture) => AnalyticsResources.Get(nameof (ANALYTICS_NOT_ENABLED), culture);

    public static string ANALYTICS_PAUSED() => AnalyticsResources.Get(nameof (ANALYTICS_PAUSED));

    public static string ANALYTICS_PAUSED(CultureInfo culture) => AnalyticsResources.Get(nameof (ANALYTICS_PAUSED), culture);

    public static string ANALYTICS_DELETING() => AnalyticsResources.Get(nameof (ANALYTICS_DELETING));

    public static string ANALYTICS_DELETING(CultureInfo culture) => AnalyticsResources.Get(nameof (ANALYTICS_DELETING), culture);

    public static string BUGS_NAME() => AnalyticsResources.Get(nameof (BUGS_NAME));

    public static string BUGS_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (BUGS_NAME), culture);

    public static string BY_MONTH() => AnalyticsResources.Get(nameof (BY_MONTH));

    public static string BY_MONTH(CultureInfo culture) => AnalyticsResources.Get(nameof (BY_MONTH), culture);

    public static string CURRENT_VIEW_DESCRIPTION_FORMAT(object arg0) => AnalyticsResources.Format(nameof (CURRENT_VIEW_DESCRIPTION_FORMAT), arg0);

    public static string CURRENT_VIEW_DESCRIPTION_FORMAT(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (CURRENT_VIEW_DESCRIPTION_FORMAT), culture, arg0);

    public static string CURRENT_VIEW_NAME_FORMAT(object arg0) => AnalyticsResources.Format(nameof (CURRENT_VIEW_NAME_FORMAT), arg0);

    public static string CURRENT_VIEW_NAME_FORMAT(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (CURRENT_VIEW_NAME_FORMAT), culture, arg0);

    public static string DAILY() => AnalyticsResources.Get(nameof (DAILY));

    public static string DAILY(CultureInfo culture) => AnalyticsResources.Get(nameof (DAILY), culture);

    public static string DAYS_30() => AnalyticsResources.Get(nameof (DAYS_30));

    public static string DAYS_30(CultureInfo culture) => AnalyticsResources.Get(nameof (DAYS_30), culture);

    public static string HISTORICAL_DESCRIPTION_FORMAT(object arg0, object arg1, object arg2) => AnalyticsResources.Format(nameof (HISTORICAL_DESCRIPTION_FORMAT), arg0, arg1, arg2);

    public static string HISTORICAL_DESCRIPTION_FORMAT(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (HISTORICAL_DESCRIPTION_FORMAT), culture, arg0, arg1, arg2);
    }

    public static string HISTORICAL_DESCRIPTION_OTHER_FORMAT(object arg0, object arg1) => AnalyticsResources.Format(nameof (HISTORICAL_DESCRIPTION_OTHER_FORMAT), arg0, arg1);

    public static string HISTORICAL_DESCRIPTION_OTHER_FORMAT(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (HISTORICAL_DESCRIPTION_OTHER_FORMAT), culture, arg0, arg1);
    }

    public static string SOME_HISTORY_NAME_FORMAT(object arg0, object arg1) => AnalyticsResources.Format(nameof (SOME_HISTORY_NAME_FORMAT), arg0, arg1);

    public static string SOME_HISTORY_NAME_FORMAT(object arg0, object arg1, CultureInfo culture) => AnalyticsResources.Format(nameof (SOME_HISTORY_NAME_FORMAT), culture, arg0, arg1);

    public static string MONTHLY() => AnalyticsResources.Get(nameof (MONTHLY));

    public static string MONTHLY(CultureInfo culture) => AnalyticsResources.Get(nameof (MONTHLY), culture);

    public static string WEEKLY() => AnalyticsResources.Get(nameof (WEEKLY));

    public static string WEEKLY(CultureInfo culture) => AnalyticsResources.Get(nameof (WEEKLY), culture);

    public static string WEEKS_26() => AnalyticsResources.Get(nameof (WEEKS_26));

    public static string WEEKS_26(CultureInfo culture) => AnalyticsResources.Get(nameof (WEEKS_26), culture);

    public static string WORK_ITEM_NAME() => AnalyticsResources.Get(nameof (WORK_ITEM_NAME));

    public static string WORK_ITEM_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (WORK_ITEM_NAME), culture);

    public static string WORK_ITEM_NAME_LOWER() => AnalyticsResources.Get(nameof (WORK_ITEM_NAME_LOWER));

    public static string WORK_ITEM_NAME_LOWER(CultureInfo culture) => AnalyticsResources.Get(nameof (WORK_ITEM_NAME_LOWER), culture);

    public static string VIEW_MISSING_MASHUP_FUNCTION_PARAMETER(object arg0, object arg1) => AnalyticsResources.Format(nameof (VIEW_MISSING_MASHUP_FUNCTION_PARAMETER), arg0, arg1);

    public static string VIEW_MISSING_MASHUP_FUNCTION_PARAMETER(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (VIEW_MISSING_MASHUP_FUNCTION_PARAMETER), culture, arg0, arg1);
    }

    public static string VIEW_SCHEMA_GENERATION_FAILED() => AnalyticsResources.Get(nameof (VIEW_SCHEMA_GENERATION_FAILED));

    public static string VIEW_SCHEMA_GENERATION_FAILED(CultureInfo culture) => AnalyticsResources.Get(nameof (VIEW_SCHEMA_GENERATION_FAILED), culture);

    public static string VIEW_ODATA_METADATA_READ_FAILED() => AnalyticsResources.Get(nameof (VIEW_ODATA_METADATA_READ_FAILED));

    public static string VIEW_ODATA_METADATA_READ_FAILED(CultureInfo culture) => AnalyticsResources.Get(nameof (VIEW_ODATA_METADATA_READ_FAILED), culture);

    public static string VIEW_TRANSLATE_MULTIPLE_SELECT_PATH_SEGMENTS(object arg0) => AnalyticsResources.Format(nameof (VIEW_TRANSLATE_MULTIPLE_SELECT_PATH_SEGMENTS), arg0);

    public static string VIEW_TRANSLATE_MULTIPLE_SELECT_PATH_SEGMENTS(
      object arg0,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (VIEW_TRANSLATE_MULTIPLE_SELECT_PATH_SEGMENTS), culture, arg0);
    }

    public static string VIEW_TRANSLATE_NON_PRIMITIVE_PROPERTY(object arg0) => AnalyticsResources.Format(nameof (VIEW_TRANSLATE_NON_PRIMITIVE_PROPERTY), arg0);

    public static string VIEW_TRANSLATE_NON_PRIMITIVE_PROPERTY(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (VIEW_TRANSLATE_NON_PRIMITIVE_PROPERTY), culture, arg0);

    public static string VIEW_BACKLOGS_NOT_FOUND(object arg0) => AnalyticsResources.Format(nameof (VIEW_BACKLOGS_NOT_FOUND), arg0);

    public static string VIEW_BACKLOGS_NOT_FOUND(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (VIEW_BACKLOGS_NOT_FOUND), culture, arg0);

    public static string ALL_HISTORY_BY_MONTH_NAME_FORMAT(object arg0) => AnalyticsResources.Format(nameof (ALL_HISTORY_BY_MONTH_NAME_FORMAT), arg0);

    public static string ALL_HISTORY_BY_MONTH_NAME_FORMAT(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (ALL_HISTORY_BY_MONTH_NAME_FORMAT), culture, arg0);

    public static string ExceptionViewsDefinitionRequired() => AnalyticsResources.Get(nameof (ExceptionViewsDefinitionRequired));

    public static string ExceptionViewsDefinitionRequired(CultureInfo culture) => AnalyticsResources.Get(nameof (ExceptionViewsDefinitionRequired), culture);

    public static string ExceptionViewsInvalidFilterOperator() => AnalyticsResources.Get(nameof (ExceptionViewsInvalidFilterOperator));

    public static string ExceptionViewsInvalidFilterOperator(CultureInfo culture) => AnalyticsResources.Get(nameof (ExceptionViewsInvalidFilterOperator), culture);

    public static string VIEW_INVALID_HISTORY_CONFIGURATION() => AnalyticsResources.Get(nameof (VIEW_INVALID_HISTORY_CONFIGURATION));

    public static string VIEW_INVALID_HISTORY_CONFIGURATION(CultureInfo culture) => AnalyticsResources.Get(nameof (VIEW_INVALID_HISTORY_CONFIGURATION), culture);

    public static string VIEW_INVALID_HISTORY_GRANULARITY_CONFIGURATION() => AnalyticsResources.Get(nameof (VIEW_INVALID_HISTORY_GRANULARITY_CONFIGURATION));

    public static string VIEW_INVALID_HISTORY_GRANULARITY_CONFIGURATION(CultureInfo culture) => AnalyticsResources.Get(nameof (VIEW_INVALID_HISTORY_GRANULARITY_CONFIGURATION), culture);

    public static string ODATA_PROPERTY_EXISTED_EARLIER(object arg0) => AnalyticsResources.Format(nameof (ODATA_PROPERTY_EXISTED_EARLIER), arg0);

    public static string ODATA_PROPERTY_EXISTED_EARLIER(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (ODATA_PROPERTY_EXISTED_EARLIER), culture, arg0);

    public static string ODATA_PROPERTY_NEVER_EXISTED(object arg0) => AnalyticsResources.Format(nameof (ODATA_PROPERTY_NEVER_EXISTED), arg0);

    public static string ODATA_PROPERTY_NEVER_EXISTED(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (ODATA_PROPERTY_NEVER_EXISTED), culture, arg0);

    public static string ExceptionUndefinedValueNotAllowed(object arg0) => AnalyticsResources.Format(nameof (ExceptionUndefinedValueNotAllowed), arg0);

    public static string ExceptionUndefinedValueNotAllowed(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (ExceptionUndefinedValueNotAllowed), culture, arg0);

    public static string InvalidVisibilityError() => AnalyticsResources.Get(nameof (InvalidVisibilityError));

    public static string InvalidVisibilityError(CultureInfo culture) => AnalyticsResources.Get(nameof (InvalidVisibilityError), culture);

    public static string QUERY_EUII_NOT_ALLOWED() => AnalyticsResources.Get(nameof (QUERY_EUII_NOT_ALLOWED));

    public static string QUERY_EUII_NOT_ALLOWED(CultureInfo culture) => AnalyticsResources.Get(nameof (QUERY_EUII_NOT_ALLOWED), culture);

    public static string NO_VIEW_ANALYTICS_PERMISSION_COLLECTION() => AnalyticsResources.Get(nameof (NO_VIEW_ANALYTICS_PERMISSION_COLLECTION));

    public static string NO_VIEW_ANALYTICS_PERMISSION_COLLECTION(CultureInfo culture) => AnalyticsResources.Get(nameof (NO_VIEW_ANALYTICS_PERMISSION_COLLECTION), culture);

    public static string ODATA_NOT_AUTHENTICATED_ERROR() => AnalyticsResources.Get(nameof (ODATA_NOT_AUTHENTICATED_ERROR));

    public static string ODATA_NOT_AUTHENTICATED_ERROR(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_NOT_AUTHENTICATED_ERROR), culture);

    public static string MODEL_SYNCING() => AnalyticsResources.Get(nameof (MODEL_SYNCING));

    public static string MODEL_SYNCING(CultureInfo culture) => AnalyticsResources.Get(nameof (MODEL_SYNCING), culture);

    public static string ODATA_CannotSerializerNull(object arg0) => AnalyticsResources.Format(nameof (ODATA_CannotSerializerNull), arg0);

    public static string ODATA_CannotSerializerNull(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (ODATA_CannotSerializerNull), culture, arg0);

    public static string ODATA_CannotWriteType(object arg0, object arg1) => AnalyticsResources.Format(nameof (ODATA_CannotWriteType), arg0, arg1);

    public static string ODATA_CannotWriteType(object arg0, object arg1, CultureInfo culture) => AnalyticsResources.Format(nameof (ODATA_CannotWriteType), culture, arg0, arg1);

    public static string ODATA_NullElementInCollection() => AnalyticsResources.Get(nameof (ODATA_NullElementInCollection));

    public static string ODATA_NullElementInCollection(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_NullElementInCollection), culture);

    public static string ODATA_TYPE_CANNOT_BE_SERIALIZED(object arg0, object arg1) => AnalyticsResources.Format(nameof (ODATA_TYPE_CANNOT_BE_SERIALIZED), arg0, arg1);

    public static string ODATA_TYPE_CANNOT_BE_SERIALIZED(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (ODATA_TYPE_CANNOT_BE_SERIALIZED), culture, arg0, arg1);
    }

    public static string ODATA_PROPERTY_EXISTED_IN_AGGREGATE(object arg0) => AnalyticsResources.Format(nameof (ODATA_PROPERTY_EXISTED_IN_AGGREGATE), arg0);

    public static string ODATA_PROPERTY_EXISTED_IN_AGGREGATE(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (ODATA_PROPERTY_EXISTED_IN_AGGREGATE), culture, arg0);

    public static string EXTENSION_INSTALL_FAIL_MESSAGE() => AnalyticsResources.Get(nameof (EXTENSION_INSTALL_FAIL_MESSAGE));

    public static string EXTENSION_INSTALL_FAIL_MESSAGE(CultureInfo culture) => AnalyticsResources.Get(nameof (EXTENSION_INSTALL_FAIL_MESSAGE), culture);

    public static string ARITHMETIC_EXPRESSIONS_WITHOUT_COMMON_NUMERIC_TYPES() => AnalyticsResources.Get(nameof (ARITHMETIC_EXPRESSIONS_WITHOUT_COMMON_NUMERIC_TYPES));

    public static string ARITHMETIC_EXPRESSIONS_WITHOUT_COMMON_NUMERIC_TYPES(CultureInfo culture) => AnalyticsResources.Get(nameof (ARITHMETIC_EXPRESSIONS_WITHOUT_COMMON_NUMERIC_TYPES), culture);

    public static string WRAPPER_EXCEPTION_WHEN_REQUEST_CANCELLED(object arg0) => AnalyticsResources.Format(nameof (WRAPPER_EXCEPTION_WHEN_REQUEST_CANCELLED), arg0);

    public static string WRAPPER_EXCEPTION_WHEN_REQUEST_CANCELLED(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (WRAPPER_EXCEPTION_WHEN_REQUEST_CANCELLED), culture, arg0);

    public static string ACLS_MISMATCH_BEFORE_AND_AFTER(object arg0, object arg1) => AnalyticsResources.Format(nameof (ACLS_MISMATCH_BEFORE_AND_AFTER), arg0, arg1);

    public static string ACLS_MISMATCH_BEFORE_AND_AFTER(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (ACLS_MISMATCH_BEFORE_AND_AFTER), culture, arg0, arg1);
    }

    public static string CANNOT_CONVERT_IDS(object arg0) => AnalyticsResources.Format(nameof (CANNOT_CONVERT_IDS), arg0);

    public static string CANNOT_CONVERT_IDS(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (CANNOT_CONVERT_IDS), culture, arg0);

    public static string COLLECTION_VALID_USER_DOES_NOT_HAVE_1_ACL(object arg0) => AnalyticsResources.Format(nameof (COLLECTION_VALID_USER_DOES_NOT_HAVE_1_ACL), arg0);

    public static string COLLECTION_VALID_USER_DOES_NOT_HAVE_1_ACL(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (COLLECTION_VALID_USER_DOES_NOT_HAVE_1_ACL), culture, arg0);

    public static string DO_NOT_ENUMERATE_ODATA_ENUMERABLE_MORE_THAN_ONCE() => AnalyticsResources.Get(nameof (DO_NOT_ENUMERATE_ODATA_ENUMERABLE_MORE_THAN_ONCE));

    public static string DO_NOT_ENUMERATE_ODATA_ENUMERABLE_MORE_THAN_ONCE(CultureInfo culture) => AnalyticsResources.Get(nameof (DO_NOT_ENUMERATE_ODATA_ENUMERABLE_MORE_THAN_ONCE), culture);

    public static string EFFECTIVE_ALLOW_CHANGED(object arg0, object arg1, object arg2) => AnalyticsResources.Format(nameof (EFFECTIVE_ALLOW_CHANGED), arg0, arg1, arg2);

    public static string EFFECTIVE_ALLOW_CHANGED(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (EFFECTIVE_ALLOW_CHANGED), culture, arg0, arg1, arg2);
    }

    public static string EFFECTIVE_DENY_CHANGED(object arg0, object arg1, object arg2) => AnalyticsResources.Format(nameof (EFFECTIVE_DENY_CHANGED), arg0, arg1, arg2);

    public static string EFFECTIVE_DENY_CHANGED(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (EFFECTIVE_DENY_CHANGED), culture, arg0, arg1, arg2);
    }

    public static string IDENTITIES_MISMATCH_BEFORE_AND_AFTER(object arg0, object arg1) => AnalyticsResources.Format(nameof (IDENTITIES_MISMATCH_BEFORE_AND_AFTER), arg0, arg1);

    public static string IDENTITIES_MISMATCH_BEFORE_AND_AFTER(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (IDENTITIES_MISMATCH_BEFORE_AND_AFTER), culture, arg0, arg1);
    }

    public static string INVALID_NODE() => AnalyticsResources.Get(nameof (INVALID_NODE));

    public static string INVALID_NODE(CultureInfo culture) => AnalyticsResources.Get(nameof (INVALID_NODE), culture);

    public static string METHOD_SHOULD_NOT_BE_CALLED() => AnalyticsResources.Get(nameof (METHOD_SHOULD_NOT_BE_CALLED));

    public static string METHOD_SHOULD_NOT_BE_CALLED(CultureInfo culture) => AnalyticsResources.Get(nameof (METHOD_SHOULD_NOT_BE_CALLED), culture);

    public static string MINVERSION_GREATER_THAN_MAXVERSION(object arg0, object arg1) => AnalyticsResources.Format(nameof (MINVERSION_GREATER_THAN_MAXVERSION), arg0, arg1);

    public static string MINVERSION_GREATER_THAN_MAXVERSION(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (MINVERSION_GREATER_THAN_MAXVERSION), culture, arg0, arg1);
    }

    public static string NO_ANALYTICS_PERMISSION_FOR_VALID_COLLECTION_USER(object arg0) => AnalyticsResources.Format(nameof (NO_ANALYTICS_PERMISSION_FOR_VALID_COLLECTION_USER), arg0);

    public static string NO_ANALYTICS_PERMISSION_FOR_VALID_COLLECTION_USER(
      object arg0,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (NO_ANALYTICS_PERMISSION_FOR_VALID_COLLECTION_USER), culture, arg0);
    }

    public static string NO_ANALYTICS_PERMISSION_FOR_VALID_PROJECT_USER(object arg0) => AnalyticsResources.Format(nameof (NO_ANALYTICS_PERMISSION_FOR_VALID_PROJECT_USER), arg0);

    public static string NO_ANALYTICS_PERMISSION_FOR_VALID_PROJECT_USER(
      object arg0,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (NO_ANALYTICS_PERMISSION_FOR_VALID_PROJECT_USER), culture, arg0);
    }

    public static string NO_ANALYTICS_VIEW_PERMISSION_FOR_VALID_USER(object arg0) => AnalyticsResources.Format(nameof (NO_ANALYTICS_VIEW_PERMISSION_FOR_VALID_USER), arg0);

    public static string NO_ANALYTICS_VIEW_PERMISSION_FOR_VALID_USER(
      object arg0,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (NO_ANALYTICS_VIEW_PERMISSION_FOR_VALID_USER), culture, arg0);
    }

    public static string PROJECT_VALID_USER_DOES_NOT_HAVE_1_ACL(object arg0) => AnalyticsResources.Format(nameof (PROJECT_VALID_USER_DOES_NOT_HAVE_1_ACL), arg0);

    public static string PROJECT_VALID_USER_DOES_NOT_HAVE_1_ACL(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (PROJECT_VALID_USER_DOES_NOT_HAVE_1_ACL), culture, arg0);

    public static string UNABLE_TO_SERIALIZE_THE_MODEL() => AnalyticsResources.Get(nameof (UNABLE_TO_SERIALIZE_THE_MODEL));

    public static string UNABLE_TO_SERIALIZE_THE_MODEL(CultureInfo culture) => AnalyticsResources.Get(nameof (UNABLE_TO_SERIALIZE_THE_MODEL), culture);

    public static string UNEXPECTED_PROPERTY(object arg0, object arg1) => AnalyticsResources.Format(nameof (UNEXPECTED_PROPERTY), arg0, arg1);

    public static string UNEXPECTED_PROPERTY(object arg0, object arg1, CultureInfo culture) => AnalyticsResources.Format(nameof (UNEXPECTED_PROPERTY), culture, arg0, arg1);

    public static string UNEXPECTED_TYPE(object arg0, object arg1) => AnalyticsResources.Format(nameof (UNEXPECTED_TYPE), arg0, arg1);

    public static string UNEXPECTED_TYPE(object arg0, object arg1, CultureInfo culture) => AnalyticsResources.Format(nameof (UNEXPECTED_TYPE), culture, arg0, arg1);

    public static string UNKNOWN_BINARY_OPERATOR() => AnalyticsResources.Get(nameof (UNKNOWN_BINARY_OPERATOR));

    public static string UNKNOWN_BINARY_OPERATOR(CultureInfo culture) => AnalyticsResources.Get(nameof (UNKNOWN_BINARY_OPERATOR), culture);

    public static string UNSUPPORTED_AGGREGATION_METHOD(object arg0) => AnalyticsResources.Format(nameof (UNSUPPORTED_AGGREGATION_METHOD), arg0);

    public static string UNSUPPORTED_AGGREGATION_METHOD(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (UNSUPPORTED_AGGREGATION_METHOD), culture, arg0);

    public static string UNSUPPORTED_CURRENT_PROJECT_FILTER_VISITOR_TYPES(object arg0) => AnalyticsResources.Format(nameof (UNSUPPORTED_CURRENT_PROJECT_FILTER_VISITOR_TYPES), arg0);

    public static string UNSUPPORTED_CURRENT_PROJECT_FILTER_VISITOR_TYPES(
      object arg0,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (UNSUPPORTED_CURRENT_PROJECT_FILTER_VISITOR_TYPES), culture, arg0);
    }

    public static string UNSUPPORTED_CUSTOM_FIELD(object arg0) => AnalyticsResources.Format(nameof (UNSUPPORTED_CUSTOM_FIELD), arg0);

    public static string UNSUPPORTED_CUSTOM_FIELD(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (UNSUPPORTED_CUSTOM_FIELD), culture, arg0);

    public static string UNSUPPORTED_SKIP_TOKEN_PAGING(object arg0) => AnalyticsResources.Format(nameof (UNSUPPORTED_SKIP_TOKEN_PAGING), arg0);

    public static string UNSUPPORTED_SKIP_TOKEN_PAGING(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (UNSUPPORTED_SKIP_TOKEN_PAGING), culture, arg0);

    public static string UNSUPPORTED_SQL_OPTIONS(object arg0, object arg1) => AnalyticsResources.Format(nameof (UNSUPPORTED_SQL_OPTIONS), arg0, arg1);

    public static string UNSUPPORTED_SQL_OPTIONS(object arg0, object arg1, CultureInfo culture) => AnalyticsResources.Format(nameof (UNSUPPORTED_SQL_OPTIONS), culture, arg0, arg1);

    public static string UNSUPPORTED_TRANSFORMATION(object arg0) => AnalyticsResources.Format(nameof (UNSUPPORTED_TRANSFORMATION), arg0);

    public static string UNSUPPORTED_TRANSFORMATION(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (UNSUPPORTED_TRANSFORMATION), culture, arg0);

    public static string UNSUPPORTED_UNARY_OPERATOR(object arg0) => AnalyticsResources.Format(nameof (UNSUPPORTED_UNARY_OPERATOR), arg0);

    public static string UNSUPPORTED_UNARY_OPERATOR(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (UNSUPPORTED_UNARY_OPERATOR), culture, arg0);

    public static string UNSUPPORTED_VALUE_MODE() => AnalyticsResources.Get(nameof (UNSUPPORTED_VALUE_MODE));

    public static string UNSUPPORTED_VALUE_MODE(CultureInfo culture) => AnalyticsResources.Get(nameof (UNSUPPORTED_VALUE_MODE), culture);

    public static string WIDGET_QUERY_IS_NOT_BUILTIN_SHAPE(object arg0, object arg1) => AnalyticsResources.Format(nameof (WIDGET_QUERY_IS_NOT_BUILTIN_SHAPE), arg0, arg1);

    public static string WIDGET_QUERY_IS_NOT_BUILTIN_SHAPE(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (WIDGET_QUERY_IS_NOT_BUILTIN_SHAPE), culture, arg0, arg1);
    }

    public static string AGGREGATE_EXCEPTIONS(object arg0, object arg1, object arg2, object arg3) => AnalyticsResources.Format(nameof (AGGREGATE_EXCEPTIONS), arg0, arg1, arg2, arg3);

    public static string AGGREGATE_EXCEPTIONS(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (AGGREGATE_EXCEPTIONS), culture, arg0, arg1, arg2, arg3);
    }

    public static string AGGREGATE_EXCEPTIONS_WITH_2_ARGUMENTS(object arg0, object arg1) => AnalyticsResources.Format(nameof (AGGREGATE_EXCEPTIONS_WITH_2_ARGUMENTS), arg0, arg1);

    public static string AGGREGATE_EXCEPTIONS_WITH_2_ARGUMENTS(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (AGGREGATE_EXCEPTIONS_WITH_2_ARGUMENTS), culture, arg0, arg1);
    }

    public static string ARGUMENT_HAS_TO_BE_NULL(object arg0, object arg1) => AnalyticsResources.Format(nameof (ARGUMENT_HAS_TO_BE_NULL), arg0, arg1);

    public static string ARGUMENT_HAS_TO_BE_NULL(object arg0, object arg1, CultureInfo culture) => AnalyticsResources.Format(nameof (ARGUMENT_HAS_TO_BE_NULL), culture, arg0, arg1);

    public static string DISABLE_STREAMS_LEFT() => AnalyticsResources.Get(nameof (DISABLE_STREAMS_LEFT));

    public static string DISABLE_STREAMS_LEFT(CultureInfo culture) => AnalyticsResources.Get(nameof (DISABLE_STREAMS_LEFT), culture);

    public static string ERROR_READING_GUID(object arg0) => AnalyticsResources.Format(nameof (ERROR_READING_GUID), arg0);

    public static string ERROR_READING_GUID(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (ERROR_READING_GUID), culture, arg0);

    public static string HAS_ENABLED_STREAMS_ALREADY(object arg0, object arg1) => AnalyticsResources.Format(nameof (HAS_ENABLED_STREAMS_ALREADY), arg0, arg1);

    public static string HAS_ENABLED_STREAMS_ALREADY(object arg0, object arg1, CultureInfo culture) => AnalyticsResources.Format(nameof (HAS_ENABLED_STREAMS_ALREADY), culture, arg0, arg1);

    public static string INVALID_TRANSFORM_DEFINITION(object arg0) => AnalyticsResources.Format(nameof (INVALID_TRANSFORM_DEFINITION), arg0);

    public static string INVALID_TRANSFORM_DEFINITION(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (INVALID_TRANSFORM_DEFINITION), culture, arg0);

    public static string JOB_COUNT_MISMATCH(object arg0, object arg1) => AnalyticsResources.Format(nameof (JOB_COUNT_MISMATCH), arg0, arg1);

    public static string JOB_COUNT_MISMATCH(object arg0, object arg1, CultureInfo culture) => AnalyticsResources.Format(nameof (JOB_COUNT_MISMATCH), culture, arg0, arg1);

    public static string KEYS_ONLY_NOT_SUPPORTED() => AnalyticsResources.Get(nameof (KEYS_ONLY_NOT_SUPPORTED));

    public static string KEYS_ONLY_NOT_SUPPORTED(CultureInfo culture) => AnalyticsResources.Get(nameof (KEYS_ONLY_NOT_SUPPORTED), culture);

    public static string LINE_NUMBER() => AnalyticsResources.Get(nameof (LINE_NUMBER));

    public static string LINE_NUMBER(CultureInfo culture) => AnalyticsResources.Get(nameof (LINE_NUMBER), culture);

    public static string LINE_POSITION() => AnalyticsResources.Get(nameof (LINE_POSITION));

    public static string LINE_POSITION(CultureInfo culture) => AnalyticsResources.Get(nameof (LINE_POSITION), culture);

    public static string METHOD_INTRODUCED_IN_VERSION(object arg0) => AnalyticsResources.Format(nameof (METHOD_INTRODUCED_IN_VERSION), arg0);

    public static string METHOD_INTRODUCED_IN_VERSION(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (METHOD_INTRODUCED_IN_VERSION), culture, arg0);

    public static string MISSING_REQUIRED_PROPERTY(object arg0) => AnalyticsResources.Format(nameof (MISSING_REQUIRED_PROPERTY), arg0);

    public static string MISSING_REQUIRED_PROPERTY(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (MISSING_REQUIRED_PROPERTY), culture, arg0);

    public static string NO_DATA_TO_STAGE() => AnalyticsResources.Get(nameof (NO_DATA_TO_STAGE));

    public static string NO_DATA_TO_STAGE(CultureInfo culture) => AnalyticsResources.Get(nameof (NO_DATA_TO_STAGE), culture);

    public static string NO_ENABLED_STREAMS(object arg0) => AnalyticsResources.Format(nameof (NO_ENABLED_STREAMS), arg0);

    public static string NO_ENABLED_STREAMS(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (NO_ENABLED_STREAMS), culture, arg0);

    public static string NO_EXPORT_SPROC(object arg0) => AnalyticsResources.Format(nameof (NO_EXPORT_SPROC), arg0);

    public static string NO_EXPORT_SPROC(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (NO_EXPORT_SPROC), culture, arg0);

    public static string PATH() => AnalyticsResources.Get(nameof (PATH));

    public static string PATH(CultureInfo culture) => AnalyticsResources.Get(nameof (PATH), culture);

    public static string PROPERTY_ALREADY_DEFINED(object arg0) => AnalyticsResources.Format(nameof (PROPERTY_ALREADY_DEFINED), arg0);

    public static string PROPERTY_ALREADY_DEFINED(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (PROPERTY_ALREADY_DEFINED), culture, arg0);

    public static string PROPERTY_SHOULD_BE_TRUE_WHEN_OTHER_PROPERTY_IS_USED(
      object arg0,
      object arg1)
    {
      return AnalyticsResources.Format(nameof (PROPERTY_SHOULD_BE_TRUE_WHEN_OTHER_PROPERTY_IS_USED), arg0, arg1);
    }

    public static string PROPERTY_SHOULD_BE_TRUE_WHEN_OTHER_PROPERTY_IS_USED(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (PROPERTY_SHOULD_BE_TRUE_WHEN_OTHER_PROPERTY_IS_USED), culture, arg0, arg1);
    }

    public static string SERVICE_LEVEL_EXCEPTION() => AnalyticsResources.Get(nameof (SERVICE_LEVEL_EXCEPTION));

    public static string SERVICE_LEVEL_EXCEPTION(CultureInfo culture) => AnalyticsResources.Get(nameof (SERVICE_LEVEL_EXCEPTION), culture);

    public static string SQL_COMMAND_NOT_BOUND_TO_CONTEXT(object arg0) => AnalyticsResources.Format(nameof (SQL_COMMAND_NOT_BOUND_TO_CONTEXT), arg0);

    public static string SQL_COMMAND_NOT_BOUND_TO_CONTEXT(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (SQL_COMMAND_NOT_BOUND_TO_CONTEXT), culture, arg0);

    public static string TRANSFORM_PRIORITY_CONFLICTS(object arg0, object arg1) => AnalyticsResources.Format(nameof (TRANSFORM_PRIORITY_CONFLICTS), arg0, arg1);

    public static string TRANSFORM_PRIORITY_CONFLICTS(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (TRANSFORM_PRIORITY_CONFLICTS), culture, arg0, arg1);
    }

    public static string UNABLE_TO_READ_TRANSFORM_RESULT() => AnalyticsResources.Get(nameof (UNABLE_TO_READ_TRANSFORM_RESULT));

    public static string UNABLE_TO_READ_TRANSFORM_RESULT(CultureInfo culture) => AnalyticsResources.Get(nameof (UNABLE_TO_READ_TRANSFORM_RESULT), culture);

    public static string UNEXPECTED_RESULT_SET() => AnalyticsResources.Get(nameof (UNEXPECTED_RESULT_SET));

    public static string UNEXPECTED_RESULT_SET(CultureInfo culture) => AnalyticsResources.Get(nameof (UNEXPECTED_RESULT_SET), culture);

    public static string UNEXPECTED_TOKEN(object arg0) => AnalyticsResources.Format(nameof (UNEXPECTED_TOKEN), arg0);

    public static string UNEXPECTED_TOKEN(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (UNEXPECTED_TOKEN), culture, arg0);

    public static string UNKNOWN_SQL_TABLE_TYPE(object arg0, object arg1) => AnalyticsResources.Format(nameof (UNKNOWN_SQL_TABLE_TYPE), arg0, arg1);

    public static string UNKNOWN_SQL_TABLE_TYPE(object arg0, object arg1, CultureInfo culture) => AnalyticsResources.Format(nameof (UNKNOWN_SQL_TABLE_TYPE), culture, arg0, arg1);

    public static string UNKNOWN_TABLE_NAME(object arg0) => AnalyticsResources.Format(nameof (UNKNOWN_TABLE_NAME), arg0);

    public static string UNKNOWN_TABLE_NAME(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (UNKNOWN_TABLE_NAME), culture, arg0);

    public static string UNSUPPORTED_PROPERTY(object arg0) => AnalyticsResources.Format(nameof (UNSUPPORTED_PROPERTY), arg0);

    public static string UNSUPPORTED_PROPERTY(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (UNSUPPORTED_PROPERTY), culture, arg0);

    public static string UNSUPPORTED_SQL_DB_TYPE(object arg0) => AnalyticsResources.Format(nameof (UNSUPPORTED_SQL_DB_TYPE), arg0);

    public static string UNSUPPORTED_SQL_DB_TYPE(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (UNSUPPORTED_SQL_DB_TYPE), culture, arg0);

    public static string UNSUPPORTED_TYPE_FOR_CUSTOM_JSON_SERIALIZER(object arg0) => AnalyticsResources.Format(nameof (UNSUPPORTED_TYPE_FOR_CUSTOM_JSON_SERIALIZER), arg0);

    public static string UNSUPPORTED_TYPE_FOR_CUSTOM_JSON_SERIALIZER(
      object arg0,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (UNSUPPORTED_TYPE_FOR_CUSTOM_JSON_SERIALIZER), culture, arg0);
    }

    public static string V7_DOES_NOT_SUPPORT_BIGINT() => AnalyticsResources.Get(nameof (V7_DOES_NOT_SUPPORT_BIGINT));

    public static string V7_DOES_NOT_SUPPORT_BIGINT(CultureInfo culture) => AnalyticsResources.Get(nameof (V7_DOES_NOT_SUPPORT_BIGINT), culture);

    public static string WATERMARK_MISMATCH(object arg0, object arg1, object arg2) => AnalyticsResources.Format(nameof (WATERMARK_MISMATCH), arg0, arg1, arg2);

    public static string WATERMARK_MISMATCH(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (WATERMARK_MISMATCH), culture, arg0, arg1, arg2);
    }

    public static string ENTITY_FIELD_NAME_REVISED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVISED_DATE));

    public static string ENTITY_FIELD_NAME_REVISED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVISED_DATE), culture);

    public static string ENTITY_FIELD_NAME_IS_CURRENT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_CURRENT));

    public static string ENTITY_FIELD_NAME_IS_CURRENT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_CURRENT), culture);

    public static string ENTITY_FIELD_NAME_IS_LAST_REVISION_OF_DAY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_LAST_REVISION_OF_DAY));

    public static string ENTITY_FIELD_NAME_IS_LAST_REVISION_OF_DAY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_LAST_REVISION_OF_DAY), culture);

    public static string ENTITY_FIELD_NAME_PARENT_WORK_ITEM_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PARENT_WORK_ITEM_ID));

    public static string ENTITY_FIELD_NAME_PARENT_WORK_ITEM_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PARENT_WORK_ITEM_ID), culture);

    public static string ENTITY_FIELD_NAME_TAGS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TAGS));

    public static string ENTITY_FIELD_NAME_TAGS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TAGS), culture);

    public static string ENTITY_FIELD_NAME_STATE_CATEGORY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STATE_CATEGORY));

    public static string ENTITY_FIELD_NAME_STATE_CATEGORY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STATE_CATEGORY), culture);

    public static string ENTITY_FIELD_NAME_IN_PROGRESS_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IN_PROGRESS_DATE));

    public static string ENTITY_FIELD_NAME_IN_PROGRESS_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IN_PROGRESS_DATE), culture);

    public static string ENTITY_FIELD_NAME_COMPLETED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMPLETED_DATE));

    public static string ENTITY_FIELD_NAME_COMPLETED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMPLETED_DATE), culture);

    public static string ENTITY_FIELD_NAME_LEAD_TIME_DAYS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LEAD_TIME_DAYS));

    public static string ENTITY_FIELD_NAME_LEAD_TIME_DAYS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LEAD_TIME_DAYS), culture);

    public static string ENTITY_FIELD_NAME_CYCLE_TIME_DAYS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CYCLE_TIME_DAYS));

    public static string ENTITY_FIELD_NAME_CYCLE_TIME_DAYS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CYCLE_TIME_DAYS), culture);

    public static string ENTITY_FIELD_NAME_DATE_VALUE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DATE_VALUE));

    public static string ENTITY_FIELD_NAME_DATE_VALUE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DATE_VALUE), culture);

    public static string ENTITY_FIELD_NAME_COLUMN_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COLUMN_ID));

    public static string ENTITY_FIELD_NAME_COLUMN_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COLUMN_ID), culture);

    public static string ENTITY_FIELD_NAME_COLUMN_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COLUMN_NAME));

    public static string ENTITY_FIELD_NAME_COLUMN_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COLUMN_NAME), culture);

    public static string ENTITY_FIELD_NAME_COLUMN_ORDER() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COLUMN_ORDER));

    public static string ENTITY_FIELD_NAME_COLUMN_ORDER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COLUMN_ORDER), culture);

    public static string ENTITY_FIELD_NAME_COLUMN_ITEM_LIMIT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COLUMN_ITEM_LIMIT));

    public static string ENTITY_FIELD_NAME_COLUMN_ITEM_LIMIT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COLUMN_ITEM_LIMIT), culture);

    public static string ENTITY_FIELD_NAME_IS_DONE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_DONE));

    public static string ENTITY_FIELD_NAME_IS_DONE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_DONE), culture);

    public static string ENTITY_FIELD_NAME_BOARD_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BOARD_ID));

    public static string ENTITY_FIELD_NAME_BOARD_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BOARD_ID), culture);

    public static string ENTITY_FIELD_NAME_BOARD_CATEGORY_REFERENCE_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BOARD_CATEGORY_REFERENCE_NAME));

    public static string ENTITY_FIELD_NAME_BOARD_CATEGORY_REFERENCE_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BOARD_CATEGORY_REFERENCE_NAME), culture);

    public static string ENTITY_FIELD_NAME_BOARD_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BOARD_NAME));

    public static string ENTITY_FIELD_NAME_BOARD_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BOARD_NAME), culture);

    public static string ENTITY_FIELD_NAME_BOARD_LEVEL() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BOARD_LEVEL));

    public static string ENTITY_FIELD_NAME_BOARD_LEVEL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BOARD_LEVEL), culture);

    public static string ENTITY_FIELD_NAME_BACKLOG_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BACKLOG_TYPE));

    public static string ENTITY_FIELD_NAME_BACKLOG_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BACKLOG_TYPE), culture);

    public static string ENTITY_FIELD_NAME_IS_BOARD_VISIBLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_BOARD_VISIBLE));

    public static string ENTITY_FIELD_NAME_IS_BOARD_VISIBLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_BOARD_VISIBLE), culture);

    public static string ENTITY_FIELD_NAME_LANE_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LANE_ID));

    public static string ENTITY_FIELD_NAME_LANE_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LANE_ID), culture);

    public static string ENTITY_FIELD_NAME_LANE_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LANE_NAME));

    public static string ENTITY_FIELD_NAME_LANE_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LANE_NAME), culture);

    public static string ENTITY_FIELD_NAME_LANE_ORDER() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LANE_ORDER));

    public static string ENTITY_FIELD_NAME_LANE_ORDER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LANE_ORDER), culture);

    public static string ENTITY_FIELD_NAME_IS_COLUMN_SPLIT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_COLUMN_SPLIT));

    public static string ENTITY_FIELD_NAME_IS_COLUMN_SPLIT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_COLUMN_SPLIT), culture);

    public static string ENTITY_FIELD_NAME_IS_DEFAULT_LANE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_DEFAULT_LANE));

    public static string ENTITY_FIELD_NAME_IS_DEFAULT_LANE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_DEFAULT_LANE), culture);

    public static string ENTITY_FIELD_NAME_CHANGED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CHANGED_DATE));

    public static string ENTITY_FIELD_NAME_CHANGED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CHANGED_DATE), culture);

    public static string ENTITY_FIELD_NAME_DONE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DONE));

    public static string ENTITY_FIELD_NAME_DONE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DONE), culture);

    public static string ENTITY_FIELD_NAME_SOURCE_WORK_ITEM_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SOURCE_WORK_ITEM_ID));

    public static string ENTITY_FIELD_NAME_SOURCE_WORK_ITEM_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SOURCE_WORK_ITEM_ID), culture);

    public static string ENTITY_FIELD_NAME_TARGET_WORK_ITEM_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TARGET_WORK_ITEM_ID));

    public static string ENTITY_FIELD_NAME_TARGET_WORK_ITEM_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TARGET_WORK_ITEM_ID), culture);

    public static string ENTITY_FIELD_NAME_CREATED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CREATED_DATE));

    public static string ENTITY_FIELD_NAME_CREATED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CREATED_DATE), culture);

    public static string ENTITY_FIELD_NAME_DELETED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DELETED_DATE));

    public static string ENTITY_FIELD_NAME_DELETED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DELETED_DATE), culture);

    public static string ENTITY_FIELD_NAME_COMMENT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMENT));

    public static string ENTITY_FIELD_NAME_COMMENT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMENT), culture);

    public static string ENTITY_FIELD_NAME_LINK_TYPE_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINK_TYPE_ID));

    public static string ENTITY_FIELD_NAME_LINK_TYPE_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINK_TYPE_ID), culture);

    public static string ENTITY_FIELD_NAME_LINK_TYPE_REFERENCE_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINK_TYPE_REFERENCE_NAME));

    public static string ENTITY_FIELD_NAME_LINK_TYPE_REFERENCE_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINK_TYPE_REFERENCE_NAME), culture);

    public static string ENTITY_FIELD_NAME_LINK_TYPE_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINK_TYPE_NAME));

    public static string ENTITY_FIELD_NAME_LINK_TYPE_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINK_TYPE_NAME), culture);

    public static string ENTITY_FIELD_NAME_LINK_TYPE_IS_ACYCLIC() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINK_TYPE_IS_ACYCLIC));

    public static string ENTITY_FIELD_NAME_LINK_TYPE_IS_ACYCLIC(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINK_TYPE_IS_ACYCLIC), culture);

    public static string ENTITY_FIELD_NAME_LINK_TYPE_IS_DIRECTIONAL() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINK_TYPE_IS_DIRECTIONAL));

    public static string ENTITY_FIELD_NAME_LINK_TYPE_IS_DIRECTIONAL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINK_TYPE_IS_DIRECTIONAL), culture);

    public static string ENTITY_FIELD_NAME_DAY_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DAY_NAME));

    public static string ENTITY_FIELD_NAME_DAY_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DAY_NAME), culture);

    public static string ENTITY_FIELD_NAME_DAY_SHORT_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DAY_SHORT_NAME));

    public static string ENTITY_FIELD_NAME_DAY_SHORT_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DAY_SHORT_NAME), culture);

    public static string ENTITY_FIELD_NAME_DAY_OF_WEEK() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DAY_OF_WEEK));

    public static string ENTITY_FIELD_NAME_DAY_OF_WEEK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DAY_OF_WEEK), culture);

    public static string ENTITY_FIELD_NAME_DAY_OF_MONTH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DAY_OF_MONTH));

    public static string ENTITY_FIELD_NAME_DAY_OF_MONTH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DAY_OF_MONTH), culture);

    public static string ENTITY_FIELD_NAME_DAY_OF_YEAR() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DAY_OF_YEAR));

    public static string ENTITY_FIELD_NAME_DAY_OF_YEAR(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DAY_OF_YEAR), culture);

    public static string ENTITY_FIELD_NAME_WEEK_STARTING_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WEEK_STARTING_DATE));

    public static string ENTITY_FIELD_NAME_WEEK_STARTING_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WEEK_STARTING_DATE), culture);

    public static string ENTITY_FIELD_NAME_WEEK_ENDING_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WEEK_ENDING_DATE));

    public static string ENTITY_FIELD_NAME_WEEK_ENDING_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WEEK_ENDING_DATE), culture);

    public static string ENTITY_FIELD_NAME_MONTH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_MONTH));

    public static string ENTITY_FIELD_NAME_MONTH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_MONTH), culture);

    public static string ENTITY_FIELD_NAME_MONTH_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_MONTH_NAME));

    public static string ENTITY_FIELD_NAME_MONTH_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_MONTH_NAME), culture);

    public static string ENTITY_FIELD_NAME_MONTH_SHORT_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_MONTH_SHORT_NAME));

    public static string ENTITY_FIELD_NAME_MONTH_SHORT_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_MONTH_SHORT_NAME), culture);

    public static string ENTITY_FIELD_NAME_MONTH_OF_YEAR() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_MONTH_OF_YEAR));

    public static string ENTITY_FIELD_NAME_MONTH_OF_YEAR(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_MONTH_OF_YEAR), culture);

    public static string ENTITY_FIELD_NAME_YEAR_MONTH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_YEAR_MONTH));

    public static string ENTITY_FIELD_NAME_YEAR_MONTH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_YEAR_MONTH), culture);

    public static string ENTITY_FIELD_NAME_YEAR() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_YEAR));

    public static string ENTITY_FIELD_NAME_YEAR(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_YEAR), culture);

    public static string ENTITY_FIELD_NAME_PROJECT_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PROJECT_ID));

    public static string ENTITY_FIELD_NAME_PROJECT_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PROJECT_ID), culture);

    public static string ENTITY_FIELD_NAME_PROJECT_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PROJECT_NAME));

    public static string ENTITY_FIELD_NAME_PROJECT_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PROJECT_NAME), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_ID));

    public static string ENTITY_FIELD_NAME_ITERATION_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_ID), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_NAME));

    public static string ENTITY_FIELD_NAME_ITERATION_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_NAME), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_PATH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_PATH));

    public static string ENTITY_FIELD_NAME_ITERATION_PATH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_PATH), culture);

    public static string ENTITY_FIELD_NAME_START_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_START_DATE));

    public static string ENTITY_FIELD_NAME_START_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_START_DATE), culture);

    public static string ENTITY_FIELD_NAME_END_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_END_DATE));

    public static string ENTITY_FIELD_NAME_END_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_END_DATE), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_1() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_1));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_1(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_1), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_2() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_2));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_2(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_2), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_3() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_3));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_3(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_3), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_4() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_4));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_4(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_4), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_5() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_5));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_5(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_5), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_6() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_6));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_6(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_6), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_7() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_7));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_7(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_7), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_8() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_8));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_8(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_8), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_9() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_9));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_9(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_9), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_10() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_10));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_10(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_10), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_11() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_11));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_11(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_11), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_12() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_12));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_12(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_12), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_13() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_13));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_13(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_13), culture);

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_14() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_14));

    public static string ENTITY_FIELD_NAME_ITERATION_LEVEL_14(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION_LEVEL_14), culture);

    public static string ENTITY_FIELD_NAME_IS_ENDED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_ENDED));

    public static string ENTITY_FIELD_NAME_IS_ENDED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_ENDED), culture);

    public static string ENTITY_FIELD_NAME_AREA_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_ID));

    public static string ENTITY_FIELD_NAME_AREA_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_ID), culture);

    public static string ENTITY_FIELD_NAME_AREA_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_NAME));

    public static string ENTITY_FIELD_NAME_AREA_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_NAME), culture);

    public static string ENTITY_FIELD_NAME_AREA_PATH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_PATH));

    public static string ENTITY_FIELD_NAME_AREA_PATH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_PATH), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_1() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_1));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_1(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_1), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_2() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_2));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_2(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_2), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_3() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_3));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_3(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_3), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_4() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_4));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_4(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_4), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_5() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_5));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_5(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_5), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_6() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_6));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_6(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_6), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_7() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_7));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_7(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_7), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_8() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_8));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_8(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_8), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_9() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_9));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_9(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_9), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_10() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_10));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_10(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_10), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_11() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_11));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_11(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_11), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_12() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_12));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_12(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_12), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_13() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_13));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_13(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_13), culture);

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_14() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_14));

    public static string ENTITY_FIELD_NAME_AREA_LEVEL_14(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA_LEVEL_14), culture);

    public static string ENTITY_FIELD_NAME_DEPTH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPTH));

    public static string ENTITY_FIELD_NAME_DEPTH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPTH), culture);

    public static string ENTITY_FIELD_NAME_TAG_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TAG_ID));

    public static string ENTITY_FIELD_NAME_TAG_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TAG_ID), culture);

    public static string ENTITY_FIELD_NAME_TAG_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TAG_NAME));

    public static string ENTITY_FIELD_NAME_TAG_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TAG_NAME), culture);

    public static string ENTITY_FIELD_NAME_TEAM_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEAM_ID));

    public static string ENTITY_FIELD_NAME_TEAM_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEAM_ID), culture);

    public static string ENTITY_FIELD_NAME_USER_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_ID));

    public static string ENTITY_FIELD_NAME_USER_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_ID), culture);

    public static string ENTITY_FIELD_NAME_USER_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_NAME));

    public static string ENTITY_FIELD_NAME_USER_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_NAME), culture);

    public static string ENTITY_FIELD_NAME_USER_EMAIL() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_EMAIL));

    public static string ENTITY_FIELD_NAME_USER_EMAIL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_EMAIL), culture);

    public static string ENTITY_FIELD_NAME_USER_GITHUB_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_GITHUB_ID));

    public static string ENTITY_FIELD_NAME_USER_GITHUB_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_GITHUB_ID), culture);

    public static string ENTITY_FIELD_NAME_USER_USERTYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_USERTYPE));

    public static string ENTITY_FIELD_NAME_USER_USERTYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_USERTYPE), culture);

    public static string ENTITY_FIELD_NAME_FIELD_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FIELD_NAME));

    public static string ENTITY_FIELD_NAME_FIELD_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FIELD_NAME), culture);

    public static string ENTITY_FIELD_NAME_FIELD_REFERENCE_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FIELD_REFERENCE_NAME));

    public static string ENTITY_FIELD_NAME_FIELD_REFERENCE_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FIELD_REFERENCE_NAME), culture);

    public static string ENTITY_FIELD_NAME_FIELD_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FIELD_TYPE));

    public static string ENTITY_FIELD_NAME_FIELD_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FIELD_TYPE), culture);

    public static string ENTITY_FIELD_NAME_WORK_ITEM_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WORK_ITEM_TYPE));

    public static string ENTITY_FIELD_NAME_WORK_ITEM_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WORK_ITEM_TYPE), culture);

    public static string ENTITY_SET_NAME_AREAS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_AREAS));

    public static string ENTITY_SET_NAME_AREAS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_AREAS), culture);

    public static string ENTITY_SET_NAME_BOARD_LOCATIONS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_BOARD_LOCATIONS));

    public static string ENTITY_SET_NAME_BOARD_LOCATIONS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_BOARD_LOCATIONS), culture);

    public static string ENTITY_SET_NAME_BRANCHES() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_BRANCHES));

    public static string ENTITY_SET_NAME_BRANCHES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_BRANCHES), culture);

    public static string ENTITY_SET_NAME_BUILDS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_BUILDS));

    public static string ENTITY_SET_NAME_BUILDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_BUILDS), culture);

    public static string ENTITY_SET_NAME_PIPELINE_RUNS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PIPELINE_RUNS));

    public static string ENTITY_SET_NAME_PIPELINE_RUNS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PIPELINE_RUNS), culture);

    public static string ENTITY_SET_NAME_GITHUBPULLREQUEST() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUBPULLREQUEST));

    public static string ENTITY_SET_NAME_GITHUBPULLREQUEST(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUBPULLREQUEST), culture);

    public static string ENTITY_SET_NAME_GITHUBPULLREQUESTSNAPSHOT() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUBPULLREQUESTSNAPSHOT));

    public static string ENTITY_SET_NAME_GITHUBPULLREQUESTSNAPSHOT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUBPULLREQUESTSNAPSHOT), culture);

    public static string ENTITY_SET_NAME_GITHUBBRANCH() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUBBRANCH));

    public static string ENTITY_SET_NAME_GITHUBBRANCH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUBBRANCH), culture);

    public static string ENTITY_SET_NAME_DATES() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_DATES));

    public static string ENTITY_SET_NAME_DATES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_DATES), culture);

    public static string ENTITY_SET_NAME_ITERATIONS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_ITERATIONS));

    public static string ENTITY_SET_NAME_ITERATIONS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_ITERATIONS), culture);

    public static string ENTITY_SET_NAME_PROCESSES() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PROCESSES));

    public static string ENTITY_SET_NAME_PROCESSES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PROCESSES), culture);

    public static string ENTITY_SET_NAME_PROJECTS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PROJECTS));

    public static string ENTITY_SET_NAME_PROJECTS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PROJECTS), culture);

    public static string ENTITY_SET_NAME_RELEASES() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_RELEASES));

    public static string ENTITY_SET_NAME_RELEASES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_RELEASES), culture);

    public static string ENTITY_SET_NAME_RELEASE_ENVIRONMENTS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_RELEASE_ENVIRONMENTS));

    public static string ENTITY_SET_NAME_RELEASE_ENVIRONMENTS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_RELEASE_ENVIRONMENTS), culture);

    public static string ENTITY_SET_NAME_TAGS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TAGS));

    public static string ENTITY_SET_NAME_TAGS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TAGS), culture);

    public static string ENTITY_SET_NAME_TEAMS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEAMS));

    public static string ENTITY_SET_NAME_TEAMS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEAMS), culture);

    public static string ENTITY_SET_NAME_TESTS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TESTS));

    public static string ENTITY_SET_NAME_TESTS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TESTS), culture);

    public static string ENTITY_SET_NAME_TEST_RESULTS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_RESULTS));

    public static string ENTITY_SET_NAME_TEST_RESULTS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_RESULTS), culture);

    public static string ENTITY_SET_NAME_TEST_RUNS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_RUNS));

    public static string ENTITY_SET_NAME_TEST_RUNS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_RUNS), culture);

    public static string ENTITY_SET_NAME_USERS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_USERS));

    public static string ENTITY_SET_NAME_USERS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_USERS), culture);

    public static string ENTITY_SET_NAME_WORK_ITEMS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_WORK_ITEMS));

    public static string ENTITY_SET_NAME_WORK_ITEMS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_WORK_ITEMS), culture);

    public static string ENTITY_SET_NAME_WORK_ITEMS_LAST_30_DAYS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_WORK_ITEMS_LAST_30_DAYS));

    public static string ENTITY_SET_NAME_WORK_ITEMS_LAST_30_DAYS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_WORK_ITEMS_LAST_30_DAYS), culture);

    public static string ENTITY_SET_NAME_WORK_ITEM_LINKS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_WORK_ITEM_LINKS));

    public static string ENTITY_SET_NAME_WORK_ITEM_LINKS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_WORK_ITEM_LINKS), culture);

    public static string ENTITY_SET_NAME_WORK_ITEM_REVISIONS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_WORK_ITEM_REVISIONS));

    public static string ENTITY_SET_NAME_WORK_ITEM_REVISIONS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_WORK_ITEM_REVISIONS), culture);

    public static string ENTITY_SET_NAME_WORK_ITEMS_TODAY() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_WORK_ITEMS_TODAY));

    public static string ENTITY_SET_NAME_WORK_ITEMS_TODAY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_WORK_ITEMS_TODAY), culture);

    public static string ENTITY_FIELD_NAME_PROJECT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PROJECT));

    public static string ENTITY_FIELD_NAME_PROJECT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PROJECT), culture);

    public static string ENTITY_FIELD_NAME_AREA() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA));

    public static string ENTITY_FIELD_NAME_AREA(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AREA), culture);

    public static string ENTITY_FIELD_NAME_ITERATION() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION));

    public static string ENTITY_FIELD_NAME_ITERATION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ITERATION), culture);

    public static string ENTITY_FIELD_NAME_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DATE));

    public static string ENTITY_FIELD_NAME_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DATE), culture);

    public static string ENTITY_FIELD_NAME_TEAM_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEAM_NAME));

    public static string ENTITY_FIELD_NAME_TEAM_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEAM_NAME), culture);

    public static string ENTITY_FIELD_NAME_ACTIVATED_BY_USER_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVATED_BY_USER_NAME));

    public static string ENTITY_FIELD_NAME_ACTIVATED_BY_USER_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVATED_BY_USER_NAME), culture);

    public static string ENTITY_FIELD_NAME_ASSIGNED_TO_USER_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ASSIGNED_TO_USER_NAME));

    public static string ENTITY_FIELD_NAME_ASSIGNED_TO_USER_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ASSIGNED_TO_USER_NAME), culture);

    public static string ENTITY_FIELD_NAME_BACKLOG_CATEGORY_REFERENCE_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BACKLOG_CATEGORY_REFERENCE_NAME));

    public static string ENTITY_FIELD_NAME_BACKLOG_CATEGORY_REFERENCE_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BACKLOG_CATEGORY_REFERENCE_NAME), culture);

    public static string ENTITY_FIELD_NAME_BACKLOG_LEVEL() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BACKLOG_LEVEL));

    public static string ENTITY_FIELD_NAME_BACKLOG_LEVEL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BACKLOG_LEVEL), culture);

    public static string ENTITY_FIELD_NAME_BACKLOG_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BACKLOG_NAME));

    public static string ENTITY_FIELD_NAME_BACKLOG_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BACKLOG_NAME), culture);

    public static string ENTITY_FIELD_NAME_CHANGED_BY_USER_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CHANGED_BY_USER_NAME));

    public static string ENTITY_FIELD_NAME_CHANGED_BY_USER_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CHANGED_BY_USER_NAME), culture);

    public static string ENTITY_FIELD_NAME_CLOSED_BY_USER_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CLOSED_BY_USER_NAME));

    public static string ENTITY_FIELD_NAME_CLOSED_BY_USER_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CLOSED_BY_USER_NAME), culture);

    public static string ENTITY_FIELD_NAME_CREATED_BY_USER_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CREATED_BY_USER_NAME));

    public static string ENTITY_FIELD_NAME_CREATED_BY_USER_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CREATED_BY_USER_NAME), culture);

    public static string ENTITY_FIELD_NAME_HAS_BACKLOG() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_HAS_BACKLOG));

    public static string ENTITY_FIELD_NAME_HAS_BACKLOG(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_HAS_BACKLOG), culture);

    public static string ENTITY_FIELD_NAME_IS_BUG_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_BUG_TYPE));

    public static string ENTITY_FIELD_NAME_IS_BUG_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_BUG_TYPE), culture);

    public static string ENTITY_FIELD_NAME_IS_HIDDEN_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_HIDDEN_TYPE));

    public static string ENTITY_FIELD_NAME_IS_HIDDEN_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_HIDDEN_TYPE), culture);

    public static string ENTITY_FIELD_NAME_RESOLVED_BY_USER_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESOLVED_BY_USER_NAME));

    public static string ENTITY_FIELD_NAME_RESOLVED_BY_USER_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESOLVED_BY_USER_NAME), culture);

    public static string ENTITY_FIELD_NAME_WORK_ITEM_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WORK_ITEM_COUNT));

    public static string ENTITY_FIELD_NAME_WORK_ITEM_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WORK_ITEM_COUNT), culture);

    public static string ENTITY_FIELD_NAME_WORK_ITEM_TYPE_CATEGORY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WORK_ITEM_TYPE_CATEGORY));

    public static string ENTITY_FIELD_NAME_WORK_ITEM_TYPE_CATEGORY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WORK_ITEM_TYPE_CATEGORY), culture);

    public static string ENTITY_FIELD_NAME_TEST_RUN_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_RUN_ID));

    public static string ENTITY_FIELD_NAME_TEST_RUN_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_RUN_ID), culture);

    public static string ENTITY_FIELD_NAME_IS_AUTOMATED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_AUTOMATED));

    public static string ENTITY_FIELD_NAME_IS_AUTOMATED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_AUTOMATED), culture);

    public static string ENTITY_FIELD_NAME_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TITLE));

    public static string ENTITY_FIELD_NAME_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_RUN_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_RUN_TYPE));

    public static string ENTITY_FIELD_NAME_TEST_RUN_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_RUN_TYPE), culture);

    public static string ENTITY_FIELD_NAME_WORKFLOW() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WORKFLOW));

    public static string ENTITY_FIELD_NAME_WORKFLOW(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WORKFLOW), culture);

    public static string ENTITY_FIELD_NAME_STARTED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STARTED_DATE));

    public static string ENTITY_FIELD_NAME_STARTED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STARTED_DATE), culture);

    public static string ENTITY_FIELD_NAME_RUN_DURATION_SECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RUN_DURATION_SECONDS));

    public static string ENTITY_FIELD_NAME_RUN_DURATION_SECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RUN_DURATION_SECONDS), culture);

    public static string ENTITY_FIELD_NAME_DURATION_SECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DURATION_SECONDS));

    public static string ENTITY_FIELD_NAME_DURATION_SECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DURATION_SECONDS), culture);

    public static string ENTITY_FIELD_NAME_RESULT_DURATION_SECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_DURATION_SECONDS));

    public static string ENTITY_FIELD_NAME_RESULT_DURATION_SECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_DURATION_SECONDS), culture);

    public static string ENTITY_FIELD_NAME_RESULT_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_COUNT), culture);

    public static string ENTITY_FIELD_NAME_RESULT_PASS_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_PASS_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_PASS_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_PASS_COUNT), culture);

    public static string ENTITY_FIELD_NAME_RESULT_FAIL_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_FAIL_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_FAIL_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_FAIL_COUNT), culture);

    public static string ENTITY_FIELD_NAME_RESULT_NONE_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_NONE_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_NONE_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_NONE_COUNT), culture);

    public static string ENTITY_FIELD_NAME_RESULT_INCONCLUSIVE_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_INCONCLUSIVE_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_INCONCLUSIVE_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_INCONCLUSIVE_COUNT), culture);

    public static string ENTITY_FIELD_NAME_RESULT_TIMEOUT_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_TIMEOUT_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_TIMEOUT_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_TIMEOUT_COUNT), culture);

    public static string ENTITY_FIELD_NAME_RESULT_ABORTED_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_ABORTED_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_ABORTED_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_ABORTED_COUNT), culture);

    public static string ENTITY_FIELD_NAME_RESULT_BLOCKED_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_BLOCKED_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_BLOCKED_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_BLOCKED_COUNT), culture);

    public static string ENTITY_FIELD_NAME_RESULT_NOT_EXECUTED_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_NOT_EXECUTED_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_NOT_EXECUTED_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_NOT_EXECUTED_COUNT), culture);

    public static string ENTITY_FIELD_NAME_RESULT_WARNING_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_WARNING_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_WARNING_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_WARNING_COUNT), culture);

    public static string ENTITY_FIELD_NAME_RESULT_ERROR_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_ERROR_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_ERROR_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_ERROR_COUNT), culture);

    public static string ENTITY_FIELD_NAME_RESULT_NOT_APPLICABLE_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_NOT_APPLICABLE_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_NOT_APPLICABLE_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_NOT_APPLICABLE_COUNT), culture);

    public static string ENTITY_FIELD_NAME_RESULT_NOT_IMPACTED_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_NOT_IMPACTED_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_NOT_IMPACTED_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_NOT_IMPACTED_COUNT), culture);

    public static string ENTITY_FIELD_NAME_HAS_DETAIL() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_HAS_DETAIL));

    public static string ENTITY_FIELD_NAME_HAS_DETAIL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_HAS_DETAIL), culture);

    public static string ENTITY_FIELD_NAME_TEST_CASE_REFERENCE_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_CASE_REFERENCE_ID));

    public static string ENTITY_FIELD_NAME_TEST_CASE_REFERENCE_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_CASE_REFERENCE_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_NAME));

    public static string ENTITY_FIELD_NAME_TEST_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_NAME), culture);

    public static string ENTITY_FIELD_NAME_TEST_OUTCOME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_OUTCOME));

    public static string ENTITY_FIELD_NAME_TEST_OUTCOME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_OUTCOME), culture);

    public static string ENTITY_FIELD_NAME_TEST_RESULT_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_RESULT_ID));

    public static string ENTITY_FIELD_NAME_TEST_RESULT_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_RESULT_ID), culture);

    public static string ENTITY_FIELD_NAME_CONTAINER_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTAINER_NAME));

    public static string ENTITY_FIELD_NAME_CONTAINER_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTAINER_NAME), culture);

    public static string ENTITY_FIELD_NAME_FULLY_QUALIFIED_TEST_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FULLY_QUALIFIED_TEST_NAME));

    public static string ENTITY_FIELD_NAME_FULLY_QUALIFIED_TEST_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FULLY_QUALIFIED_TEST_NAME), culture);

    public static string ENTITY_FIELD_NAME_PRIORITY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PRIORITY));

    public static string ENTITY_FIELD_NAME_PRIORITY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PRIORITY), culture);

    public static string ENTITY_FIELD_NAME_TEST_OWNER() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_OWNER));

    public static string ENTITY_FIELD_NAME_TEST_OWNER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_OWNER), culture);

    public static string ENTITY_FIELD_NAME_BRANCH_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BRANCH_NAME));

    public static string ENTITY_FIELD_NAME_BRANCH_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BRANCH_NAME), culture);

    public static string ENTITY_FIELD_NAME_BUILD_DEFINITION_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_DEFINITION_ID));

    public static string ENTITY_FIELD_NAME_BUILD_DEFINITION_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_DEFINITION_ID), culture);

    public static string ENTITY_FIELD_NAME_BUILD_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_ID));

    public static string ENTITY_FIELD_NAME_BUILD_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_ID), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_RUN_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_RUN_ID));

    public static string ENTITY_FIELD_NAME_PIPELINE_RUN_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_RUN_ID), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_DEFINITION_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_DEFINITION_ID));

    public static string ENTITY_FIELD_NAME_RELEASE_DEFINITION_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_DEFINITION_ID), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_ENVIRONMENT_DEFINITION_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_ENVIRONMENT_DEFINITION_ID));

    public static string ENTITY_FIELD_NAME_RELEASE_ENVIRONMENT_DEFINITION_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_ENVIRONMENT_DEFINITION_ID), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_ENVIRONMENT_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_ENVIRONMENT_ID));

    public static string ENTITY_FIELD_NAME_RELEASE_ENVIRONMENT_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_ENVIRONMENT_ID), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_ID));

    public static string ENTITY_FIELD_NAME_RELEASE_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_ID), culture);

    public static string ENTITY_FIELD_NAME_REPOSITORY_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY_ID));

    public static string ENTITY_FIELD_NAME_REPOSITORY_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY_ID), culture);

    public static string ENTITY_FIELD_NAME_REPOSITORY_URL() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY_URL));

    public static string ENTITY_FIELD_NAME_REPOSITORY_URL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY_URL), culture);

    public static string ENTITY_FIELD_NAME_REPOSITORY_VSTS_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY_VSTS_ID));

    public static string ENTITY_FIELD_NAME_REPOSITORY_VSTS_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY_VSTS_ID), culture);

    public static string ENTITY_FIELD_NAME_WORKITEM_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WORKITEM_ID));

    public static string ENTITY_FIELD_NAME_WORKITEM_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WORKITEM_ID), culture);

    public static string ENTITY_FIELD_NAME_ACTIVATED_BY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVATED_BY));

    public static string ENTITY_FIELD_NAME_ACTIVATED_BY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVATED_BY), culture);

    public static string ENTITY_FIELD_NAME_ACTIVATED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVATED_DATE));

    public static string ENTITY_FIELD_NAME_ACTIVATED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVATED_DATE), culture);

    public static string ENTITY_FIELD_NAME_ACTIVITY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY));

    public static string ENTITY_FIELD_NAME_ACTIVITY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY), culture);

    public static string ENTITY_FIELD_NAME_ASSIGNED_TO() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ASSIGNED_TO));

    public static string ENTITY_FIELD_NAME_ASSIGNED_TO(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ASSIGNED_TO), culture);

    public static string ENTITY_FIELD_NAME_AUTOMATED_TEST_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AUTOMATED_TEST_ID));

    public static string ENTITY_FIELD_NAME_AUTOMATED_TEST_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AUTOMATED_TEST_ID), culture);

    public static string ENTITY_FIELD_NAME_AUTOMATED_TEST_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AUTOMATED_TEST_NAME));

    public static string ENTITY_FIELD_NAME_AUTOMATED_TEST_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AUTOMATED_TEST_NAME), culture);

    public static string ENTITY_FIELD_NAME_AUTOMATED_TEST_STORAGE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AUTOMATED_TEST_STORAGE));

    public static string ENTITY_FIELD_NAME_AUTOMATED_TEST_STORAGE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AUTOMATED_TEST_STORAGE), culture);

    public static string ENTITY_FIELD_NAME_AUTOMATED_TEST_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AUTOMATED_TEST_TYPE));

    public static string ENTITY_FIELD_NAME_AUTOMATED_TEST_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AUTOMATED_TEST_TYPE), culture);

    public static string ENTITY_FIELD_NAME_AUTOMATION_STATUS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AUTOMATION_STATUS));

    public static string ENTITY_FIELD_NAME_AUTOMATION_STATUS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AUTOMATION_STATUS), culture);

    public static string ENTITY_FIELD_NAME_BACKLOG_PRIORITY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BACKLOG_PRIORITY));

    public static string ENTITY_FIELD_NAME_BACKLOG_PRIORITY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BACKLOG_PRIORITY), culture);

    public static string ENTITY_FIELD_NAME_BLOCKED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BLOCKED));

    public static string ENTITY_FIELD_NAME_BLOCKED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BLOCKED), culture);

    public static string ENTITY_FIELD_NAME_BUSINESS_VALUE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUSINESS_VALUE));

    public static string ENTITY_FIELD_NAME_BUSINESS_VALUE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUSINESS_VALUE), culture);

    public static string ENTITY_FIELD_NAME_CHANGED_BY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CHANGED_BY));

    public static string ENTITY_FIELD_NAME_CHANGED_BY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CHANGED_BY), culture);

    public static string ENTITY_FIELD_NAME_CLOSED_BY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CLOSED_BY));

    public static string ENTITY_FIELD_NAME_CLOSED_BY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CLOSED_BY), culture);

    public static string ENTITY_FIELD_NAME_CLOSED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CLOSED_DATE));

    public static string ENTITY_FIELD_NAME_CLOSED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CLOSED_DATE), culture);

    public static string ENTITY_FIELD_NAME_COMMITTED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITTED));

    public static string ENTITY_FIELD_NAME_COMMITTED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITTED), culture);

    public static string ENTITY_FIELD_NAME_COMPLETED_WORK() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMPLETED_WORK));

    public static string ENTITY_FIELD_NAME_COMPLETED_WORK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMPLETED_WORK), culture);

    public static string ENTITY_FIELD_NAME_CREATED_BY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CREATED_BY));

    public static string ENTITY_FIELD_NAME_CREATED_BY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CREATED_BY), culture);

    public static string ENTITY_FIELD_NAME_DISCIPLINE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DISCIPLINE));

    public static string ENTITY_FIELD_NAME_DISCIPLINE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DISCIPLINE), culture);

    public static string ENTITY_FIELD_NAME_DUE_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DUE_DATE));

    public static string ENTITY_FIELD_NAME_DUE_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DUE_DATE), culture);

    public static string ENTITY_FIELD_NAME_EFFORT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_EFFORT));

    public static string ENTITY_FIELD_NAME_EFFORT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_EFFORT), culture);

    public static string ENTITY_FIELD_NAME_ESCALATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ESCALATE));

    public static string ENTITY_FIELD_NAME_ESCALATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ESCALATE), culture);

    public static string ENTITY_FIELD_NAME_FINISH_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FINISH_DATE));

    public static string ENTITY_FIELD_NAME_FINISH_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FINISH_DATE), culture);

    public static string ENTITY_FIELD_NAME_FOUND_IN() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FOUND_IN));

    public static string ENTITY_FIELD_NAME_FOUND_IN(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FOUND_IN), culture);

    public static string ENTITY_FIELD_NAME_FOUND_IN_ENVIRONMENT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FOUND_IN_ENVIRONMENT));

    public static string ENTITY_FIELD_NAME_FOUND_IN_ENVIRONMENT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FOUND_IN_ENVIRONMENT), culture);

    public static string ENTITY_FIELD_NAME_HOW_FOUND() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_HOW_FOUND));

    public static string ENTITY_FIELD_NAME_HOW_FOUND(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_HOW_FOUND), culture);

    public static string ENTITY_FIELD_NAME_INTEGRATION_BUILD() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_INTEGRATION_BUILD));

    public static string ENTITY_FIELD_NAME_INTEGRATION_BUILD(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_INTEGRATION_BUILD), culture);

    public static string ENTITY_FIELD_NAME_ISSUE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ISSUE));

    public static string ENTITY_FIELD_NAME_ISSUE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ISSUE), culture);

    public static string ENTITY_FIELD_NAME_IS_DELETED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_DELETED));

    public static string ENTITY_FIELD_NAME_IS_DELETED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_DELETED), culture);

    public static string ENTITY_FIELD_NAME_ORIGINAL_ESTIMATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ORIGINAL_ESTIMATE));

    public static string ENTITY_FIELD_NAME_ORIGINAL_ESTIMATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ORIGINAL_ESTIMATE), culture);

    public static string ENTITY_FIELD_NAME_PROBABILITY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PROBABILITY));

    public static string ENTITY_FIELD_NAME_PROBABILITY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PROBABILITY), culture);

    public static string ENTITY_FIELD_NAME_RATING() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RATING));

    public static string ENTITY_FIELD_NAME_RATING(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RATING), culture);

    public static string ENTITY_FIELD_NAME_REASON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REASON));

    public static string ENTITY_FIELD_NAME_REASON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REASON), culture);

    public static string ENTITY_FIELD_NAME_REMAINING_WORK() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REMAINING_WORK));

    public static string ENTITY_FIELD_NAME_REMAINING_WORK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REMAINING_WORK), culture);

    public static string ENTITY_FIELD_NAME_REQUIREMENT_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUIREMENT_TYPE));

    public static string ENTITY_FIELD_NAME_REQUIREMENT_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUIREMENT_TYPE), culture);

    public static string ENTITY_FIELD_NAME_REQUIRES_REVIEW() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUIRES_REVIEW));

    public static string ENTITY_FIELD_NAME_REQUIRES_REVIEW(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUIRES_REVIEW), culture);

    public static string ENTITY_FIELD_NAME_REQUIRES_TEST() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUIRES_TEST));

    public static string ENTITY_FIELD_NAME_REQUIRES_TEST(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUIRES_TEST), culture);

    public static string ENTITY_FIELD_NAME_RESOLVED_BY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESOLVED_BY));

    public static string ENTITY_FIELD_NAME_RESOLVED_BY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESOLVED_BY), culture);

    public static string ENTITY_FIELD_NAME_RESOLVED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESOLVED_DATE));

    public static string ENTITY_FIELD_NAME_RESOLVED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESOLVED_DATE), culture);

    public static string ENTITY_FIELD_NAME_RESOLVED_REASON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESOLVED_REASON));

    public static string ENTITY_FIELD_NAME_RESOLVED_REASON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESOLVED_REASON), culture);

    public static string ENTITY_FIELD_NAME_REVISION() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVISION));

    public static string ENTITY_FIELD_NAME_REVISION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVISION), culture);

    public static string ENTITY_FIELD_NAME_RISK() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RISK));

    public static string ENTITY_FIELD_NAME_RISK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RISK), culture);

    public static string ENTITY_FIELD_NAME_ROOT_CAUSE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ROOT_CAUSE));

    public static string ENTITY_FIELD_NAME_ROOT_CAUSE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ROOT_CAUSE), culture);

    public static string ENTITY_FIELD_NAME_SEVERITY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SEVERITY));

    public static string ENTITY_FIELD_NAME_SEVERITY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SEVERITY), culture);

    public static string ENTITY_FIELD_NAME_SIZE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SIZE));

    public static string ENTITY_FIELD_NAME_SIZE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SIZE), culture);

    public static string ENTITY_FIELD_NAME_STACK_RANK() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STACK_RANK));

    public static string ENTITY_FIELD_NAME_STACK_RANK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STACK_RANK), culture);

    public static string ENTITY_FIELD_NAME_STATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STATE));

    public static string ENTITY_FIELD_NAME_STATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STATE), culture);

    public static string ENTITY_FIELD_NAME_STATE_CHANGE_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STATE_CHANGE_DATE));

    public static string ENTITY_FIELD_NAME_STATE_CHANGE_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STATE_CHANGE_DATE), culture);

    public static string ENTITY_FIELD_NAME_STORY_POINTS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STORY_POINTS));

    public static string ENTITY_FIELD_NAME_STORY_POINTS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STORY_POINTS), culture);

    public static string ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_1() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_1));

    public static string ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_1(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_1), culture);

    public static string ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_2() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_2));

    public static string ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_2(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_2), culture);

    public static string ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_3() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_3));

    public static string ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_3(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SUBJECT_MATTER_EXPERT_3), culture);

    public static string ENTITY_FIELD_NAME_TARGET_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TARGET_DATE));

    public static string ENTITY_FIELD_NAME_TARGET_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TARGET_DATE), culture);

    public static string ENTITY_FIELD_NAME_TARGET_RESOLVE_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TARGET_RESOLVE_DATE));

    public static string ENTITY_FIELD_NAME_TARGET_RESOLVE_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TARGET_RESOLVE_DATE), culture);

    public static string ENTITY_FIELD_NAME_TASK_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_TYPE));

    public static string ENTITY_FIELD_NAME_TASK_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_TYPE), culture);

    public static string ENTITY_FIELD_NAME_TIME_CRITICALITY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TIME_CRITICALITY));

    public static string ENTITY_FIELD_NAME_TIME_CRITICALITY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TIME_CRITICALITY), culture);

    public static string ENTITY_FIELD_NAME_TRIAGE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TRIAGE));

    public static string ENTITY_FIELD_NAME_TRIAGE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TRIAGE), culture);

    public static string ENTITY_FIELD_NAME_USER_ACCEPTANCE_TEST() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_ACCEPTANCE_TEST));

    public static string ENTITY_FIELD_NAME_USER_ACCEPTANCE_TEST(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_ACCEPTANCE_TEST), culture);

    public static string ENTITY_FIELD_NAME_VALUE_AREA() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_VALUE_AREA));

    public static string ENTITY_FIELD_NAME_VALUE_AREA(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_VALUE_AREA), culture);

    public static string ENTITY_FIELD_NAME_WATERMARK() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WATERMARK));

    public static string ENTITY_FIELD_NAME_WATERMARK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_WATERMARK), culture);

    public static string ENUM_TYPE_PERIOD_NONE() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_NONE));

    public static string ENUM_TYPE_PERIOD_NONE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_NONE), culture);

    public static string ENUM_TYPE_PERIOD_DAY() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_DAY));

    public static string ENUM_TYPE_PERIOD_DAY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_DAY), culture);

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_SUNDAY() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_SUNDAY));

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_SUNDAY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_SUNDAY), culture);

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_MONDAY() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_MONDAY));

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_MONDAY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_MONDAY), culture);

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_TUESDAY() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_TUESDAY));

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_TUESDAY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_TUESDAY), culture);

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_WEDNESDAY() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_WEDNESDAY));

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_WEDNESDAY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_WEDNESDAY), culture);

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_THURSDAY() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_THURSDAY));

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_THURSDAY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_THURSDAY), culture);

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_FRIDAY() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_FRIDAY));

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_FRIDAY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_FRIDAY), culture);

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_SATURDAY() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_SATURDAY));

    public static string ENUM_TYPE_PERIOD_WEEK_ENDING_ON_SATURDAY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_WEEK_ENDING_ON_SATURDAY), culture);

    public static string ENUM_TYPE_PERIOD_MONTH() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_MONTH));

    public static string ENUM_TYPE_PERIOD_MONTH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_MONTH), culture);

    public static string ENUM_TYPE_PERIOD_QUARTER() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_QUARTER));

    public static string ENUM_TYPE_PERIOD_QUARTER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_QUARTER), culture);

    public static string ENUM_TYPE_PERIOD_YEAR() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_YEAR));

    public static string ENUM_TYPE_PERIOD_YEAR(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_YEAR), culture);

    public static string ENUM_TYPE_PERIOD_ALL() => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_ALL));

    public static string ENUM_TYPE_PERIOD_ALL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PERIOD_ALL), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_UNSPECIFIED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_UNSPECIFIED));

    public static string ENUM_TYPE_TEST_OUTCOME_UNSPECIFIED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_UNSPECIFIED), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_NONE() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_NONE));

    public static string ENUM_TYPE_TEST_OUTCOME_NONE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_NONE), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_PASSED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_PASSED));

    public static string ENUM_TYPE_TEST_OUTCOME_PASSED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_PASSED), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_FAILED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_FAILED));

    public static string ENUM_TYPE_TEST_OUTCOME_FAILED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_FAILED), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_INCONCLUSIVE() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_INCONCLUSIVE));

    public static string ENUM_TYPE_TEST_OUTCOME_INCONCLUSIVE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_INCONCLUSIVE), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_TIMEOUT() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_TIMEOUT));

    public static string ENUM_TYPE_TEST_OUTCOME_TIMEOUT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_TIMEOUT), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_ABORTED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_ABORTED));

    public static string ENUM_TYPE_TEST_OUTCOME_ABORTED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_ABORTED), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_BLOCKED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_BLOCKED));

    public static string ENUM_TYPE_TEST_OUTCOME_BLOCKED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_BLOCKED), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_NOT_EXECUTED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_NOT_EXECUTED));

    public static string ENUM_TYPE_TEST_OUTCOME_NOT_EXECUTED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_NOT_EXECUTED), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_WARNING() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_WARNING));

    public static string ENUM_TYPE_TEST_OUTCOME_WARNING(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_WARNING), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_ERROR() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_ERROR));

    public static string ENUM_TYPE_TEST_OUTCOME_ERROR(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_ERROR), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_NOT_APPLICABLE() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_NOT_APPLICABLE));

    public static string ENUM_TYPE_TEST_OUTCOME_NOT_APPLICABLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_NOT_APPLICABLE), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_PAUSED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_PAUSED));

    public static string ENUM_TYPE_TEST_OUTCOME_PAUSED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_PAUSED), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_IN_PROGRESS() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_IN_PROGRESS));

    public static string ENUM_TYPE_TEST_OUTCOME_IN_PROGRESS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_IN_PROGRESS), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_NOT_IMPACTED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_NOT_IMPACTED));

    public static string ENUM_TYPE_TEST_OUTCOME_NOT_IMPACTED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_NOT_IMPACTED), culture);

    public static string ENUM_TYPE_TEST_OUTCOME_MAX_VALUE() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_MAX_VALUE));

    public static string ENUM_TYPE_TEST_OUTCOME_MAX_VALUE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_OUTCOME_MAX_VALUE), culture);

    public static string ENUM_TYPE_BOARD_COLUMN_SPLIT_DOING() => AnalyticsResources.Get(nameof (ENUM_TYPE_BOARD_COLUMN_SPLIT_DOING));

    public static string ENUM_TYPE_BOARD_COLUMN_SPLIT_DOING(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BOARD_COLUMN_SPLIT_DOING), culture);

    public static string ENUM_TYPE_BOARD_COLUMN_SPLIT_DONE() => AnalyticsResources.Get(nameof (ENUM_TYPE_BOARD_COLUMN_SPLIT_DONE));

    public static string ENUM_TYPE_BOARD_COLUMN_SPLIT_DONE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BOARD_COLUMN_SPLIT_DONE), culture);

    public static string ENUM_TYPE_BOARD_COLUMN_SPLIT_UNKNOWN() => AnalyticsResources.Get(nameof (ENUM_TYPE_BOARD_COLUMN_SPLIT_UNKNOWN));

    public static string ENUM_TYPE_BOARD_COLUMN_SPLIT_UNKNOWN(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BOARD_COLUMN_SPLIT_UNKNOWN), culture);

    public static string ENUM_TYPE_TEST_RUN_TYPE_AUTOMATED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RUN_TYPE_AUTOMATED));

    public static string ENUM_TYPE_TEST_RUN_TYPE_AUTOMATED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RUN_TYPE_AUTOMATED), culture);

    public static string ENUM_TYPE_TEST_RUN_TYPE_MANUAL() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RUN_TYPE_MANUAL));

    public static string ENUM_TYPE_TEST_RUN_TYPE_MANUAL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RUN_TYPE_MANUAL), culture);

    public static string ENUM_TYPE_SOURCE_WORK_FLOW_BUILD() => AnalyticsResources.Get(nameof (ENUM_TYPE_SOURCE_WORK_FLOW_BUILD));

    public static string ENUM_TYPE_SOURCE_WORK_FLOW_BUILD(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_SOURCE_WORK_FLOW_BUILD), culture);

    public static string ENUM_TYPE_SOURCE_WORK_FLOW_RELEASE() => AnalyticsResources.Get(nameof (ENUM_TYPE_SOURCE_WORK_FLOW_RELEASE));

    public static string ENUM_TYPE_SOURCE_WORK_FLOW_RELEASE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_SOURCE_WORK_FLOW_RELEASE), culture);

    public static string ENUM_TYPE_SOURCE_WORK_FLOW_MANUAL() => AnalyticsResources.Get(nameof (ENUM_TYPE_SOURCE_WORK_FLOW_MANUAL));

    public static string ENUM_TYPE_SOURCE_WORK_FLOW_MANUAL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_SOURCE_WORK_FLOW_MANUAL), culture);

    public static string ENTITY_FIELD_NAME_BUILD_PIPELINE_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_PIPELINE_NAME));

    public static string ENTITY_FIELD_NAME_BUILD_PIPELINE_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_PIPELINE_NAME), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_NAME));

    public static string ENTITY_FIELD_NAME_PIPELINE_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_NAME), culture);

    public static string ENTITY_SET_NAME_BUILD_PIPELINES() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_BUILD_PIPELINES));

    public static string ENTITY_SET_NAME_BUILD_PIPELINES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_BUILD_PIPELINES), culture);

    public static string ENTITY_SET_NAME_PIPELINES() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PIPELINES));

    public static string ENTITY_SET_NAME_PIPELINES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PIPELINES), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_PIPELINE_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_PIPELINE_NAME));

    public static string ENTITY_FIELD_NAME_RELEASE_PIPELINE_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_PIPELINE_NAME), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_PIPELINE_PATH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_PIPELINE_PATH));

    public static string ENTITY_FIELD_NAME_RELEASE_PIPELINE_PATH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_PIPELINE_PATH), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_PIPELINE_VERSION() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_PIPELINE_VERSION));

    public static string ENTITY_FIELD_NAME_RELEASE_PIPELINE_VERSION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_PIPELINE_VERSION), culture);

    public static string ENTITY_SET_NAME_RELEASE_PIPELINES() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_RELEASE_PIPELINES));

    public static string ENTITY_SET_NAME_RELEASE_PIPELINES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_RELEASE_PIPELINES), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_STAGE_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_STAGE_NAME));

    public static string ENTITY_FIELD_NAME_RELEASE_STAGE_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_STAGE_NAME), culture);

    public static string ENTITY_SET_NAME_RELEASE_STAGES() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_RELEASE_STAGES));

    public static string ENTITY_SET_NAME_RELEASE_STAGES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_RELEASE_STAGES), culture);

    public static string ENTITY_SET_NAME_TEST_RESULTS_DAILY() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_RESULTS_DAILY));

    public static string ENTITY_SET_NAME_TEST_RESULTS_DAILY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_RESULTS_DAILY), culture);

    public static string ENTITY_FIELD_NAME_BUILD_PIPELINE_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_PIPELINE_ID));

    public static string ENTITY_FIELD_NAME_BUILD_PIPELINE_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_PIPELINE_ID), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_ID));

    public static string ENTITY_FIELD_NAME_PIPELINE_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_ID), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_JOB() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_JOB));

    public static string ENTITY_FIELD_NAME_PIPELINE_JOB(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_JOB), culture);

    public static string ENTITY_FIELD_NAME_FULL_JOB_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FULL_JOB_NAME));

    public static string ENTITY_FIELD_NAME_FULL_JOB_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FULL_JOB_NAME), culture);

    public static string ENTITY_FIELD_NAME_STRATEGY_ATTRIBUTES() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STRATEGY_ATTRIBUTES));

    public static string ENTITY_FIELD_NAME_STRATEGY_ATTRIBUTES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STRATEGY_ATTRIBUTES), culture);

    public static string ENTITY_FIELD_NAME_JOB_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_JOB_NAME));

    public static string ENTITY_FIELD_NAME_JOB_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_JOB_NAME), culture);

    public static string ENTITY_FIELD_NAME_STAGE_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STAGE_NAME));

    public static string ENTITY_FIELD_NAME_STAGE_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STAGE_NAME), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_PIPELINE_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_PIPELINE_ID));

    public static string ENTITY_FIELD_NAME_RELEASE_PIPELINE_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_PIPELINE_ID), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_STAGE_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_STAGE_ID));

    public static string ENTITY_FIELD_NAME_RELEASE_STAGE_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_STAGE_ID), culture);

    public static string ENTITY_SET_NAME_RELEASE_PIPELINE_ACTIVITY_ATTRIBUTES() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_RELEASE_PIPELINE_ACTIVITY_ATTRIBUTES));

    public static string ENTITY_SET_NAME_RELEASE_PIPELINE_ACTIVITY_ATTRIBUTES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_RELEASE_PIPELINE_ACTIVITY_ATTRIBUTES), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_STEP() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_STEP));

    public static string ENTITY_FIELD_NAME_PIPELINE_STEP(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_STEP), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_ACTIVITY_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_ACTIVITY_TYPE));

    public static string ENTITY_FIELD_NAME_PIPELINE_ACTIVITY_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_ACTIVITY_TYPE), culture);

    public static string ENTITY_FIELD_NAME_ACTIVITY_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_TYPE));

    public static string ENTITY_FIELD_NAME_ACTIVITY_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_TYPE), culture);

    public static string ENTITYSET_NAME_TASKS() => AnalyticsResources.Get(nameof (ENTITYSET_NAME_TASKS));

    public static string ENTITYSET_NAME_TASKS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITYSET_NAME_TASKS), culture);

    public static string ENTITY_FIELD_NAME_TASK_DISPLAY_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_DISPLAY_NAME));

    public static string ENTITY_FIELD_NAME_TASK_DISPLAY_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_DISPLAY_NAME), culture);

    public static string ENTITY_FIELD_NAME_TASK_DEFINITION_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_DEFINITION_NAME));

    public static string ENTITY_FIELD_NAME_TASK_DEFINITION_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_DEFINITION_NAME), culture);

    public static string ENTITY_FIELD_NAME_TASK_DEFINITION_VERSION() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_DEFINITION_VERSION));

    public static string ENTITY_FIELD_NAME_TASK_DEFINITION_VERSION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_DEFINITION_VERSION), culture);

    public static string ENTITYSET_NAME_RELEASE_DEPLOYMENT_ATTRIBUTES() => AnalyticsResources.Get(nameof (ENTITYSET_NAME_RELEASE_DEPLOYMENT_ATTRIBUTES));

    public static string ENTITYSET_NAME_RELEASE_DEPLOYMENT_ATTRIBUTES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITYSET_NAME_RELEASE_DEPLOYMENT_ATTRIBUTES), culture);

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_REASON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_REASON));

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_REASON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_REASON), culture);

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_OUTCOME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_OUTCOME));

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_OUTCOME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_OUTCOME), culture);

    public static string ENTITY_FIELD_NAME_OPERATION_OUTCOME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_OPERATION_OUTCOME));

    public static string ENTITY_FIELD_NAME_OPERATION_OUTCOME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_OPERATION_OUTCOME), culture);

    public static string ENTITY_FIELD_NAME_ATTEMPT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ATTEMPT));

    public static string ENTITY_FIELD_NAME_ATTEMPT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ATTEMPT), culture);

    public static string ENTITYSET_NAME_PIPELINE_ARTIFACTS() => AnalyticsResources.Get(nameof (ENTITYSET_NAME_PIPELINE_ARTIFACTS));

    public static string ENTITYSET_NAME_PIPELINE_ARTIFACTS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITYSET_NAME_PIPELINE_ARTIFACTS), culture);

    public static string ENTITY_FIELD_NAME_ARTIFACT_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ARTIFACT_TYPE));

    public static string ENTITY_FIELD_NAME_ARTIFACT_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ARTIFACT_TYPE), culture);

    public static string ENTITY_FIELD_NAME_ARTIFACT_ROLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ARTIFACT_ROLE));

    public static string ENTITY_FIELD_NAME_ARTIFACT_ROLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ARTIFACT_ROLE), culture);

    public static string ENTITYSET_NAME_RELEASE_PIPELINE_ACTIVITIES() => AnalyticsResources.Get(nameof (ENTITYSET_NAME_RELEASE_PIPELINE_ACTIVITIES));

    public static string ENTITYSET_NAME_RELEASE_PIPELINE_ACTIVITIES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITYSET_NAME_RELEASE_PIPELINE_ACTIVITIES), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_CREATED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_CREATED_DATE));

    public static string ENTITY_FIELD_NAME_RELEASE_CREATED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_CREATED_DATE), culture);

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_QUEUED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_QUEUED_DATE));

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_QUEUED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_QUEUED_DATE), culture);

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_STARTED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_STARTED_DATE));

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_STARTED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_STARTED_DATE), culture);

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_COMPLETED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_COMPLETED_DATE));

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_COMPLETED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_COMPLETED_DATE), culture);

    public static string ENTITY_FIELD_NAME_ACTIVITY_STARTED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_STARTED_DATE));

    public static string ENTITY_FIELD_NAME_ACTIVITY_STARTED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_STARTED_DATE), culture);

    public static string ENTITY_FIELD_NAME_ACTIVITY_COMPLETED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_COMPLETED_DATE));

    public static string ENTITY_FIELD_NAME_ACTIVITY_COMPLETED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_COMPLETED_DATE), culture);

    public static string ENTITY_FIELD_NAME_APPROVAL_ASSIGNED_TO() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_APPROVAL_ASSIGNED_TO));

    public static string ENTITY_FIELD_NAME_APPROVAL_ASSIGNED_TO(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_APPROVAL_ASSIGNED_TO), culture);

    public static string ENTITY_FIELD_NAME_APPROVED_BY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_APPROVED_BY));

    public static string ENTITY_FIELD_NAME_APPROVED_BY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_APPROVED_BY), culture);

    public static string ENTITY_FIELD_NAME_TASK_LOG_PATH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_LOG_PATH));

    public static string ENTITY_FIELD_NAME_TASK_LOG_PATH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_LOG_PATH), culture);

    public static string ENTITY_FIELD_NAME_ACTIVITY_OUTCOME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_OUTCOME));

    public static string ENTITY_FIELD_NAME_ACTIVITY_OUTCOME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_OUTCOME), culture);

    public static string ENTITY_FIELD_NAME_SKIPPED_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SKIPPED_COUNT));

    public static string ENTITY_FIELD_NAME_SKIPPED_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SKIPPED_COUNT), culture);

    public static string ENTITY_FIELD_NAME_ACTIVITY_DURATION_SECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_DURATION_SECONDS));

    public static string ENTITY_FIELD_NAME_ACTIVITY_DURATION_SECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_DURATION_SECONDS), culture);

    public static string ENTITYSET_NAME_RELEASE_DEPLOYMENTS() => AnalyticsResources.Get(nameof (ENTITYSET_NAME_RELEASE_DEPLOYMENTS));

    public static string ENTITYSET_NAME_RELEASE_DEPLOYMENTS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITYSET_NAME_RELEASE_DEPLOYMENTS), culture);

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_ID));

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_ID), culture);

    public static string ENTITY_FIELD_NAME_REQUESTED_BY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUESTED_BY));

    public static string ENTITY_FIELD_NAME_REQUESTED_BY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUESTED_BY), culture);

    public static string ENTITY_FIELD_NAME_REQUESTED_FOR() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUESTED_FOR));

    public static string ENTITY_FIELD_NAME_REQUESTED_FOR(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUESTED_FOR), culture);

    public static string ENTITY_FIELD_NAME_PASSED_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PASSED_COUNT));

    public static string ENTITY_FIELD_NAME_PASSED_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PASSED_COUNT), culture);

    public static string ENTITY_FIELD_NAME_FAILED_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FAILED_COUNT));

    public static string ENTITY_FIELD_NAME_FAILED_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FAILED_COUNT), culture);

    public static string ENTITY_FIELD_NAME_CANCELED_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CANCELED_COUNT));

    public static string ENTITY_FIELD_NAME_CANCELED_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CANCELED_COUNT), culture);

    public static string ENTITY_FIELD_NAME_ATTEMPT_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ATTEMPT_COUNT));

    public static string ENTITY_FIELD_NAME_ATTEMPT_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ATTEMPT_COUNT), culture);

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_DURATION_IN_MINUTES() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_DURATION_IN_MINUTES));

    public static string ENTITY_FIELD_NAME_DEPLOYMENT_DURATION_IN_MINUTES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_DEPLOYMENT_DURATION_IN_MINUTES), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_NAME));

    public static string ENTITY_FIELD_NAME_RELEASE_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_NAME), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_REASON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_REASON));

    public static string ENTITY_FIELD_NAME_RELEASE_REASON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_REASON), culture);

    public static string ENTITY_FIELD_NAME_RELEASE_STATUS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_STATUS));

    public static string ENTITY_FIELD_NAME_RELEASE_STATUS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELEASE_STATUS), culture);

    public static string ENUM_TYPE_ARTIFACT_ROLE_NOT_PRIMARY() => AnalyticsResources.Get(nameof (ENUM_TYPE_ARTIFACT_ROLE_NOT_PRIMARY));

    public static string ENUM_TYPE_ARTIFACT_ROLE_NOT_PRIMARY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ARTIFACT_ROLE_NOT_PRIMARY), culture);

    public static string ENUM_TYPE_ARTIFACT_ROLE_PRIMARY() => AnalyticsResources.Get(nameof (ENUM_TYPE_ARTIFACT_ROLE_PRIMARY));

    public static string ENUM_TYPE_ARTIFACT_ROLE_PRIMARY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ARTIFACT_ROLE_PRIMARY), culture);

    public static string ENUM_TYPE_DEPLOYMENT_REASON_NONE() => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_REASON_NONE));

    public static string ENUM_TYPE_DEPLOYMENT_REASON_NONE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_REASON_NONE), culture);

    public static string ENUM_TYPE_DEPLOYMENT_REASON_MANUAL() => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_REASON_MANUAL));

    public static string ENUM_TYPE_DEPLOYMENT_REASON_MANUAL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_REASON_MANUAL), culture);

    public static string ENUM_TYPE_DEPLOYMENT_REASON_AUTOMATED() => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_REASON_AUTOMATED));

    public static string ENUM_TYPE_DEPLOYMENT_REASON_AUTOMATED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_REASON_AUTOMATED), culture);

    public static string ENUM_TYPE_DEPLOYMENT_REASON_SCHEDULED() => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_REASON_SCHEDULED));

    public static string ENUM_TYPE_DEPLOYMENT_REASON_SCHEDULED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_REASON_SCHEDULED), culture);

    public static string ENUM_TYPE_DEPLOYMENT_REASON_REDEPLOY_TRIGGER() => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_REASON_REDEPLOY_TRIGGER));

    public static string ENUM_TYPE_DEPLOYMENT_REASON_REDEPLOY_TRIGGER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_REASON_REDEPLOY_TRIGGER), culture);

    public static string ENUM_TYPE_DEPLOYMENT_OUTCOME_UNDEFINED() => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_OUTCOME_UNDEFINED));

    public static string ENUM_TYPE_DEPLOYMENT_OUTCOME_UNDEFINED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_OUTCOME_UNDEFINED), culture);

    public static string ENUM_TYPE_DEPLOYMENT_OUTCOME_NOT_DEPLOYED() => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_OUTCOME_NOT_DEPLOYED));

    public static string ENUM_TYPE_DEPLOYMENT_OUTCOME_NOT_DEPLOYED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_OUTCOME_NOT_DEPLOYED), culture);

    public static string ENUM_TYPE_DEPLOYMENT_OUTCOME_IN_PROGRESS() => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_OUTCOME_IN_PROGRESS));

    public static string ENUM_TYPE_DEPLOYMENT_OUTCOME_IN_PROGRESS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_OUTCOME_IN_PROGRESS), culture);

    public static string ENUM_TYPE_DEPLOYMENT_OUTCOME_SUCCEEDED() => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_OUTCOME_SUCCEEDED));

    public static string ENUM_TYPE_DEPLOYMENT_OUTCOME_SUCCEEDED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_OUTCOME_SUCCEEDED), culture);

    public static string ENUM_TYPE_DEPLOYMENT_OUTCOME_PARTIALLY_SUCCEEDED() => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_OUTCOME_PARTIALLY_SUCCEEDED));

    public static string ENUM_TYPE_DEPLOYMENT_OUTCOME_PARTIALLY_SUCCEEDED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_OUTCOME_PARTIALLY_SUCCEEDED), culture);

    public static string ENUM_TYPE_DEPLOYMENT_OUTCOME_FAILED() => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_OUTCOME_FAILED));

    public static string ENUM_TYPE_DEPLOYMENT_OUTCOME_FAILED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_DEPLOYMENT_OUTCOME_FAILED), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_UNDEFINED() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_UNDEFINED));

    public static string ENUM_TYPE_OPERATION_OUTCOME_UNDEFINED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_UNDEFINED), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_QUEUED() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_QUEUED));

    public static string ENUM_TYPE_OPERATION_OUTCOME_QUEUED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_QUEUED), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_SCHEDULED() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_SCHEDULED));

    public static string ENUM_TYPE_OPERATION_OUTCOME_SCHEDULED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_SCHEDULED), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_PENDING() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_PENDING));

    public static string ENUM_TYPE_OPERATION_OUTCOME_PENDING(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_PENDING), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_APPROVED() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_APPROVED));

    public static string ENUM_TYPE_OPERATION_OUTCOME_APPROVED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_APPROVED), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_REJECTED() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_REJECTED));

    public static string ENUM_TYPE_OPERATION_OUTCOME_REJECTED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_REJECTED), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_DEFERRED() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_DEFERRED));

    public static string ENUM_TYPE_OPERATION_OUTCOME_DEFERRED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_DEFERRED), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_QUEUED_FOR_AGENT() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_QUEUED_FOR_AGENT));

    public static string ENUM_TYPE_OPERATION_OUTCOME_QUEUED_FOR_AGENT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_QUEUED_FOR_AGENT), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_PHASE_IN_PROGRESS() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_PHASE_IN_PROGRESS));

    public static string ENUM_TYPE_OPERATION_OUTCOME_PHASE_IN_PROGRESS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_PHASE_IN_PROGRESS), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_PHASE_SUCCEEDED() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_PHASE_SUCCEEDED));

    public static string ENUM_TYPE_OPERATION_OUTCOME_PHASE_SUCCEEDED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_PHASE_SUCCEEDED), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_PHASE_PARTIALLY_SUCCEEDED() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_PHASE_PARTIALLY_SUCCEEDED));

    public static string ENUM_TYPE_OPERATION_OUTCOME_PHASE_PARTIALLY_SUCCEEDED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_PHASE_PARTIALLY_SUCCEEDED), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_PHASE_FAILED() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_PHASE_FAILED));

    public static string ENUM_TYPE_OPERATION_OUTCOME_PHASE_FAILED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_PHASE_FAILED), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_CANCELED() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_CANCELED));

    public static string ENUM_TYPE_OPERATION_OUTCOME_CANCELED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_CANCELED), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_PHASE_CANCELED() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_PHASE_CANCELED));

    public static string ENUM_TYPE_OPERATION_OUTCOME_PHASE_CANCELED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_PHASE_CANCELED), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_MANUAL_INTERVENTION_PENDING() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_MANUAL_INTERVENTION_PENDING));

    public static string ENUM_TYPE_OPERATION_OUTCOME_MANUAL_INTERVENTION_PENDING(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_MANUAL_INTERVENTION_PENDING), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_QUEUED_FOR_PIPELINE() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_QUEUED_FOR_PIPELINE));

    public static string ENUM_TYPE_OPERATION_OUTCOME_QUEUED_FOR_PIPELINE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_QUEUED_FOR_PIPELINE), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_CANCELLING() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_CANCELLING));

    public static string ENUM_TYPE_OPERATION_OUTCOME_CANCELLING(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_CANCELLING), culture);

    public static string ENUM_TYPE_OPERATION_OUTCOME_GATE_FAILED() => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_GATE_FAILED));

    public static string ENUM_TYPE_OPERATION_OUTCOME_GATE_FAILED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_OPERATION_OUTCOME_GATE_FAILED), culture);

    public static string ENUM_TYPE_PIPELINE_STEP_PRE_DEPLOY() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_STEP_PRE_DEPLOY));

    public static string ENUM_TYPE_PIPELINE_STEP_PRE_DEPLOY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_STEP_PRE_DEPLOY), culture);

    public static string ENUM_TYPE_PIPELINE_STEP_DEPLOY() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_STEP_DEPLOY));

    public static string ENUM_TYPE_PIPELINE_STEP_DEPLOY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_STEP_DEPLOY), culture);

    public static string ENUM_TYPE_PIPELINE_STEP_POST_DEPLOY() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_STEP_POST_DEPLOY));

    public static string ENUM_TYPE_PIPELINE_STEP_POST_DEPLOY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_STEP_POST_DEPLOY), culture);

    public static string ENUM_TYPE_PIPELINE_STEP_PRE_GATE() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_STEP_PRE_GATE));

    public static string ENUM_TYPE_PIPELINE_STEP_PRE_GATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_STEP_PRE_GATE), culture);

    public static string ENUM_TYPE_PIPELINE_STEP_POST_GATE() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_STEP_POST_GATE));

    public static string ENUM_TYPE_PIPELINE_STEP_POST_GATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_STEP_POST_GATE), culture);

    public static string ENUM_TYPE_ACTIVITY_TYPE_APPROVAL() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_TYPE_APPROVAL));

    public static string ENUM_TYPE_ACTIVITY_TYPE_APPROVAL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_TYPE_APPROVAL), culture);

    public static string ENUM_TYPE_ACTIVITY_TYPE_GATE_TASK() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_TYPE_GATE_TASK));

    public static string ENUM_TYPE_ACTIVITY_TYPE_GATE_TASK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_TYPE_GATE_TASK), culture);

    public static string ENUM_TYPE_ACTIVITY_TYPE_DEPLOY_TASK() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_TYPE_DEPLOY_TASK));

    public static string ENUM_TYPE_ACTIVITY_TYPE_DEPLOY_TASK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_TYPE_DEPLOY_TASK), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_UNDEFINED() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_UNDEFINED));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_UNDEFINED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_UNDEFINED), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_PENDING() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_PENDING));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_PENDING(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_PENDING), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_REJECTED() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_REJECTED));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_REJECTED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_REJECTED), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_ABANDONED() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_ABANDONED));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_ABANDONED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_ABANDONED), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_STOPPED() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_STOPPED));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_STOPPED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_STOPPED), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_REASSIGNED() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_REASSIGNED));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_REASSIGNED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_REASSIGNED), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_DONE() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_DONE));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_DONE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_DONE), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_CANCELED() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_CANCELED));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_CANCELED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_CANCELED), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_SKIPPED() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_SKIPPED));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_SKIPPED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_SKIPPED), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_PARTIALLY_SUCCEEDED() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_PARTIALLY_SUCCEEDED));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_PARTIALLY_SUCCEEDED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_PARTIALLY_SUCCEEDED), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_SUCCEEDED() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_SUCCEEDED));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_SUCCEEDED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_SUCCEEDED), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_SUCCEEDED_WITH_ISSUES() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_SUCCEEDED_WITH_ISSUES));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_SUCCEEDED_WITH_ISSUES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_SUCCEEDED_WITH_ISSUES), culture);

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_FAILED() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_FAILED));

    public static string ENUM_TYPE_ACTIVITY_OUTCOME_FAILED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_OUTCOME_FAILED), culture);

    public static string ENTITY_FIELD_NAME_PRIMARY_BRANCH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PRIMARY_BRANCH));

    public static string ENTITY_FIELD_NAME_PRIMARY_BRANCH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PRIMARY_BRANCH), culture);

    public static string FAILED_TO_SPLIT_PARTITION(object arg0, object arg1, object arg2) => AnalyticsResources.Format(nameof (FAILED_TO_SPLIT_PARTITION), arg0, arg1, arg2);

    public static string FAILED_TO_SPLIT_PARTITION(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (FAILED_TO_SPLIT_PARTITION), culture, arg0, arg1, arg2);
    }

    public static string STAGE_ENVELOPE_PARSE_FIELD_IGNORED(object arg0, object arg1) => AnalyticsResources.Format(nameof (STAGE_ENVELOPE_PARSE_FIELD_IGNORED), arg0, arg1);

    public static string STAGE_ENVELOPE_PARSE_FIELD_IGNORED(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (STAGE_ENVELOPE_PARSE_FIELD_IGNORED), culture, arg0, arg1);
    }

    public static string QUERY_EXCEEDS_MAX_WIDTH() => AnalyticsResources.Get(nameof (QUERY_EXCEEDS_MAX_WIDTH));

    public static string QUERY_EXCEEDS_MAX_WIDTH(CultureInfo culture) => AnalyticsResources.Get(nameof (QUERY_EXCEEDS_MAX_WIDTH), culture);

    public static string NUMBERS_OF_RECORDS_DONT_MATCH_FOR_TABLE(
      object arg0,
      object arg1,
      object arg2)
    {
      return AnalyticsResources.Format(nameof (NUMBERS_OF_RECORDS_DONT_MATCH_FOR_TABLE), arg0, arg1, arg2);
    }

    public static string NUMBERS_OF_RECORDS_DONT_MATCH_FOR_TABLE(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (NUMBERS_OF_RECORDS_DONT_MATCH_FOR_TABLE), culture, arg0, arg1, arg2);
    }

    public static string SESSION_TERMINATED_THRESHOLD_ELAPSED_TIME_EXCEEDED(
      object arg0,
      object arg1)
    {
      return AnalyticsResources.Format(nameof (SESSION_TERMINATED_THRESHOLD_ELAPSED_TIME_EXCEEDED), arg0, arg1);
    }

    public static string SESSION_TERMINATED_THRESHOLD_ELAPSED_TIME_EXCEEDED(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (SESSION_TERMINATED_THRESHOLD_ELAPSED_TIME_EXCEEDED), culture, arg0, arg1);
    }

    public static string ENTITY_FIELD_NAME_COMMENT_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMENT_COUNT));

    public static string ENTITY_FIELD_NAME_COMMENT_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMENT_COUNT), culture);

    public static string ENTITY_FIELD_NAME_TASK_RESULT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_RESULT));

    public static string ENTITY_FIELD_NAME_TASK_RESULT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_RESULT), culture);

    public static string ENUM_TYPE_BUILD_TASK_OUTCOME_SUCCEEDED() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_TASK_OUTCOME_SUCCEEDED));

    public static string ENUM_TYPE_BUILD_TASK_OUTCOME_SUCCEEDED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_TASK_OUTCOME_SUCCEEDED), culture);

    public static string ENUM_TYPE_BUILD_TASK_OUTCOME_SUCCEEDED_WITH_ISSUES() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_TASK_OUTCOME_SUCCEEDED_WITH_ISSUES));

    public static string ENUM_TYPE_BUILD_TASK_OUTCOME_SUCCEEDED_WITH_ISSUES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_TASK_OUTCOME_SUCCEEDED_WITH_ISSUES), culture);

    public static string ENUM_TYPE_BUILD_TASK_OUTCOME_FAILED() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_TASK_OUTCOME_FAILED));

    public static string ENUM_TYPE_BUILD_TASK_OUTCOME_FAILED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_TASK_OUTCOME_FAILED), culture);

    public static string ENUM_TYPE_BUILD_TASK_OUTCOME_CANCELED() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_TASK_OUTCOME_CANCELED));

    public static string ENUM_TYPE_BUILD_TASK_OUTCOME_CANCELED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_TASK_OUTCOME_CANCELED), culture);

    public static string ENUM_TYPE_BUILD_TASK_OUTCOME_SKIPPED() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_TASK_OUTCOME_SKIPPED));

    public static string ENUM_TYPE_BUILD_TASK_OUTCOME_SKIPPED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_TASK_OUTCOME_SKIPPED), culture);

    public static string ENUM_TYPE_BUILD_TASK_OUTCOME_ABANDONED() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_TASK_OUTCOME_ABANDONED));

    public static string ENUM_TYPE_BUILD_TASK_OUTCOME_ABANDONED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_TASK_OUTCOME_ABANDONED), culture);

    public static string ENUM_TYPE_BUILD_REASON_NONE() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_NONE));

    public static string ENUM_TYPE_BUILD_REASON_NONE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_NONE), culture);

    public static string ENUM_TYPE_BUILD_REASON_MANUAL() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_MANUAL));

    public static string ENUM_TYPE_BUILD_REASON_MANUAL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_MANUAL), culture);

    public static string ENUM_TYPE_BUILD_REASON_INDIVIDUAL_CI() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_INDIVIDUAL_CI));

    public static string ENUM_TYPE_BUILD_REASON_INDIVIDUAL_CI(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_INDIVIDUAL_CI), culture);

    public static string ENUM_TYPE_BUILD_REASON_BATCHED_CI() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_BATCHED_CI));

    public static string ENUM_TYPE_BUILD_REASON_BATCHED_CI(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_BATCHED_CI), culture);

    public static string ENUM_TYPE_BUILD_REASON_SCHEDULE() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_SCHEDULE));

    public static string ENUM_TYPE_BUILD_REASON_SCHEDULE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_SCHEDULE), culture);

    public static string ENUM_TYPE_BUILD_REASON_USER_CREATED() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_USER_CREATED));

    public static string ENUM_TYPE_BUILD_REASON_USER_CREATED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_USER_CREATED), culture);

    public static string ENUM_TYPE_BUILD_REASON_VALIDATE_SHELVESET() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_VALIDATE_SHELVESET));

    public static string ENUM_TYPE_BUILD_REASON_VALIDATE_SHELVESET(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_VALIDATE_SHELVESET), culture);

    public static string ENUM_TYPE_BUILD_REASON_CHECK_IN_SHELVESET() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_CHECK_IN_SHELVESET));

    public static string ENUM_TYPE_BUILD_REASON_CHECK_IN_SHELVESET(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_CHECK_IN_SHELVESET), culture);

    public static string ENUM_TYPE_BUILD_REASON_PULL_REQUEST() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_PULL_REQUEST));

    public static string ENUM_TYPE_BUILD_REASON_PULL_REQUEST(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_PULL_REQUEST), culture);

    public static string ENUM_TYPE_BUILD_REASON_BUILD_COMPLETION() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_BUILD_COMPLETION));

    public static string ENUM_TYPE_BUILD_REASON_BUILD_COMPLETION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_REASON_BUILD_COMPLETION), culture);

    public static string ENUM_TYPE_BUILD_PROCESS_TYPE_DESIGNER() => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_PROCESS_TYPE_DESIGNER));

    public static string ENUM_TYPE_BUILD_PROCESS_TYPE_DESIGNER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_BUILD_PROCESS_TYPE_DESIGNER), culture);

    public static string ENTITY_FIELD_NAME_BUILD_PIPELINE_VERSION() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_PIPELINE_VERSION));

    public static string ENTITY_FIELD_NAME_BUILD_PIPELINE_VERSION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_PIPELINE_VERSION), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_VERSION() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_VERSION));

    public static string ENTITY_FIELD_NAME_PIPELINE_VERSION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_VERSION), culture);

    public static string ENTITY_FIELD_NAME_BUILD_PIPELINE_PROCESS_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_PIPELINE_PROCESS_TYPE));

    public static string ENTITY_FIELD_NAME_BUILD_PIPELINE_PROCESS_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_PIPELINE_PROCESS_TYPE), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_PROCESS_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_PROCESS_TYPE));

    public static string ENTITY_FIELD_NAME_PIPELINE_PROCESS_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_PROCESS_TYPE), culture);

    public static string ENTITY_FIELD_NAME_BUILD_PIPELINE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_PIPELINE));

    public static string ENTITY_FIELD_NAME_BUILD_PIPELINE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_PIPELINE), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE));

    public static string ENTITY_FIELD_NAME_PIPELINE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_ENVIRONMENT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_ENVIRONMENT));

    public static string ENTITY_FIELD_NAME_PIPELINE_ENVIRONMENT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_ENVIRONMENT), culture);

    public static string ENTITY_FIELD_NAME_BRANCH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BRANCH));

    public static string ENTITY_FIELD_NAME_BRANCH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BRANCH), culture);

    public static string ENTITY_FIELD_NAME_BUILD_NUMBER() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_NUMBER));

    public static string ENTITY_FIELD_NAME_BUILD_NUMBER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_NUMBER), culture);

    public static string ENTITY_FIELD_NAME_RUN_NUMBER() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RUN_NUMBER));

    public static string ENTITY_FIELD_NAME_RUN_NUMBER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RUN_NUMBER), culture);

    public static string ENTITY_FIELD_NAME_BUILD_NUMBER_REVISION() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_NUMBER_REVISION));

    public static string ENTITY_FIELD_NAME_BUILD_NUMBER_REVISION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_NUMBER_REVISION), culture);

    public static string ENTITY_FIELD_NAME_NUMBER_REVISION() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_NUMBER_REVISION));

    public static string ENTITY_FIELD_NAME_NUMBER_REVISION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_NUMBER_REVISION), culture);

    public static string ENTITY_FIELD_NAME_BUILD_REASON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_REASON));

    public static string ENTITY_FIELD_NAME_BUILD_REASON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_REASON), culture);

    public static string ENTITY_FIELD_NAME_RUN_REASON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RUN_REASON));

    public static string ENTITY_FIELD_NAME_RUN_REASON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RUN_REASON), culture);

    public static string ENTITY_FIELD_NAME_BUILD_RESULT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_RESULT));

    public static string ENTITY_FIELD_NAME_BUILD_RESULT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_RESULT), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_RUN_OUTCOME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_RUN_OUTCOME));

    public static string ENTITY_FIELD_NAME_PIPELINE_RUN_OUTCOME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_RUN_OUTCOME), culture);

    public static string ENTITY_FIELD_NAME_RUN_OUTCOME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RUN_OUTCOME));

    public static string ENTITY_FIELD_NAME_RUN_OUTCOME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RUN_OUTCOME), culture);

    public static string ENTITY_FIELD_NAME_QUEUED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_QUEUED_DATE));

    public static string ENTITY_FIELD_NAME_QUEUED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_QUEUED_DATE), culture);

    public static string ENTITY_FIELD_NAME_QUEUED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_QUEUED_ON));

    public static string ENTITY_FIELD_NAME_QUEUED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_QUEUED_ON), culture);

    public static string ENTITY_FIELD_NAME_STARTED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STARTED_ON));

    public static string ENTITY_FIELD_NAME_STARTED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STARTED_ON), culture);

    public static string ENTITY_FIELD_NAME_JOB_STARTED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_JOB_STARTED_ON));

    public static string ENTITY_FIELD_NAME_JOB_STARTED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_JOB_STARTED_ON), culture);

    public static string ENTITY_FIELD_NAME_JOB_ENDED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_JOB_ENDED_ON));

    public static string ENTITY_FIELD_NAME_JOB_ENDED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_JOB_ENDED_ON), culture);

    public static string ENTITY_FIELD_NAME_BUILD_DURATION_SECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_DURATION_SECONDS));

    public static string ENTITY_FIELD_NAME_BUILD_DURATION_SECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_DURATION_SECONDS), culture);

    public static string ENTITY_FIELD_NAME_QUEUE_DURATION_SECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_QUEUE_DURATION_SECONDS));

    public static string ENTITY_FIELD_NAME_QUEUE_DURATION_SECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_QUEUE_DURATION_SECONDS), culture);

    public static string ENTITY_FIELD_NAME_TOTAL_DURATION_SECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TOTAL_DURATION_SECONDS));

    public static string ENTITY_FIELD_NAME_TOTAL_DURATION_SECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TOTAL_DURATION_SECONDS), culture);

    public static string ENTITY_FIELD_NAME_TASK() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK));

    public static string ENTITY_FIELD_NAME_TASK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_TASK() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_TASK));

    public static string ENTITY_FIELD_NAME_PIPELINE_TASK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_TASK), culture);

    public static string ENTITY_FIELD_NAME_BUILD_CREATED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_CREATED_ON));

    public static string ENTITY_FIELD_NAME_BUILD_CREATED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_CREATED_ON), culture);

    public static string ENTITY_FIELD_NAME_BUILD_QUEUED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_QUEUED_ON));

    public static string ENTITY_FIELD_NAME_BUILD_QUEUED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_QUEUED_ON), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_RUN_QUEUED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_RUN_QUEUED_ON));

    public static string ENTITY_FIELD_NAME_PIPELINE_RUN_QUEUED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_RUN_QUEUED_ON), culture);

    public static string ENTITY_FIELD_NAME_BUILD_STARTED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_STARTED_ON));

    public static string ENTITY_FIELD_NAME_BUILD_STARTED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_STARTED_ON), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_RUN_STARTED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_RUN_STARTED_ON));

    public static string ENTITY_FIELD_NAME_PIPELINE_RUN_STARTED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_RUN_STARTED_ON), culture);

    public static string ENTITY_FIELD_NAME_BUILD_COMPLETED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_COMPLETED_ON));

    public static string ENTITY_FIELD_NAME_BUILD_COMPLETED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BUILD_COMPLETED_ON), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_RUN_COMPLETED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_RUN_COMPLETED_ON));

    public static string ENTITY_FIELD_NAME_PIPELINE_RUN_COMPLETED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_RUN_COMPLETED_ON), culture);

    public static string ENTITY_FIELD_NAME_ACTIVITY_STARTED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_STARTED_ON));

    public static string ENTITY_FIELD_NAME_ACTIVITY_STARTED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_STARTED_ON), culture);

    public static string ENTITY_FIELD_NAME_ACTIVITY_COMPLETED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_COMPLETED_ON));

    public static string ENTITY_FIELD_NAME_ACTIVITY_COMPLETED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACTIVITY_COMPLETED_ON), culture);

    public static string ENTITY_FIELD_NAME_COMPLETED_ON() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMPLETED_ON));

    public static string ENTITY_FIELD_NAME_COMPLETED_ON(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMPLETED_ON), culture);

    public static string ENTITY_FIELD_NAME_SUCCEEDED_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SUCCEEDED_COUNT));

    public static string ENTITY_FIELD_NAME_SUCCEEDED_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SUCCEEDED_COUNT), culture);

    public static string ENTITY_FIELD_NAME_PARTIALLY_SUCCEEDED_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PARTIALLY_SUCCEEDED_COUNT));

    public static string ENTITY_FIELD_NAME_PARTIALLY_SUCCEEDED_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PARTIALLY_SUCCEEDED_COUNT), culture);

    public static string ENTITY_FIELD_NAME_ABANDONED_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ABANDONED_COUNT));

    public static string ENTITY_FIELD_NAME_ABANDONED_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ABANDONED_COUNT), culture);

    public static string ENTITY_FIELD_NAME_SUCCEEDED_WITH_ISSUES_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SUCCEEDED_WITH_ISSUES_COUNT));

    public static string ENTITY_FIELD_NAME_SUCCEEDED_WITH_ISSUES_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SUCCEEDED_WITH_ISSUES_COUNT), culture);

    public static string ODATA_QUERY_SELECT_EXPAND_TOO_WIDE(object arg0, object arg1) => AnalyticsResources.Format(nameof (ODATA_QUERY_SELECT_EXPAND_TOO_WIDE), arg0, arg1);

    public static string ODATA_QUERY_SELECT_EXPAND_TOO_WIDE(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (ODATA_QUERY_SELECT_EXPAND_TOO_WIDE), culture, arg0, arg1);
    }

    public static string ENTITY_FIELD_NAME_TASK_DEFINITION_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_DEFINITION_ID));

    public static string ENTITY_FIELD_NAME_TASK_DEFINITION_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TASK_DEFINITION_ID), culture);

    public static string ODATA_QUERY_WIT_DESCENDANTS_TOO_WIDE(object arg0, object arg1) => AnalyticsResources.Format(nameof (ODATA_QUERY_WIT_DESCENDANTS_TOO_WIDE), arg0, arg1);

    public static string ODATA_QUERY_WIT_DESCENDANTS_TOO_WIDE(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (ODATA_QUERY_WIT_DESCENDANTS_TOO_WIDE), culture, arg0, arg1);
    }

    public static string ENTITY_SET_NAME_TEST_CONFIGURATIONS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_CONFIGURATIONS));

    public static string ENTITY_SET_NAME_TEST_CONFIGURATIONS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_CONFIGURATIONS), culture);

    public static string ENTITY_FIELD_NAME_TEST_CONFIGURATION_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_CONFIGURATION_ID));

    public static string ENTITY_FIELD_NAME_TEST_CONFIGURATION_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_CONFIGURATION_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_CONFIGURATION_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_CONFIGURATION_NAME));

    public static string ENTITY_FIELD_NAME_TEST_CONFIGURATION_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_CONFIGURATION_NAME), culture);

    public static string ENTITY_FIELD_NAME_TEST_CONFIGURATION_STATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_CONFIGURATION_STATE));

    public static string ENTITY_FIELD_NAME_TEST_CONFIGURATION_STATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_CONFIGURATION_STATE), culture);

    public static string ENTITY_SET_NAME_TEST_SUITES() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_SUITES));

    public static string ENTITY_SET_NAME_TEST_SUITES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_SUITES), culture);

    public static string ENTITY_SET_NAME_TEST_POINTS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_POINTS));

    public static string ENTITY_SET_NAME_TEST_POINTS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_POINTS), culture);

    public static string ENUM_TYPE_TEST_RESULT_STATE_COMPLETED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_COMPLETED));

    public static string ENUM_TYPE_TEST_RESULT_STATE_COMPLETED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_COMPLETED), culture);

    public static string ENUM_TYPE_TEST_RESULT_STATE_IN_PROGRESS() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_IN_PROGRESS));

    public static string ENUM_TYPE_TEST_RESULT_STATE_IN_PROGRESS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_IN_PROGRESS), culture);

    public static string ENUM_TYPE_TEST_RESULT_STATE_MAX_VALUE() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_MAX_VALUE));

    public static string ENUM_TYPE_TEST_RESULT_STATE_MAX_VALUE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_MAX_VALUE), culture);

    public static string ENUM_TYPE_TEST_RESULT_STATE_PAUSED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_PAUSED));

    public static string ENUM_TYPE_TEST_RESULT_STATE_PAUSED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_PAUSED), culture);

    public static string ENUM_TYPE_TEST_RESULT_STATE_PENDING() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_PENDING));

    public static string ENUM_TYPE_TEST_RESULT_STATE_PENDING(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_PENDING), culture);

    public static string ENUM_TYPE_TEST_RESULT_STATE_QUEUED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_QUEUED));

    public static string ENUM_TYPE_TEST_RESULT_STATE_QUEUED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_QUEUED), culture);

    public static string ENUM_TYPE_TEST_RESULT_STATE_UNSPECIFIED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_UNSPECIFIED));

    public static string ENUM_TYPE_TEST_RESULT_STATE_UNSPECIFIED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_RESULT_STATE_UNSPECIFIED), culture);

    public static string ENUM_TYPE_TEST_AUTOMATION_STATUS_MAX_VALUE() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_AUTOMATION_STATUS_MAX_VALUE));

    public static string ENUM_TYPE_TEST_AUTOMATION_STATUS_MAX_VALUE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_AUTOMATION_STATUS_MAX_VALUE), culture);

    public static string ENUM_TYPE_TEST_AUTOMATION_STATUS_NOT_AUTOMATED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_AUTOMATION_STATUS_NOT_AUTOMATED));

    public static string ENUM_TYPE_TEST_AUTOMATION_STATUS_NOT_AUTOMATED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_AUTOMATION_STATUS_NOT_AUTOMATED), culture);

    public static string ENUM_TYPE_TEST_AUTOMATION_STATUS_PLANNED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_AUTOMATION_STATUS_PLANNED));

    public static string ENUM_TYPE_TEST_AUTOMATION_STATUS_PLANNED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_AUTOMATION_STATUS_PLANNED), culture);

    public static string ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_STATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_STATE));

    public static string ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_STATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_STATE), culture);

    public static string ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_OUTCOME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_OUTCOME));

    public static string ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_OUTCOME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_OUTCOME), culture);

    public static string ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_CHANGED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_CHANGED_DATE));

    public static string ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_CHANGED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_LAST_RESULT_CHANGED_DATE), culture);

    public static string ENTITY_FIELD_NAME_TEST_POINT_TESTER() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_TESTER));

    public static string ENTITY_FIELD_NAME_TEST_POINT_TESTER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_TESTER), culture);

    public static string ENTITY_FIELD_NAME_TEST_POINT_TEST_CASE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_TEST_CASE));

    public static string ENTITY_FIELD_NAME_TEST_POINT_TEST_CASE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_TEST_CASE), culture);

    public static string ENTITY_FIELD_NAME_TEST_POINT_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_ID));

    public static string ENTITY_FIELD_NAME_TEST_POINT_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_POINT_TEST_SUITE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_TEST_SUITE));

    public static string ENTITY_FIELD_NAME_TEST_POINT_TEST_SUITE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_TEST_SUITE), culture);

    public static string ENTITY_FIELD_NAME_TEST_POINT_TEST_CONFIGURATION() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_TEST_CONFIGURATION));

    public static string ENTITY_FIELD_NAME_TEST_POINT_TEST_CONFIGURATION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_POINT_TEST_CONFIGURATION), culture);

    public static string ENTITY_FIELD_NAME_REQUIREMENT_WORK_ITEM() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUIREMENT_WORK_ITEM));

    public static string ENTITY_FIELD_NAME_REQUIREMENT_WORK_ITEM(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUIREMENT_WORK_ITEM), culture);

    public static string ENTITY_FIELD_NAME_TEST_PLAN_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_PLAN_ID));

    public static string ENTITY_FIELD_NAME_TEST_PLAN_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_PLAN_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_PLAN_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_PLAN_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_PLAN_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_PLAN_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_DEPTH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_DEPTH));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_DEPTH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_DEPTH), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_10_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_10_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_10_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_10_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_10_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_10_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_10_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_10_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_11_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_11_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_11_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_11_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_11_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_11_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_11_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_11_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_12_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_12_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_12_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_12_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_12_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_12_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_12_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_12_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_13_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_13_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_13_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_13_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_13_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_13_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_13_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_13_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_14_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_14_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_14_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_14_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_14_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_14_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_14_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_14_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_1_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_1_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_1_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_1_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_1_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_1_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_1_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_1_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_2_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_2_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_2_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_2_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_2_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_2_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_2_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_2_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_3_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_3_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_3_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_3_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_3_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_3_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_3_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_3_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_4_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_4_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_4_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_4_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_4_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_4_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_4_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_4_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_5_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_5_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_5_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_5_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_5_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_5_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_5_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_5_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_6_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_6_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_6_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_6_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_6_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_6_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_6_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_6_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_7_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_7_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_7_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_7_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_7_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_7_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_7_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_7_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_8_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_8_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_8_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_8_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_8_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_8_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_8_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_8_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_9_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_9_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_9_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_9_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_9_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_9_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_9_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_LEVEL_9_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_ORDER_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_ORDER_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_ORDER_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_ORDER_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_REQUIREMENT_WORK_ITEM_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_REQUIREMENT_WORK_ITEM_ID));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_REQUIREMENT_WORK_ITEM_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_REQUIREMENT_WORK_ITEM_ID), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_TITLE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_TITLE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_TITLE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_TITLE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_TYPE));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_TYPE), culture);

    public static string ENTITY_FIELD_NAME_TEST_SUITE_WORK_ITEM() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_WORK_ITEM));

    public static string ENTITY_FIELD_NAME_TEST_SUITE_WORK_ITEM(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_SUITE_WORK_ITEM), culture);

    public static string ENUM_TYPE_TEST_SUITE_TYPE_NONE() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_SUITE_TYPE_NONE));

    public static string ENUM_TYPE_TEST_SUITE_TYPE_NONE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_SUITE_TYPE_NONE), culture);

    public static string ENUM_TYPE_TEST_SUITE_TYPE_QUERY_BASED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_SUITE_TYPE_QUERY_BASED));

    public static string ENUM_TYPE_TEST_SUITE_TYPE_QUERY_BASED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_SUITE_TYPE_QUERY_BASED), culture);

    public static string ENUM_TYPE_TEST_SUITE_TYPE_REQUIREMENT_BASED() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_SUITE_TYPE_REQUIREMENT_BASED));

    public static string ENUM_TYPE_TEST_SUITE_TYPE_REQUIREMENT_BASED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_SUITE_TYPE_REQUIREMENT_BASED), culture);

    public static string ENUM_TYPE_TEST_SUITE_TYPE_STATIC() => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_SUITE_TYPE_STATIC));

    public static string ENUM_TYPE_TEST_SUITE_TYPE_STATIC(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_TEST_SUITE_TYPE_STATIC), culture);

    public static string ENTITY_FIELD_NAME_TEST_CASE_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_CASE_ID));

    public static string ENTITY_FIELD_NAME_TEST_CASE_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_CASE_ID), culture);

    public static string PROJECT_SCOPE_REQUIRED() => AnalyticsResources.Get(nameof (PROJECT_SCOPE_REQUIRED));

    public static string PROJECT_SCOPE_REQUIRED(CultureInfo culture) => AnalyticsResources.Get(nameof (PROJECT_SCOPE_REQUIRED), culture);

    public static string ANALYTICS_EXTENSION_DENIED() => AnalyticsResources.Get(nameof (ANALYTICS_EXTENSION_DENIED));

    public static string ANALYTICS_EXTENSION_DENIED(CultureInfo culture) => AnalyticsResources.Get(nameof (ANALYTICS_EXTENSION_DENIED), culture);

    public static string ENTITY_SET_NAME_TEST_POINT_HISTORY_SNAPSHOT() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_POINT_HISTORY_SNAPSHOT));

    public static string ENTITY_SET_NAME_TEST_POINT_HISTORY_SNAPSHOT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TEST_POINT_HISTORY_SNAPSHOT), culture);

    public static string ENTITY_NOT_FOUND(object arg0) => AnalyticsResources.Format(nameof (ENTITY_NOT_FOUND), arg0);

    public static string ENTITY_NOT_FOUND(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (ENTITY_NOT_FOUND), culture, arg0);

    public static string WORK_ITEM_REVISION_LATENCY_SECONDS(object arg0) => AnalyticsResources.Format(nameof (WORK_ITEM_REVISION_LATENCY_SECONDS), arg0);

    public static string WORK_ITEM_REVISION_LATENCY_SECONDS(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (WORK_ITEM_REVISION_LATENCY_SECONDS), culture, arg0);

    public static string WORK_ITEM_LINK_LATENCY_SECONDS(object arg0) => AnalyticsResources.Format(nameof (WORK_ITEM_LINK_LATENCY_SECONDS), arg0);

    public static string WORK_ITEM_LINK_LATENCY_SECONDS(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (WORK_ITEM_LINK_LATENCY_SECONDS), culture, arg0);

    public static string TEST_RUN_LATENCY_SECONDS(object arg0) => AnalyticsResources.Format(nameof (TEST_RUN_LATENCY_SECONDS), arg0);

    public static string TEST_RUN_LATENCY_SECONDS(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (TEST_RUN_LATENCY_SECONDS), culture, arg0);

    public static string VIEW_DATA_NOT_FOUND() => AnalyticsResources.Get(nameof (VIEW_DATA_NOT_FOUND));

    public static string VIEW_DATA_NOT_FOUND(CultureInfo culture) => AnalyticsResources.Get(nameof (VIEW_DATA_NOT_FOUND), culture);

    public static string ENUM_TYPE_USERTYPE_UNRECOGNIZED() => AnalyticsResources.Get(nameof (ENUM_TYPE_USERTYPE_UNRECOGNIZED));

    public static string ENUM_TYPE_USERTYPE_UNRECOGNIZED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_USERTYPE_UNRECOGNIZED), culture);

    public static string ENUM_TYPE_USERTYPE_USER() => AnalyticsResources.Get(nameof (ENUM_TYPE_USERTYPE_USER));

    public static string ENUM_TYPE_USERTYPE_USER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_USERTYPE_USER), culture);

    public static string ENUM_TYPE_USERTYPE_ORGANIZATION() => AnalyticsResources.Get(nameof (ENUM_TYPE_USERTYPE_ORGANIZATION));

    public static string ENUM_TYPE_USERTYPE_ORGANIZATION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_USERTYPE_ORGANIZATION), culture);

    public static string ENUM_TYPE_USERTYPE_BOT() => AnalyticsResources.Get(nameof (ENUM_TYPE_USERTYPE_BOT));

    public static string ENUM_TYPE_USERTYPE_BOT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_USERTYPE_BOT), culture);

    public static string ENTITY_FIELD_NAME_TEST_PLAN_WORK_ITEM() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_PLAN_WORK_ITEM));

    public static string ENTITY_FIELD_NAME_TEST_PLAN_WORK_ITEM(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEST_PLAN_WORK_ITEM), culture);

    public static string ENTITY_FIELD_NAME_IS_FLAKY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_FLAKY));

    public static string ENTITY_FIELD_NAME_IS_FLAKY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_FLAKY), culture);

    public static string ENTITY_FIELD_NAME_RESULT_FLAKY_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_FLAKY_COUNT));

    public static string ENTITY_FIELD_NAME_RESULT_FLAKY_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RESULT_FLAKY_COUNT), culture);

    public static string UNABLE_TO_PARSE_HEADER(object arg0) => AnalyticsResources.Format(nameof (UNABLE_TO_PARSE_HEADER), arg0);

    public static string UNABLE_TO_PARSE_HEADER(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (UNABLE_TO_PARSE_HEADER), culture, arg0);

    public static string ENTITY_SET_NAME_PIPELINE_TASK() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PIPELINE_TASK));

    public static string ENTITY_SET_NAME_PIPELINE_TASK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PIPELINE_TASK), culture);

    public static string ENTITY_SET_NAME_PIPELINE_RUN_ACTIVITY_RESULTS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PIPELINE_RUN_ACTIVITY_RESULTS));

    public static string ENTITY_SET_NAME_PIPELINE_RUN_ACTIVITY_RESULTS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PIPELINE_RUN_ACTIVITY_RESULTS), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_OUTCOME_NONE() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_OUTCOME_NONE));

    public static string ENUM_TYPE_PIPELINE_RUN_OUTCOME_NONE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_OUTCOME_NONE), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_OUTCOME_SUCCEED() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_OUTCOME_SUCCEED));

    public static string ENUM_TYPE_PIPELINE_RUN_OUTCOME_SUCCEED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_OUTCOME_SUCCEED), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_OUTCOME_PARTIALLY_SUCCEEDED() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_OUTCOME_PARTIALLY_SUCCEEDED));

    public static string ENUM_TYPE_PIPELINE_RUN_OUTCOME_PARTIALLY_SUCCEEDED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_OUTCOME_PARTIALLY_SUCCEEDED), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_OUTCOME_FAILED() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_OUTCOME_FAILED));

    public static string ENUM_TYPE_PIPELINE_RUN_OUTCOME_FAILED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_OUTCOME_FAILED), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_OUTCOME_CANCELED() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_OUTCOME_CANCELED));

    public static string ENUM_TYPE_PIPELINE_RUN_OUTCOME_CANCELED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_OUTCOME_CANCELED), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_NONE() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_NONE));

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_NONE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_NONE), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_MANUAL() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_MANUAL));

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_MANUAL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_MANUAL), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_INDIVIDUAL_CI() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_INDIVIDUAL_CI));

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_INDIVIDUAL_CI(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_INDIVIDUAL_CI), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_BATCHED_CI() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_BATCHED_CI));

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_BATCHED_CI(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_BATCHED_CI), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_SCHEDULE() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_SCHEDULE));

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_SCHEDULE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_SCHEDULE), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_USER_CREATED() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_USER_CREATED));

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_USER_CREATED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_USER_CREATED), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_VALIDATE_SHELVESET() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_VALIDATE_SHELVESET));

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_VALIDATE_SHELVESET(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_VALIDATE_SHELVESET), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_CHECK_IN_SHELVESET() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_CHECK_IN_SHELVESET));

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_CHECK_IN_SHELVESET(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_CHECK_IN_SHELVESET), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_PULL_REQUEST() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_PULL_REQUEST));

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_PULL_REQUEST(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_PULL_REQUEST), culture);

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_BUILD_COMPLETION() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_BUILD_COMPLETION));

    public static string ENUM_TYPE_PIPELINE_RUN_REASON_BUILD_COMPLETION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_RUN_REASON_BUILD_COMPLETION), culture);

    public static string ENUM_TYPE_PIPELINE_TASK_OUTCOME_SUCCEEDED() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_TASK_OUTCOME_SUCCEEDED));

    public static string ENUM_TYPE_PIPELINE_TASK_OUTCOME_SUCCEEDED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_TASK_OUTCOME_SUCCEEDED), culture);

    public static string ENUM_TYPE_PIPELINE_TASK_OUTCOME_SUCCEEDED_WITH_ISSUES() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_TASK_OUTCOME_SUCCEEDED_WITH_ISSUES));

    public static string ENUM_TYPE_PIPELINE_TASK_OUTCOME_SUCCEEDED_WITH_ISSUES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_TASK_OUTCOME_SUCCEEDED_WITH_ISSUES), culture);

    public static string ENUM_TYPE_PIPELINE_TASK_OUTCOME_FAILED() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_TASK_OUTCOME_FAILED));

    public static string ENUM_TYPE_PIPELINE_TASK_OUTCOME_FAILED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_TASK_OUTCOME_FAILED), culture);

    public static string ENUM_TYPE_PIPELINE_TASK_OUTCOME_CANCELED() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_TASK_OUTCOME_CANCELED));

    public static string ENUM_TYPE_PIPELINE_TASK_OUTCOME_CANCELED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_TASK_OUTCOME_CANCELED), culture);

    public static string ENUM_TYPE_PIPELINE_TASK_OUTCOME_SKIPPED() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_TASK_OUTCOME_SKIPPED));

    public static string ENUM_TYPE_PIPELINE_TASK_OUTCOME_SKIPPED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_TASK_OUTCOME_SKIPPED), culture);

    public static string ENUM_TYPE_PIPELINE_TASK_OUTCOME_ABANDONED() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_TASK_OUTCOME_ABANDONED));

    public static string ENUM_TYPE_PIPELINE_TASK_OUTCOME_ABANDONED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_TASK_OUTCOME_ABANDONED), culture);

    public static string ENUM_TYPE_PIPELINE_PROCESS_TYPE_DESIGNER() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_PROCESS_TYPE_DESIGNER));

    public static string ENUM_TYPE_PIPELINE_PROCESS_TYPE_DESIGNER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_PROCESS_TYPE_DESIGNER), culture);

    public static string ENTITY_FIELD_NAME_JOB_IDENTIFIER() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_JOB_IDENTIFIER));

    public static string ENTITY_FIELD_NAME_JOB_IDENTIFIER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_JOB_IDENTIFIER), culture);

    public static string ENUM_TYPE_PIPELINE_ACTIVITY_TYPE_TASK() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_ACTIVITY_TYPE_TASK));

    public static string ENUM_TYPE_PIPELINE_ACTIVITY_TYPE_TASK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_ACTIVITY_TYPE_TASK), culture);

    public static string ENUM_TYPE_PIPELINE_ACTIVITY_TYP_AGENT_WAIT() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_ACTIVITY_TYP_AGENT_WAIT));

    public static string ENUM_TYPE_PIPELINE_ACTIVITY_TYP_AGENT_WAIT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_ACTIVITY_TYP_AGENT_WAIT), culture);

    public static string ENUM_TYPE_PIPELINE_ACTIVITY_TYP_APPROVAL() => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_ACTIVITY_TYP_APPROVAL));

    public static string ENUM_TYPE_PIPELINE_ACTIVITY_TYP_APPROVAL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PIPELINE_ACTIVITY_TYP_APPROVAL), culture);

    public static string ENTITY_FIELD_NAME_RELATIVE_START_TIME_JOB_SECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELATIVE_START_TIME_JOB_SECONDS));

    public static string ENTITY_FIELD_NAME_RELATIVE_START_TIME_JOB_SECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELATIVE_START_TIME_JOB_SECONDS), culture);

    public static string ENTITY_FIELD_NAME_RELATIVE_START_TIME_RUN_SECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELATIVE_START_TIME_RUN_SECONDS));

    public static string ENTITY_FIELD_NAME_RELATIVE_START_TIME_RUN_SECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELATIVE_START_TIME_RUN_SECONDS), culture);

    public static string ENTITY_FIELD_NAME_RELATIVE_START_TIME_STAGE_SECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELATIVE_START_TIME_STAGE_SECONDS));

    public static string ENTITY_FIELD_NAME_RELATIVE_START_TIME_STAGE_SECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_RELATIVE_START_TIME_STAGE_SECONDS), culture);

    public static string ENTITY_FIELD_NAME_COMMIT_TO_ENVIRONMENT_DURATION_SECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMIT_TO_ENVIRONMENT_DURATION_SECONDS));

    public static string ENTITY_FIELD_NAME_COMMIT_TO_ENVIRONMENT_DURATION_SECONDS(
      CultureInfo culture)
    {
      return AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMIT_TO_ENVIRONMENT_DURATION_SECONDS), culture);
    }

    public static string ENTITY_FIELD_NAME_COMMIT_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMIT_DATE));

    public static string ENTITY_FIELD_NAME_COMMIT_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMIT_DATE), culture);

    public static string ENTITY_FIELD_NAME_JOB_STARTED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_JOB_STARTED_DATE));

    public static string ENTITY_FIELD_NAME_JOB_STARTED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_JOB_STARTED_DATE), culture);

    public static string ENTITY_FIELD_NAME_JOB_END_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_JOB_END_DATE));

    public static string ENTITY_FIELD_NAME_JOB_END_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_JOB_END_DATE), culture);

    public static string ENTITY_FIELD_NAME_ENVIRONMENT_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ENVIRONMENT_NAME));

    public static string ENTITY_FIELD_NAME_ENVIRONMENT_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ENVIRONMENT_NAME), culture);

    public static string ENTITY_FIELD_NAME_ENVIRONMENT_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ENVIRONMENT_ID));

    public static string ENTITY_FIELD_NAME_ENVIRONMENT_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ENVIRONMENT_ID), culture);

    public static string ENTITY_SET_NAME_COMMIT_TO_DEPLOYMENT() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_COMMIT_TO_DEPLOYMENT));

    public static string ENTITY_SET_NAME_COMMIT_TO_DEPLOYMENT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_COMMIT_TO_DEPLOYMENT), culture);

    public static string ENTITY_SET_NAME_PIPELINE_ENVIRONMENT() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PIPELINE_ENVIRONMENT));

    public static string ENTITY_SET_NAME_PIPELINE_ENVIRONMENT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_PIPELINE_ENVIRONMENT), culture);

    public static string ENTITY_SET_NAME_TASKAGENTPOOLSIZESNAPSHOT() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TASKAGENTPOOLSIZESNAPSHOT));

    public static string ENTITY_SET_NAME_TASKAGENTPOOLSIZESNAPSHOT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TASKAGENTPOOLSIZESNAPSHOT), culture);

    public static string ENTITY_FIELD_NAME_STAGE_IDENTIFIER() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STAGE_IDENTIFIER));

    public static string ENTITY_FIELD_NAME_STAGE_IDENTIFIER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_STAGE_IDENTIFIER), culture);

    public static string ENTITY_FIELD_NAME_ACCOUNT_COMPANY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_COMPANY));

    public static string ENTITY_FIELD_NAME_ACCOUNT_COMPANY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_COMPANY), culture);

    public static string ENTITY_FIELD_NAME_ACCOUNT_CREATIONDATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_CREATIONDATE));

    public static string ENTITY_FIELD_NAME_ACCOUNT_CREATIONDATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_CREATIONDATE), culture);

    public static string ENTITY_FIELD_NAME_ACCOUNT_DESCRIPTION() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_DESCRIPTION));

    public static string ENTITY_FIELD_NAME_ACCOUNT_DESCRIPTION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_DESCRIPTION), culture);

    public static string ENTITY_FIELD_NAME_ACCOUNT_EMAIL() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_EMAIL));

    public static string ENTITY_FIELD_NAME_ACCOUNT_EMAIL(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_EMAIL), culture);

    public static string ENTITY_FIELD_NAME_ACCOUNT_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_ID));

    public static string ENTITY_FIELD_NAME_ACCOUNT_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_ID), culture);

    public static string ENTITY_FIELD_NAME_ACCOUNT_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_NAME));

    public static string ENTITY_FIELD_NAME_ACCOUNT_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_NAME), culture);

    public static string ENTITY_FIELD_NAME_ACCOUNT_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_TYPE));

    public static string ENTITY_FIELD_NAME_ACCOUNT_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ACCOUNT_TYPE), culture);

    public static string ENTITY_SET_NAME_GITHUB_ACCOUNTS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUB_ACCOUNTS));

    public static string ENTITY_SET_NAME_GITHUB_ACCOUNTS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUB_ACCOUNTS), culture);

    public static string ENTITY_SET_NAME_GITHUB_COMMITS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUB_COMMITS));

    public static string ENTITY_SET_NAME_GITHUB_COMMITS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUB_COMMITS), culture);

    public static string ENTITY_SET_NAME_GITHUB_REPOSITORIES() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUB_REPOSITORIES));

    public static string ENTITY_SET_NAME_GITHUB_REPOSITORIES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUB_REPOSITORIES), culture);

    public static string ENTITY_SET_NAME_GITHUB_TEAMS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUB_TEAMS));

    public static string ENTITY_SET_NAME_GITHUB_TEAMS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUB_TEAMS), culture);

    public static string ENTITY_SET_NAME_GITHUB_USERS() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUB_USERS));

    public static string ENTITY_SET_NAME_GITHUB_USERS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUB_USERS), culture);

    public static string ENTITY_FIELD_NAME_REPOSITORY_CREATIONDATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY_CREATIONDATE));

    public static string ENTITY_FIELD_NAME_REPOSITORY_CREATIONDATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY_CREATIONDATE), culture);

    public static string ENTITY_FIELD_NAME_REPOSITORY_DESCRIPTION() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY_DESCRIPTION));

    public static string ENTITY_FIELD_NAME_REPOSITORY_DESCRIPTION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY_DESCRIPTION), culture);

    public static string ENTITY_FIELD_NAME_REPOSITORY_NAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY_NAME));

    public static string ENTITY_FIELD_NAME_REPOSITORY_NAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY_NAME), culture);

    public static string ENTITY_FIELD_NAME_TEAM_GHI_DESCRIPTION() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEAM_GHI_DESCRIPTION));

    public static string ENTITY_FIELD_NAME_TEAM_GHI_DESCRIPTION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEAM_GHI_DESCRIPTION), culture);

    public static string ENTITY_FIELD_NAME_TEAM_GHI_NUM_MEMBERS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEAM_GHI_NUM_MEMBERS));

    public static string ENTITY_FIELD_NAME_TEAM_GHI_NUM_MEMBERS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEAM_GHI_NUM_MEMBERS), culture);

    public static string ENTITY_FIELD_NAME_TEAM_GHI_PARENT_TEAMSK() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEAM_GHI_PARENT_TEAMSK));

    public static string ENTITY_FIELD_NAME_TEAM_GHI_PARENT_TEAMSK(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEAM_GHI_PARENT_TEAMSK), culture);

    public static string ENTITY_FIELD_NAME_USER_GHI_CREATIONDATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_GHI_CREATIONDATE));

    public static string ENTITY_FIELD_NAME_USER_GHI_CREATIONDATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_GHI_CREATIONDATE), culture);

    public static string ENTITY_FIELD_NAME_USER_GHI_LOGIN() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_GHI_LOGIN));

    public static string ENTITY_FIELD_NAME_USER_GHI_LOGIN(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_GHI_LOGIN), culture);

    public static string ENTITY_FIELD_NAME_USER_GHI_UNIQUENAME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_GHI_UNIQUENAME));

    public static string ENTITY_FIELD_NAME_USER_GHI_UNIQUENAME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_GHI_UNIQUENAME), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_SHA() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_SHA));

    public static string ENTITY_FIELD_NAME_COMMITS_SHA(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_SHA), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_INPULLREQUEST() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_INPULLREQUEST));

    public static string ENTITY_FIELD_NAME_COMMITS_INPULLREQUEST(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_INPULLREQUEST), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_ISMERGECOMMIT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_ISMERGECOMMIT));

    public static string ENTITY_FIELD_NAME_COMMITS_ISMERGECOMMIT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_ISMERGECOMMIT), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_LINES_ADDED_NEW() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_LINES_ADDED_NEW));

    public static string ENTITY_FIELD_NAME_COMMITS_LINES_ADDED_NEW(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_LINES_ADDED_NEW), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_LINES_CHURNEDBYOTHERS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_LINES_CHURNEDBYOTHERS));

    public static string ENTITY_FIELD_NAME_COMMITS_LINES_CHURNEDBYOTHERS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_LINES_CHURNEDBYOTHERS), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_LINES_REFACTORED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_LINES_REFACTORED));

    public static string ENTITY_FIELD_NAME_COMMITS_LINES_REFACTORED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_LINES_REFACTORED), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_LINES_SELFCHURNED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_LINES_SELFCHURNED));

    public static string ENTITY_FIELD_NAME_COMMITS_LINES_SELFCHURNED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_LINES_SELFCHURNED), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_LINES_ADDED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_LINES_ADDED));

    public static string ENTITY_FIELD_NAME_COMMITS_LINES_ADDED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_LINES_ADDED), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_LINES_DELETED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_LINES_DELETED));

    public static string ENTITY_FIELD_NAME_COMMITS_LINES_DELETED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_LINES_DELETED), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_ACCOUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_ACCOUNT));

    public static string ENTITY_FIELD_NAME_COMMITS_ACCOUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_ACCOUNT), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_AUTHOR() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_AUTHOR));

    public static string ENTITY_FIELD_NAME_COMMITS_AUTHOR(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_AUTHOR), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_COMMITER() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_COMMITER));

    public static string ENTITY_FIELD_NAME_COMMITS_COMMITER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_COMMITER), culture);

    public static string ENTITY_FIELD_NAME_COMMITS_REPOSITORY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_REPOSITORY));

    public static string ENTITY_FIELD_NAME_COMMITS_REPOSITORY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMITS_REPOSITORY), culture);

    public static string ENTITY_FIELD_NAME_REPOSITORY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY));

    public static string ENTITY_FIELD_NAME_REPOSITORY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REPOSITORY), culture);

    public static string ENTITY_SET_NAME_REPOSITORIES_GHI() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_REPOSITORIES_GHI));

    public static string ENTITY_SET_NAME_REPOSITORIES_GHI(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_REPOSITORIES_GHI), culture);

    public static string ENUM_TYPE_ACCOUNT_TYPE_ORGANIZATION() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACCOUNT_TYPE_ORGANIZATION));

    public static string ENUM_TYPE_ACCOUNT_TYPE_ORGANIZATION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACCOUNT_TYPE_ORGANIZATION), culture);

    public static string ENUM_TYPE_ACCOUNT_TYPE_USER() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACCOUNT_TYPE_USER));

    public static string ENUM_TYPE_ACCOUNT_TYPE_USER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACCOUNT_TYPE_USER), culture);

    public static string ENTITY_FIELD_NAME_TEAM_GHI_PARENT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEAM_GHI_PARENT));

    public static string ENTITY_FIELD_NAME_TEAM_GHI_PARENT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TEAM_GHI_PARENT), culture);

    public static string ENTITY_FIELD_NAME_USER_COMPANY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_COMPANY));

    public static string ENTITY_FIELD_NAME_USER_COMPANY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_USER_COMPANY), culture);

    public static string ENTITY_FIELD_NAME_PULLREQUEST_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PULLREQUEST_ID));

    public static string ENTITY_FIELD_NAME_PULLREQUEST_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PULLREQUEST_ID), culture);

    public static string ENTITY_FIELD_NAME_MERGED_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_MERGED_DATE));

    public static string ENTITY_FIELD_NAME_MERGED_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_MERGED_DATE), culture);

    public static string ENTITY_FIELD_NUMBER() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NUMBER));

    public static string ENTITY_FIELD_NUMBER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NUMBER), culture);

    public static string ENTITY_FIELD_NAME_LINES_ADDED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINES_ADDED));

    public static string ENTITY_FIELD_NAME_LINES_ADDED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINES_ADDED), culture);

    public static string ENTITY_FIELD_NAME_LINES_DELETED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINES_DELETED));

    public static string ENTITY_FIELD_NAME_LINES_DELETED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINES_DELETED), culture);

    public static string ENTITY_FIELD_NAME_SOURCE_BRANCH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SOURCE_BRANCH));

    public static string ENTITY_FIELD_NAME_SOURCE_BRANCH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SOURCE_BRANCH), culture);

    public static string ENTITY_FIELD_NAME_TARGET_BRANCH() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TARGET_BRANCH));

    public static string ENTITY_FIELD_NAME_TARGET_BRANCH(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TARGET_BRANCH), culture);

    public static string ENTITY_FIELD_NAME_AUTHOR() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AUTHOR));

    public static string ENTITY_FIELD_NAME_AUTHOR(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_AUTHOR), culture);

    public static string ENTITY_FIELD_NAME_MERGEDBY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_MERGEDBY));

    public static string ENTITY_FIELD_NAME_MERGEDBY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_MERGEDBY), culture);

    public static string ENTITY_FIELD_NAME_ISDRAFT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ISDRAFT));

    public static string ENTITY_FIELD_NAME_ISDRAFT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ISDRAFT), culture);

    public static string ENTITY_FIELD_NAME_COMMENTS_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMENTS_COUNT));

    public static string ENTITY_FIELD_NAME_COMMENTS_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMENTS_COUNT), culture);

    public static string ENTITY_FIELD_NAME_REVIEWERS_REQUEST_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWERS_REQUEST_COUNT));

    public static string ENTITY_FIELD_NAME_REVIEWERS_REQUEST_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWERS_REQUEST_COUNT), culture);

    public static string ENTITY_FIELD_NAME_REVIEWERS_VOTE_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWERS_VOTE_COUNT));

    public static string ENTITY_FIELD_NAME_REVIEWERS_VOTE_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWERS_VOTE_COUNT), culture);

    public static string ENTITY_FIELD_NAME_ASSIGNEES_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ASSIGNEES_COUNT));

    public static string ENTITY_FIELD_NAME_ASSIGNEES_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ASSIGNEES_COUNT), culture);

    public static string ENTITY_FIELD_NAME_COMMIT_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMIT_COUNT));

    public static string ENTITY_FIELD_NAME_COMMIT_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_COMMIT_COUNT), culture);

    public static string ENTITY_FIELD_NAME_LINES_COVERED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINES_COVERED));

    public static string ENTITY_FIELD_NAME_LINES_COVERED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_LINES_COVERED), culture);

    public static string ENTITY_FIELD_NAME_TIMETOOPEN_INSECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TIMETOOPEN_INSECONDS));

    public static string ENTITY_FIELD_NAME_TIMETOOPEN_INSECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TIMETOOPEN_INSECONDS), culture);

    public static string ENTITY_FIELD_NAME_TIMETOMERGE_INSECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TIMETOMERGE_INSECONDS));

    public static string ENTITY_FIELD_NAME_TIMETOMERGE_INSECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TIMETOMERGE_INSECONDS), culture);

    public static string ENTITY_FIELD_NAME_TIMETOCLOSE_INSECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TIMETOCLOSE_INSECONDS));

    public static string ENTITY_FIELD_NAME_TIMETOCLOSE_INSECONDS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TIMETOCLOSE_INSECONDS), culture);

    public static string ENTITY_FIELD_NAME_BRANCH_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BRANCH_ID));

    public static string ENTITY_FIELD_NAME_BRANCH_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BRANCH_ID), culture);

    public static string ENTITY_FIELD_NAME_BRANCH_CREATEDDATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BRANCH_CREATEDDATE));

    public static string ENTITY_FIELD_NAME_BRANCH_CREATEDDATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_BRANCH_CREATEDDATE), culture);

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ACCOUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ACCOUNT));

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ACCOUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ACCOUNT), culture);

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ACTIVITY_BY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ACTIVITY_BY));

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ACTIVITY_BY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ACTIVITY_BY), culture);

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ARTIFACT_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ARTIFACT_TYPE));

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ARTIFACT_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ARTIFACT_TYPE), culture);

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ARTIFACT_IDENTIFIER() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ARTIFACT_IDENTIFIER));

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ARTIFACT_IDENTIFIER(
      CultureInfo culture)
    {
      return AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_ARTIFACT_IDENTIFIER), culture);
    }

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_DATE));

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_DATE), culture);

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_REPOSITORY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_REPOSITORY));

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_REPOSITORY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_REPOSITORY), culture);

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_TYPE));

    public static string ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_CONTRIBUTOR_ACTIVITY_TYPE), culture);

    public static string ENTITY_SET_NAME_GITHUB_CONTRIBUTOR_ACTIVITIES() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUB_CONTRIBUTOR_ACTIVITIES));

    public static string ENTITY_SET_NAME_GITHUB_CONTRIBUTOR_ACTIVITIES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_GITHUB_CONTRIBUTOR_ACTIVITIES), culture);

    public static string ENUM_TYPE_ACTIVITY_ARTIFACT_TYPE_COMMIT() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_ARTIFACT_TYPE_COMMIT));

    public static string ENUM_TYPE_ACTIVITY_ARTIFACT_TYPE_COMMIT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_ARTIFACT_TYPE_COMMIT), culture);

    public static string ENUM_TYPE_ACTIVITY_ARTIFACT_TYPE_PULL_REQUEST() => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_ARTIFACT_TYPE_PULL_REQUEST));

    public static string ENUM_TYPE_ACTIVITY_ARTIFACT_TYPE_PULL_REQUEST(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_ACTIVITY_ARTIFACT_TYPE_PULL_REQUEST), culture);

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_CLOSED() => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_CLOSED));

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_CLOSED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_CLOSED), culture);

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_COMMENTED() => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_COMMENTED));

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_COMMENTED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_COMMENTED), culture);

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_CREATED() => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_CREATED));

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_CREATED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_CREATED), culture);

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_MERGED() => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_MERGED));

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_MERGED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_MERGED), culture);

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REOPEN() => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REOPEN));

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REOPEN(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REOPEN), culture);

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEWED() => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEWED));

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEWED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEWED), culture);

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEW_REQUEST() => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEW_REQUEST));

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEW_REQUEST(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEW_REQUEST), culture);

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEW_REQUEST_REMOVED() => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEW_REQUEST_REMOVED));

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEW_REQUEST_REMOVED(
      CultureInfo culture)
    {
      return AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_REVIEW_REQUEST_REMOVED), culture);
    }

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_INACTIVE() => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_INACTIVE));

    public static string ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_INACTIVE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_CONTRIBUTOR_ACTIVITY_TYPE_INACTIVE), culture);

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_PULLREQUEST_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_PULLREQUEST_ID));

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_PULLREQUEST_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_PULLREQUEST_ID), culture);

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_ACCOUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_ACCOUNT));

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_ACCOUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_ACCOUNT), culture);

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REPOSITORY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REPOSITORY));

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REPOSITORY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REPOSITORY), culture);

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_DATE));

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_DATE), culture);

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_ACTIVITY_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_ACTIVITY_TYPE));

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_ACTIVITY_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_ACTIVITY_TYPE), culture);

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_ACTIVITY_BY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_ACTIVITY_BY));

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_ACTIVITY_BY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_ACTIVITY_BY), culture);

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEW_VOTE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEW_VOTE));

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEW_VOTE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEW_VOTE), culture);

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEWER_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEWER_TYPE));

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEWER_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEWER_TYPE), culture);

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEWING_USER() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEWING_USER));

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEWING_USER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEWING_USER), culture);

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEWING_TEAM() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEWING_TEAM));

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEWING_TEAM(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_REVIEWING_TEAM), culture);

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_IS_CURRENT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_IS_CURRENT));

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_IS_CURRENT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_IS_CURRENT), culture);

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_TURNAROUND_TIME_IN_SECONDS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_TURNAROUND_TIME_IN_SECONDS));

    public static string ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_TURNAROUND_TIME_IN_SECONDS(
      CultureInfo culture)
    {
      return AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REVIEWER_ACTIVITY_TURNAROUND_TIME_IN_SECONDS), culture);
    }

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_ACTIVITY_TYPE_REVIEW_REQUEST() => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_ACTIVITY_TYPE_REVIEW_REQUEST));

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_ACTIVITY_TYPE_REVIEW_REQUEST(
      CultureInfo culture)
    {
      return AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_ACTIVITY_TYPE_REVIEW_REQUEST), culture);
    }

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_ACTIVITY_TYPE_REVIEW_REQUEST_REMOVED() => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_ACTIVITY_TYPE_REVIEW_REQUEST_REMOVED));

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_ACTIVITY_TYPE_REVIEW_REQUEST_REMOVED(
      CultureInfo culture)
    {
      return AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_ACTIVITY_TYPE_REVIEW_REQUEST_REMOVED), culture);
    }

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_ACTIVITY_TYPE_REVIEWED() => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_ACTIVITY_TYPE_REVIEWED));

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_ACTIVITY_TYPE_REVIEWED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_ACTIVITY_TYPE_REVIEWED), culture);

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_PENDING() => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_PENDING));

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_PENDING(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_PENDING), culture);

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_COMMENT() => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_COMMENT));

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_COMMENT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_COMMENT), culture);

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_COMMENT_REQUEST_CHANGES() => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_COMMENT_REQUEST_CHANGES));

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_COMMENT_REQUEST_CHANGES(
      CultureInfo culture)
    {
      return AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_COMMENT_REQUEST_CHANGES), culture);
    }

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_APPROVE() => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_APPROVE));

    public static string ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_APPROVE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEW_VOTE_TYPE_APPROVE), culture);

    public static string ENUM_TYPE_PULLREQUEST_REVIEWER_TYPE_USER() => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEWER_TYPE_USER));

    public static string ENUM_TYPE_PULLREQUEST_REVIEWER_TYPE_USER(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEWER_TYPE_USER), culture);

    public static string ENUM_TYPE_PULLREQUEST_REVIEWER_TYPE_TEAM() => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEWER_TYPE_TEAM));

    public static string ENUM_TYPE_PULLREQUEST_REVIEWER_TYPE_TEAM(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PULLREQUEST_REVIEWER_TYPE_TEAM), culture);

    public static string ENTITY_FIELD_NAME_PROJECT_VISIBILITY() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PROJECT_VISIBILITY));

    public static string ENTITY_FIELD_NAME_PROJECT_VISIBILITY(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PROJECT_VISIBILITY), culture);

    public static string ENUM_TYPE_PROJECT_VISIBILITY_ORGANIZATION() => AnalyticsResources.Get(nameof (ENUM_TYPE_PROJECT_VISIBILITY_ORGANIZATION));

    public static string ENUM_TYPE_PROJECT_VISIBILITY_ORGANIZATION(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PROJECT_VISIBILITY_ORGANIZATION), culture);

    public static string ENUM_TYPE_PROJECT_VISIBILITY_PRIVATE() => AnalyticsResources.Get(nameof (ENUM_TYPE_PROJECT_VISIBILITY_PRIVATE));

    public static string ENUM_TYPE_PROJECT_VISIBILITY_PRIVATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PROJECT_VISIBILITY_PRIVATE), culture);

    public static string ENUM_TYPE_PROJECT_VISIBILITY_PUBLIC() => AnalyticsResources.Get(nameof (ENUM_TYPE_PROJECT_VISIBILITY_PUBLIC));

    public static string ENUM_TYPE_PROJECT_VISIBILITY_PUBLIC(CultureInfo culture) => AnalyticsResources.Get(nameof (ENUM_TYPE_PROJECT_VISIBILITY_PUBLIC), culture);

    public static string ENTITY_FIELD_NAME_PARALLELISM_TAG() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PARALLELISM_TAG));

    public static string ENTITY_FIELD_NAME_PARALLELISM_TAG(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PARALLELISM_TAG), culture);

    public static string ENTITY_FIELD_NAME_IS_HOSTED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_HOSTED));

    public static string ENTITY_FIELD_NAME_IS_HOSTED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_HOSTED), culture);

    public static string ENTITY_FIELD_NAME_IS_RUNNING() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_RUNNING));

    public static string ENTITY_FIELD_NAME_IS_RUNNING(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_RUNNING), culture);

    public static string ENTITY_FIELD_NAME_IS_QUEUED() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_QUEUED));

    public static string ENTITY_FIELD_NAME_IS_QUEUED(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_QUEUED), culture);

    public static string ENTITY_FIELD_NAME_TOTAL_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TOTAL_COUNT));

    public static string ENTITY_FIELD_NAME_TOTAL_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TOTAL_COUNT), culture);

    public static string ENTITY_FIELD_NAME_TOTAL_MINUTES() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TOTAL_MINUTES));

    public static string ENTITY_FIELD_NAME_TOTAL_MINUTES(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_TOTAL_MINUTES), culture);

    public static string ENTITY_FIELD_NAME_IS_PREMIUM() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_PREMIUM));

    public static string ENTITY_FIELD_NAME_IS_PREMIUM(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_IS_PREMIUM), culture);

    public static string ENTITY_FIELD_NAME_FAILED_TO_REACH_ALL_PROVIDERS() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FAILED_TO_REACH_ALL_PROVIDERS));

    public static string ENTITY_FIELD_NAME_FAILED_TO_REACH_ALL_PROVIDERS(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_FAILED_TO_REACH_ALL_PROVIDERS), culture);

    public static string ENTITY_SET_NAME_TASKAGENTREQUESTSNAPSHOT() => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TASKAGENTREQUESTSNAPSHOT));

    public static string ENTITY_SET_NAME_TASKAGENTREQUESTSNAPSHOT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_SET_NAME_TASKAGENTREQUESTSNAPSHOT), culture);

    public static string ENTITY_FIELD_NAME_SAMPLING_DATE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SAMPLING_DATE));

    public static string ENTITY_FIELD_NAME_SAMPLING_DATE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SAMPLING_DATE), culture);

    public static string ENTITY_FIELD_NAME_SAMPLING_HOUR() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SAMPLING_HOUR));

    public static string ENTITY_FIELD_NAME_SAMPLING_HOUR(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SAMPLING_HOUR), culture);

    public static string ENTITY_FIELD_NAME_SAMPLING_TIME() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SAMPLING_TIME));

    public static string ENTITY_FIELD_NAME_SAMPLING_TIME(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_SAMPLING_TIME), culture);

    public static string ENTITY_FIELD_NAME_PIPELINE_TYPE() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_TYPE));

    public static string ENTITY_FIELD_NAME_PIPELINE_TYPE(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_PIPELINE_TYPE), culture);

    public static string ENTITY_FIELD_NAME_ONLINE_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ONLINE_COUNT));

    public static string ENTITY_FIELD_NAME_ONLINE_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_ONLINE_COUNT), culture);

    public static string ENTITY_FIELD_NAME_OFFLINE_COUNT() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_OFFLINE_COUNT));

    public static string ENTITY_FIELD_NAME_OFFLINE_COUNT(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_OFFLINE_COUNT), culture);

    public static string ENTITY_FIELD_NAME_POOL_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_POOL_ID));

    public static string ENTITY_FIELD_NAME_POOL_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_POOL_ID), culture);

    public static string ENTITY_FIELD_NAME_REQUEST_ID() => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUEST_ID));

    public static string ENTITY_FIELD_NAME_REQUEST_ID(CultureInfo culture) => AnalyticsResources.Get(nameof (ENTITY_FIELD_NAME_REQUEST_ID), culture);

    public static string ODATA_SELECT_NOT_VALID() => AnalyticsResources.Get(nameof (ODATA_SELECT_NOT_VALID));

    public static string ODATA_SELECT_NOT_VALID(CultureInfo culture) => AnalyticsResources.Get(nameof (ODATA_SELECT_NOT_VALID), culture);
  }
}
