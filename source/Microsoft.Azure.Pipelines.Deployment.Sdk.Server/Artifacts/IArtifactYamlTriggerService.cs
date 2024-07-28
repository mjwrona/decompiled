// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts.IArtifactYamlTriggerService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2CF55160-AB9F-45A3-BD33-54D24F269988
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Sdk.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts
{
  [DefaultServiceImplementation(typeof (FrameworkArtifactYamlTriggerService))]
  public interface IArtifactYamlTriggerService : IVssFrameworkService
  {
    void AddTriggers(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      RepositoryResource repositoryResource,
      string yamlFileName,
      PipelineBuilder pipelineBuilder,
      string sourceVersion,
      Uri repositoryUrl);

    void UpdateTriggers(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      RepositoryResource repositoryResource,
      string yamlFileName,
      PipelineBuilder pipelineBuilder,
      string sourceVersion,
      Uri repositoryUrl);

    void DeleteTriggers(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> definitionIds);
  }
}
