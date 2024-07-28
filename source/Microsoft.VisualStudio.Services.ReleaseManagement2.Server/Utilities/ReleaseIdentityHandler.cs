// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ReleaseIdentityHandler
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public class ReleaseIdentityHandler
  {
    public static ReleaseDefinitionShallowReference GetReleaseDefinitionShallowReference(
      IVssRequestContext context,
      int definitionId,
      string definitionName,
      string path,
      Guid projectId)
    {
      return ShallowReferencesHelper.CreateDefinitionShallowReference(context, projectId, definitionId, definitionName, path);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "environment is bulky by design")]
    public static IEnumerable<string> GetTeamFoundationIds(
      IEnumerable<ReleaseEnvironment> environments)
    {
      if (environments == null)
        throw new ArgumentNullException(nameof (environments));
      IList<IdentityRef> identityRefs = (IList<IdentityRef>) new List<IdentityRef>();
      foreach (ReleaseEnvironment environment in environments)
      {
        identityRefs.Add(environment.Owner);
        identityRefs.Add(environment.ReleaseCreatedBy);
        Action<List<ReleaseApproval>> action1 = (Action<List<ReleaseApproval>>) (approvals =>
        {
          foreach (ReleaseApproval approval in approvals)
          {
            identityRefs.Add(approval.Approver);
            identityRefs.Add(approval.ApprovedBy);
            if (approval.History != null)
            {
              foreach (ReleaseApprovalHistory releaseApprovalHistory in approval.History)
              {
                identityRefs.Add(releaseApprovalHistory.Approver);
                identityRefs.Add(releaseApprovalHistory.ChangedBy);
              }
            }
          }
        });
        action1(environment.PreDeployApprovals);
        action1(environment.PostDeployApprovals);
        foreach (VariableGroup variableGroup in (IEnumerable<VariableGroup>) environment.VariableGroups)
        {
          identityRefs.Add(variableGroup.CreatedBy);
          identityRefs.Add(variableGroup.ModifiedBy);
        }
        Action<ReleaseDefinitionApprovals> action2 = (Action<ReleaseDefinitionApprovals>) (approvals =>
        {
          if (approvals == null)
            return;
          foreach (ReleaseDefinitionApprovalStep approval in approvals.Approvals)
            identityRefs.Add(approval.Approver);
        });
        action2(environment.PreApprovalsSnapshot);
        action2(environment.PostApprovalsSnapshot);
        foreach (DeploymentAttempt deployStep in environment.DeploySteps)
        {
          if (deployStep != null)
          {
            identityRefs.AddRange<IdentityRef, IList<IdentityRef>>((IEnumerable<IdentityRef>) new IdentityRef[3]
            {
              deployStep.RequestedBy,
              deployStep.RequestedFor,
              deployStep.LastModifiedBy
            });
            foreach (ReleaseDeployPhase releaseDeployPhase in (IEnumerable<ReleaseDeployPhase>) deployStep.ReleaseDeployPhases)
            {
              foreach (ManualIntervention manualIntervention in releaseDeployPhase.ManualInterventions)
              {
                if (manualIntervention.Approver != null)
                  identityRefs.Add(manualIntervention.Approver);
              }
            }
          }
        }
      }
      return ReleaseIdentityHandler.GetUniqueTeamFoundationIds((IEnumerable<IdentityRef>) identityRefs);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    public static IList<string> GetTeamFoundationIds(Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      List<IdentityRef> source = new List<IdentityRef>();
      source.Add(release.CreatedBy);
      source.Add(release.CreatedFor);
      source.Add(release.ModifiedBy);
      IEnumerable<string> teamFoundationIds = ReleaseIdentityHandler.GetTeamFoundationIds((IEnumerable<ReleaseEnvironment>) release.Environments);
      return (IList<string>) source.Where<IdentityRef>((Func<IdentityRef, bool>) (id => id != null)).Select<IdentityRef, string>((Func<IdentityRef, string>) (identity => identity.Id)).Union<string>(teamFoundationIds).Distinct<string>().ToList<string>();
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Required for functionality.")]
    public void PopulateIdentities(
      IVssRequestContext context,
      ReleaseEnvironment environment,
      IdentityHelper identityHelper)
    {
      ReleaseIdentityHandler.PopulateIdentities((IEnumerable<ReleaseEnvironment>) new ReleaseEnvironment[1]
      {
        environment
      }, identityHelper);
    }

    private static IEnumerable<string> GetUniqueTeamFoundationIds(
      IEnumerable<IdentityRef> identities)
    {
      IDictionary<string, IdentityRef> dictionary = (IDictionary<string, IdentityRef>) new Dictionary<string, IdentityRef>();
      if (identities != null)
      {
        foreach (IdentityRef identity in identities)
        {
          if (identity != null && !string.IsNullOrEmpty(identity.Id))
            dictionary[identity.Id] = identity;
        }
      }
      return (IEnumerable<string>) dictionary.Keys;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Required for functionality.")]
    private static void PopulateIdentities(
      IEnumerable<ReleaseEnvironment> environments,
      IdentityHelper identityHelper)
    {
      if (environments == null)
        throw new ArgumentNullException(nameof (environments));
      if (identityHelper == null)
        throw new ArgumentNullException(nameof (identityHelper));
      foreach (ReleaseEnvironment environment in environments)
      {
        environment.Owner = identityHelper.GetIdentity(environment.Owner);
        environment.ReleaseCreatedBy = identityHelper.GetIdentity(environment.ReleaseCreatedBy);
        foreach (DeploymentAttempt deployStep in environment.DeploySteps)
        {
          deployStep.RequestedBy = identityHelper.GetIdentity(deployStep.RequestedBy);
          deployStep.RequestedFor = identityHelper.GetIdentity(deployStep.RequestedFor);
          deployStep.LastModifiedBy = identityHelper.GetIdentity(deployStep.LastModifiedBy);
        }
      }
      foreach (ReleaseApproval releaseApproval in environments.SelectMany<ReleaseEnvironment, ReleaseApproval>((Func<ReleaseEnvironment, IEnumerable<ReleaseApproval>>) (e => (IEnumerable<ReleaseApproval>) e.PreDeployApprovals)))
      {
        releaseApproval.Approver = identityHelper.GetIdentity(releaseApproval.Approver);
        releaseApproval.ApprovedBy = identityHelper.GetIdentity(releaseApproval.ApprovedBy);
        if (releaseApproval.History != null)
        {
          foreach (ReleaseApprovalHistory releaseApprovalHistory in releaseApproval.History)
          {
            releaseApprovalHistory.Approver = identityHelper.GetIdentity(releaseApprovalHistory.Approver);
            releaseApprovalHistory.ChangedBy = identityHelper.GetIdentity(releaseApprovalHistory.ChangedBy);
          }
        }
      }
      foreach (ReleaseApproval releaseApproval in environments.SelectMany<ReleaseEnvironment, ReleaseApproval>((Func<ReleaseEnvironment, IEnumerable<ReleaseApproval>>) (e => (IEnumerable<ReleaseApproval>) e.PostDeployApprovals)))
      {
        releaseApproval.Approver = identityHelper.GetIdentity(releaseApproval.Approver);
        releaseApproval.ApprovedBy = identityHelper.GetIdentity(releaseApproval.ApprovedBy);
        if (releaseApproval.History != null)
        {
          foreach (ReleaseApprovalHistory releaseApprovalHistory in releaseApproval.History)
          {
            releaseApprovalHistory.Approver = identityHelper.GetIdentity(releaseApprovalHistory.Approver);
            releaseApprovalHistory.ChangedBy = identityHelper.GetIdentity(releaseApprovalHistory.ChangedBy);
          }
        }
      }
      foreach (ReleaseDefinitionApprovalStep definitionApprovalStep in environments.Select<ReleaseEnvironment, ReleaseDefinitionApprovals>((Func<ReleaseEnvironment, ReleaseDefinitionApprovals>) (e => e.PreApprovalsSnapshot)).Where<ReleaseDefinitionApprovals>((Func<ReleaseDefinitionApprovals, bool>) (e => e != null)).SelectMany<ReleaseDefinitionApprovals, ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovals, IEnumerable<ReleaseDefinitionApprovalStep>>) (e => (IEnumerable<ReleaseDefinitionApprovalStep>) e.Approvals)))
        definitionApprovalStep.Approver = identityHelper.GetIdentity(definitionApprovalStep.Approver);
      foreach (ReleaseDefinitionApprovalStep definitionApprovalStep in environments.Select<ReleaseEnvironment, ReleaseDefinitionApprovals>((Func<ReleaseEnvironment, ReleaseDefinitionApprovals>) (e => e.PostApprovalsSnapshot)).Where<ReleaseDefinitionApprovals>((Func<ReleaseDefinitionApprovals, bool>) (e => e != null)).SelectMany<ReleaseDefinitionApprovals, ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovals, IEnumerable<ReleaseDefinitionApprovalStep>>) (e => (IEnumerable<ReleaseDefinitionApprovalStep>) e.Approvals)))
        definitionApprovalStep.Approver = identityHelper.GetIdentity(definitionApprovalStep.Approver);
      foreach (ReleaseDeployPhase releaseDeployPhase in environments.SelectMany<ReleaseEnvironment, DeploymentAttempt>((Func<ReleaseEnvironment, IEnumerable<DeploymentAttempt>>) (e => (IEnumerable<DeploymentAttempt>) e.DeploySteps)).SelectMany<DeploymentAttempt, ReleaseDeployPhase>((Func<DeploymentAttempt, IEnumerable<ReleaseDeployPhase>>) (ds => (IEnumerable<ReleaseDeployPhase>) ds.ReleaseDeployPhases)))
      {
        List<ManualIntervention> list = releaseDeployPhase.ManualInterventions.ToList<ManualIntervention>();
        ManualInterventionIdentityHandler.PopulateIdentities((IList<ManualIntervention>) list, identityHelper);
        releaseDeployPhase.ManualInterventions = (IEnumerable<ManualIntervention>) list;
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Required for functionality.")]
    public void PopulateIdentities(Release release, IdentityHelper identityHelper)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (identityHelper == null)
        throw new ArgumentNullException(nameof (identityHelper));
      release.CreatedBy = identityHelper.GetIdentity(release.CreatedBy);
      release.CreatedFor = identityHelper.GetIdentity(release.CreatedFor);
      release.ModifiedBy = identityHelper.GetIdentity(release.ModifiedBy);
      ReleaseIdentityHandler.PopulateIdentities((IEnumerable<ReleaseEnvironment>) release.Environments, identityHelper);
    }
  }
}
