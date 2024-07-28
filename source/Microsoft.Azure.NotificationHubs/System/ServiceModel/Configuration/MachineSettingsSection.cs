// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Configuration.MachineSettingsSection
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.ComponentModel;
using System.Configuration;

namespace System.ServiceModel.Configuration
{
  internal class MachineSettingsSection : ConfigurationSection
  {
    private static bool enableLoggingKnownPii;
    private static bool hasInitialized;
    private static object syncRoot = new object();
    private const string enableLoggingKnownPiiKey = "enableLoggingKnownPii";
    private ConfigurationPropertyCollection properties;

    protected override ConfigurationPropertyCollection Properties
    {
      get
      {
        if (this.properties == null)
          this.properties = new ConfigurationPropertyCollection()
          {
            new ConfigurationProperty("enableLoggingKnownPii", typeof (bool), (object) false, (TypeConverter) null, (ConfigurationValidatorBase) null, ConfigurationPropertyOptions.None)
          };
        return this.properties;
      }
    }

    public static bool EnableLoggingKnownPii
    {
      get
      {
        if (!MachineSettingsSection.hasInitialized)
        {
          lock (MachineSettingsSection.syncRoot)
          {
            if (!MachineSettingsSection.hasInitialized)
            {
              MachineSettingsSection.enableLoggingKnownPii = (bool) ((ConfigurationElement) ConfigurationManager.GetSection("system.serviceModel/machineSettings"))["enableLoggingKnownPii"];
              MachineSettingsSection.hasInitialized = true;
            }
          }
        }
        return MachineSettingsSection.enableLoggingKnownPii;
      }
    }
  }
}
