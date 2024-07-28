// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.BaseSecuredObject
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [DataContract]
  public abstract class BaseSecuredObject : ISecuredObject
  {
    internal Guid m_namespaceId;
    internal int m_requiredPermissions;
    internal string m_token;

    protected BaseSecuredObject()
    {
    }

    protected BaseSecuredObject(ISecuredObject securedObject)
    {
      if (securedObject == null)
        return;
      this.m_namespaceId = securedObject.NamespaceId;
      this.m_requiredPermissions = securedObject.RequiredPermissions;
      this.m_token = securedObject.GetToken();
    }

    Guid ISecuredObject.NamespaceId => this.m_namespaceId;

    int ISecuredObject.RequiredPermissions => this.m_requiredPermissions;

    string ISecuredObject.GetToken() => this.m_token;
  }
}
