// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.JobResources
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class JobResources
  {
    [DataMember(Name = "Containers", EmitDefaultValue = false)]
    private List<ContainerResource> m_containers;
    [DataMember(Name = "Endpoints", EmitDefaultValue = false)]
    private List<ServiceEndpoint> m_endpoints;
    [DataMember(Name = "Repositories", EmitDefaultValue = false)]
    private List<RepositoryResource> m_repositories;
    [DataMember(Name = "SecureFiles", EmitDefaultValue = false)]
    private List<SecureFile> m_secureFiles;

    public List<ContainerResource> Containers
    {
      get
      {
        if (this.m_containers == null)
          this.m_containers = new List<ContainerResource>();
        return this.m_containers;
      }
    }

    public List<ServiceEndpoint> Endpoints
    {
      get
      {
        if (this.m_endpoints == null)
          this.m_endpoints = new List<ServiceEndpoint>();
        return this.m_endpoints;
      }
    }

    public List<RepositoryResource> Repositories
    {
      get
      {
        if (this.m_repositories == null)
          this.m_repositories = new List<RepositoryResource>();
        return this.m_repositories;
      }
    }

    public List<SecureFile> SecureFiles
    {
      get
      {
        if (this.m_secureFiles == null)
          this.m_secureFiles = new List<SecureFile>();
        return this.m_secureFiles;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<ContainerResource> containers = this.m_containers;
      // ISSUE: explicit non-virtual call
      if ((containers != null ? (__nonvirtual (containers.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_containers = (List<ContainerResource>) null;
      List<ServiceEndpoint> endpoints = this.m_endpoints;
      // ISSUE: explicit non-virtual call
      if ((endpoints != null ? (__nonvirtual (endpoints.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_endpoints = (List<ServiceEndpoint>) null;
      List<RepositoryResource> repositories = this.m_repositories;
      // ISSUE: explicit non-virtual call
      if ((repositories != null ? (__nonvirtual (repositories.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_repositories = (List<RepositoryResource>) null;
      List<SecureFile> secureFiles = this.m_secureFiles;
      // ISSUE: explicit non-virtual call
      if ((secureFiles != null ? (__nonvirtual (secureFiles.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_secureFiles = (List<SecureFile>) null;
    }
  }
}
