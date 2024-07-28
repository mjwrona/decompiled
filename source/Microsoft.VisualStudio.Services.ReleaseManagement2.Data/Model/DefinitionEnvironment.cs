// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DefinitionEnvironment
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.TeamFoundation.Framework.Server;
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
  public class DefinitionEnvironment : ForeignKeyModelBase
  {
    private const string StrongBoxLookupKeyPrefixFormat = "{0}/Environments/{1}/Variables";
    private IList<PropertyValue> properties;

    public int Id { get; set; }

    public Guid ProjectId { get; set; }

    public string Name { get; set; }

    public int Rank { get; set; }

    public Guid OwnerId { get; set; }

    public EnvironmentOptions EnvironmentOptions { get; set; }

    public ApprovalOptions PreApprovalOptions { get; set; }

    public ApprovalOptions PostApprovalOptions { get; set; }

    public IList<DefinitionEnvironmentStep> GetStepsForTests { get; private set; }

    public IList<DeployPhase> DeployPhases { get; }

    public IDictionary<string, ConfigurationVariableValue> Variables { get; private set; }

    public IList<int> VariableGroups { get; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    public IList<Condition> Conditions { get; set; }

    public EnvironmentExecutionPolicy ExecutionPolicy { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    public IList<ReleaseSchedule> Schedules { get; set; }

    public int ReleaseDefinitionId { get; set; }

    public int CurrentReleaseId { get; set; }

    public EnvironmentRetentionPolicy RetentionPolicy { get; set; }

    public ProcessParameters ProcessParameters { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    public IList<EnvironmentTrigger> EnvironmentTriggers { get; set; }

    public string BadgeUrl { get; set; }

    public IList<PropertyValue> Properties
    {
      get
      {
        if (this.properties == null)
          this.properties = (IList<PropertyValue>) new List<PropertyValue>();
        return this.properties;
      }
    }

    public ReleaseDefinitionGatesStep PreDeploymentGates { get; set; }

    public ReleaseDefinitionGatesStep PostDeploymentGates { get; set; }

    public DateTime ModifiedOn { get; set; }

    public DefinitionEnvironment()
    {
      this.GetStepsForTests = (IList<DefinitionEnvironmentStep>) new List<DefinitionEnvironmentStep>();
      this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.VariableGroups = (IList<int>) new List<int>();
      this.Conditions = (IList<Condition>) new List<Condition>();
      this.ExecutionPolicy = new EnvironmentExecutionPolicy();
      this.EnvironmentOptions = new EnvironmentOptions();
      this.Schedules = (IList<ReleaseSchedule>) new List<ReleaseSchedule>();
      this.DeployPhases = (IList<DeployPhase>) new List<DeployPhase>();
      this.PreDeploymentGates = new ReleaseDefinitionGatesStep();
      this.PostDeploymentGates = new ReleaseDefinitionGatesStep();
      this.EnvironmentTriggers = (IList<EnvironmentTrigger>) new List<EnvironmentTrigger>();
    }

    public virtual string GetSecretVariableLookupPrefix(int releaseDefinitionRevision) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/Environments/{1}/Variables", (object) releaseDefinitionRevision, (object) this.Id);

    public virtual void FillSecrets(DefinitionEnvironment environmentWithSecrets)
    {
      if (environmentWithSecrets == null)
        throw new ArgumentNullException(nameof (environmentWithSecrets));
      VariablesUtility.FillSecrets(environmentWithSecrets.Variables, this.Variables);
    }

    public DefinitionEnvironment DeepClone()
    {
      DefinitionEnvironment environment = new DefinitionEnvironment()
      {
        Id = this.Id,
        Name = this.Name,
        Rank = this.Rank,
        OwnerId = this.OwnerId,
        CurrentReleaseId = this.CurrentReleaseId
      };
      this.GetStepsForTests.ToList<DefinitionEnvironmentStep>().ForEach((Action<DefinitionEnvironmentStep>) (step => environment.GetStepsForTests.Add(step.Clone())));
      this.DeployPhases.ToList<DeployPhase>().ForEach((Action<DeployPhase>) (phase => environment.DeployPhases.Add(phase.DeepClone())));
      VariablesUtility.FillVariables(this.Variables, environment.Variables);
      environment.VariableGroups.AddRange<int, IList<int>>((IEnumerable<int>) this.VariableGroups);
      environment.EnvironmentOptions = new EnvironmentOptions()
      {
        EmailNotificationType = this.EnvironmentOptions.EmailNotificationType,
        EmailRecipients = this.EnvironmentOptions.EmailRecipients
      };
      environment.ExecutionPolicy = new EnvironmentExecutionPolicy()
      {
        ConcurrencyCount = this.ExecutionPolicy.ConcurrencyCount,
        QueueDepthCount = this.ExecutionPolicy.QueueDepthCount
      };
      environment.PreApprovalOptions = this.PreApprovalOptions?.DeepClone();
      environment.PostApprovalOptions = this.PostApprovalOptions?.DeepClone();
      if (this.Conditions != null)
        this.Conditions.ToList<Condition>().ForEach((Action<Condition>) (condition => environment.Conditions.Add(condition.Clone())));
      if (this.Properties.Any<PropertyValue>())
      {
        foreach (PropertyValue property in (IEnumerable<PropertyValue>) this.Properties)
          environment.Properties.Add(property);
      }
      return environment;
    }

    public virtual bool HasSecretsWithValues() => VariablesUtility.HasSecretsWithValues(this.Variables);

    public virtual bool HasSecrets() => VariablesUtility.HasSecrets(this.Variables);

    public bool ExecutionPolicyEquals(DefinitionEnvironment environment) => environment != null && this.GetConcurrencyCount() == environment.GetConcurrencyCount() && this.GetQueueDepthCount() == environment.GetQueueDepthCount();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupt serialized data")]
    public int GetConcurrencyCount() => this.ExecutionPolicy != null && this.ExecutionPolicy.ConcurrencyCount > 0 ? this.ExecutionPolicy.ConcurrencyCount : 0;

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupt serialized data")]
    public int GetQueueDepthCount() => this.ExecutionPolicy != null && this.ExecutionPolicy.ConcurrencyCount > 0 && this.ExecutionPolicy.QueueDepthCount == 1 ? 1 : 0;

    public IEnumerable<DefinitionEnvironmentStep> GetSteps(EnvironmentStepType stepType) => this.GetStepsForTests.Where<DefinitionEnvironmentStep>((Func<DefinitionEnvironmentStep, bool>) (s => s.StepType == stepType));
  }
}
