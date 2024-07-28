// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AzComm.WebApi.AzCommWebApiResources
// Assembly: Microsoft.VisualStudio.Services.AzComm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1B69FBB-72A0-4C7F-B8FC-E0B0311A8184
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.AzComm.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.AzComm.WebApi
{
  internal static class AzCommWebApiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (AzCommWebApiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AzCommWebApiResources.s_resMgr;

    private static string Get(string resourceName) => AzCommWebApiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AzCommWebApiResources.Get(resourceName) : AzCommWebApiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AzCommWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AzCommWebApiResources.GetInt(resourceName) : (int) AzCommWebApiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AzCommWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AzCommWebApiResources.GetBool(resourceName) : (bool) AzCommWebApiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AzCommWebApiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AzCommWebApiResources.Get(resourceName, culture);
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

    public static string DuplicateNoteIdException(object arg0) => AzCommWebApiResources.Format(nameof (DuplicateNoteIdException), arg0);

    public static string DuplicateNoteIdException(object arg0, CultureInfo culture) => AzCommWebApiResources.Format(nameof (DuplicateNoteIdException), culture, arg0);

    public static string NoteNotFoundException(object arg0) => AzCommWebApiResources.Format(nameof (NoteNotFoundException), arg0);

    public static string NoteNotFoundException(object arg0, CultureInfo culture) => AzCommWebApiResources.Format(nameof (NoteNotFoundException), culture, arg0);
  }
}
