// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySearchKind
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  [Flags]
  public enum IdentitySearchKind
  {
    None = 0,
    DisplayNamePrefix = 1,
    EmailPrefix = 2,
    AccountNamePrefix = 4,
    DomainAccountNamePrefix = 8,
    AppId = 16, // 0x00000010
  }
}
