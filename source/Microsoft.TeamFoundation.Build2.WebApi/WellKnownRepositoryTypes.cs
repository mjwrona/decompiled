// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.WellKnownRepositoryTypes
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [Obsolete("Use RepositoryTypes instead.")]
  public static class WellKnownRepositoryTypes
  {
    public const string TfsVersionControl = "TfsVersionControl";
    public const string TfsGit = "TfsGit";
    public const string Git = "Git";
    public const string GitHub = "GitHub";
    public const string GitHubEnterprise = "GitHubEnterprise";
    public const string Bitbucket = "Bitbucket";
    public const string Svn = "Svn";
  }
}
