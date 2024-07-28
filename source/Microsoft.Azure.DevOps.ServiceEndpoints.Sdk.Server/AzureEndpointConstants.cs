// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureEndpointConstants
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  [GenerateAllConstants(null)]
  public static class AzureEndpointConstants
  {
    public const string TenantId = "tenantid";
    public const string AppObjectId = "appObjectId";
    public const string SpnObjectId = "spnObjectId";
    public const string SpnCreationMode = "creationMode";
    public const string AzureSubscriptionId = "subscriptionId";
    public const string AzureSubscriptionName = "subscriptionName";
    public const string ResourceGroupName = "resourceGroupName";
    public const string AzureManagementGroupId = "managementGroupId";
    public const string MlWorkspaceName = "mlWorkspaceName";
    public const string MlWorkspaceId = "mlWorkspaceId";
    public const string MlWorkspaceLocation = "mlWorkspaceLocation";
    public const string AzureManagementGroupName = "managementGroupName";
    public const string ManagementGroupString = "ManagementGroup";
    public const string WorkspaceString = "AzureMLWorkspace";
    public const string SubscriptionString = "Subscription";
    public const string SpnCreationModeAutomatic = "Automatic";
    public const string SpnCreationModeManual = "Manual";
    public const string SpnClientId = "serviceprincipalid";
    public const string SpnKey = "servicePrincipalKey";
    public const string AuthenticationType = "authenticationType";
    public const string SpnKeyAuthType = "spnKey";
    public const string CertAuthType = "spnCertificate";
    public const string AzureSpnCertificate = "servicePrincipalCertificate";
    public const string AzureRmEndpointType = "azurerm";
    public const string AzureClassicEndpointType = "azure";
    public const string ContributorRoleId = "b24988ac-6180-42a0-ab88-20f7382dd24c";
    public const string OwnerRoleId = "8e3af657-a8ff-443c-a75c-2fe8c4bcb635";
    public const string UserAccessAdministratorRoleId = "18d7d88d-d35e-4fb5-a5c3-7773c20a72d9";
    public const string AzureSpnRoleAssignmentId = "azureSpnRoleAssignmentId";
    public const string AzureSpnPermissions = "azureSpnPermissions";
    public const string ScopeLevel = "scopeLevel";
    public const string AppToken = "AppToken";
    public const string PublishProfile = "publishProfile";
    public const string ResourceId = "resourceId";
    public const string AppServiceProviderType = "microsoft.web/sites";
    public const string ResourceGroups = "resourceGroups";
    public const string Subscriptions = "subscriptions";
    public const string WorkloadIdentityFederationSubject = "workloadIdentityFederationSubject";
    public const string WorkloadIdentityFederationIssuer = "workloadIdentityFederationIssuer";
    public const string ConvertAuthenticationSchemeOperation = "ConvertAuthenticationScheme";
    public const string ConvertAuthenticationSchemeInProgressStatus = "converting_scheme";
    public const string ConvertAuthenticationSchemeFailedStatus = "converting_scheme_failed";
    public const string ChangePropertiesOperation = "ChangeProperties";
    public const string IsDraft = "isDraft";
    public const string RevertSchemeDeadline = "revertSchemeDeadline";
    public const string RevertSchemeDeadlineExpired = "revertSchemeDeadlineExpired";
    internal const string DefaultArmUriString = "resourceManagerUrl";
    internal const string DefaultGraphUriString = "graphUrl";
    internal const string DefaultAuthorityUriString = "environmentAuthorityUrl";
    internal const string DefaultGraphApiVersionString = "defaultGraphApiVersion";
    internal const string DefaultGalleryUrlString = "galleryUrl";
    internal const string DefaultServiceManagementUriString = "serviceManagementUrl";
    internal const string DefaultActiveDirectoryAuthorityString = "activeDirectoryAuthority";
    internal const string DefaultArmManagementPortalUriString = "armManagementPortalUrl";
    internal const string DefaultAzureKeyVaultDnsSuffixString = "AzureKeyVaultDnsSuffix";
    internal const string DefaultAzureKeyVaultServiceEndpointResourceIdString = "AzureKeyVaultServiceEndpointResourceId";
    internal const string DefaultActiveDirectoryServiceEndpointResourceIdString = "ActiveDirectoryServiceEndpointResourceId";
    internal const string ServicePrincipalJobName = "AzureServicePrincipalEndpointJob";
    internal const string ServicePrincipalJobExtensionName = "Microsoft.Azure.DevOps.ServiceEndpoints.Server.Extensions.AzureServicePrincipalEndpointJob";
    internal const string PublishProfileJobName = "AzurePublishProfileEndpointJob";
    internal const string PublishProfileJobExtensionName = "Microsoft.Azure.DevOps.ServiceEndpoints.Server.Extensions.AzurePublishProfileEndpointJob";
    internal const string ArmUriRegistryPath = "/Service/Commerce/AzureResourceManager/BaseUrl";
    internal const string ArmSubscriptionsFormat = "subscriptions?api-version={0}";
    internal const string DynamicAppDisplayNamePrefix = "VisualStudioSPN";
    internal const string DynamicAppUrl = "https://VisualStudio/SPN";
    internal const string ArmRoleAssignmentFormat = "{0}/providers/Microsoft.Authorization/roleAssignments/{1}?api-version={2}";
    internal const string ArmGetRoleAssignmentFormat = "{0}/providers/Microsoft.Authorization/roleAssignments?api-version={1}&$filter=principalId eq '{2}'";
    internal const string ArmApiVersion = "2015-07-01";
    internal const string EnableAdfsAuthentication = "EnableAdfsAuthentication";
    internal const string AdfsUrlSuffix = "/adfs/";
    internal const string AuthorizationHeader = "Authorization";
    internal const string ContentTypeHeader = "Content-Type";
    internal const string AcceptHeader = "Accept";
    internal const string TransferEncodingHeader = "TransferEncoding";
    internal const string ContentLengthHeader = "Content-Length";
    internal const string ArmResourceUri = "https://management.core.windows.net/";
    internal const string VstsClientId = "499b84ac-1321-427f-aa17-267ca6975798";
    internal const string DefaultArmUri = "https://management.azure.com";
    internal const string DefaultGraphUri = "https://graph.windows.net/";
    internal const string DefaultMicrosoftGraphUri = "https://graph.microsoft.com/";
    internal const string DefaultMicrosoftGraphApiVersion = "v1.0";
    internal const string DefaultAuthorityUri = "https://login.windows.net/";
    internal const string MsalDefaultAuthorityUri = "https://login.microsoftonline.com/";
    internal const string DefaultGraphApiVersion = "1.6";
    internal const string PpeArmUri = "https://api-dogfood.resources.windows-int.net/";
    internal const string PpeGraphUri = "https://graph.ppe.windows.net/";
    internal const string PpeMicrosoftGraphUri = "https://graph.microsoft-ppe.com/";
    internal const string PpeMicrosoftGraphApiVersion = "v1.0";
    internal const string PpeAuthorityUri = "https://login.windows-ppe.net/";
    internal const string PpeGraphApiVersion = "1.5-internal";
    internal const string SubScriptionId = "subscriptionId";
    internal const string DefaultManagementGroupApiVersion = "2018-03-01-preview";
    internal const string DefaultListSubscriptionsApiVersion = "2019-06-01";
    internal const string DefaultAzureEnvironment = "AzureCloud";
    internal const string EnvironmentString = "environment";
    internal const string AzureStackString = "AzureStack";
  }
}
