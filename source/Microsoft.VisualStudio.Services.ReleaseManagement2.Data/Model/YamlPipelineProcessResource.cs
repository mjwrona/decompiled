// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlPipelineProcessResource
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public sealed class YamlPipelineProcessResource
  {
    private List<AgentPoolQueueReference> queues;
    private List<ServiceEndpointReference> endpoints;

    public IList<AgentPoolQueueReference> Queues
    {
      get
      {
        if (this.queues == null)
          this.queues = new List<AgentPoolQueueReference>();
        return (IList<AgentPoolQueueReference>) this.queues;
      }
    }

    public IList<ServiceEndpointReference> Endpoints
    {
      get
      {
        if (this.endpoints == null)
          this.endpoints = new List<ServiceEndpointReference>();
        return (IList<ServiceEndpointReference>) this.endpoints;
      }
    }
  }
}
