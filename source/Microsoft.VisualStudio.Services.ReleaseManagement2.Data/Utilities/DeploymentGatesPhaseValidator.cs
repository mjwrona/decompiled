// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.DeploymentGatesPhaseValidator
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

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  internal class DeploymentGatesPhaseValidator : BasePhaseValidator
  {
    public DeploymentGatesPhaseValidator(string deployPhaseName)
      : base(deployPhaseName)
    {
    }

    public override void ValidateArtifactsDownloadInput(
      BaseDeploymentInput deploymentInput,
      IList<Artifact> linkedArtifacts)
    {
    }

    public override void ValidateDeploymentInputOverrideInputs(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, ConfigurationVariableValue> variables,
      string phaseName)
    {
    }

    protected override IDictionary<string, string> GetInvalidInputs(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, ConfigurationVariableValue> variables,
      IVssRequestContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      Dictionary<string, string> invalidInputs = new Dictionary<string, string>();
      GatesDeploymentInput gatesDeploymentInput = (GatesDeploymentInput) deploymentInput;
      if (gatesDeploymentInput.StabilizationTime < 0 || gatesDeploymentInput.StabilizationTime > 2880)
        invalidInputs.Add("StabilizationTime", gatesDeploymentInput.StabilizationTime.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (gatesDeploymentInput.MinimumSuccessDuration < 0 || gatesDeploymentInput.MinimumSuccessDuration > 2880)
        invalidInputs.Add("MinimumSuccessDuration", gatesDeploymentInput.MinimumSuccessDuration.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      int intervalInMinutes = DeploymentGatesHelper.GetMinimalSamplingIntervalInMinutes(context, new int?(gatesDeploymentInput.TimeoutInMinutes));
      if (gatesDeploymentInput.SamplingInterval < intervalInMinutes || gatesDeploymentInput.SamplingInterval > 1440)
        invalidInputs.Add("SamplingInterval", gatesDeploymentInput.SamplingInterval.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return (IDictionary<string, string>) invalidInputs;
    }

    protected override IList<string> GetModifiedImmutableProperties(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput)
    {
      return (IList<string>) new List<string>();
    }

    protected override IList<string> GetModifiedProperties(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput)
    {
      List<string> modifiedProperties = new List<string>();
      GatesDeploymentInput deploymentInput1 = (GatesDeploymentInput) webApiDeploymentInput;
      GatesDeploymentInput gatesDeploymentInput = (GatesDeploymentInput) serverDeploymentInput;
      if (deploymentInput1.StabilizationTime != gatesDeploymentInput.StabilizationTime)
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<int>((Expression<Func<int>>) (() => deploymentInput1.StabilizationTime)));
      if (deploymentInput1.MinimumSuccessDuration != gatesDeploymentInput.MinimumSuccessDuration)
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<int>((Expression<Func<int>>) (() => deploymentInput1.MinimumSuccessDuration)));
      if (deploymentInput1.SamplingInterval != gatesDeploymentInput.SamplingInterval)
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<int>((Expression<Func<int>>) (() => deploymentInput1.SamplingInterval)));
      return (IList<string>) modifiedProperties;
    }
  }
}
