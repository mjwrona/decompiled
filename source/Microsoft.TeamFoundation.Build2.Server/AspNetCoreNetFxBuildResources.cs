// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.AspNetCoreNetFxBuildResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class AspNetCoreNetFxBuildResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (AspNetCoreNetFxBuildResources), typeof (AspNetCoreNetFxBuildResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AspNetCoreNetFxBuildResources.s_resMgr;

    private static string Get(string resourceName) => AspNetCoreNetFxBuildResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AspNetCoreNetFxBuildResources.Get(resourceName) : AspNetCoreNetFxBuildResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AspNetCoreNetFxBuildResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AspNetCoreNetFxBuildResources.GetInt(resourceName) : (int) AspNetCoreNetFxBuildResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AspNetCoreNetFxBuildResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AspNetCoreNetFxBuildResources.GetBool(resourceName) : (bool) AspNetCoreNetFxBuildResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AspNetCoreNetFxBuildResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AspNetCoreNetFxBuildResources.Get(resourceName, culture);
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

    public static string ArtifactLabelMarkdown() => AspNetCoreNetFxBuildResources.Get(nameof (ArtifactLabelMarkdown));

    public static string ArtifactLabelMarkdown(CultureInfo culture) => AspNetCoreNetFxBuildResources.Get(nameof (ArtifactLabelMarkdown), culture);

    public static string ArtifactNameLabel() => AspNetCoreNetFxBuildResources.Get(nameof (ArtifactNameLabel));

    public static string ArtifactNameLabel(CultureInfo culture) => AspNetCoreNetFxBuildResources.Get(nameof (ArtifactNameLabel), culture);

    public static string Description() => AspNetCoreNetFxBuildResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => AspNetCoreNetFxBuildResources.Get(nameof (Description), culture);

    public static string Name() => AspNetCoreNetFxBuildResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => AspNetCoreNetFxBuildResources.Get(nameof (Name), culture);

    public static string PathToSolutionLabel() => AspNetCoreNetFxBuildResources.Get(nameof (PathToSolutionLabel));

    public static string PathToSolutionLabel(CultureInfo culture) => AspNetCoreNetFxBuildResources.Get(nameof (PathToSolutionLabel), culture);

    public static string PathToSolutionMarkdown() => AspNetCoreNetFxBuildResources.Get(nameof (PathToSolutionMarkdown));

    public static string PathToSolutionMarkdown(CultureInfo culture) => AspNetCoreNetFxBuildResources.Get(nameof (PathToSolutionMarkdown), culture);

    public static string TestAssemblyLabel() => AspNetCoreNetFxBuildResources.Get(nameof (TestAssemblyLabel));

    public static string TestAssemblyLabel(CultureInfo culture) => AspNetCoreNetFxBuildResources.Get(nameof (TestAssemblyLabel), culture);

    public static string TestAssemblyMarkdown() => AspNetCoreNetFxBuildResources.Get(nameof (TestAssemblyMarkdown));

    public static string TestAssemblyMarkdown(CultureInfo culture) => AspNetCoreNetFxBuildResources.Get(nameof (TestAssemblyMarkdown), culture);
  }
}
