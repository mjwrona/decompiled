// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseApproval
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class ReleaseApproval : ReleaseManagementSecuredObject
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public int Revision { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef Approver { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ApprovedBy { get; set; }

    [DataMember]
    public ApprovalType ApprovalType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime ModifiedOn { get; set; }

    [DataMember]
    public ApprovalStatus Status { get; set; }

    [DataMember]
    public string Comments { get; set; }

    [DataMember]
    public bool IsAutomated { get; set; }

    [Obsolete("Use Notifications instead.")]
    [DataMember]
    public bool IsNotificationOn { get; set; }

    [Obsolete("Use Attempt instead.")]
    [DataMember]
    public int TrialNumber { get; set; }

    [DataMember]
    public int Attempt { get; set; }

    [DataMember]
    public int Rank { get; set; }

    [XmlIgnore]
    [DataMember(EmitDefaultValue = false)]
    public List<ReleaseApprovalHistory> History { get; set; }

    [XmlIgnore]
    [Obsolete("Use ReleaseReference instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShallowReference Release
    {
      get => (ShallowReference) this.ReleaseReference;
      set => this.ReleaseReference = value.ToReleaseShallowReference();
    }

    [XmlIgnore]
    [DataMember(Name = "Release", EmitDefaultValue = false)]
    public ReleaseShallowReference ReleaseReference { get; set; }

    [XmlIgnore]
    [Obsolete("Use ReleaseDefinitionReference instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShallowReference ReleaseDefinition
    {
      get => (ShallowReference) this.ReleaseDefinitionReference;
      set => this.ReleaseDefinitionReference = value.ToReleaseDefinitionShallowReference();
    }

    [XmlIgnore]
    [DataMember(Name = "ReleaseDefinition", EmitDefaultValue = false)]
    public ReleaseDefinitionShallowReference ReleaseDefinitionReference { get; set; }

    [XmlIgnore]
    [Obsolete("Use ReleaseEnvironmentReference instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShallowReference ReleaseEnvironment
    {
      get => (ShallowReference) this.ReleaseEnvironmentReference;
      set => this.ReleaseEnvironmentReference = value.ToReleaseEnvironmentShallowReference();
    }

    [XmlIgnore]
    [DataMember(Name = "ReleaseEnvironment", EmitDefaultValue = false)]
    public ReleaseEnvironmentShallowReference ReleaseEnvironmentReference { get; set; }

    [XmlIgnore]
    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj) => obj is ReleaseApproval releaseApproval && this.IsAutomated == releaseApproval.IsAutomated && (this.IsAutomated || (this.Approver != null || releaseApproval.Approver == null) && (this.Approver == null || releaseApproval.Approver != null) && !(this.Approver.Id != releaseApproval.Approver.Id));

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.History?.ForEach((Action<ReleaseApprovalHistory>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.ReleaseReference?.SetSecuredObject(token, requiredPermissions);
      this.ReleaseDefinitionReference?.SetSecuredObject(token, requiredPermissions);
      this.ReleaseEnvironmentReference?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
