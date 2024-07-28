// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuredLocationManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecuredLocationManager : IVssFrameworkService
  {
    private IInternalLocationService m_locationService;
    private IVssSecurityNamespace m_locationSecurity;
    private IVssSecurityNamespace m_deploymentLocationSecurity;
    private const string ModifyLocationDataInHostedToken = "ModifyLocationDataInHosted";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_locationSecurity = systemRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(systemRequestContext, FrameworkSecurity.LocationNamespaceId);
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
        this.m_deploymentLocationSecurity = vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.LocationNamespaceId);
      }
      this.m_locationService = systemRequestContext.GetService<IInternalLocationService>();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void SaveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      ArgumentUtility.CheckForNull<IEnumerable<ServiceDefinition>>(serviceDefinitions, nameof (serviceDefinitions));
      this.CheckHostedWritePermission(requestContext);
      foreach (ServiceDefinition serviceDefinition in serviceDefinitions)
      {
        ArgumentUtility.CheckForNull<ServiceDefinition>(serviceDefinition, "serviceDefinition");
        this.m_locationSecurity.CheckPermission(requestContext, LocationHelper.CreateSecurityToken(serviceDefinition.ServiceType, serviceDefinition.Identifier), 2, false);
      }
      this.m_locationService.SaveServiceDefinitions(requestContext, serviceDefinitions);
    }

    public IEnumerable<AccessMapping> GetAccessMappings(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        this.m_locationSecurity.CheckPermission(requestContext, FrameworkSecurity.AccessMappingsToken, 1);
      return this.m_locationService.GetAccessMappings(requestContext);
    }

    public AccessMapping DetermineAccessMapping(IVssRequestContext requestContext)
    {
      this.m_locationSecurity.CheckPermission(requestContext, FrameworkSecurity.AccessMappingsToken, 1);
      return this.m_locationService.DetermineAccessMapping(requestContext);
    }

    public long GetLastChangeId(IVssRequestContext requestContext) => this.m_locationService.GetLastChangeId(requestContext);

    public void RemoveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      ArgumentUtility.CheckForNull<IEnumerable<ServiceDefinition>>(serviceDefinitions, nameof (serviceDefinitions));
      this.CheckHostedWritePermission(requestContext);
      foreach (ServiceDefinition serviceDefinition in serviceDefinitions)
      {
        ArgumentUtility.CheckForNull<ServiceDefinition>(serviceDefinition, "serviceDefinition");
        this.m_locationSecurity.CheckPermission(requestContext, LocationHelper.CreateSecurityToken(serviceDefinition.ServiceType, serviceDefinition.Identifier), 2, false);
      }
      this.m_locationService.RemoveServiceDefinitions(requestContext, serviceDefinitions);
    }

    public void ConfigureAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping,
      bool makeDefault)
    {
      this.m_locationSecurity.CheckPermission(requestContext, FrameworkSecurity.AccessMappingsToken, 2, false);
      this.m_locationService.ConfigureAccessMapping(requestContext, accessMapping, makeDefault);
    }

    public void SetDefaultAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      this.m_locationSecurity.CheckPermission(requestContext, FrameworkSecurity.AccessMappingsToken, 2, false);
      this.m_locationService.SetDefaultAccessMapping(requestContext, accessMapping);
    }

    public void RemoveAccessMapping(IVssRequestContext requestContext, AccessMapping accessMapping)
    {
      this.m_locationSecurity.CheckPermission(requestContext, FrameworkSecurity.AccessMappingsToken, 2, false);
      this.m_locationService.RemoveAccessMapping(requestContext, accessMapping);
    }

    public IEnumerable<ServiceDefinition> QueryServices(
      IVssRequestContext requestContext,
      string serviceType = null)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        this.m_locationSecurity.CheckPermission(requestContext, FrameworkSecurity.ServiceDefinitionsToken, 1);
      return this.m_locationService.FindServiceDefinitions(requestContext, serviceType);
    }

    internal IEnumerable<ServiceDefinition> QueryNonInheritedServiceDefinitions(
      IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        this.m_locationSecurity.CheckPermission(requestContext, FrameworkSecurity.ServiceDefinitionsToken, 1);
      return this.m_locationService.FindNonInheritedDefinitions(requestContext);
    }

    public ServiceDefinition QueryService(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      bool allowFaultIn = true,
      bool previewFaultIn = false)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        this.m_locationSecurity.CheckPermission(requestContext, FrameworkSecurity.ServiceDefinitionsToken, 1);
      return !allowFaultIn ? this.m_locationService.FindServiceDefinition(requestContext, serviceType, identifier) : this.m_locationService.FindServiceDefinitionWithFaultIn(requestContext, serviceType, identifier, previewFaultIn);
    }

    public void Connect(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      this.m_locationSecurity.CheckPermission(requestContext, FrameworkSecurity.LocationNamespaceRootToken, 1);
    }

    private void CheckHostedWritePermission(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      this.m_deploymentLocationSecurity.CheckPermission(requestContext.To(TeamFoundationHostType.Deployment), "ModifyLocationDataInHosted", 2);
    }
  }
}
