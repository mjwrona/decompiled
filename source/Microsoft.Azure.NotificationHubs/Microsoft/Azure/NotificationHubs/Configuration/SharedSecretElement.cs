// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Configuration.SharedSecretElement
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.ComponentModel;
using System.Configuration;

namespace Microsoft.Azure.NotificationHubs.Configuration
{
  public class SharedSecretElement : ConfigurationElement
  {
    private const int MinIssuerNameSize = 0;
    private const int MinIssuerSecretSize = 0;
    private ConfigurationPropertyCollection properties;

    internal SharedSecretElement()
    {
    }

    [ConfigurationProperty("issuerName", IsRequired = true)]
    [StringValidator(MinLength = 0, MaxLength = 128)]
    public string IssuerName
    {
      get => (string) this["issuerName"];
      set => this["issuerName"] = (object) value;
    }

    [ConfigurationProperty("issuerSecret", IsRequired = true)]
    [StringValidator(MinLength = 0, MaxLength = 128)]
    public string IssuerSecret
    {
      get => (string) this["issuerSecret"];
      set => this["issuerSecret"] = (object) value;
    }

    [ConfigurationProperty("tokenScope", IsRequired = false, DefaultValue = TokenScope.Entity)]
    [ServiceModelEnumValidator(typeof (TokenScopeHelper))]
    public TokenScope TokenScope
    {
      get => (TokenScope) this["tokenScope"];
      set => this["tokenScope"] = (object) value;
    }

    internal bool IsValid => !string.IsNullOrWhiteSpace(this.IssuerName) && !string.IsNullOrWhiteSpace(this.IssuerSecret);

    protected override ConfigurationPropertyCollection Properties
    {
      get
      {
        if (this.properties == null)
        {
          ConfigurationPropertyCollection properties = base.Properties;
          properties.Add(new ConfigurationProperty("issuerName", typeof (string), (object) string.Empty, (TypeConverter) null, (ConfigurationValidatorBase) new StringValidator(0, 128), ConfigurationPropertyOptions.IsRequired));
          properties.Add(new ConfigurationProperty("issuerSecret", typeof (string), (object) string.Empty, (TypeConverter) null, (ConfigurationValidatorBase) new StringValidator(0, 128), ConfigurationPropertyOptions.IsRequired));
          properties.Add(new ConfigurationProperty("tokenScope", typeof (TokenScope), (object) TokenScope.Entity, (TypeConverter) null, (ConfigurationValidatorBase) new ServiceModelEnumValidator(typeof (TokenScopeHelper)), ConfigurationPropertyOptions.None));
          this.properties = properties;
        }
        return this.properties;
      }
    }

    public void CopyFrom(SharedSecretElement source)
    {
      this.IssuerName = source.IssuerName;
      this.IssuerSecret = source.IssuerSecret;
      this.TokenScope = source.TokenScope;
    }
  }
}
