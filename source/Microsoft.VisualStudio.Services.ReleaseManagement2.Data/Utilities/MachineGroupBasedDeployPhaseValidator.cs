// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.MachineGroupBasedDeployPhaseValidator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public class MachineGroupBasedDeployPhaseValidator : DeployPhaseValidator
  {
    public MachineGroupBasedDeployPhaseValidator(string deployPhaseName)
      : base(deployPhaseName)
    {
    }

    protected override IDictionary<string, string> GetInvalidInputs(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, ConfigurationVariableValue> variables,
      IVssRequestContext context)
    {
      IDictionary<string, string> invalidInputs = base.GetInvalidInputs(deploymentInput, variables, context);
      MachineGroupDeploymentInput groupDeploymentInput = (MachineGroupDeploymentInput) deploymentInput;
      if (groupDeploymentInput.HealthPercent < 0 || groupDeploymentInput.HealthPercent > 100)
        invalidInputs.Add("HealthPercent", groupDeploymentInput.HealthPercent.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.Equals(groupDeploymentInput.DeploymentHealthOption, "AllTargetsInParallel") && !string.Equals(groupDeploymentInput.DeploymentHealthOption, "HalfOfTargetsInParallel") && !string.Equals(groupDeploymentInput.DeploymentHealthOption, "QuarterOfTargetsInParallel") && !string.Equals(groupDeploymentInput.DeploymentHealthOption, "OneTargetAtATime") && !string.Equals(groupDeploymentInput.DeploymentHealthOption, "Custom") && !string.IsNullOrEmpty(groupDeploymentInput.DeploymentHealthOption))
        invalidInputs.Add("DeploymentHealthOption", groupDeploymentInput.DeploymentHealthOption);
      return invalidInputs;
    }

    protected override IList<string> GetModifiedProperties(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput)
    {
      MachineGroupDeploymentInput deploymentInput1 = (MachineGroupDeploymentInput) webApiDeploymentInput;
      MachineGroupDeploymentInput groupDeploymentInput = (MachineGroupDeploymentInput) serverDeploymentInput;
      IList<string> modifiedProperties = base.GetModifiedProperties(webApiDeploymentInput, serverDeploymentInput);
      if (deploymentInput1.HealthPercent != groupDeploymentInput.HealthPercent)
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<int>((Expression<Func<int>>) (() => deploymentInput1.HealthPercent)));
      if (!string.Equals(deploymentInput1.DeploymentHealthOption, groupDeploymentInput.DeploymentHealthOption))
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<string>((Expression<Func<string>>) (() => deploymentInput1.DeploymentHealthOption)));
      return modifiedProperties;
    }

    public override void ValidateDeploymentInputOverrideInputs(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, ConfigurationVariableValue> variables,
      string phaseName)
    {
      if (deploymentInput == null)
        return;
      BasePhaseValidator.ValidateOverrideInputs((IList<PropertyInfo>) typeof (MachineGroupDeploymentInput).GetProperties(BindingFlags.Instance | BindingFlags.Public), deploymentInput.OverrideInputs, variables, phaseName);
    }
  }
}
