// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ShallowReference
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [KnownType(typeof (ReleaseDefinition))]
  [KnownType(typeof (ReleaseDefinitionShallowReference))]
  [KnownType(typeof (ReleaseEnvironmentShallowReference))]
  [KnownType(typeof (ReleaseShallowReference))]
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("This is deprecated. Use the specific type's reference, e.g. ReleaseDefinitionReference")]
  public class ShallowReference : ReleaseManagementSecuredObject
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "The name Url is appropariate name for this property and it signifies REST URL of resource")]
    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    public ShallowReference() => this.Links = new ReferenceLinks();

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      ReferenceLinks links = this.Links;
      this.Links = links != null ? links.GetSecuredReferenceLinks(token, requiredPermissions) : (ReferenceLinks) null;
    }
  }
}
