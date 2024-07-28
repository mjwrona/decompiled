// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.SecuredObject
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class SecuredObject : ISecuredObject
  {
    private readonly Guid m_namespaceId;
    private readonly int m_requiredPermissions;
    private readonly string m_token;

    public SecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      this.m_namespaceId = namespaceId;
      this.m_requiredPermissions = requiredPermissions;
      this.m_token = token;
    }

    public Guid NamespaceId => this.m_namespaceId;

    public int RequiredPermissions => this.m_requiredPermissions;

    public string GetToken() => this.m_token;
  }
}
