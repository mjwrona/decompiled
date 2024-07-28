// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentAttempt
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class DeploymentAttempt : ReleaseManagementSecuredObject
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public int DeploymentId { get; set; }

    [DataMember]
    public int Attempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DeploymentReason Reason { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DeploymentStatus Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DeploymentOperationStatus OperationStatus { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [XmlIgnore]
    [DataMember(EmitDefaultValue = false)]
    public IList<ReleaseDeployPhase> ReleaseDeployPhases { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef RequestedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef RequestedFor { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime QueuedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef LastModifiedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime LastModifiedOn { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool HasStarted { get; set; }

    [Obsolete("Use ReleaseDeployPhase.DeploymentJobs.Tasks instead.")]
    [DataMember(EmitDefaultValue = false)]
    public List<ReleaseTask> Tasks { get; set; }

    [Obsolete("Use ReleaseDeployPhase.DeploymentJobs.Job instead.")]
    [DataMember(EmitDefaultValue = false)]
    public ReleaseTask Job { get; set; }

    [Obsolete("Use ReleaseDeployPhase.RunPlanId instead.")]
    [DataMember]
    public Guid RunPlanId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseGates PreDeploymentGates { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseGates PostDeploymentGates { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [Obsolete("Instead use Issues which contains both errors and warnings related to deployment")]
    public string ErrorLog { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<Issue> Issues { get; private set; }

    public DeploymentAttempt()
    {
      this.Tasks = new List<ReleaseTask>();
      this.ReleaseDeployPhases = (IList<ReleaseDeployPhase>) new List<ReleaseDeployPhase>();
      this.Issues = new List<Issue>();
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IList<ReleaseDeployPhase> releaseDeployPhases = this.ReleaseDeployPhases;
      if (releaseDeployPhases != null)
        releaseDeployPhases.ForEach<ReleaseDeployPhase>((Action<ReleaseDeployPhase>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.Tasks?.ForEach((Action<ReleaseTask>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.Job?.SetSecuredObject(token, requiredPermissions);
      this.PreDeploymentGates?.SetSecuredObject(token, requiredPermissions);
      this.PostDeploymentGates?.SetSecuredObject(token, requiredPermissions);
      this.Issues?.ForEach((Action<Issue>) (i => i?.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
