// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.GitRepositoryInfo
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebServer;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  public class GitRepositoryInfo : VersionControlRepositoryInfo
  {
    public GitRepositoryInfo(IVssRequestContext requestContext, ITfsGitRepository repository)
      : base(VersionControlRepositoryType.Git, (VersionControlProvider) new GitVersionControlProvider(requestContext, repository))
    {
    }

    public GitVersionControlProvider GitProvider => (GitVersionControlProvider) this.Provider;
  }
}
