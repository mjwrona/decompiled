// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.ServerResources
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Boards.CssNodes
{
  internal static class ServerResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ServerResources), typeof (ServerResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ServerResources.s_resMgr;

    private static string Get(string resourceName) => ServerResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ServerResources.Get(resourceName) : ServerResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ServerResources.GetInt(resourceName) : (int) ServerResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ServerResources.GetBool(resourceName) : (bool) ServerResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ServerResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ServerResources.Get(resourceName, culture);
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

    public static string GSS_ACTIVE_DIRECTORY_AUTHENTICATION_EXCEPTION(object arg0, object arg1) => ServerResources.Format("GSS.ACTIVE_DIRECTORY_AUTHENTICATION_EXCEPTION", arg0, arg1);

    public static string GSS_ACTIVE_DIRECTORY_AUTHENTICATION_EXCEPTION(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format("GSS.ACTIVE_DIRECTORY_AUTHENTICATION_EXCEPTION", culture, arg0, arg1);
    }

    public static string GSS_ACTIVE_DIRECTORY_NO_IDENTITY_EXCEPTION() => ServerResources.Get("GSS.ACTIVE_DIRECTORY_NO_IDENTITY_EXCEPTION");

    public static string GSS_ACTIVE_DIRECTORY_NO_IDENTITY_EXCEPTION(CultureInfo culture) => ServerResources.Get("GSS.ACTIVE_DIRECTORY_NO_IDENTITY_EXCEPTION", culture);

    public static string GSS_ACTIVE_DIRECTORY_EXTRACT_SID_EXCEPTION() => ServerResources.Get("GSS.ACTIVE_DIRECTORY_EXTRACT_SID_EXCEPTION");

    public static string GSS_ACTIVE_DIRECTORY_EXTRACT_SID_EXCEPTION(CultureInfo culture) => ServerResources.Get("GSS.ACTIVE_DIRECTORY_EXTRACT_SID_EXCEPTION", culture);

    public static string GSS_READ_IDENTITY_EXCEPTION() => ServerResources.Get("GSS.READ_IDENTITY_EXCEPTION");

    public static string GSS_READ_IDENTITY_EXCEPTION(CultureInfo culture) => ServerResources.Get("GSS.READ_IDENTITY_EXCEPTION", culture);

    public static string GSS_LOOKUP_SID_EXCEPTION(object arg0) => ServerResources.Format("GSS.LOOKUP_SID_EXCEPTION", arg0);

    public static string GSS_LOOKUP_SID_EXCEPTION(object arg0, CultureInfo culture) => ServerResources.Format("GSS.LOOKUP_SID_EXCEPTION", culture, arg0);

    public static string GSS_APP_CACHE_MISS_UPDATE(object arg0) => ServerResources.Format("GSS.APP-CACHE_MISS_UPDATE", arg0);

    public static string GSS_APP_CACHE_MISS_UPDATE(object arg0, CultureInfo culture) => ServerResources.Format("GSS.APP-CACHE_MISS_UPDATE", culture, arg0);

    public static string GSS_APP_GROUP_NOT_FOUND() => ServerResources.Get("GSS.APP-GROUP-NOT-FOUND");

    public static string GSS_APP_GROUP_NOT_FOUND(CultureInfo culture) => ServerResources.Get("GSS.APP-GROUP-NOT-FOUND", culture);

    public static string GSS_BAD_CLASSID_ACTIONID_PAIR_EXCEPTION(object arg0, object arg1) => ServerResources.Format("GSS.BAD-CLASSID-ACTIONID-PAIR-EXCEPTION", arg0, arg1);

    public static string GSS_BAD_CLASSID_ACTIONID_PAIR_EXCEPTION(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format("GSS.BAD-CLASSID-ACTIONID-PAIR-EXCEPTION", culture, arg0, arg1);
    }

    public static string GSS_BADPARENTOBJECTCLASSIDERROR(object arg0, object arg1) => ServerResources.Format("GSS.BADPARENTOBJECTCLASSIDERROR", arg0, arg1);

    public static string GSS_BADPARENTOBJECTCLASSIDERROR(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format("GSS.BADPARENTOBJECTCLASSIDERROR", culture, arg0, arg1);
    }

    public static string GSS_BUILT_IN_GROUP() => ServerResources.Get("GSS.BUILT-IN-GROUP");

    public static string GSS_BUILT_IN_GROUP(CultureInfo culture) => ServerResources.Get("GSS.BUILT-IN-GROUP", culture);

    public static string GSS_CACHE_UPDATE_FAILED(object arg0, object arg1, object arg2) => ServerResources.Format("GSS.CACHE_UPDATE_FAILED", arg0, arg1, arg2);

    public static string GSS_CACHE_UPDATE_FAILED(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format("GSS.CACHE_UPDATE_FAILED", culture, arg0, arg1, arg2);
    }

    public static string GSS_CAN_MANAGE_PROJECT() => ServerResources.Get("GSS.CAN-MANAGE-PROJECT");

    public static string GSS_CAN_MANAGE_PROJECT(CultureInfo culture) => ServerResources.Get("GSS.CAN-MANAGE-PROJECT", culture);

    public static string GSS_CANNOT_BIND_TO_APPGROUP() => ServerResources.Get("GSS.CANNOT-BIND-TO-APPGROUP");

    public static string GSS_CANNOT_BIND_TO_APPGROUP(CultureInfo culture) => ServerResources.Get("GSS.CANNOT-BIND-TO-APPGROUP", culture);

    public static string GSS_CANNOT_GET_CALLER_SID() => ServerResources.Get("GSS.CANNOT-GET-CALLER-SID");

    public static string GSS_CANNOT_GET_CALLER_SID(CultureInfo culture) => ServerResources.Get("GSS.CANNOT-GET-CALLER-SID", culture);

    public static string GSS_CANNOT_REMOVE_LAST_MEMBER() => ServerResources.Get("GSS.CANNOT-REMOVE-LAST-MEMBER");

    public static string GSS_CANNOT_REMOVE_LAST_MEMBER(CultureInfo culture) => ServerResources.Get("GSS.CANNOT-REMOVE-LAST-MEMBER", culture);

    public static string GSS_CLASSIDDOESNOTEXISTERROR(object arg0) => ServerResources.Format("GSS.CLASSIDDOESNOTEXISTERROR", arg0);

    public static string GSS_CLASSIDDOESNOTEXISTERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.CLASSIDDOESNOTEXISTERROR", culture, arg0);

    public static string GSS_SECURITYOBJECTDOESNOTEXISTERROR(object arg0) => ServerResources.Format("GSS.SECURITYOBJECTDOESNOTEXISTERROR", arg0);

    public static string GSS_SECURITYOBJECTDOESNOTEXISTERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.SECURITYOBJECTDOESNOTEXISTERROR", culture, arg0);

    public static string GSS_SECURITYACTIONDOESNOTEXISTERROR(object arg0) => ServerResources.Format("GSS.SECURITYACTIONDOESNOTEXISTERROR", arg0);

    public static string GSS_SECURITYACTIONDOESNOTEXISTERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.SECURITYACTIONDOESNOTEXISTERROR", culture, arg0);

    public static string GSS_DELETEACEERROR(object arg0, object arg1, object arg2, object arg3) => ServerResources.Format("GSS.DELETEACEERROR", arg0, arg1, arg2, arg3);

    public static string GSS_DELETEACEERROR(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format("GSS.DELETEACEERROR", culture, arg0, arg1, arg2, arg3);
    }

    public static string GSS_CREATEACENOOBJECTERROR(object arg0) => ServerResources.Format("GSS.CREATEACENOOBJECTERROR", arg0);

    public static string GSS_CREATEACENOOBJECTERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.CREATEACENOOBJECTERROR", culture, arg0);

    public static string GSS_CREATEACENOACTIONERROR() => ServerResources.Get("GSS.CREATEACENOACTIONERROR");

    public static string GSS_CREATEACENOACTIONERROR(CultureInfo culture) => ServerResources.Get("GSS.CREATEACENOACTIONERROR", culture);

    public static string GSS_CYCLIC_MEMBERSHIP() => ServerResources.Get("GSS.CYCLIC-MEMBERSHIP");

    public static string GSS_CYCLIC_MEMBERSHIP(CultureInfo culture) => ServerResources.Get("GSS.CYCLIC-MEMBERSHIP", culture);

    public static string GSS_DATABASE_CONFIG_EXCEPTION_MISSING_ROW() => ServerResources.Get("GSS.DATABASE-CONFIG-EXCEPTION-MISSING-ROW");

    public static string GSS_DATABASE_CONFIG_EXCEPTION_MISSING_ROW(CultureInfo culture) => ServerResources.Get("GSS.DATABASE-CONFIG-EXCEPTION-MISSING-ROW", culture);

    public static string GSS_DATABASE_CONFIG_EXCEPTION_EXTRA_ROWS() => ServerResources.Get("GSS.DATABASE-CONFIG-EXCEPTION-EXTRA-ROWS");

    public static string GSS_DATABASE_CONFIG_EXCEPTION_EXTRA_ROWS(CultureInfo culture) => ServerResources.Get("GSS.DATABASE-CONFIG-EXCEPTION-EXTRA-ROWS", culture);

    public static string GSS_DATABASE_CONFIG_EXCEPTION_MISSING_RESULT_SET1() => ServerResources.Get("GSS.DATABASE-CONFIG-EXCEPTION-MISSING-RESULT-SET1");

    public static string GSS_DATABASE_CONFIG_EXCEPTION_MISSING_RESULT_SET1(CultureInfo culture) => ServerResources.Get("GSS.DATABASE-CONFIG-EXCEPTION-MISSING-RESULT-SET1", culture);

    public static string GSS_DATABASE_CONFIG_EXCEPTION_MISSING_RESULT_SET2() => ServerResources.Get("GSS.DATABASE-CONFIG-EXCEPTION-MISSING-RESULT-SET2");

    public static string GSS_DATABASE_CONFIG_EXCEPTION_MISSING_RESULT_SET2(CultureInfo culture) => ServerResources.Get("GSS.DATABASE-CONFIG-EXCEPTION-MISSING-RESULT-SET2", culture);

    public static string GSS_DIRECTORYSERVICESCOMEXCEPTION(object arg0, object arg1) => ServerResources.Format("GSS.DIRECTORYSERVICESCOMEXCEPTION", arg0, arg1);

    public static string GSS_DIRECTORYSERVICESCOMEXCEPTION(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format("GSS.DIRECTORYSERVICESCOMEXCEPTION", culture, arg0, arg1);
    }

    public static string GSS_DIRECTORYSERVICESQUERYEXCEPTION() => ServerResources.Get("GSS.DIRECTORYSERVICESQUERYEXCEPTION");

    public static string GSS_DIRECTORYSERVICESQUERYEXCEPTION(CultureInfo culture) => ServerResources.Get("GSS.DIRECTORYSERVICESQUERYEXCEPTION", culture);

    public static string GSS_DUPLICATE_MEMBER() => ServerResources.Get("GSS.DUPLICATE-MEMBER");

    public static string GSS_DUPLICATE_MEMBER(CultureInfo culture) => ServerResources.Get("GSS.DUPLICATE-MEMBER", culture);

    public static string GSS_DUPLICATE_NAME() => ServerResources.Get("GSS.DUPLICATE-NAME");

    public static string GSS_DUPLICATE_NAME(CultureInfo culture) => ServerResources.Get("GSS.DUPLICATE-NAME", culture);

    public static string GSS_ID_CREATED_EVENT_FIRED(object arg0) => ServerResources.Format("GSS.ID-CREATED-EVENT-FIRED", arg0);

    public static string GSS_ID_CREATED_EVENT_FIRED(object arg0, CultureInfo culture) => ServerResources.Format("GSS.ID-CREATED-EVENT-FIRED", culture, arg0);

    public static string GSS_ID_DELETED(object arg0) => ServerResources.Format("GSS.ID-DELETED", arg0);

    public static string GSS_ID_DELETED(object arg0, CultureInfo culture) => ServerResources.Format("GSS.ID-DELETED", culture, arg0);

    public static string GSS_ID_MEMBER_CHANGE_EVENT_FIRED(object arg0) => ServerResources.Format("GSS.ID-MEMBER-CHANGE-EVENT-FIRED", arg0);

    public static string GSS_ID_MEMBER_CHANGE_EVENT_FIRED(object arg0, CultureInfo culture) => ServerResources.Format("GSS.ID-MEMBER-CHANGE-EVENT-FIRED", culture, arg0);

    public static string GSS_INTERNALSTOREDPROCDUREERROR() => ServerResources.Get("GSS.INTERNALSTOREDPROCDUREERROR");

    public static string GSS_INTERNALSTOREDPROCDUREERROR(CultureInfo culture) => ServerResources.Get("GSS.INTERNALSTOREDPROCDUREERROR", culture);

    public static string GSS_NOT_AUTHENTICATED() => ServerResources.Get("GSS.NOT-AUTHENTICATED");

    public static string GSS_NOT_AUTHENTICATED(CultureInfo culture) => ServerResources.Get("GSS.NOT-AUTHENTICATED", culture);

    public static string GSS_REGISTER_OBJECT_PROJECT_MISMATCH(object arg0, object arg1) => ServerResources.Format("GSS.REGISTER-OBJECT-PROJECT-MISMATCH", arg0, arg1);

    public static string GSS_REGISTER_OBJECT_PROJECT_MISMATCH(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format("GSS.REGISTER-OBJECT-PROJECT-MISMATCH", culture, arg0, arg1);
    }

    public static string GSS_RETRIEVE_IDENTITY_FAILED(object arg0) => ServerResources.Format("GSS.RETRIEVE-IDENTITY-FAILED", arg0);

    public static string GSS_RETRIEVE_IDENTITY_FAILED(object arg0, CultureInfo culture) => ServerResources.Format("GSS.RETRIEVE-IDENTITY-FAILED", culture, arg0);

    public static string GSS_RETRIEVE_MEMBERS_FAILED(object arg0) => ServerResources.Format("GSS.RETRIEVE-MEMBERS-FAILED", arg0);

    public static string GSS_RETRIEVE_MEMBERS_FAILED(object arg0, CultureInfo culture) => ServerResources.Format("GSS.RETRIEVE-MEMBERS-FAILED", culture, arg0);

    public static string GSS_SLOW_AD_READ(object arg0, object arg1) => ServerResources.Format("GSS.SLOW_AD_READ", arg0, arg1);

    public static string GSS_SLOW_AD_READ(object arg0, object arg1, CultureInfo culture) => ServerResources.Format("GSS.SLOW_AD_READ", culture, arg0, arg1);

    public static string GSS_UNKNOWN_ACCESS_CHECK_TYPE() => ServerResources.Get("GSS.UNKNOWN-ACCESS-CHECK-TYPE");

    public static string GSS_UNKNOWN_ACCESS_CHECK_TYPE(CultureInfo culture) => ServerResources.Get("GSS.UNKNOWN-ACCESS-CHECK-TYPE", culture);

    public static string GSS_GLOBALGROUPCREATIONERROR(object arg0) => ServerResources.Format("GSS.GLOBALGROUPCREATIONERROR", arg0);

    public static string GSS_GLOBALGROUPCREATIONERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.GLOBALGROUPCREATIONERROR", culture, arg0);

    public static string GSS_PROJECTGROUPCREATIONERROR(object arg0, object arg1) => ServerResources.Format("GSS.PROJECTGROUPCREATIONERROR", arg0, arg1);

    public static string GSS_PROJECTGROUPCREATIONERROR(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format("GSS.PROJECTGROUPCREATIONERROR", culture, arg0, arg1);
    }

    public static string GSS_UNREGISTERPROJECTERROR(object arg0) => ServerResources.Format("GSS.UNREGISTERPROJECTERROR", arg0);

    public static string GSS_UNREGISTERPROJECTERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.UNREGISTERPROJECTERROR", culture, arg0);

    public static string GSS_REGISTEROBJECTEXISTSERROR(object arg0) => ServerResources.Format("GSS.REGISTEROBJECTEXISTSERROR", arg0);

    public static string GSS_REGISTEROBJECTEXISTSERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.REGISTEROBJECTEXISTSERROR", culture, arg0);

    public static string GSS_REGISTEROBJECTNOCLASSERROR(object arg0) => ServerResources.Format("GSS.REGISTEROBJECTNOCLASSERROR", arg0);

    public static string GSS_REGISTEROBJECTNOCLASSERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.REGISTEROBJECTNOCLASSERROR", culture, arg0);

    public static string GSS_REGISTEROBJECTNOPROJECTERROR(object arg0) => ServerResources.Format("GSS.REGISTEROBJECTNOPROJECTERROR", arg0);

    public static string GSS_REGISTEROBJECTNOPROJECTERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.REGISTEROBJECTNOPROJECTERROR", culture, arg0);

    public static string GSS_REGISTEROBJECTBADPARENTERROR(object arg0) => ServerResources.Format("GSS.REGISTEROBJECTBADPARENTERROR", arg0);

    public static string GSS_REGISTEROBJECTBADPARENTERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.REGISTEROBJECTBADPARENTERROR", culture, arg0);

    public static string GSS_REGISTERPROJECTERROR(object arg0) => ServerResources.Format("GSS.REGISTERPROJECTERROR", arg0);

    public static string GSS_REGISTERPROJECTERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.REGISTERPROJECTERROR", culture, arg0);

    public static string GSS_CIRCULAR_OBJECT_INHERITANCE(object arg0, object arg1) => ServerResources.Format("GSS.CIRCULAR-OBJECT-INHERITANCE", arg0, arg1);

    public static string GSS_CIRCULAR_OBJECT_INHERITANCE(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format("GSS.CIRCULAR-OBJECT-INHERITANCE", culture, arg0, arg1);
    }

    public static string GSS_INVALID_XML_IN_IDENTITY(object arg0) => ServerResources.Format("GSS.INVALID_XML_IN_IDENTITY", arg0);

    public static string GSS_INVALID_XML_IN_IDENTITY(object arg0, CultureInfo culture) => ServerResources.Format("GSS.INVALID_XML_IN_IDENTITY", culture, arg0);

    public static string GSS_USING_CACHED_IDENTITY(object arg0, object arg1) => ServerResources.Format("GSS.USING_CACHED_IDENTITY", arg0, arg1);

    public static string GSS_USING_CACHED_IDENTITY(object arg0, object arg1, CultureInfo culture) => ServerResources.Format("GSS.USING_CACHED_IDENTITY", culture, arg0, arg1);

    public static string GSS_AD_READ_MEMBER_ERROR(object arg0, object arg1) => ServerResources.Format("GSS.AD_READ_MEMBER_ERROR", arg0, arg1);

    public static string GSS_AD_READ_MEMBER_ERROR(object arg0, object arg1, CultureInfo culture) => ServerResources.Format("GSS.AD_READ_MEMBER_ERROR", culture, arg0, arg1);

    public static string GSS_AD_MEMBER_PROPERTY_ERROR(object arg0) => ServerResources.Format("GSS.AD_MEMBER_PROPERTY_ERROR", arg0);

    public static string GSS_AD_MEMBER_PROPERTY_ERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.AD_MEMBER_PROPERTY_ERROR", culture, arg0);

    public static string GSS_AD_PRIMARY_GROUP_QUERY_ERROR(object arg0) => ServerResources.Format("GSS.AD_PRIMARY_GROUP_QUERY_ERROR", arg0);

    public static string GSS_AD_PRIMARY_GROUP_QUERY_ERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.AD_PRIMARY_GROUP_QUERY_ERROR", culture, arg0);

    public static string GSS_SYNC_IDENTITY_ERROR(object arg0) => ServerResources.Format("GSS.SYNC_IDENTITY_ERROR", arg0);

    public static string GSS_SYNC_IDENTITY_ERROR(object arg0, CultureInfo culture) => ServerResources.Format("GSS.SYNC_IDENTITY_ERROR", culture, arg0);

    public static string GSS_EVERYONE_MEMBERSHIP() => ServerResources.Get("GSS.EVERYONE-MEMBERSHIP");

    public static string GSS_EVERYONE_MEMBERSHIP(CultureInfo culture) => ServerResources.Get("GSS.EVERYONE-MEMBERSHIP", culture);

    public static string CSS_NODE_CREATE_CHILDREN() => ServerResources.Get(nameof (CSS_NODE_CREATE_CHILDREN));

    public static string CSS_NODE_CREATE_CHILDREN(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_CREATE_CHILDREN), culture);

    public static string CSS_NODE_DELETE() => ServerResources.Get(nameof (CSS_NODE_DELETE));

    public static string CSS_NODE_DELETE(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_DELETE), culture);

    public static string CSS_NODE_GENERIC_READ() => ServerResources.Get(nameof (CSS_NODE_GENERIC_READ));

    public static string CSS_NODE_GENERIC_READ(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_GENERIC_READ), culture);

    public static string CSS_NODE_GENERIC_WRITE() => ServerResources.Get(nameof (CSS_NODE_GENERIC_WRITE));

    public static string CSS_NODE_GENERIC_WRITE(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_GENERIC_WRITE), culture);

    public static string CSS_NODE_WORK_ITEM_READ() => ServerResources.Get(nameof (CSS_NODE_WORK_ITEM_READ));

    public static string CSS_NODE_WORK_ITEM_READ(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_WORK_ITEM_READ), culture);

    public static string CSS_NODE_WORK_ITEM_WRITE() => ServerResources.Get(nameof (CSS_NODE_WORK_ITEM_WRITE));

    public static string CSS_NODE_WORK_ITEM_WRITE(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_WORK_ITEM_WRITE), culture);

    public static string CSS_EMPTY_ARGUMENT(object arg0) => ServerResources.Format("CSS.EMPTY-ARGUMENT", arg0);

    public static string CSS_EMPTY_ARGUMENT(object arg0, CultureInfo culture) => ServerResources.Format("CSS.EMPTY-ARGUMENT", culture, arg0);

    public static string CSS_NEGATIVE_ARGUMENT(object arg0) => ServerResources.Format("CSS.NEGATIVE-ARGUMENT", arg0);

    public static string CSS_NEGATIVE_ARGUMENT(object arg0, CultureInfo culture) => ServerResources.Format("CSS.NEGATIVE-ARGUMENT", culture, arg0);

    public static string CSS_INVALID_ARRAY(object arg0) => ServerResources.Format("CSS.INVALID-ARRAY", arg0);

    public static string CSS_INVALID_ARRAY(object arg0, CultureInfo culture) => ServerResources.Format("CSS.INVALID-ARRAY", culture, arg0);

    public static string CSS_INVALID_URI(object arg0, object arg1) => ServerResources.Format("CSS.INVALID-URI", arg0, arg1);

    public static string CSS_INVALID_URI(object arg0, object arg1, CultureInfo culture) => ServerResources.Format("CSS.INVALID-URI", culture, arg0, arg1);

    public static string CSS_INVALID_NAME(object arg0) => ServerResources.Format("CSS.INVALID-NAME", arg0);

    public static string CSS_INVALID_NAME(object arg0, CultureInfo culture) => ServerResources.Format("CSS.INVALID-NAME", culture, arg0);

    public static string CSS_INVALID_PATH(object arg0) => ServerResources.Format("CSS.INVALID-PATH", arg0);

    public static string CSS_INVALID_PATH(object arg0, CultureInfo culture) => ServerResources.Format("CSS.INVALID-PATH", culture, arg0);

    public static string CSS_INVALID_STATE(object arg0) => ServerResources.Format("CSS.INVALID-STATE", arg0);

    public static string CSS_INVALID_STATE(object arg0, CultureInfo culture) => ServerResources.Format("CSS.INVALID-STATE", culture, arg0);

    public static string CSS_INVALID_TYPE(object arg0) => ServerResources.Format("CSS.INVALID-TYPE", arg0);

    public static string CSS_INVALID_TYPE(object arg0, CultureInfo culture) => ServerResources.Format("CSS.INVALID-TYPE", culture, arg0);

    public static string CSS_INVALID_SCHEMA() => ServerResources.Get("CSS.INVALID-SCHEMA");

    public static string CSS_INVALID_SCHEMA(CultureInfo culture) => ServerResources.Get("CSS.INVALID-SCHEMA", culture);

    public static string CSS_INVALID_ROOT_NODE_COUNT() => ServerResources.Get("CSS.INVALID-ROOT-NODE-COUNT");

    public static string CSS_INVALID_ROOT_NODE_COUNT(CultureInfo culture) => ServerResources.Get("CSS.INVALID-ROOT-NODE-COUNT", culture);

    public static string CSS_INVALID_ROOT_NODES_SAME_NAME() => ServerResources.Get("CSS.INVALID-ROOT-NODES-SAME-NAME");

    public static string CSS_INVALID_ROOT_NODES_SAME_NAME(CultureInfo culture) => ServerResources.Get("CSS.INVALID-ROOT-NODES-SAME-NAME", culture);

    public static string CSS_INVALID_ROOT_NODES_SAME_TYPE() => ServerResources.Get("CSS.INVALID-ROOT-NODES-SAME-TYPE");

    public static string CSS_INVALID_ROOT_NODES_SAME_TYPE(CultureInfo culture) => ServerResources.Get("CSS.INVALID-ROOT-NODES-SAME-TYPE", culture);

    public static string CSS_PARENT_CHILD_MISMATCH() => ServerResources.Get("CSS.PARENT-CHILD-MISMATCH");

    public static string CSS_PARENT_CHILD_MISMATCH(CultureInfo culture) => ServerResources.Get("CSS.PARENT-CHILD-MISMATCH", culture);

    public static string CSS_SIBLING_NAME_CONFLICT() => ServerResources.Get("CSS.SIBLING-NAME-CONFLICT");

    public static string CSS_SIBLING_NAME_CONFLICT(CultureInfo culture) => ServerResources.Get("CSS.SIBLING-NAME-CONFLICT", culture);

    public static string CSS_NODE_DOES_NOT_EXIST_URI(object arg0) => ServerResources.Format("CSS.NODE-DOES-NOT-EXIST-URI", arg0);

    public static string CSS_NODE_DOES_NOT_EXIST_URI(object arg0, CultureInfo culture) => ServerResources.Format("CSS.NODE-DOES-NOT-EXIST-URI", culture, arg0);

    public static string CSS_NODE_DOES_NOT_EXIST_PATH(object arg0) => ServerResources.Format("CSS.NODE-DOES-NOT-EXIST-PATH", arg0);

    public static string CSS_NODE_DOES_NOT_EXIST_PATH(object arg0, CultureInfo culture) => ServerResources.Format("CSS.NODE-DOES-NOT-EXIST-PATH", culture, arg0);

    public static string CSS_PROJECT_DOES_NOT_EXIST_URI(object arg0) => ServerResources.Format("CSS.PROJECT-DOES-NOT-EXIST-URI", arg0);

    public static string CSS_PROJECT_DOES_NOT_EXIST_URI(object arg0, CultureInfo culture) => ServerResources.Format("CSS.PROJECT-DOES-NOT-EXIST-URI", culture, arg0);

    public static string CSS_PROJECT_DOES_NOT_EXIST_NAME(object arg0) => ServerResources.Format("CSS.PROJECT-DOES-NOT-EXIST-NAME", arg0);

    public static string CSS_PROJECT_DOES_NOT_EXIST_NAME(object arg0, CultureInfo culture) => ServerResources.Format("CSS.PROJECT-DOES-NOT-EXIST-NAME", culture, arg0);

    public static string CSS_PARENT_NODE_DOES_NOT_EXIST(object arg0) => ServerResources.Format("CSS.PARENT-NODE-DOES-NOT-EXIST", arg0);

    public static string CSS_PARENT_NODE_DOES_NOT_EXIST(object arg0, CultureInfo culture) => ServerResources.Format("CSS.PARENT-NODE-DOES-NOT-EXIST", culture, arg0);

    public static string CSS_RECLASSIFICATION_NODE_DOES_NOT_EXIST(object arg0) => ServerResources.Format("CSS.RECLASSIFICATION-NODE-DOES-NOT-EXIST", arg0);

    public static string CSS_RECLASSIFICATION_NODE_DOES_NOT_EXIST(object arg0, CultureInfo culture) => ServerResources.Format("CSS.RECLASSIFICATION-NODE-DOES-NOT-EXIST", culture, arg0);

    public static string CSS_PROJECT_ALREADY_EXISTS(object arg0) => ServerResources.Format("CSS.PROJECT-ALREADY-EXISTS", arg0);

    public static string CSS_PROJECT_ALREADY_EXISTS(object arg0, CultureInfo culture) => ServerResources.Format("CSS.PROJECT-ALREADY-EXISTS", culture, arg0);

    public static string CSS_NODE_ALREADY_EXISTS(object arg0) => ServerResources.Format("CSS.NODE-ALREADY-EXISTS", arg0);

    public static string CSS_NODE_ALREADY_EXISTS(object arg0, CultureInfo culture) => ServerResources.Format("CSS.NODE-ALREADY-EXISTS", culture, arg0);

    public static string CSS_CANNOT_MODIFY_ROOT_NODE(object arg0) => ServerResources.Format("CSS.CANNOT-MODIFY-ROOT-NODE", arg0);

    public static string CSS_CANNOT_MODIFY_ROOT_NODE(object arg0, CultureInfo culture) => ServerResources.Format("CSS.CANNOT-MODIFY-ROOT-NODE", culture, arg0);

    public static string CSS_MOVE_ARGUMENT_OUT_OF_RANGE(object arg0, object arg1) => ServerResources.Format("CSS.MOVE-ARGUMENT-OUT-OF-RANGE", arg0, arg1);

    public static string CSS_MOVE_ARGUMENT_OUT_OF_RANGE(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format("CSS.MOVE-ARGUMENT-OUT-OF-RANGE", culture, arg0, arg1);
    }

    public static string CSS_CANNOT_CREATE_CIRCULAR_REFERENCE() => ServerResources.Get("CSS.CANNOT-CREATE-CIRCULAR_REFERENCE");

    public static string CSS_CANNOT_CREATE_CIRCULAR_REFERENCE(CultureInfo culture) => ServerResources.Get("CSS.CANNOT-CREATE-CIRCULAR_REFERENCE", culture);

    public static string CSS_CANNOT_CHANGE_TREES(object arg0, object arg1) => ServerResources.Format("CSS.CANNOT-CHANGE-TREES", arg0, arg1);

    public static string CSS_CANNOT_CHANGE_TREES(object arg0, object arg1, CultureInfo culture) => ServerResources.Format("CSS.CANNOT-CHANGE-TREES", culture, arg0, arg1);

    public static string CSS_MAX_DEPTH_EXCEEDED() => ServerResources.Get("CSS.MAX-DEPTH-EXCEEDED");

    public static string CSS_MAX_DEPTH_EXCEEDED(CultureInfo culture) => ServerResources.Get("CSS.MAX-DEPTH-EXCEEDED", culture);

    public static string CSS_CANNOT_RECLASSIFY_TO_SELF() => ServerResources.Get("CSS.CANNOT-RECLASSIFY-TO-SELF");

    public static string CSS_CANNOT_RECLASSIFY_TO_SELF(CultureInfo culture) => ServerResources.Get("CSS.CANNOT-RECLASSIFY-TO-SELF", culture);

    public static string CSS_RECLASSIFICATION_TO_DIFFERENT_TREE(object arg0, object arg1) => ServerResources.Format("CSS.RECLASSIFICATION-TO-DIFFERENT-TREE", arg0, arg1);

    public static string CSS_RECLASSIFICATION_TO_DIFFERENT_TREE(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format("CSS.RECLASSIFICATION-TO-DIFFERENT-TREE", culture, arg0, arg1);
    }

    public static string CSS_RECLASSIFICATION_TO_SUBTREE(object arg0, object arg1) => ServerResources.Format("CSS.RECLASSIFICATION-TO-SUBTREE", arg0, arg1);

    public static string CSS_RECLASSIFICATION_TO_SUBTREE(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format("CSS.RECLASSIFICATION-TO-SUBTREE", culture, arg0, arg1);
    }

    public static string CSS_CANNOT_ADD_DATE_TO_NONITERATION(object arg0) => ServerResources.Format("CSS.CANNOT-ADD-DATE-TO-NONITERATION", arg0);

    public static string CSS_CANNOT_ADD_DATE_TO_NONITERATION(object arg0, CultureInfo culture) => ServerResources.Format("CSS.CANNOT-ADD-DATE-TO-NONITERATION", culture, arg0);

    public static string CSS_MUST_SPECIFY_BOTH_DATES() => ServerResources.Get("CSS.MUST-SPECIFY-BOTH-DATES");

    public static string CSS_MUST_SPECIFY_BOTH_DATES(CultureInfo culture) => ServerResources.Get("CSS.MUST-SPECIFY-BOTH-DATES", culture);

    public static string CSS_DATE_RANGE_INVALID() => ServerResources.Get("CSS.DATE-RANGE-INVALID");

    public static string CSS_DATE_RANGE_INVALID(CultureInfo culture) => ServerResources.Get("CSS.DATE-RANGE-INVALID", culture);

    public static string CSS_START_AFTER_FINISH() => ServerResources.Get("CSS.START-AFTER-FINISH");

    public static string CSS_START_AFTER_FINISH(CultureInfo culture) => ServerResources.Get("CSS.START-AFTER-FINISH", culture);

    public static string CSS_INVALID_PROJECT_PROPERTY_NAME() => ServerResources.Get("CSS.INVALID-PROJECT-PROPERTY-NAME");

    public static string CSS_INVALID_PROJECT_PROPERTY_NAME(CultureInfo culture) => ServerResources.Get("CSS.INVALID-PROJECT-PROPERTY-NAME", culture);

    public static string GSS_BACKGROUND_SYNC_EXCEPTION() => ServerResources.Get("GSS.BACKGROUND_SYNC_EXCEPTION");

    public static string GSS_BACKGROUND_SYNC_EXCEPTION(CultureInfo culture) => ServerResources.Get("GSS.BACKGROUND_SYNC_EXCEPTION", culture);

    public static string GSS_INVALID_DOMAIN(object arg0) => ServerResources.Format("GSS.INVALID_DOMAIN", arg0);

    public static string GSS_INVALID_DOMAIN(object arg0, CultureInfo culture) => ServerResources.Format("GSS.INVALID_DOMAIN", culture, arg0);

    public static string CSS_INVALID_PROPERTY_UPDATE() => ServerResources.Get("CSS.INVALID-PROPERTY-UPDATE");

    public static string CSS_INVALID_PROPERTY_UPDATE(CultureInfo culture) => ServerResources.Get("CSS.INVALID-PROPERTY-UPDATE", culture);

    public static string CSS_INCOMPATIBLE_CLIENT_FOR_PCW() => ServerResources.Get("CSS.INCOMPATIBLE-CLIENT-FOR-PCW");

    public static string CSS_INCOMPATIBLE_CLIENT_FOR_PCW(CultureInfo culture) => ServerResources.Get("CSS.INCOMPATIBLE-CLIENT-FOR-PCW", culture);

    public static string CSS_INCOMPATIBLE_CLIENT_FOR_TEMPLATES() => ServerResources.Get("CSS.INCOMPATIBLE-CLIENT-FOR-TEMPLATES");

    public static string CSS_INCOMPATIBLE_CLIENT_FOR_TEMPLATES(CultureInfo culture) => ServerResources.Get("CSS.INCOMPATIBLE-CLIENT-FOR-TEMPLATES", culture);

    public static string CSS_INCOMPATIBLE_CLIENT_FOR_PROJECTDELETE() => ServerResources.Get("CSS.INCOMPATIBLE-CLIENT-FOR-PROJECTDELETE");

    public static string CSS_INCOMPATIBLE_CLIENT_FOR_PROJECTDELETE(CultureInfo culture) => ServerResources.Get("CSS.INCOMPATIBLE-CLIENT-FOR-PROJECTDELETE", culture);

    public static string CSS_NODE_MANAGE_TEST_PLANS() => ServerResources.Get(nameof (CSS_NODE_MANAGE_TEST_PLANS));

    public static string CSS_NODE_MANAGE_TEST_PLANS(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_MANAGE_TEST_PLANS), culture);

    public static string SYNC_ITEM_DOES_NOT_EXIST_OR_ACCESS_DENIED() => ServerResources.Get("SYNC.ITEM_DOES_NOT_EXIST_OR_ACCESS_DENIED");

    public static string SYNC_ITEM_DOES_NOT_EXIST_OR_ACCESS_DENIED(CultureInfo culture) => ServerResources.Get("SYNC.ITEM_DOES_NOT_EXIST_OR_ACCESS_DENIED", culture);

    public static string SYNC_BAD_BASELINE_REV() => ServerResources.Get("SYNC.BAD_BASELINE_REV");

    public static string SYNC_BAD_BASELINE_REV(CultureInfo culture) => ServerResources.Get("SYNC.BAD_BASELINE_REV", culture);

    public static string SYNC_SUPERSEDED_BASELINE_REV() => ServerResources.Get("SYNC.SUPERSEDED_BASELINE_REV");

    public static string SYNC_SUPERSEDED_BASELINE_REV(CultureInfo culture) => ServerResources.Get("SYNC.SUPERSEDED_BASELINE_REV", culture);

    public static string CSS_NODE_MANAGE_TEST_SUITES() => ServerResources.Get(nameof (CSS_NODE_MANAGE_TEST_SUITES));

    public static string CSS_NODE_MANAGE_TEST_SUITES(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_MANAGE_TEST_SUITES), culture);

    public static string CSS_NODE_WORK_ITEM_SAVE_COMMENT() => ServerResources.Get(nameof (CSS_NODE_WORK_ITEM_SAVE_COMMENT));

    public static string CSS_NODE_WORK_ITEM_SAVE_COMMENT(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_WORK_ITEM_SAVE_COMMENT), culture);
  }
}
