// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildDefinitionStepExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildDefinitionStepExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep ToWebApiBuildDefinitionStep(
      this Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep srvBuildDefinitionStep,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBuildDefinitionStep == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep) null;
      Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep buildDefinitionStep = new Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep(securedObject)
      {
        Inputs = srvBuildDefinitionStep.Inputs,
        Enabled = srvBuildDefinitionStep.Enabled,
        ContinueOnError = srvBuildDefinitionStep.ContinueOnError,
        AlwaysRun = srvBuildDefinitionStep.AlwaysRun,
        DisplayName = srvBuildDefinitionStep.DisplayName,
        TimeoutInMinutes = srvBuildDefinitionStep.TimeoutInMinutes,
        RetryCountOnTaskFailure = srvBuildDefinitionStep.RetryCountOnTaskFailure,
        Condition = srvBuildDefinitionStep.Condition,
        RefName = srvBuildDefinitionStep.RefName,
        Environment = srvBuildDefinitionStep.Environment
      };
      if (srvBuildDefinitionStep.TaskDefinition != null)
        buildDefinitionStep.TaskDefinition = new Microsoft.TeamFoundation.Build.WebApi.TaskDefinitionReference(securedObject)
        {
          Id = srvBuildDefinitionStep.TaskDefinition.Id,
          VersionSpec = srvBuildDefinitionStep.TaskDefinition.VersionSpec,
          DefinitionType = srvBuildDefinitionStep.TaskDefinition.DefinitionType
        };
      return buildDefinitionStep;
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep ToBuildServerBuildDefinitionStep(
      this Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep webApiBuildDefinitionStep)
    {
      if (webApiBuildDefinitionStep == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep) null;
      Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep buildDefinitionStep = new Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep()
      {
        Inputs = webApiBuildDefinitionStep.Inputs,
        Enabled = webApiBuildDefinitionStep.Enabled,
        ContinueOnError = webApiBuildDefinitionStep.ContinueOnError,
        AlwaysRun = webApiBuildDefinitionStep.AlwaysRun,
        DisplayName = webApiBuildDefinitionStep.DisplayName,
        TimeoutInMinutes = webApiBuildDefinitionStep.TimeoutInMinutes,
        RetryCountOnTaskFailure = webApiBuildDefinitionStep.RetryCountOnTaskFailure,
        Condition = webApiBuildDefinitionStep.Condition,
        RefName = webApiBuildDefinitionStep.RefName,
        Environment = webApiBuildDefinitionStep.Environment
      };
      if (webApiBuildDefinitionStep.TaskDefinition != null)
        buildDefinitionStep.TaskDefinition = new Microsoft.TeamFoundation.Build2.Server.TaskDefinitionReference()
        {
          Id = webApiBuildDefinitionStep.TaskDefinition.Id,
          VersionSpec = webApiBuildDefinitionStep.TaskDefinition.VersionSpec,
          DefinitionType = webApiBuildDefinitionStep.TaskDefinition.DefinitionType
        };
      return buildDefinitionStep;
    }
  }
}
