// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.ProjectAnalysisResources
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  internal static class ProjectAnalysisResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ProjectAnalysisResources), typeof (ProjectAnalysisResources).GetTypeInfo().Assembly);
    public const string NoDefaultBranchFoundOnRepo = "NoDefaultBranchFoundOnRepo";
    public const string RepositoryNotFound = "RepositoryNotFound";
    public const string LanguageMetadataExists = "LanguageMetadataExists";
    public const string LanguageMetadataNotFound = "LanguageMetadataNotFound";
    public const string GitFilePathNotFound = "GitFilePathNotFound";

    public static ResourceManager Manager => ProjectAnalysisResources.s_resMgr;

    public static string Get(string resourceName) => ProjectAnalysisResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    public static string Get(string resourceName, CultureInfo culture) => culture == null ? ProjectAnalysisResources.Get(resourceName) : ProjectAnalysisResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ProjectAnalysisResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ProjectAnalysisResources.GetInt(resourceName) : (int) ProjectAnalysisResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ProjectAnalysisResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ProjectAnalysisResources.GetBool(resourceName) : (bool) ProjectAnalysisResources.s_resMgr.GetObject(resourceName, culture);

    public static string Format(string resourceName, params object[] args) => ProjectAnalysisResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    public static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ProjectAnalysisResources.Get(resourceName, culture);
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
