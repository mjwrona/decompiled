// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildOption
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [InheritedExport]
  public interface IBuildOption
  {
    void ApplyPreValidation(
      BuildOption option,
      IOrchestrationEnvironment environment,
      List<TaskOrchestrationJob> jobs);

    void ApplyPreValidation(BuildOption option, IOrchestrationEnvironment environment);

    void PostBuildOperations(
      IVssRequestContext requestContext,
      BuildOption option,
      TaskOrchestrationPlan plan,
      BuildData build,
      BuildDefinition definition);

    BuildOptionDefinition GetDefinition(IVssRequestContext requestContext, Guid projectId);

    bool Validate(
      BuildOption option,
      IDictionary<string, BuildDefinitionVariable> variables,
      out string errorMessage);

    void AfterDeserialize(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.WebApi.BuildOption option,
      Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition);

    void AfterDeserialize(
      IVssRequestContext requestContext,
      BuildOption option,
      BuildDefinition definition);

    void SetAdditionalContainerInputs(BuildOption option, TaskOrchestrationContainer container);

    void CheckSupported(
      IVssRequestContext requestContext,
      ApiResourceVersion apiResourceVersion,
      int definitionProcessType);

    bool IsSupported(
      IVssRequestContext requestContext,
      ApiResourceVersion apiResourceVersion,
      int definitionProcessType);
  }
}
