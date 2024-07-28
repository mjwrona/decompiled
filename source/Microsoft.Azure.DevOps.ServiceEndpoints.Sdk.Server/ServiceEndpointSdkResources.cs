// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointSdkResources
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal static class ServiceEndpointSdkResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ServiceEndpointSdkResources), typeof (ServiceEndpointSdkResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ServiceEndpointSdkResources.s_resMgr;

    private static string Get(string resourceName) => ServiceEndpointSdkResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ServiceEndpointSdkResources.Get(resourceName) : ServiceEndpointSdkResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ServiceEndpointSdkResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ServiceEndpointSdkResources.GetInt(resourceName) : (int) ServiceEndpointSdkResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ServiceEndpointSdkResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ServiceEndpointSdkResources.GetBool(resourceName) : (bool) ServiceEndpointSdkResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ServiceEndpointSdkResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ServiceEndpointSdkResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string EndpointNotFound(object arg0) => ServiceEndpointSdkResources.Format(nameof (EndpointNotFound), arg0);

    public static string EndpointNotFound(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (EndpointNotFound), culture, arg0);

    public static string AzdevAccessTokenCacheKeyLookupResultIsInvalidError() => ServiceEndpointSdkResources.Get(nameof (AzdevAccessTokenCacheKeyLookupResultIsInvalidError));

    public static string AzdevAccessTokenCacheKeyLookupResultIsInvalidError(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (AzdevAccessTokenCacheKeyLookupResultIsInvalidError), culture);

    public static string AzdevAccessTokenKeyNotFoundError() => ServiceEndpointSdkResources.Get(nameof (AzdevAccessTokenKeyNotFoundError));

    public static string AzdevAccessTokenKeyNotFoundError(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (AzdevAccessTokenKeyNotFoundError), culture);

    public static string AzdevAccessTokenCacheKeyLookupResultIsNullError() => ServiceEndpointSdkResources.Get(nameof (AzdevAccessTokenCacheKeyLookupResultIsNullError));

    public static string AzdevAccessTokenCacheKeyLookupResultIsNullError(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (AzdevAccessTokenCacheKeyLookupResultIsNullError), culture);

    public static string AzdevAccessTokenIsNullError() => ServiceEndpointSdkResources.Get(nameof (AzdevAccessTokenIsNullError));

    public static string AzdevAccessTokenIsNullError(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (AzdevAccessTokenIsNullError), culture);

    public static string AzdevIdTokenKeyNotFoundError() => ServiceEndpointSdkResources.Get(nameof (AzdevIdTokenKeyNotFoundError));

    public static string AzdevIdTokenKeyNotFoundError(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (AzdevIdTokenKeyNotFoundError), culture);

    public static string AzdevNonceNotFoundError() => ServiceEndpointSdkResources.Get(nameof (AzdevNonceNotFoundError));

    public static string AzdevNonceNotFoundError(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (AzdevNonceNotFoundError), culture);

    public static string ExceptionMessage() => ServiceEndpointSdkResources.Get(nameof (ExceptionMessage));

    public static string ExceptionMessage(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (ExceptionMessage), culture);

    public static string FailedToGenerateToken(object arg0, object arg1, object arg2) => ServiceEndpointSdkResources.Format(nameof (FailedToGenerateToken), arg0, arg1, arg2);

    public static string FailedToGenerateToken(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Format(nameof (FailedToGenerateToken), culture, arg0, arg1, arg2);
    }

    public static string FailedToObtainToken() => ServiceEndpointSdkResources.Get(nameof (FailedToObtainToken));

    public static string FailedToObtainToken(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (FailedToObtainToken), culture);

    public static string InvalidAzureEndpointAuthorizer(object arg0) => ServiceEndpointSdkResources.Format(nameof (InvalidAzureEndpointAuthorizer), arg0);

    public static string InvalidAzureEndpointAuthorizer(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (InvalidAzureEndpointAuthorizer), culture, arg0);

    public static string InvalidAzureManagementCertificate() => ServiceEndpointSdkResources.Get(nameof (InvalidAzureManagementCertificate));

    public static string InvalidAzureManagementCertificate(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (InvalidAzureManagementCertificate), culture);

    public static string InvalidEndpointAuthorizer(object arg0) => ServiceEndpointSdkResources.Format(nameof (InvalidEndpointAuthorizer), arg0);

    public static string InvalidEndpointAuthorizer(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (InvalidEndpointAuthorizer), culture, arg0);

    public static string InvalidEndpointId(object arg0) => ServiceEndpointSdkResources.Format(nameof (InvalidEndpointId), arg0);

    public static string InvalidEndpointId(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (InvalidEndpointId), culture, arg0);

    public static string InvalidScopeId(object arg0) => ServiceEndpointSdkResources.Format(nameof (InvalidScopeId), arg0);

    public static string InvalidScopeId(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (InvalidScopeId), culture, arg0);

    public static string ResourceUrlNotSupported(object arg0, object arg1) => ServiceEndpointSdkResources.Format(nameof (ResourceUrlNotSupported), arg0, arg1);

    public static string ResourceUrlNotSupported(object arg0, object arg1, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (ResourceUrlNotSupported), culture, arg0, arg1);

    public static string NoAzureCertificate() => ServiceEndpointSdkResources.Get(nameof (NoAzureCertificate));

    public static string NoAzureCertificate(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (NoAzureCertificate), culture);

    public static string NoAzureServicePrincipal() => ServiceEndpointSdkResources.Get(nameof (NoAzureServicePrincipal));

    public static string NoAzureServicePrincipal(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (NoAzureServicePrincipal), culture);

    public static string NoUsernamePassword() => ServiceEndpointSdkResources.Get(nameof (NoUsernamePassword));

    public static string NoUsernamePassword(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (NoUsernamePassword), culture);

    public static string NullSessionToken(object arg0, object arg1) => ServiceEndpointSdkResources.Format(nameof (NullSessionToken), arg0, arg1);

    public static string NullSessionToken(object arg0, object arg1, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (NullSessionToken), culture, arg0, arg1);

    public static string ServiceEndPointNotFound(object arg0) => ServiceEndpointSdkResources.Format(nameof (ServiceEndPointNotFound), arg0);

    public static string ServiceEndPointNotFound(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (ServiceEndPointNotFound), culture, arg0);

    public static string UnableToConnectToTheAzureStackEnvironment() => ServiceEndpointSdkResources.Get(nameof (UnableToConnectToTheAzureStackEnvironment));

    public static string UnableToConnectToTheAzureStackEnvironment(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (UnableToConnectToTheAzureStackEnvironment), culture);

    public static string SpecifiedAzureRMEndpointIsInvalid() => ServiceEndpointSdkResources.Get(nameof (SpecifiedAzureRMEndpointIsInvalid));

    public static string SpecifiedAzureRMEndpointIsInvalid(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (SpecifiedAzureRMEndpointIsInvalid), culture);

    public static string UnableToPopulateAzureStackData() => ServiceEndpointSdkResources.Get(nameof (UnableToPopulateAzureStackData));

    public static string UnableToPopulateAzureStackData(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (UnableToPopulateAzureStackData), culture);

    public static string FailedToObtainAzureStackActiveDirectoryResourceId() => ServiceEndpointSdkResources.Get(nameof (FailedToObtainAzureStackActiveDirectoryResourceId));

    public static string FailedToObtainAzureStackActiveDirectoryResourceId(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (FailedToObtainAzureStackActiveDirectoryResourceId), culture);

    public static string PopulateAzureStackSupportedForAzureRmEndpoints(object arg0, object arg1) => ServiceEndpointSdkResources.Format(nameof (PopulateAzureStackSupportedForAzureRmEndpoints), arg0, arg1);

    public static string PopulateAzureStackSupportedForAzureRmEndpoints(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Format(nameof (PopulateAzureStackSupportedForAzureRmEndpoints), culture, arg0, arg1);
    }

    public static string PopulateAzureStackSupportedForAzureStackeEnvironment() => ServiceEndpointSdkResources.Get(nameof (PopulateAzureStackSupportedForAzureStackeEnvironment));

    public static string PopulateAzureStackSupportedForAzureStackeEnvironment(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (PopulateAzureStackSupportedForAzureStackeEnvironment), culture);

    public static string RequestFailedAsUnableToConnectAzureStack(object arg0, object arg1) => ServiceEndpointSdkResources.Format(nameof (RequestFailedAsUnableToConnectAzureStack), arg0, arg1);

    public static string RequestFailedAsUnableToConnectAzureStack(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Format(nameof (RequestFailedAsUnableToConnectAzureStack), culture, arg0, arg1);
    }

    public static string NoStorageAccountDetails() => ServiceEndpointSdkResources.Get(nameof (NoStorageAccountDetails));

    public static string NoStorageAccountDetails(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (NoStorageAccountDetails), culture);

    public static string AuthenticationSchemeNotFound(object arg0) => ServiceEndpointSdkResources.Format(nameof (AuthenticationSchemeNotFound), arg0);

    public static string AuthenticationSchemeNotFound(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (AuthenticationSchemeNotFound), culture, arg0);

    public static string AuthenticationSchemeNotFoundInServiceEndpoint(object arg0, object arg1) => ServiceEndpointSdkResources.Format(nameof (AuthenticationSchemeNotFoundInServiceEndpoint), arg0, arg1);

    public static string AuthenticationSchemeNotFoundInServiceEndpoint(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Format(nameof (AuthenticationSchemeNotFoundInServiceEndpoint), culture, arg0, arg1);
    }

    public static string ResponseSizeExceeded() => ServiceEndpointSdkResources.Get(nameof (ResponseSizeExceeded));

    public static string ResponseSizeExceeded(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (ResponseSizeExceeded), culture);

    public static string NoStorageAccountName() => ServiceEndpointSdkResources.Get(nameof (NoStorageAccountName));

    public static string NoStorageAccountName(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (NoStorageAccountName), culture);

    public static string NoStorageAccessKeyFound() => ServiceEndpointSdkResources.Get(nameof (NoStorageAccessKeyFound));

    public static string NoStorageAccessKeyFound(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (NoStorageAccessKeyFound), culture);

    public static string NoSubscriptionId() => ServiceEndpointSdkResources.Get(nameof (NoSubscriptionId));

    public static string NoSubscriptionId(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (NoSubscriptionId), culture);

    public static string StorageAccountNotFound() => ServiceEndpointSdkResources.Get(nameof (StorageAccountNotFound));

    public static string StorageAccountNotFound(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (StorageAccountNotFound), culture);

    public static string GetAccessTokenUsingAzdevTokenError() => ServiceEndpointSdkResources.Get(nameof (GetAccessTokenUsingAzdevTokenError));

    public static string GetAccessTokenUsingAzdevTokenError(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (GetAccessTokenUsingAzdevTokenError), culture);

    public static string AuthorizationParameterNotFound(object arg0) => ServiceEndpointSdkResources.Format(nameof (AuthorizationParameterNotFound), arg0);

    public static string AuthorizationParameterNotFound(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (AuthorizationParameterNotFound), culture, arg0);

    public static string AuthorizationTokenFetchFailed(object arg0, object arg1) => ServiceEndpointSdkResources.Format(nameof (AuthorizationTokenFetchFailed), arg0, arg1);

    public static string AuthorizationTokenFetchFailed(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Format(nameof (AuthorizationTokenFetchFailed), culture, arg0, arg1);
    }

    public static string AuthorizationTokenFetchTimeout(object arg0) => ServiceEndpointSdkResources.Format(nameof (AuthorizationTokenFetchTimeout), arg0);

    public static string AuthorizationTokenFetchTimeout(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (AuthorizationTokenFetchTimeout), culture, arg0);

    public static string AccessTokenKeyNotFoundInServiceEndpointAuthorizationParameters() => ServiceEndpointSdkResources.Get(nameof (AccessTokenKeyNotFoundInServiceEndpointAuthorizationParameters));

    public static string AccessTokenKeyNotFoundInServiceEndpointAuthorizationParameters(
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Get(nameof (AccessTokenKeyNotFoundInServiceEndpointAuthorizationParameters), culture);
    }

    public static string ServiceEndpointInvalid() => ServiceEndpointSdkResources.Get(nameof (ServiceEndpointInvalid));

    public static string ServiceEndpointInvalid(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (ServiceEndpointInvalid), culture);

    public static string AzdevAccessTokenStrongBoxLookupKeyIsInvalid() => ServiceEndpointSdkResources.Get(nameof (AzdevAccessTokenStrongBoxLookupKeyIsInvalid));

    public static string AzdevAccessTokenStrongBoxLookupKeyIsInvalid(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (AzdevAccessTokenStrongBoxLookupKeyIsInvalid), culture);

    public static string VstsAccessTokenStrongBoxLookupValueIsInvalid() => ServiceEndpointSdkResources.Get(nameof (VstsAccessTokenStrongBoxLookupValueIsInvalid));

    public static string VstsAccessTokenStrongBoxLookupValueIsInvalid(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (VstsAccessTokenStrongBoxLookupValueIsInvalid), culture);

    public static string AccessTokenStoreInvalid() => ServiceEndpointSdkResources.Get(nameof (AccessTokenStoreInvalid));

    public static string AccessTokenStoreInvalid(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (AccessTokenStoreInvalid), culture);

    public static string CouldNotParseKubeConfig(object arg0) => ServiceEndpointSdkResources.Format(nameof (CouldNotParseKubeConfig), arg0);

    public static string CouldNotParseKubeConfig(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (CouldNotParseKubeConfig), culture, arg0);

    public static string InvalidCertificate(object arg0) => ServiceEndpointSdkResources.Format(nameof (InvalidCertificate), arg0);

    public static string InvalidCertificate(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (InvalidCertificate), culture, arg0);

    public static string NoClusterId() => ServiceEndpointSdkResources.Get(nameof (NoClusterId));

    public static string NoClusterId(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (NoClusterId), culture);

    public static string NoKubeConfig() => ServiceEndpointSdkResources.Get(nameof (NoKubeConfig));

    public static string NoKubeConfig(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (NoKubeConfig), culture);

    public static string NoClientCertOrKey() => ServiceEndpointSdkResources.Get(nameof (NoClientCertOrKey));

    public static string NoClientCertOrKey(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (NoClientCertOrKey), culture);

    public static string InvalidAzureRmEndpointAuthorizer(object arg0) => ServiceEndpointSdkResources.Format(nameof (InvalidAzureRmEndpointAuthorizer), arg0);

    public static string InvalidAzureRmEndpointAuthorizer(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (InvalidAzureRmEndpointAuthorizer), culture, arg0);

    public static string FailedToObtainTokenUsingUserPrincipal() => ServiceEndpointSdkResources.Get(nameof (FailedToObtainTokenUsingUserPrincipal));

    public static string FailedToObtainTokenUsingUserPrincipal(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (FailedToObtainTokenUsingUserPrincipal), culture);

    public static string ContributorGroup() => ServiceEndpointSdkResources.Get(nameof (ContributorGroup));

    public static string ContributorGroup(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (ContributorGroup), culture);

    public static string LibraryItemAccessDeniedForCreate(object arg0) => ServiceEndpointSdkResources.Format(nameof (LibraryItemAccessDeniedForCreate), arg0);

    public static string LibraryItemAccessDeniedForCreate(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (LibraryItemAccessDeniedForCreate), culture, arg0);

    public static string OAuthConfiguration() => ServiceEndpointSdkResources.Get(nameof (OAuthConfiguration));

    public static string OAuthConfiguration(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (OAuthConfiguration), culture);

    public static string AdministratorRole() => ServiceEndpointSdkResources.Get(nameof (AdministratorRole));

    public static string AdministratorRole(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (AdministratorRole), culture);

    public static string CreatorRole() => ServiceEndpointSdkResources.Get(nameof (CreatorRole));

    public static string CreatorRole(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (CreatorRole), culture);

    public static string LibraryAdministratorRoleDescription() => ServiceEndpointSdkResources.Get(nameof (LibraryAdministratorRoleDescription));

    public static string LibraryAdministratorRoleDescription(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (LibraryAdministratorRoleDescription), culture);

    public static string LibraryCreatorRoleDescription() => ServiceEndpointSdkResources.Get(nameof (LibraryCreatorRoleDescription));

    public static string LibraryCreatorRoleDescription(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (LibraryCreatorRoleDescription), culture);

    public static string LibraryReaderRoleDescription() => ServiceEndpointSdkResources.Get(nameof (LibraryReaderRoleDescription));

    public static string LibraryReaderRoleDescription(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (LibraryReaderRoleDescription), culture);

    public static string LibraryUserRoleDescription() => ServiceEndpointSdkResources.Get(nameof (LibraryUserRoleDescription));

    public static string LibraryUserRoleDescription(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (LibraryUserRoleDescription), culture);

    public static string ReaderRole() => ServiceEndpointSdkResources.Get(nameof (ReaderRole));

    public static string ReaderRole(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (ReaderRole), culture);

    public static string UserRole() => ServiceEndpointSdkResources.Get(nameof (UserRole));

    public static string UserRole(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (UserRole), culture);

    public static string OAuthConfigurationAccessDeniedForCreate() => ServiceEndpointSdkResources.Get(nameof (OAuthConfigurationAccessDeniedForCreate));

    public static string OAuthConfigurationAccessDeniedForCreate(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (OAuthConfigurationAccessDeniedForCreate), culture);

    public static string ProjectBuildAdminAccountName() => ServiceEndpointSdkResources.Get(nameof (ProjectBuildAdminAccountName));

    public static string ProjectBuildAdminAccountName(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (ProjectBuildAdminAccountName), culture);

    public static string ProjectReleaseAdminAccountName() => ServiceEndpointSdkResources.Get(nameof (ProjectReleaseAdminAccountName));

    public static string ProjectReleaseAdminAccountName(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (ProjectReleaseAdminAccountName), culture);

    public static string ProjectReleaseManagerGroupName() => ServiceEndpointSdkResources.Get(nameof (ProjectReleaseManagerGroupName));

    public static string ProjectReleaseManagerGroupName(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (ProjectReleaseManagerGroupName), culture);

    public static string OAuthRedirectUrlIsInvalidError(object arg0) => ServiceEndpointSdkResources.Format(nameof (OAuthRedirectUrlIsInvalidError), arg0);

    public static string OAuthRedirectUrlIsInvalidError(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (OAuthRedirectUrlIsInvalidError), culture, arg0);

    public static string AbsoluteUriNotAllowed() => ServiceEndpointSdkResources.Get(nameof (AbsoluteUriNotAllowed));

    public static string AbsoluteUriNotAllowed(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (AbsoluteUriNotAllowed), culture);

    public static string BodySizeLimitExceeded(object arg0) => ServiceEndpointSdkResources.Format(nameof (BodySizeLimitExceeded), arg0);

    public static string BodySizeLimitExceeded(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (BodySizeLimitExceeded), culture, arg0);

    public static string EndpointAdministratorsGroup() => ServiceEndpointSdkResources.Get(nameof (EndpointAdministratorsGroup));

    public static string EndpointAdministratorsGroup(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (EndpointAdministratorsGroup), culture);

    public static string EndpointCreatorsGroup() => ServiceEndpointSdkResources.Get(nameof (EndpointCreatorsGroup));

    public static string EndpointCreatorsGroup(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (EndpointCreatorsGroup), culture);

    public static string EndpointCreatorsGroupDescription() => ServiceEndpointSdkResources.Get(nameof (EndpointCreatorsGroupDescription));

    public static string EndpointCreatorsGroupDescription(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (EndpointCreatorsGroupDescription), culture);

    public static string EndpontGroupCreationFailed(object arg0) => ServiceEndpointSdkResources.Format(nameof (EndpontGroupCreationFailed), arg0);

    public static string EndpontGroupCreationFailed(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (EndpontGroupCreationFailed), culture, arg0);

    public static string InsufficientPermissions() => ServiceEndpointSdkResources.Get(nameof (InsufficientPermissions));

    public static string InsufficientPermissions(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (InsufficientPermissions), culture);

    public static string OperationNotAllowedForNonServicePrincipal() => ServiceEndpointSdkResources.Get(nameof (OperationNotAllowedForNonServicePrincipal));

    public static string OperationNotAllowedForNonServicePrincipal(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (OperationNotAllowedForNonServicePrincipal), culture);

    public static string ProjectLevelEndpointAdministratorsGroupDescription() => ServiceEndpointSdkResources.Get(nameof (ProjectLevelEndpointAdministratorsGroupDescription));

    public static string ProjectLevelEndpointAdministratorsGroupDescription(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (ProjectLevelEndpointAdministratorsGroupDescription), culture);

    public static string ServiceEndpointRequestFailedMessage(object arg0, object arg1, object arg2) => ServiceEndpointSdkResources.Format(nameof (ServiceEndpointRequestFailedMessage), arg0, arg1, arg2);

    public static string ServiceEndpointRequestFailedMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Format(nameof (ServiceEndpointRequestFailedMessage), culture, arg0, arg1, arg2);
    }

    public static string UntrustedHost(object arg0) => ServiceEndpointSdkResources.Format(nameof (UntrustedHost), arg0);

    public static string UntrustedHost(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (UntrustedHost), culture, arg0);

    public static string KubernetesNoUserDataFound(object arg0) => ServiceEndpointSdkResources.Format(nameof (KubernetesNoUserDataFound), arg0);

    public static string KubernetesNoUserDataFound(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (KubernetesNoUserDataFound), culture, arg0);

    public static string KubernetesInvalidKubeconfig(object arg0) => ServiceEndpointSdkResources.Format(nameof (KubernetesInvalidKubeconfig), arg0);

    public static string KubernetesInvalidKubeconfig(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (KubernetesInvalidKubeconfig), culture, arg0);

    public static string KubernetesKubeconfigFieldNotFound(object arg0) => ServiceEndpointSdkResources.Format(nameof (KubernetesKubeconfigFieldNotFound), arg0);

    public static string KubernetesKubeconfigFieldNotFound(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (KubernetesKubeconfigFieldNotFound), culture, arg0);

    public static string ProjectCollectionAdministratorsGroup() => ServiceEndpointSdkResources.Get(nameof (ProjectCollectionAdministratorsGroup));

    public static string ProjectCollectionAdministratorsGroup(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (ProjectCollectionAdministratorsGroup), culture);

    public static string ErrorInSplitAuthorizationParameters(object arg0, object arg1, object arg2) => ServiceEndpointSdkResources.Format(nameof (ErrorInSplitAuthorizationParameters), arg0, arg1, arg2);

    public static string ErrorInSplitAuthorizationParameters(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Format(nameof (ErrorInSplitAuthorizationParameters), culture, arg0, arg1, arg2);
    }

    public static string KubernetesKubeconfigNoFieldPresent(object arg0) => ServiceEndpointSdkResources.Format(nameof (KubernetesKubeconfigNoFieldPresent), arg0);

    public static string KubernetesKubeconfigNoFieldPresent(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (KubernetesKubeconfigNoFieldPresent), culture, arg0);

    public static string KubernetesNoAuthorizationParameters() => ServiceEndpointSdkResources.Get(nameof (KubernetesNoAuthorizationParameters));

    public static string KubernetesNoAuthorizationParameters(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (KubernetesNoAuthorizationParameters), culture);

    public static string KubernetesKubeconfigErrorWhileExtractingNode(object arg0) => ServiceEndpointSdkResources.Format(nameof (KubernetesKubeconfigErrorWhileExtractingNode), arg0);

    public static string KubernetesKubeconfigErrorWhileExtractingNode(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Format(nameof (KubernetesKubeconfigErrorWhileExtractingNode), culture, arg0);
    }

    public static string KubernetesCannotFetchClusterCredentialsMissingClusterId() => ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingClusterId));

    public static string KubernetesCannotFetchClusterCredentialsMissingClusterId(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingClusterId), culture);

    public static string KubernetesCannotFetchClusterCredentialsMissingSubscriptionId() => ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingSubscriptionId));

    public static string KubernetesCannotFetchClusterCredentialsMissingSubscriptionId(
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingSubscriptionId), culture);
    }

    public static string KubernetesCannotFetchClusterCredentialsMissingSubscriptionName() => ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingSubscriptionName));

    public static string KubernetesCannotFetchClusterCredentialsMissingSubscriptionName(
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingSubscriptionName), culture);
    }

    public static string KubernetesCannotFetchClusterCredentialsMissingEnvironment() => ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingEnvironment));

    public static string KubernetesCannotFetchClusterCredentialsMissingEnvironment(
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingEnvironment), culture);
    }

    public static string KubernetesCannotFetchClusterCredentialsMissingTenantId() => ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingTenantId));

    public static string KubernetesCannotFetchClusterCredentialsMissingTenantId(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingTenantId), culture);

    public static string KubernetesCannotFetchClusterCredentialsMissingAzureSpnId() => ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingAzureSpnId));

    public static string KubernetesCannotFetchClusterCredentialsMissingAzureSpnId(
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingAzureSpnId), culture);
    }

    public static string KubernetesCannotFetchClusterCredentialsMissingAzureSpnKey() => ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingAzureSpnKey));

    public static string KubernetesCannotFetchClusterCredentialsMissingAzureSpnKey(
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingAzureSpnKey), culture);
    }

    public static string KubernetesCannotFetchClusterCredentialsMissingCloudUrl() => ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingCloudUrl));

    public static string KubernetesCannotFetchClusterCredentialsMissingCloudUrl(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingCloudUrl), culture);

    public static string KubernetesCannotFetchClusterCredentialsMissingSpnCreationMethod() => ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingSpnCreationMethod));

    public static string KubernetesCannotFetchClusterCredentialsMissingSpnCreationMethod(
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Get(nameof (KubernetesCannotFetchClusterCredentialsMissingSpnCreationMethod), culture);
    }

    public static string KubernetesErrorFetchingClusterCredentialsServerError(object arg0) => ServiceEndpointSdkResources.Format(nameof (KubernetesErrorFetchingClusterCredentialsServerError), arg0);

    public static string KubernetesErrorFetchingClusterCredentialsServerError(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Format(nameof (KubernetesErrorFetchingClusterCredentialsServerError), culture, arg0);
    }

    public static string KubernetesErrorFetchingClusterCredentialsInvalidResponse() => ServiceEndpointSdkResources.Get(nameof (KubernetesErrorFetchingClusterCredentialsInvalidResponse));

    public static string KubernetesErrorFetchingClusterCredentialsInvalidResponse(
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Get(nameof (KubernetesErrorFetchingClusterCredentialsInvalidResponse), culture);
    }

    public static string KubernetesErrorFetchingClusterCredentialsEmptyKubeConfig() => ServiceEndpointSdkResources.Get(nameof (KubernetesErrorFetchingClusterCredentialsEmptyKubeConfig));

    public static string KubernetesErrorFetchingClusterCredentialsEmptyKubeConfig(
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Get(nameof (KubernetesErrorFetchingClusterCredentialsEmptyKubeConfig), culture);
    }

    public static string EndpointAccessDeniedForUseOperation() => ServiceEndpointSdkResources.Get(nameof (EndpointAccessDeniedForUseOperation));

    public static string EndpointAccessDeniedForUseOperation(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (EndpointAccessDeniedForUseOperation), culture);

    public static string ServiceEndpointRequestFailedGenericMessage(object arg0, object arg1) => ServiceEndpointSdkResources.Format(nameof (ServiceEndpointRequestFailedGenericMessage), arg0, arg1);

    public static string ServiceEndpointRequestFailedGenericMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Format(nameof (ServiceEndpointRequestFailedGenericMessage), culture, arg0, arg1);
    }

    public static string ServiceEndpointRequestFailedMessageWithoutPrefix(object arg0, object arg1) => ServiceEndpointSdkResources.Format(nameof (ServiceEndpointRequestFailedMessageWithoutPrefix), arg0, arg1);

    public static string ServiceEndpointRequestFailedMessageWithoutPrefix(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Format(nameof (ServiceEndpointRequestFailedMessageWithoutPrefix), culture, arg0, arg1);
    }

    public static string InvalidUrlLoopback() => ServiceEndpointSdkResources.Get(nameof (InvalidUrlLoopback));

    public static string InvalidUrlLoopback(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (InvalidUrlLoopback), culture);

    public static string InvalidUrlSpecialPurposeFormat(object arg0) => ServiceEndpointSdkResources.Format(nameof (InvalidUrlSpecialPurposeFormat), arg0);

    public static string InvalidUrlSpecialPurposeFormat(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (InvalidUrlSpecialPurposeFormat), culture, arg0);

    public static string KubeConfigParseError(object arg0) => ServiceEndpointSdkResources.Format(nameof (KubeConfigParseError), arg0);

    public static string KubeConfigParseError(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (KubeConfigParseError), culture, arg0);

    public static string Error_CallbackStateFormatIncorrect(object arg0) => ServiceEndpointSdkResources.Format(nameof (Error_CallbackStateFormatIncorrect), arg0);

    public static string Error_CallbackStateFormatIncorrect(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (Error_CallbackStateFormatIncorrect), culture, arg0);

    public static string UriParseError() => ServiceEndpointSdkResources.Get(nameof (UriParseError));

    public static string UriParseError(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (UriParseError), culture);

    public static string NoAzureOidcFederation() => ServiceEndpointSdkResources.Get(nameof (NoAzureOidcFederation));

    public static string NoAzureOidcFederation(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (NoAzureOidcFederation), culture);

    public static string FailedToGenerateOidcToken(object arg0) => ServiceEndpointSdkResources.Format(nameof (FailedToGenerateOidcToken), arg0);

    public static string FailedToGenerateOidcToken(object arg0, CultureInfo culture) => ServiceEndpointSdkResources.Format(nameof (FailedToGenerateOidcToken), culture, arg0);

    public static string WorkloadIdentityFederationDisabled() => ServiceEndpointSdkResources.Get(nameof (WorkloadIdentityFederationDisabled));

    public static string WorkloadIdentityFederationDisabled(CultureInfo culture) => ServiceEndpointSdkResources.Get(nameof (WorkloadIdentityFederationDisabled), culture);

    public static string FailedToSignIntoAadWithWorkloadIdentityFederation(object arg0) => ServiceEndpointSdkResources.Format(nameof (FailedToSignIntoAadWithWorkloadIdentityFederation), arg0);

    public static string FailedToSignIntoAadWithWorkloadIdentityFederation(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointSdkResources.Format(nameof (FailedToSignIntoAadWithWorkloadIdentityFederation), culture, arg0);
    }
  }
}
