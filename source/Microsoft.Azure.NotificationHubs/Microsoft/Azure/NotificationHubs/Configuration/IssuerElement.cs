// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Configuration.IssuerElement
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.ComponentModel;
using System.Configuration;

namespace Microsoft.Azure.NotificationHubs.Configuration
{
  public class IssuerElement : ConfigurationElement
  {
    private ConfigurationPropertyCollection properties;

    internal IssuerElement()
    {
    }

    [ConfigurationProperty("issuerAddress", DefaultValue = "")]
    [StringValidator(MinLength = 0, MaxLength = 2048)]
    public string Address
    {
      get => (string) this["issuerAddress"];
      set => this["issuerAddress"] = (object) value;
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get
      {
        if (this.properties == null)
        {
          ConfigurationPropertyCollection properties = base.Properties;
          properties.Add(new ConfigurationProperty("address", typeof (string), (object) string.Empty, (TypeConverter) null, (ConfigurationValidatorBase) new StringValidator(0, 2048), ConfigurationPropertyOptions.None));
          this.properties = properties;
        }
        return this.properties;
      }
    }

    public void CopyFrom(ConfigurationElement from) => this.Address = ((IssuerElement) from).Address;
  }
}
