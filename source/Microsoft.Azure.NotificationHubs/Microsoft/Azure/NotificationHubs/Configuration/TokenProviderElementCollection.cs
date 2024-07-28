// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Configuration.TokenProviderElementCollection
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using System.Configuration;

namespace Microsoft.Azure.NotificationHubs.Configuration
{
  [ConfigurationCollection(typeof (TokenProviderElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
  public sealed class TokenProviderElementCollection : ConfigurationElementCollection
  {
    protected override ConfigurationElement CreateNewElement() => (ConfigurationElement) new TokenProviderElement();

    protected override object GetElementKey(ConfigurationElement element)
    {
      TokenProviderElement tokenProviderElement = (TokenProviderElement) element;
      return tokenProviderElement != null && !string.IsNullOrEmpty(tokenProviderElement.Name) ? (object) tokenProviderElement.Name : throw new ConfigurationErrorsException(SRCore.NullOrEmptyConfigurationAttribute((object) "name", (object) "tokenProvider"));
    }

    public TokenProviderElement this[string name] => (TokenProviderElement) this.BaseGet((object) name);
  }
}
