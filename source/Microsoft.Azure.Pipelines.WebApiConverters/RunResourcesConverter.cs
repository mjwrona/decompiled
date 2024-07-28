// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.RunResourcesConverter
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  internal class RunResourcesConverter
  {
    public static readonly VersionedObjectFactory VersionedObjectFactory = new VersionedObjectFactory(new IVersionedObjectCreator[1]
    {
      (IVersionedObjectCreator) new VersionedObjectCreator<RunResourcesConverter>(1)
    });
    private const int ResourceVersion = 1;

    public Microsoft.Azure.Pipelines.WebApi.RunResources ToWebApiRunResources(
      IVssRequestContext requestContext,
      Microsoft.Azure.Pipelines.Server.ObjectModel.RunResources source,
      Microsoft.Azure.Pipelines.Server.ObjectModel.Run securingRun)
    {
      if (source == null)
        return (Microsoft.Azure.Pipelines.WebApi.RunResources) null;
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.Server.ObjectModel.Run>(securingRun, nameof (securingRun));
      ISecuredObject securedObject = securingRun.ToSecuredObject();
      Microsoft.Azure.Pipelines.WebApi.RunResources webApiRunResources = new Microsoft.Azure.Pipelines.WebApi.RunResources(securedObject);
      if (requestContext.IsFeatureEnabled("Build2.GetRunRestAPIConsumedResources"))
      {
        foreach (KeyValuePair<string, Microsoft.Azure.Pipelines.Server.ObjectModel.PipelineResource> pipeline in (IEnumerable<KeyValuePair<string, Microsoft.Azure.Pipelines.Server.ObjectModel.PipelineResource>>) source.Pipelines)
        {
          Microsoft.Azure.Pipelines.Server.ObjectModel.PipelineResource pipelineResource1 = pipeline.Value;
          PipelineConverter versionedObject = requestContext.CreateVersionedObject<PipelineConverter>(1);
          Microsoft.Azure.Pipelines.WebApi.PipelineResource pipelineResource2 = new Microsoft.Azure.Pipelines.WebApi.PipelineResource()
          {
            Pipeline = versionedObject.ToWebApiPipelineReference(requestContext, pipelineResource1.Pipeline, securedObject),
            Version = pipelineResource1.Parameters.Version
          };
          webApiRunResources.Pipelines.Add(pipeline.Key, pipelineResource2);
        }
        foreach (KeyValuePair<string, Microsoft.Azure.Pipelines.Server.ObjectModel.ContainerResource> container in (IEnumerable<KeyValuePair<string, Microsoft.Azure.Pipelines.Server.ObjectModel.ContainerResource>>) source.Containers)
        {
          Microsoft.Azure.Pipelines.Server.ObjectModel.ContainerResource containerResource1 = container.Value;
          Microsoft.Azure.Pipelines.WebApi.ContainerResource containerResource2 = new Microsoft.Azure.Pipelines.WebApi.ContainerResource()
          {
            Container = containerResource1.Container.ToWebApiContainer(securedObject)
          };
          webApiRunResources.Containers.Add(container.Key, containerResource2);
        }
      }
      foreach (KeyValuePair<string, Microsoft.Azure.Pipelines.Server.ObjectModel.RepositoryResource> repository in (IEnumerable<KeyValuePair<string, Microsoft.Azure.Pipelines.Server.ObjectModel.RepositoryResource>>) source.Repositories)
      {
        Microsoft.Azure.Pipelines.Server.ObjectModel.RepositoryResource repositoryResource1 = repository.Value;
        Microsoft.Azure.Pipelines.WebApi.RepositoryResource repositoryResource2 = new Microsoft.Azure.Pipelines.WebApi.RepositoryResource(securedObject)
        {
          Repository = repositoryResource1.Repository.ToWebApiRepository(securedObject),
          RefName = repositoryResource1.RefName,
          Version = repositoryResource1.Version
        };
        webApiRunResources.Repositories.Add(repository.Key, repositoryResource2);
      }
      return webApiRunResources;
    }
  }
}
