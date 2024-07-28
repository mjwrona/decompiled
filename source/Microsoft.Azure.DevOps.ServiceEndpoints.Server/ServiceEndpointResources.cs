// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.ServiceEndpointResources
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  internal static class ServiceEndpointResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ServiceEndpointResources), typeof (ServiceEndpointResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ServiceEndpointResources.s_resMgr;

    private static string Get(string resourceName) => ServiceEndpointResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ServiceEndpointResources.Get(resourceName) : ServiceEndpointResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ServiceEndpointResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ServiceEndpointResources.GetInt(resourceName) : (int) ServiceEndpointResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ServiceEndpointResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ServiceEndpointResources.GetBool(resourceName) : (bool) ServiceEndpointResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ServiceEndpointResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ServiceEndpointResources.Get(resourceName, culture);
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

    public static string AbsoluteUriNotAllowed() => ServiceEndpointResources.Get(nameof (AbsoluteUriNotAllowed));

    public static string AbsoluteUriNotAllowed(CultureInfo culture) => ServiceEndpointResources.Get(nameof (AbsoluteUriNotAllowed), culture);

    public static string AdministratorRole() => ServiceEndpointResources.Get(nameof (AdministratorRole));

    public static string AdministratorRole(CultureInfo culture) => ServiceEndpointResources.Get(nameof (AdministratorRole), culture);

    public static string AuthSchemeForOverrideNotFound() => ServiceEndpointResources.Get(nameof (AuthSchemeForOverrideNotFound));

    public static string AuthSchemeForOverrideNotFound(CultureInfo culture) => ServiceEndpointResources.Get(nameof (AuthSchemeForOverrideNotFound), culture);

    public static string AzureServicePrincipalIsNotReady(object arg0, object arg1, object arg2) => ServiceEndpointResources.Format(nameof (AzureServicePrincipalIsNotReady), arg0, arg1, arg2);

    public static string AzureServicePrincipalIsNotReady(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (AzureServicePrincipalIsNotReady), culture, arg0, arg1, arg2);
    }

    public static string BitbucketNotRegisteredError() => ServiceEndpointResources.Get(nameof (BitbucketNotRegisteredError));

    public static string BitbucketNotRegisteredError(CultureInfo culture) => ServiceEndpointResources.Get(nameof (BitbucketNotRegisteredError), culture);

    public static string BitbucketRefreshTokenError() => ServiceEndpointResources.Get(nameof (BitbucketRefreshTokenError));

    public static string BitbucketRefreshTokenError(CultureInfo culture) => ServiceEndpointResources.Get(nameof (BitbucketRefreshTokenError), culture);

    public static string BitbucketNullRefreshTokenError() => ServiceEndpointResources.Get(nameof (BitbucketNullRefreshTokenError));

    public static string BitbucketNullRefreshTokenError(CultureInfo culture) => ServiceEndpointResources.Get(nameof (BitbucketNullRefreshTokenError), culture);

    public static string BodyNotExpectedInRequest(object arg0) => ServiceEndpointResources.Format(nameof (BodyNotExpectedInRequest), arg0);

    public static string BodyNotExpectedInRequest(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (BodyNotExpectedInRequest), culture, arg0);

    public static string BodySizeLimitExceeded(object arg0) => ServiceEndpointResources.Format(nameof (BodySizeLimitExceeded), arg0);

    public static string BodySizeLimitExceeded(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (BodySizeLimitExceeded), culture, arg0);

    public static string CallToUntrustedHostNotAllowed(object arg0) => ServiceEndpointResources.Format(nameof (CallToUntrustedHostNotAllowed), arg0);

    public static string CallToUntrustedHostNotAllowed(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (CallToUntrustedHostNotAllowed), culture, arg0);

    public static string CollectionEndpointInvalidScheme() => ServiceEndpointResources.Get(nameof (CollectionEndpointInvalidScheme));

    public static string CollectionEndpointInvalidScheme(CultureInfo culture) => ServiceEndpointResources.Get(nameof (CollectionEndpointInvalidScheme), culture);

    public static string ContributorGroup() => ServiceEndpointResources.Get(nameof (ContributorGroup));

    public static string ContributorGroup(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ContributorGroup), culture);

    public static string CouldNotCreateAADapp(object arg0) => ServiceEndpointResources.Format(nameof (CouldNotCreateAADapp), arg0);

    public static string CouldNotCreateAADapp(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (CouldNotCreateAADapp), culture, arg0);

    public static string CouldNotFindServicePrincipal(object arg0) => ServiceEndpointResources.Format(nameof (CouldNotFindServicePrincipal), arg0);

    public static string CouldNotFindServicePrincipal(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (CouldNotFindServicePrincipal), culture, arg0);

    public static string CreatorRole() => ServiceEndpointResources.Get(nameof (CreatorRole));

    public static string CreatorRole(CultureInfo culture) => ServiceEndpointResources.Get(nameof (CreatorRole), culture);

    public static string DatasourceAuthSchemeOverridesDoNotMatchAuthSchemeInputs() => ServiceEndpointResources.Get(nameof (DatasourceAuthSchemeOverridesDoNotMatchAuthSchemeInputs));

    public static string DatasourceAuthSchemeOverridesDoNotMatchAuthSchemeInputs(CultureInfo culture) => ServiceEndpointResources.Get(nameof (DatasourceAuthSchemeOverridesDoNotMatchAuthSchemeInputs), culture);

    public static string DataSourceDetailsMissingError() => ServiceEndpointResources.Get(nameof (DataSourceDetailsMissingError));

    public static string DataSourceDetailsMissingError(CultureInfo culture) => ServiceEndpointResources.Get(nameof (DataSourceDetailsMissingError), culture);

    public static string DataSourceNameAndUrlBothMentionedError() => ServiceEndpointResources.Get(nameof (DataSourceNameAndUrlBothMentionedError));

    public static string DataSourceNameAndUrlBothMentionedError(CultureInfo culture) => ServiceEndpointResources.Get(nameof (DataSourceNameAndUrlBothMentionedError), culture);

    public static string DataSourceNameAndUrlBothMissingError() => ServiceEndpointResources.Get(nameof (DataSourceNameAndUrlBothMissingError));

    public static string DataSourceNameAndUrlBothMissingError(CultureInfo culture) => ServiceEndpointResources.Get(nameof (DataSourceNameAndUrlBothMissingError), culture);

    public static string DataSourceNotFound(object arg0) => ServiceEndpointResources.Format(nameof (DataSourceNotFound), arg0);

    public static string DataSourceNotFound(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (DataSourceNotFound), culture, arg0);

    public static string EndpointAccessDeniedForAdminOperation() => ServiceEndpointResources.Get(nameof (EndpointAccessDeniedForAdminOperation));

    public static string EndpointAccessDeniedForAdminOperation(CultureInfo culture) => ServiceEndpointResources.Get(nameof (EndpointAccessDeniedForAdminOperation), culture);

    public static string EndpointAccessDeniedForUseOperation() => ServiceEndpointResources.Get(nameof (EndpointAccessDeniedForUseOperation));

    public static string EndpointAccessDeniedForUseOperation(CultureInfo culture) => ServiceEndpointResources.Get(nameof (EndpointAccessDeniedForUseOperation), culture);

    public static string EndpointAdministratorsGroup() => ServiceEndpointResources.Get(nameof (EndpointAdministratorsGroup));

    public static string EndpointAdministratorsGroup(CultureInfo culture) => ServiceEndpointResources.Get(nameof (EndpointAdministratorsGroup), culture);

    public static string EndpointAdministratorsRoleDescription() => ServiceEndpointResources.Get(nameof (EndpointAdministratorsRoleDescription));

    public static string EndpointAdministratorsRoleDescription(CultureInfo culture) => ServiceEndpointResources.Get(nameof (EndpointAdministratorsRoleDescription), culture);

    public static string EndpointCreatorsGroup() => ServiceEndpointResources.Get(nameof (EndpointCreatorsGroup));

    public static string EndpointCreatorsGroup(CultureInfo culture) => ServiceEndpointResources.Get(nameof (EndpointCreatorsGroup), culture);

    public static string EndpointCreatorsGroupDescription() => ServiceEndpointResources.Get(nameof (EndpointCreatorsGroupDescription));

    public static string EndpointCreatorsGroupDescription(CultureInfo culture) => ServiceEndpointResources.Get(nameof (EndpointCreatorsGroupDescription), culture);

    public static string EndpointFieldNotExpectedForSpnAutoCreateEndpoint(object arg0) => ServiceEndpointResources.Format(nameof (EndpointFieldNotExpectedForSpnAutoCreateEndpoint), arg0);

    public static string EndpointFieldNotExpectedForSpnAutoCreateEndpoint(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (EndpointFieldNotExpectedForSpnAutoCreateEndpoint), culture, arg0);
    }

    public static string EndpointFieldNotExpectedForSpnManualCreateEndpoint(object arg0) => ServiceEndpointResources.Format(nameof (EndpointFieldNotExpectedForSpnManualCreateEndpoint), arg0);

    public static string EndpointFieldNotExpectedForSpnManualCreateEndpoint(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (EndpointFieldNotExpectedForSpnManualCreateEndpoint), culture, arg0);
    }

    public static string EndpointFieldNotSpecified() => ServiceEndpointResources.Get(nameof (EndpointFieldNotSpecified));

    public static string EndpointFieldNotSpecified(CultureInfo culture) => ServiceEndpointResources.Get(nameof (EndpointFieldNotSpecified), culture);

    public static string EndpointIdsIsEmptyError() => ServiceEndpointResources.Get(nameof (EndpointIdsIsEmptyError));

    public static string EndpointIdsIsEmptyError(CultureInfo culture) => ServiceEndpointResources.Get(nameof (EndpointIdsIsEmptyError), culture);

    public static string EndpointNotFound(object arg0) => ServiceEndpointResources.Format(nameof (EndpointNotFound), arg0);

    public static string EndpointNotFound(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (EndpointNotFound), culture, arg0);

    public static string EndpointOperationNotSupported(object arg0, object arg1) => ServiceEndpointResources.Format(nameof (EndpointOperationNotSupported), arg0, arg1);

    public static string EndpointOperationNotSupported(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (EndpointOperationNotSupported), culture, arg0, arg1);
    }

    public static string EndpointTypeCannotBeUpdated(object arg0) => ServiceEndpointResources.Format(nameof (EndpointTypeCannotBeUpdated), arg0);

    public static string EndpointTypeCannotBeUpdated(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (EndpointTypeCannotBeUpdated), culture, arg0);

    public static string EndpointUserRoleDescription() => ServiceEndpointResources.Get(nameof (EndpointUserRoleDescription));

    public static string EndpointUserRoleDescription(CultureInfo culture) => ServiceEndpointResources.Get(nameof (EndpointUserRoleDescription), culture);

    public static string EndpontGroupCreationFailed(object arg0) => ServiceEndpointResources.Format(nameof (EndpontGroupCreationFailed), arg0);

    public static string EndpontGroupCreationFailed(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (EndpontGroupCreationFailed), culture, arg0);

    public static string ExpectedValueForServicePrincipalIdOrSetCreationModeAutomatic() => ServiceEndpointResources.Get(nameof (ExpectedValueForServicePrincipalIdOrSetCreationModeAutomatic));

    public static string ExpectedValueForServicePrincipalIdOrSetCreationModeAutomatic(
      CultureInfo culture)
    {
      return ServiceEndpointResources.Get(nameof (ExpectedValueForServicePrincipalIdOrSetCreationModeAutomatic), culture);
    }

    public static string FailedToRemoveAzurePermissionError(object arg0, object arg1, object arg2) => ServiceEndpointResources.Format(nameof (FailedToRemoveAzurePermissionError), arg0, arg1, arg2);

    public static string FailedToRemoveAzurePermissionError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (FailedToRemoveAzurePermissionError), culture, arg0, arg1, arg2);
    }

    public static string FailedToRemoveAzurePermissionErrorOnManagementGroup(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceEndpointResources.Format(nameof (FailedToRemoveAzurePermissionErrorOnManagementGroup), arg0, arg1, arg2);
    }

    public static string FailedToRemoveAzurePermissionErrorOnManagementGroup(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (FailedToRemoveAzurePermissionErrorOnManagementGroup), culture, arg0, arg1, arg2);
    }

    public static string FailedToSetAzurePermissionError(object arg0, object arg1, object arg2) => ServiceEndpointResources.Format(nameof (FailedToSetAzurePermissionError), arg0, arg1, arg2);

    public static string FailedToSetAzurePermissionError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (FailedToSetAzurePermissionError), culture, arg0, arg1, arg2);
    }

    public static string FailedToSetAzurePermissionErrorOnManagementGroup(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceEndpointResources.Format(nameof (FailedToSetAzurePermissionErrorOnManagementGroup), arg0, arg1, arg2);
    }

    public static string FailedToSetAzurePermissionErrorOnManagementGroup(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (FailedToSetAzurePermissionErrorOnManagementGroup), culture, arg0, arg1, arg2);
    }

    public static string GetAzureManagementGroupsFailed(object arg0) => ServiceEndpointResources.Format(nameof (GetAzureManagementGroupsFailed), arg0);

    public static string GetAzureManagementGroupsFailed(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (GetAzureManagementGroupsFailed), culture, arg0);

    public static string GetAzureSubscriptionsFailed(object arg0) => ServiceEndpointResources.Format(nameof (GetAzureSubscriptionsFailed), arg0);

    public static string GetAzureSubscriptionsFailed(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (GetAzureSubscriptionsFailed), culture, arg0);

    public static string InvalidAuthenticationScheme(object arg0) => ServiceEndpointResources.Format(nameof (InvalidAuthenticationScheme), arg0);

    public static string InvalidAuthenticationScheme(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (InvalidAuthenticationScheme), culture, arg0);

    public static string InvalidAuthType() => ServiceEndpointResources.Get(nameof (InvalidAuthType));

    public static string InvalidAuthType(CultureInfo culture) => ServiceEndpointResources.Get(nameof (InvalidAuthType), culture);

    public static string InvalidDataSourceBindingBothMentioned() => ServiceEndpointResources.Get(nameof (InvalidDataSourceBindingBothMentioned));

    public static string InvalidDataSourceBindingBothMentioned(CultureInfo culture) => ServiceEndpointResources.Get(nameof (InvalidDataSourceBindingBothMentioned), culture);

    public static string InvalidDataSourceBindingEndpointIdInvalid() => ServiceEndpointResources.Get(nameof (InvalidDataSourceBindingEndpointIdInvalid));

    public static string InvalidDataSourceBindingEndpointIdInvalid(CultureInfo culture) => ServiceEndpointResources.Get(nameof (InvalidDataSourceBindingEndpointIdInvalid), culture);

    public static string InvalidEndpointInput(object arg0) => ServiceEndpointResources.Format(nameof (InvalidEndpointInput), arg0);

    public static string InvalidEndpointInput(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (InvalidEndpointInput), culture, arg0);

    public static string InvalidEndpointType(object arg0) => ServiceEndpointResources.Format(nameof (InvalidEndpointType), arg0);

    public static string InvalidEndpointType(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (InvalidEndpointType), culture, arg0);

    public static string InvalidEnvironmentProvided(object arg0) => ServiceEndpointResources.Format(nameof (InvalidEnvironmentProvided), arg0);

    public static string InvalidEnvironmentProvided(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (InvalidEnvironmentProvided), culture, arg0);

    public static string InvalidMSIEndpointField(object arg0) => ServiceEndpointResources.Format(nameof (InvalidMSIEndpointField), arg0);

    public static string InvalidMSIEndpointField(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (InvalidMSIEndpointField), culture, arg0);

    public static string InvalidScopeId(object arg0) => ServiceEndpointResources.Format(nameof (InvalidScopeId), arg0);

    public static string InvalidScopeId(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (InvalidScopeId), culture, arg0);

    public static string InvalidScopeLevel(object arg0) => ServiceEndpointResources.Format(nameof (InvalidScopeLevel), arg0);

    public static string InvalidScopeLevel(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (InvalidScopeLevel), culture, arg0);

    public static string InvalidSpnOperation(object arg0) => ServiceEndpointResources.Format(nameof (InvalidSpnOperation), arg0);

    public static string InvalidSpnOperation(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (InvalidSpnOperation), culture, arg0);

    public static string InvalidUrl(object arg0) => ServiceEndpointResources.Format(nameof (InvalidUrl), arg0);

    public static string InvalidUrl(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (InvalidUrl), culture, arg0);

    public static string InvalidUrlForProvidedEnvironment(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServiceEndpointResources.Format(nameof (InvalidUrlForProvidedEnvironment), arg0, arg1, arg2, arg3);
    }

    public static string InvalidUrlForProvidedEnvironment(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (InvalidUrlForProvidedEnvironment), culture, arg0, arg1, arg2, arg3);
    }

    public static string KubernetesAuthTypeDoesNotMatchWithAuthScheme(object arg0, object arg1) => ServiceEndpointResources.Format(nameof (KubernetesAuthTypeDoesNotMatchWithAuthScheme), arg0, arg1);

    public static string KubernetesAuthTypeDoesNotMatchWithAuthScheme(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (KubernetesAuthTypeDoesNotMatchWithAuthScheme), culture, arg0, arg1);
    }

    public static string LibraryAdministratorRoleDescription() => ServiceEndpointResources.Get(nameof (LibraryAdministratorRoleDescription));

    public static string LibraryAdministratorRoleDescription(CultureInfo culture) => ServiceEndpointResources.Get(nameof (LibraryAdministratorRoleDescription), culture);

    public static string LibraryCreatorRoleDescription() => ServiceEndpointResources.Get(nameof (LibraryCreatorRoleDescription));

    public static string LibraryCreatorRoleDescription(CultureInfo culture) => ServiceEndpointResources.Get(nameof (LibraryCreatorRoleDescription), culture);

    public static string LibraryItemAccessDeniedForCreate(object arg0) => ServiceEndpointResources.Format(nameof (LibraryItemAccessDeniedForCreate), arg0);

    public static string LibraryItemAccessDeniedForCreate(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (LibraryItemAccessDeniedForCreate), culture, arg0);

    public static string LibraryReaderRoleDescription() => ServiceEndpointResources.Get(nameof (LibraryReaderRoleDescription));

    public static string LibraryReaderRoleDescription(CultureInfo culture) => ServiceEndpointResources.Get(nameof (LibraryReaderRoleDescription), culture);

    public static string LibraryUserRoleDescription() => ServiceEndpointResources.Get(nameof (LibraryUserRoleDescription));

    public static string LibraryUserRoleDescription(CultureInfo culture) => ServiceEndpointResources.Get(nameof (LibraryUserRoleDescription), culture);

    public static string MissingProperty(object arg0) => ServiceEndpointResources.Format(nameof (MissingProperty), arg0);

    public static string MissingProperty(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (MissingProperty), culture, arg0);

    public static string NullAuthorizationNotAllowed() => ServiceEndpointResources.Get(nameof (NullAuthorizationNotAllowed));

    public static string NullAuthorizationNotAllowed(CultureInfo culture) => ServiceEndpointResources.Get(nameof (NullAuthorizationNotAllowed), culture);

    public static string OAuth2NotSupportedForEndpointType(object arg0) => ServiceEndpointResources.Format(nameof (OAuth2NotSupportedForEndpointType), arg0);

    public static string OAuth2NotSupportedForEndpointType(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (OAuth2NotSupportedForEndpointType), culture, arg0);

    public static string OauthArgumentError() => ServiceEndpointResources.Get(nameof (OauthArgumentError));

    public static string OauthArgumentError(CultureInfo culture) => ServiceEndpointResources.Get(nameof (OauthArgumentError), culture);

    public static string OAuthConfiguration() => ServiceEndpointResources.Get(nameof (OAuthConfiguration));

    public static string OAuthConfiguration(CultureInfo culture) => ServiceEndpointResources.Get(nameof (OAuthConfiguration), culture);

    public static string OAuthConfigurationAccessDeniedForAdminOperation() => ServiceEndpointResources.Get(nameof (OAuthConfigurationAccessDeniedForAdminOperation));

    public static string OAuthConfigurationAccessDeniedForAdminOperation(CultureInfo culture) => ServiceEndpointResources.Get(nameof (OAuthConfigurationAccessDeniedForAdminOperation), culture);

    public static string OAuthConfigurationAccessDeniedForView() => ServiceEndpointResources.Get(nameof (OAuthConfigurationAccessDeniedForView));

    public static string OAuthConfigurationAccessDeniedForView(CultureInfo culture) => ServiceEndpointResources.Get(nameof (OAuthConfigurationAccessDeniedForView), culture);

    public static string OAuthConfigurationExist(object arg0) => ServiceEndpointResources.Format(nameof (OAuthConfigurationExist), arg0);

    public static string OAuthConfigurationExist(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (OAuthConfigurationExist), culture, arg0);

    public static string OAuthConfigurationNotFound(object arg0) => ServiceEndpointResources.Format(nameof (OAuthConfigurationNotFound), arg0);

    public static string OAuthConfigurationNotFound(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (OAuthConfigurationNotFound), culture, arg0);

    public static string OAuthConfigurationSecretsNotFound(object arg0) => ServiceEndpointResources.Format(nameof (OAuthConfigurationSecretsNotFound), arg0);

    public static string OAuthConfigurationSecretsNotFound(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (OAuthConfigurationSecretsNotFound), culture, arg0);

    public static string OAuthConfigurationUrlClientDetailsUpdateForbidden() => ServiceEndpointResources.Get(nameof (OAuthConfigurationUrlClientDetailsUpdateForbidden));

    public static string OAuthConfigurationUrlClientDetailsUpdateForbidden(CultureInfo culture) => ServiceEndpointResources.Get(nameof (OAuthConfigurationUrlClientDetailsUpdateForbidden), culture);

    public static string OperationNotAllowedForNonServicePrincipal() => ServiceEndpointResources.Get(nameof (OperationNotAllowedForNonServicePrincipal));

    public static string OperationNotAllowedForNonServicePrincipal(CultureInfo culture) => ServiceEndpointResources.Get(nameof (OperationNotAllowedForNonServicePrincipal), culture);

    public static string PaginationFieldsMissing() => ServiceEndpointResources.Get(nameof (PaginationFieldsMissing));

    public static string PaginationFieldsMissing(CultureInfo culture) => ServiceEndpointResources.Get(nameof (PaginationFieldsMissing), culture);

    public static string PasswordText() => ServiceEndpointResources.Get(nameof (PasswordText));

    public static string PasswordText(CultureInfo culture) => ServiceEndpointResources.Get(nameof (PasswordText), culture);

    public static string ProjectBuildAdminAccountName() => ServiceEndpointResources.Get(nameof (ProjectBuildAdminAccountName));

    public static string ProjectBuildAdminAccountName(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ProjectBuildAdminAccountName), culture);

    public static string ProjectLevelEndpointAdministratorsGroupDescription() => ServiceEndpointResources.Get(nameof (ProjectLevelEndpointAdministratorsGroupDescription));

    public static string ProjectLevelEndpointAdministratorsGroupDescription(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ProjectLevelEndpointAdministratorsGroupDescription), culture);

    public static string ProjectReleaseAdminAccountName() => ServiceEndpointResources.Get(nameof (ProjectReleaseAdminAccountName));

    public static string ProjectReleaseAdminAccountName(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ProjectReleaseAdminAccountName), culture);

    public static string ProjectReleaseManagerGroupName() => ServiceEndpointResources.Get(nameof (ProjectReleaseManagerGroupName));

    public static string ProjectReleaseManagerGroupName(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ProjectReleaseManagerGroupName), culture);

    public static string ReaderRole() => ServiceEndpointResources.Get(nameof (ReaderRole));

    public static string ReaderRole(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ReaderRole), culture);

    public static string ResourceUrlMentionedError() => ServiceEndpointResources.Get(nameof (ResourceUrlMentionedError));

    public static string ResourceUrlMentionedError(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ResourceUrlMentionedError), culture);

    public static string ResponseSizeExceeded(object arg0) => ServiceEndpointResources.Format(nameof (ResponseSizeExceeded), arg0);

    public static string ResponseSizeExceeded(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (ResponseSizeExceeded), culture, arg0);

    public static string ServiceEndpointIdAndDetailsBothMissingError() => ServiceEndpointResources.Get(nameof (ServiceEndpointIdAndDetailsBothMissingError));

    public static string ServiceEndpointIdAndDetailsBothMissingError(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ServiceEndpointIdAndDetailsBothMissingError), culture);

    public static string ServiceEndpointInvalidOwner() => ServiceEndpointResources.Get(nameof (ServiceEndpointInvalidOwner));

    public static string ServiceEndpointInvalidOwner(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ServiceEndpointInvalidOwner), culture);

    public static string ServiceEndPointNotFound(object arg0) => ServiceEndpointResources.Format(nameof (ServiceEndPointNotFound), arg0);

    public static string ServiceEndPointNotFound(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (ServiceEndPointNotFound), culture, arg0);

    public static string ServiceEndpointRequestVerbUnsupported(object arg0) => ServiceEndpointResources.Format(nameof (ServiceEndpointRequestVerbUnsupported), arg0);

    public static string ServiceEndpointRequestVerbUnsupported(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (ServiceEndpointRequestVerbUnsupported), culture, arg0);

    public static string ServiceEndpointTypeNotFound(object arg0, object arg1) => ServiceEndpointResources.Format(nameof (ServiceEndpointTypeNotFound), arg0, arg1);

    public static string ServiceEndpointTypeNotFound(object arg0, object arg1, CultureInfo culture) => ServiceEndpointResources.Format(nameof (ServiceEndpointTypeNotFound), culture, arg0, arg1);

    public static string ServiceEndpointUrlChangeNotAllowed(object arg0) => ServiceEndpointResources.Format(nameof (ServiceEndpointUrlChangeNotAllowed), arg0);

    public static string ServiceEndpointUrlChangeNotAllowed(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (ServiceEndpointUrlChangeNotAllowed), culture, arg0);

    public static string ServiceEndpointUrlChangeNotAllowedForAzureStack() => ServiceEndpointResources.Get(nameof (ServiceEndpointUrlChangeNotAllowedForAzureStack));

    public static string ServiceEndpointUrlChangeNotAllowedForAzureStack(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ServiceEndpointUrlChangeNotAllowedForAzureStack), culture);

    public static string ServicePrincipalClientIDRequiredForManualCreationMode() => ServiceEndpointResources.Get(nameof (ServicePrincipalClientIDRequiredForManualCreationMode));

    public static string ServicePrincipalClientIDRequiredForManualCreationMode(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ServicePrincipalClientIDRequiredForManualCreationMode), culture);

    public static string ServicePrincipalKeyRequiredForManualCreationMode() => ServiceEndpointResources.Get(nameof (ServicePrincipalKeyRequiredForManualCreationMode));

    public static string ServicePrincipalKeyRequiredForManualCreationMode(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ServicePrincipalKeyRequiredForManualCreationMode), culture);

    public static string ShouldStartWithEndpointOrConfigurationUrlError() => ServiceEndpointResources.Get(nameof (ShouldStartWithEndpointOrConfigurationUrlError));

    public static string ShouldStartWithEndpointOrConfigurationUrlError(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ShouldStartWithEndpointOrConfigurationUrlError), culture);

    public static string TryingToShareEndpointWithSameProject() => ServiceEndpointResources.Get(nameof (TryingToShareEndpointWithSameProject));

    public static string TryingToShareEndpointWithSameProject(CultureInfo culture) => ServiceEndpointResources.Get(nameof (TryingToShareEndpointWithSameProject), culture);

    public static string UnableToPopulateAzureStackData() => ServiceEndpointResources.Get(nameof (UnableToPopulateAzureStackData));

    public static string UnableToPopulateAzureStackData(CultureInfo culture) => ServiceEndpointResources.Get(nameof (UnableToPopulateAzureStackData), culture);

    public static string UntrustedHost(object arg0) => ServiceEndpointResources.Format(nameof (UntrustedHost), arg0);

    public static string UntrustedHost(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (UntrustedHost), culture, arg0);

    public static string UpdateAuthorizationDataFailedError(object arg0) => ServiceEndpointResources.Format(nameof (UpdateAuthorizationDataFailedError), arg0);

    public static string UpdateAuthorizationDataFailedError(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (UpdateAuthorizationDataFailedError), culture, arg0);

    public static string UpdateCountExceeded(object arg0) => ServiceEndpointResources.Format(nameof (UpdateCountExceeded), arg0);

    public static string UpdateCountExceeded(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (UpdateCountExceeded), culture, arg0);

    public static string UrlNotExpectedForOAuth2Endpoint(object arg0) => ServiceEndpointResources.Format(nameof (UrlNotExpectedForOAuth2Endpoint), arg0);

    public static string UrlNotExpectedForOAuth2Endpoint(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (UrlNotExpectedForOAuth2Endpoint), culture, arg0);

    public static string UserRole() => ServiceEndpointResources.Get(nameof (UserRole));

    public static string UserRole(CultureInfo culture) => ServiceEndpointResources.Get(nameof (UserRole), culture);

    public static string FailedToObtainTokenForClient(object arg0) => ServiceEndpointResources.Format(nameof (FailedToObtainTokenForClient), arg0);

    public static string FailedToObtainTokenForClient(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (FailedToObtainTokenForClient), culture, arg0);

    public static string KubernetesGetSecretError(object arg0) => ServiceEndpointResources.Format(nameof (KubernetesGetSecretError), arg0);

    public static string KubernetesGetSecretError(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (KubernetesGetSecretError), culture, arg0);

    public static string KubernetesGetServiceAccountError(object arg0) => ServiceEndpointResources.Format(nameof (KubernetesGetServiceAccountError), arg0);

    public static string KubernetesGetServiceAccountError(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (KubernetesGetServiceAccountError), culture, arg0);

    public static string KubernetesRoleBindingCreationError(object arg0) => ServiceEndpointResources.Format(nameof (KubernetesRoleBindingCreationError), arg0);

    public static string KubernetesRoleBindingCreationError(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (KubernetesRoleBindingCreationError), culture, arg0);

    public static string KubernetesServiceAccountCreationError(object arg0) => ServiceEndpointResources.Format(nameof (KubernetesServiceAccountCreationError), arg0);

    public static string KubernetesServiceAccountCreationError(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (KubernetesServiceAccountCreationError), culture, arg0);

    public static string KubernetesMissingSecretError() => ServiceEndpointResources.Get(nameof (KubernetesMissingSecretError));

    public static string KubernetesMissingSecretError(CultureInfo culture) => ServiceEndpointResources.Get(nameof (KubernetesMissingSecretError), culture);

    public static string CannotUpdateConfidentialAuthDataKubernetesAzureSubscriptionType(object arg0) => ServiceEndpointResources.Format(nameof (CannotUpdateConfidentialAuthDataKubernetesAzureSubscriptionType), arg0);

    public static string CannotUpdateConfidentialAuthDataKubernetesAzureSubscriptionType(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (CannotUpdateConfidentialAuthDataKubernetesAzureSubscriptionType), culture, arg0);
    }

    public static string CannotUpdateEndpointDataKubernetesAzureSubscriptionType(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceEndpointResources.Format(nameof (CannotUpdateEndpointDataKubernetesAzureSubscriptionType), arg0, arg1, arg2);
    }

    public static string CannotUpdateEndpointDataKubernetesAzureSubscriptionType(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (CannotUpdateEndpointDataKubernetesAzureSubscriptionType), culture, arg0, arg1, arg2);
    }

    public static string DeploymentNotProperlyRegisteredWithGitHub(object arg0, object arg1) => ServiceEndpointResources.Format(nameof (DeploymentNotProperlyRegisteredWithGitHub), arg0, arg1);

    public static string DeploymentNotProperlyRegisteredWithGitHub(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (DeploymentNotProperlyRegisteredWithGitHub), culture, arg0, arg1);
    }

    public static string DeploymentNotRegisteredWithGitHub(object arg0, object arg1) => ServiceEndpointResources.Format(nameof (DeploymentNotRegisteredWithGitHub), arg0, arg1);

    public static string DeploymentNotRegisteredWithGitHub(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (DeploymentNotRegisteredWithGitHub), culture, arg0, arg1);
    }

    public static string KubernetesErrorFetchingCluster(object arg0) => ServiceEndpointResources.Format(nameof (KubernetesErrorFetchingCluster), arg0);

    public static string KubernetesErrorFetchingCluster(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (KubernetesErrorFetchingCluster), culture, arg0);

    public static string KubernetesErrorFetchingClusterInvalidResponse() => ServiceEndpointResources.Get(nameof (KubernetesErrorFetchingClusterInvalidResponse));

    public static string KubernetesErrorFetchingClusterInvalidResponse(CultureInfo culture) => ServiceEndpointResources.Get(nameof (KubernetesErrorFetchingClusterInvalidResponse), culture);

    public static string KubernetesInvalidValueForPropertyEnableRbac(object arg0) => ServiceEndpointResources.Format(nameof (KubernetesInvalidValueForPropertyEnableRbac), arg0);

    public static string KubernetesInvalidValueForPropertyEnableRbac(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (KubernetesInvalidValueForPropertyEnableRbac), culture, arg0);
    }

    public static string CannotUpdateNonConfidentialAuthDataKubernetesAzureSubscriptionType(
      object arg0)
    {
      return ServiceEndpointResources.Format(nameof (CannotUpdateNonConfidentialAuthDataKubernetesAzureSubscriptionType), arg0);
    }

    public static string CannotUpdateNonConfidentialAuthDataKubernetesAzureSubscriptionType(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (CannotUpdateNonConfidentialAuthDataKubernetesAzureSubscriptionType), culture, arg0);
    }

    public static string InsufficientPrivilegesOnAAD(object arg0) => ServiceEndpointResources.Format(nameof (InsufficientPrivilegesOnAAD), arg0);

    public static string InsufficientPrivilegesOnAAD(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (InsufficientPrivilegesOnAAD), culture, arg0);

    public static string InsufficientPrivilegesOnSubscription() => ServiceEndpointResources.Get(nameof (InsufficientPrivilegesOnSubscription));

    public static string InsufficientPrivilegesOnSubscription(CultureInfo culture) => ServiceEndpointResources.Get(nameof (InsufficientPrivilegesOnSubscription), culture);

    public static string InsufficientPrivilegesOverKeyVault(object arg0) => ServiceEndpointResources.Format(nameof (InsufficientPrivilegesOverKeyVault), arg0);

    public static string InsufficientPrivilegesOverKeyVault(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (InsufficientPrivilegesOverKeyVault), culture, arg0);

    public static string InsufficientPrivilegesOverScope(object arg0, object arg1) => ServiceEndpointResources.Format(nameof (InsufficientPrivilegesOverScope), arg0, arg1);

    public static string InsufficientPrivilegesOverScope(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (InsufficientPrivilegesOverScope), culture, arg0, arg1);
    }

    public static string SharingServiceConnectinNotAllowed() => ServiceEndpointResources.Get(nameof (SharingServiceConnectinNotAllowed));

    public static string SharingServiceConnectinNotAllowed(CultureInfo culture) => ServiceEndpointResources.Get(nameof (SharingServiceConnectinNotAllowed), culture);

    public static string ServiceEndpointNameAlreadyExists(object arg0) => ServiceEndpointResources.Format(nameof (ServiceEndpointNameAlreadyExists), arg0);

    public static string ServiceEndpointNameAlreadyExists(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (ServiceEndpointNameAlreadyExists), culture, arg0);

    public static string AzureKeyVaultSpnObjectNotFoundException(object arg0) => ServiceEndpointResources.Format(nameof (AzureKeyVaultSpnObjectNotFoundException), arg0);

    public static string AzureKeyVaultSpnObjectNotFoundException(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (AzureKeyVaultSpnObjectNotFoundException), culture, arg0);

    public static string KubernetesCreateNamespaceJobNamespaceNotFound(object arg0) => ServiceEndpointResources.Format(nameof (KubernetesCreateNamespaceJobNamespaceNotFound), arg0);

    public static string KubernetesCreateNamespaceJobNamespaceNotFound(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (KubernetesCreateNamespaceJobNamespaceNotFound), culture, arg0);
    }

    public static string KubernetesNamespaceCreationError(object arg0) => ServiceEndpointResources.Format(nameof (KubernetesNamespaceCreationError), arg0);

    public static string KubernetesNamespaceCreationError(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (KubernetesNamespaceCreationError), culture, arg0);

    public static string FailedToRemoveAzurePermissionErrorOnMLWorkspace(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceEndpointResources.Format(nameof (FailedToRemoveAzurePermissionErrorOnMLWorkspace), arg0, arg1, arg2);
    }

    public static string FailedToRemoveAzurePermissionErrorOnMLWorkspace(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (FailedToRemoveAzurePermissionErrorOnMLWorkspace), culture, arg0, arg1, arg2);
    }

    public static string FailedToSetAzurePermissionErrorOnMLWorkspace(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceEndpointResources.Format(nameof (FailedToSetAzurePermissionErrorOnMLWorkspace), arg0, arg1, arg2);
    }

    public static string FailedToSetAzurePermissionErrorOnMLWorkspace(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (FailedToSetAzurePermissionErrorOnMLWorkspace), culture, arg0, arg1, arg2);
    }

    public static string JiraConnectAppServiceEndpointUpdateNotAllowed() => ServiceEndpointResources.Get(nameof (JiraConnectAppServiceEndpointUpdateNotAllowed));

    public static string JiraConnectAppServiceEndpointUpdateNotAllowed(CultureInfo culture) => ServiceEndpointResources.Get(nameof (JiraConnectAppServiceEndpointUpdateNotAllowed), culture);

    public static string RefreshTokenError(object arg0, object arg1) => ServiceEndpointResources.Format(nameof (RefreshTokenError), arg0, arg1);

    public static string RefreshTokenError(object arg0, object arg1, CultureInfo culture) => ServiceEndpointResources.Format(nameof (RefreshTokenError), culture, arg0, arg1);

    public static string CannotUpdateConfidentialAuthDataAzureContainerRegistry(object arg0) => ServiceEndpointResources.Format(nameof (CannotUpdateConfidentialAuthDataAzureContainerRegistry), arg0);

    public static string CannotUpdateConfidentialAuthDataAzureContainerRegistry(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (CannotUpdateConfidentialAuthDataAzureContainerRegistry), culture, arg0);
    }

    public static string CannotUpdateEndpointDataDockerACRType(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceEndpointResources.Format(nameof (CannotUpdateEndpointDataDockerACRType), arg0, arg1, arg2);
    }

    public static string CannotUpdateEndpointDataDockerACRType(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (CannotUpdateEndpointDataDockerACRType), culture, arg0, arg1, arg2);
    }

    public static string CannotUpdateNonConfidentialAuthDataAzureContainerRegistry(object arg0) => ServiceEndpointResources.Format(nameof (CannotUpdateNonConfidentialAuthDataAzureContainerRegistry), arg0);

    public static string CannotUpdateNonConfidentialAuthDataAzureContainerRegistry(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (CannotUpdateNonConfidentialAuthDataAzureContainerRegistry), culture, arg0);
    }

    public static string CouldNotCreateAADappHelpMessage() => ServiceEndpointResources.Get(nameof (CouldNotCreateAADappHelpMessage));

    public static string CouldNotCreateAADappHelpMessage(CultureInfo culture) => ServiceEndpointResources.Get(nameof (CouldNotCreateAADappHelpMessage), culture);

    public static string RoleAssignmentHelpMessageForManagementGroup() => ServiceEndpointResources.Get(nameof (RoleAssignmentHelpMessageForManagementGroup));

    public static string RoleAssignmentHelpMessageForManagementGroup(CultureInfo culture) => ServiceEndpointResources.Get(nameof (RoleAssignmentHelpMessageForManagementGroup), culture);

    public static string RoleAssignmentHelpMessageForMLWorkspace() => ServiceEndpointResources.Get(nameof (RoleAssignmentHelpMessageForMLWorkspace));

    public static string RoleAssignmentHelpMessageForMLWorkspace(CultureInfo culture) => ServiceEndpointResources.Get(nameof (RoleAssignmentHelpMessageForMLWorkspace), culture);

    public static string RoleAssignmentHelpMessageForSubscription() => ServiceEndpointResources.Get(nameof (RoleAssignmentHelpMessageForSubscription));

    public static string RoleAssignmentHelpMessageForSubscription(CultureInfo culture) => ServiceEndpointResources.Get(nameof (RoleAssignmentHelpMessageForSubscription), culture);

    public static string AtleastOneProjectReferenceRequired() => ServiceEndpointResources.Get(nameof (AtleastOneProjectReferenceRequired));

    public static string AtleastOneProjectReferenceRequired(CultureInfo culture) => ServiceEndpointResources.Get(nameof (AtleastOneProjectReferenceRequired), culture);

    public static string ServiceEndpointAccessDeniedForDeleteOperation() => ServiceEndpointResources.Get(nameof (ServiceEndpointAccessDeniedForDeleteOperation));

    public static string ServiceEndpointAccessDeniedForDeleteOperation(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ServiceEndpointAccessDeniedForDeleteOperation), culture);

    public static string ServiceEndpointProjectReferenceInformationIdInvalid() => ServiceEndpointResources.Get(nameof (ServiceEndpointProjectReferenceInformationIdInvalid));

    public static string ServiceEndpointProjectReferenceInformationIdInvalid(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ServiceEndpointProjectReferenceInformationIdInvalid), culture);

    public static string ServiceEndpointProjectReferenceInformationNameInvalid() => ServiceEndpointResources.Get(nameof (ServiceEndpointProjectReferenceInformationNameInvalid));

    public static string ServiceEndpointProjectReferenceInformationNameInvalid(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ServiceEndpointProjectReferenceInformationNameInvalid), culture);

    public static string ServiceEndpointProjectReferenceNameNull() => ServiceEndpointResources.Get(nameof (ServiceEndpointProjectReferenceNameNull));

    public static string ServiceEndpointProjectReferenceNameNull(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ServiceEndpointProjectReferenceNameNull), culture);

    public static string ServiceEndpointProjectReferenceInformationNotProvided(object arg0) => ServiceEndpointResources.Format(nameof (ServiceEndpointProjectReferenceInformationNotProvided), arg0);

    public static string ServiceEndpointProjectReferenceInformationNotProvided(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (ServiceEndpointProjectReferenceInformationNotProvided), culture, arg0);
    }

    public static string ServiceEndpointAccessDeniedForCollectionAdminOperation() => ServiceEndpointResources.Get(nameof (ServiceEndpointAccessDeniedForCollectionAdminOperation));

    public static string ServiceEndpointAccessDeniedForCollectionAdminOperation(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ServiceEndpointAccessDeniedForCollectionAdminOperation), culture);

    public static string GitHubApplicationUninstalled() => ServiceEndpointResources.Get(nameof (GitHubApplicationUninstalled));

    public static string GitHubApplicationUninstalled(CultureInfo culture) => ServiceEndpointResources.Get(nameof (GitHubApplicationUninstalled), culture);

    public static string ErrorGeneratingInstallationAccessToken(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceEndpointResources.Format(nameof (ErrorGeneratingInstallationAccessToken), arg0, arg1, arg2);
    }

    public static string ErrorGeneratingInstallationAccessToken(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (ErrorGeneratingInstallationAccessToken), culture, arg0, arg1, arg2);
    }

    public static string InstallationAccessTokenCachingError(object arg0, object arg1) => ServiceEndpointResources.Format(nameof (InstallationAccessTokenCachingError), arg0, arg1);

    public static string InstallationAccessTokenCachingError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (InstallationAccessTokenCachingError), culture, arg0, arg1);
    }

    public static string InstallationIdNotFound() => ServiceEndpointResources.Get(nameof (InstallationIdNotFound));

    public static string InstallationIdNotFound(CultureInfo culture) => ServiceEndpointResources.Get(nameof (InstallationIdNotFound), culture);

    public static string DeploymentNotProperlyRegisteredWithBitbucket(object arg0, object arg1) => ServiceEndpointResources.Format(nameof (DeploymentNotProperlyRegisteredWithBitbucket), arg0, arg1);

    public static string DeploymentNotProperlyRegisteredWithBitbucket(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (DeploymentNotProperlyRegisteredWithBitbucket), culture, arg0, arg1);
    }

    public static string DeploymentNotRegisteredWithBitbucket(object arg0, object arg1) => ServiceEndpointResources.Format(nameof (DeploymentNotRegisteredWithBitbucket), arg0, arg1);

    public static string DeploymentNotRegisteredWithBitbucket(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (DeploymentNotRegisteredWithBitbucket), culture, arg0, arg1);
    }

    public static string FailedToLoadServiceEndpointTypeFromContribution(object arg0) => ServiceEndpointResources.Format(nameof (FailedToLoadServiceEndpointTypeFromContribution), arg0);

    public static string FailedToLoadServiceEndpointTypeFromContribution(
      object arg0,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (FailedToLoadServiceEndpointTypeFromContribution), culture, arg0);
    }

    public static string CouldNotFetchPublishProfile(object arg0) => ServiceEndpointResources.Format(nameof (CouldNotFetchPublishProfile), arg0);

    public static string CouldNotFetchPublishProfile(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (CouldNotFetchPublishProfile), culture, arg0);

    public static string CouldNotFetchPublishProfileHelpMessage(object arg0) => ServiceEndpointResources.Format(nameof (CouldNotFetchPublishProfileHelpMessage), arg0);

    public static string CouldNotFetchPublishProfileHelpMessage(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (CouldNotFetchPublishProfileHelpMessage), culture, arg0);

    public static string InvalidPublishProfileEndpointField(object arg0) => ServiceEndpointResources.Format(nameof (InvalidPublishProfileEndpointField), arg0);

    public static string InvalidPublishProfileEndpointField(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (InvalidPublishProfileEndpointField), culture, arg0);

    public static string MultipleEndpointUpdateShouldBeFromSameProject() => ServiceEndpointResources.Get(nameof (MultipleEndpointUpdateShouldBeFromSameProject));

    public static string MultipleEndpointUpdateShouldBeFromSameProject(CultureInfo culture) => ServiceEndpointResources.Get(nameof (MultipleEndpointUpdateShouldBeFromSameProject), culture);

    public static string EndpointCreatorRoleDescription() => ServiceEndpointResources.Get(nameof (EndpointCreatorRoleDescription));

    public static string EndpointCreatorRoleDescription(CultureInfo culture) => ServiceEndpointResources.Get(nameof (EndpointCreatorRoleDescription), culture);

    public static string EndpointReaderRoleDescription() => ServiceEndpointResources.Get(nameof (EndpointReaderRoleDescription));

    public static string EndpointReaderRoleDescription(CultureInfo culture) => ServiceEndpointResources.Get(nameof (EndpointReaderRoleDescription), culture);

    public static string EndpointAccessDeniedForCreate() => ServiceEndpointResources.Get(nameof (EndpointAccessDeniedForCreate));

    public static string EndpointAccessDeniedForCreate(CultureInfo culture) => ServiceEndpointResources.Get(nameof (EndpointAccessDeniedForCreate), culture);

    public static string CannotUpgradeAuthorizationSchemeError(object arg0) => ServiceEndpointResources.Format(nameof (CannotUpgradeAuthorizationSchemeError), arg0);

    public static string CannotUpgradeAuthorizationSchemeError(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (CannotUpgradeAuthorizationSchemeError), culture, arg0);

    public static string UpgradeAuthorizationSchemeNotSupported(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceEndpointResources.Format(nameof (UpgradeAuthorizationSchemeNotSupported), arg0, arg1, arg2);
    }

    public static string UpgradeAuthorizationSchemeNotSupported(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (UpgradeAuthorizationSchemeNotSupported), culture, arg0, arg1, arg2);
    }

    public static string UpgradeCannotChangeProperties(object arg0) => ServiceEndpointResources.Format(nameof (UpgradeCannotChangeProperties), arg0);

    public static string UpgradeCannotChangeProperties(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (UpgradeCannotChangeProperties), culture, arg0);

    public static string AnotherOperationAlreadyInProgress(object arg0) => ServiceEndpointResources.Format(nameof (AnotherOperationAlreadyInProgress), arg0);

    public static string AnotherOperationAlreadyInProgress(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (AnotherOperationAlreadyInProgress), culture, arg0);

    public static string ProjectReferenceMissing() => ServiceEndpointResources.Get(nameof (ProjectReferenceMissing));

    public static string ProjectReferenceMissing(CultureInfo culture) => ServiceEndpointResources.Get(nameof (ProjectReferenceMissing), culture);

    public static string ConversionBetweenAuthSchemesNotSupported(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceEndpointResources.Format(nameof (ConversionBetweenAuthSchemesNotSupported), arg0, arg1, arg2);
    }

    public static string ConversionBetweenAuthSchemesNotSupported(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (ConversionBetweenAuthSchemesNotSupported), culture, arg0, arg1, arg2);
    }

    public static string CouldNotDowngradeMissingRecoveryData(object arg0, object arg1) => ServiceEndpointResources.Format(nameof (CouldNotDowngradeMissingRecoveryData), arg0, arg1);

    public static string CouldNotDowngradeMissingRecoveryData(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceEndpointResources.Format(nameof (CouldNotDowngradeMissingRecoveryData), culture, arg0, arg1);
    }

    public static string CannotUpdateWorkloadIdentityIssuer(object arg0) => ServiceEndpointResources.Format(nameof (CannotUpdateWorkloadIdentityIssuer), arg0);

    public static string CannotUpdateWorkloadIdentityIssuer(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (CannotUpdateWorkloadIdentityIssuer), culture, arg0);

    public static string CannotUpdateWorkloadIdentitySubject(object arg0) => ServiceEndpointResources.Format(nameof (CannotUpdateWorkloadIdentitySubject), arg0);

    public static string CannotUpdateWorkloadIdentitySubject(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (CannotUpdateWorkloadIdentitySubject), culture, arg0);

    public static string OutdatedServiceConnectionName(object arg0) => ServiceEndpointResources.Format(nameof (OutdatedServiceConnectionName), arg0);

    public static string OutdatedServiceConnectionName(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (OutdatedServiceConnectionName), culture, arg0);

    public static string CreationOfNewEndpointsTemporarilySuspended() => ServiceEndpointResources.Get(nameof (CreationOfNewEndpointsTemporarilySuspended));

    public static string CreationOfNewEndpointsTemporarilySuspended(CultureInfo culture) => ServiceEndpointResources.Get(nameof (CreationOfNewEndpointsTemporarilySuspended), culture);

    public static string EndpointDisabledError(object arg0) => ServiceEndpointResources.Format(nameof (EndpointDisabledError), arg0);

    public static string EndpointDisabledError(object arg0, CultureInfo culture) => ServiceEndpointResources.Format(nameof (EndpointDisabledError), culture, arg0);
  }
}
