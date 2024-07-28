// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.WebApi.CommentResourceReference
// Assembly: Microsoft.Azure.DevOps.Comments.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A55BAA93-5FAF-48BE-A9EC-2F097131C70D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DevOps.Comments.WebApi
{
  [DataContract]
  public class CommentResourceReference : ISecuredObject
  {
    internal Guid m_namespaceId;
    internal int m_requiredPermissions;
    internal string m_token;

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    Guid ISecuredObject.NamespaceId => this.m_namespaceId;

    int ISecuredObject.RequiredPermissions => this.m_requiredPermissions;

    string ISecuredObject.GetToken() => this.m_token;

    public void SetSecuredObject(ISecuredObject securedObject)
    {
      if (securedObject == null)
        return;
      this.m_namespaceId = securedObject.NamespaceId;
      this.m_requiredPermissions = securedObject.RequiredPermissions;
      this.m_token = securedObject.GetToken();
    }
  }
}
