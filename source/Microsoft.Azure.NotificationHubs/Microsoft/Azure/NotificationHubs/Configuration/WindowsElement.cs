// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Configuration.WindowsElement
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Configuration;

namespace Microsoft.Azure.NotificationHubs.Configuration
{
  public class WindowsElement : ConfigurationElement
  {
    private ConfigurationPropertyCollection properties;

    internal WindowsElement()
    {
    }

    [ConfigurationProperty("useDefaultCredentials", IsRequired = false, DefaultValue = true)]
    public bool UseDefaultCredentials
    {
      get => (bool) this["useDefaultCredentials"];
      set => this["useDefaultCredentials"] = (object) value;
    }

    [ConfigurationProperty("userName", IsRequired = false)]
    public string UserName
    {
      get => (string) this["userName"];
      set => this["userName"] = (object) value;
    }

    [ConfigurationProperty("password", IsRequired = false)]
    public string Password
    {
      get => (string) this["password"];
      set => this["password"] = (object) value;
    }

    [ConfigurationProperty("domain", IsRequired = false)]
    public string Domain
    {
      get => (string) this["domain"];
      set => this["domain"] = (object) value;
    }

    [ConfigurationProperty("stsUris", IsRequired = true, IsDefaultCollection = false)]
    public virtual StsUriElementCollection StsUris => (StsUriElementCollection) this["stsUris"];

    internal bool IsValid
    {
      get
      {
        bool flag = this.UseDefaultCredentials && string.IsNullOrWhiteSpace(this.UserName) && string.IsNullOrWhiteSpace(this.Password) && string.IsNullOrWhiteSpace(this.Domain) || !this.UseDefaultCredentials && !string.IsNullOrWhiteSpace(this.UserName);
        return ((this.StsUris == null ? 0 : (this.StsUris.Count > 0 ? 1 : 0)) & (flag ? 1 : 0)) != 0;
      }
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get
      {
        if (this.properties == null)
        {
          ConfigurationPropertyCollection properties = base.Properties;
          properties.Add(new ConfigurationProperty("useDefaultCredentials", typeof (bool), (object) true));
          properties.Add(new ConfigurationProperty("userName", typeof (string)));
          properties.Add(new ConfigurationProperty("password", typeof (string)));
          properties.Add(new ConfigurationProperty("domain", typeof (string)));
          properties.Add(new ConfigurationProperty("stsUris", typeof (StsUriElementCollection)));
          this.properties = properties;
        }
        return this.properties;
      }
    }

    public void CopyFrom(WindowsElement source)
    {
      this.UseDefaultCredentials = source.UseDefaultCredentials;
      this.UserName = source.UserName;
      this.Password = source.Password;
      this.Domain = source.Domain;
    }
  }
}
