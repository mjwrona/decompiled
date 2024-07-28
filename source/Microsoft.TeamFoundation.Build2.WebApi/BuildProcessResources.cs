// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildProcessResources
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public sealed class BuildProcessResources : BaseSecuredObject
  {
    [DataMember(Name = "Queues", EmitDefaultValue = false)]
    private List<AgentPoolQueueReference> m_queues;
    [DataMember(Name = "Endpoints", EmitDefaultValue = false)]
    private List<ServiceEndpointReference> m_endpoints;
    [DataMember(Name = "Files", EmitDefaultValue = false)]
    private List<SecureFileReference> m_files;
    [DataMember(Name = "VariableGroups", EmitDefaultValue = false)]
    private List<VariableGroupReference> m_variableGroups;

    public BuildProcessResources()
    {
    }

    internal BuildProcessResources(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    public IList<AgentPoolQueueReference> Queues
    {
      get
      {
        if (this.m_queues == null)
          this.m_queues = new List<AgentPoolQueueReference>();
        return (IList<AgentPoolQueueReference>) this.m_queues;
      }
      set => this.m_queues = new List<AgentPoolQueueReference>((IEnumerable<AgentPoolQueueReference>) value);
    }

    public IList<ServiceEndpointReference> Endpoints
    {
      get
      {
        if (this.m_endpoints == null)
          this.m_endpoints = new List<ServiceEndpointReference>();
        return (IList<ServiceEndpointReference>) this.m_endpoints;
      }
      set => this.m_endpoints = new List<ServiceEndpointReference>((IEnumerable<ServiceEndpointReference>) value);
    }

    public IList<SecureFileReference> Files
    {
      get
      {
        if (this.m_files == null)
          this.m_files = new List<SecureFileReference>();
        return (IList<SecureFileReference>) this.m_files;
      }
      set => this.m_files = new List<SecureFileReference>((IEnumerable<SecureFileReference>) value);
    }

    public IList<VariableGroupReference> VariableGroups
    {
      get
      {
        if (this.m_variableGroups == null)
          this.m_variableGroups = new List<VariableGroupReference>();
        return (IList<VariableGroupReference>) this.m_variableGroups;
      }
      set => this.m_variableGroups = new List<VariableGroupReference>((IEnumerable<VariableGroupReference>) value);
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<AgentPoolQueueReference> queues = this.m_queues;
      // ISSUE: explicit non-virtual call
      if ((queues != null ? (__nonvirtual (queues.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_queues = (List<AgentPoolQueueReference>) null;
      List<ServiceEndpointReference> endpoints = this.m_endpoints;
      // ISSUE: explicit non-virtual call
      if ((endpoints != null ? (__nonvirtual (endpoints.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_endpoints = (List<ServiceEndpointReference>) null;
      List<SecureFileReference> files = this.m_files;
      // ISSUE: explicit non-virtual call
      if ((files != null ? (__nonvirtual (files.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_files = (List<SecureFileReference>) null;
      List<VariableGroupReference> variableGroups = this.m_variableGroups;
      // ISSUE: explicit non-virtual call
      if ((variableGroups != null ? (__nonvirtual (variableGroups.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_variableGroups = (List<VariableGroupReference>) null;
    }
  }
}
