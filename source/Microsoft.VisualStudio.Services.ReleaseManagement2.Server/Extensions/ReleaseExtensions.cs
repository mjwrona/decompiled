// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseExtensions
  {
    private const string TraceLayer = "ReleaseExtensions";

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release ToContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IVssRequestContext context,
      Guid projectId,
      bool includeTasks)
    {
      return release.ToContract(context, projectId, includeTasks, ApprovalFilters.ManualApprovals | ApprovalFilters.AutomatedApprovals);
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release ToContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IVssRequestContext context,
      Guid projectId,
      bool includeTasks,
      bool includeTasksOnlyForInProgressEnvironments)
    {
      return release.ToContract(context, projectId, includeTasks, ApprovalFilters.ManualApprovals | ApprovalFilters.AutomatedApprovals, includeTasksOnlyForInProgressEnvironments: includeTasksOnlyForInProgressEnvironments);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release ToContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IVssRequestContext context,
      Guid projectId,
      bool includeTasks,
      ApprovalFilters approvalFilters,
      int numberOfGateRecords = 5,
      bool includeTasksOnlyForInProgressEnvironments = false)
    {
      return ((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release[1]
      {
        release
      }).ToContract(context, projectId, includeTasks, approvalFilters, numberOfGateRecords, includeTasksOnlyForInProgressEnvironments).FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>();
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release ToShallowContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IVssRequestContext context,
      Guid projectId,
      ApprovalFilters approvalFilters)
    {
      return ReleaseExtensions.ToShallowContractImplementation(release.ToContract(context, projectId, true, approvalFilters));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By design")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release ToShallowContractImplementation(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      release.Variables.Clear();
      release.VariableGroups.Clear();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) release.Environments)
      {
        environment.Variables.Clear();
        environment.VariableGroups.Clear();
        environment.EnvironmentOptions = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions) null;
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) environment.DeployPhasesSnapshot)
          deployPhase.WorkflowTasks.Clear();
        if (!environment.PreDeployApprovals.IsNullOrEmpty<ReleaseApproval>())
        {
          int latestPreApprovalAttempt = environment.PreDeployApprovals.Max<ReleaseApproval>((Func<ReleaseApproval, int>) (preApproval => preApproval.Attempt));
          environment.PreDeployApprovals.RemoveAll((Predicate<ReleaseApproval>) (preApproval => preApproval.Attempt != latestPreApprovalAttempt));
        }
        if (!environment.PostDeployApprovals.IsNullOrEmpty<ReleaseApproval>())
        {
          int latestPostApprovalAttempt = environment.PostDeployApprovals.Max<ReleaseApproval>((Func<ReleaseApproval, int>) (postApproval => postApproval.Attempt));
          environment.PostDeployApprovals.RemoveAll((Predicate<ReleaseApproval>) (postApproval => postApproval.Attempt != latestPostApprovalAttempt));
        }
        if (!environment.DeploySteps.IsNullOrEmpty<DeploymentAttempt>())
        {
          int latestAttempt = environment.DeploySteps.Max<DeploymentAttempt>((Func<DeploymentAttempt, int>) (deployStep => deployStep.Attempt));
          environment.DeploySteps.RemoveAll((Predicate<DeploymentAttempt>) (deployStep => deployStep.Attempt != latestAttempt));
          foreach (DeploymentAttempt deployStep in environment.DeploySteps)
          {
            deployStep.ErrorLog = (string) null;
            deployStep.Issues.Clear();
            foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase releaseDeployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase>) deployStep.ReleaseDeployPhases)
            {
              releaseDeployPhase.ErrorLog = (string) null;
              if (releaseDeployPhase.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.MachineGroupBasedDeployment)
                releaseDeployPhase.DeploymentJobs.Clear();
              foreach (DeploymentJob deploymentJob in (IEnumerable<DeploymentJob>) releaseDeployPhase.DeploymentJobs)
              {
                foreach (ReleaseTask task in (IEnumerable<ReleaseTask>) deploymentJob.Tasks)
                  task.Issues.Clear();
                if (deploymentJob.Job != null)
                  deploymentJob.Job.Issues.Clear();
              }
            }
          }
        }
      }
      return release;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "TODO - refactor to decrease cyclomatic complexity number to 25")]
    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release ToReleaseContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IVssRequestContext context,
      Guid projectId,
      bool includeTasks,
      bool includeAutomatedApprovals,
      bool includeManualApprovals,
      bool includeApprovalSnapshots,
      int numberOfGateRecords,
      bool includeTasksOnlyForInprogressEnvironments = false)
    {
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(context, "Service", "ReleaseExtensions.ToReleaseContract", 1961073))
      {
        if (!release.ProjectId.Equals(Guid.Empty))
          projectId = release.ProjectId;
        foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) release.LinkedArtifacts)
          linkedArtifact.PopulateReleaseArtifact(context, projectId);
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release contract = release.ConvertModelToContract(context, projectId);
        releaseManagementTimer.RecordLap("Service", "ReleaseExtenstions.ToReleaseContract.ArtifactsPopulated", 1961214);
        if (includeTasks)
        {
          contract.FillReleaseTasks(context, projectId, release, includeTasksOnlyForInprogressEnvironments, numberOfGateRecords);
          ReleaseExtensions.PopulateTaskLogUrls(context, contract, projectId);
        }
        else
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase in contract.Environments.SelectMany<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>>) (e => (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) e.DeployPhasesSnapshot)))
            deployPhase.WorkflowTasks = (IList<WorkflowTask>) null;
        }
        releaseManagementTimer.RecordLap("Service", "ReleaseExtenstions.ToReleaseContract.TasksFilled", 1961215);
        if (includeAutomatedApprovals | includeManualApprovals)
        {
          Predicate<ReleaseApproval> match = (Predicate<ReleaseApproval>) (approval =>
          {
            bool releaseContract = false;
            if (!includeManualApprovals)
              releaseContract |= !approval.IsAutomated;
            if (!includeAutomatedApprovals)
              releaseContract |= approval.IsAutomated;
            return releaseContract;
          });
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) contract.Environments)
          {
            environment.PreDeployApprovals?.RemoveAll(match);
            environment.PostDeployApprovals?.RemoveAll(match);
          }
        }
        else
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) contract.Environments)
          {
            environment.PreDeployApprovals?.RemoveAll((Predicate<ReleaseApproval>) (approval => true));
            environment.PostDeployApprovals?.RemoveAll((Predicate<ReleaseApproval>) (approval => true));
          }
        }
        if (includeApprovalSnapshots)
        {
          ReleaseExtensions.PopulateReleaseDefinitionApprovals(contract, release);
        }
        else
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) contract.Environments)
          {
            environment.PreApprovalsSnapshot = (ReleaseDefinitionApprovals) null;
            environment.PostApprovalsSnapshot = (ReleaseDefinitionApprovals) null;
          }
        }
        ReleaseManagementArtifactPropertyKinds.CopyProperties(contract.Properties, release.Properties);
        releaseManagementTimer.RecordLap("Service", "ReleaseExtenstions.ToReleaseContract.ApprovalsPopulated", 1961216);
        return contract;
      }
    }

    private static void PopulateTaskLogUrls(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Guid projectId)
    {
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) webApiRelease.Environments)
        environment.PopulateTaskLogUrls(requestContext, webApiRelease.Id, projectId);
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release FromContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      IVssRequestContext context)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release model = webApiRelease.ConvertContractToModel(context.IsFeatureEnabled("VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion"));
      model.ModifiedBy = context.GetUserId(true);
      return model;
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> ToContract(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releaseList,
      IVssRequestContext context,
      Guid projectId,
      bool includeTasks,
      ApprovalFilters approvalFilters,
      int numberOfGateRecords = 5,
      bool includeTasksOnlyForInProgressEnvironments = false)
    {
      using (ReleaseManagementTimer.Create(context, "Service", "ReleaseExtensions.ReleaseModelToWebApi", 1961005))
      {
        bool includeAutomatedApprovals = (approvalFilters & ApprovalFilters.AutomatedApprovals) != 0;
        bool includeManualApprovals = (approvalFilters & ApprovalFilters.ManualApprovals) != 0;
        bool includeApprovalSnapshots = (approvalFilters & ApprovalFilters.ApprovalSnapshots) != 0;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return releaseList.ToContractImplementation(context, projectId, includeTasks, includeAutomatedApprovals, includeManualApprovals, includeApprovalSnapshots, ReleaseExtensions.\u003C\u003EO.\u003C0\u003E__ToReleaseContract ?? (ReleaseExtensions.\u003C\u003EO.\u003C0\u003E__ToReleaseContract = new Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, IVssRequestContext, Guid, bool, bool, bool, bool, int, bool, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>(ReleaseExtensions.ToReleaseContract)), ReleaseExtensions.\u003C\u003EO.\u003C1\u003E__PopulateIdentities ?? (ReleaseExtensions.\u003C\u003EO.\u003C1\u003E__PopulateIdentities = new Action<List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>, IVssRequestContext, bool>(ReleaseExtensions.PopulateIdentities)), true, numberOfGateRecords, includeTasksOnlyForInProgressEnvironments);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> ToContract(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releaseList,
      IVssRequestContext context,
      Guid projectId,
      bool includeIdentityUrls = true,
      int numberOfGateRecords = 5)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return releaseList.ToContractImplementation(context, projectId, false, true, true, false, ReleaseExtensions.\u003C\u003EO.\u003C0\u003E__ToReleaseContract ?? (ReleaseExtensions.\u003C\u003EO.\u003C0\u003E__ToReleaseContract = new Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, IVssRequestContext, Guid, bool, bool, bool, bool, int, bool, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>(ReleaseExtensions.ToReleaseContract)), ReleaseExtensions.\u003C\u003EO.\u003C1\u003E__PopulateIdentities ?? (ReleaseExtensions.\u003C\u003EO.\u003C1\u003E__PopulateIdentities = new Action<List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>, IVssRequestContext, bool>(ReleaseExtensions.PopulateIdentities)), includeIdentityUrls, numberOfGateRecords, false);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Testing requirements")]
    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> ToContractImplementation(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> releaseList,
      IVssRequestContext context,
      Guid projectId,
      bool includeTasks,
      bool includeAutomatedApprovals,
      bool includeManualApprovals,
      bool includeApprovalSnapshots,
      Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, IVssRequestContext, Guid, bool, bool, bool, bool, int, bool, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> convertToWebApiRelease,
      Action<List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>, IVssRequestContext, bool> populateIdentities,
      bool includeIdentityUrls,
      int numberOfGateRecords,
      bool includeTasksOnlyForInProgressEnvironments)
    {
      if (convertToWebApiRelease == null)
        throw new ArgumentNullException(nameof (convertToWebApiRelease));
      if (populateIdentities == null)
        throw new ArgumentNullException(nameof (populateIdentities));
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> list = releaseList.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) (release => convertToWebApiRelease(release, context, projectId, includeTasks, includeAutomatedApprovals, includeManualApprovals, includeApprovalSnapshots, numberOfGateRecords, includeTasksOnlyForInProgressEnvironments))).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>();
      populateIdentities(list, context, includeIdentityUrls);
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) list;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release ToDefinitionSnapshotContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease,
      IVssRequestContext tfsRequestContext,
      Guid projectId)
    {
      if (serverRelease == null)
        throw new ArgumentNullException(nameof (serverRelease));
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) serverRelease.LinkedArtifacts)
        linkedArtifact.PopulateReleaseArtifact(tfsRequestContext, projectId);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release contract = serverRelease.ConvertModelToContract(tfsRequestContext, projectId);
      ReleaseExtensions.PopulateReleaseDefinitionApprovals(contract, serverRelease);
      ReleaseExtensions.PopulateIdentities(new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>()
      {
        contract
      }, tfsRequestContext, true);
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) contract.Environments)
      {
        environment.DeploySteps = (List<DeploymentAttempt>) null;
        environment.PreDeployApprovals = (List<ReleaseApproval>) null;
        environment.PostDeployApprovals = (List<ReleaseApproval>) null;
      }
      return contract;
    }

    public static void NormalizeWebApiReleases(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> releases,
      ReleaseExpands expands)
    {
      if (releases == null)
        throw new ArgumentNullException(nameof (releases));
      bool flag1 = (expands & ReleaseExpands.Artifacts) == ReleaseExpands.Artifacts;
      bool flag2 = (expands & ReleaseExpands.Approvals) == ReleaseExpands.Approvals;
      bool flag3 = (expands & ReleaseExpands.ManualInterventions) == ReleaseExpands.ManualInterventions;
      bool flag4 = flag2 | flag3 || (expands & ReleaseExpands.Environments) == ReleaseExpands.Environments;
      bool flag5 = (expands & ReleaseExpands.Variables) == ReleaseExpands.Variables;
      bool flag6 = (expands & ReleaseExpands.Tags) == ReleaseExpands.Tags;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release in releases)
      {
        if (!flag6)
          release.Tags.Clear();
        if (!flag5)
          release.Variables.Clear();
        release.VariableGroups.Clear();
        if (!flag1)
          release.Artifacts = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>) null;
        if (flag4)
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) release.Environments)
          {
            if (!flag5)
              environment.Variables.Clear();
            environment.VariableGroups.Clear();
            environment.DeployPhasesSnapshot.Clear();
            environment.PreDeploymentGatesSnapshot = (ReleaseDefinitionGatesStep) null;
            environment.PostDeploymentGatesSnapshot = (ReleaseDefinitionGatesStep) null;
          }
        }
        else
          release.Environments = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) null;
        if (!flag2 && release.Environments != null)
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) release.Environments)
          {
            if (environment.PreApprovalsSnapshot != null)
              environment.PreApprovalsSnapshot = (ReleaseDefinitionApprovals) null;
            if (environment.PostApprovalsSnapshot != null)
              environment.PostApprovalsSnapshot = (ReleaseDefinitionApprovals) null;
            if (environment.PreDeployApprovals != null)
              environment.PreDeployApprovals = (List<ReleaseApproval>) null;
            if (environment.PostDeployApprovals != null)
              environment.PostDeployApprovals = (List<ReleaseApproval>) null;
          }
        }
      }
    }

    private static void PopulateIdentities(
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> releases,
      IVssRequestContext context,
      bool includeIdentityUrls)
    {
      using (ReleaseManagementTimer.Create(context, "Service", "ReleaseExtensions.PopulateIdentities", 1961101))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ReleaseExtensions.PopulateIdentitiesImplementation((IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) releases, context, ReleaseExtensions.\u003C\u003EO.\u003C2\u003E__GetIdentityHelper ?? (ReleaseExtensions.\u003C\u003EO.\u003C2\u003E__GetIdentityHelper = new Func<IVssRequestContext, IList<string>, bool, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper>(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper.GetIdentityHelper)), new Action<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper>(new ReleaseIdentityHandler().PopulateIdentities), includeIdentityUrls);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Testing requirements")]
    public static void PopulateIdentitiesImplementation(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> releases,
      IVssRequestContext context,
      Func<IVssRequestContext, IList<string>, bool, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper> getIdentityMap,
      Action<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper> updateReferences,
      bool includeIdentityUrls)
    {
      if (releases == null)
        throw new ArgumentNullException(nameof (releases));
      if (getIdentityMap == null)
        throw new ArgumentNullException(nameof (getIdentityMap));
      if (updateReferences == null)
        throw new ArgumentNullException(nameof (updateReferences));
      HashSet<string> source = new HashSet<string>();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) releases)
      {
        IList<string> teamFoundationIds = ReleaseIdentityHandler.GetTeamFoundationIds(release);
        source.UnionWith((IEnumerable<string>) teamFoundationIds);
      }
      Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper identityMap = getIdentityMap(context, (IList<string>) source.ToList<string>(), includeIdentityUrls);
      releases.ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>().ForEach((Action<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release>) (release => updateReferences(release, identityMap)));
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> GetEditableEnvironments(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      if (webApiRelease == null)
        throw new ArgumentNullException(nameof (webApiRelease));
      IEnumerable<int> editableEnvironmentIds = serverRelease != null ? serverRelease.GetEditableEnvironmentIds() : throw new ArgumentNullException(nameof (serverRelease));
      return webApiRelease.Environments.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, bool>) (env => editableEnvironmentIds.Contains<int>(env.Id)));
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> GetNonEditableEnvironments(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      if (webApiRelease == null)
        throw new ArgumentNullException(nameof (webApiRelease));
      IEnumerable<int> editableEnvironmentIds = serverRelease != null ? serverRelease.GetEditableEnvironmentIds() : throw new ArgumentNullException(nameof (serverRelease));
      return webApiRelease.Environments.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, bool>) (env => !editableEnvironmentIds.Contains<int>(env.Id)));
    }

    public static IEnumerable<int> GetEditableEnvironmentDefinitionEnvironmentIds(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      return release.Environments.Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.Status != ReleaseEnvironmentStatus.InProgress)).Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int>) (env => env.DefinitionEnvironmentId));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseUpdateMetadata FromContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseUpdateMetadata releaseUpdateMetadata)
    {
      if (releaseUpdateMetadata == null)
        throw new ArgumentNullException(nameof (releaseUpdateMetadata));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseUpdateMetadata releaseUpdateMetadata1 = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseUpdateMetadata();
      releaseUpdateMetadata1.Status = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus) releaseUpdateMetadata.Status;
      releaseUpdateMetadata1.KeepForever = releaseUpdateMetadata.KeepForever;
      releaseUpdateMetadata1.Comment = releaseUpdateMetadata.Comment;
      releaseUpdateMetadata1.Name = releaseUpdateMetadata.Name;
      if (releaseUpdateMetadata.ManualEnvironments != null)
        releaseUpdateMetadata1.ManualEnvironments.AddRange<string, IList<string>>((IEnumerable<string>) releaseUpdateMetadata.ManualEnvironments);
      return releaseUpdateMetadata1;
    }

    private static IEnumerable<int> GetEditableEnvironmentIds(this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release) => release.Environments.Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.Status != ReleaseEnvironmentStatus.InProgress)).Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int>) (env => env.Id));

    private static void PopulateReleaseDefinitionApprovals(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      if (serverRelease.DefinitionSnapshot == null || serverRelease.DefinitionSnapshot.Environments == null)
        return;
      foreach (DefinitionEnvironmentData environment in (IEnumerable<DefinitionEnvironmentData>) serverRelease.DefinitionSnapshot.Environments)
      {
        DefinitionEnvironmentData snapshotEnvironment = environment;
        webApiRelease.GetReleaseEnvironmentByDefinitionEnvironmentId(snapshotEnvironment.Id).PopulateApprovalsSnapshots(serverRelease.Environments.First<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (e => e.DefinitionEnvironmentId == snapshotEnvironment.Id)), snapshotEnvironment);
      }
    }

    public static ArtifactSpec CreateArtifactSpec(this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release, Guid dataspaceIdentifier)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      return ReleasePropertyExtensions.CreateArtifactSpec(release.Id, dataspaceIdentifier);
    }

    public static void PopulateProperties(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IVssRequestContext requestContext,
      Guid dataspaceIdentifier,
      IEnumerable<string> propertiesFilter)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      using (new MethodScope(requestContext, nameof (ReleaseExtensions), nameof (PopulateProperties)))
        release.Properties.AddRange<PropertyValue, IList<PropertyValue>>((IEnumerable<PropertyValue>) ReleasePropertyExtensions.GetReleasePropertyValues(requestContext, release.Id, dataspaceIdentifier, propertiesFilter));
    }

    public static void PopulateProperties(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IVssRequestContext requestContext,
      Guid dataspaceIdentifier)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      release.Properties.Clear();
      release.PopulateProperties(requestContext, dataspaceIdentifier, (IEnumerable<string>) null);
    }

    public static PullRequestRelease GetPullRequestReleaseObject(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      PullRequestRelease requestReleaseObject = (PullRequestRelease) null;
      if (release != null && release.LinkedArtifacts.Any<ArtifactSource>() && release.Reason == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.PullRequest)
      {
        bool flag = false;
        foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) release.LinkedArtifacts)
        {
          Dictionary<string, string> dictionary = linkedArtifact.SourceData.ToDictionary<KeyValuePair<string, InputValue>, string, string>((Func<KeyValuePair<string, InputValue>, string>) (e => e.Key), (Func<KeyValuePair<string, InputValue>, string>) (e => e.Value.Value));
          if (int.TryParse(dictionary.GetValueOrDefault<string, string>(WellKnownPullRequestVariables.PullRequestId, string.Empty), out int _))
          {
            dictionary["version"] = linkedArtifact.SourceData["version"].Data["sourceVersion"].ToString();
            string artifactRepositoryId = ArtifactUtility.GetArtifactRepositoryId(linkedArtifact);
            if (artifactRepositoryId != null)
              dictionary["repository"] = artifactRepositoryId;
            flag = true;
            requestReleaseObject = ReleaseExtensions.GetPullRequestReleaseObject(dictionary, requestContext, release, projectId, linkedArtifact.Alias);
            break;
          }
        }
        if (!flag)
          requestContext.TraceAlways(1976454, TraceLevel.Error, "ReleaseManagementService", "Events", "PullRequestRelease object is not created for ReleaseId : {0} because No artifact found with PullRequestId data", (object) release.Id);
      }
      return requestReleaseObject;
    }

    private static PullRequestRelease GetPullRequestReleaseObject(
      Dictionary<string, string> pullRequestVariables,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Guid projectId,
      string artifactAlias)
    {
      string valueOrDefault1 = pullRequestVariables.GetValueOrDefault<string, string>("repository", string.Empty);
      string valueOrDefault2 = pullRequestVariables.GetValueOrDefault<string, string>(WellKnownPullRequestVariables.PullRequestId, string.Empty);
      string valueOrDefault3 = pullRequestVariables.GetValueOrDefault<string, string>("version", string.Empty);
      string valueOrDefault4 = pullRequestVariables.GetValueOrDefault<string, string>(WellKnownPullRequestVariables.PullRequestSourceBranchCommitId, string.Empty);
      string valueOrDefault5 = pullRequestVariables.GetValueOrDefault<string, string>(WellKnownPullRequestVariables.PullRequestMergedAt, string.Empty);
      string valueOrDefault6 = pullRequestVariables.GetValueOrDefault<string, string>(WellKnownPullRequestVariables.PullRequestSystemType, string.Empty);
      pullRequestVariables.GetValueOrDefault<string, string>(WellKnownPullRequestVariables.PullRequestTargetBranch, string.Empty);
      string valueOrDefault7 = pullRequestVariables.GetValueOrDefault<string, string>(WellKnownPullRequestVariables.PullRequestIterationId, string.Empty);
      DateTime result1 = DateTime.UtcNow;
      int result2 = 0;
      int num;
      ref int local = ref num;
      if (!int.TryParse(valueOrDefault2, out local))
        return (PullRequestRelease) null;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.PullRequestSystemType result3;
      if (Enum.TryParse<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.PullRequestSystemType>(valueOrDefault6, out result3) && result3 == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.PullRequestSystemType.TfsGit)
      {
        if (!int.TryParse(valueOrDefault7, out result2) || result2 == 0)
        {
          requestContext.TraceAlways(1976454, TraceLevel.Error, "ReleaseManagementService", "Events", "PullRequestRelease object is not created for ReleaseId : {0} because Iteration Id not found for PullRequestId: {1}, RepositoryId: {2} and SourceCommitId: {3}", (object) release.Id, (object) num, (object) valueOrDefault1, (object) valueOrDefault4);
          return (PullRequestRelease) null;
        }
      }
      else if (string.IsNullOrEmpty(valueOrDefault5) || !DateTime.TryParse(valueOrDefault5, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result1))
      {
        requestContext.TraceAlways(1976454, TraceLevel.Error, "ReleaseManagementService", "Events", "PullRequestRelease object is not created for ReleaseId : {0} because MergedAt is not set for artifact : {1}", (object) release.Id, (object) artifactAlias);
        return (PullRequestRelease) null;
      }
      return new PullRequestRelease()
      {
        ProjectId = projectId,
        PullRequestId = num,
        ReleaseId = release.Id,
        MergeCommitId = valueOrDefault3,
        IterationId = result2,
        MergedAt = result1,
        IsActive = true
      };
    }
  }
}
