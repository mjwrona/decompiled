// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraphTrace
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  internal static class MicrosoftGraphTrace
  {
    public const string Area = "MicrosoftGraph";
    public const int MicrosoftGraphClientProcessRequestRequestEnter = 44750000;
    public const int MicrosoftGraphClientProcessRequestRequestInformation = 44750001;
    public const int MicrosoftGraphClientProcessRequestRequestClientException = 44750002;
    public const int MicrosoftGraphClientProcessRequestRequestNotFound = 44750003;
    public const int MicrosoftGraphClientProcessRequestRequestUnauthorized = 44750004;
    public const int MicrosoftGraphClientProcessRequestRequestServiceException = 44750005;
    public const int MicrosoftGraphClientProcessRequestRequestExecuteException = 44750006;
    public const int MicrosoftGraphClientProcessRequestRequestLeave = 44750007;
    public const int MicrosoftGraphClientInvalidNextLinkResponse = 44750223;
    public const int MicrosoftGraphClientProcessRequestRequestValidateError = 44750008;
    public const int MicrosoftGraphClientProcessRequestRequestDenied = 44750009;
    public const int MicrosoftGraphClientGetUserStatusWithIdRequestException = 44750010;
    public const int MicrosoftGraphClientGetUserStatusWithIdRequestExecuteNotFound = 44750012;
    public const int MicrosoftGraphClientGetUserStatusWithIdRequestExecuteLeave = 44750013;
    public const int MicrosoftGraphClientGetUsersRequestExecuteEnter = 44750020;
    public const int MicrosoftGraphClientGetUsersRequestExecuteEnterPaged = 44750021;
    public const int MicrosoftGraphClientGetUsersRequestExecuteException = 44750022;
    public const int MicrosoftGraphClientGetUsersRequestExecuteLeave = 44750023;
    public const int MicrosoftGraphClientGetUsersExpandManager = 44750024;
    public const int MicrosoftGraphClientGetServicePrincipalsRequestExecuteEnter = 44750025;
    public const int MicrosoftGraphClientGetServicePrincipalsRequestExecuteEnterPaged = 44750026;
    public const int MicrosoftGraphClientGetServicePrincipalsRequestExecuteLeave = 44750027;
    public const int MicrosoftGraphClientGetGroupsRequestExecuteEnter = 44750030;
    public const int MicrosoftGraphClientGetGroupsRequestExecuteEnterPaged = 44750031;
    public const int MicrosoftGraphClientGetGroupsRequestExecuteLeave = 44750032;
    public const int MicrosoftGraphClientGetDirectoryRoleMembersRequestExecuteEnter = 44750040;
    public const int MicrosoftGraphClientGetDirectoryRoleMembersRequestException = 44750041;
    public const int MicrosoftGraphClientGetDirectoryRoleMembersRequestExecuteLeave = 44750042;
    public const int MicrosoftGraphClientGetDirectoryRolesWithIdsRequestExecuteEnter = 44750045;
    public const int MicrosoftGraphClientGetDirectoryRolesWithIdsRequestExecuteLeave = 44750046;
    public const int MicrosoftGraphClientGetTenantRequestExecuteEnter = 44750050;
    public const int MicrosoftGraphClientGetTenantRequestExecuteException = 44750051;
    public const int MicrosoftGraphClientGetTenantRequestExecuteLeave = 44750052;
    public const int MicrosoftGraphClientGetUserThumbnailRequestExecuteEnter = 44750060;
    public const int MicrosoftGraphClientGetUserThumbnailRequestExecuteException = 44750061;
    public const int MicrosoftGraphClientGetUserThumbnailRequestExecuteLeave = 44750062;
    public const int MicrosoftGraphClientGetUsersWithIdsRequestExecuteEnter = 44750070;
    public const int MicrosoftGraphClientGetUsersWithIdsRequestExecuteLeave = 44750071;
    public const int MicrosoftGraphClientGetDescendantIdsRequestExecuteEnter = 44750080;
    public const int MicrosoftGraphClientGetDescendantIdsRequestExecuteEnterPaged = 44750081;
    public const int MicrosoftGraphClientGetDescendantIdsRequestExecuteException = 44750082;
    public const int MicrosoftGraphClientGetDescendantIdsRequestExecuteLeave = 44750083;
    public const int MicrosoftGraphClientGetDescendantsRequestExecuteEnter = 44750090;
    public const int MicrosoftGraphClientGetDescendantsRequestExecuteEnterPaged = 44750091;
    public const int MicrosoftGraphClientGetDescendantsRequestExecuteException = 44750092;
    public const int MicrosoftGraphClientGetDescendantsRequestExecuteLeave = 44750093;
    public const int MicrosoftGraphClientGetDirectoryRolesRequestExecuteEnter = 44750100;
    public const int MicrosoftGraphClientGetDirectoryRolesRequestExecuteException = 44750101;
    public const int MicrosoftGraphClientGetDirectoryRolesRequestExecuteLeave = 44750102;
    public const int MicrosoftGraphClientGetGroupsWithIdsRequestExecuteEnter = 44750110;
    public const int MicrosoftGraphClientGetGroupsWithIdsRequestExecuteLeave = 44750111;
    public const int MicrosoftGraphClientGetUserRolesAndGroupsRequestExecuteEnter = 44750120;
    public const int MicrosoftGraphClientGetUserRolesAndGroupsRequestExecuteException = 44750121;
    public const int MicrosoftGraphClientGetUserRolesAndGroupsRequestExecuteLeave = 44750122;
    public const int MicrosoftGraphClientGetSoftDeletedObjectsRequestExecuteEnter = 44750130;
    public const int MicrosoftGraphClientGetSoftDeletedObjectsRequestExecuteException = 44750131;
    public const int MicrosoftGraphClientGetSoftDeletedObjectsRequestExecuteLeave = 44750132;
    public const int MicrosoftGraphClientGetAncestorIdsRequestExecuteEnter = 44750140;
    public const int MicrosoftGraphClientGetAncestorIdsRequestExecuteException = 44750141;
    public const int MicrosoftGraphClientGetAncestorIdsRequestExecuteLeave = 44750142;
    public const int MicrosoftGraphClientGetAncestorIdsRequestExecuteExecuteNotFound = 44750143;
    public const int MicrosoftGraphClientUpdateApplicationRequestExecuteEnter = 44750150;
    public const int MicrosoftGraphClientUpdateApplicationRequestExecuteException = 44750151;
    public const int MicrosoftGraphClientUpdateApplicationRequestExecuteLeave = 44750152;
    public const int MicrosoftGraphClientCreateApplicationRequestExecuteEnter = 44750160;
    public const int MicrosoftGraphClientCreateApplicationRequestExecuteException = 44750161;
    public const int MicrosoftGraphClientCreateApplicationRequestExecuteLeave = 44750162;
    public const int MicrosoftGraphClientCreateServicePrincipalRequestExecuteEnter = 44750170;
    public const int MicrosoftGraphClientCreateServicePrincipalRequestExecuteException = 44750171;
    public const int MicrosoftGraphClientCreateServicePrincipalRequestExecuteLeave = 44750172;
    public const int MicrosoftGraphClientAddApplicationPasswordRequestExecuteEnter = 44750180;
    public const int MicrosoftGraphClientAddApplicationPasswordRequestExecuteException = 44750181;
    public const int MicrosoftGraphClientAddApplicationPasswordRequestExecuteLeave = 44750182;
    public const int MicrosoftGraphClientRemoveApplicationPasswordRequestExecuteEnter = 44750190;
    public const int MicrosoftGraphClientRemoveApplicationPasswordRequestExecuteException = 44750191;
    public const int MicrosoftGraphClientRemoveApplicationPasswordRequestExecuteLeave = 44750192;
    public const int MicrosoftGraphClientDeleteApplicationRequestExecuteEnter = 44750200;
    public const int MicrosoftGraphClientDeleteApplicationRequestExecuteException = 44750201;
    public const int MicrosoftGraphClientDeleteApplicationRequestExecuteLeave = 44750202;
    public const int MicrosoftGraphClientDeleteServicePrincipalRequestExecuteEnter = 44750210;
    public const int MicrosoftGraphClientDeleteServicePrincipalRequestExecuteException = 44750211;
    public const int MicrosoftGraphClientDeleteServicePrincipalRequestExecuteLeave = 44750212;
    public const int MicrosoftGraphClientGetApplicationByIdRequestExecuteEnter = 44750220;
    public const int MicrosoftGraphClientGetApplicationByIdRequestExecuteException = 44750221;
    public const int MicrosoftGraphClientGetApplicationByIdRequestExecuteLeave = 44750222;
    public const int MicrosoftGraphClientAddApplicationFederatedCredentialsRequestExecuteEnter = 44750230;
    public const int MicrosoftGraphClientAddApplicationFederatedCredentialsRequestExecuteException = 44750231;
    public const int MicrosoftGraphClientAddApplicationFederatedCredentialsRequestExecuteLeave = 44750232;
    public const int MicrosoftGraphClientRemoveApplicationFederatedCredentialsRequestExecuteEnter = 44750233;
    public const int MicrosoftGraphClientRemoveApplicationFederatedCredentialsRequestExecuteException = 44750234;
    public const int MicrosoftGraphClientRemoveApplicationFederatedCredentialsRequestExecuteLeave = 44750235;
    public const int MicrosoftGraphClientGetTenantsRequestExecuteEnter = 44750240;
    public const int MicrosoftGraphClientGetTenantsRequestExecuteLeave = 44750241;
    public const int MicrosoftGraphClientGetOrganizationRequestExecuteEnter = 44750250;
    public const int MicrosoftGraphClientGetOrganizationRequestExecuteException = 44750251;
    public const int MicrosoftGraphClientGetOrganizationRequestExecuteLeave = 44750252;
    public const int MicrosoftGraphClientGetProfileDataRequestExecuteEnter = 44750260;
    public const int MicrosoftGraphClientGetProfileDataRequestExecuteException = 44750261;
    public const int MicrosoftGraphClientGetProfileDataRequestExecuteLeave = 44750262;
  }
}
