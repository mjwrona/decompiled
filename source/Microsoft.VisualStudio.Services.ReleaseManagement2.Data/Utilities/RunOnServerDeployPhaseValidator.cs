// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.RunOnServerDeployPhaseValidator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public class RunOnServerDeployPhaseValidator : BasePhaseValidator
  {
    public RunOnServerDeployPhaseValidator(string deployPhaseName)
      : base(deployPhaseName)
    {
    }

    protected override IDictionary<string, string> GetInvalidInputs(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, ConfigurationVariableValue> variables,
      IVssRequestContext context)
    {
      Dictionary<string, string> invalidInputs = new Dictionary<string, string>();
      ServerDeploymentInput serverDeploymentInput = (ServerDeploymentInput) deploymentInput;
      ParallelExecutionTypes parallelExecutionType = serverDeploymentInput.ParallelExecution.ParallelExecutionType;
      switch (parallelExecutionType)
      {
        case ParallelExecutionTypes.None:
        case ParallelExecutionTypes.MultiConfiguration:
          foreach (KeyValuePair<string, string> invalidInput in (IEnumerable<KeyValuePair<string, string>>) ParallelExecutionHandlerFactory.GetHandler(serverDeploymentInput.ParallelExecution, (Action<string>) (messages => { })).GetInvalidInputs(variables, context))
          {
            if (!invalidInputs.ContainsKey(invalidInput.Key))
              invalidInputs[invalidInput.Key] = invalidInput.Value;
          }
          return (IDictionary<string, string>) invalidInputs;
        default:
          invalidInputs["ParallelExecutionType"] = parallelExecutionType.ToString();
          goto case ParallelExecutionTypes.None;
      }
    }

    protected override IList<string> GetModifiedImmutableProperties(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput)
    {
      ServerDeploymentInput serverDeploymentInput1 = (ServerDeploymentInput) webApiDeploymentInput;
      ServerDeploymentInput deploymentInput2 = (ServerDeploymentInput) serverDeploymentInput;
      List<string> immutableProperties = new List<string>();
      if (serverDeploymentInput1.ParallelExecution.ParallelExecutionType != deploymentInput2.ParallelExecution.ParallelExecutionType)
        immutableProperties.Add(BasePhaseValidator.GetPropertyName<ParallelExecutionTypes>((Expression<Func<ParallelExecutionTypes>>) (() => deploymentInput2.ParallelExecution.ParallelExecutionType)));
      return (IList<string>) immutableProperties;
    }

    protected override IList<string> GetModifiedProperties(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput)
    {
      ServerDeploymentInput deploymentInput1 = (ServerDeploymentInput) webApiDeploymentInput;
      ServerDeploymentInput serverDeploymentInput1 = (ServerDeploymentInput) serverDeploymentInput;
      List<string> modifiedProperties = new List<string>();
      if (!deploymentInput1.ParallelExecution.Equals(serverDeploymentInput1.ParallelExecution))
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<ExecutionInput>((Expression<Func<ExecutionInput>>) (() => deploymentInput1.ParallelExecution)));
      return (IList<string>) modifiedProperties;
    }

    public override void ValidateDeploymentInputOverrideInputs(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, ConfigurationVariableValue> variables,
      string phaseName)
    {
    }

    public override void ValidateArtifactsDownloadInput(
      BaseDeploymentInput deploymentInput,
      IList<Artifact> linkedArtifacts)
    {
    }
  }
}
