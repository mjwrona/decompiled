// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.TraceConstants
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public static class TraceConstants
  {
    public const string Area = "ServiceEndpoints";
    public const string ResourceService = "ResourceService";
    public const string ServiceEndpoints = "ServiceEndpoints";
    public const string WebApiProxy = "WebApiProxy";
    public const string ServiceLayer = "Service";
    public const string JobLayer = "Job";
    public const int UpdateDuplicateServiceEndpointNameTraceEnter = 20029050;
    public const int UpdateDuplicateServiceEndpointNameThrewException = 20029051;
    public const int UpdateDuplicateServiceEndpointNameTraceLeave = 20029099;
    public const int QueueJobForAutoSpnDelete = 34000801;
    public const int FailedToUpdateAuthorizationDataForEndpoint = 34000802;
    public const int AccessControlListsOrphaned = 34000803;
    public const int RevokeOAuthAccessTokenFailedException = 34000804;
    public const int QueueJobForAutoSpnCreate = 34000805;
    public const int QueueJobForAutoSpnUpdate = 34000806;
    public const int AutoSpnAadAppCreated = 34000807;
    public const int AutoSpnRoleUnssignedForAadApp = 34000808;
    public const int AutoSpnUpdateCredsForAadApp = 34000809;
    public const int HandleHttpResponseError = 34000810;
    public const int AutoSpnWaitingToBeReady = 34000811;
    public const int AutoSpnTryingCreatingAadApp = 34000812;
    public const int AutoSpnAddingAadAppToGraphConnection = 34000813;
    public const int ManualSpnGetAadApp = 34000814;
    public const int AadAppDeletionOperationFailed = 34000815;
    public const int SetSpnAzurePermissions = 34000816;
    public const int RemoveSpnAzurePermissions = 34000817;
    public const int LoadServiceEndpointTypeFailed = 34000818;
    public const int CannotAddMembersToEndpointAdmin = 34000819;
    public const int AddingProjectAdministratorToEndpointCreator = 34000820;
    public const int StrongBoxItemNotFound = 34000821;
    public const int InvalidEndpointUrl = 34000822;
    public const int MissingOAuthConfigurationStrongBoxDrawer = 34000823;
    public const int MissingClientSecretOAuthConfiguration = 34000824;
    public const int GetAzureSubscriptionsFailed = 34000825;
    public const int GetAzureManagementGroupsFailed = 34000826;
    public const int FailedToAcquireAccessTokenUsingVstsToken = 34000827;
    public const int FailedToRefreshAccessTokenUsing = 34000828;
    public const int AutoServicePrincipalCreated = 34000829;
    public const int CreateAutoServicePrincipalOperationException = 34000830;
    public const int UpdateEndpointAfterCreateOfSpn = 34000831;
    public const int AutoServicePrincipalUpdated = 34000832;
    public const int UpdateAutoServicePrincipalOperationException = 34000833;
    public const int AutoServicePrincipalDeleted = 34000834;
    public const int DeleteAutoServicePrincipalOperationException = 34000835;
    public const int EndpointNotFound = 34000836;
    public const int ReadEndpointOperationFailed = 34000837;
    public const int AutoServicePrincipalEndpointJobOperationType = 34000838;
    public const int CompleteSetRoleAssignments = 34000839;
    public const int CompleteRemoveRoleAssignments = 34000840;
    public const int SlowCall = 34000841;
    public const int QueueJobForKubernetesServiceAccountSetup = 34000842;
    public const int KubernetesServiceAccountSetupCompleted = 34000843;
    public const int KubernetesServiceAccountSetupOperationException = 34000844;
    public const int UpdateEndpointAfterKubernetesServiceAccountSetup = 34000845;
    public const int KubernetesEndpointJobOperationType = 34000846;
    public const int StrongBoxInvalidContent = 34000847;
    public const int RefreshOAuth2Token = 34000848;
    public const int DeletePermissionsOnResourceDeletion = 34000849;
    public const int GetServiceEndpointExecutionRecords = 34000850;
    public const int RefreshInstallationTokenCache = 34000851;
    public const int PublishProfileFetched = 34000852;
    public const int PublishProfileFetchOperationException = 34000853;
    public const int UpdateEndpointAfterPublishProfileFetched = 34000854;
    public const int RefreshOAuth2TokenException = 34000855;
    public const int GetServiceEndpointSharingDetailsFailed = 34000856;
    public const int AutoSpnTryingAddingApplicationPassword = 34000857;
    public const int AutoSpnTryingRemoveApplicationPassword = 34000858;
    public const int AadServicePrincipalDeletionOperationFailed = 34000859;
    public const int GetServicePrincipalByAppIdFailed = 34000860;
    public const int AutoSpnTryAddingFederation = 34000861;
    public const int AutoSpnTryRemovingFederation = 34000862;
    public const int TryUpgradeSchemeSucceeded = 34000863;
    public const int TryUpgradeSchemeFailed = 34000864;
    public const int DowngradeSchemeSucceeded = 34000865;
    public const int DowngradeSchemeFailed = 34000866;
    public const int TryIssueTokenUsingWorkloadIdentityFederationFailed = 34000867;
    public const int UpgradeToWorkloadIdentityFederationJobAttemptSucceeded = 34000868;
    public const int TryUpgradeToWorkloadIdentityFederationJobAttemptFailed = 34000869;
    public const int TryUpgradeToWorkloadIdentityFederationJobAttemptsExhaused = 34000870;
    public const int TryUpgradeToWorkloadIdentityFederationJobUnknownError = 34000871;
    public const int TryUpgradeToWorkloadIdentityFederationJobSecretsDeleted = 34000872;
    public const int TryUpgradeToWorkloadIdentityFederationJobFailedToDeleteSecrets = 34000873;
    public const int TryUpgradeToWorkloadIdentityFederationNoLongerConverting = 34000874;
    public const int CheckAuthSchemePostMigration = 34001100;
    public const int PlatformOAuthConfigurationServiceStart = 34000900;
    public static int InternalAuthConfigurationStore = 34000901;
    public static int InternalOAuthConfigurationSecretReadInfo = 34000902;
    public const int GitHubInstallationAccessTokenRefreshStart = 34001200;
    public const int InvalidRefreshAuthenticationParameters = 34001201;
    public const int EmptyGitHubInstallationAccessToken = 34001202;
    public const int AuthDataRefreshForServicePrincipalCaller = 34001203;
    public const int ConfidentialParametersClearing = 34001204;
    public const int EndpointDataFilteringWithNoDetails = 34001205;
    public const int NoEndpointDetailsForNonServicePrincipals = 34001206;
    public const int NoEndpointDetailsForNonServicePrincipals2 = 34001207;
    public const int EndpointDataFilteringRequestedPermissions = 34001208;
    public const int EndpointDataFilteringShallow = 34001209;
    public const int EndpointDataFilteringWithDetails = 34001210;
    public const int EndpointDataFilteringPermissionsOverview = 34001211;
    public const int GitHubInstallationAccessTokenRefreshFinish = 34001212;
  }
}
