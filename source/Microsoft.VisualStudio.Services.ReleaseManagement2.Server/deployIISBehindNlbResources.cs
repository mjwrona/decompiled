// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployIISBehindNlbResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployIISBehindNlbResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployIISBehindNlbResources), IntrospectionExtensions.GetTypeInfo(typeof (deployIISBehindNlbResources)).Assembly);

    public static ResourceManager Manager => deployIISBehindNlbResources.s_resMgr;

    private static string Get(string resourceName) => deployIISBehindNlbResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployIISBehindNlbResources.Get(resourceName) : deployIISBehindNlbResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployIISBehindNlbResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployIISBehindNlbResources.GetInt(resourceName) : (int) deployIISBehindNlbResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployIISBehindNlbResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployIISBehindNlbResources.GetBool(resourceName) : (bool) deployIISBehindNlbResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployIISBehindNlbResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployIISBehindNlbResources.Get(resourceName, culture);
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

    public static string ConnectedServiceNameLabel() => deployIISBehindNlbResources.Get(nameof (ConnectedServiceNameLabel));

    public static string ConnectedServiceNameLabel(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (ConnectedServiceNameLabel), culture);

    public static string ConnectedServiceNameMarkdown() => deployIISBehindNlbResources.Get(nameof (ConnectedServiceNameMarkdown));

    public static string ConnectedServiceNameMarkdown(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (ConnectedServiceNameMarkdown), culture);

    public static string ConnectionStringLabel() => deployIISBehindNlbResources.Get(nameof (ConnectionStringLabel));

    public static string ConnectionStringLabel(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (ConnectionStringLabel), culture);

    public static string ConnectionStringMarkdown() => deployIISBehindNlbResources.Get(nameof (ConnectionStringMarkdown));

    public static string ConnectionStringMarkdown(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (ConnectionStringMarkdown), culture);

    public static string DatabaseNameLabel() => deployIISBehindNlbResources.Get(nameof (DatabaseNameLabel));

    public static string DatabaseNameLabel(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (DatabaseNameLabel), culture);

    public static string DatabaseNameMarkdown() => deployIISBehindNlbResources.Get(nameof (DatabaseNameMarkdown));

    public static string DatabaseNameMarkdown(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (DatabaseNameMarkdown), culture);

    public static string Description() => deployIISBehindNlbResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (Description), culture);

    public static string InlineSqlLabel() => deployIISBehindNlbResources.Get(nameof (InlineSqlLabel));

    public static string InlineSqlLabel(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (InlineSqlLabel), culture);

    public static string InlineSqlMarkdown() => deployIISBehindNlbResources.Get(nameof (InlineSqlMarkdown));

    public static string InlineSqlMarkdown(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (InlineSqlMarkdown), culture);

    public static string LoadBalancerLabel() => deployIISBehindNlbResources.Get(nameof (LoadBalancerLabel));

    public static string LoadBalancerLabel(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (LoadBalancerLabel), culture);

    public static string LoadBalancerMarkdown() => deployIISBehindNlbResources.Get(nameof (LoadBalancerMarkdown));

    public static string LoadBalancerMarkdown(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (LoadBalancerMarkdown), culture);

    public static string Name() => deployIISBehindNlbResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (Name), culture);

    public static string ResourceGroupNameLabel() => deployIISBehindNlbResources.Get(nameof (ResourceGroupNameLabel));

    public static string ResourceGroupNameLabel(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (ResourceGroupNameLabel), culture);

    public static string ResourceGroupNameMarkdown() => deployIISBehindNlbResources.Get(nameof (ResourceGroupNameMarkdown));

    public static string ResourceGroupNameMarkdown(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (ResourceGroupNameMarkdown), culture);

    public static string SqlFileLabel() => deployIISBehindNlbResources.Get(nameof (SqlFileLabel));

    public static string SqlFileLabel(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (SqlFileLabel), culture);

    public static string SqlFileMarkdown() => deployIISBehindNlbResources.Get(nameof (SqlFileMarkdown));

    public static string SqlFileMarkdown(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (SqlFileMarkdown), culture);

    public static string TargetMethodLabel() => deployIISBehindNlbResources.Get(nameof (TargetMethodLabel));

    public static string TargetMethodLabel(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (TargetMethodLabel), culture);

    public static string TargetMethodMarkdown() => deployIISBehindNlbResources.Get(nameof (TargetMethodMarkdown));

    public static string TargetMethodMarkdown(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (TargetMethodMarkdown), culture);

    public static string TaskTypeLabel() => deployIISBehindNlbResources.Get(nameof (TaskTypeLabel));

    public static string TaskTypeLabel(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (TaskTypeLabel), culture);

    public static string TaskTypeMarkdown() => deployIISBehindNlbResources.Get(nameof (TaskTypeMarkdown));

    public static string TaskTypeMarkdown(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (TaskTypeMarkdown), culture);

    public static string WebSiteNameLabel() => deployIISBehindNlbResources.Get(nameof (WebSiteNameLabel));

    public static string WebSiteNameLabel(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (WebSiteNameLabel), culture);

    public static string WebSiteNameMarkdown() => deployIISBehindNlbResources.Get(nameof (WebSiteNameMarkdown));

    public static string WebSiteNameMarkdown(CultureInfo culture) => deployIISBehindNlbResources.Get(nameof (WebSiteNameMarkdown), culture);
  }
}
