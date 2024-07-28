// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class ManualIntervention : ReleaseManagementSecuredObject
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public ManualInterventionStatus Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef Approver { get; set; }

    [DataMember]
    public Guid TaskInstanceId { get; set; }

    [DataMember]
    public string Comments { get; set; }

    [DataMember]
    public string Instructions { get; set; }

    [DataMember]
    public DateTime CreatedOn { get; set; }

    [DataMember]
    public DateTime ModifiedOn { get; set; }

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

    [Obsolete("Use ReleaseEnvironmentReference instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShallowReference ReleaseEnvironment
    {
      get => (ShallowReference) this.ReleaseEnvironmentReference;
      set => this.ReleaseEnvironmentReference = value.ToReleaseEnvironmentShallowReference();
    }

    [DataMember(Name = "ReleaseEnvironment", EmitDefaultValue = false)]
    public ReleaseEnvironmentShallowReference ReleaseEnvironmentReference { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Reviewed")]
    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.ReleaseReference?.SetSecuredObject(token, requiredPermissions);
      this.ReleaseDefinitionReference?.SetSecuredObject(token, requiredPermissions);
      this.ReleaseEnvironmentReference?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
