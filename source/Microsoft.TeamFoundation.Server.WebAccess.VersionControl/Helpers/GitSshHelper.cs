// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.Helpers.GitSshHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Git.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl.Helpers
{
  public static class GitSshHelper
  {
    public static string GetSshUrl(
      TfsWebContext tfsWebContext,
      string repositoryRemoteUrl,
      out bool sshEnabled)
    {
      string sshUrl = GitServerUtils.GetSshUrl(tfsWebContext.TfsRequestContext, repositoryRemoteUrl, out sshEnabled);
      return sshEnabled ? sshUrl : string.Empty;
    }
  }
}
