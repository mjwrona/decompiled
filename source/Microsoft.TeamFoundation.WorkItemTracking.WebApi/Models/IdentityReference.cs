// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.IdentityReference
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class IdentityReference : IdentityRef, ISecuredObject
  {
    private int permission;
    private string token;
    private Guid namespaceId;

    public IdentityReference()
    {
    }

    public IdentityReference(IdentityRef identityRef, string witIdentityName)
      : this(identityRef)
    {
      this.Name = witIdentityName;
    }

    public IdentityReference(IdentityRef identityRef)
    {
      if (identityRef == null)
        return;
      this.Id = identityRef.Id;
      this.DisplayName = identityRef.DisplayName;
      this.DirectoryAlias = identityRef.DirectoryAlias;
      this.ImageUrl = identityRef.ImageUrl;
      this.Inactive = identityRef.Inactive;
      this.IsAadIdentity = identityRef.IsAadIdentity;
      this.IsContainer = identityRef.IsContainer;
      this.ProfileUrl = identityRef.ProfileUrl;
      this.UniqueName = identityRef.UniqueName;
      this.Url = identityRef.Url;
      this.Descriptor = identityRef.Descriptor;
      this.Links = identityRef.Links;
      ISecuredObject securedObject = (ISecuredObject) identityRef;
      this.token = securedObject.GetToken();
      this.namespaceId = securedObject.NamespaceId;
      this.permission = securedObject.RequiredPermissions;
    }

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid Id
    {
      get => !string.IsNullOrEmpty(base.Id) ? Guid.Parse(base.Id) : Guid.Empty;
      set => this.Id = value.ToString();
    }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    [JsonConverter(typeof (DefaultValueOnPublicAccessJsonConverter<string>))]
    public string Name { get; set; }

    Guid ISecuredObject.NamespaceId => this.namespaceId;

    int ISecuredObject.RequiredPermissions => this.permission;

    string ISecuredObject.GetToken() => this.token;
  }
}
