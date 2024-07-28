// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Configuration.TransportClientEndpointBehaviorElement
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.ComponentModel;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Microsoft.Azure.NotificationHubs.Configuration
{
  public class TransportClientEndpointBehaviorElement : BehaviorExtensionElement
  {
    private ConfigurationPropertyCollection properties;

    [ConfigurationProperty("tokenProvider")]
    public TokenProviderElement TokenProvider => (TokenProviderElement) this["tokenProvider"];

    public override Type BehaviorType => typeof (TransportClientEndpointBehavior);

    protected override ConfigurationPropertyCollection Properties
    {
      get
      {
        if (this.properties == null)
        {
          ConfigurationPropertyCollection properties = base.Properties;
          properties.Add(new ConfigurationProperty("tokenProvider", typeof (TokenProviderElement), (object) null, (TypeConverter) null, (ConfigurationValidatorBase) null, ConfigurationPropertyOptions.None));
          this.properties = properties;
        }
        return this.properties;
      }
    }

    public override void CopyFrom(ServiceModelExtensionElement from)
    {
      base.CopyFrom(from);
      this.TokenProvider.CopyFrom((ServiceModelExtensionElement) ((TransportClientEndpointBehaviorElement) from).TokenProvider);
    }

    protected override object CreateBehavior()
    {
      TransportClientEndpointBehavior behavior = new TransportClientEndpointBehavior();
      if (this.TokenProvider.IsValid)
      {
        behavior.TokenProvider = this.TokenProvider.CreateTokenProvider();
        behavior.CredentialType = TransportClientCredentialType.TokenProvider;
      }
      else
        behavior.CredentialType = TransportClientCredentialType.Unauthenticated;
      return (object) behavior;
    }
  }
}
