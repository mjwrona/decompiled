// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.ReleaseEnvironmentSecurityValidations
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public class ReleaseEnvironmentSecurityValidations
  {
    private IVssRequestContext context;
    private Guid projectId;
    private string folderPath;
    private Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release updatedRelease;
    private Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release existingRelease;
    private Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release existingServerRelease;
    private IDistributedTaskPoolService poolService;
    private IList<TaskDefinition> allTaskDefinitions;

    protected ReleaseEnvironmentSecurityValidations(
      IVssRequestContext context,
      Guid projectId,
      string folderPath,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release updatedRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release existingRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release existingServerRelease)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (updatedRelease == null)
        throw new ArgumentNullException(nameof (updatedRelease));
      if (existingRelease == null)
        throw new ArgumentNullException(nameof (existingRelease));
      this.context = context;
      this.projectId = projectId;
      this.folderPath = folderPath;
      this.updatedRelease = updatedRelease;
      this.existingRelease = existingRelease;
      this.existingServerRelease = existingServerRelease;
    }

    public ReleaseEnvironmentSecurityValidations(
      IVssRequestContext context,
      Guid projectId,
      string folderPath,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release updatedRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release existingServerRelease)
      : this(context, projectId, folderPath, updatedRelease, existingServerRelease.ToContract(context, projectId, true), existingServerRelease)
    {
    }

    public void CheckSecurityPermission()
    {
      this.CheckManageReleaseApproversPermission();
      this.CheckEditReleaseEnvironmentPermission();
    }

    public void CheckUsePermissionOnQueuesAddedOrChanged()
    {
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> releaseVariables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[2]
      {
        this.updatedRelease.Variables,
        VariableGroupsMerger.GetMergedGroupVariables(this.updatedRelease.VariableGroups)
      });
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) this.updatedRelease.Environments)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment updatedEnv = environment;
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment releaseEnvironment = this.existingRelease.Environments.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, bool>) (x => x.Id == updatedEnv.Id));
        IEnumerable<int> allQueueIds1 = updatedEnv.GetAllQueueIds(releaseVariables);
        if (releaseEnvironment == null)
        {
          this.CheckUsePermissionForQueueIds(allQueueIds1);
        }
        else
        {
          IEnumerable<int> allQueueIds2 = releaseEnvironment.GetAllQueueIds(releaseVariables);
          this.CheckUsePermissionForQueueIds(allQueueIds1.Except<int>(allQueueIds2));
        }
      }
    }

    public void CheckUsePermissionOnMachineGroupsAddedOrChanged()
    {
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> releaseVariables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[2]
      {
        this.updatedRelease.Variables,
        VariableGroupsMerger.GetMergedGroupVariables(this.updatedRelease.VariableGroups)
      });
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) this.updatedRelease.Environments)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment updatedEnv = environment;
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment releaseEnvironment = this.existingRelease.Environments.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, bool>) (x => x.Id == updatedEnv.Id));
        IEnumerable<int> allMachineGroupIds1 = updatedEnv.GetAllMachineGroupIds(releaseVariables);
        if (releaseEnvironment == null)
        {
          this.CheckUsePermissionForMachineGroupIds(allMachineGroupIds1);
        }
        else
        {
          IEnumerable<int> allMachineGroupIds2 = releaseEnvironment.GetAllMachineGroupIds(releaseVariables);
          this.CheckUsePermissionForMachineGroupIds(allMachineGroupIds1.Except<int>(allMachineGroupIds2));
        }
      }
    }

    private static bool AreApproversChanged(
      IEnumerable<ReleaseDefinitionApprovalStep> newApprovalSteps,
      IEnumerable<ReleaseDefinitionApprovalStep> existingApprovalSteps)
    {
      if (newApprovalSteps == null)
        throw new ArgumentNullException(nameof (newApprovalSteps));
      if (existingApprovalSteps == null)
        throw new ArgumentNullException(nameof (existingApprovalSteps));
      return newApprovalSteps.Any<ReleaseDefinitionApprovalStep>() && ReleaseDefinitionApproverValidations.CompareReleaseApprovers(newApprovalSteps, existingApprovalSteps);
    }

    private void CheckUsePermissionForQueueIds(IEnumerable<int> queueIds)
    {
      foreach (int queueId in queueIds)
        this.PoolService.CheckUsePermissionForQueue(this.context, this.projectId, queueId);
    }

    private void CheckUsePermissionForMachineGroupIds(IEnumerable<int> machineGroupIds)
    {
      foreach (int machineGroupId in machineGroupIds)
        this.PoolService.CheckUsePermissionForDeploymentGroup(this.context, this.projectId, machineGroupId);
    }

    private IDistributedTaskPoolService PoolService
    {
      get
      {
        if (this.poolService == null)
          this.poolService = this.context.GetService<IDistributedTaskPoolService>();
        return this.poolService;
      }
    }

    private void CheckManageReleaseApproversPermission()
    {
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> approverChangedEnvironments = new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>();
      if (!this.TryGetEnvironmentsWithApproverChanged(out approverChangedEnvironments))
        return;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in approverChangedEnvironments)
        this.EnsureSecurityPermission(environment, ReleaseManagementSecurityPermissions.ManageReleaseApprovers);
    }

    public void CheckEditReleaseEnvironmentPermission()
    {
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> editedEnvironments = new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>();
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> releaseEnvironmentList = new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>();
      if (this.TryGetEditedEnvironments(out editedEnvironments))
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in editedEnvironments)
        {
          this.EnsureSecurityPermission(environment, ReleaseManagementSecurityPermissions.EditReleaseEnvironment);
          releaseEnvironmentList.Add(environment);
        }
      }
      if (this.existingRelease != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment releaseEnvironment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) ServiceEndpointVariablesHelper.GetReleaseEnvironmentsImpactedByVariableUpdate(this.context, this.existingRelease, this.updatedRelease, this.AllTaskDefinitions))
        {
          if (!releaseEnvironmentList.Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, int>) (env => env.DefinitionEnvironmentId)).Contains<int>(releaseEnvironment.DefinitionEnvironmentId))
            releaseEnvironmentList.Add(releaseEnvironment);
        }
      }
      if (!releaseEnvironmentList.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>())
        return;
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> dictionary = this.updatedRelease.Variables.Concat<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>((IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) VariableGroupsMerger.GetMergedGroupVariables(this.updatedRelease.VariableGroups)).GroupBy<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, string>) (x => x.Key)).ToDictionary<IGrouping<string, KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>, string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((Func<IGrouping<string, KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>, string>) (y => y.Key), (Func<IGrouping<string, KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) (y => y.First<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>().Value));
      ServiceEndpointPermissionValidator.EnsureServiceEndpointSecurityPermission(this.context, this.projectId, (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) releaseEnvironmentList, (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) dictionary, this.existingRelease, this.AllTaskDefinitions);
    }

    private bool TryGetEnvironmentsWithApproverChanged(
      out List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> approverChangedEnvironments)
    {
      approverChangedEnvironments = new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>();
      bool withApproverChanged = false;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment1 in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) this.updatedRelease.Environments)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment updatedEnv = environment1;
        bool flag = false;
        DefinitionEnvironmentData definitionEnvironmentData = this.existingServerRelease.DefinitionSnapshot.Environments.FirstOrDefault<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (x => x.Id == updatedEnv.DefinitionEnvironmentId));
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment2 = this.existingServerRelease.GetEnvironment(updatedEnv.Id);
        if (definitionEnvironmentData != null)
        {
          IEnumerable<DefinitionEnvironmentStepData> environmentStepDatas1 = definitionEnvironmentData.Steps.Where<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (x => x.StepType == EnvironmentStepType.PreDeploy));
          IEnumerable<DefinitionEnvironmentStepData> environmentStepDatas2 = definitionEnvironmentData.Steps.Where<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (x => x.StepType == EnvironmentStepType.PostDeploy));
          if (ReleaseEnvironmentSecurityValidations.AreApproversChanged((IEnumerable<ReleaseDefinitionApprovalStep>) updatedEnv.PreApprovalsSnapshot.Approvals, environmentStepDatas1.Select<DefinitionEnvironmentStepData, ReleaseDefinitionApprovalStep>((Func<DefinitionEnvironmentStepData, ReleaseDefinitionApprovalStep>) (x => x.ToReleaseDefinitionApprovalStep()))) || ReleaseEnvironmentSecurityValidations.AreApproversChanged((IEnumerable<ReleaseDefinitionApprovalStep>) updatedEnv.PostApprovalsSnapshot.Approvals, environmentStepDatas2.Select<DefinitionEnvironmentStepData, ReleaseDefinitionApprovalStep>((Func<DefinitionEnvironmentStepData, ReleaseDefinitionApprovalStep>) (x => x.ToReleaseDefinitionApprovalStep()))))
          {
            approverChangedEnvironments.Add(updatedEnv);
            withApproverChanged = true;
            flag = true;
          }
          if (!flag && environment2 != null && (ReleaseEnvironmentValidations.AreApprovalOptionsModified(updatedEnv.PreApprovalsSnapshot, environment2.PreApprovalOptions, environmentStepDatas1) || ReleaseEnvironmentValidations.AreApprovalOptionsModified(updatedEnv.PostApprovalsSnapshot, environment2.PostApprovalOptions, environmentStepDatas2) || !updatedEnv.PreDeploymentGatesSnapshot.AreReleaseDefinitionGatesEqual(environment2.PreDeploymentGates) || !updatedEnv.PostDeploymentGatesSnapshot.AreReleaseDefinitionGatesEqual(environment2.PostDeploymentGates)))
          {
            approverChangedEnvironments.Add(updatedEnv);
            withApproverChanged = true;
          }
        }
      }
      return withApproverChanged;
    }

    private bool TryGetEditedEnvironments(out List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> editedEnvironments)
    {
      editedEnvironments = new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>();
      bool editedEnvironments1 = false;
      if (this.existingServerRelease != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) this.updatedRelease.Environments)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment updatedEnv = environment;
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment serverEnvironment = this.existingServerRelease.Environments.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (x => x.Id == updatedEnv.Id));
          DefinitionEnvironmentData definitionEnvironmentData = this.existingServerRelease.GetDefinitionEnvironmentData(updatedEnv.Name);
          if (serverEnvironment != null && !ReleaseEnvironmentValidations.AreEnvironmentsSame(updatedEnv, serverEnvironment, definitionEnvironmentData))
          {
            editedEnvironments.Add(updatedEnv);
            editedEnvironments1 = true;
          }
        }
      }
      return editedEnvironments1;
    }

    private void EnsureSecurityPermission(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      ReleaseManagementSecurityPermissions permission)
    {
      if (!this.DoesUserHasSecurityPermission(environment, permission))
      {
        ResourceAccessException innerException = new ResourceAccessException(this.context.RootContext.GetUserId().ToString(), permission);
        throw new UnauthorizedRequestException(innerException.Message, (Exception) innerException);
      }
    }

    private bool DoesUserHasSecurityPermission(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      ReleaseManagementSecurityPermissions permission)
    {
      return this.context.HasPermission(this.projectId, this.folderPath, this.existingRelease.ReleaseDefinitionReference.Id, environment.DefinitionEnvironmentId, permission);
    }

    private IList<TaskDefinition> AllTaskDefinitions
    {
      get
      {
        if (this.allTaskDefinitions == null || this.allTaskDefinitions.Count<TaskDefinition>() <= 0)
          this.allTaskDefinitions = this.PoolService.GetTaskDefinitions(this.context.Elevate());
        return this.allTaskDefinitions;
      }
    }
  }
}
