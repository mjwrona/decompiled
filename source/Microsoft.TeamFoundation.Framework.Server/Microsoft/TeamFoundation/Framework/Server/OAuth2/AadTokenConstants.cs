// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.AadTokenConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal static class AadTokenConstants
  {
    public const string AltSecIdClaimPrefixForMsaUser = "1:live.com:";
    public const string AltSecIdClaimPrefixForCspPartnerUser = "5::";
    public const string LiveIdUniqueNameClaimPrefix = "live.com#";
    public const string LiveIdentityProviderClaimValue = "uri:WindowsLiveId";
    public const string AADIdentityProviderClaimValue = "uri:MicrosoftOnline";
    public const string LiveIdentityProvider = "Live.com";
    public const string OneTimePasswordIdentityProvider = "mail";
    public const string OneTimePasswordUniqueNameClaimPrefix = "mail#";
    public const string IdTypAppType = "app";
  }
}
