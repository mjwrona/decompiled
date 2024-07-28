// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.CpuCapControlFlags
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using System;

namespace Microsoft.TeamFoundation.Ssh.Server
{
  [Flags]
  internal enum CpuCapControlFlags
  {
    CpuRateControlEnable = 1,
    CpuRateControlWeightBased = 2,
    CpuRateControlHardCap = 4,
    CpuRateControlNotify = 8,
    CpuRateControlMinMaxRate = 16, // 0x00000010
  }
}
