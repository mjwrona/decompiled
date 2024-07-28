// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitServiceEventTypes
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  public class GitServiceEventTypes
  {
    public const string GitPushEvent = "git.push.create";
    public const string GitRepoCreatedEvent = "git.repo.create";
    public const string GitRepoRenamedEvent = "git.repo.rename";
    public const string GitRepoDeletedEvent = "git.repo.delete";
    public const string GitRepoDisabledEvent = "git.repo.disable";
    public const string GitImportSucceededEvent = "ms.vss-code.git-import-succeeded-event";
    public const string GitImportFailedEvent = "ms.vss-code.git-import-failed-event";
    public const string GitPullRequestCreatedEvent = "git.pullrequest.created";
    public const string GitPullRequestMergeEvent = "git.pullrequest.merged";
    public const string GitPullRequestStatusUpdatedEvent = "git.pullrequest.status-updated";
  }
}
