// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ImsOperation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  [Flags]
  internal enum ImsOperation
  {
    None = 0,
    IdentitiesByDescriptor = 1,
    IdentitiesByDisplayName = 2,
    Children = 4,
    Descendants = 8,
    IdentitiesInScope = 32, // 0x00000020
    IdentitiesByAccountName = 64, // 0x00000040
    IdentitiesById = 128, // 0x00000080
    IdentityIdsByDisplayNamePrefixSearch = 256, // 0x00000100
    IdentityIdsByEmailPrefixSearch = 512, // 0x00000200
    IdentityIdsByAccountNamePrefixSearch = 1024, // 0x00000400
    MruIdentityIds = 2048, // 0x00000800
    IdentityIdsByDomainAccountNamePrefixSearch = 4096, // 0x00001000
    IdentityIdsByAppIdSearch = 8192, // 0x00002000
  }
}
