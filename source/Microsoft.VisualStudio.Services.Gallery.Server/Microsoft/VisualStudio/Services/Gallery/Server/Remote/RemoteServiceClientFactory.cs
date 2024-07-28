// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Remote.RemoteServiceClientFactory
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Commerce.Client;
using Microsoft.VisualStudio.Services.Commerce.WebApi;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote.Billing;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote.ExtensionManagement;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Remote
{
  public class RemoteServiceClientFactory : IRemoteServiceClientFactory
  {
    private readonly string CommerceGuid = "00000047-0000-8888-8000-000000000000";
    private const string s_area = "gallery";
    private const string s_layer = "clientFactory";

    public IExtensionManagementClient GetNewExtensionManagementClient(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid serviceOwner)
    {
      WrappedHttpClient<ExtensionManagementHttpClient> wrappedEmsClient = (WrappedHttpClient<ExtensionManagementHttpClient>) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        LocationHttpClient spsLocationClient = LocationServiceHelper.GetSpsLocationClient(requestContext, hostId);
        ServiceDefinition serviceDefinition1 = spsLocationClient.GetServiceDefinitionAsync("LocationService2", serviceOwner).SyncResult<ServiceDefinition>();
        if (serviceDefinition1 == null || serviceDefinition1.GetLocationMapping(AccessMappingConstants.ServerAccessMappingMoniker) == null)
        {
          foreach (ServiceDefinition serviceDefinition2 in spsLocationClient.GetServiceDefinitionsAsync().SyncResult<IEnumerable<ServiceDefinition>>())
          {
            if (serviceDefinition2.TryGetProperty("Microsoft.TeamFoundation.Location.CollectionName", out object _) && serviceDefinition2.GetLocationMapping(AccessMappingConstants.HostGuidAccessMappingMoniker) != null)
            {
              serviceDefinition1 = LocationServiceHelper.GetSpsLocationClient(requestContext, serviceDefinition2.Identifier).GetServiceDefinitionAsync("LocationService2", serviceOwner).SyncResult<ServiceDefinition>();
              break;
            }
          }
        }
        if (serviceDefinition1 != null)
        {
          LocationMapping locationMapping = serviceDefinition1.GetLocationMapping(AccessMappingConstants.HostGuidAccessMappingMoniker);
          if (locationMapping != null)
            wrappedEmsClient = new WrappedHttpClient<ExtensionManagementHttpClient>(this.CreateClientAtNonDeploymentLevel<ExtensionManagementHttpClient>(requestContext, new Uri(locationMapping.Location)));
        }
        if (wrappedEmsClient == null)
          throw new LocationMappingDoesNotExistException("LocationService2", serviceOwner.ToString(), AccessMappingConstants.ServerAccessMappingMoniker);
      }
      else
      {
        IVssRequestContext vssRequestContext = requestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, hostId, RequestContextType.UserContext);
        wrappedEmsClient = new WrappedHttpClient<ExtensionManagementHttpClient>(vssRequestContext.GetClient<ExtensionManagementHttpClient>(), vssRequestContext);
      }
      return (IExtensionManagementClient) new ExtensionManagementClient(wrappedEmsClient, new GalleryKPIHelpers());
    }

    public IBillingClient GetNewBillingClient(IVssRequestContext requestContext) => (IBillingClient) new BillingClient(requestContext.GetClient<BillingHttpClient>(), new GalleryKPIHelpers());

    public IBillingClient GetNewNonDeploymentLevelBillingClient(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      Uri accountLevelUri = this.GetAccountLevelUri(requestContext, hostId, ServiceInstanceTypes.SPS);
      return (IBillingClient) new BillingClient(this.CreateClientAtNonDeploymentLevel<BillingHttpClient>(requestContext, accountLevelUri), new GalleryKPIHelpers());
    }

    public OfferSubscriptionHttpClient GetNewNonDeploymentLevelCommerceClient(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      Uri accountLevelUri = this.GetAccountLevelUri(requestContext, hostId, new Guid(this.CommerceGuid));
      return this.CreateClientAtNonDeploymentLevel<OfferSubscriptionHttpClient>(requestContext, accountLevelUri);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We dont want the call to fail in case we encounter any type of failure while resolving Application ID using UrlHostResolutionService")]
    public Uri GetAccountLevelUri(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid instanceType)
    {
      Uri accountLevelUri = this.ResolveUrlFromHost(requestContext, hostId, instanceType);
      if (accountLevelUri == (Uri) null)
      {
        ISubscriptionService service = requestContext.GetService<ISubscriptionService>();
        try
        {
          ISubscriptionAccount subscriptionAccount = service.GetSubscriptionAccount(requestContext, AccountProviderNamespace.OnPremise, hostId);
          if (!string.IsNullOrEmpty(subscriptionAccount?.AccountName))
          {
            Guid collectionId;
            if (HostNameResolver.TryGetCollectionServiceHostId(requestContext, subscriptionAccount.AccountName, out collectionId))
              accountLevelUri = this.ResolveUrlFromHost(requestContext, collectionId, instanceType);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12061078, "gallery", "clientFactory", ex);
          throw;
        }
      }
      return accountLevelUri;
    }

    public OrganizationHttpClient GetOrganizationClient(
      IVssRequestContext requestContext,
      Guid organizationId)
    {
      OrganizationHttpClient organizationClient = (OrganizationHttpClient) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        Uri spsUriForHostId = PartitionedClientHelper.GetSpsUriForHostId(requestContext, organizationId);
        if (spsUriForHostId != (Uri) null)
          organizationClient = this.CreateClientAtNonDeploymentLevel<OrganizationHttpClient>(requestContext, spsUriForHostId);
      }
      return organizationClient;
    }

    public IExtensionManagementClient GetEMSClient(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid serviceOwner)
    {
      WrappedHttpClient<ExtensionManagementHttpClient> wrappedEmsClient = (WrappedHttpClient<ExtensionManagementHttpClient>) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        Uri hostUri = requestContext.GetService<IUrlHostResolutionService>().GetHostUri(requestContext, hostId, serviceOwner);
        if (hostUri != (Uri) null)
          wrappedEmsClient = new WrappedHttpClient<ExtensionManagementHttpClient>(this.CreateClientAtNonDeploymentLevel<ExtensionManagementHttpClient>(requestContext, hostUri));
      }
      return (IExtensionManagementClient) new ExtensionManagementClient(wrappedEmsClient, new GalleryKPIHelpers());
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We dont want the call to fail in case we encounter any type of failure while resolving Application ID using UrlHostResolutionService")]
    private Uri ResolveUrlFromHost(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid instanceType)
    {
      Uri uri = (Uri) null;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        uri = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, hostId, instanceType);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061078, "gallery", "clientFactory", ex);
      }
      return uri;
    }

    private T CreateClientAtNonDeploymentLevel<T>(IVssRequestContext requestContext, Uri baseUri) where T : VssHttpClientBase => (requestContext.ClientProvider as ICreateClient).CreateClient<T>(requestContext, baseUri, "", (ApiResourceLocationCollection) null);
  }
}
