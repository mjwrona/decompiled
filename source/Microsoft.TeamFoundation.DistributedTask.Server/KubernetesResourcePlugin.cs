// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.KubernetesResourcePlugin
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class KubernetesResourcePlugin
  {
    public static async Task<EnvironmentResourceReference> CreateDynamicResourceAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      EnvironmentResourceReference sourceResource,
      int environmentId,
      string newResourceName)
    {
      KubernetesResource environmentResource = requestContext.GetService<KubernetesResourceService>().GetEnvironmentResource(requestContext, projectId, environmentId, sourceResource.Id);
      KubernetesResource resource = new KubernetesResource();
      resource.Name = newResourceName;
      resource.Namespace = newResourceName;
      resource.ClusterName = environmentResource.ClusterName;
      resource.ServiceEndpointId = environmentResource.ServiceEndpointId;
      resource.EnvironmentReference = new EnvironmentReference()
      {
        Id = environmentId
      };
      resource.Tags = (IList<string>) new List<string>();
      resource.Tags.Add("Review");
      KubernetesResource kubernetesResource = await requestContext.GetService<IKubernetesResourceService>().AddEnvironmentResourceAsync(requestContext, projectId, resource);
      IEnvironmentResourceReferenceService service = requestContext.GetService<IEnvironmentResourceReferenceService>();
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = projectId;
      int environmentId1 = environmentId;
      string name = kubernetesResource.Name;
      int? resourceId = new int?();
      string resourceName = name;
      EnvironmentResourceType? resourceType = new EnvironmentResourceType?();
      return (await service.GetEnvironmentResourceReferencesAsync(requestContext1, projectId1, environmentId1, resourceId, resourceName, resourceType))[0];
    }
  }
}
