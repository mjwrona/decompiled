// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.OAuth2TokenValidators
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  [Flags]
  public enum OAuth2TokenValidators
  {
    None = 0,
    AAD = 1,
    S2S = 2,
    DelegatedAuth = 4,
    HostAuthentication = 8,
    UserAuthentication = 16, // 0x00000010
    AADServicePrincipal = 32, // 0x00000020
    AADCookie = 64, // 0x00000040
    UserAuthentication_VS2012 = 128, // 0x00000080
    S2S_ARR = 256, // 0x00000100
  }
}
