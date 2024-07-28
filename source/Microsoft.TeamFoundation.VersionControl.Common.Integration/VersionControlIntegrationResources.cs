// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VersionControlIntegrationResources
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  internal static class VersionControlIntegrationResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (VersionControlIntegrationResources).GetTypeInfo().Assembly);
    public const string VersionControlNotRegistered = "VersionControlNotRegistered";
    public const string ChangesetFormat = "ChangesetFormat";
    public const string CheckinContentTitle = "CheckinContentTitle";
    public const string CheckinContentTitleNoComment = "CheckinContentTitleNoComment";
    public const string CheckinEmailTitle = "CheckinEmailTitle";
    public const string CheckinEmailTitleTemplate = "CheckinEmailTitleTemplate";
    public const string ShelvesetFormat = "ShelvesetFormat";
    public const string ShelvesetContentTitle = "ShelvesetContentTitle";
    public const string ShelvesetContentTitleNoComment = "ShelvesetContentTitleNoComment";
    public const string ShelvesetEmailTitle = "ShelvesetEmailTitle";
    public const string ShelvesetEmailTitleTemplate = "ShelvesetEmailTitleTemplate";
    public const string ShelvesetEmailTitleTemplateNoType = "ShelvesetEmailTitleTemplateNoType";
    public const string ShelvesetCreated = "ShelvesetCreated";
    public const string ShelvesetUpdated = "ShelvesetUpdated";
    public const string ShelvedItem = "ShelvedItem";
    public const string TeamFoundationServerNameMissing = "TeamFoundationServerNameMissing";
    public const string TeamProjectListSeparator = "TeamProjectListSeparator";
    public const string VersionedItem = "VersionedItem";

    public static ResourceManager Manager => VersionControlIntegrationResources.s_resMgr;

    public static string Get(string resourceName) => VersionControlIntegrationResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    public static string Get(string resourceName, CultureInfo culture) => culture == null ? VersionControlIntegrationResources.Get(resourceName) : VersionControlIntegrationResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) VersionControlIntegrationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? VersionControlIntegrationResources.GetInt(resourceName) : (int) VersionControlIntegrationResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) VersionControlIntegrationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? VersionControlIntegrationResources.GetBool(resourceName) : (bool) VersionControlIntegrationResources.s_resMgr.GetObject(resourceName, culture);

    public static string Format(string resourceName, params object[] args) => VersionControlIntegrationResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    public static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = VersionControlIntegrationResources.Get(resourceName, culture);
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
