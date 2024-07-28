// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.IArtifactResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IArtifactResolver
  {
    Guid GetArtifactDownloadTaskId(Resource resource);

    void PopulateMappedTaskInputs(Resource resource, TaskStep taskStep);

    bool ResolveStep(
      IPipelineContext pipelineContext,
      JobStep step,
      out IList<TaskStep> resolvedSteps);

    bool ResolveStep(IResourceStore resourceStore, TaskStep taskStep, out string errorMessage);

    bool ValidateDeclaredResource(Resource resource, out PipelineValidationError error);
  }
}
