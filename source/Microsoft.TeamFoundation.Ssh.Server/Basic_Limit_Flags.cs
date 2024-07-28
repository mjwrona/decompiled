// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Basic_Limit_Flags
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using System;

namespace Microsoft.TeamFoundation.Ssh.Server
{
  [Flags]
  internal enum Basic_Limit_Flags : uint
  {
    JOB_OBJECT_LIMIT_ACTIVE_PROCESS = 8,
    JOB_OBJECT_LIMIT_AFFINITY = 16, // 0x00000010
    JOB_OBJECT_LIMIT_BREAKAWAY_OK = 2048, // 0x00000800
    JOB_OBJECT_LIMIT_DIE_ON_UNHANDLED_EXCEPTION = 1024, // 0x00000400
    JOB_OBJECT_LIMIT_JOB_MEMORY = 512, // 0x00000200
    JOB_OBJECT_LIMIT_JOB_TIME = 4,
    JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 8192, // 0x00002000
    JOB_OBJECT_LIMIT_PRESERVE_JOB_TIME = 64, // 0x00000040
    JOB_OBJECT_LIMIT_PRIORITY_CLASS = 32, // 0x00000020
    JOB_OBJECT_LIMIT_PROCESS_MEMORY = 256, // 0x00000100
    JOB_OBJECT_LIMIT_PROCESS_TIME = 2,
    JOB_OBJECT_LIMIT_SCHEDULING_CLASS = 128, // 0x00000080
    JOB_OBJECT_LIMIT_SILENT_BREAKAWAY_OK = 4096, // 0x00001000
    JOB_OBJECT_LIMIT_SUBSET_AFFINITY = 16384, // 0x00004000
    JOB_OBJECT_LIMIT_WORKINGSET = 1,
  }
}
