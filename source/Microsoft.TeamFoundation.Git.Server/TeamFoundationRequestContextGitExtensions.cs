// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationRequestContextGitExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class TeamFoundationRequestContextGitExtensions
  {
    public static GitOdbComponent CreateGitOdbComponent(this IVssRequestContext rc, RepoKey repoKey) => rc.CreateGitOdbComponent(repoKey.OdbId);

    public static GitOdbComponent CreateGitOdbComponent(this IVssRequestContext rc, OdbId odbId)
    {
      using (rc.AllowCrossDataspaceAccess())
        return rc.CreateComponent<GitOdbComponent>("GitOdb", odbId.Value);
    }

    internal static GitCoreComponent CreateGitCoreComponent(this IVssRequestContext requestContext) => requestContext.CreateComponent<GitCoreComponent>("Git");

    internal static GitCoreComponent CreateReadOnlyGitCoreComponent(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("Git.Server.UseReadOnlyComponent") ? requestContext.CreateComponent<GitCoreComponent>("Git", new DatabaseConnectionType?(DatabaseConnectionType.IntentReadOnly)) : requestContext.CreateGitCoreComponent();
    }
  }
}
