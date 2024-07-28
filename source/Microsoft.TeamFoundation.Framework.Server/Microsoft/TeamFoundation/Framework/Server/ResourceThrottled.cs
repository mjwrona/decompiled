// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ResourceThrottled
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Flags]
  internal enum ResourceThrottled
  {
    PhysicalDatabaseSpaceSoftThrottle = 1,
    PhysicalDatabaseSpaceHardThrottle = 2,
    PhysicalLogSpaceSoftThrottle = 4,
    PhysicalLogSpaceHardThrottle = 8,
    LogWriteIODelaySoftThrottle = 16, // 0x00000010
    LogWriteIODelayHardThrottle = 32, // 0x00000020
    DataReadIODelaySoftThrottle = 64, // 0x00000040
    DataReadIODelayHardThrottle = 128, // 0x00000080
    CPUSoftThrottle = 256, // 0x00000100
    CPUHardThrottle = 512, // 0x00000200
    DatabaseSizeSoftThrottle = 1024, // 0x00000400
    DatabaseSizeHardThrottle = 2048, // 0x00000800
    InternalSoftThrottle = 4096, // 0x00001000
    InternalHardThrottle = 8192, // 0x00002000
    SQLWorkerThreadsSoftThrottle = 16384, // 0x00004000
    SQLWorkerThreadsHardThrottle = 32768, // 0x00008000
    Internal2SoftThrottle = 65536, // 0x00010000
    Internal2HardThrottle = 131072, // 0x00020000
  }
}
