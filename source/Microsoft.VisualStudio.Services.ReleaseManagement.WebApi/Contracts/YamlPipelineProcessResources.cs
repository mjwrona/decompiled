// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.YamlPipelineProcessResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  [ClientInternalUseOnly(false)]
  internal sealed class YamlPipelineProcessResources : ReleaseManagementSecuredObject
  {
    [DataMember(Name = "Queues", EmitDefaultValue = false)]
    private List<AgentPoolQueueReference> queues;
    [DataMember(Name = "Endpoints", EmitDefaultValue = false)]
    private List<ServiceEndpointReference> endpoints;

    public IList<AgentPoolQueueReference> Queues
    {
      get
      {
        if (this.queues == null)
          this.queues = new List<AgentPoolQueueReference>();
        return (IList<AgentPoolQueueReference>) this.queues;
      }
      set => this.queues = new List<AgentPoolQueueReference>((IEnumerable<AgentPoolQueueReference>) value);
    }

    public IList<ServiceEndpointReference> Endpoints
    {
      get
      {
        if (this.endpoints == null)
          this.endpoints = new List<ServiceEndpointReference>();
        return (IList<ServiceEndpointReference>) this.endpoints;
      }
      set => this.endpoints = new List<ServiceEndpointReference>((IEnumerable<ServiceEndpointReference>) value);
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<AgentPoolQueueReference> queues = this.queues;
      // ISSUE: explicit non-virtual call
      if ((queues != null ? (__nonvirtual (queues.Count) == 0 ? 1 : 0) : 0) != 0)
        this.queues = (List<AgentPoolQueueReference>) null;
      List<ServiceEndpointReference> endpoints = this.endpoints;
      // ISSUE: explicit non-virtual call
      if ((endpoints != null ? (__nonvirtual (endpoints.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.endpoints = (List<ServiceEndpointReference>) null;
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IList<AgentPoolQueueReference> queues = this.Queues;
      if (queues != null)
        queues.ForEach<AgentPoolQueueReference>((Action<AgentPoolQueueReference>) (i => i.SetSecuredObject(token, requiredPermissions)));
      IList<ServiceEndpointReference> endpoints = this.Endpoints;
      if (endpoints == null)
        return;
      endpoints.ForEach<ServiceEndpointReference>((Action<ServiceEndpointReference>) (i => i.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
