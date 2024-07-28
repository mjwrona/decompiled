// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.KubernetesResourceDataProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class KubernetesResourceDataProvider : IEnvironmentResourceDataProvider
  {
    public Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint GetEndPoint(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int resourceId)
    {
      KubernetesResource environmentResource = requestContext.GetService<IKubernetesResourceService>().GetEnvironmentResource(requestContext, projectId, environmentId, resourceId);
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endPoint = (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint) null;
      if (environmentResource != null && environmentResource.ServiceEndpointId != Guid.Empty)
        endPoint = requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, projectId, environmentResource.ServiceEndpointId);
      return endPoint;
    }
  }
}
