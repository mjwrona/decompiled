// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Configuration.TokenProviderElement
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.ServiceModel.Configuration;

namespace Microsoft.Azure.NotificationHubs.Configuration
{
  public class TokenProviderElement : ClientCredentialsElement
  {
    private ConfigurationPropertyCollection properties;

    internal TokenProviderElement()
    {
    }

    [ConfigurationProperty("name", IsRequired = false, IsKey = true)]
    [StringValidator(MinLength = 0, InvalidCharacters = " ")]
    public string Name
    {
      get => (string) this["name"];
      set => this["name"] = (object) value;
    }

    [ConfigurationProperty("sharedSecret")]
    public SharedSecretElement SharedSecret => (SharedSecretElement) this["sharedSecret"];

    [ConfigurationProperty("windowsAuthentication")]
    public WindowsElement WindowsAuthentication => (WindowsElement) this["windowsAuthentication"];

    [ConfigurationProperty("sharedAccessSignature")]
    public SharedAccessSignatureElement SharedAccessSignature => (SharedAccessSignatureElement) this["sharedAccessSignature"];

    internal bool IsValid => this.SharedSecret.IsValid || this.WindowsAuthentication.IsValid || this.SharedAccessSignature.IsValid;

    protected override ConfigurationPropertyCollection Properties
    {
      get
      {
        if (this.properties == null)
        {
          ConfigurationPropertyCollection properties = base.Properties;
          properties.Add(new ConfigurationProperty("name", typeof (string), (object) null, (TypeConverter) null, (ConfigurationValidatorBase) new StringValidator(1, int.MaxValue, " "), ConfigurationPropertyOptions.IsKey));
          properties.Add(new ConfigurationProperty("sharedSecret", typeof (SharedSecretElement), (object) null, (TypeConverter) null, (ConfigurationValidatorBase) null, ConfigurationPropertyOptions.None));
          properties.Add(new ConfigurationProperty("windowsAuthentication", typeof (WindowsElement), (object) null, (TypeConverter) null, (ConfigurationValidatorBase) null, ConfigurationPropertyOptions.None));
          properties.Add(new ConfigurationProperty("sharedAccessSignature", typeof (SharedAccessSignatureElement), (object) null, (TypeConverter) null, (ConfigurationValidatorBase) null, ConfigurationPropertyOptions.None));
          this.properties = properties;
        }
        return this.properties;
      }
    }

    public override void CopyFrom(ServiceModelExtensionElement from)
    {
      TokenProviderElement tokenProviderElement = (TokenProviderElement) from;
      base.CopyFrom(from);
      this.SharedSecret.CopyFrom(tokenProviderElement.SharedSecret);
      this.WindowsAuthentication.CopyFrom(tokenProviderElement.WindowsAuthentication);
      this.SharedAccessSignature.CopyFrom(tokenProviderElement.SharedAccessSignature);
      this.Name = tokenProviderElement.Name;
    }

    internal TokenProvider CreateTokenProvider() => this.WindowsAuthentication != null && this.WindowsAuthentication.IsValid ? (this.WindowsAuthentication.UseDefaultCredentials ? TokenProvider.CreateWindowsTokenProvider(this.WindowsAuthentication.StsUris.Addresses) : TokenProvider.CreateWindowsTokenProvider(this.WindowsAuthentication.StsUris.Addresses, new NetworkCredential(this.WindowsAuthentication.UserName, this.WindowsAuthentication.Password, this.WindowsAuthentication.Domain))) : (this.SharedAccessSignature != null && this.SharedAccessSignature.IsValid ? TokenProvider.CreateSharedAccessSignatureTokenProvider(this.SharedAccessSignature.KeyName, this.SharedAccessSignature.Key, this.SharedAccessSignature.TokenScope) : TokenProvider.CreateSharedSecretTokenProvider(this.SharedSecret.IssuerName, this.SharedSecret.IssuerSecret, this.SharedSecret.TokenScope));
  }
}
