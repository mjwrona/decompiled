// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitStatsPerfCounters
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class GitStatsPerfCounters
  {
    private const string UriBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_Git";
    internal const string CurrentPushes = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitCurrentPushes";
    internal const string CurrentFetches = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitCurrentFetches";
    internal const string CurrentClones = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitCurrentClones";
    internal const string ObjectsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitObjectsPerSec";
    internal const string SmallRepos = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitSmallRepos";
    internal const string MediumRepos = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitMediumRepos";
    internal const string LargeRepos = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitLargeRepos";
    internal const string XLargeRepos = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitXLargeRepos";
    internal const string CommitManifestJobs = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitCommitManifestJobs";
    internal const string RepackerJobs = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitRepackerJobs";
    public const string CommitMentionProcessingJobs = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitCommitMentionProcessingJobs";
    internal const string AutoCompleteJobs = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitAutoCompleteJobs";
    internal const string CompletionQueueJobs = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_GitCompletionQueueJobs";
  }
}
