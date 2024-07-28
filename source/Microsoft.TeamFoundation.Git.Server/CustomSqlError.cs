// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CustomSqlError
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal enum CustomSqlError
  {
    SqlServerDefaultUserMessage = 50000, // 0x0000C350
    TransactionRequired = 800000, // 0x000C3500
    GenericDatabaseUpdateFailure = 1200002, // 0x00124F82
    GitRepositoryNotFound = 1200009, // 0x00124F89
    GitRepositoryNameAlreadyExists = 1200010, // 0x00124F8A
    GitRefNotFound = 1200011, // 0x00124F8B
    GitRepositoryPerProjectThresholdExceeded = 1200012, // 0x00124F8C
    GitRepositoryMinimumPerProjectThresholdExceeded = 1200013, // 0x00124F8D
    GitPullRequestNotFound = 1200014, // 0x00124F8E
    GitPullRequestNotEditable = 1200015, // 0x00124F8F
    GitPullRequestExists = 1200016, // 0x00124F90
    GitPullRequestRepositoryRequiredForBranchName = 1200017, // 0x00124F91
    GitRefLockDenied = 1200018, // 0x00124F92
    GitRefUnlockDenied = 1200019, // 0x00124F93
    GitPullRequestCannotBeActivated = 1200020, // 0x00124F94
    GitCommitIdNotFound = 1200021, // 0x00124F95
    GenericError = 1200022, // 0x00124F96
    GitRepositoryNotFoundForCommits = 1200023, // 0x00124F97
    GitRepositoryNotFoundForRefs = 1200024, // 0x00124F98
    GitInvalidParentSpecified = 1200026, // 0x00124F9A
    GitDuplicateRefFavorite = 1200027, // 0x00124F9B
    GitAsyncOperationNotFound = 1200028, // 0x00124F9C
    GitAsyncOperationAlreadyExists = 1200029, // 0x00124F9D
    GitAsyncOperationUpdateFailed = 1200030, // 0x00124F9E
    GitRepositoryNameStateConstrain = 1200031, // 0x00124F9F
  }
}
