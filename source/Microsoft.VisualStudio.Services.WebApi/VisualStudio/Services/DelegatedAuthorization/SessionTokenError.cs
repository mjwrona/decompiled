// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionTokenError
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public enum SessionTokenError
  {
    None,
    DisplayNameRequired,
    InvalidDisplayName,
    InvalidValidTo,
    InvalidScope,
    UserIdRequired,
    InvalidUserId,
    InvalidUserType,
    AccessDenied,
    FailedToIssueAccessToken,
    InvalidClient,
    InvalidClientType,
    InvalidClientId,
    InvalidTargetAccounts,
    HostAuthorizationNotFound,
    AuthorizationNotFound,
    FailedToUpdateAccessToken,
    SourceNotSupported,
    InvalidSourceIP,
    InvalidSource,
    DuplicateHash,
    SSHPolicyDisabled,
    InvalidToken,
    TokenNotFound,
    InvalidAuthorizationId,
    FailedToReadTenantPolicy,
    GlobalPatPolicyViolation,
    FullScopePatPolicyViolation,
    PatLifespanPolicyViolation,
    InvalidTokenType,
    InvalidAudience,
    InvalidSubject,
    DeploymentHostNotSupported,
  }
}
