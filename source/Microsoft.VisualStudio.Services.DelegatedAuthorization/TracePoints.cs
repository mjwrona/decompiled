// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.TracePoints
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal static class TracePoints
  {
    private const int DelegatedAuthorizationServiceStart = 1048000;
    private const int DelegatedAuthorizationServiceEnd = 1048999;
    internal const int DelegatedAuthorizationComponentInitiateAuthorizationEnter = 1048501;
    internal const int DelegatedAuthorizationComponentInitiateAuthorizationException = 1048502;
    internal const int DelegatedAuthorizationComponentInitiateAuthorizationLeave = 1048503;
    internal const int DelegatedAuthorizationComponentAuthorizeEnter = 1048504;
    internal const int DelegatedAuthorizationComponentAuthorizeException = 1048505;
    internal const int DelegatedAuthorizationComponentAuthorizeLeave = 1048506;
    internal const int DelegatedAuthorizationComponentGetAuthorizationsEnter = 1048507;
    internal const int DelegatedAuthorizationComponentGetAuthorizationsException = 1048508;
    internal const int DelegatedAuthorizationComponentGetAuthorizationsLeave = 1048509;
    internal const int DelegatedAuthorizationComponentCreateAccessTokenEnter = 1048510;
    internal const int DelegatedAuthorizationComponentCreateAccessTokenException = 1048511;
    internal const int DelegatedAuthorizationComponentCreateAccessTokenLeave = 1048512;
    internal const int DelegatedAuthorizationComponentRefreshAccessTokenEnter = 1048513;
    internal const int DelegatedAuthorizationComponentRefreshAccessTokenException = 1048514;
    internal const int DelegatedAuthorizationComponentRefreshAccessTokenLeave = 1048515;
    internal const int DelegatedAuthorizationComponentRevokeEnter = 1048516;
    internal const int DelegatedAuthorizationComponentRevokeException = 1048517;
    internal const int DelegatedAuthorizationComponentRevokeLeave = 1048518;
    internal const int DelegatedAuthorizationComponentCreateRegistrationEnter = 1048519;
    internal const int DelegatedAuthorizationComponentCreateRegistrationException = 1048520;
    internal const int DelegatedAuthorizationComponentCreateRegistrationLeave = 1048521;
    internal const int DelegatedAuthorizationComponentDeleteRegistrationEnter = 1048522;
    internal const int DelegatedAuthorizationComponentDeleteRegistrationException = 1048523;
    internal const int DelegatedAuthorizationComponentDeleteRegistrationLeave = 1048524;
    internal const int PlatformDelegatedAuthorizationDeleteRegistrationEnter = 1048525;
    internal const int PlatformDelegatedAuthorizationDeleteRegistrationException = 1048526;
    internal const int PlatformDelegatedAuthorizationDeleteRegistrationLeave = 1048527;
    internal const int DelegatedAuthorizationComponentGetRegistrationEnter = 1048528;
    internal const int DelegatedAuthorizationComponentGetRegistrationException = 1048529;
    internal const int DelegatedAuthorizationComponentGetRegistrationLeave = 1048530;
    internal const int DelegatedAuthorizationComponentListRegistrationsEnter = 1048531;
    internal const int DelegatedAuthorizationComponentListRegistrationsException = 1048532;
    internal const int DelegatedAuthorizationComponentListRegistrationsLeave = 1048533;
    internal const int DelegatedAuthorizationComponentUpdateRegistrationEnter = 1048534;
    internal const int DelegatedAuthorizationComponentUpdateRegistrationException = 1048535;
    internal const int DelegatedAuthorizationComponentUpdateRegistrationLeave = 1048536;
    internal const int DelegatedAuthorizationComponentListAccessTokenKeysEnter = 1048537;
    internal const int DelegatedAuthorizationComponentListAccessTokenKeysException = 1048538;
    internal const int DelegatedAuthorizationComponentListAccessTokenKeysLeave = 1048539;
    internal const int DelegatedAuthorizationComponentGetHostAuthorizationEnter = 1048540;
    internal const int DelegatedAuthorizationComponenGetHostAuthorizationException = 1048541;
    internal const int DelegatedAuthorizationComponentGetHostAuthorizationLeave = 1048542;
    internal const int DelegatedAuthorizationComponentCreateHostAuthorizationEnter = 1048543;
    internal const int DelegatedAuthorizationComponentCreateHostAuthorizationException = 1048544;
    internal const int DelegatedAuthorizationComponentCreateHostAuthorizationLeave = 1048545;
    internal const int DelegatedAuthorizationComponentRevokeHostAuthorizationEnter = 1048546;
    internal const int DelegatedAuthorizationComponentRevokeHostAuthorizationException = 1048547;
    internal const int DelegatedAuthorizationComponentRevokeHostAuthorizationLeave = 1048548;
    internal const int DelegatedAuthorizationComponentGetAccessTokenKeyEnter = 1048549;
    internal const int DelegatedAuthorizationComponentGetAccessTokenKeyException = 1048550;
    internal const int DelegatedAuthorizationComponentGetAccessTokenKeyLeave = 1048551;
    internal const int DelegatedAuthorizationComponentUpdateAccessTokenEnter = 1048552;
    internal const int DelegatedAuthorizationComponentUpdateAccessTokenException = 1048553;
    internal const int DelegatedAuthorizationComponentUpdateAccessTokenLeave = 1048554;
    internal const int DelegatedAuthorizationComponentRevokeAccessTokenKeyEnter = 1048555;
    internal const int DelegatedAuthorizationComponentRevokeAccessTokenKeyException = 1048556;
    internal const int DelegatedAuthorizationComponentRevokeAccessTokenKeyLeave = 1048557;
    internal const int DelegatedAuthorizationComponentIssueTokenPairEnter = 1048558;
    internal const int DelegatedAuthorizationComponentIssueTokenPairException = 1048559;
    internal const int DelegatedAuthorizationComponentIssueTokenPairLeave = 1048560;
    internal const int DelegatedAuthorizationComponentGetAuthorizationEnter = 1048561;
    internal const int DelegatedAuthorizationComponentGetAuthorizationException = 1048562;
    internal const int DelegatedAuthorizationComponentGetAuthorizationLeave = 1048563;
    internal const int DelegatedAuthorizationComponentGetAuthorizationIdsEnter = 1048564;
    internal const int DelegatedAuthorizationComponentGetAuthorizationIdsException = 1048565;
    internal const int DelegatedAuthorizationComponentGetAuthorizationIdsLeave = 1048566;
    internal const int DelegatedAuthorizationComponentUpdateAudienceAndAccessHashForSSHKeyEnter = 1048567;
    internal const int DelegatedAuthorizationComponentUpdateAudienceAndAccessHashForSSHKeyException = 1048568;
    internal const int DelegatedAuthorizationComponentUpdateAudienceAndAccessHashForSSHKeyLeave = 1048569;
    internal const int DelegatedAuthorizationComponentUpdateAudienceForPATsAndHostAuthEnter = 1048570;
    internal const int DelegatedAuthorizationComponentUpdateAudienceForPATsAndHostAuthException = 1048571;
    internal const int DelegatedAuthorizationComponentUpdateAudienceForPATsAndHostAuthLeave = 1048572;
    internal const int DelegatedAuthorizationComponentGetSSHKeysForCollectionEnter = 1048573;
    internal const int DelegatedAuthorizationComponentGetSSHKeysForCollectionException = 1048574;
    internal const int DelegatedAuthorizationComponentGetSSHKeysForCollectionLeave = 1048575;
    internal const int TokenExpirationComponentGetExpiringTokensEnter = 1048576;
    internal const int TokenExpirationComponentGetExpiringTokensException = 1048577;
    internal const int TokenExpirationComponentGetExpiringTokensLeave = 1048578;
    internal const int DelegatedAuthorizationComponentRemoveAccessTokenKeyEnter = 1048810;
    internal const int DelegatedAuthorizationComponentRemoveAccessTokenKeyException = 1048811;
    internal const int DelegatedAuthorizationComponentRemoveAccessTokenKeyLeave = 1048812;
    internal const int DelegatedAuthorizationComponentInsertHostAuthorizationEnter = 1048813;
    internal const int DelegatedAuthorizationComponentInsertHostAuthorizationException = 1048814;
    internal const int DelegatedAuthorizationComponentInsertHostAuthorizationLeave = 1048815;
    internal const int DelegatedAuthorizationComponentOauth2ControllerStart = 1048600;
    internal const int DelegatedAuthorizationComponentOauth2ControllerEnd = 1048700;

    internal static class PlatformDelegatedAuthorizationService
    {
      internal static class ListSessionTokensByUserId
      {
        internal const int Enter = 1048800;
        internal const int PermissionDenied = 1048807;
        internal const int Exception = 1048808;
        internal const int Leave = 1048809;
      }
    }
  }
}
