// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Configuration.SharedAccessSignatureElement
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.ComponentModel;
using System.Configuration;

namespace Microsoft.Azure.NotificationHubs.Configuration
{
  public class SharedAccessSignatureElement : ConfigurationElement
  {
    private const int MinKeyNameSize = 0;
    private const int MinKeySize = 0;
    private ConfigurationPropertyCollection properties;

    internal SharedAccessSignatureElement()
    {
    }

    [ConfigurationProperty("keyName", IsRequired = true)]
    [StringValidator(MinLength = 0, MaxLength = 256)]
    public string KeyName
    {
      get => (string) this["keyName"];
      set => this["keyName"] = (object) value;
    }

    [ConfigurationProperty("key", IsRequired = true)]
    [StringValidator(MinLength = 0, MaxLength = 256)]
    public string Key
    {
      get => (string) this["key"];
      set => this["key"] = (object) value;
    }

    [ConfigurationProperty("tokenScope", IsRequired = false, DefaultValue = TokenScope.Entity)]
    [ServiceModelEnumValidator(typeof (TokenScopeHelper))]
    public TokenScope TokenScope
    {
      get => (TokenScope) this["tokenScope"];
      set => this["tokenScope"] = (object) value;
    }

    internal bool IsValid => !string.IsNullOrWhiteSpace(this.KeyName) && !string.IsNullOrWhiteSpace(this.Key);

    protected override ConfigurationPropertyCollection Properties
    {
      get
      {
        if (this.properties == null)
        {
          ConfigurationPropertyCollection properties = base.Properties;
          properties.Add(new ConfigurationProperty("key", typeof (string), (object) string.Empty, (TypeConverter) null, (ConfigurationValidatorBase) new StringValidator(0, 256), ConfigurationPropertyOptions.IsRequired));
          properties.Add(new ConfigurationProperty("keyName", typeof (string), (object) string.Empty, (TypeConverter) null, (ConfigurationValidatorBase) new StringValidator(0, 256), ConfigurationPropertyOptions.IsRequired));
          properties.Add(new ConfigurationProperty("tokenScope", typeof (TokenScope), (object) TokenScope.Entity, (TypeConverter) null, (ConfigurationValidatorBase) new ServiceModelEnumValidator(typeof (TokenScopeHelper)), ConfigurationPropertyOptions.None));
          this.properties = properties;
        }
        return this.properties;
      }
    }

    public void CopyFrom(SharedAccessSignatureElement source)
    {
      this.Key = source.Key;
      this.KeyName = source.KeyName;
      this.TokenScope = source.TokenScope;
    }
  }
}
