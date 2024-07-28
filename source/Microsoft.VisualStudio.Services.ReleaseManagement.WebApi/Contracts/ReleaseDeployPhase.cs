// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [KnownType(typeof (ReleaseGatesPhase))]
  [JsonConverter(typeof (ReleaseDeployPhaseJsonConverter))]
  [DataContract]
  public class ReleaseDeployPhase : ReleaseManagementSecuredObject
  {
    [DataMember]
    [Obsolete]
    public int Id { get; set; }

    [DataMember]
    public string PhaseId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int Rank { get; set; }

    [DataMember]
    public DeployPhaseTypes PhaseType { get; set; }

    [DataMember]
    public DeployPhaseStatus Status { get; set; }

    [DataMember]
    public Guid? RunPlanId { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IList<DeploymentJob> DeploymentJobs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ErrorLog { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<ManualIntervention> ManualInterventions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartedOn { get; set; }

    public ReleaseDeployPhase()
    {
      this.DeploymentJobs = (IList<DeploymentJob>) new List<DeploymentJob>();
      this.ManualInterventions = (IEnumerable<ManualIntervention>) new List<ManualIntervention>();
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IList<DeploymentJob> deploymentJobs = this.DeploymentJobs;
      if (deploymentJobs != null)
        deploymentJobs.ForEach<DeploymentJob>((Action<DeploymentJob>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      IEnumerable<ManualIntervention> manualInterventions = this.ManualInterventions;
      if (manualInterventions == null)
        return;
      manualInterventions.ForEach<ManualIntervention>((Action<ManualIntervention>) (i => i?.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
