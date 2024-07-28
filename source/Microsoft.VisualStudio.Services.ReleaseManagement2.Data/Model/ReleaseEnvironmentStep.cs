// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentStep
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseEnvironmentStep
  {
    public int ReleaseId { get; set; }

    public Guid ProjectId { get; set; }

    public int ReleaseEnvironmentId { get; set; }

    public int Id { get; set; }

    public string ReleaseName { get; set; }

    public string ReleaseEnvironmentName { get; set; }

    public int ReleaseDefinitionId { get; set; }

    public string ReleaseDefinitionName { get; set; }

    public string ReleaseDefinitionPath { get; set; }

    public int DefinitionEnvironmentId { get; set; }

    public int DefinitionEnvironmentRank { get; set; }

    public bool IsAutomated { get; set; }

    public EnvironmentStepType StepType { get; set; }

    public Guid ApproverId { get; set; }

    public int Rank { get; set; }

    public int TrialNumber { get; set; }

    public ReleaseEnvironmentStepStatus Status { get; set; }

    public string ApproverComment { get; set; }

    public Guid ActualApproverId { get; set; }

    public int GroupStepId { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }

    public string Logs { get; set; }

    public bool HasStarted { get; set; }

    public Guid ApprovalTimeoutJobId { get; set; }

    public DateTime DeploymentStartTime { get; set; }

    public Guid? RunPlanId { get; set; }

    public DateTime? DeploymentLastModifiedOn { get; set; }

    public bool IsApprovalStep() => this.StepType == EnvironmentStepType.PreDeploy || this.StepType == EnvironmentStepType.PostDeploy;

    public bool IsGateStep() => this.StepType == EnvironmentStepType.PreGate || this.StepType == EnvironmentStepType.PostGate;

    public ReleaseEnvironmentStep Clone() => new ReleaseEnvironmentStep()
    {
      Id = this.Id,
      IsAutomated = this.IsAutomated,
      TrialNumber = this.TrialNumber,
      ApproverId = this.ApproverId,
      ActualApproverId = this.ActualApproverId,
      ApproverComment = this.ApproverComment,
      Status = this.Status,
      StepType = this.StepType,
      Rank = this.Rank,
      DefinitionEnvironmentId = this.DefinitionEnvironmentId,
      DefinitionEnvironmentRank = this.DefinitionEnvironmentRank,
      CreatedOn = this.CreatedOn,
      ModifiedOn = this.ModifiedOn,
      ReleaseId = this.ReleaseId,
      ReleaseEnvironmentId = this.ReleaseEnvironmentId,
      GroupStepId = this.GroupStepId,
      Logs = this.Logs,
      HasStarted = this.HasStarted,
      ApprovalTimeoutJobId = this.ApprovalTimeoutJobId,
      RunPlanId = this.RunPlanId
    };

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupting serialized data.")]
    public Guid GetDeferredExecutionJobId() => this.ApprovalTimeoutJobId;

    public virtual IEnumerable<ReleaseEnvironmentStep> EvaluateApprovalPolicyAutoTriggeredAndPreviousEnvironmentApproved(
      Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (this.StepType == EnvironmentStepType.PreDeploy)
      {
        Guid currentStepApproverId = this.ApproverId;
        ReleaseEnvironment environmentByName1 = release.GetEnvironmentByName(this.ReleaseEnvironmentName);
        if (environmentByName1 != null && environmentByName1.PreApprovalOptions != null && environmentByName1.PreApprovalOptions.AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped)
        {
          Deployment deployment = environmentByName1.DeploymentAttempts.OrderByDescending<Deployment, int>((Func<Deployment, int>) (x => x.Attempt)).FirstOrDefault<Deployment>();
          if (deployment != null && (deployment.Reason == DeploymentReason.Automated || deployment.Reason == DeploymentReason.Scheduled))
          {
            foreach (ReleaseCondition condition in (IEnumerable<ReleaseCondition>) environmentByName1.Conditions)
            {
              if (condition.ConditionType == ConditionType.EnvironmentState)
              {
                ReleaseEnvironment environmentByName2 = release.GetEnvironmentByName(condition.Name);
                if (environmentByName2 != null)
                {
                  Deployment previousEnvironmentLatestAttempt = environmentByName2.DeploymentAttempts.OrderByDescending<Deployment, int>((Func<Deployment, int>) (x => x.Attempt)).FirstOrDefault<Deployment>();
                  if (previousEnvironmentLatestAttempt != null)
                  {
                    foreach (ReleaseEnvironmentStep releaseEnvironmentStep in environmentByName2.GetStepsForTests.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.StepType == EnvironmentStepType.PreDeploy && step.ApproverId == currentStepApproverId && step.Status == ReleaseEnvironmentStepStatus.Done && step.TrialNumber == previousEnvironmentLatestAttempt.Attempt)))
                      yield return releaseEnvironmentStep;
                  }
                }
              }
            }
          }
        }
      }
    }
  }
}
