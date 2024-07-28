// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.PipelineAuthorizationResources
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Pipelines.Authorization.Server
{
  internal static class PipelineAuthorizationResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (PipelineAuthorizationResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => PipelineAuthorizationResources.s_resMgr;

    private static string Get(string resourceName) => PipelineAuthorizationResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? PipelineAuthorizationResources.Get(resourceName) : PipelineAuthorizationResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) PipelineAuthorizationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? PipelineAuthorizationResources.GetInt(resourceName) : (int) PipelineAuthorizationResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) PipelineAuthorizationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? PipelineAuthorizationResources.GetBool(resourceName) : (bool) PipelineAuthorizationResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => PipelineAuthorizationResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = PipelineAuthorizationResources.Get(resourceName, culture);
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

    public static string MissingUsePermissionExceptionMessage() => PipelineAuthorizationResources.Get(nameof (MissingUsePermissionExceptionMessage));

    public static string MissingUsePermissionExceptionMessage(CultureInfo culture) => PipelineAuthorizationResources.Get(nameof (MissingUsePermissionExceptionMessage), culture);

    public static string MissingViewPermissionExceptionMessage() => PipelineAuthorizationResources.Get(nameof (MissingViewPermissionExceptionMessage));

    public static string MissingViewPermissionExceptionMessage(CultureInfo culture) => PipelineAuthorizationResources.Get(nameof (MissingViewPermissionExceptionMessage), culture);

    public static string MissingAdminPermissionExceptionMessage() => PipelineAuthorizationResources.Get(nameof (MissingAdminPermissionExceptionMessage));

    public static string MissingAdminPermissionExceptionMessage(CultureInfo culture) => PipelineAuthorizationResources.Get(nameof (MissingAdminPermissionExceptionMessage), culture);

    public static string UrlBodyResourceTypeMismatch() => PipelineAuthorizationResources.Get(nameof (UrlBodyResourceTypeMismatch));

    public static string UrlBodyResourceTypeMismatch(CultureInfo culture) => PipelineAuthorizationResources.Get(nameof (UrlBodyResourceTypeMismatch), culture);

    public static string UrlBodyResourceIdMismatch() => PipelineAuthorizationResources.Get(nameof (UrlBodyResourceIdMismatch));

    public static string UrlBodyResourceIdMismatch(CultureInfo culture) => PipelineAuthorizationResources.Get(nameof (UrlBodyResourceIdMismatch), culture);

    public static string DuplicateDefinitionIds() => PipelineAuthorizationResources.Get(nameof (DuplicateDefinitionIds));

    public static string DuplicateDefinitionIds(CultureInfo culture) => PipelineAuthorizationResources.Get(nameof (DuplicateDefinitionIds), culture);

    public static string ResourcesQueryRequiresTypeAndId() => PipelineAuthorizationResources.Get(nameof (ResourcesQueryRequiresTypeAndId));

    public static string ResourcesQueryRequiresTypeAndId(CultureInfo culture) => PipelineAuthorizationResources.Get(nameof (ResourcesQueryRequiresTypeAndId), culture);

    public static string ResourcesQueryRequiresType() => PipelineAuthorizationResources.Get(nameof (ResourcesQueryRequiresType));

    public static string ResourcesQueryRequiresType(CultureInfo culture) => PipelineAuthorizationResources.Get(nameof (ResourcesQueryRequiresType), culture);

    public static string InvalidResourceType() => PipelineAuthorizationResources.Get(nameof (InvalidResourceType));

    public static string InvalidResourceType(CultureInfo culture) => PipelineAuthorizationResources.Get(nameof (InvalidResourceType), culture);

    public static string ResourceIdLengthOverflow(object arg0) => PipelineAuthorizationResources.Format(nameof (ResourceIdLengthOverflow), arg0);

    public static string ResourceIdLengthOverflow(object arg0, CultureInfo culture) => PipelineAuthorizationResources.Format(nameof (ResourceIdLengthOverflow), culture, arg0);

    public static string MissingResourceObjectInBatchRequest() => PipelineAuthorizationResources.Get(nameof (MissingResourceObjectInBatchRequest));

    public static string MissingResourceObjectInBatchRequest(CultureInfo culture) => PipelineAuthorizationResources.Get(nameof (MissingResourceObjectInBatchRequest), culture);

    public static string NullPermissionsRequest() => PipelineAuthorizationResources.Get(nameof (NullPermissionsRequest));

    public static string NullPermissionsRequest(CultureInfo culture) => PipelineAuthorizationResources.Get(nameof (NullPermissionsRequest), culture);

    public static string MissingProjectAdminPermissionExceptionMessage() => PipelineAuthorizationResources.Get(nameof (MissingProjectAdminPermissionExceptionMessage));

    public static string MissingProjectAdminPermissionExceptionMessage(CultureInfo culture) => PipelineAuthorizationResources.Get(nameof (MissingProjectAdminPermissionExceptionMessage), culture);
  }
}
