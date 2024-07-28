// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Location.TeamFoundationConnectionService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.Core.Location
{
  public sealed class TeamFoundationConnectionService : IVssFrameworkService
  {
    private Guid m_catalogResourceId;
    private Guid m_parentCatalogResourceId;

    internal TeamFoundationConnectionService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.RefreshCatalogResourceIds(systemRequestContext);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ConnectionData Connect(
      IVssRequestContext requestContext,
      int connectOptions,
      int lastChangeId,
      TeamFoundationSupportedFeatures supportedFeatures)
    {
      SecuredLocationManager service1 = requestContext.GetService<SecuredLocationManager>();
      ILocationService service2 = requestContext.GetService<ILocationService>();
      service1.Connect(requestContext);
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
      TeamFoundationIdentity authenticatedUser = IdentityUtil.Convert(authenticatedIdentity);
      TeamFoundationIdentity authorizedUser = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      if (authenticatedUser == null)
        throw new IdentityNotFoundException(authenticatedIdentity?.Descriptor);
      if (authorizedUser == null)
        throw new IdentityNotFoundException(requestContext.UserContext);
      string serverVersion = (string) null;
      TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
      if (executionEnvironment.IsOnPremisesDeployment)
        serverVersion = requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.ServiceLevel;
      LocationServiceData locationServiceData;
      if ((connectOptions & 1) == 1)
      {
        locationServiceData = this.GetLocationServiceData(requestContext, lastChangeId);
      }
      else
      {
        long lastChangeId1 = service1.GetLastChangeId(requestContext);
        locationServiceData = new LocationServiceData(lastChangeId1, service2.GetDefaultAccessMapping(requestContext).Moniker, (long) lastChangeId == lastChangeId1, LocationServiceHelper.GetClientCacheTimeToLive(requestContext));
      }
      authenticatedUser.PrepareForWebServiceSerialization(supportedFeatures);
      authorizedUser.PrepareForWebServiceSerialization(supportedFeatures);
      ServerCapabilities serverCapabilities = ServerCapabilities.None;
      executionEnvironment = requestContext.ExecutionEnvironment;
      if (executionEnvironment.IsHostedDeployment)
        serverCapabilities |= ServerCapabilities.Hosted;
      if (requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationMailService>().Enabled)
        serverCapabilities |= ServerCapabilities.Email;
      Guid instanceId;
      Guid catalogResourceId;
      this.GetVirtualValues(requestContext, out instanceId, out catalogResourceId);
      return new ConnectionData(instanceId, catalogResourceId, authenticatedUser, authorizedUser, LocationServiceHelper.GetWebApplicationRelativeDirectory(requestContext), locationServiceData, serverCapabilities, serverVersion);
    }

    public LocationServiceData GetLocationServiceData(
      IVssRequestContext requestContext,
      int lastChangeId)
    {
      SecuredLocationManager service1 = requestContext.GetService<SecuredLocationManager>();
      ILocationService service2 = requestContext.GetService<ILocationService>();
      if ((long) lastChangeId == service1.GetLastChangeId(requestContext))
        return new LocationServiceData(service1.GetLastChangeId(requestContext), (IEnumerable<ServiceDefinition>) null, LocationCompatUtil.Convert(requestContext, service1.GetAccessMappings(requestContext)), service2.GetDefaultAccessMapping(requestContext).Moniker, true, LocationServiceHelper.GetClientCacheTimeToLive(requestContext), requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection));
      IEnumerable<ServiceDefinition> serviceDefinitions = LocationCompatUtil.Convert(requestContext, service1.QueryServices(requestContext));
      return new LocationServiceData(service1.GetLastChangeId(requestContext), serviceDefinitions, LocationCompatUtil.Convert(requestContext, service1.GetAccessMappings(requestContext)), service2.GetDefaultAccessMapping(requestContext).Moniker, false, LocationServiceHelper.GetClientCacheTimeToLive(requestContext), requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection));
    }

    private void RefreshCatalogResourceIds(IVssRequestContext requestContext)
    {
      List<CatalogNode> catalogNodeList1;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        ITeamFoundationCatalogService service = vssRequestContext.GetService<ITeamFoundationCatalogService>();
        catalogNodeList1 = service.QueryNodes(vssRequestContext, CatalogRoots.OrganizationalPath + CatalogConstants.FullRecurseStars, CatalogResourceTypes.ProjectCollection, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
        {
          new KeyValuePair<string, string>("InstanceId", requestContext.ServiceHost.InstanceId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture))
        });
        List<CatalogNode> catalogNodeList2 = service.QueryNodes(vssRequestContext, CatalogRoots.OrganizationalPath + CatalogConstants.SingleRecurseStar, CatalogResourceTypes.TeamFoundationServerInstance);
        if (catalogNodeList2 != null && catalogNodeList2.Count > 0)
          this.m_parentCatalogResourceId = catalogNodeList2[0].ResourceIdentifier;
      }
      else
        catalogNodeList1 = requestContext.GetService<ITeamFoundationCatalogService>().QueryNodes(requestContext, CatalogRoots.OrganizationalPath + CatalogConstants.SingleRecurseStar, CatalogResourceTypes.TeamFoundationServerInstance);
      this.m_catalogResourceId = Guid.Empty;
      if (catalogNodeList1 == null || catalogNodeList1.Count <= 0)
        return;
      this.m_catalogResourceId = catalogNodeList1[0].ResourceIdentifier;
    }

    private void GetVirtualValues(
      IVssRequestContext requestContext,
      out Guid instanceId,
      out Guid catalogResourceId)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.RelativePath().StartsWith(LocationServiceConstants.ApplicationLocationServiceRelativePath, StringComparison.Ordinal))
      {
        instanceId = this.m_parentCatalogResourceId;
        catalogResourceId = this.m_parentCatalogResourceId;
      }
      else
      {
        instanceId = requestContext.ServiceHost.InstanceId;
        catalogResourceId = this.m_catalogResourceId;
      }
    }
  }
}
