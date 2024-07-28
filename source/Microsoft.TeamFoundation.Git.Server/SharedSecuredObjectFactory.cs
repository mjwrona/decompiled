// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.SharedSecuredObjectFactory
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class SharedSecuredObjectFactory
  {
    public static ISecuredObject CreateTeamProjectReadOnly(Guid projectId)
    {
      string token = TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(projectId));
      return (ISecuredObject) new SharedSecuredObjectFactory.SharedSecuredObject(TeamProjectSecurityConstants.NamespaceId, TeamProjectSecurityConstants.GenericRead, token);
    }

    private class SharedSecuredObject : ISecuredObject
    {
      private readonly Guid m_namespaceId;
      private readonly string m_token;
      private readonly int m_requiredPermissions;

      public SharedSecuredObject(Guid namespaceId, int requiredPermissions, string token)
      {
        this.m_namespaceId = namespaceId;
        this.m_requiredPermissions = requiredPermissions;
        this.m_token = token;
      }

      string ISecuredObject.GetToken() => this.m_token;

      Guid ISecuredObject.NamespaceId => this.m_namespaceId;

      int ISecuredObject.RequiredPermissions => this.m_requiredPermissions;
    }
  }
}
