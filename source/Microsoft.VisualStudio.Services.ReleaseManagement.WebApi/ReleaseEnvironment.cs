// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class ReleaseEnvironment : ReleaseManagementSecuredObject
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public int ReleaseId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public EnvironmentStatus Status { get; set; }

    [XmlIgnore]
    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, ConfigurationVariableValue> Variables { get; private set; }

    [XmlIgnore]
    [DataMember(EmitDefaultValue = false)]
    public IList<VariableGroup> VariableGroups { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public List<ReleaseApproval> PreDeployApprovals { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<ReleaseApproval> PostDeployApprovals { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseDefinitionApprovals PreApprovalsSnapshot { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseDefinitionApprovals PostApprovalsSnapshot { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<DeploymentAttempt> DeploySteps { get; set; }

    [DataMember]
    public int Rank { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int DefinitionEnvironmentId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [Obsolete("Use DeploymentInput.QueueId instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int QueueId { get; set; }

    [XmlIgnore]
    [DataMember(EmitDefaultValue = false)]
    public EnvironmentOptions EnvironmentOptions { get; set; }

    [XmlIgnore]
    [DataMember(EmitDefaultValue = false)]
    [Obsolete("Use DeploymentInput.Demands instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<Demand> Demands { get; private set; }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [DataMember(EmitDefaultValue = false)]
    public List<ReleaseCondition> Conditions { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CreatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? ModifiedOn { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [XmlIgnore]
    [DataMember(EmitDefaultValue = false)]
    [Obsolete("Use DeployPhase.WorkflowTasks instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<WorkflowTask> WorkflowTasks { get; set; }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Caller will be added soon")]
    [XmlIgnore]
    [DataMember(EmitDefaultValue = false)]
    public IList<DeployPhase> DeployPhasesSnapshot { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef Owner { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? ScheduledDeploymentTime { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [DataMember(EmitDefaultValue = false)]
    public List<ReleaseSchedule> Schedules { get; }

    [Obsolete("Use ReleaseReference instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShallowReference Release
    {
      get => (ShallowReference) this.ReleaseReference;
      set => this.ReleaseReference = value.ToReleaseShallowReference();
    }

    [DataMember(Name = "Release", EmitDefaultValue = false)]
    public ReleaseShallowReference ReleaseReference { get; set; }

    [Obsolete("Use ReleaseDefinitionReference instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShallowReference ReleaseDefinition
    {
      get => (ShallowReference) this.ReleaseDefinitionReference;
      set => this.ReleaseDefinitionReference = value.ToReleaseDefinitionShallowReference();
    }

    [DataMember(Name = "ReleaseDefinition", EmitDefaultValue = false)]
    public ReleaseDefinitionShallowReference ReleaseDefinitionReference { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ReleaseCreatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string TriggerReason { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public double TimeToDeploy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [Obsolete("Use Release object Description instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string ReleaseDescription { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? NextScheduledUtcTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProcessParameters ProcessParameters { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseDefinitionGatesStep PreDeploymentGatesSnapshot { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseDefinitionGatesStep PostDeploymentGatesSnapshot { get; set; }

    public ReleaseEnvironment()
    {
      this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.VariableGroups = (IList<VariableGroup>) new List<VariableGroup>();
      this.EnvironmentOptions = new EnvironmentOptions();
      this.PreDeployApprovals = new List<ReleaseApproval>();
      this.PostDeployApprovals = new List<ReleaseApproval>();
      this.Demands = new List<Demand>();
      this.WorkflowTasks = new List<WorkflowTask>();
      this.Conditions = new List<ReleaseCondition>();
      this.DeployPhasesSnapshot = (IList<DeployPhase>) new List<DeployPhase>();
      this.DeploySteps = new List<DeploymentAttempt>();
      this.PreApprovalsSnapshot = new ReleaseDefinitionApprovals();
      this.PostApprovalsSnapshot = new ReleaseDefinitionApprovals();
      this.Schedules = new List<ReleaseSchedule>();
      this.PreDeploymentGatesSnapshot = new ReleaseDefinitionGatesStep();
      this.PostDeploymentGatesSnapshot = new ReleaseDefinitionGatesStep();
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IDictionary<string, ConfigurationVariableValue> variables = this.Variables;
      if (variables != null)
        variables.ForEach<KeyValuePair<string, ConfigurationVariableValue>>((Action<KeyValuePair<string, ConfigurationVariableValue>>) (i => i.Value?.SetSecuredObject(token, requiredPermissions)));
      IList<VariableGroup> variableGroups = this.VariableGroups;
      if (variableGroups != null)
        variableGroups.ForEach<VariableGroup>((Action<VariableGroup>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.PreDeployApprovals?.ForEach((Action<ReleaseApproval>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.PostDeployApprovals?.ForEach((Action<ReleaseApproval>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.PreApprovalsSnapshot?.SetSecuredObject(token, requiredPermissions);
      this.PostApprovalsSnapshot?.SetSecuredObject(token, requiredPermissions);
      this.DeploySteps?.ForEach((Action<DeploymentAttempt>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.EnvironmentOptions?.SetSecuredObject(token, requiredPermissions);
      this.Demands?.ForEach((Action<Demand>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.WorkflowTasks?.ForEach((Action<WorkflowTask>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.Conditions?.ForEach((Action<ReleaseCondition>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      IList<DeployPhase> deployPhasesSnapshot = this.DeployPhasesSnapshot;
      if (deployPhasesSnapshot != null)
        deployPhasesSnapshot.ForEach<DeployPhase>((Action<DeployPhase>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.Schedules?.ForEach((Action<ReleaseSchedule>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.ReleaseReference?.SetSecuredObject(token, requiredPermissions);
      this.ReleaseDefinitionReference?.SetSecuredObject(token, requiredPermissions);
      this.PreDeploymentGatesSnapshot?.SetSecuredObject(token, requiredPermissions);
      this.PostDeploymentGatesSnapshot?.SetSecuredObject(token, requiredPermissions);
      this.ProcessParameters = this.ProcessParameters?.Clone((ISecuredObject) new ReleaseManagementSecuredObject(token, requiredPermissions));
    }
  }
}
