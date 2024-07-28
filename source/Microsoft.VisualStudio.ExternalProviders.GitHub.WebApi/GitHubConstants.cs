// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GitHubConstants
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  public static class GitHubConstants
  {
    public static readonly string InstallationId = "installationId";

    public static class FileMode
    {
      public static readonly int File = 100644;
      public static readonly int Executable = 100755;
      public static readonly int Subdirectory = 40000;
      public static readonly int Submodule = 160000;
      public static readonly int Symlink = 120000;
    }

    public static class StatusState
    {
      public static readonly string Success = "success";
      public static readonly string Pending = "pending";
      public static readonly string Failure = "failure";
      public static readonly string Error = "error";
    }

    public static class DeploymentStatus
    {
      public static readonly string Queued = "queued";
      public static readonly string InProgress = "in_progress";
      public static readonly string Success = "success";
      public static readonly string Failure = "failure";
      public static readonly string Error = "error";
      public static readonly string Inactive = "inactive";
    }

    public static class ContentType
    {
      public static readonly string File = "file";
      public static readonly string Directory = "dir";
      public static readonly string Symlink = "symlink";
      public static readonly string Submodule = "submodule";
    }

    public static class ObjectType
    {
      public static readonly string Commit = "commit";
      public static readonly string Tag = "tag";
    }

    public static class TargetType
    {
      public static readonly string Organization = nameof (Organization);
      public static readonly string User = nameof (User);
    }

    public static class StrongBoxKey
    {
      public static readonly string GitHubOAuthTokenDrawer = "GitHubOauthCallback";
      public static readonly string GitHubLaunchPrimary = "GitHubLaunchPrivateKeyPrimary";
      public static readonly string GitHubInstallationTokenSignatureSecret = nameof (GitHubInstallationTokenSignatureSecret);
      public static readonly string GitHubInstallationTokenSignatureSecretLookupKey = "TokenSignatureSecretKey";
      public static readonly string GitHubPipelineUserTokenDrawer = nameof (GitHubPipelineUserTokenDrawer);
      public static readonly string GitHubAzureBoardsPrivateKeyPrimary = nameof (GitHubAzureBoardsPrivateKeyPrimary);
      public static readonly string GitHubBoardsAppUserTokenDrawer = nameof (GitHubBoardsAppUserTokenDrawer);
      public static readonly string GitHubAzureDevOpsConnectorPrivateKey = nameof (GitHubAzureDevOpsConnectorPrivateKey);
    }

    public static class GitHubResponseHeaderKey
    {
      public const string GitHubEnterpriseVersionHeaderKey = "X-GitHub-Enterprise-Version";
    }

    public static class HeaderKey
    {
      public const string HeaderEventType = "X-GitHub-Event";
      public const string HeaderDeliveryId = "X-GitHub-Delivery";
      public const string HeaderEventSignature = "X-Hub-Signature";
      public const string HeaderEventSignature256 = "X-Hub-Signature-256";
    }

    public static class Action
    {
      public const string ActionKey = "action";
      public const string DeletedAction = "deleted";
      public const string ClosedAction = "closed";
    }

    public static class EventType
    {
      public const string CheckRun = "check_run";
      public const string CheckSuite = "check_suite";
      public const string Installation = "installation";
      public const string InstallationRespositories = "installation_repositories";
      public const string MarketplacePurchase = "marketplace_purchase";
      public const string PullRequest = "pull_request";
      public const string Push = "push";
      public const string Ping = "ping";
      public const string Issue = "issues";
      public const string IssueComment = "issue_comment";
      public const string CommitComment = "commit_comment";
      public const string Repository = "repository";
    }

    public static class ServiceEndpointDataKey
    {
      public const string OrgNodeId = "OrgNodeId";
      public const string IsValid = "IsValid";
      public const string OrgIntId = "orgIntId";
      public const string LoginPropertyName = "GitHubHandle";
    }

    public static class OrganizationRoles
    {
      public const string All = "all";
      public const string Member = "member";
      public const string Owner = "admin";
    }

    public static class WellKnownGitHubErrorMessages
    {
      public const string BadCredentials = "Bad credentials";
      public const string NotFound = "Not found";
    }

    public static class WellKnownGitHubErrorTypes
    {
      public const string FailedToGetInstallationToken = "Failed_To_Get_Installation_Token";
    }
  }
}
