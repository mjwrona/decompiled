// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.SourceProviderAttributesExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class SourceProviderAttributesExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.SourceProviderAttributes ToWebApiSourceProviderAttributes(
      this Microsoft.TeamFoundation.Build2.Server.SourceProviderAttributes srvSourceProviderAttributes)
    {
      if (srvSourceProviderAttributes == null)
        return (Microsoft.TeamFoundation.Build.WebApi.SourceProviderAttributes) null;
      Microsoft.TeamFoundation.Build.WebApi.SourceProviderAttributes providerAttributes = new Microsoft.TeamFoundation.Build.WebApi.SourceProviderAttributes()
      {
        Name = srvSourceProviderAttributes.Name,
        Availability = (Microsoft.TeamFoundation.Build.WebApi.SourceProviderAvailability) srvSourceProviderAttributes.Availability,
        IsExternal = srvSourceProviderAttributes.IsExternal,
        SupportedCapabilities = srvSourceProviderAttributes.SupportedCapabilities
      };
      if (srvSourceProviderAttributes.SupportedTriggers != null)
      {
        providerAttributes.SupportedTriggers = (IList<Microsoft.TeamFoundation.Build.WebApi.SupportedTrigger>) new List<Microsoft.TeamFoundation.Build.WebApi.SupportedTrigger>();
        foreach (Microsoft.TeamFoundation.Build2.Server.SupportedTrigger supportedTrigger in (IEnumerable<Microsoft.TeamFoundation.Build2.Server.SupportedTrigger>) srvSourceProviderAttributes.SupportedTriggers)
          providerAttributes.SupportedTriggers.Add(supportedTrigger.ToWebApiSupportedTrigger());
      }
      return providerAttributes;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.SupportedTrigger ToWebApiSupportedTrigger(
      this Microsoft.TeamFoundation.Build2.Server.SupportedTrigger srvSupportedTrigger)
    {
      if (srvSupportedTrigger == null)
        return (Microsoft.TeamFoundation.Build.WebApi.SupportedTrigger) null;
      Microsoft.TeamFoundation.Build.WebApi.SupportedTrigger supportedTrigger = new Microsoft.TeamFoundation.Build.WebApi.SupportedTrigger()
      {
        Type = (Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType) srvSupportedTrigger.Type,
        NotificationType = srvSupportedTrigger.NotificationType,
        DefaultPollingInterval = srvSupportedTrigger.DefaultPollingInterval
      };
      if (srvSupportedTrigger.SupportedCapabilities != null)
      {
        supportedTrigger.SupportedCapabilities = (IDictionary<string, Microsoft.TeamFoundation.Build.WebApi.SupportLevel>) new Dictionary<string, Microsoft.TeamFoundation.Build.WebApi.SupportLevel>();
        foreach (KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.SupportLevel> supportedCapability in (IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.SupportLevel>>) srvSupportedTrigger.SupportedCapabilities)
          supportedTrigger.SupportedCapabilities.Add(supportedCapability.Key, (Microsoft.TeamFoundation.Build.WebApi.SupportLevel) supportedCapability.Value);
      }
      return supportedTrigger;
    }
  }
}
