// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.AzureContainerRepository.AzureContainerRepositoryHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.AzureContainerRepository
{
  public static class AzureContainerRepositoryHelper
  {
    public static IList<ContainerImageTrigger> GetAzureContainerRepositoryTriggers(
      ReleaseDefinition definition)
    {
      IList<ContainerImageTrigger> repositoryTriggers = (IList<ContainerImageTrigger>) new List<ContainerImageTrigger>();
      if (definition != null && definition.Triggers != null)
      {
        foreach (ReleaseTriggerBase trigger in (IEnumerable<ReleaseTriggerBase>) definition.Triggers)
        {
          if (trigger.TriggerType == ReleaseTriggerType.ContainerImage)
          {
            ContainerImageTrigger containerRepoTrigger = (ContainerImageTrigger) trigger;
            ArtifactSource artifactSource = definition.LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (x => x.Alias == containerRepoTrigger.Alias)).SingleOrDefault<ArtifactSource>();
            if (artifactSource != null && artifactSource.IsAzureContainerRepositoryArtifact)
              repositoryTriggers.Add(containerRepoTrigger);
          }
        }
      }
      return repositoryTriggers;
    }
  }
}
