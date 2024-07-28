// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.TraceConstants
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal static class TraceConstants
  {
    public const string Area = "ServiceEndpoints";
    public const string CustomerIntelligenceHelper = "CustomerIntelligenceHelper";
    public const string WebApiProxy = "WebApiProxy";
    public const string ServiceEndpoints = "ServiceEndpoints";
    public const string AzureKeyVault = "AzureKeyVault";
    public const string AzureAccessTokenHelperSdk = "AzureAccessTokenHelperSdk";
    public const string AadAuthentication = "AadAuthentication";
    public const string KubernetesAuthorizer = "KubernetesAuthorizer";
    public const int IssueSessionTokenFailed = 10015005;
    public const int IssuesessionTokenNullResult = 10015006;
    public const int RefreshFixedTaskCache = 10015141;
    public const int InvalidatingTaskDefinitionCache = 10015144;
    public const int PopulatingTaskDefinitionCache = 10015145;
    public const int CustomerIntelligence = 10016107;
    public const int GetAccessTokenEmptyUsingDefault = 34000201;
    public const int AcquiringUserAccessToken = 34000202;
    public const int FailedToAcquireAccessTokenUsingVstsToken = 34000203;
    public const int AzureStackDependencyDataFailure = 34000204;
    public const int TryGetAccessTokenFromStrongBoxFailed = 34000205;
    public const int TryGetAccessTokenFromPropertyCacheThrewException = 34000206;
    public const int TryGetAccessTokenFromPropertyCacheFailed = 34000207;
    public const int TryGetIdTokenFromPropertyCacheFailed = 34000208;
    public const int TryGetNonceFromPropertyCacheFailed = 34000209;
    public const int TryDeleteAccessTokenFromPropertyCacheThrewException = 34000210;
    public const int CreateOAuthRequestFailed = 34000211;
    public const int CannotAddMembersToEndpointAdmin = 34000212;
    public const int AddingProjectAdministratorToEndpointCreator = 34000213;
    public const int CompleteSetRoleAssignments = 34000214;
    public const int CompleteRemoveRoleAssignments = 34000215;
    public const int ServiceEndpointQueryFailed = 34000216;
    public const int KuberentesAuthorizerTimer = 34000217;
    public const int KuberentesAuthorization = 34000218;
    public const int AddProjectAdminToCollectionAdminForVariableGroup = 34000219;
    public const int ServiceEndpointSecurityMissmatch = 34000220;
    public const int FailedToAcquireAccessTokenUsingVstsTokenMsal = 34000221;
    public const int AcquiredAccessTokenUsingVstsTokenMsal = 34000222;
    public const int AcquiredUserAccessTokenUserRefreshToken = 34000223;
    public const int FailedToAcquireAccessTokenUsingRefreshTokenMsal = 34000224;
    public const int AcquiringServiceConnectionOidcToken = 34000225;
    public const int UsingFirstServiceEndpointReferenceToConstructIdTokenSubjectInSharedServiceConnection = 34000226;
    public const int ServiceEndpointJwtSeriazlizerComparison = 34000227;
    public const int AccessTokenStoredInSpsPropertyCache = 1048670;
    public const int AccessTokenNotStoredInSpsPropertyCache = 1048671;
  }
}
