// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.AuthenticationTracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  internal static class AuthenticationTracePoints
  {
    internal const int UpgradeFromFedAuthToUserAuthToken = 17000;
    internal const int IssueUserAuthSessionToken = 17001;
    internal const int PreserveValidFromBetweenTokenRenewals = 17002;
    internal const int IssueUserAuthTokenFromCurentTimestamp = 17003;
    internal const int UseValidToPropertyForShouldRefreshCheck = 17004;
    internal const int UseValidFromPropertyForShouldRefreshCheck = 17005;
    internal const int OAuthTokenInvalidAppIdClaim = 17006;
  }
}
