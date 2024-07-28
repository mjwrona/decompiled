// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployIISWebsiteResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployIISWebsiteResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployIISWebsiteResources), IntrospectionExtensions.GetTypeInfo(typeof (deployIISWebsiteResources)).Assembly);

    public static ResourceManager Manager => deployIISWebsiteResources.s_resMgr;

    private static string Get(string resourceName) => deployIISWebsiteResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployIISWebsiteResources.Get(resourceName) : deployIISWebsiteResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployIISWebsiteResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployIISWebsiteResources.GetInt(resourceName) : (int) deployIISWebsiteResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployIISWebsiteResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployIISWebsiteResources.GetBool(resourceName) : (bool) deployIISWebsiteResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployIISWebsiteResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployIISWebsiteResources.Get(resourceName, culture);
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

    public static string ActionIISWebsiteLabel() => deployIISWebsiteResources.Get(nameof (ActionIISWebsiteLabel));

    public static string ActionIISWebsiteLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (ActionIISWebsiteLabel), culture);

    public static string ActionIISWebsiteMarkdown() => deployIISWebsiteResources.Get(nameof (ActionIISWebsiteMarkdown));

    public static string ActionIISWebsiteMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (ActionIISWebsiteMarkdown), culture);

    public static string AddBindingLabel() => deployIISWebsiteResources.Get(nameof (AddBindingLabel));

    public static string AddBindingLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (AddBindingLabel), culture);

    public static string AddBindingMarkdown() => deployIISWebsiteResources.Get(nameof (AddBindingMarkdown));

    public static string AddBindingMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (AddBindingMarkdown), culture);

    public static string AppPoolNameForWebsiteLabel() => deployIISWebsiteResources.Get(nameof (AppPoolNameForWebsiteLabel));

    public static string AppPoolNameForWebsiteLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (AppPoolNameForWebsiteLabel), culture);

    public static string AppPoolNameForWebsiteMarkdown() => deployIISWebsiteResources.Get(nameof (AppPoolNameForWebsiteMarkdown));

    public static string AppPoolNameForWebsiteMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (AppPoolNameForWebsiteMarkdown), culture);

    public static string AppPoolNameLabel() => deployIISWebsiteResources.Get(nameof (AppPoolNameLabel));

    public static string AppPoolNameLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (AppPoolNameLabel), culture);

    public static string AppPoolNameMarkdown() => deployIISWebsiteResources.Get(nameof (AppPoolNameMarkdown));

    public static string AppPoolNameMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (AppPoolNameMarkdown), culture);

    public static string BindingsLabel() => deployIISWebsiteResources.Get(nameof (BindingsLabel));

    public static string BindingsLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (BindingsLabel), culture);

    public static string BindingsMarkdown() => deployIISWebsiteResources.Get(nameof (BindingsMarkdown));

    public static string BindingsMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (BindingsMarkdown), culture);

    public static string ConnectionStringLabel() => deployIISWebsiteResources.Get(nameof (ConnectionStringLabel));

    public static string ConnectionStringLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (ConnectionStringLabel), culture);

    public static string ConnectionStringMarkdown() => deployIISWebsiteResources.Get(nameof (ConnectionStringMarkdown));

    public static string ConnectionStringMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (ConnectionStringMarkdown), culture);

    public static string CreateOrUpdateAppPoolForWebsiteLabel() => deployIISWebsiteResources.Get(nameof (CreateOrUpdateAppPoolForWebsiteLabel));

    public static string CreateOrUpdateAppPoolForWebsiteLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (CreateOrUpdateAppPoolForWebsiteLabel), culture);

    public static string CreateOrUpdateAppPoolForWebsiteMarkdown() => deployIISWebsiteResources.Get(nameof (CreateOrUpdateAppPoolForWebsiteMarkdown));

    public static string CreateOrUpdateAppPoolForWebsiteMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (CreateOrUpdateAppPoolForWebsiteMarkdown), culture);

    public static string DatabaseNameLabel() => deployIISWebsiteResources.Get(nameof (DatabaseNameLabel));

    public static string DatabaseNameLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (DatabaseNameLabel), culture);

    public static string DatabaseNameMarkdown() => deployIISWebsiteResources.Get(nameof (DatabaseNameMarkdown));

    public static string DatabaseNameMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (DatabaseNameMarkdown), culture);

    public static string Description() => deployIISWebsiteResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (Description), culture);

    public static string IISDeploymentTypeLabel() => deployIISWebsiteResources.Get(nameof (IISDeploymentTypeLabel));

    public static string IISDeploymentTypeLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (IISDeploymentTypeLabel), culture);

    public static string IISDeploymentTypeMarkdown() => deployIISWebsiteResources.Get(nameof (IISDeploymentTypeMarkdown));

    public static string IISDeploymentTypeMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (IISDeploymentTypeMarkdown), culture);

    public static string InlineSqlLabel() => deployIISWebsiteResources.Get(nameof (InlineSqlLabel));

    public static string InlineSqlLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (InlineSqlLabel), culture);

    public static string InlineSqlMarkdown() => deployIISWebsiteResources.Get(nameof (InlineSqlMarkdown));

    public static string InlineSqlMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (InlineSqlMarkdown), culture);

    public static string Name() => deployIISWebsiteResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (Name), culture);

    public static string SqlFileLabel() => deployIISWebsiteResources.Get(nameof (SqlFileLabel));

    public static string SqlFileLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (SqlFileLabel), culture);

    public static string SqlFileMarkdown() => deployIISWebsiteResources.Get(nameof (SqlFileMarkdown));

    public static string SqlFileMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (SqlFileMarkdown), culture);

    public static string TargetMethodLabel() => deployIISWebsiteResources.Get(nameof (TargetMethodLabel));

    public static string TargetMethodLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (TargetMethodLabel), culture);

    public static string TargetMethodMarkdown() => deployIISWebsiteResources.Get(nameof (TargetMethodMarkdown));

    public static string TargetMethodMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (TargetMethodMarkdown), culture);

    public static string TaskTypeLabel() => deployIISWebsiteResources.Get(nameof (TaskTypeLabel));

    public static string TaskTypeLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (TaskTypeLabel), culture);

    public static string TaskTypeMarkdown() => deployIISWebsiteResources.Get(nameof (TaskTypeMarkdown));

    public static string TaskTypeMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (TaskTypeMarkdown), culture);

    public static string VirtualPathForApplicationLabel() => deployIISWebsiteResources.Get(nameof (VirtualPathForApplicationLabel));

    public static string VirtualPathForApplicationLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (VirtualPathForApplicationLabel), culture);

    public static string VirtualPathForApplicationMarkdown() => deployIISWebsiteResources.Get(nameof (VirtualPathForApplicationMarkdown));

    public static string VirtualPathForApplicationMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (VirtualPathForApplicationMarkdown), culture);

    public static string WebsiteNameLabel() => deployIISWebsiteResources.Get(nameof (WebsiteNameLabel));

    public static string WebsiteNameLabel(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (WebsiteNameLabel), culture);

    public static string WebsiteNameMarkdown() => deployIISWebsiteResources.Get(nameof (WebsiteNameMarkdown));

    public static string WebsiteNameMarkdown(CultureInfo culture) => deployIISWebsiteResources.Get(nameof (WebsiteNameMarkdown), culture);
  }
}
