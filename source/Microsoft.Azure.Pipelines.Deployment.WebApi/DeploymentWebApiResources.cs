// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.DeploymentWebApiResources
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi
{
  internal static class DeploymentWebApiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (DeploymentWebApiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => DeploymentWebApiResources.s_resMgr;

    private static string Get(string resourceName) => DeploymentWebApiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? DeploymentWebApiResources.Get(resourceName) : DeploymentWebApiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DeploymentWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DeploymentWebApiResources.GetInt(resourceName) : (int) DeploymentWebApiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DeploymentWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DeploymentWebApiResources.GetBool(resourceName) : (bool) DeploymentWebApiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => DeploymentWebApiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DeploymentWebApiResources.Get(resourceName, culture);
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

    public static string DuplicateNoteIdException(object arg0) => DeploymentWebApiResources.Format(nameof (DuplicateNoteIdException), arg0);

    public static string DuplicateNoteIdException(object arg0, CultureInfo culture) => DeploymentWebApiResources.Format(nameof (DuplicateNoteIdException), culture, arg0);

    public static string NoteNotFoundException(object arg0) => DeploymentWebApiResources.Format(nameof (NoteNotFoundException), arg0);

    public static string NoteNotFoundException(object arg0, CultureInfo culture) => DeploymentWebApiResources.Format(nameof (NoteNotFoundException), culture, arg0);
  }
}
