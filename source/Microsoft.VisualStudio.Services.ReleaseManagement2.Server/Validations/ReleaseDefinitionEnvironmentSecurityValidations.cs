// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.ReleaseDefinitionEnvironmentSecurityValidations
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public class ReleaseDefinitionEnvironmentSecurityValidations
  {
    private const string TaskInputTypePrefix = "connectedService:";
    private IVssRequestContext context;
    private Guid projectId;
    private Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition updatedReleaseDefinition;
    private Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition existingReleaseDefinition;
    private IDistributedTaskPoolService poolService;
    private IList<TaskAgentQueue> allTaskAgentQueues;
    private IList<TaskDefinition> allTaskDefinitions;
    private IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> updatedRDVariables;
    private IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> existingRDVariables;
    private IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> allVariableGroupsInUpdatedRD;
    private IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> allVariableGroupsInExistingRD;
    private IDictionary<string, List<int>> environmentToQueueMap;
    private IList<int> allQueueIdsInUse;
    private int maxTopSupported = 25;
    private readonly Func<IVssRequestContext, Guid, int, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> getReleaseDefinition;

    internal ReleaseDefinitionEnvironmentSecurityValidations(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition,
      Func<IVssRequestContext, Guid, int, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> getReleaseDefinition)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      if (getReleaseDefinition == null)
        throw new ArgumentNullException(nameof (getReleaseDefinition));
      this.context = context;
      this.projectId = projectId;
      this.updatedReleaseDefinition = releaseDefinition;
      this.getReleaseDefinition = getReleaseDefinition;
      this.allVariableGroupsInUpdatedRD = ReleaseDefinitionVariableGroupUtility.GetAllVariableGroupsInRD(context, projectId, this.updatedReleaseDefinition);
      this.updatedRDVariables = ReleaseDefinitionVariableGroupUtility.GetVariableGroupVariables(this.updatedReleaseDefinition.VariableGroups, this.allVariableGroupsInUpdatedRD);
      this.environmentToQueueMap = (IDictionary<string, List<int>>) new Dictionary<string, List<int>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.allQueueIdsInUse = (IList<int>) new List<int>();
      if (this.updatedReleaseDefinition.Variables != null)
        this.updatedRDVariables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[2]
        {
          this.updatedReleaseDefinition.Variables,
          this.updatedRDVariables
        });
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) releaseDefinition.Environments)
      {
        List<int> list = environment.GetAllQueueIds(this.updatedRDVariables, this.allVariableGroupsInUpdatedRD).ToList<int>();
        this.environmentToQueueMap[environment.Name] = list;
        this.allQueueIdsInUse.AddRange<int, IList<int>>((IEnumerable<int>) list);
      }
      this.allQueueIdsInUse = (IList<int>) this.allQueueIdsInUse.Distinct<int>().ToList<int>();
    }

    public ReleaseDefinitionEnvironmentSecurityValidations(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
      : this(context, projectId, releaseDefinition, ReleaseDefinitionEnvironmentSecurityValidations.\u003C\u003EO.\u003C0\u003E__GetReleaseDefinition ?? (ReleaseDefinitionEnvironmentSecurityValidations.\u003C\u003EO.\u003C0\u003E__GetReleaseDefinition = new Func<IVssRequestContext, Guid, int, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition>(ReleaseDefinitionEnvironmentSecurityValidations.GetReleaseDefinition)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public void CheckSecurityPermissionForEdit()
    {
      this.CheckDeleteEnvironmentPermission();
      this.CheckManageReleaseApproversPermission();
      this.CheckEditReleaseEnvironmentPermission();
      this.CheckEndpointsPermissionOnNewlyAddedEnvironments();
    }

    public void CheckEndpointsPermissionOnEnvironmentsForCreate() => ServiceEndpointPermissionValidator.EnsureServiceEndpointSecurityPermission(this.context, this.projectId, (IEnumerable<ReleaseDefinitionEnvironment>) this.updatedReleaseDefinition.Environments, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseDefinitionExtensions.GetVariablesFromReleaseDefinition(this.updatedReleaseDefinition, this.context, this.projectId), this.ExistingReleaseDefinition, this.AllTaskDefinitions);

    public void CheckExistenceOfUsedResources()
    {
      this.EnsureTaskAgentQueuesExists(this.updatedReleaseDefinition.Environments);
      this.EnsureDeploymentGroupsExists(this.updatedReleaseDefinition.Environments);
    }

    public void CheckExistenceOfUsedResourcesInChangedEnvironments()
    {
      List<ReleaseDefinitionEnvironment> editedEnvironments = new List<ReleaseDefinitionEnvironment>();
      if (!this.TryGetEditedEnvironments(out editedEnvironments))
        return;
      this.EnsureTaskAgentQueuesExists((IList<ReleaseDefinitionEnvironment>) editedEnvironments);
      this.EnsureDeploymentGroupsExists((IList<ReleaseDefinitionEnvironment>) editedEnvironments);
    }

    public void CheckUsePermissionOnQueuesUsed()
    {
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) this.updatedReleaseDefinition.Environments)
        this.CheckUsePermissionForQueueIds((IEnumerable<int>) this.GetQueueIdsUsedInEnvironment(environment.Name));
    }

    public void CheckUsePermissionOnQueuesAddedOrChanged()
    {
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) this.updatedReleaseDefinition.Environments)
      {
        ReleaseDefinitionEnvironment updatedEnv = environment;
        ReleaseDefinitionEnvironment releaseDefinitionEnvironment = this.ExistingReleaseDefinition.Environments.FirstOrDefault<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (x => x.Id == updatedEnv.Id));
        IList<int> usedInEnvironment = this.GetQueueIdsUsedInEnvironment(updatedEnv.Name);
        if (releaseDefinitionEnvironment == null)
        {
          this.CheckUsePermissionForQueueIds((IEnumerable<int>) usedInEnvironment);
        }
        else
        {
          IEnumerable<int> allQueueIds = releaseDefinitionEnvironment.GetAllQueueIds(this.existingRDVariables, this.allVariableGroupsInExistingRD);
          this.CheckUsePermissionForQueueIds(usedInEnvironment.Except<int>(allQueueIds));
        }
      }
    }

    public void CheckUsePermissionOnMachineGroupsUsed()
    {
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) this.updatedReleaseDefinition.Environments)
        this.CheckUsePermissionForMachineGroupIds(environment.GetAllMachineGroupIds(this.updatedRDVariables, this.allVariableGroupsInUpdatedRD));
    }

    public void CheckUsePermissionOnMachineGroupsAddedOrChanged()
    {
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) this.updatedReleaseDefinition.Environments)
      {
        ReleaseDefinitionEnvironment updatedEnv = environment;
        ReleaseDefinitionEnvironment releaseDefinitionEnvironment = this.ExistingReleaseDefinition.Environments.FirstOrDefault<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (x => x.Id == updatedEnv.Id));
        IEnumerable<int> allMachineGroupIds1 = updatedEnv.GetAllMachineGroupIds(this.updatedRDVariables, this.allVariableGroupsInUpdatedRD);
        if (releaseDefinitionEnvironment == null)
        {
          this.CheckUsePermissionForMachineGroupIds(allMachineGroupIds1);
        }
        else
        {
          IEnumerable<int> allMachineGroupIds2 = releaseDefinitionEnvironment.GetAllMachineGroupIds(this.existingRDVariables, this.allVariableGroupsInExistingRD);
          this.CheckUsePermissionForMachineGroupIds(allMachineGroupIds1.Except<int>(allMachineGroupIds2));
        }
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This has to do active processing of release definitions, so better be a method.")]
    public IEnumerable<ReleaseDefinitionEnvironment> GetDeletedEnvironments() => this.updatedReleaseDefinition.Environments.Count < this.ExistingReleaseDefinition.Environments.Count ? this.ExistingReleaseDefinition.Environments.Except<ReleaseDefinitionEnvironment>((IEnumerable<ReleaseDefinitionEnvironment>) this.updatedReleaseDefinition.Environments, (IEqualityComparer<ReleaseDefinitionEnvironment>) new ReleaseDefinitionEnvironmentSecurityValidations.ReleaseDefinitionEnvironmentIdComparer()) : (IEnumerable<ReleaseDefinitionEnvironment>) null;

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition GetReleaseDefinition(
      IVssRequestContext context,
      Guid projectId,
      int id)
    {
      return Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseDefinitionExtensions.GetReleaseDefinition(context.GetService<ReleaseDefinitionsService>(), context, projectId, id, (IEnumerable<string>) null);
    }

    private static void ReplaceExistingEnvironmentNameWithUpdatedEnvironmentNameInConditions(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition existingReleaseDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition updatedReleaseDefinition)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) existingReleaseDefinition.Environments)
      {
        ReleaseDefinitionEnvironment existingEnv = environment;
        ReleaseDefinitionEnvironment definitionEnvironment = updatedReleaseDefinition.Environments.FirstOrDefault<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (x => x.Id == existingEnv.Id));
        if (definitionEnvironment != null)
          dictionary[existingEnv.Name] = definitionEnvironment.Name;
      }
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) existingReleaseDefinition.Environments)
      {
        foreach (Condition condition in (IEnumerable<Condition>) environment.Conditions)
        {
          if (condition.ConditionType.Equals((object) ConditionType.EnvironmentState) && dictionary.ContainsKey(condition.Name))
            condition.Name = dictionary[condition.Name];
        }
      }
    }

    private IList<int> GetQueueIdsUsedInEnvironment(string environmentName) => !this.environmentToQueueMap.ContainsKey(environmentName) ? (IList<int>) new List<int>() : (IList<int>) this.environmentToQueueMap[environmentName];

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

    private void EnsureTaskAgentQueuesExists(IList<ReleaseDefinitionEnvironment> environments)
    {
      IEnumerable<int> allTaskAgentQueuesInUse = this.AllTaskAgentQueues.Select<TaskAgentQueue, int>((Func<TaskAgentQueue, int>) (q => q.Id));
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) environments)
      {
        IList<int> usedInEnvironment = this.GetQueueIdsUsedInEnvironment(environment.Name);
        if (usedInEnvironment.Any<int>())
        {
          List<int> list = usedInEnvironment.Where<int>((Func<int, bool>) (x => !allTaskAgentQueuesInUse.Contains<int>(x))).ToList<int>();
          if (list.Any<int>())
          {
            string str = string.Join<int>(", ", (IEnumerable<int>) list);
            throw new ReleaseManagementUnauthorizedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.QueueNotFound, (object) environment.Name, (object) str));
          }
        }
      }
    }

    private void EnsureDeploymentGroupsExists(IList<ReleaseDefinitionEnvironment> environments)
    {
      HashSet<int> existingDeploymentGroupIds = new HashSet<int>();
      HashSet<int> source = new HashSet<int>();
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) environments)
        source.UnionWith(environment.GetAllMachineGroupIds(this.updatedRDVariables, this.allVariableGroupsInUpdatedRD));
      List<int> list = source.ToList<int>();
      for (int index = 0; index < list.Count; index += this.maxTopSupported)
      {
        IList<DeploymentGroup> deploymentGroupsByIds = this.GetDeploymentGroupsByIds((IList<int>) list.GetRange(index, Math.Min(this.maxTopSupported, list.Count - index)));
        existingDeploymentGroupIds.UnionWith(deploymentGroupsByIds.Select<DeploymentGroup, int>((Func<DeploymentGroup, int>) (d => d.Id)));
      }
      if (!source.Any<int>())
        return;
      IEnumerable<int> ints = source.Where<int>((Func<int, bool>) (x => !existingDeploymentGroupIds.Contains(x)));
      if (ints.Any<int>())
        throw new ReleaseManagementUnauthorizedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.DeploymentGroupNotFound, (object) string.Join<int>(", ", ints)));
    }

    private Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition ExistingReleaseDefinition
    {
      get
      {
        if (this.existingReleaseDefinition == null && this.updatedReleaseDefinition != null && this.updatedReleaseDefinition.Id != 0)
        {
          this.existingReleaseDefinition = this.getReleaseDefinition(this.context, this.projectId, this.updatedReleaseDefinition.Id);
          if (this.existingReleaseDefinition != null)
          {
            ReleaseDefinitionEnvironmentSecurityValidations.ReplaceExistingEnvironmentNameWithUpdatedEnvironmentNameInConditions(this.existingReleaseDefinition, this.updatedReleaseDefinition);
            this.allVariableGroupsInExistingRD = ReleaseDefinitionVariableGroupUtility.GetAllVariableGroupsInRD(this.context, this.projectId, this.existingReleaseDefinition);
            this.existingRDVariables = ReleaseDefinitionVariableGroupUtility.GetVariableGroupVariables(this.existingReleaseDefinition.VariableGroups, this.allVariableGroupsInExistingRD);
            if (this.existingReleaseDefinition.Variables != null)
              this.existingRDVariables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[2]
              {
                this.existingReleaseDefinition.Variables,
                this.existingRDVariables
              });
          }
        }
        return this.existingReleaseDefinition;
      }
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

    private IList<TaskAgentQueue> AllTaskAgentQueues
    {
      get
      {
        if (this.allTaskAgentQueues == null)
        {
          if (this.allQueueIdsInUse.Count == 0)
            this.allTaskAgentQueues = (IList<TaskAgentQueue>) new List<TaskAgentQueue>();
          else if (this.allQueueIdsInUse.Count == 1)
          {
            try
            {
              this.allTaskAgentQueues = (IList<TaskAgentQueue>) new List<TaskAgentQueue>()
              {
                this.PoolService.GetAgentQueue(this.context.Elevate(), this.projectId, this.allQueueIdsInUse[0])
              };
            }
            catch (TaskAgentPoolNotFoundException ex)
            {
              this.allTaskAgentQueues = (IList<TaskAgentQueue>) new List<TaskAgentQueue>();
            }
          }
          else
            this.allTaskAgentQueues = this.PoolService.GetAgentQueues(this.context.Elevate(), this.projectId, (IEnumerable<int>) this.allQueueIdsInUse);
        }
        return this.allTaskAgentQueues;
      }
    }

    private IList<DeploymentGroup> GetDeploymentGroupsByIds(IList<int> machineGroupIds) => this.PoolService.GetDeploymentGroupsByIds(this.context.Elevate(), this.projectId, machineGroupIds);

    private IList<TaskDefinition> AllTaskDefinitions
    {
      get
      {
        if (this.allTaskDefinitions == null || this.allTaskDefinitions.Count<TaskDefinition>() <= 0)
          this.allTaskDefinitions = this.PoolService.GetTaskDefinitions(this.context.Elevate());
        return this.allTaskDefinitions;
      }
    }

    private void CheckDeleteEnvironmentPermission()
    {
      IEnumerable<ReleaseDefinitionEnvironment> deletedEnvironments = this.GetDeletedEnvironments();
      if (deletedEnvironments == null || deletedEnvironments.Count<ReleaseDefinitionEnvironment>() <= 0)
        return;
      foreach (ReleaseDefinitionEnvironment environment in deletedEnvironments)
        this.EnsureSecurityPermission(environment, ReleaseManagementSecurityPermissions.DeleteReleaseEnvironment);
    }

    private void CheckManageReleaseApproversPermission()
    {
      List<ReleaseDefinitionEnvironment> approverChangedEnvironments = new List<ReleaseDefinitionEnvironment>();
      if (!this.TryGetEnvironmentsWithApproverChanged(out approverChangedEnvironments))
        return;
      foreach (ReleaseDefinitionEnvironment environment in approverChangedEnvironments)
        this.EnsureSecurityPermission(environment, ReleaseManagementSecurityPermissions.ManageReleaseApprovers);
    }

    private void CheckEditReleaseEnvironmentPermission()
    {
      List<ReleaseDefinitionEnvironment> editedEnvironments = new List<ReleaseDefinitionEnvironment>();
      if (this.TryGetEditedEnvironments(out editedEnvironments))
      {
        foreach (ReleaseDefinitionEnvironment environment in editedEnvironments)
          this.EnsureSecurityPermission(environment, ReleaseManagementSecurityPermissions.EditReleaseEnvironment);
      }
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> releaseDefinition = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseDefinitionExtensions.GetVariablesFromReleaseDefinition(this.updatedReleaseDefinition, this.context, this.projectId);
      List<ReleaseDefinitionEnvironment> first = new List<ReleaseDefinitionEnvironment>();
      List<ReleaseDefinitionEnvironment> editedEnvironmentsWithWorkflowChanged;
      if (this.TryGetEditedEnvironmentsWithWorkflowChanged(out editedEnvironmentsWithWorkflowChanged))
        first.AddRange((IEnumerable<ReleaseDefinitionEnvironment>) editedEnvironmentsWithWorkflowChanged);
      IList<ReleaseDefinitionEnvironment> byVariableUpdate = ServiceEndpointVariablesHelper.GetReleaseEnvironmentsImpactedByVariableUpdate(this.context, this.ExistingReleaseDefinition, this.updatedReleaseDefinition, this.AllTaskDefinitions);
      ServiceEndpointPermissionValidator.EnsureServiceEndpointSecurityPermission(this.context, this.projectId, first.Union<ReleaseDefinitionEnvironment>((IEnumerable<ReleaseDefinitionEnvironment>) byVariableUpdate), releaseDefinition, this.ExistingReleaseDefinition, this.AllTaskDefinitions);
    }

    private void CheckEndpointsPermissionOnNewlyAddedEnvironments()
    {
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> releaseDefinition = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseDefinitionExtensions.GetVariablesFromReleaseDefinition(this.updatedReleaseDefinition, this.context, this.projectId);
      List<ReleaseDefinitionEnvironment> newAddedEnvironments;
      if (!this.TryGetNewAddedEnvironments(out newAddedEnvironments))
        return;
      ServiceEndpointPermissionValidator.EnsureServiceEndpointSecurityPermission(this.context, this.projectId, (IEnumerable<ReleaseDefinitionEnvironment>) newAddedEnvironments, releaseDefinition, this.ExistingReleaseDefinition, this.AllTaskDefinitions);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method is supposed to return true/false only")]
    private bool TryGetEditedEnvironmentsWithWorkflowChanged(
      out List<ReleaseDefinitionEnvironment> editedEnvironmentsWithWorkflowChanged)
    {
      editedEnvironmentsWithWorkflowChanged = new List<ReleaseDefinitionEnvironment>();
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) this.updatedReleaseDefinition.Environments)
      {
        ReleaseDefinitionEnvironment updatedEnv = environment;
        ReleaseDefinitionEnvironment definitionEnvironment = this.ExistingReleaseDefinition.Environments.FirstOrDefault<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (x => x.Id == updatedEnv.Id));
        if (definitionEnvironment != null)
        {
          List<DeployPhaseSnapshot> list = definitionEnvironment.DeployPhases.Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, DeployPhaseSnapshot>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, DeployPhaseSnapshot>) (e => e.ToServerDeployPhaseSnapshot())).ToList<DeployPhaseSnapshot>();
          try
          {
            ReleaseEnvironmentValidations.ValidateDeployPhaseSnapshotsAreNotModified(updatedEnv.Name, updatedEnv.DeployPhases, (IList<DeployPhaseSnapshot>) list, true);
          }
          catch (Exception ex)
          {
            editedEnvironmentsWithWorkflowChanged.Add(updatedEnv);
          }
        }
      }
      return editedEnvironmentsWithWorkflowChanged.Any<ReleaseDefinitionEnvironment>();
    }

    private bool TryGetEnvironmentsWithApproverChanged(
      out List<ReleaseDefinitionEnvironment> approverChangedEnvironments)
    {
      approverChangedEnvironments = new List<ReleaseDefinitionEnvironment>();
      bool withApproverChanged = false;
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) this.updatedReleaseDefinition.Environments)
      {
        ReleaseDefinitionEnvironment updatedEnv = environment;
        ReleaseDefinitionEnvironment existingEnvironment = this.ExistingReleaseDefinition.Environments.FirstOrDefault<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (x => x.Id == updatedEnv.Id));
        if (existingEnvironment != null && (ReleaseDefinitionApproverValidations.CompareDefinitionEnvironmentApprovals(updatedEnv, existingEnvironment) || ReleaseDefinitionApproverValidations.CompareDefinitionEnvironmentGates(updatedEnv, existingEnvironment)))
        {
          approverChangedEnvironments.Add(updatedEnv);
          withApproverChanged = true;
        }
      }
      return withApproverChanged;
    }

    private bool TryGetEditedEnvironments(
      out List<ReleaseDefinitionEnvironment> editedEnvironments)
    {
      editedEnvironments = new List<ReleaseDefinitionEnvironment>();
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) this.updatedReleaseDefinition.Environments)
      {
        ReleaseDefinitionEnvironment updatedEnv = environment;
        ReleaseDefinitionEnvironment definitionEnvironment = this.ExistingReleaseDefinition.Environments.FirstOrDefault<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (x => x.Id == updatedEnv.Id));
        if (definitionEnvironment != null && !definitionEnvironment.Equals((object) updatedEnv, true))
          editedEnvironments.Add(updatedEnv);
      }
      return editedEnvironments.Any<ReleaseDefinitionEnvironment>();
    }

    private bool TryGetNewAddedEnvironments(
      out List<ReleaseDefinitionEnvironment> newAddedEnvironments)
    {
      newAddedEnvironments = new List<ReleaseDefinitionEnvironment>();
      bool addedEnvironments = false;
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) this.updatedReleaseDefinition.Environments)
      {
        ReleaseDefinitionEnvironment updatedEnv = environment;
        if (this.ExistingReleaseDefinition.Environments.FirstOrDefault<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (x => x.Id == updatedEnv.Id)) == null)
        {
          newAddedEnvironments.Add(updatedEnv);
          addedEnvironments = true;
        }
      }
      return addedEnvironments;
    }

    private void EnsureSecurityPermission(
      ReleaseDefinitionEnvironment environment,
      ReleaseManagementSecurityPermissions permission)
    {
      if (!this.DoesUserHasSecurityPermission(environment, permission))
      {
        ResourceAccessException innerException = new ResourceAccessException(this.context.RootContext.GetUserId().ToString(), permission);
        throw new UnauthorizedRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) innerException.Message, (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EnvironmentName, (object) environment.Name)), (Exception) innerException);
      }
    }

    private bool DoesUserHasSecurityPermission(
      ReleaseDefinitionEnvironment environment,
      ReleaseManagementSecurityPermissions permission)
    {
      return this.context.HasPermission(this.projectId, this.updatedReleaseDefinition.Path, this.updatedReleaseDefinition.Id, environment.Id, permission);
    }

    internal class ReleaseDefinitionEnvironmentIdComparer : 
      IEqualityComparer<ReleaseDefinitionEnvironment>
    {
      public int GetHashCode(ReleaseDefinitionEnvironment obj)
      {
        int hashCode = 0;
        if (obj != null)
          hashCode = obj.Id.GetHashCode();
        return hashCode;
      }

      [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Already null guard is there.")]
      public bool Equals(ReleaseDefinitionEnvironment x, ReleaseDefinitionEnvironment y)
      {
        bool flag = false;
        if (x != null || y != null)
          flag = x.Id == y.Id;
        return flag;
      }
    }
  }
}
