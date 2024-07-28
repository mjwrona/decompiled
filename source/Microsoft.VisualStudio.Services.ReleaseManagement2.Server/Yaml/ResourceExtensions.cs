// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Yaml.ResourceExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Yaml
{
  internal static class ResourceExtensions
  {
    public static YamlPipelineProcessResource ToReleaseResources(this PipelineResources resources)
    {
      if (resources == null)
        throw new ArgumentNullException(nameof (resources));
      YamlPipelineProcessResource releaseResources = new YamlPipelineProcessResource();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      releaseResources.Queues.AddRange<AgentPoolQueueReference, IList<AgentPoolQueueReference>>(resources.Queues.Select<AgentQueueReference, AgentPoolQueueReference>(ResourceExtensions.\u003C\u003EO.\u003C0\u003E__ToReleaseQueueReference ?? (ResourceExtensions.\u003C\u003EO.\u003C0\u003E__ToReleaseQueueReference = new Func<AgentQueueReference, AgentPoolQueueReference>(ResourceExtensions.ToReleaseQueueReference))));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      releaseResources.Endpoints.AddRange<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ServiceEndpointReference, IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ServiceEndpointReference>>(resources.Endpoints.Select<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ServiceEndpointReference>(ResourceExtensions.\u003C\u003EO.\u003C1\u003E__ToReleaseEndpointReference ?? (ResourceExtensions.\u003C\u003EO.\u003C1\u003E__ToReleaseEndpointReference = new Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ServiceEndpointReference>(ResourceExtensions.ToReleaseEndpointReference))));
      return releaseResources;
    }

    public static PipelineResources ToPipelineResources(this YamlPipelineProcessResource resources)
    {
      if (resources == null)
        throw new ArgumentNullException(nameof (resources));
      PipelineResources pipelineResources = new PipelineResources();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      pipelineResources.Queues.AddRange<AgentQueueReference, ISet<AgentQueueReference>>(resources.Queues.Select<AgentPoolQueueReference, AgentQueueReference>(ResourceExtensions.\u003C\u003EO.\u003C2\u003E__ToPipelineQueueReference ?? (ResourceExtensions.\u003C\u003EO.\u003C2\u003E__ToPipelineQueueReference = new Func<AgentPoolQueueReference, AgentQueueReference>(ResourceExtensions.ToPipelineQueueReference))));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      pipelineResources.Endpoints.AddRange<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference, ISet<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference>>(resources.Endpoints.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ServiceEndpointReference, Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference>(ResourceExtensions.\u003C\u003EO.\u003C3\u003E__ToPipelineEndpointReference ?? (ResourceExtensions.\u003C\u003EO.\u003C3\u003E__ToPipelineEndpointReference = new Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ServiceEndpointReference, Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference>(ResourceExtensions.ToPipelineEndpointReference))));
      return pipelineResources;
    }

    private static Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference ToPipelineEndpointReference(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ServiceEndpointReference endpoint)
    {
      return new Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference()
      {
        Id = endpoint.Id
      };
    }

    private static AgentQueueReference ToPipelineQueueReference(AgentPoolQueueReference queue) => new AgentQueueReference()
    {
      Id = queue.Id
    };

    private static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ServiceEndpointReference ToReleaseEndpointReference(
      Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference endpoint)
    {
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ServiceEndpointReference()
      {
        Id = endpoint.Id
      };
    }

    private static AgentPoolQueueReference ToReleaseQueueReference(AgentQueueReference queue) => new AgentPoolQueueReference()
    {
      Id = queue.Id
    };
  }
}
