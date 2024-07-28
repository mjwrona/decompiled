// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.SecuredObject
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class SecuredObject : ISecuredObject
  {
    protected Guid NamespaceId { get; set; }

    protected int RequiredPermissions { get; set; }

    protected string Token { get; set; }

    public SecuredObject()
    {
    }

    public SecuredObject(Guid namespaceId, string token, int requiredPermissions)
    {
      this.NamespaceId = namespaceId;
      this.Token = token;
      this.RequiredPermissions = requiredPermissions;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SetSecuredObject(Guid namespaceId, string token, int requiredPermissions)
    {
      this.NamespaceId = namespaceId;
      this.RequiredPermissions = requiredPermissions;
      this.Token = token;
    }

    Guid ISecuredObject.NamespaceId => this.NamespaceId;

    int ISecuredObject.RequiredPermissions => this.RequiredPermissions;

    string ISecuredObject.GetToken() => this.Token != null ? this.Token : throw new ArgumentNullException("Token");
  }
}
