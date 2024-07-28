// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.ProjectAnalysisConstants
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public static class ProjectAnalysisConstants
  {
    public static class FeatureFlags
    {
      public const string EnableProjectAnalysis = "ProjectAnalysis.Server.EnableProjectAnalysis";
      public const string EnableTfvcAnalysis = "ProjectAnalysis.Server.EnableProjectAnalysis.Tfvc";
    }

    internal static class Sql
    {
      internal const int MaxBranchNameLength = 400;
      internal const int MaxLanguageBreakdownLength = 2147483647;
    }

    internal static class CustomerIntelligence
    {
      public const string Area = "Microsoft.TeamFoundation.ProjectAnalysis.Server";
      public const string Action = "Action";
      public const string ActionLanguageJobFinished = "LanguageJobFinished";
      public const string ActionLanguageJobQueued = "LanguageJobQueued";
      public const string ActionLanguageJobReceived = "LanguageJobReceived";
      public const string ActionLanguageJobStarted = "LanguageJobStarted";
      public const string ActionLanguageJobStopped = "LanguageJobStopped";
      public const string ActionSaveHistoricalData = "SaveHistoricalData";
      public const string FeatureLanguage = "Language";
      public const string KeyBranch = "Branch";
      public const string KeyChangeSetId = "ChangeSetId";
      public const string KeyCommitId = "CommitId";
      public const string KeyFileCount = "FileCount";
      public const string KeyJobId = "JobId";
      public const string KeyJobData = "JobData";
      public const string KeyLanguageBreakdown = "LanguageBreakdown";
      public const string KeyLastFileCount = "LastFileCount";
      public const string KeyLastUpdatedTime = "LastUpdatedTime";
      public const string KeyNoPreexistingRecord = "NoPreexistingRecord";
      public const string KeyRepositoryId = "RepositoryId";
      public const string KeyRepositoryType = "RepositoryType";
      public const string KeySubmittedFrom = "SubmittedFrom";
      public const string KeyTimeElaspsedInMilliseconds = "TimeElapsedInMilliseconds";
    }

    public static class Settings
    {
      public static readonly string ProjectAnalysisPath = "/Service/ProjectAnalysis/";
      public static readonly string LanguageMetadataRepoClassifierPath = ProjectAnalysisConstants.Settings.ProjectAnalysisPath + "LanguageMetadata/";
      public static readonly string SmallRepoKey = ProjectAnalysisConstants.Settings.LanguageMetadataRepoClassifierPath + "SmallRepositorySize";
      public static readonly string MidSizedRepoKey = ProjectAnalysisConstants.Settings.LanguageMetadataRepoClassifierPath + "MediumRepositorySize";
      public static readonly string MidSizedRepoAnalyseFreqKey = ProjectAnalysisConstants.Settings.LanguageMetadataRepoClassifierPath + "MediumRepositoryAnalyseFrequencyInMinutes";
      public static readonly string LargeSizedRepoAnalyseFreqKey = ProjectAnalysisConstants.Settings.LanguageMetadataRepoClassifierPath + "LargeRepositoryAnalyseFrequencyInMinutes";
      public const int DefaultSmallRepoSizeCap = 30000;
      public const int DefaultMidSizedRepoCap = 150000;
      public const int DefaultMinutesBeforeScheduleMidSizedRepo = 1440;
      public const int DefaultMinutesBeforeScheduleLargeRepo = 4320;
      public static readonly string ProjectAnalysisTfvcRepositoryPath = ProjectAnalysisConstants.Settings.ProjectAnalysisPath + "TfvcRepository/";
      public static readonly string TfvcPageSizeKey = ProjectAnalysisConstants.Settings.ProjectAnalysisTfvcRepositoryPath + "PageSize";
      public static readonly string TfvcSleepTimeKey = ProjectAnalysisConstants.Settings.ProjectAnalysisTfvcRepositoryPath + "SleepTime";
      public const int DefaultPageSize = 10000;
      public const int DefaultTimeToSleep = 1000;
    }
  }
}
