// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.manageAndDeployIISWebsiteResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class manageAndDeployIISWebsiteResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (manageAndDeployIISWebsiteResources), IntrospectionExtensions.GetTypeInfo(typeof (manageAndDeployIISWebsiteResources)).Assembly);

    public static ResourceManager Manager => manageAndDeployIISWebsiteResources.s_resMgr;

    private static string Get(string resourceName) => manageAndDeployIISWebsiteResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? manageAndDeployIISWebsiteResources.Get(resourceName) : manageAndDeployIISWebsiteResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) manageAndDeployIISWebsiteResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? manageAndDeployIISWebsiteResources.GetInt(resourceName) : (int) manageAndDeployIISWebsiteResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) manageAndDeployIISWebsiteResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? manageAndDeployIISWebsiteResources.GetBool(resourceName) : (bool) manageAndDeployIISWebsiteResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => manageAndDeployIISWebsiteResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = manageAndDeployIISWebsiteResources.Get(resourceName, culture);
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

    public static string ActionIISWebsiteLabel() => manageAndDeployIISWebsiteResources.Get(nameof (ActionIISWebsiteLabel));

    public static string ActionIISWebsiteLabel(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (ActionIISWebsiteLabel), culture);

    public static string ActionIISWebsiteMarkdown() => manageAndDeployIISWebsiteResources.Get(nameof (ActionIISWebsiteMarkdown));

    public static string ActionIISWebsiteMarkdown(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (ActionIISWebsiteMarkdown), culture);

    public static string AddBindingLabel() => manageAndDeployIISWebsiteResources.Get(nameof (AddBindingLabel));

    public static string AddBindingLabel(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (AddBindingLabel), culture);

    public static string AddBindingMarkdown() => manageAndDeployIISWebsiteResources.Get(nameof (AddBindingMarkdown));

    public static string AddBindingMarkdown(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (AddBindingMarkdown), culture);

    public static string AppPoolNameLabel() => manageAndDeployIISWebsiteResources.Get(nameof (AppPoolNameLabel));

    public static string AppPoolNameLabel(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (AppPoolNameLabel), culture);

    public static string AppPoolNameMarkdown() => manageAndDeployIISWebsiteResources.Get(nameof (AppPoolNameMarkdown));

    public static string AppPoolNameMarkdown(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (AppPoolNameMarkdown), culture);

    public static string BindingsLabel() => manageAndDeployIISWebsiteResources.Get(nameof (BindingsLabel));

    public static string BindingsLabel(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (BindingsLabel), culture);

    public static string BindingsMarkdown() => manageAndDeployIISWebsiteResources.Get(nameof (BindingsMarkdown));

    public static string BindingsMarkdown(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (BindingsMarkdown), culture);

    public static string Description() => manageAndDeployIISWebsiteResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (Description), culture);

    public static string IISDeploymentTypeLabel() => manageAndDeployIISWebsiteResources.Get(nameof (IISDeploymentTypeLabel));

    public static string IISDeploymentTypeLabel(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (IISDeploymentTypeLabel), culture);

    public static string IISDeploymentTypeMarkdown() => manageAndDeployIISWebsiteResources.Get(nameof (IISDeploymentTypeMarkdown));

    public static string IISDeploymentTypeMarkdown(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (IISDeploymentTypeMarkdown), culture);

    public static string Name() => manageAndDeployIISWebsiteResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (Name), culture);

    public static string VirtualPathForApplicationLabel() => manageAndDeployIISWebsiteResources.Get(nameof (VirtualPathForApplicationLabel));

    public static string VirtualPathForApplicationLabel(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (VirtualPathForApplicationLabel), culture);

    public static string VirtualPathForApplicationMarkdown() => manageAndDeployIISWebsiteResources.Get(nameof (VirtualPathForApplicationMarkdown));

    public static string VirtualPathForApplicationMarkdown(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (VirtualPathForApplicationMarkdown), culture);

    public static string WebsiteNameLabel() => manageAndDeployIISWebsiteResources.Get(nameof (WebsiteNameLabel));

    public static string WebsiteNameLabel(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (WebsiteNameLabel), culture);

    public static string WebsiteNameMarkdown() => manageAndDeployIISWebsiteResources.Get(nameof (WebsiteNameMarkdown));

    public static string WebsiteNameMarkdown(CultureInfo culture) => manageAndDeployIISWebsiteResources.Get(nameof (WebsiteNameMarkdown), culture);
  }
}
