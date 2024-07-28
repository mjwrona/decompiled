// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.RunResourcesParameters
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public class RunResourcesParameters
  {
    public RunResourcesParameters()
    {
    }

    private RunResourcesParameters(RunResourcesParameters parameters)
    {
      foreach (KeyValuePair<string, BuildResourceParameters> build in parameters.Builds)
        this.Builds.Add(build.Key, new BuildResourceParameters(build.Value.Version));
      foreach (KeyValuePair<string, ContainerResourceParameters> container in parameters.Containers)
        this.Containers.Add(container.Key, new ContainerResourceParameters(container.Value.Version));
      foreach (KeyValuePair<string, PackageResourceParameters> package in parameters.Packages)
        this.Packages.Add(package.Key, new PackageResourceParameters(package.Value.Version));
      foreach (KeyValuePair<string, PipelineResourceParameters> pipeline in parameters.Pipelines)
        this.Pipelines.Add(pipeline.Key, new PipelineResourceParameters(pipeline.Value.Version));
      foreach (KeyValuePair<string, RepositoryResourceParameters> repository in parameters.Repositories)
        this.Repositories.Add(repository.Key, new RepositoryResourceParameters(repository.Value.RefName, repository.Value.Version));
    }

    public Dictionary<string, BuildResourceParameters> Builds { get; } = new Dictionary<string, BuildResourceParameters>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, ContainerResourceParameters> Containers { get; } = new Dictionary<string, ContainerResourceParameters>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, PackageResourceParameters> Packages { get; } = new Dictionary<string, PackageResourceParameters>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, PipelineResourceParameters> Pipelines { get; } = new Dictionary<string, PipelineResourceParameters>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, RepositoryResourceParameters> Repositories { get; } = new Dictionary<string, RepositoryResourceParameters>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public RunResourcesParameters Clone() => new RunResourcesParameters(this);
  }
}
