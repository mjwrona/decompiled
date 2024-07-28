// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ServerTypesResources
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.Types
{
  internal static class ServerTypesResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ServerTypesResources), typeof (ServerTypesResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ServerTypesResources.s_resMgr;

    private static string Get(string resourceName) => ServerTypesResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ServerTypesResources.Get(resourceName) : ServerTypesResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ServerTypesResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ServerTypesResources.GetInt(resourceName) : (int) ServerTypesResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ServerTypesResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ServerTypesResources.GetBool(resourceName) : (bool) ServerTypesResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ServerTypesResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ServerTypesResources.Get(resourceName, culture);
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

    public static string ProjectOperationNotAllowedIfOrganizationInReadOnlyMode() => ServerTypesResources.Get(nameof (ProjectOperationNotAllowedIfOrganizationInReadOnlyMode));

    public static string ProjectOperationNotAllowedIfOrganizationInReadOnlyMode(CultureInfo culture) => ServerTypesResources.Get(nameof (ProjectOperationNotAllowedIfOrganizationInReadOnlyMode), culture);
  }
}
