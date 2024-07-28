// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.SecuredTagList
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [DataContract]
  public sealed class SecuredTagList : List<string>, ISecuredObject
  {
    private readonly ISecuredObject m_securedObject;

    public SecuredTagList(IEnumerable<string> tags, ISecuredObject securedObject)
      : base(tags)
    {
      this.m_securedObject = securedObject;
    }

    Guid ISecuredObject.NamespaceId => this.m_securedObject.NamespaceId;

    int ISecuredObject.RequiredPermissions => this.m_securedObject.RequiredPermissions;

    string ISecuredObject.GetToken() => this.m_securedObject.GetToken();
  }
}
