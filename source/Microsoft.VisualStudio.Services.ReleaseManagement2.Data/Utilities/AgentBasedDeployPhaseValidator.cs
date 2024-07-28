// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.AgentBasedDeployPhaseValidator
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
using System.Reflection;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public class AgentBasedDeployPhaseValidator : DeployPhaseValidator
  {
    public AgentBasedDeployPhaseValidator(string deployPhaseName)
      : base(deployPhaseName)
    {
    }

    protected override IDictionary<string, string> GetInvalidInputs(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, ConfigurationVariableValue> variables,
      IVssRequestContext context)
    {
      IDictionary<string, string> invalidInputs = base.GetInvalidInputs(deploymentInput, variables, context);
      foreach (KeyValuePair<string, string> invalidInput in (IEnumerable<KeyValuePair<string, string>>) ParallelExecutionHandlerFactory.GetHandler(((AgentDeploymentInput) deploymentInput).ParallelExecution, (Action<string>) (messages => { })).GetInvalidInputs(variables, context))
      {
        if (!invalidInputs.ContainsKey(invalidInput.Key))
          invalidInputs[invalidInput.Key] = invalidInput.Value;
      }
      return invalidInputs;
    }

    protected override IList<string> GetModifiedProperties(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput)
    {
      AgentDeploymentInput deploymentInput1 = (AgentDeploymentInput) webApiDeploymentInput;
      AgentDeploymentInput deploymentInput2 = (AgentDeploymentInput) serverDeploymentInput;
      IList<string> modifiedProperties = base.GetModifiedProperties(webApiDeploymentInput, serverDeploymentInput);
      if (!deploymentInput1.ParallelExecution.Equals(deploymentInput2.ParallelExecution) && (!deploymentInput1.IsDefaultParallelExecutionInput() || !deploymentInput2.IsDefaultParallelExecutionInput()))
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<ExecutionInput>((Expression<Func<ExecutionInput>>) (() => deploymentInput1.ParallelExecution)));
      if (deploymentInput1.ImageId != deploymentInput2.ImageId)
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<int>((Expression<Func<int>>) (() => deploymentInput2.ImageId)));
      return modifiedProperties;
    }

    protected override IList<string> GetModifiedImmutableProperties(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput)
    {
      AgentDeploymentInput agentDeploymentInput = (AgentDeploymentInput) webApiDeploymentInput;
      AgentDeploymentInput deploymentInput2 = (AgentDeploymentInput) serverDeploymentInput;
      IList<string> immutableProperties = base.GetModifiedImmutableProperties(webApiDeploymentInput, serverDeploymentInput);
      if (agentDeploymentInput.ParallelExecution.ParallelExecutionType != deploymentInput2.ParallelExecution.ParallelExecutionType)
        immutableProperties.Add(BasePhaseValidator.GetPropertyName<ParallelExecutionTypes>((Expression<Func<ParallelExecutionTypes>>) (() => deploymentInput2.ParallelExecution.ParallelExecutionType)));
      if (agentDeploymentInput.ImageId != deploymentInput2.ImageId)
        immutableProperties.Add(BasePhaseValidator.GetPropertyName<int>((Expression<Func<int>>) (() => deploymentInput2.ImageId)));
      return immutableProperties;
    }

    public override void ValidateDeploymentInputOverrideInputs(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, ConfigurationVariableValue> variables,
      string phaseName)
    {
      if (deploymentInput == null)
        return;
      BasePhaseValidator.ValidateOverrideInputs((IList<PropertyInfo>) typeof (AgentDeploymentInput).GetProperties(BindingFlags.Instance | BindingFlags.Public), deploymentInput.OverrideInputs, variables, phaseName);
    }
  }
}
