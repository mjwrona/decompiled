// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Builders.YamlReleaseBuilder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Yaml;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Builders
{
  internal class YamlReleaseBuilder : ReleaseBuilderBase, IReleaseBuilder
  {
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "To be refactored.")]
    public Release Build(
      ReleaseDefinition definition,
      CreateReleaseParameters createReleaseParameters,
      IVssRequestContext requestContext,
      ReleaseProjectInfo projectInfo)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      if (createReleaseParameters == null)
        throw new ArgumentNullException(nameof (createReleaseParameters));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlPipelineProcess pipelineProcess = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlPipelineProcess) definition.PipelineProcess;
      YamlLoadResult yamlLoadResult = YamlHelper.Load(requestContext, projectInfo.Id, definition.Id, pipelineProcess.FileSource, pipelineProcess.FileName, pipelineProcess.Resources, false);
      if (yamlLoadResult.Errors.Any<string>())
        throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.YamlPipelineNotValid, (object) string.Join(",", (IEnumerable<string>) yamlLoadResult.Errors)));
      Release release = new Release()
      {
        ReleaseDefinitionId = definition.Id,
        ReleaseDefinitionRevision = definition.Revision,
        Status = createReleaseParameters.IsDraft ? ReleaseStatus.Draft : ReleaseStatus.Active,
        Description = createReleaseParameters.Description,
        CreatedBy = createReleaseParameters.CreatedBy,
        CreatedFor = createReleaseParameters.CreatedFor,
        ModifiedBy = createReleaseParameters.CreatedBy,
        Reason = createReleaseParameters.Reason,
        ReleaseNameFormat = YamlReleaseBuilder.SetReleaseFormatMask(yamlLoadResult.PipelineTemplate)
      };
      release.DefinitionSnapshot = new ReleaseDefinitionEnvironmentsSnapshot();
      foreach (Stage stage in (IEnumerable<Stage>) yamlLoadResult.PipelineTemplate.Stages)
      {
        Stage yamlEnvironment = stage;
        DefinitionEnvironment definitionEnvironment = definition.Environments.SingleOrDefault<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (x => string.Equals(x.Name, yamlEnvironment.Name, StringComparison.OrdinalIgnoreCase)));
        if (definitionEnvironment != null)
        {
          DefinitionEnvironmentData definitionEnvironmentData = new DefinitionEnvironmentData();
          definitionEnvironmentData.Name = definitionEnvironment.Name;
          definitionEnvironmentData.Id = definitionEnvironment.Id;
          definitionEnvironmentData.Rank = definitionEnvironment.Rank;
          DefinitionEnvironmentStepData environmentStepData1 = new DefinitionEnvironmentStepData()
          {
            ApproverId = Guid.Empty,
            DefinitionEnvironmentId = definitionEnvironment.Id,
            Rank = 1,
            StepType = EnvironmentStepType.PreDeploy,
            IsAutomated = true
          };
          DefinitionEnvironmentStepData environmentStepData2 = new DefinitionEnvironmentStepData()
          {
            ApproverId = Guid.Empty,
            DefinitionEnvironmentId = definitionEnvironment.Id,
            Rank = 2,
            StepType = EnvironmentStepType.Deploy,
            IsAutomated = true
          };
          DefinitionEnvironmentStepData environmentStepData3 = new DefinitionEnvironmentStepData()
          {
            ApproverId = Guid.Empty,
            DefinitionEnvironmentId = definitionEnvironment.Id,
            Rank = 3,
            StepType = EnvironmentStepType.PostDeploy,
            IsAutomated = true
          };
          definitionEnvironmentData.Steps.Add(environmentStepData1);
          definitionEnvironmentData.Steps.Add(environmentStepData2);
          definitionEnvironmentData.Steps.Add(environmentStepData3);
          release.DefinitionSnapshot.Environments.Add(definitionEnvironmentData);
          ReleaseEnvironment releaseEnvironment = new ReleaseEnvironment()
          {
            Name = definitionEnvironment.Name,
            DefinitionEnvironmentId = definitionEnvironment.Id,
            Rank = definitionEnvironment.Rank,
            OwnerId = definitionEnvironment.OwnerId,
            PreApprovalOptions = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions(),
            PostApprovalOptions = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions(),
            EnvironmentOptions = new EnvironmentOptions(),
            ProcessParameters = new ProcessParameters(),
            PreDeploymentGates = new ReleaseDefinitionGatesStep(),
            PostDeploymentGates = new ReleaseDefinitionGatesStep()
          };
          releaseEnvironment.DeploymentSnapshot = (IDeploymentSnapshot) new YamlDeploymentSnapshot()
          {
            Process = (IOrchestrationProcess) new Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineProcess((IList<Stage>) new Stage[1]
            {
              yamlEnvironment
            })
          };
          releaseEnvironment.Conditions.Clear();
          releaseEnvironment.Conditions.Add(new ReleaseCondition("ReleaseStarted", ConditionType.Event, (string) null, new bool?()));
          release.Environments.Add(releaseEnvironment);
        }
      }
      VariablesUtility.FillVariables(definition.Variables, release.Variables);
      ReleaseBuilderBase.PopulateArtifactData(release, definition, createReleaseParameters.ArtifactData, requestContext, createReleaseParameters.TriggeringArtifactAlias);
      this.ReleaseNameFiller(release, definition, requestContext, projectInfo.Name);
      ReleaseManagementArtifactPropertyKinds.CopyProperties(release.Properties, createReleaseParameters.Properties);
      return release;
    }

    private static string SetReleaseFormatMask(PipelineTemplate pipeline) => !string.IsNullOrWhiteSpace(pipeline.Name) ? pipeline.Name : "Release-$(Rev:r)";
  }
}
