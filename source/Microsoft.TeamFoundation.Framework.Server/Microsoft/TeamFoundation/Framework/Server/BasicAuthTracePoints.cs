// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BasicAuthTracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class BasicAuthTracePoints
  {
    internal static class PlatformBasicAuthServiceTracePoints
    {
      internal const int IsValidBasicCredentialEnter = 1007000;
      internal const int IsValidBasicCredentialBasicCredentialsDisabled = 1007001;
      internal const int IsValidBasicCredentialBasicCredentialsDisabledForUser = 1007004;
      internal const int IsValidBasicCredentialInvalidPassword = 1007002;
      internal const int IsValidBasicCredentialBadAlgorithm = 1007003;
      internal const int IsValidBasicCredentialLeave = 1007009;
      internal const int SetBasicCredentialEnter = 1007010;
      internal const int SetBasicCredentialException = 1007018;
      internal const int SetBasicCredentialLeave = 1007019;
      internal const int DeleteBasicCredentialEnter = 1007020;
      internal const int DeleteBasicCredentialSuccess = 1007022;
      internal const int DeleteBasicCredentialLeave = 1007029;
      internal const int EnableDisabledAccountEnter = 1007040;
      internal const int EnableDisabledAccountLeave = 1007041;
    }

    internal static class FrameworkBasicAuthServiceTracePoints
    {
      internal const int IsValidBasicCredentialEnter = 1007000;
      internal const int IsValidBasicCredentialLeave = 1007009;
      internal const int IsValidBasicCredentialIdentityNotFound = 1007005;
      internal const int IsValidBasicCredentialCacheMatch = 1007007;
      internal const int IsValidBasicCredentialCacheMismatch = 1007015;
      internal const int IsValidBasicCredentialCacheMiss = 1007016;
      internal const int IsValidBasicCredentialCacheHashSet = 1007013;
      internal const int IsValidBasicCredentialCacheHashInvalidated = 1007014;
      internal const int IsValidBasicCredentialValid = 1007006;
      internal const int IsValidBasicCredentialNotValid = 1007008;
      internal const int SetBasicCredentialEnter = 1007010;
      internal const int SetBasicCredentialsError = 1007018;
      internal const int SetBasicCredentialLeave = 1007019;
      internal const int DeleteBasicCredentialEnter = 1007020;
      internal const int DeleteBasicCredentialsSuccess = 1007022;
      internal const int DeleteBasicCredentialLeave = 1007029;
      internal const int IsBasicAuthDisabledEnter = 1007040;
      internal const int IsBasicAuthDisabledLeave = 1007041;
      internal const int HasBasicCredentialEnter = 1007042;
      internal const int HasBasicCredentialLeave = 1007043;
      internal const int EnableDisabledAccountEnter = 1007050;
      internal const int EnableDisabledAccountLeave = 1007051;
      internal const int EnableDisabledAccountError = 1007052;
    }

    internal static class FrameworkBasicAuthCacheTracePoints
    {
      internal const int TrySetHashRedisException = 1007053;
      internal const int TryInvalidateHashRedisException = 1007054;
      internal const int TryGetHashRedisException = 1007055;
    }

    internal static class BasicAuthenticationModuleTracepoints
    {
      internal const int OnAuthenticatRequestException = 1007060;
      internal const int MultipleIdentitiesFoundForUserName = 1007061;
      internal const int BasicAuthDisabled = 1007062;
      internal const int BasicAuthPolicyException = 1007063;

      internal static class VerifyDelegatedAuthToken
      {
        internal const int Enter = 1048200;
        internal const int FeatureNotEnabled = 1048201;
        internal const int UserNamePasswordMismatch = 1048202;
        internal const int AuthenticatingAsOAuth = 1048203;
        internal const int Exception = 1048204;
        internal const int InvalidSessionToken = 1048205;
        internal const int Leave = 1048206;
      }

      internal static class GetDelegatedAuthTokenFromCompactToken
      {
        internal const int StrongboxDrawerNotFound = 1048206;
        internal const int TokenNotFound = 1048207;
        internal const int Exchange = 1048208;
        internal const int AccessTokenNull = 1048209;
      }
    }

    internal static class EligibleActorTracePoints
    {
      internal const int IdentityNullValue = 1007070;
      internal const int IdentityDescriptorNullValue = 1007071;
      internal const int IdentityDescriptorByMasterIdNullValue = 1007072;
      internal const int ResolvedEligibleActorNullValue = 1007073;
    }
  }
}
