// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildProcessResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public sealed class BuildProcessResources
  {
    [DataMember(Name = "endpoints", EmitDefaultValue = false)]
    private HashSet<ServiceEndpointReference> m_endpoints;
    [DataMember(Name = "files", EmitDefaultValue = false)]
    private HashSet<SecureFileReference> m_files;
    [DataMember(Name = "queues", EmitDefaultValue = false)]
    private HashSet<AgentPoolQueueReference> m_queues;
    [DataMember(Name = "variableGroups", EmitDefaultValue = false)]
    private HashSet<VariableGroupReference> m_variableGroups;

    public void Add(ResourceReference resourceReference)
    {
      switch (resourceReference)
      {
        case ServiceEndpointReference _:
          this.Endpoints.Add((ServiceEndpointReference) resourceReference);
          break;
        case SecureFileReference _:
          this.Files.Add((SecureFileReference) resourceReference);
          break;
        case AgentPoolQueueReference _:
          this.Queues.Add((AgentPoolQueueReference) resourceReference);
          break;
        case VariableGroupReference _:
          this.VariableGroups.Add((VariableGroupReference) resourceReference);
          break;
      }
    }

    public int Count => this.Endpoints.Count + this.Files.Count + this.Queues.Count + this.VariableGroups.Count;

    public IEnumerable<ResourceReference> AllResources => this.Endpoints.Cast<ResourceReference>().Concat<ResourceReference>((IEnumerable<ResourceReference>) this.Files).Concat<ResourceReference>((IEnumerable<ResourceReference>) this.Queues).Concat<ResourceReference>((IEnumerable<ResourceReference>) this.VariableGroups);

    public BuildProcessResources GetAuthorizedResources()
    {
      BuildProcessResources authorizedResources = new BuildProcessResources();
      authorizedResources.Endpoints.AddRange<ServiceEndpointReference, ISet<ServiceEndpointReference>>(this.Endpoints.Where<ServiceEndpointReference>((Func<ServiceEndpointReference, bool>) (e => e.Authorized)));
      authorizedResources.Files.AddRange<SecureFileReference, ISet<SecureFileReference>>(this.Files.Where<SecureFileReference>((Func<SecureFileReference, bool>) (f => f.Authorized)));
      authorizedResources.Queues.AddRange<AgentPoolQueueReference, ISet<AgentPoolQueueReference>>(this.Queues.Where<AgentPoolQueueReference>((Func<AgentPoolQueueReference, bool>) (q => q.Authorized)));
      authorizedResources.VariableGroups.AddRange<VariableGroupReference, ISet<VariableGroupReference>>(this.VariableGroups.Where<VariableGroupReference>((Func<VariableGroupReference, bool>) (vg => vg.Authorized)));
      return authorizedResources;
    }

    public BuildProcessResources GetUnauthorizedResources()
    {
      BuildProcessResources unauthorizedResources = new BuildProcessResources();
      unauthorizedResources.Endpoints.AddRange<ServiceEndpointReference, ISet<ServiceEndpointReference>>(this.Endpoints.Where<ServiceEndpointReference>((Func<ServiceEndpointReference, bool>) (e => !e.Authorized)));
      unauthorizedResources.Files.AddRange<SecureFileReference, ISet<SecureFileReference>>(this.Files.Where<SecureFileReference>((Func<SecureFileReference, bool>) (f => !f.Authorized)));
      unauthorizedResources.Queues.AddRange<AgentPoolQueueReference, ISet<AgentPoolQueueReference>>(this.Queues.Where<AgentPoolQueueReference>((Func<AgentPoolQueueReference, bool>) (q => !q.Authorized)));
      unauthorizedResources.VariableGroups.AddRange<VariableGroupReference, ISet<VariableGroupReference>>(this.VariableGroups.Where<VariableGroupReference>((Func<VariableGroupReference, bool>) (vg => !vg.Authorized)));
      return unauthorizedResources;
    }

    public BuildProcessResources Where(Func<ResourceReference, bool> predicate)
    {
      ArgumentUtility.CheckForNull<Func<ResourceReference, bool>>(predicate, nameof (predicate));
      BuildProcessResources processResources = new BuildProcessResources();
      foreach (ResourceReference allResource in this.AllResources)
      {
        if (predicate(allResource))
          processResources.Add(allResource);
      }
      return processResources;
    }

    public void Clear()
    {
      this.Endpoints.Clear();
      this.Files.Clear();
      this.Queues.Clear();
      this.VariableGroups.Clear();
    }

    public void MergeWith(BuildProcessResources other)
    {
      ArgumentUtility.CheckForNull<BuildProcessResources>(other, nameof (other));
      this.Endpoints.AddRange<ServiceEndpointReference, ISet<ServiceEndpointReference>>((IEnumerable<ServiceEndpointReference>) other.Endpoints);
      this.Files.AddRange<SecureFileReference, ISet<SecureFileReference>>((IEnumerable<SecureFileReference>) other.Files);
      this.Queues.AddRange<AgentPoolQueueReference, ISet<AgentPoolQueueReference>>((IEnumerable<AgentPoolQueueReference>) other.Queues);
      this.VariableGroups.AddRange<VariableGroupReference, ISet<VariableGroupReference>>((IEnumerable<VariableGroupReference>) other.VariableGroups);
    }

    public void ExceptWith(BuildProcessResources other)
    {
      ArgumentUtility.CheckForNull<BuildProcessResources>(other, nameof (other));
      this.Endpoints.ExceptWith((IEnumerable<ServiceEndpointReference>) other.Endpoints);
      this.Files.ExceptWith((IEnumerable<SecureFileReference>) other.Files);
      this.Queues.ExceptWith((IEnumerable<AgentPoolQueueReference>) other.Queues);
      this.VariableGroups.ExceptWith((IEnumerable<VariableGroupReference>) other.VariableGroups);
    }

    public void IntersectWith(BuildProcessResources other)
    {
      ArgumentUtility.CheckForNull<BuildProcessResources>(other, nameof (other));
      this.Endpoints.IntersectWith((IEnumerable<ServiceEndpointReference>) other.Endpoints);
      this.Files.IntersectWith((IEnumerable<SecureFileReference>) other.Files);
      this.Queues.IntersectWith((IEnumerable<AgentPoolQueueReference>) other.Queues);
      this.VariableGroups.IntersectWith((IEnumerable<VariableGroupReference>) other.VariableGroups);
    }

    public bool Contains(ResourceReference resourceReference)
    {
      switch (resourceReference)
      {
        case null:
          return false;
        case ServiceEndpointReference _:
          return this.Endpoints.Contains((ServiceEndpointReference) resourceReference);
        case SecureFileReference _:
          return this.Files.Contains((SecureFileReference) resourceReference);
        case AgentPoolQueueReference _:
          return this.Queues.Contains((AgentPoolQueueReference) resourceReference);
        case VariableGroupReference _:
          return this.VariableGroups.Contains((VariableGroupReference) resourceReference);
        default:
          return false;
      }
    }

    public static BuildProcessResources Merge(
      BuildProcessResources left,
      BuildProcessResources right)
    {
      ArgumentUtility.CheckForNull<BuildProcessResources>(left, nameof (left));
      ArgumentUtility.CheckForNull<BuildProcessResources>(right, nameof (right));
      BuildProcessResources processResources = new BuildProcessResources();
      processResources.MergeWith(left);
      processResources.MergeWith(right);
      return processResources;
    }

    public ISet<ServiceEndpointReference> Endpoints
    {
      get
      {
        if (this.m_endpoints == null)
          this.m_endpoints = new HashSet<ServiceEndpointReference>((IEqualityComparer<ServiceEndpointReference>) new EndpointComparer());
        return (ISet<ServiceEndpointReference>) this.m_endpoints;
      }
    }

    public ISet<SecureFileReference> Files
    {
      get
      {
        if (this.m_files == null)
          this.m_files = new HashSet<SecureFileReference>((IEqualityComparer<SecureFileReference>) new FileComparer());
        return (ISet<SecureFileReference>) this.m_files;
      }
    }

    public ISet<AgentPoolQueueReference> Queues
    {
      get
      {
        if (this.m_queues == null)
          this.m_queues = new HashSet<AgentPoolQueueReference>((IEqualityComparer<AgentPoolQueueReference>) new QueueComparer());
        return (ISet<AgentPoolQueueReference>) this.m_queues;
      }
    }

    public ISet<VariableGroupReference> VariableGroups
    {
      get
      {
        if (this.m_variableGroups == null)
          this.m_variableGroups = new HashSet<VariableGroupReference>((IEqualityComparer<VariableGroupReference>) new VariableGroupComparer());
        return (ISet<VariableGroupReference>) this.m_variableGroups;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      HashSet<ServiceEndpointReference> endpoints = this.m_endpoints;
      // ISSUE: explicit non-virtual call
      if ((endpoints != null ? (__nonvirtual (endpoints.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_endpoints = (HashSet<ServiceEndpointReference>) null;
      HashSet<SecureFileReference> files = this.m_files;
      // ISSUE: explicit non-virtual call
      if ((files != null ? (__nonvirtual (files.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_files = (HashSet<SecureFileReference>) null;
      HashSet<AgentPoolQueueReference> queues = this.m_queues;
      // ISSUE: explicit non-virtual call
      if ((queues != null ? (__nonvirtual (queues.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_queues = (HashSet<AgentPoolQueueReference>) null;
      HashSet<VariableGroupReference> variableGroups = this.m_variableGroups;
      // ISSUE: explicit non-virtual call
      if ((variableGroups != null ? (__nonvirtual (variableGroups.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_variableGroups = (HashSet<VariableGroupReference>) null;
    }
  }
}
