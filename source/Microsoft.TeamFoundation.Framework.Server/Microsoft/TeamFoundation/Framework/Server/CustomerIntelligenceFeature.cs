// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CustomerIntelligenceFeature
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class CustomerIntelligenceFeature
  {
    public static readonly string AccountAssignSucceeded = "AccountCreate";
    public static readonly string AccountAssignFailed = nameof (AccountAssignFailed);
    public static readonly string AccountCreateSucceeded = "AccountCreateOnDemand";
    public static readonly string AccountCreateFailed = nameof (AccountCreateFailed);
    public static readonly string PutAccountInTrial = nameof (PutAccountInTrial);
    public static readonly string DeleteAccount = nameof (DeleteAccount);
    public static readonly string AccountHostAssign = nameof (AccountHostAssign);
    public static readonly string AccountHostCreate = nameof (AccountHostCreate);
    public static readonly string AccountHostCleanup = nameof (AccountHostCleanup);
    public static readonly string AccountRename = nameof (AccountRename);
    public static readonly string ProjectRename = nameof (ProjectRename);
    public static readonly string AccountOwnerChange = nameof (AccountOwnerChange);
    public static readonly string AccountMember = nameof (AccountMember);
    public static readonly string AccountCreationByMicrosoftMSAIdentity = nameof (AccountCreationByMicrosoftMSAIdentity);
    public static readonly string ActivateOrganization = nameof (ActivateOrganization);
    public static readonly string DeactivateOrganization = nameof (DeactivateOrganization);
    public static readonly string RenameOrganization = nameof (RenameOrganization);
    public static readonly string AddUserToAccount = nameof (AddUserToAccount);
    public static readonly string UrlDecoding = nameof (UrlDecoding);
    public static readonly string MemberEntitlementFailedAdd = nameof (MemberEntitlementFailedAdd);
    public static readonly string MemberEntitlementAdd = nameof (MemberEntitlementAdd);
    public static readonly string ExtensionEdit = nameof (ExtensionEdit);
    public static readonly string ProjectEdit = nameof (ProjectEdit);
    public static readonly string AddGuestUserToAad = nameof (AddGuestUserToAad);
    public static readonly string BillingCycleDifferences = nameof (BillingCycleDifferences);
    public static readonly string AccessLevelEdit = nameof (AccessLevelEdit);
    public static readonly string TraverseDescendants = nameof (TraverseDescendants);
    public static readonly string StakeholderLicenseEvaluated = nameof (StakeholderLicenseEvaluated);
    public static readonly string BasicPlusTestPlansMigration = nameof (BasicPlusTestPlansMigration);
    public static readonly string LicenseUsageReport = nameof (LicenseUsageReport);
    public static readonly string ExpandGroup = nameof (ExpandGroup);
    public static readonly string GetRulesForUser = nameof (GetRulesForUser);
    public static readonly string CreateGroupExtensionRule = nameof (CreateGroupExtensionRule);
    public static readonly string DeleteGroupExtensionRule = nameof (DeleteGroupExtensionRule);
    public static readonly string DeleteExtensionRulesForGroup = nameof (DeleteExtensionRulesForGroup);
    public static readonly string UpdateGroupExtensionRuleStatus = nameof (UpdateGroupExtensionRuleStatus);
    public static readonly string UpdateGroupExtensionRuleMissingLicenseCount = nameof (UpdateGroupExtensionRuleMissingLicenseCount);
    public static readonly string SetGroupLicenseRule = nameof (SetGroupLicenseRule);
    public static readonly string DeleteGroupLicenseRule = nameof (DeleteGroupLicenseRule);
    public static readonly string UpdateGroupLicenseRuleStatus = nameof (UpdateGroupLicenseRuleStatus);
    public static readonly string UpdateGroupLicenseRuleMissingLicenseCount = nameof (UpdateGroupLicenseRuleMissingLicenseCount);
    public static readonly string AssignedLicensesAndExtensions = nameof (AssignedLicensesAndExtensions);
    public static readonly string AppliedGroupRules = nameof (AppliedGroupRules);
    public static readonly string EvaluatedGroupRulesForNewUser = nameof (EvaluatedGroupRulesForNewUser);
    public static readonly string AppliedGroupRulesToNewUser = nameof (AppliedGroupRulesToNewUser);
    public static readonly string RevertGroupLicensing = nameof (RevertGroupLicensing);
    public static readonly string SignIn = nameof (SignIn);
    public static readonly string SignedIn = nameof (SignedIn);
    public static readonly string SignOut = nameof (SignOut);
    public static readonly string AuthenticationFailed = nameof (AuthenticationFailed);
    public static readonly string DeploymentGroupMembershipChange = nameof (DeploymentGroupMembershipChange);
    public static readonly string Download = nameof (Download);
    public static readonly string DownloadFlowPath = "DownloadFlow";
    public static readonly string DownloadPageView = nameof (DownloadPageView);
    public static readonly string CreateAccountPageView = nameof (CreateAccountPageView);
    public static readonly string CreateProfilePageView = nameof (CreateProfilePageView);
    public static readonly string IdentityService = nameof (IdentityService);
    public static readonly string ProfileCreate = nameof (ProfileCreate);
    public static readonly string ProfileUpdate = nameof (ProfileUpdate);
    public static readonly string ProfileOptOut = nameof (ProfileOptOut);
    public static readonly string SubscriptionCreate = nameof (SubscriptionCreate);
    public static readonly string PostClientNotification = nameof (PostClientNotification);
    public static readonly string ActiveClientConnection = nameof (ActiveClientConnection);
    public static readonly string TraceAccount = nameof (TraceAccount);
    public static readonly string SendEmail = nameof (SendEmail);
    public static readonly string EmailInteraction = nameof (EmailInteraction);
    public static readonly string ResetAccountTrial = nameof (ResetAccountTrial);
    public static readonly string ExtendAccountTrial = nameof (ExtendAccountTrial);
    public static readonly string UnlinkAccountFromAzureSubscription = nameof (UnlinkAccountFromAzureSubscription);
    public static readonly string AccountSoftDeleted = nameof (AccountSoftDeleted);
    public static readonly string AccountReenabled = nameof (AccountReenabled);
    public static readonly string AccountReenabledWithNewName = nameof (AccountReenabledWithNewName);
    public static readonly string SoftDeleteCleanup = nameof (SoftDeleteCleanup);
    public static readonly string SoftDeleteCleanupSucceeded = nameof (SoftDeleteCleanupSucceeded);
    public static readonly string SoftDeleteCleanupFailed = nameof (SoftDeleteCleanupFailed);
    public static readonly string AccountTenantChange = nameof (AccountTenantChange);
    public static readonly string ProjectPackage = nameof (ProjectPackage);
    public static readonly string ContributionUsage = nameof (ContributionUsage);
    public static readonly string KanbanBoardPayload = nameof (KanbanBoardPayload);
    public static readonly string KanbanBoardWorkItemSource = nameof (KanbanBoardWorkItemSource);
    public static readonly string IdentityAudit = nameof (IdentityAudit);
    public static readonly string GroupAudit = nameof (GroupAudit);
    public static readonly string SileProfileCreationFailure = nameof (SileProfileCreationFailure);
    public static readonly string ExportCSV = nameof (ExportCSV);
    public static readonly string CertificateAuthenticationRequestFailed = nameof (CertificateAuthenticationRequestFailed);
    public static readonly string HttpException = nameof (HttpException);
    public static readonly string AccountFirstRedirection = nameof (AccountFirstRedirection);
    public static readonly string HostedSecretService = nameof (HostedSecretService);
    public static readonly string WorkItemUpdate = nameof (WorkItemUpdate);
    public static readonly string TestPlanWITApi = nameof (TestPlanWITApi);
  }
}
