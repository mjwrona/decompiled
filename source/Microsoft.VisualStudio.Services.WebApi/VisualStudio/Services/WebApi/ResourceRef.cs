// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ResourceRef
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [DataContract]
  public class ResourceRef : ISecuredObject
  {
    private ISecuredObject m_securedObject;

    public ResourceRef()
    {
    }

    public ResourceRef(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "url", EmitDefaultValue = false)]
    public string Url { get; set; }

    public void SetSecuredObject(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    private ISecuredObject SecuredObject => this.m_securedObject != null ? this.m_securedObject : throw new InvalidOperationException("SecuredObject required but not set.");

    Guid ISecuredObject.NamespaceId => this.SecuredObject.NamespaceId;

    int ISecuredObject.RequiredPermissions => this.SecuredObject.RequiredPermissions;

    string ISecuredObject.GetToken() => this.SecuredObject.GetToken();
  }
}
