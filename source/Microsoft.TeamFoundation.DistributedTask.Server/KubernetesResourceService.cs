// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.KubernetesResourceService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.Converters;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class KubernetesResourceService : 
    EnvironmentResourceService<KubernetesResource>,
    IKubernetesResourceService,
    IEnvironmentResourceService<KubernetesResource>,
    IVssFrameworkService
  {
    private static readonly string Collection = nameof (Collection);
    private ServiceEndpointSecurity m_serviceEndpointSecurity;

    public KubernetesResourceService() => this.m_serviceEndpointSecurity = new ServiceEndpointSecurity();

    protected KubernetesResourceService(
      Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity> getCurrentUserIdentity,
      Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef> toIdentityRef,
      ServiceEndpointSecurity serviceEndpointSecurity)
      : base(getCurrentUserIdentity, toIdentityRef)
    {
      this.m_serviceEndpointSecurity = serviceEndpointSecurity;
    }

    public async Task<KubernetesResource> AddKubernetesResourceAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      KubernetesResourceCreateParameters createParameters)
    {
      KubernetesResourceService kubernetesResourceService = this;
      KubernetesResource kubernetesResource1;
      using (new MethodScope(requestContext, kubernetesResourceService.c_layer, nameof (AddKubernetesResourceAsync)))
      {
        if (createParameters == null)
          throw new KubernetesResourceException(TaskResources.EnvironmentResourceCreationFailed((object) TaskResources.KubernetesResourceMissingInputs()));
        if (createParameters is KubernetesResourceCreateParametersNewEndpoint && !kubernetesResourceService.IsEnvironmentOwnedServiceConnectionsFFEnabled(requestContext))
          throw new NotSupportedException();
        kubernetesResourceService.m_securityProvider.CheckManagePermissions(requestContext, projectId, environmentId);
        KubernetesResource resource = createParameters.ToResource(environmentId);
        kubernetesResourceService.ValidateKubernetesResourceCreateParameters(requestContext, projectId, resource);
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint = (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint) null;
        if (createParameters is KubernetesResourceCreateParametersNewEndpoint parametersNewEndpoint)
        {
          endpoint = kubernetesResourceService.CreateKubernetesServiceConnection(requestContext, projectId, parametersNewEndpoint.Endpoint.ToServiceEndpoint());
          resource.ServiceEndpointId = endpoint.Id;
        }
        KubernetesResource kubernetesResource2;
        try
        {
          kubernetesResource2 = await kubernetesResourceService.AddEnvironmentResourceAsync(requestContext, projectId, resource);
          requestContext.TraceInfo(10015216, "EnvironmentService", "Adding Kubernetes resource with Id: {0}.", (object) kubernetesResource2.Id);
        }
        catch (Exception ex)
        {
          if (endpoint != null && endpoint.Id != Guid.Empty)
            kubernetesResourceService.DeleteKubernetesServiceConnection(requestContext, projectId, endpoint.Id);
          throw new KubernetesResourceException(TaskResources.EnvironmentResourceCreationFailed((object) ex.Message));
        }
        kubernetesResource1 = kubernetesResource2;
      }
      return kubernetesResource1;
    }

    public KubernetesResource UpdateKubernetesResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      KubernetesResourcePatchParameters patchRequest)
    {
      using (new MethodScope(requestContext, this.c_layer, nameof (UpdateKubernetesResource)))
      {
        this.m_securityProvider.CheckManagePermissions(requestContext, projectId, environmentId);
        this.ValidateKuberntesResourcePatchParameters(requestContext, patchRequest);
        KubernetesResource environmentResource = this.GetEnvironmentResource(requestContext, projectId, environmentId, patchRequest.ResourceId);
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint = this.ExecuteElevatedOperation(requestContext, projectId, environmentResource.ServiceEndpointId, KubernetesResourceElevatedOperation.GetServiceConnection);
        requestContext.TraceInfo(10015217, "EnvironmentService", "Update Kubernetes resource with Id: {0}.", (object) environmentResource.Id);
        if (endpoint == null)
          throw new KubernetesResourceException(TaskResources.KubernetesResourceUpdateFailed((object) environmentResource.ServiceEndpointId));
        this.UpdateKubernetesServiceConnection(requestContext, projectId, endpoint, patchRequest.ProviderType, patchRequest.AuthorizationParameters);
        return environmentResource;
      }
    }

    public void DeleteKubernetesResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int resourceId)
    {
      this.m_securityProvider.CheckManagePermissions(requestContext, projectId, environmentId);
      requestContext.TraceInfo(10015218, "EnvironmentService", "Delete Kubernetes resource with Id: {0}.", (object) resourceId);
      if (this.IsEnvironmentOwnedServiceConnectionsFFEnabled(requestContext))
      {
        KubernetesResource environmentResource = this.GetEnvironmentResource(requestContext, projectId, environmentId, resourceId);
        this.DeleteKubernetesServiceConnection(requestContext, projectId, environmentResource.ServiceEndpointId);
      }
      this.DeleteEnvironmentResource(requestContext, projectId, environmentId, resourceId);
    }

    public void DeleteKubernetesResources(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId)
    {
      this.m_securityProvider.CheckManagePermissions(requestContext, projectId, environmentId);
      if (this.IsEnvironmentOwnedServiceConnectionsFFEnabled(requestContext))
      {
        EnvironmentInstance environment = requestContext.GetService<IEnvironmentService>().GetEnvironmentById(requestContext, projectId, environmentId, includeResourceReferences: true);
        if (environment != null && environment.Resources != null && environment.Resources.Any<EnvironmentResourceReference>())
        {
          IEnumerable<int> ints = environment.Resources.Where<EnvironmentResourceReference>((Func<EnvironmentResourceReference, bool>) (x => x.Type == EnvironmentResourceType.Kubernetes)).Select<EnvironmentResourceReference, int>((Func<EnvironmentResourceReference, int>) (x => x.Id));
          if (ints.Any<int>())
          {
            IList<KubernetesResource> environmentResourcesById = this.GetEnvironmentResourcesById(requestContext, projectId, environmentId, ints);
            if (environmentResourcesById.Any<KubernetesResource>())
            {
              IEnumerable<Guid> endpointIds = environmentResourcesById.Select<KubernetesResource, Guid>((Func<KubernetesResource, Guid>) (x => x.ServiceEndpointId));
              Dictionary<Guid, KubernetesResource> serviceEndpointToResourceMap = environmentResourcesById.ToDictionary<KubernetesResource, Guid>((Func<KubernetesResource, Guid>) (x => x.ServiceEndpointId));
              IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> serviceEndpointList = this.ExecuteElevatedOperation(requestContext, projectId, endpointIds, KubernetesResourceElevatedOperation.QueryEnvironmentOwnedServiceConnections);
              if (serviceEndpointList.Any<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>())
                serviceEndpointList.ForEach<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Action<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) (endpoint =>
                {
                  try
                  {
                    this.ExecuteElevatedOperation(requestContext, projectId, endpoint.Id, KubernetesResourceElevatedOperation.DeleteServiceConnection);
                  }
                  catch (Exception ex)
                  {
                    throw new KubernetesResourceException(TaskResources.KubernetesResourcesDeletionFailed((object) environment.Name, (object) serviceEndpointToResourceMap[endpoint.Id].Name, (object) ex.Message));
                  }
                }));
            }
          }
        }
      }
      this.DeleteEnvironmentResources(requestContext, projectId, environmentId);
    }

    protected override IEnvironmentResourceComponent<KubernetesResource> GetResourceComponent(
      IVssRequestContext requestContext)
    {
      return (IEnvironmentResourceComponent<KubernetesResource>) requestContext.CreateComponent<KubernetesResourceComponent>();
    }

    protected override async Task AuthorizePipelineResourceAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      KubernetesResource resource)
    {
      await requestContext.GetService<IPipelineResourceAuthorizationProxyService>().InheritAuthorizationPolicyFromEnvironmentAsync(requestContext, projectId, resource.EnvironmentReference.Id, resource.ServiceEndpointId.ToString(), typeof (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint).ToString());
    }

    private Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint CreateKubernetesServiceConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint)
    {
      int num = this.IsEnvironmentOwnedServiceConnectionsFFEnabled(requestContext) ? 1 : 0;
      if (num != 0)
        endpoint.Owner = ServiceEndpointOwner.Environment;
      endpoint = this.ExecuteElevatedOperation(requestContext, projectId, endpoint, KubernetesResourceElevatedOperation.CreateServiceConnection);
      if (num != 0)
        this.ExecuteElevatedOperation(requestContext, projectId, endpoint, KubernetesResourceElevatedOperation.RemoveServiceConnectionUserPermissions);
      return endpoint;
    }

    private void UpdateKubernetesServiceConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      string providerType,
      IDictionary<string, string> authorizationParameters)
    {
      if (string.Equals(providerType, "CustomProvider", StringComparison.OrdinalIgnoreCase))
      {
        string str1;
        if (authorizationParameters.TryGetValue("apiToken", out str1) && !string.IsNullOrEmpty(str1))
          endpoint.Authorization.Parameters["apiToken"] = str1;
        string str2;
        if (authorizationParameters.TryGetValue("serviceAccountCertificate", out str2) && !string.IsNullOrEmpty(str2))
          endpoint.Authorization.Parameters["serviceAccountCertificate"] = str2;
        string str3;
        if (authorizationParameters.TryGetValue("acceptUntrustedCerts", out str3) && !string.IsNullOrEmpty(str3))
          endpoint.Data["acceptUntrustedCerts"] = str3;
      }
      else if (string.Equals(providerType, "AzureKubernetesServiceProvider", StringComparison.OrdinalIgnoreCase))
        throw new NotSupportedException();
      this.ExecuteElevatedOperation(requestContext, projectId, endpoint, KubernetesResourceElevatedOperation.UpdateServiceConnection);
    }

    private void DeleteKubernetesServiceConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid serviceEndpointId)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = this.ExecuteElevatedOperation(requestContext, projectId, serviceEndpointId, KubernetesResourceElevatedOperation.GetServiceConnection);
      if (serviceEndpoint == null || !string.Equals(serviceEndpoint.Owner, ServiceEndpointOwner.Environment, StringComparison.OrdinalIgnoreCase))
        return;
      this.ExecuteElevatedOperation(requestContext, projectId, serviceEndpointId, KubernetesResourceElevatedOperation.DeleteServiceConnection);
    }

    private void ValidateKuberntesResourcePatchParameters(
      IVssRequestContext requestContext,
      KubernetesResourcePatchParameters patchRequest)
    {
      ArgumentUtility.CheckForNull<KubernetesResourcePatchParameters>(patchRequest, nameof (patchRequest));
      ArgumentUtility.CheckForNull<string>(patchRequest.ProviderType, "ProviderType");
      if (!string.Equals(patchRequest.ProviderType, "CustomProvider", StringComparison.OrdinalIgnoreCase) && !string.Equals(patchRequest.ProviderType, "AzureKubernetesServiceProvider", StringComparison.OrdinalIgnoreCase))
        throw new KubernetesResourceException(TaskResources.KubernetesResourceInvalidProvider((object) patchRequest.ProviderType));
    }

    private Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint ExecuteElevatedOperation(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      KubernetesResourceElevatedOperation operation)
    {
      IServiceEndpointService2 service = requestContext.GetService<IServiceEndpointService2>();
      switch (operation)
      {
        case KubernetesResourceElevatedOperation.CreateServiceConnection:
          endpoint = service.CreateServiceEndpoint(requestContext.Elevate(), projectId, endpoint);
          break;
        case KubernetesResourceElevatedOperation.UpdateServiceConnection:
          endpoint = !string.Equals(endpoint.Owner, ServiceEndpointOwner.Environment, StringComparison.OrdinalIgnoreCase) ? service.UpdateServiceEndpoint(requestContext, projectId, endpoint, (string) null) : service.UpdateServiceEndpoint(requestContext.Elevate(), projectId, endpoint, (string) null);
          break;
        case KubernetesResourceElevatedOperation.RemoveServiceConnectionUserPermissions:
          this.m_serviceEndpointSecurity.RemoveServiceEndpointAccessControlList(requestContext.Elevate(), endpoint.Id, projectId.ToString());
          this.m_serviceEndpointSecurity.RemoveServiceEndpointAccessControlList(requestContext.Elevate(), endpoint.Id, KubernetesResourceService.Collection);
          this.m_serviceEndpointSecurity.SetInheritance(requestContext, endpoint.Id, projectId.ToString(), false);
          this.m_serviceEndpointSecurity.SetInheritance(requestContext, endpoint.Id, KubernetesResourceService.Collection, false);
          break;
      }
      return endpoint;
    }

    private Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint ExecuteElevatedOperation(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid endpointId,
      KubernetesResourceElevatedOperation operation)
    {
      IServiceEndpointService2 service = requestContext.GetService<IServiceEndpointService2>();
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint) null;
      switch (operation)
      {
        case KubernetesResourceElevatedOperation.GetServiceConnection:
          serviceEndpoint = service.GetServiceEndpoint(requestContext.Elevate(), projectId, endpointId);
          break;
        case KubernetesResourceElevatedOperation.DeleteServiceConnection:
          service.DeleteServiceEndpoint(requestContext.Elevate(), projectId, endpointId);
          break;
      }
      return serviceEndpoint;
    }

    private IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> ExecuteElevatedOperation(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> endpointIds,
      KubernetesResourceElevatedOperation operation)
    {
      IServiceEndpointService2 service = requestContext.GetService<IServiceEndpointService2>();
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> serviceEndpointList = new List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>();
      if (operation == KubernetesResourceElevatedOperation.QueryEnvironmentOwnedServiceConnections)
        serviceEndpointList = service.QueryServiceEndpoints(requestContext.Elevate(), projectId, "kubernetes", (IEnumerable<string>) null, endpointIds, ServiceEndpointOwner.Environment, true);
      return (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) serviceEndpointList;
    }

    private void ValidateKubernetesResourceCreateParameters(
      IVssRequestContext requestContext,
      Guid projectId,
      KubernetesResource resource)
    {
      ArgumentValidation.CheckKubernetesResource(resource);
      Guid serviceEndpointId = resource.ServiceEndpointId;
      if (resource.ServiceEndpointId != Guid.Empty && requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, projectId, resource.ServiceEndpointId, ServiceEndpointActionFilter.Use)?.Url == (Uri) null)
        throw new Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException(TaskResources.InvalidServiceEndpoint((object) resource.ServiceEndpointId));
    }

    private bool IsEnvironmentOwnedServiceConnectionsFFEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Environments.EnableEnvironmentOwnedServiceConnections");

    protected override string c_layer => nameof (KubernetesResourceService);
  }
}
