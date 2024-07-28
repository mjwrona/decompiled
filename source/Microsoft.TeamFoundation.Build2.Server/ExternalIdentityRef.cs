// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ExternalIdentityRef
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public sealed class ExternalIdentityRef : IdentityRef, ISecuredObject
  {
    private ISecuredObject m_securedObject;

    public ExternalIdentityRef(ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      this.m_securedObject = securedObject;
    }

    Guid ISecuredObject.NamespaceId => this.m_securedObject.NamespaceId;

    int ISecuredObject.RequiredPermissions => this.m_securedObject.RequiredPermissions;

    string ISecuredObject.GetToken() => this.m_securedObject.GetToken();
  }
}
