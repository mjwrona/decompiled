// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobDefinitionFlags
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Flags]
  internal enum TeamFoundationJobDefinitionFlags
  {
    None = 0,
    RunOnce = 1,
    IgnoreDormancy = 2,
    AllowDelete = 4,
    DisableDuringUpgrade = 8,
    UseServicingContext = 16, // 0x00000010
    __Reserved1__ = 32, // 0x00000020
    SelfService = 64, // 0x00000040
    __Reserved2__ = 128, // 0x00000080
  }
}
