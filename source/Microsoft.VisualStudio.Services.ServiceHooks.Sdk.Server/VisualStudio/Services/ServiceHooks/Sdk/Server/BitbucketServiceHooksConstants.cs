// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.BitbucketServiceHooksConstants
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public class BitbucketServiceHooksConstants
  {
    public const string ServiceHooksExternalGitPushEventType = "externalgit.push";
    public const string ServiceHooksExternalGitPullRequestType = "externalgit.pullrequest";
    public const string BitbucketPushEventType = "repo:push";
    public const string BitbucketPullRequestCreatedEventType = "pullrequest:created";
    public const string BitbucketPullRequestUpdatedEventType = "pullrequest:updated";
    public const string BitbucketPublisherId = "bitbucket";
    public const string BitbucketInputIdHookId = "hookId";
    public const string BitbucketInputIdChannelId = "channelId";
    public const string BitbucketInputIdProject = "project";
    public const string BitbucketInputIdConnectedServiceId = "connectedServiceId";
    public const string BitbucketInputIdConnectedServicesList = "connectedServicesList";
    public const string BitbucketInputIdRepo = "repo";
    public const string BitbucketInputIdBranch = "branch";
    public const string BitbucketApiVersion = "2.0";
    public const string BitbucketDrawerName = "bitbucket";
    public const string BitbucketWebhookSecretKey = "WebhookSecret";
    public const string Path = "path";
    public const string BitbucketApiUrl = "apiUrl";
    public const string BitbucketCloneUrl = "cloneUrl";
    public const string Username = "username";
    public const string BitbucketRepoFullName = "fullName";
    public const string BitbucketDefaultBranch = "defaultBranch";
    public const string Type = "type";
    public const string ID = "id";
    public const string CommitID = "commitId";
    public const string Url = "url";
    public const string IsFolder = "isFolder";
    public const string Tree = "tree";
    public const string Children = "children";
    public const string ReposInputId = "repos";
    public const string RepoContentInputId = "repoContent";
    public const string RepoBranchesInputId = "repoBranches";
    public const string RepoFileContentInputId = "repoFileContent";
  }
}
