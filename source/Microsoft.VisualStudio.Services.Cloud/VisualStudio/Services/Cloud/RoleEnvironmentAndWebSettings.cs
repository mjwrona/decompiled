// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RoleEnvironmentAndWebSettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Web.Configuration;
using System.Web.Hosting;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RoleEnvironmentAndWebSettings : IApplicationSettings
  {
    private static readonly string s_area = "Configuration";
    private static readonly string s_layer = "WebApplicationSettings";

    public string this[string key]
    {
      get
      {
        string appSetting;
        if (!AzureRoleUtil.Configuration.Settings.TryGetValue(key, out appSetting) && HostingEnvironment.IsHosted)
          appSetting = WebConfigurationManager.AppSettings[key];
        return appSetting;
      }
    }

    public string ConfigDbConnectionString => this[FrameworkServerConstants.ConfigDbConnectionStringKey];

    public string ConfigDbUserId => this[FrameworkServerConstants.ConfigDbUserIdKey];

    public string ConfigDbPassword => this[FrameworkServerConstants.ConfigDbPasswordKey];

    public Guid InstanceId
    {
      get
      {
        string input = this[FrameworkServerConstants.InstanceIdKey];
        Guid result = Guid.Empty;
        if (!string.IsNullOrEmpty(input) && !Guid.TryParse(input, out result))
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, RoleEnvironmentAndWebSettings.s_area, RoleEnvironmentAndWebSettings.s_layer, "Can't parse InstanceId value ('{0}'} as a guid.", (object) input);
        return result;
      }
    }
  }
}
