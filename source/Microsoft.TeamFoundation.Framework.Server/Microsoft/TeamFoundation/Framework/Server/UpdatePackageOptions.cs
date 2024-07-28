// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UpdatePackageOptions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Flags]
  public enum UpdatePackageOptions
  {
    None = 0,
    TestPatch = 1,
    Eval = 256, // 0x00000100
    GoLive = 512, // 0x00000200
    RTM = 1024, // 0x00000400
  }
}
