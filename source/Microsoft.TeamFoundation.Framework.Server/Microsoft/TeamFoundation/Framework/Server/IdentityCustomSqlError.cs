// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityCustomSqlError
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class IdentityCustomSqlError
  {
    public const int GenericError = 400000;
    public const int GroupCreationError = 400001;
    public const int UpdateGroupError = 400002;
    public const int AddMemberNoGroupError = 400003;
    public const int AddMemberCyclicMembershipError = 400004;
    public const int AddMemberIdentityAlreadyMemberError = 400005;
    public const int FindGroupNameDoesNotExistError = 400006;
    public const int FindGroupSidDoesNotExistError = 400007;
    public const int RemoveGroupMemberError = 400008;
    public const int RemoveLastAdminGroupMemberError = 400009;
    public const int RemoveNonexistentGroupError = 400010;
    public const int RemoveSpecialGroupError = 400012;
    public const int GroupScopeAlreadyExists = 400016;
    public const int RemoveGroupMemberNotMemberError = 400022;
    public const int AddGroupMemberIllegalMemberForLicenseGroup = 400023;
    public const int AddGroupMemberLicenseGroupFull = 400024;
    public const int GroupScopeDoesNotExist = 400025;
    public const int GroupNameAlreadyExists = 400026;
    public const int AddProjectGroupToGlobalGroup = 400031;
    public const int AddGroupProjectsDontMatch = 400032;
    public const int IdentityDomainMismatch = 400033;
    public const int DeleteLastLicensedAdmin = 400036;
    public const int IdentityDomainDoesNotExist = 400037;
    public const int IdentityNotFound = 400038;
    public const int CannotAccessIdentitiesAfterDetach = 400039;
    public const int IdentityAccountNameAlreadyInUse = 400040;
    public const int GroupNameNotRecognized = 400043;
    public const int IdentityStoreReadOnlyMode = 400044;
    public const int IdentityAliasAlreadyInUse = 400045;
    public const int UpdateGroupScopeVisibilityFailed = 400046;
    public const int DynamicIdentityTypeCreationNotSupported = 400047;
    public const int TooManyResults = 400048;
    public const int HistoricalIdentityNotFound = 400100;
    public const int InvalidIdTranslationData = 400101;
    public const int IdTranslationsAreMigrated = 400102;
    public const int InvalidIdentityKeyMapData = 400103;
    public const int InvalidIdentityKeyMapTypeId = 400104;
    public const int MinGroupSequenceIdError = 400105;
    public const int InvalidStorageKeyTranslationData = 400201;
    public const int RestoreGroupScopeValidationError = 400202;
    public const int RestoreGroupScopeExecutionError = 400203;
  }
}
