// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitServerConstants
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class GitServerConstants
  {
    public static readonly string ServerName = "Git";
    internal static readonly DateTime UtcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    internal static readonly Version MinimumRecommendedGitVersion = new Version("2.17.1");
    internal static readonly Sha1Id EmptyBlobHash = new Sha1Id("e69de29bb2d1d6434b8b29ae775ad8c2e48c5391");
    internal const long ObjectMaterializationLimit = 16777216;
    internal const string GitOdbDataspaceCategory = "GitOdb";
    internal const string GitCoreComponentServiceName = "GitCore";
    internal const string RepoIdClaim = "repoIds";
    public const string GitCommandStatusSli = "GitCommandStatusSli";
    public const int MaxCommitEntriesCount = 1000000;
    public const int MaxReviewersCount = 1000;
    public const int DefaultConcurrencyWorkUnitSize = 32;
    public const int DefaultConcurrencySize = 10000;
    public const int DefaultConcurrencyTarpit = 50;
    public const int DefaultMaxPathLength = 32766;
    public const int DefaultMaxPathComponentLength = 4096;
  }
}
