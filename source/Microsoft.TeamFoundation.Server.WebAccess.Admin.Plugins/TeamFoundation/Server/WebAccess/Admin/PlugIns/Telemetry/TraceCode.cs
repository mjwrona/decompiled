// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Telemetry.TraceCode
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Telemetry
{
  public enum TraceCode
  {
    IssueSessionTokenException = 1005003, // 0x000F55CB
    IssueDeleteSessionTokenException = 1005004, // 0x000F55CC
    IssueNameAvailabilitySessionTokenException = 1005005, // 0x000F55CD
    FailureToResolveTargetUrl = 1005006, // 0x000F55CE
    OrganizationSettingsGetTimeZones = 10050001, // 0x009959D1
    IssueSessionTokenError = 10050002, // 0x009959D2
    OrganizationSettingsFetchUserTenant = 10050050, // 0x00995A02
    OrganizationSettingsFetchOrgTenant = 10050051, // 0x00995A03
    DisconnectedUserAPI = 10050052, // 0x00995A04
    DisconnectedUserQueryAadFailure = 10050053, // 0x00995A05
    DisconnectedUserTooManyUsersWarning = 10050054, // 0x00995A06
    DisconnectedGetTenantFailure = 10050055, // 0x00995A07
    DisconnectedGetOrgMembersFailure = 10050056, // 0x00995A08
    OrganizationSettingsNoIdentityDescriptor = 10050057, // 0x00995A09
    PermissionsTraceFailure = 10050058, // 0x00995A0A
    NumberOfOrgUsers = 10050059, // 0x00995A0B
    GroupsWithoutSubjectDescriptor = 10050061, // 0x00995A0D
    DataProviderParam = 10050062, // 0x00995A0E
    DataProviderReadIdentityException = 10050063, // 0x00995A0F
    DataProviderSecurityClientException = 10050064, // 0x00995A10
    PermissionsDataProviderException = 10050065, // 0x00995A11
    PermissionsTraceDataProviderException = 10050066, // 0x00995A12
    ProfileSettingsPreferencesFailure = 10050067, // 0x00995A13
    ProfileSettingsLoadFailure = 10050068, // 0x00995A14
    ProjectAdministratorsLoadFailure = 10050069, // 0x00995A15
    ProjectOverviewDataLoadFailure = 10050070, // 0x00995A16
    ProjectOverviewImageDataLoadFailure = 10050071, // 0x00995A17
    ProjectPermissionsDataLoadFailure = 10050072, // 0x00995A18
    AuthorizationsDataLoadFailure = 10050073, // 0x00995A19
    RevokeAuthorizationFailure = 10050074, // 0x00995A1A
    SetDefaultTeamException = 10050075, // 0x00995A1B
    SecurityMembersUnauthorizedAccessException = 10050076, // 0x00995A1C
    SecurityMembersGeneralException = 10050077, // 0x00995A1D
    SecurityViewUpdateException = 10050078, // 0x00995A1E
    ProfileDelayedDataException = 10050079, // 0x00995A1F
    GetSPSSignInUrl = 10050080, // 0x00995A20
    GetCurrentUserTenant = 10050081, // 0x00995A21
    SetTeamAdminException = 10050082, // 0x00995A22
    CreateAreaPathException = 10050083, // 0x00995A23
    OrganizationProjectsLoadFailure = 10050084, // 0x00995A24
    GetAadTenantPolicyException = 10050085, // 0x00995A25
    ParseObjectIdListException = 10050086, // 0x00995A26
    GetProjectLevelPermissionsException = 10050087, // 0x00995A27
    ReadIdentitiesFromDdsException = 10050088, // 0x00995A28
    DataProviderMembersPivotException = 10050089, // 0x00995A29
    SecurityViewDataProviderException = 10050090, // 0x00995A2A
    RemoveAdminDataProviderException = 10050091, // 0x00995A2B
    getTeamInProjectDataProviderException = 10050092, // 0x00995A2C
    LeaveOrganizationException = 10050093, // 0x00995A2D
    IsNameAvailableException = 10050094, // 0x00995A2E
    RestoreOrganizationException = 10050095, // 0x00995A2F
    TeamAdminListException = 10050096, // 0x00995A30
    IsMaterializedMember = 10050097, // 0x00995A31
    IsDefaultTeamDataProviderException = 10050098, // 0x00995A32
    ResolveProjectScopedUserPermissionException = 10050099, // 0x00995A33
    DataProviderWellKnownGroupsException = 10050101, // 0x00995A35
    OwnerIdentityNullValue = 10050102, // 0x00995A36
    ProfileSyncDefaultValue = 10050103, // 0x00995A37
    ProfileSyncDefaultValueGeneralException = 10050104, // 0x00995A38
    ProfileSyncDefaultValueAttributeNotFoundException = 10050105, // 0x00995A39
    ProfileSyncFeatureStateListener_Enter = 10050106, // 0x00995A3A
    ProfileSyncFeatureStateListener_Leave = 10050107, // 0x00995A3B
    ProfileSyncFeatureStateListener_Enabling_Error = 10050108, // 0x00995A3C
    ProfileSyncFeatureStateListener_Disabling_Error = 10050109, // 0x00995A3D
    ProfileSyncFeatureStateListener_GetSyncedUserProfileData_Enter = 10050110, // 0x00995A3E
    ProfileSyncFeatureStateListener_GetSyncedUserProfileData_Leave = 10050111, // 0x00995A3F
  }
}
