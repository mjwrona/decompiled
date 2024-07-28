// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.RunResources
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public class RunResources
  {
    private Dictionary<string, RepositoryResource> m_repositories = new Dictionary<string, RepositoryResource>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, PipelineResource> m_pipelines = new Dictionary<string, PipelineResource>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, ContainerResource> m_containers = new Dictionary<string, ContainerResource>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, RepositoryResource> Repositories => (IReadOnlyDictionary<string, RepositoryResource>) this.m_repositories;

    public IReadOnlyDictionary<string, ContainerResource> Containers => (IReadOnlyDictionary<string, ContainerResource>) this.m_containers;

    public IReadOnlyDictionary<string, PipelineResource> Pipelines => (IReadOnlyDictionary<string, PipelineResource>) this.m_pipelines;

    public void AddRepositoryResource(
      string resourceName,
      Repository repository,
      string refName,
      string version)
    {
      ArgumentUtility.CheckForNull<Repository>(repository, nameof (repository));
      RepositoryResource repositoryResource = new RepositoryResource(repository, refName, version);
      this.m_repositories.Add(resourceName, repositoryResource);
    }

    public void AddPipelineResource(
      string resourceName,
      PipelineReference pipeline,
      string version)
    {
      ArgumentUtility.CheckForNull<PipelineReference>(pipeline, nameof (pipeline));
      PipelineResource pipelineResource = new PipelineResource(pipeline, new PipelineResourceParameters(version));
      this.m_pipelines.Add(resourceName, pipelineResource);
    }

    public void AddContainerResource(string resourceName, Container container)
    {
      ArgumentUtility.CheckForNull<Container>(container, nameof (container));
      ContainerResource containerResource = new ContainerResource(container);
      this.m_containers.Add(resourceName, containerResource);
    }
  }
}
