// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.TracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public static class TracePoints
  {
    private const int AuthenticationCheckStart = 1010000;
    private const int AuthenticationCheckEnd = 1010999;
    private const int NextAvailable = 5511200;

    public static class AADAuthTokenValidatorTracePoints
    {
      public static int IdentityValidationFailure = 5510100;
      public static int PuidClaimDecision = 5510101;
      public static int AadTokenScopesValidation = 5510102;
      public static int PuidClaimInfo = 5510602;
      public static int IdentityProviderClaimDecision = 5510600;
      public static int UniqueNameClaimDecision = 5510601;
      public static int EmailClaimDecision = 5510603;
      public static int AccountNameDecision = 5510651;
      public static int DisplayNameClaimDecision = 5510652;
      public static int AccessTokenDecision = 5510604;
      public static int RefreshTokenDecision = 5510605;
      public static int AadTokenError = 5510606;
      public static int TenantCreationDecision = 5510607;
      public static int TenantDisambiguationDecision = 5510608;
      public static int FoundSilentProfileCreationRequest = 5510609;
      public static int IsSilentAadProfileRequestException = 5510610;
      public static int IsSilentAadProfileRequestEnter = 5510611;
      public static int IsSilentAadProfileRequestLeave = 5510612;
    }

    public static class OAuth2TokenValidatorTracePoints
    {
      public const int ValidateTokenEnter = 5510200;
      public const int CannotValidateToken = 5510201;
      public const int ValidateTokenFailure = 5510202;
      public const int ValidateTokenLeave = 5510203;
      public const int ValidateTokenException = 5510209;
    }

    public static class ClientAuthTokenValidatorTracePoints
    {
      public const int TokenValidationFailure = 5510300;
      public const int TransformIdentityClaims = 5510310;
      public const int TransformIdentityClaimsFailure = 5510311;
      public const int TransformIdentityClaimsFromDims = 5510313;
      public const int TokenValidationIpAddress = 5510315;
      public const int TokenValidationSourceClaimsFailure = 5510316;
      public const int TokenValidationClientIpAddressFailure = 5510317;
      public const int TokenValidationIpAddressSuccess = 5510318;
      public const int TokenValidationIprRangeSuccess = 5510319;
      public const int ValidateScope = 5510320;
      public const int ValidateScopeFailure = 5510321;
      public const int ScopesContainAuthorizationGrantScope = 5510322;
      public const int ScopeParsingFailure = 5510323;
    }

    public static class S2SAuthTokenValidatorTracePoints
    {
      public const int ValidateIdentityFailure = 5510400;
      public const int TransformIdentityClaims = 5510410;
    }

    public static class OAuth2AuthenticationServiceTracePoints
    {
      public const int ValidateToken = 5510500;
      public const int ValidateTokenFailure = 5510501;
      public const int EncodedTokenNull = 5510502;
      public const int ValidateTokenTrace = 5510503;
      public const int CannotExchangeTokenTrace = 5510504;
    }

    public static class S2SCredentialsService
    {
      public const int GetS2SCredentials = 5510600;
    }

    public static class ScopeTemplateService
    {
      public const int GetScopeTemplateEntry = 5510701;
    }

    public static class FirstPartyS2SAuthTokenValidatorTracePoints
    {
      public const int IdpTypClaimMismatch = 55107800;
      public const int ScopeClaimPresent = 55107801;
      public const int UniqueNameClaimPresent = 55107802;
      public const int AppIdACrClaimNotPresent = 55107803;
      public const int AppIdACrClaimMismatch = 55107804;
    }

    public static class AADServicePrincipalAuthTokenValidatorTracePoints
    {
      public const int MandatoryClaimMissing = 55107900;
      public const int InvalidClaimPresent = 55107901;
    }

    public static class CspTokenHelperTracePoints
    {
      public const int AadTokenContainsCspClaim = 5511000;
      public const int DetectedCspDAPtoken = 5511001;
      public const int DetectedCspGDAPtoken = 5511002;
      public const int EncounteredGDAPTokenWithDAPRoles = 5511003;
      public const int EncounteredDAPTokenWithCSPv2OnlyWidsClaim = 5511004;
    }

    public static class JwtTracerTracePoints
    {
      public const int JwtUsed = 5511100;
    }
  }
}
