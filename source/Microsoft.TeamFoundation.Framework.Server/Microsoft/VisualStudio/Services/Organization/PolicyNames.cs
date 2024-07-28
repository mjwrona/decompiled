// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PolicyNames
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  [GenerateAllConstants(null)]
  public static class PolicyNames
  {
    public const string DisallowBasicAuthentication = "Policy.DisallowBasicAuthentication";
    public const string DisallowOAuthAuthentication = "Policy.DisallowOAuthAuthentication";
    public const string DisallowAadGuestUserAccess = "Policy.DisallowAadGuestUserAccess";
    public const string DisallowSecureShell = "Policy.DisallowSecureShell";
    public const string EnforceAadAuthorization = "Policy.EnforceAadAuthorization";
    public const string AuthenticationCredentialValidFrom = "Policy.AuthenticationCredentialValidFrom";
    public const string IsInternal = "Policy.IsInternal";
    public const string AllowAnonymousAccess = "Policy.AllowAnonymousAccess";
    public const string AllowOrgAccess = "Policy.AllowOrgAccess";
    public const string EnforceAADConditionalAccess = "Policy.EnforceAADConditionalAccess";
    public const string IsReadOnly = "Policy.IsReadOnly";
    public const string AllowGitHubInvitations = "Policy.AllowGitHubInvitationsAccessToken";
    public const string AllowRequestAccess = "Policy.AllowRequestAccessToken";
    public const string LogAuditEvents = "Policy.LogAuditEvents";
    public const string AllowTeamAdminsInvitations = "Policy.AllowTeamAdminsInvitationsAccessToken";
    public const string ArtifactsExternalPackageProtection = "Policy.ArtifactsExternalPackageProtectionToken";
    public const string Namespace = "Policy";
    private const string NamespaceSeparator = ".";
    private const string DissallowBasicAuthenticationToken = "DisallowBasicAuthentication";
    private const string DissallowOAuthAuthenticationToken = "DisallowOAuthAuthentication";
    private const string DisallowAadGuestUserAccessToken = "DisallowAadGuestUserAccess";
    private const string DisallowSecureShellToken = "DisallowSecureShell";
    private const string EnforceAadAuthorizationToken = "EnforceAadAuthorization";
    private const string AllowAnonymousAccessToken = "AllowAnonymousAccess";
    private const string AllowOrgAccessToken = "AllowOrgAccess";
    private const string AuthenticationCredentialValidFromToken = "AuthenticationCredentialValidFrom";
    private const string IsInternalToken = "IsInternal";
    private const string EnforceConditionalAccessToken = "EnforceAADConditionalAccess";
    private const string IsReadOnlyToken = "IsReadOnly";
    private const string AllowGitHubInvitationsAccessToken = "AllowGitHubInvitationsAccessToken";
    private const string AllowRequestAccessToken = "AllowRequestAccessToken";
    private const string AllowTeamAdminsInvitationsAccessToken = "AllowTeamAdminsInvitationsAccessToken";
    private const string ArtifactsExternalPackageProtectionToken = "ArtifactsExternalPackageProtectionToken";
    private const string LogAuditEventsToken = "LogAuditEvents";
    public static readonly HashSet<string> KnownSystemPolicies = new HashSet<string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)
    {
      "Policy.AuthenticationCredentialValidFrom",
      "Policy.EnforceAadAuthorization",
      "Policy.IsInternal",
      "Policy.IsReadOnly"
    };
  }
}
