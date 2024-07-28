// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Deployment name is more suitable")]
  [DataContract]
  public class Deployment : ReleaseManagementSecuredObject
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public ReleaseReference Release { get; set; }

    [Obsolete("Use ReleaseDefinitionReference instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShallowReference ReleaseDefinition
    {
      get => (ShallowReference) this.ReleaseDefinitionReference;
      set => this.ReleaseDefinitionReference = value.ToReleaseDefinitionShallowReference();
    }

    [DataMember(Name = "ReleaseDefinition")]
    public ReleaseDefinitionShallowReference ReleaseDefinitionReference { get; set; }

    [Obsolete("Use ReleaseEnvironmentReference instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShallowReference ReleaseEnvironment
    {
      get => (ShallowReference) this.ReleaseEnvironmentReference;
      set => this.ReleaseEnvironmentReference = value.ToReleaseEnvironmentShallowReference();
    }

    [DataMember(Name = "ReleaseEnvironment")]
    public ReleaseEnvironmentShallowReference ReleaseEnvironmentReference { get; set; }

    [DataMember]
    public ProjectReference ProjectReference { get; set; }

    [DataMember]
    public int DefinitionEnvironmentId { get; set; }

    [DataMember]
    public int Attempt { get; set; }

    [DataMember]
    public DeploymentReason Reason { get; set; }

    [DataMember]
    public DeploymentStatus DeploymentStatus { get; set; }

    [DataMember]
    public DeploymentOperationStatus OperationStatus { get; set; }

    [DataMember]
    public IdentityRef RequestedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef RequestedFor { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime QueuedOn { get; set; }

    [DataMember]
    public DateTime StartedOn { get; set; }

    [DataMember]
    public DateTime CompletedOn { get; set; }

    [DataMember]
    public DateTime LastModifiedOn { get; set; }

    [DataMember]
    public IdentityRef LastModifiedBy { get; set; }

    [DataMember]
    public List<Condition> Conditions { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? ScheduledDeploymentTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<ReleaseApproval> PreDeployApprovals { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<ReleaseApproval> PostDeployApprovals { get; set; }

    [Obsolete("Use ReleaseReference instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    public Deployment()
    {
      this.PreDeployApprovals = new List<ReleaseApproval>();
      this.PostDeployApprovals = new List<ReleaseApproval>();
      this.Release = new ReleaseReference();
      this.Conditions = new List<Condition>();
      this.Links = new ReferenceLinks();
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.Release?.SetSecuredObject(token, requiredPermissions);
      this.ReleaseDefinitionReference?.SetSecuredObject(token, requiredPermissions);
      this.ReleaseEnvironmentReference?.SetSecuredObject(token, requiredPermissions);
      this.Conditions?.ForEach((Action<Condition>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.PreDeployApprovals?.ForEach((Action<ReleaseApproval>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.PostDeployApprovals?.ForEach((Action<ReleaseApproval>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.ReleaseDefinition?.SetSecuredObject(token, requiredPermissions);
      this.ReleaseEnvironment?.SetSecuredObject(token, requiredPermissions);
      ReferenceLinks links = this.Links;
      this.Links = links != null ? links.GetSecuredReferenceLinks(token, requiredPermissions) : (ReferenceLinks) null;
    }
  }
}
