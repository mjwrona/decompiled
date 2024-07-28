// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.JobAgentSettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class JobAgentSettings : IJobAgentSettings, IApplicationSettings
  {
    public string this[string key] => AzureRoleUtil.Configuration.GetStringSetting(key, (string) null);

    public string ConfigDbConnectionString
    {
      get
      {
        string str = this[FrameworkServerConstants.ConfigDbConnectionStringKey];
        return !string.IsNullOrEmpty(str) ? str : throw new ApplicationException(FrameworkResources.JobAgentConfigurationError((object) FrameworkServerConstants.ConfigDbConnectionStringKey));
      }
    }

    public string ConfigDbUserId => this[FrameworkServerConstants.ConfigDbUserIdKey];

    public string ConfigDbPassword => this[FrameworkServerConstants.ConfigDbPasswordKey];

    public Guid InstanceId
    {
      get
      {
        string g = this[FrameworkServerConstants.InstanceIdKey];
        if (string.IsNullOrEmpty(g))
          return Guid.Empty;
        try
        {
          return new Guid(g);
        }
        catch (FormatException ex)
        {
          throw new ApplicationException(FrameworkResources.ConfigurationGuidError((object) FrameworkServerConstants.InstanceIdKey), (Exception) ex);
        }
      }
    }

    public TimeSpan MaximumStopTime => TimeSpan.FromMinutes(1.0);

    public TimeSpan ForceQueueCheckInterval => TimeSpan.FromMinutes(5.0);

    public override string ToString() => "[" + string.Join(", ", ((IEnumerable<PropertyInfo>) this.GetType().GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (x => x.GetIndexParameters().Length == 0)).Select<PropertyInfo, string>((Func<PropertyInfo, string>) (x => x.Name + "=" + x.GetValue((object) this)))) + "]";
  }
}
