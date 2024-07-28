// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ResourceStrings
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class ResourceStrings
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ResourceStrings), typeof (ResourceStrings).GetTypeInfo().Assembly);
    public const string BadServerConfig = "BadServerConfig";
    public const string FileDownloadInvalidFileId = "FileDownloadInvalidFileId";
    public const string FileDownloadUserNotAuthenticated = "FileDownloadUserNotAuthenticated";
    public const string FileUploadInvalidGuid = "FileUploadInvalidGuid";
    public const string FileUploadInvalidAreaNodeUri = "FileUploadInvalidAreaNodeUri";
    public const string FileUploadUserNotAuthenticated = "FileUploadUserNotAuthenticated";
    public const string InvalidRequestId = "InvalidRequestId";
    public const string InvalidRowVersion = "InvalidRowVersion";
    public const string NoMetadataTablesRequested = "NoMetadataTablesRequested";
    public const string NoQueryXml = "NoQueryXml";
    public const string RequestNotCancellable = "RequestNotCancellable";
    public const string UpdatePackageRequired = "UpdatePackageRequired";
    public const string UserNotInServiceGroup = "UserNotInServiceGroup";
    public const string UnknownServiceError = "UnknownServiceError";
    public const string MaxAttachmentSizeExceedsMaximum = "MaxAttachmentSizeExceedsMaximum";
    public const string InvalidQueryStringParameters = "InvalidQueryStringParameters";
    public const string RequestAlreadyActive = "RequestAlreadyActive";
    public const string XslTransformError = "XslTransformError";
    public const string StoryboardPage_ClickHereToOpenLink = "StoryboardPage_ClickHereToOpenLink";
    public const string StoryboardPage_RedirectingTo = "StoryboardPage_RedirectingTo";
    public const string EndpointNameAlreadyExists = "EndpointNameAlreadyExists";
    public const string ServiceEndpointAlreadyRecorded = "ServiceEndpointAlreadyRecorded";

    public static ResourceManager Manager => ResourceStrings.s_resMgr;

    public static string Get(string resourceName) => ResourceStrings.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    public static string Get(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.Get(resourceName) : ResourceStrings.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetInt(resourceName) : (int) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetBool(resourceName) : (bool) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    public static string Format(string resourceName, params object[] args) => ResourceStrings.Format(resourceName, CultureInfo.CurrentUICulture, args);

    public static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ResourceStrings.Get(resourceName, culture);
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
  }
}
