// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.RunResourcesParameters
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
  public class RunResourcesParameters : BaseSecuredObject
  {
    [DataMember(Name = "Builds", EmitDefaultValue = false)]
    private Dictionary<string, BuildResourceParameters> m_builds;
    [DataMember(Name = "Containers", EmitDefaultValue = false)]
    private Dictionary<string, ContainerResourceParameters> m_containers;
    [DataMember(Name = "Repositories", EmitDefaultValue = false)]
    private Dictionary<string, RepositoryResourceParameters> m_repositories;
    [DataMember(Name = "Pipelines", EmitDefaultValue = false)]
    private Dictionary<string, PipelineResourceParameters> m_pipelines;
    [DataMember(Name = "Packages", EmitDefaultValue = false)]
    private Dictionary<string, PackageResourceParameters> m_packages;

    [JsonConstructor]
    public RunResourcesParameters()
    {
    }

    internal RunResourcesParameters(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    public Dictionary<string, BuildResourceParameters> Builds
    {
      get
      {
        if (this.m_builds == null)
          this.m_builds = new Dictionary<string, BuildResourceParameters>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_builds;
      }
    }

    public Dictionary<string, ContainerResourceParameters> Containers
    {
      get
      {
        if (this.m_containers == null)
          this.m_containers = new Dictionary<string, ContainerResourceParameters>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_containers;
      }
    }

    public Dictionary<string, RepositoryResourceParameters> Repositories
    {
      get
      {
        if (this.m_repositories == null)
          this.m_repositories = new Dictionary<string, RepositoryResourceParameters>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_repositories;
      }
    }

    public Dictionary<string, PipelineResourceParameters> Pipelines
    {
      get
      {
        if (this.m_pipelines == null)
          this.m_pipelines = new Dictionary<string, PipelineResourceParameters>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_pipelines;
      }
    }

    public Dictionary<string, PackageResourceParameters> Packages
    {
      get
      {
        if (this.m_packages == null)
          this.m_packages = new Dictionary<string, PackageResourceParameters>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_packages;
      }
    }
  }
}
