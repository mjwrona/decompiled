// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.MentionStatsPerfCounters
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

namespace Microsoft.TeamFoundation.Mention.Server
{
  public static class MentionStatsPerfCounters
  {
    private const string UriBase = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_Mention";
    public const string GitCommitMentionCandidatesPerSecond = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionGitCommitMentionCandidatesPerSecond";
    public const string DiscussionMentionCandidatesPerSecond = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionDiscussionMentionCandidatesPerSecond";
    public const string WitDiscussionMentionCandidatesPerSecond = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionWitDiscussionMentionCandidatesPerSecond";
    public const string AverageMentionCandidatesProcessingTime = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionAverageMentionCandidatesProcessingTime";
    public const string AverageMentionCandidatesProcessingTimeBase = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionAverageMentionCandidatesProcessingTimeBase";
    public const string ProcessingFailuresPerSecond = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionProcessingFailuresPerSecond";
    public const string AverageWitActionProcessingTime = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionAverageWitActionProcessingTime";
    public const string AverageWitActionProcessingTimeBase = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionAverageWitActionProcessingTimeBase";
    public const string WitActionsPerSecond = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionWitActionsPerSecond";
    public const string WitActionFailuresPerSecond = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionWitActionFailuresPerSecond";
    public const string EmailActionsPerSecond = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionEmailActionsPerSecond";
    public const string EmailActionFailuresPerSecond = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionEmailActionFailuresPerSecond";
    public const string AverageEmailActionProcessingTime = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionAverageEmailActionProcessingTime";
    public const string AverageEmailActionProcessingTimeBase = "Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionAverageEmailActionProcessingTimeBase";
  }
}
