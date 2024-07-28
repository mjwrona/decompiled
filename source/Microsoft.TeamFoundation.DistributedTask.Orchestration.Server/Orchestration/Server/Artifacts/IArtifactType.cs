// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.IArtifactType
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.WebHooks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public interface IArtifactType
  {
    IDictionary<string, Guid> ArtifactDownloadTaskIds { get; }

    ArtifactTriggerConfiguration ArtifactTriggerConfiguration { get; }

    Guid ArtifactDownloadTaskId { get; }

    string ArtifactType { get; }

    IList<InputDescriptor> InputDescriptors { get; }

    IDictionary<string, string> TaskInputMapping { get; }

    IDictionary<Guid, IDictionary<string, string>> TaskInputMappings { get; }

    IDictionary<string, string> TaskInputDefaultValues { get; }

    string Type { get; }

    string Name { get; }

    string DisplayName { get; }

    string UniqueSourceIdentifier { get; }

    IDictionary<string, string> YamlInputMapping { get; }

    IDictionary<string, string> GetTaskInputMapping(Guid? taskId = null);

    void FillQueuePipelineDataParameters(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      WebHookEventPayloadInputMapper inputMapper);

    InputValues GetInputValues(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> currentInputValues);

    InputValue GetLatestVersion(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo);

    IList<string> GetValidYamlInputs();

    bool ResolveStep(
      IVssRequestContext requestContext,
      IPipelineContext pipelineContext,
      Guid projectId,
      JobStep step,
      out IList<TaskStep> resolvedSteps);
  }
}
