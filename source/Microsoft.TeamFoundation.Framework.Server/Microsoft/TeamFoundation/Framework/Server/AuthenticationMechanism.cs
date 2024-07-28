// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuthenticationMechanism
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public enum AuthenticationMechanism
  {
    None,
    Basic,
    Basic_SessionToken,
    Basic_AAD,
    Basic_AADServicePrincipal,
    PAT,
    PAT_Unscoped,
    PAT_Global,
    PAT_UnscopedGlobal,
    FedAuth,
    S2S_Legacy,
    S2S_FirstParty,
    S2S_ServicePrincipal,
    SSH,
    AAD,
    AADServicePrincipal,
    AAD_Web,
    AAD_Scoped,
    AAD_Ibiza,
    AAD_ARM,
    Test,
    SWT,
    Oauth,
    SessionToken,
    SessionToken_Unscoped,
    SessionToken_Global,
    SessionToken_UnscopedGlobal,
    UserAuthToken,
    UserAuthToken_VS2012,
    AAD_Cookie,
    DSTS,
    AAD_PoP,
    DSTS_Web,
  }
}
