// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebApplicationSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Web.Configuration;
using System.Web.Hosting;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class WebApplicationSettings : IApplicationSettings
  {
    private static readonly string s_area = "Configuration";
    private static readonly string s_layer = nameof (WebApplicationSettings);

    public string this[string key] => HostingEnvironment.IsHosted ? WebConfigurationManager.AppSettings[key] : (string) null;

    public string ConfigDbConnectionString => this[FrameworkServerConstants.ApplicationDatabaseAppSettingsKey];

    public string ConfigDbUserId => this[FrameworkServerConstants.ApplicationDatabaseSqlUserKey];

    public string ConfigDbPassword => this[FrameworkServerConstants.ApplicationDatabaseSqlPasswordKey];

    public Guid InstanceId
    {
      get
      {
        string input = this[FrameworkServerConstants.ApplicationIdAppSettingsKey];
        Guid result = Guid.Empty;
        if (!string.IsNullOrEmpty(input))
        {
          if (!Guid.TryParse(input, out result))
            TeamFoundationTracingService.TraceRaw(60021, TraceLevel.Error, WebApplicationSettings.s_area, WebApplicationSettings.s_layer, "Can't parse InstanceId value ('{0}'} as a guid.", (object) input);
          else if (result == Guid.Empty)
            TeamFoundationTracingService.TraceRaw(60022, TraceLevel.Warning, WebApplicationSettings.s_area, WebApplicationSettings.s_layer, "InstanceId was specified as Guid.Empty");
        }
        else
          TeamFoundationTracingService.TraceRaw(60023, TraceLevel.Warning, WebApplicationSettings.s_area, WebApplicationSettings.s_layer, "InstanceId was not specified");
        return result;
      }
    }
  }
}
