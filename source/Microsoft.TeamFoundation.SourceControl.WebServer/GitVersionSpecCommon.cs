// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitVersionSpecCommon
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitVersionSpecCommon
  {
    public static readonly char GitIdentifierPrefix = 'G';
    public static readonly char GitBranchIdentifier = 'B';
    public static readonly char GitCommitIdentifier = 'C';
    public static readonly char GitTagIdentifier = 'T';
    private static readonly Dictionary<Type, GitVersionType> TypeToEnumMap = new Dictionary<Type, GitVersionType>()
    {
      [typeof (GitBranchVersionSpec)] = GitVersionType.Branch,
      [typeof (GitTagVersionSpec)] = GitVersionType.Tag,
      [typeof (GitCommitVersionSpec)] = GitVersionType.Commit
    };

    public static bool IsGitVersionSpec(string versionSpec) => versionSpec.Length >= 3 && (int) char.ToUpperInvariant(versionSpec[0]) == (int) GitVersionSpecCommon.GitIdentifierPrefix;

    public static GitVersionSpec Parse(string versionSpec)
    {
      char ch = GitVersionSpecCommon.IsGitVersionSpec(versionSpec) ? char.ToUpperInvariant(versionSpec[1]) : throw new InvalidVersionSpecException(versionSpec);
      string str = versionSpec.Substring(2);
      if ((int) ch == (int) GitVersionSpecCommon.GitBranchIdentifier)
        return (GitVersionSpec) new GitBranchVersionSpec(str);
      if ((int) ch == (int) GitVersionSpecCommon.GitCommitIdentifier)
        return (GitVersionSpec) new GitCommitVersionSpec(str);
      if ((int) ch == (int) GitVersionSpecCommon.GitTagIdentifier)
        return (GitVersionSpec) new GitTagVersionSpec(str);
      throw new InvalidVersionSpecException(versionSpec);
    }

    public static GitVersionType VersionSpecToEnum(GitVersionSpec versionSpec) => GitVersionSpecCommon.TypeToEnumMap[versionSpec.GetType()];
  }
}
