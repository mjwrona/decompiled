// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseEnvironment
  {
    private const string StrongBoxLookupKeyPrefixFormat = "Environments/{0}/Variables";
    private const string StrongBoxSecretVariableGroupPrefixFormat = "Environments/{0}/VariableGroups/{1}/Variables";

    public int Id { get; set; }

    public Guid ProjectId { get; set; }

    public string Name { get; set; }

    public int ReleaseId { get; set; }

    public int DefinitionEnvironmentId { get; set; }

    public int Rank { get; set; }

    public IDictionary<string, ConfigurationVariableValue> Variables { get; private set; }

    public IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> VariableGroups { get; }

    public EnvironmentOptions EnvironmentOptions { get; set; }

    public Guid OwnerId { get; set; }

    public IList<ReleaseEnvironmentStep> GetStepsForTests { get; private set; }

    public ReleaseEnvironmentStatus Status { get; set; }

    public DateTime? ScheduledDeploymentTime { get; set; }

    public Guid ScheduledOperationId { get; set; }

    public ApprovalOptions PreApprovalOptions { get; set; }

    public ApprovalOptions PostApprovalOptions { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    public IList<ReleaseCondition> Conditions { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    public IList<ReleaseSchedule> Schedules { get; set; }

    public IList<ReleaseDeployPhase> ReleaseDeployPhases { get; private set; }

    public IList<Deployment> DeploymentAttempts { get; private set; }

    public IList<ManualIntervention> ManualInterventions { get; private set; }

    public ProcessParameters ProcessParameters { get; set; }

    public ReleaseDefinitionGatesStep PreDeploymentGates { get; set; }

    public ReleaseDefinitionGatesStep PostDeploymentGates { get; set; }

    public DateTime? DeploymentLastModifiedOn { get; set; }

    public ReleaseEnvironment()
    {
      this.GetStepsForTests = (IList<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>();
      this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.VariableGroups = (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) new List<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>();
      this.Conditions = (IList<ReleaseCondition>) new List<ReleaseCondition>();
      this.Schedules = (IList<ReleaseSchedule>) new List<ReleaseSchedule>();
      this.EnvironmentOptions = new EnvironmentOptions();
      this.ReleaseDeployPhases = (IList<ReleaseDeployPhase>) new List<ReleaseDeployPhase>();
      this.DeploymentAttempts = (IList<Deployment>) new List<Deployment>();
      this.ManualInterventions = (IList<ManualIntervention>) new List<ManualIntervention>();
      this.PreDeploymentGates = new ReleaseDefinitionGatesStep();
      this.PostDeploymentGates = new ReleaseDefinitionGatesStep();
      this.DeploymentSnapshot = (IDeploymentSnapshot) new DesignerDeploymentSnapshot();
    }

    public virtual string SecretVariableLookupPrefix => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Environments/{0}/Variables", (object) this.Id);

    public IDeploymentSnapshot DeploymentSnapshot { get; set; }

    public virtual string VariableGroupSecretsKeyPrefix(int groupId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Environments/{0}/VariableGroups/{1}/Variables", (object) this.Id, (object) groupId);

    public ReleaseEnvironment DeepClone()
    {
      ReleaseEnvironment environment = new ReleaseEnvironment()
      {
        Id = this.Id,
        Name = this.Name,
        ReleaseId = this.ReleaseId,
        DefinitionEnvironmentId = this.DefinitionEnvironmentId,
        Rank = this.Rank
      };
      if (this.PreApprovalOptions != null)
        environment.PreApprovalOptions = this.PreApprovalOptions.DeepClone();
      if (this.PostApprovalOptions != null)
        environment.PostApprovalOptions = this.PostApprovalOptions.DeepClone();
      this.GetStepsForTests.ToList<ReleaseEnvironmentStep>().ForEach((Action<ReleaseEnvironmentStep>) (step => environment.GetStepsForTests.Add(step.Clone())));
      environment.DeploymentSnapshot = this.DeploymentSnapshot.DeepClone();
      VariablesUtility.FillVariables(this.Variables, environment.Variables);
      environment.VariableGroups.AddRange<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>>((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) VariableGroupUtility.CloneVariableGroups(this.VariableGroups));
      if (this.EnvironmentOptions != null)
        environment.EnvironmentOptions = this.EnvironmentOptions.DeepClone();
      if (this.Conditions != null)
        this.Conditions.ToList<ReleaseCondition>().ForEach((Action<ReleaseCondition>) (condition => environment.Conditions.Add(condition.Clone())));
      if (this.ReleaseDeployPhases != null && this.ReleaseDeployPhases.Any<ReleaseDeployPhase>())
        this.ReleaseDeployPhases.ToList<ReleaseDeployPhase>().ForEach((Action<ReleaseDeployPhase>) (phase => environment.ReleaseDeployPhases.Add(phase.DeepClone())));
      return environment;
    }

    public virtual void FillSecrets(ReleaseEnvironment environmentWithSecrets)
    {
      if (environmentWithSecrets == null)
        throw new ArgumentNullException(nameof (environmentWithSecrets));
      VariablesUtility.FillSecrets(environmentWithSecrets.Variables, this.Variables);
      VariableGroupUtility.FillSecrets(environmentWithSecrets.VariableGroups, this.VariableGroups);
    }

    public virtual bool HasSecretsWithValues() => VariablesUtility.HasSecretsWithValues(this.Variables) || VariableGroupUtility.HasSecret(this.VariableGroups);

    public virtual bool HasSecrets() => VariablesUtility.HasSecrets(this.Variables) || VariableGroupUtility.HasSecret(this.VariableGroups);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupting serialized data.")]
    public int GetNextTrialNumberForSteps()
    {
      if (this.DeploymentAttempts.Any<Deployment>())
        return this.DeploymentAttempts.OrderBy<Deployment, DateTime>((Func<Deployment, DateTime>) (d => d.QueuedOn)).Last<Deployment>().Attempt;
      return !this.GetStepsForTests.ToList<ReleaseEnvironmentStep>().Any<ReleaseEnvironmentStep>() ? 1 : this.GetLatestTrialNumber() + 1;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupting serialized data.")]
    public int GetLatestTrialNumber()
    {
      if (this.DeploymentAttempts.Any<Deployment>())
        return this.DeploymentAttempts.OrderBy<Deployment, DateTime>((Func<Deployment, DateTime>) (d => d.QueuedOn)).Last<Deployment>().Attempt;
      return this.GetStepsForTests.Any<ReleaseEnvironmentStep>() ? this.GetStepsForTests.OrderBy<ReleaseEnvironmentStep, DateTime>((Func<ReleaseEnvironmentStep, DateTime>) (s => s.CreatedOn)).Last<ReleaseEnvironmentStep>().TrialNumber : 0;
    }

    public Deployment GetDeploymentByAttempt(int attempt) => this.DeploymentAttempts.SingleOrDefault<Deployment>((Func<Deployment, bool>) (deployment => deployment.Attempt == attempt));

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupting serialized data.")]
    public int GetLatestDeploymentId()
    {
      Deployment latestDeployment = this.GetLatestDeployment();
      return latestDeployment == null ? 0 : latestDeployment.Id;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupting serialized data.")]
    public Deployment GetLatestDeployment() => !this.DeploymentAttempts.Any<Deployment>() ? (Deployment) null : this.DeploymentAttempts.OrderBy<Deployment, DateTime>((Func<Deployment, DateTime>) (d => d.QueuedOn)).Last<Deployment>();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupting serialized data.")]
    public Guid GetLatestDeploymentRequestedFor()
    {
      Deployment latestDeployment = this.GetLatestDeployment();
      return latestDeployment == null ? Guid.Empty : latestDeployment.RequestedFor;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupting serialized data.")]
    public Deployment GetLastDeploymentAttempt() => this.DeploymentAttempts.Where<Deployment>((Func<Deployment, bool>) (x => x.OperationStatus != DeploymentOperationStatus.Queued)).OrderBy<Deployment, DateTime>((Func<Deployment, DateTime>) (d => d.QueuedOn)).LastOrDefault<Deployment>();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupting serialized data.")]
    public ReleaseEnvironmentStep GetLastRunReleaseStep() => this.GetStepsForTests.OrderBy<ReleaseEnvironmentStep, DateTime>((Func<ReleaseEnvironmentStep, DateTime>) (s => s.CreatedOn)).LastOrDefault<ReleaseEnvironmentStep>();

    public IEnumerable<ReleaseDeployPhase> GetDeployPhases(int attempt) => this.ReleaseDeployPhases.Where<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (x => x.Attempt == attempt));

    public ReleaseDeployPhase GetDeployPhase(int attempt, int deployPhaseId)
    {
      IEnumerable<ReleaseDeployPhase> deployPhases = this.GetDeployPhases(attempt);
      return deployPhaseId == 0 ? deployPhases.Single<ReleaseDeployPhase>() : deployPhases.Single<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (x => x.Id == deployPhaseId));
    }

    public ReleaseDeployPhase GetDeployPhase(int releaseDeployPhaseId) => this.ReleaseDeployPhases.Single<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (x => x.Id == releaseDeployPhaseId));

    public bool HasManualPreDeploymentApproval() => this.GetStepsForTests.Any<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (x => x.StepType == EnvironmentStepType.PreDeploy && !x.IsAutomated));

    public bool IsWaitingOnPreDeployApproval()
    {
      ReleaseEnvironmentStep pendingEnvironmentStep = this.GetFirstPendingEnvironmentStep();
      return pendingEnvironmentStep != null && pendingEnvironmentStep.StepType == EnvironmentStepType.PreDeploy && pendingEnvironmentStep.Status == ReleaseEnvironmentStepStatus.Pending && this.Status == ReleaseEnvironmentStatus.InProgress && !pendingEnvironmentStep.IsAutomated;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupting serialized data.")]
    public IEnumerable<ReleaseEnvironmentStep> GetApprovalSteps(
      EnvironmentStepType stepType,
      int trialNumber)
    {
      return this.GetStepsForTests.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (x => x.IsApprovalStep() && x.StepType == stepType && x.TrialNumber == trialNumber));
    }

    public ReleaseEnvironmentStep GetLatestApprovalStep(EnvironmentStepType stepType) => this.GetStepsForTests.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (x => x.IsApprovalStep() && x.StepType == stepType)).OrderByDescending<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (s => s.TrialNumber)).FirstOrDefault<ReleaseEnvironmentStep>();

    private ReleaseEnvironmentStep GetFirstPendingEnvironmentStep() => this.GetStepsForTests.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.Status == ReleaseEnvironmentStepStatus.Pending)).OrderBy<ReleaseEnvironmentStep, DateTime>((Func<ReleaseEnvironmentStep, DateTime>) (s => s.CreatedOn)).FirstOrDefault<ReleaseEnvironmentStep>();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Leave this as method.")]
    public int GetStepsCount() => this.GetStepsForTests.Count;

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Leave this as method.")]
    public IEnumerable<ReleaseEnvironmentStep> GetDeployOrGateSteps() => this.GetStepsForTests.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (re => re.StepType == EnvironmentStepType.Deploy || re.StepType == EnvironmentStepType.PreGate || re.StepType == EnvironmentStepType.PostGate));

    public int GetStepStatusCount(
      ReleaseEnvironmentStepStatus stepStatus,
      EnvironmentStepType stepType)
    {
      return this.GetStepsForTests.Count<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.StepType == stepType && s.Status == stepStatus));
    }

    public void AddReleaseEnvironmentStep(ReleaseEnvironmentStep step) => this.GetStepsForTests.Add(step);

    public bool CompareEnforceIdentityRevalidation(EnvironmentStepType approvalType, bool isEnabled)
    {
      switch (approvalType)
      {
        case EnvironmentStepType.PreDeploy:
          return (this.PreApprovalOptions == null ? 0 : (this.PreApprovalOptions.EnforceIdentityRevalidation ? 1 : 0)) == (isEnabled ? 1 : 0);
        case EnvironmentStepType.PostDeploy:
          return (this.PostApprovalOptions == null ? 0 : (this.PostApprovalOptions.EnforceIdentityRevalidation ? 1 : 0)) == (isEnabled ? 1 : 0);
        default:
          return false;
      }
    }

    public ApprovalOptions GetApprovalOptions(EnvironmentStepType approvalType)
    {
      if (approvalType == EnvironmentStepType.PreDeploy)
        return this.PreApprovalOptions;
      return approvalType == EnvironmentStepType.PostDeploy ? this.PostApprovalOptions : (ApprovalOptions) null;
    }

    public ReleaseDefinitionGatesStep GetGateStepData(EnvironmentStepType gateType)
    {
      if (gateType == EnvironmentStepType.PreGate)
        return this.PreDeploymentGates;
      return gateType == EnvironmentStepType.PostGate ? this.PostDeploymentGates : (ReleaseDefinitionGatesStep) null;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Leave this as method.")]
    public ReleaseEnvironmentStep GetLatestCompletedDeployStep()
    {
      int trialNumber = this.GetLatestTrialNumber();
      return this.GetStepsForTests.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (x => x.StepType == EnvironmentStepType.Deploy && x.TrialNumber == trialNumber && x.Status == ReleaseEnvironmentStepStatus.Done)).FirstOrDefault<ReleaseEnvironmentStep>();
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Leave this as method.")]
    public ReleaseEnvironmentStep GetLatestDeployStep()
    {
      int trialNumber = this.GetLatestTrialNumber();
      return this.GetStepsForTests.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (x => x.StepType == EnvironmentStepType.Deploy && x.TrialNumber == trialNumber)).FirstOrDefault<ReleaseEnvironmentStep>();
    }

    public bool IsYamlEnvironment() => this.DeploymentSnapshot is YamlDeploymentSnapshot;

    public void AddDesignerDeployPhaseSnapshot(DeployPhaseSnapshot snapshot)
    {
      if (this.IsYamlEnvironment())
        throw new NotSupportedException();
      ((DesignerDeploymentSnapshot) this.DeploymentSnapshot).DeployPhaseSnapshots.Add(snapshot);
    }

    public bool HasPhaseType(DeployPhaseTypes phaseType)
    {
      if (this.DeploymentSnapshot is DesignerDeploymentSnapshot deploymentSnapshot)
      {
        foreach (DeployPhaseSnapshot deployPhaseSnapshot in (IEnumerable<DeployPhaseSnapshot>) deploymentSnapshot.DeployPhaseSnapshots)
        {
          if (deployPhaseSnapshot.PhaseType == phaseType)
            return true;
        }
      }
      return false;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Leave this as method.")]
    public int GetLastDeploymentAttemptId()
    {
      Deployment latestDeployment = this.GetLatestDeployment();
      return latestDeployment == null ? 0 : latestDeployment.Attempt;
    }
  }
}
