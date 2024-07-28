// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TswaClientHyperlinkService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Client
{
  public sealed class TswaClientHyperlinkService : TswaHyperlinkBuilder, ITfsConnectionObject
  {
    private ILocationService m_locationService;
    private IRegistration m_registrationService;

    private TswaClientHyperlinkService()
    {
    }

    void ITfsConnectionObject.Initialize(TfsConnection server)
    {
      if (!(server is TfsConfigurationServer configurationServer))
      {
        TfsTeamProjectCollection projectCollection = server as TfsTeamProjectCollection;
        this.CollectionId = projectCollection.InstanceId;
        configurationServer = projectCollection.ConfigurationServer;
      }
      if (configurationServer == null)
        this.m_registrationService = server.GetService<IRegistration>();
      else
        this.m_locationService = configurationServer.GetService<ILocationService>();
    }

    protected override Uri GetUrl(string serviceType, string accessMappingMoniker)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceType, nameof (serviceType));
      Uri uri = (Uri) null;
      if (this.m_registrationService != null)
      {
        ServiceInterface serviceInterface = this.GetServiceInterface(serviceType);
        if (serviceInterface != null)
          uri = new Uri(serviceInterface.Url);
      }
      else
      {
        ServiceDefinition serviceDefinition = this.GetServiceDefinition(serviceType);
        if (serviceDefinition != null)
        {
          AccessMapping accessMapping = (AccessMapping) null;
          if (!string.IsNullOrEmpty(accessMappingMoniker))
            accessMapping = this.m_locationService.GetAccessMapping(accessMappingMoniker);
          return new Uri(this.m_locationService.LocationForAccessMapping(serviceDefinition, accessMapping));
        }
      }
      return !(uri == (Uri) null) ? uri : throw new NotSupportedException(TFCommonResources.TeamSystemWebAccessUrlIsMissing((object) serviceType));
    }

    private ServiceDefinition GetServiceDefinition(string serviceType) => this.m_locationService.FindServiceDefinitionsByToolType("TSWebAccess").FirstOrDefault<ServiceDefinition>((Func<ServiceDefinition, bool>) (sd => string.Equals(sd.ServiceType, serviceType, StringComparison.OrdinalIgnoreCase)));

    private ServiceInterface GetServiceInterface(string serviceType)
    {
      ServiceInterface serviceInterface = (ServiceInterface) null;
      if (this.m_registrationService != null)
      {
        RegistrationEntry[] registrationEntries = this.m_registrationService.GetRegistrationEntries("TSWebAccess");
        if (registrationEntries.Length != 0)
          serviceInterface = ((IEnumerable<ServiceInterface>) registrationEntries[0].ServiceInterfaces).FirstOrDefault<ServiceInterface>((Func<ServiceInterface, bool>) (si => VssStringComparer.ServiceInterface.Equals(si.Name, serviceType)));
      }
      return serviceInterface;
    }
  }
}
