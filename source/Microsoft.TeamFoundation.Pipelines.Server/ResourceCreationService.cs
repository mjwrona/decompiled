// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ResourceCreationService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins;
using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.Exceptions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ResourceCreationService : IResourceCreationService, IVssFrameworkService
  {
    private Guid m_serviceHostId;

    public async Task<IDictionary<string, JObject>> CreateResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, ResourceCreationParameter> resourcesToCreate)
    {
      IDictionary<string, JObject> resourcesAsync;
      using (new Tracer<EventsService>(requestContext, TracePoints.Templates.CreateResourcesEnter, TracePoints.Templates.CreateResourcesLeave, nameof (CreateResourcesAsync)))
      {
        Dictionary<string, JObject> resources = new Dictionary<string, JObject>();
        IDictionary<string, ResourceCreationParameter> dictionary1 = (IDictionary<string, ResourceCreationParameter>) resourcesToCreate.Where<KeyValuePair<string, ResourceCreationParameter>>((Func<KeyValuePair<string, ResourceCreationParameter>, bool>) (resource => resource.Value.Type != null && resource.Value.Type.StartsWith(PipelineTemplateRenderConstants.EndpointKey, StringComparison.OrdinalIgnoreCase))).ToDictionary<KeyValuePair<string, ResourceCreationParameter>, string, ResourceCreationParameter>((Func<KeyValuePair<string, ResourceCreationParameter>, string>) (p => p.Key), (Func<KeyValuePair<string, ResourceCreationParameter>, ResourceCreationParameter>) (p => p.Value));
        foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> keyValuePair in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>) await this.CreateEndpointsAsync(requestContext, projectId, dictionary1))
          resources.Add(keyValuePair.Key, JObject.FromObject((object) keyValuePair.Value));
        IDictionary<string, ResourceCreationParameter> dictionary2 = (IDictionary<string, ResourceCreationParameter>) resourcesToCreate.Where<KeyValuePair<string, ResourceCreationParameter>>((Func<KeyValuePair<string, ResourceCreationParameter>, bool>) (resource => resource.Value.Type != null && resource.Value.Type.Equals(PipelineTemplateRenderConstants.KubernetesResourceType, StringComparison.OrdinalIgnoreCase))).ToDictionary<KeyValuePair<string, ResourceCreationParameter>, string, ResourceCreationParameter>((Func<KeyValuePair<string, ResourceCreationParameter>, string>) (p => p.Key), (Func<KeyValuePair<string, ResourceCreationParameter>, ResourceCreationParameter>) (p => p.Value));
        if (dictionary2.Count > 0)
        {
          EnvironmentInstance environment = this.ValidateAndCreateEnvironment(requestContext, projectId, resourcesToCreate);
          foreach (KeyValuePair<string, KubernetesResource> keyValuePair in (IEnumerable<KeyValuePair<string, KubernetesResource>>) await this.CreateKubernetesResourcesAsync(requestContext, projectId, dictionary2, environment))
            resources.Add(keyValuePair.Key, JObject.FromObject((object) keyValuePair.Value));
        }
        resourcesAsync = (IDictionary<string, JObject>) resources;
      }
      return resourcesAsync;
    }

    private async Task<IDictionary<string, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>> CreateEndpointsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, ResourceCreationParameter> endpointsToCreate)
    {
      IDictionary<string, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> result = (IDictionary<string, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) new Dictionary<string, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>();
      if (endpointsToCreate == null)
        return result;
      IPipelineResourceAuthorizationProxyService pipelineResourceAuthSvc = requestContext.GetService<IPipelineResourceAuthorizationProxyService>();
      foreach (KeyValuePair<string, ResourceCreationParameter> keyValuePair in (IEnumerable<KeyValuePair<string, ResourceCreationParameter>>) endpointsToCreate)
      {
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint = keyValuePair.Value.ResourceToCreate.ToObject<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>();
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = this.CreateServiceEndpoint(requestContext, projectId, endpoint);
        result.Add(keyValuePair.Key, serviceEndpoint);
        await pipelineResourceAuthSvc.AuthorizeResourceForAllPipelinesAsync(requestContext, projectId, serviceEndpoint.Id.ToString(), serviceEndpoint.GetType().FullName);
      }
      return result;
    }

    private async Task<IDictionary<string, KubernetesResource>> CreateKubernetesResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, ResourceCreationParameter> kubernetesResourcesToCreate,
      EnvironmentInstance environment)
    {
      IDictionary<string, KubernetesResource> result = (IDictionary<string, KubernetesResource>) new Dictionary<string, KubernetesResource>();
      IKubernetesResourceService kubeResourceSvc = requestContext.GetService<IKubernetesResourceService>();
      foreach (KeyValuePair<string, ResourceCreationParameter> keyValuePair in (IEnumerable<KeyValuePair<string, ResourceCreationParameter>>) kubernetesResourcesToCreate)
      {
        KeyValuePair<string, ResourceCreationParameter> kubernetesResourceToCreate = keyValuePair;
        KubernetesResource resource = kubernetesResourceToCreate.Value.ResourceToCreate.ToObject<KubernetesResource>();
        resource.EnvironmentReference = new EnvironmentReference()
        {
          Id = environment.Id,
          Name = environment.Name
        };
        result.Add(kubernetesResourceToCreate.Key, await kubeResourceSvc.AddEnvironmentResourceAsync(requestContext, projectId, resource));
        kubernetesResourceToCreate = new KeyValuePair<string, ResourceCreationParameter>();
      }
      IDictionary<string, KubernetesResource> kubernetesResourcesAsync = result;
      result = (IDictionary<string, KubernetesResource>) null;
      kubeResourceSvc = (IKubernetesResourceService) null;
      return kubernetesResourcesAsync;
    }

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    private Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint CreateServiceEndpoint(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint)
    {
      string name = endpoint.Name;
      int num = 2;
      IServiceEndpointService2 service = requestContext.GetService<IServiceEndpointService2>();
      while (true)
      {
        try
        {
          return service.CreateServiceEndpoint(requestContext, projectId, endpoint);
        }
        catch (DuplicateServiceConnectionException ex)
        {
          --num;
          if (num <= 0)
            throw;
          else
            endpoint.Name = this.GetRandomName(name);
        }
      }
    }

    private EnvironmentInstance ValidateAndCreateEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, ResourceCreationParameter> resourcesToCreate)
    {
      List<ResourceCreationParameter> list = resourcesToCreate.Where<KeyValuePair<string, ResourceCreationParameter>>((Func<KeyValuePair<string, ResourceCreationParameter>, bool>) (resource => resource.Value.Type != null && resource.Value.Type.Equals("environment", StringComparison.OrdinalIgnoreCase))).Select<KeyValuePair<string, ResourceCreationParameter>, ResourceCreationParameter>((Func<KeyValuePair<string, ResourceCreationParameter>, ResourceCreationParameter>) (p => p.Value)).ToList<ResourceCreationParameter>();
      EnvironmentInstance environment = list.Count<ResourceCreationParameter>() == 1 ? list.First<ResourceCreationParameter>().ResourceToCreate.ToObject<EnvironmentInstance>() : throw new InvalidRequestException(PipelinesResources.InvalidEnvironmentCreateRequest());
      return this.CreateEnvironment(requestContext, projectId, environment);
    }

    private EnvironmentInstance CreateEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      EnvironmentInstance environment)
    {
      string prefix = string.Concat<char>(environment.Name.Where<char>((Func<char, bool>) (ch => char.IsLetterOrDigit(ch))));
      environment.Name = prefix;
      IEnvironmentService service = requestContext.GetService<IEnvironmentService>();
      while (true)
      {
        try
        {
          return service.AddEnvironment(requestContext, projectId, environment);
        }
        catch (EnvironmentExistsException ex)
        {
          environment.Name = this.GetRandomName(prefix);
        }
      }
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckProjectCollectionRequestContext();
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private string GetRandomName(string prefix)
    {
      string str = new Random().Next().ToString().Substring(0, 4);
      return PipelinesResources.ResourceNameFormat((object) prefix, (object) str);
    }
  }
}
