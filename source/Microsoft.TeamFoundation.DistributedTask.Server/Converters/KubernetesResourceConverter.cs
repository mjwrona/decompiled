// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Converters.KubernetesResourceConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Converters
{
  public static class KubernetesResourceConverter
  {
    public static KubernetesResource ToResource(
      this KubernetesResourceCreateParameters createParameters,
      int environmentId)
    {
      Guid guid = Guid.Empty;
      if (createParameters is KubernetesResourceCreateParametersExistingEndpoint existingEndpoint)
        guid = existingEndpoint.ServiceEndpointId;
      KubernetesResource resource = new KubernetesResource();
      resource.Name = createParameters.Name;
      resource.Type = EnvironmentResourceType.Kubernetes;
      resource.Namespace = createParameters.Namespace;
      resource.ClusterName = createParameters.ClusterName ?? string.Empty;
      resource.ServiceEndpointId = guid;
      resource.EnvironmentReference.Id = environmentId;
      resource.Tags = createParameters.Tags;
      return resource;
    }
  }
}
