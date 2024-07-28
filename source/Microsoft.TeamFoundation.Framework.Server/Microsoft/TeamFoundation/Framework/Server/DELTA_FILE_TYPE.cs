// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DELTA_FILE_TYPE
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Flags]
  internal enum DELTA_FILE_TYPE : long
  {
    RAW = 1,
    I386 = 2,
    IA64 = 4,
    AMD64 = 8,
    SET_RAW_ONLY = RAW, // 0x0000000000000001
    SET_EXECUTABLES = SET_RAW_ONLY | AMD64 | IA64 | I386, // 0x000000000000000F
  }
}
