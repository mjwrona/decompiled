// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ReleaseDefinitionEnvironmentsSnapshotMerger
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ReleaseDefinitionEnvironmentsSnapshotMerger
  {
    public static ReleaseDefinitionEnvironmentsSnapshot GetUpdatedReleaseDefinitionSnapshot(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease)
    {
      if (serverRelease == null)
        throw new ArgumentNullException(nameof (serverRelease));
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> releaseEnvironmentList = webApiRelease != null ? webApiRelease.GetEditableEnvironments(serverRelease).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>() : throw new ArgumentNullException(nameof (webApiRelease));
      if (!ReleaseDefinitionEnvironmentsSnapshotMerger.HasEachEnvironmentAtLeastOnePrePostDefinitionApproval((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) releaseEnvironmentList) && !ReleaseDefinitionEnvironmentsSnapshotMerger.HasAnyChangeInTheEnvironmentPreAndPostGates((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) releaseEnvironmentList, serverRelease))
        return serverRelease.DefinitionSnapshot;
      ReleaseDefinitionEnvironmentsSnapshotMerger.SnapshotApproversShouldBeValid((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) releaseEnvironmentList);
      ReleaseDefinitionEnvironmentsSnapshot definitionSnapshot = serverRelease.DefinitionSnapshot;
      ReleaseDefinitionEnvironmentsSnapshot resultRDSnapshot = ReleaseDefinitionEnvironmentsSnapshotMerger.Initialize(definitionSnapshot.GetNonEditableEnvironments(serverRelease));
      ReleaseDefinitionEnvironmentsSnapshotMerger.MergeSnapshot(resultRDSnapshot, definitionSnapshot, (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) releaseEnvironmentList);
      return resultRDSnapshot;
    }

    private static void MergeSnapshot(
      ReleaseDefinitionEnvironmentsSnapshot resultRDSnapshot,
      ReleaseDefinitionEnvironmentsSnapshot baseRDSnapshot,
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> editableEnvironments)
    {
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment editableEnvironment in editableEnvironments)
      {
        DefinitionEnvironmentData definitionEnvironmentData = ReleaseDefinitionEnvironmentsSnapshotMerger.Merge(ReleaseDefinitionEnvironmentsSnapshotMerger.GetEnvironmentSnapshot(baseRDSnapshot, editableEnvironment.Name), editableEnvironment);
        resultRDSnapshot.Environments.Add(definitionEnvironmentData);
      }
    }

    private static DefinitionEnvironmentData GetEnvironmentSnapshot(
      ReleaseDefinitionEnvironmentsSnapshot snapshot,
      string environmentName)
    {
      return snapshot.Environments.SingleOrDefault<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (e => e.Name == environmentName)) ?? throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EnvironmentWithNameDoesNotExist, (object) environmentName));
    }

    private static ReleaseDefinitionEnvironmentsSnapshot Initialize(
      IEnumerable<DefinitionEnvironmentData> nonEditableEnvironmentsData)
    {
      ReleaseDefinitionEnvironmentsSnapshot environmentsSnapshot = new ReleaseDefinitionEnvironmentsSnapshot();
      foreach (DefinitionEnvironmentData definitionEnvironmentData in nonEditableEnvironmentsData)
        environmentsSnapshot.Environments.Add(definitionEnvironmentData);
      return environmentsSnapshot;
    }

    private static DefinitionEnvironmentData Merge(
      DefinitionEnvironmentData baseSnapshot,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment newEnvironment)
    {
      DefinitionEnvironmentData definitionEnvironmentData = baseSnapshot.ShallowClone();
      int rankOffset1 = 0;
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions approvalOptions1 = newEnvironment.PreApprovalsSnapshot.ApprovalOptions;
      ApprovalExecutionOrder approvalExecutionOrder1 = approvalOptions1 != null ? approvalOptions1.ExecutionOrder : ApprovalExecutionOrder.BeforeGates;
      IList<ReleaseDefinitionApprovalStep> snapshotApprovals1 = newEnvironment.GetSnapshotApprovals(EnvironmentStepType.PreDeploy);
      if (newEnvironment.PreDeploymentGatesSnapshot.AreGatesEnabled())
      {
        int rank;
        if (approvalExecutionOrder1 == ApprovalExecutionOrder.BeforeGates)
        {
          rank = snapshotApprovals1.Max<ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovalStep, int>) (a => a.Rank)) + 1;
          rankOffset1 = 0;
        }
        else
        {
          rankOffset1 = 1;
          rank = 1;
        }
        DefinitionEnvironmentStepData environmentGateStep = ReleaseDefinitionEnvironmentsSnapshotMerger.GetDefintionEnvironmentGateStep(baseSnapshot.Id, rank, EnvironmentStepType.PreGate);
        definitionEnvironmentData.Steps.Add(environmentGateStep);
      }
      IEnumerable<DefinitionEnvironmentStepData> approvalsStepData1 = ReleaseDefinitionEnvironmentsSnapshotMerger.ToApprovalsStepData((IEnumerable<ReleaseDefinitionApprovalStep>) snapshotApprovals1, EnvironmentStepType.PreDeploy, baseSnapshot.Id, rankOffset1);
      definitionEnvironmentData.Steps.AddRange<DefinitionEnvironmentStepData, IList<DefinitionEnvironmentStepData>>(approvalsStepData1);
      DefinitionEnvironmentStepData environmentStepData = baseSnapshot.Steps.Single<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (step => step.StepType == EnvironmentStepType.Deploy));
      environmentStepData.Rank = definitionEnvironmentData.Steps.Select<DefinitionEnvironmentStepData, int>((Func<DefinitionEnvironmentStepData, int>) (s => s.Rank)).Max() + 1;
      definitionEnvironmentData.Steps.Add(environmentStepData);
      int rankOffset2 = environmentStepData.Rank;
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions approvalOptions2 = newEnvironment.PostApprovalsSnapshot.ApprovalOptions;
      ApprovalExecutionOrder approvalExecutionOrder2 = approvalOptions2 != null ? approvalOptions2.ExecutionOrder : ApprovalExecutionOrder.BeforeGates;
      IList<ReleaseDefinitionApprovalStep> snapshotApprovals2 = newEnvironment.GetSnapshotApprovals(EnvironmentStepType.PostDeploy);
      if (newEnvironment.PostDeploymentGatesSnapshot.AreGatesEnabled())
      {
        int rank;
        if (approvalExecutionOrder2 == ApprovalExecutionOrder.BeforeGates)
        {
          rank = snapshotApprovals2.Max<ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovalStep, int>) (a => a.Rank)) + environmentStepData.Rank + 1;
          rankOffset2 = environmentStepData.Rank;
        }
        else
        {
          rankOffset2 = environmentStepData.Rank + 1;
          rank = environmentStepData.Rank + 1;
        }
        DefinitionEnvironmentStepData environmentGateStep = ReleaseDefinitionEnvironmentsSnapshotMerger.GetDefintionEnvironmentGateStep(baseSnapshot.Id, rank, EnvironmentStepType.PostGate);
        definitionEnvironmentData.Steps.Add(environmentGateStep);
      }
      IEnumerable<DefinitionEnvironmentStepData> approvalsStepData2 = ReleaseDefinitionEnvironmentsSnapshotMerger.ToApprovalsStepData((IEnumerable<ReleaseDefinitionApprovalStep>) snapshotApprovals2, EnvironmentStepType.PostDeploy, baseSnapshot.Id, rankOffset2);
      definitionEnvironmentData.Steps.AddRange<DefinitionEnvironmentStepData, IList<DefinitionEnvironmentStepData>>(approvalsStepData2);
      return definitionEnvironmentData;
    }

    private static IEnumerable<DefinitionEnvironmentStepData> ToApprovalsStepData(
      IEnumerable<ReleaseDefinitionApprovalStep> approvals,
      EnvironmentStepType approvalType,
      int parentEnvironmentId,
      int rankOffset)
    {
      return approvals.Select<ReleaseDefinitionApprovalStep, DefinitionEnvironmentStepData>((Func<ReleaseDefinitionApprovalStep, DefinitionEnvironmentStepData>) (approval => ReleaseDefinitionEnvironmentsSnapshotMerger.ToApprovalStepData(approval, approvalType, parentEnvironmentId, rankOffset)));
    }

    private static DefinitionEnvironmentStepData ToApprovalStepData(
      ReleaseDefinitionApprovalStep approval,
      EnvironmentStepType approvalType,
      int parentEnvironmentId,
      int rankOffset)
    {
      Guid approverId = approval.GetApproverId();
      return new DefinitionEnvironmentStepData()
      {
        DefinitionEnvironmentId = parentEnvironmentId,
        ApproverId = approverId,
        IsAutomated = approval.IsAutomated,
        IsNotificationOn = approval.IsNotificationOn,
        Rank = rankOffset + approval.Rank,
        StepType = approvalType
      };
    }

    private static DefinitionEnvironmentStepData GetDefintionEnvironmentGateStep(
      int parentEnvironmentId,
      int rank,
      EnvironmentStepType stepType)
    {
      return new DefinitionEnvironmentStepData()
      {
        DefinitionEnvironmentId = parentEnvironmentId,
        ApproverId = Guid.Empty,
        IsAutomated = true,
        IsNotificationOn = false,
        Rank = rank,
        StepType = stepType
      };
    }

    private static void SnapshotApproversShouldBeValid(IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> environments)
    {
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in environments)
        environment.CheckSnapshotApprovals();
    }

    private static bool HasEachEnvironmentAtLeastOnePrePostDefinitionApproval(
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> environments)
    {
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in environments)
      {
        if (!environment.HasEnvironmentAtLeastOnePrePostApprovalSnapshot())
          return false;
      }
      return true;
    }

    private static bool HasAnyChangeInTheEnvironmentPreAndPostGates(
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> webApiEnvironments,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiEnvironment in webApiEnvironments)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = serverRelease.GetEnvironment(webApiEnvironment.Id);
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions approvalOptions = webApiEnvironment.PreApprovalsSnapshot.ApprovalOptions;
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions preApprovalOptions = environment.PreApprovalOptions;
        if (webApiEnvironment.PreDeploymentGatesSnapshot.AnyChangeInGatesOptionsAndApprovalsExecutionOrder(environment.PreDeploymentGates, approvalOptions, preApprovalOptions) || webApiEnvironment.PostDeploymentGatesSnapshot.AnyChangeInGatesOptionsAndApprovalsExecutionOrder(environment.PostDeploymentGates, approvalOptions, preApprovalOptions))
          return true;
      }
      return false;
    }
  }
}
