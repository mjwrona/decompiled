// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.SearchSecuredV2Object
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class SearchSecuredV2Object : ISecuredObject
  {
    private Guid m_namespaceId;
    private string m_token;
    private int m_requiredPermissions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      this.m_requiredPermissions = requiredPermissions;
      this.m_token = token;
      this.m_namespaceId = namespaceId;
    }

    protected static void SetSecuredObject(
      IEnumerable<SearchSecuredV2Object> securableObjects,
      Guid namespaceId,
      int requiredPermissions,
      string token)
    {
      if (securableObjects == null)
        return;
      foreach (SearchSecuredV2Object securableObject in securableObjects)
        securableObject.SetSecuredObject(namespaceId, requiredPermissions, token);
    }

    string ISecuredObject.GetToken() => this.m_token;

    [IgnoreDataMember]
    Guid ISecuredObject.NamespaceId => this.m_namespaceId;

    [IgnoreDataMember]
    int ISecuredObject.RequiredPermissions => this.m_requiredPermissions;
  }
}
