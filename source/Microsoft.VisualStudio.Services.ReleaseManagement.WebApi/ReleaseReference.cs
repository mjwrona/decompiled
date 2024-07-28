// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReference
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
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
  public class ReleaseReference : ReleaseManagementSecuredObject
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [Obsolete("Use Links instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "The name Url is appropariate name for this property and it signifies REST URL of resource")]
    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [DataMember(EmitDefaultValue = false)]
    [XmlIgnore]
    public IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> Artifacts { get; set; }

    [Obsolete("Use Links instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "This is used for xsl transforms, cannot be an object")]
    [DataMember(EmitDefaultValue = false)]
    public string WebAccessUri { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

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
    public DateTime CreatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ModifiedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseReason Reason { get; set; }

    public ReleaseReference()
    {
      this.Artifacts = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>();
      this.Links = new ReferenceLinks();
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> artifacts = this.Artifacts;
      if (artifacts != null)
        artifacts.ForEach<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>((Action<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.ReleaseDefinitionReference?.SetSecuredObject(token, requiredPermissions);
      this.Links = this.Links.GetSecuredReferenceLinks(token, requiredPermissions);
      this.ReleaseDefinition?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
