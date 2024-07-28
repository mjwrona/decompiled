// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.SearchSecuredObject
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class SearchSecuredObject : ISecuredObject
  {
    private Guid m_namespaceId;
    private string m_token;
    private int m_requiredPermissions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      this.m_requiredPermissions = requiredPermissions;
      this.m_token = token;
      this.m_namespaceId = namespaceId;
    }

    string ISecuredObject.GetToken() => this.m_token;

    [IgnoreDataMember]
    Guid ISecuredObject.NamespaceId => this.m_namespaceId;

    [IgnoreDataMember]
    int ISecuredObject.RequiredPermissions => this.m_requiredPermissions;
  }
}
