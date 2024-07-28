// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.RunResources
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [DataContract]
  public class RunResources : BaseSecuredObject
  {
    [DataMember(Name = "Repositories", EmitDefaultValue = false)]
    private Dictionary<string, RepositoryResource> m_repositories;
    [DataMember(Name = "Pipelines", EmitDefaultValue = false)]
    private Dictionary<string, PipelineResource> m_pipelines;
    [DataMember(Name = "Containers", EmitDefaultValue = false)]
    private Dictionary<string, ContainerResource> m_containers;

    [JsonConstructor]
    private RunResources()
    {
    }

    internal RunResources(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    public Dictionary<string, RepositoryResource> Repositories
    {
      get
      {
        if (this.m_repositories == null)
          this.m_repositories = new Dictionary<string, RepositoryResource>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_repositories;
      }
    }

    public Dictionary<string, ContainerResource> Containers
    {
      get
      {
        if (this.m_containers == null)
          this.m_containers = new Dictionary<string, ContainerResource>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_containers;
      }
    }

    public Dictionary<string, PipelineResource> Pipelines
    {
      get
      {
        if (this.m_pipelines == null)
          this.m_pipelines = new Dictionary<string, PipelineResource>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_pipelines;
      }
    }
  }
}
