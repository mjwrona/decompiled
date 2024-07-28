// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.ProcessJobObject
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Ssh.Server
{
  internal class ProcessJobObject
  {
    [StructLayout(LayoutKind.Explicit)]
    internal struct CpuJobObjectInfo
    {
      [FieldOffset(0)]
      internal ProcessJobObject.JOBOBJECT_CPU_RATE_CONTROL_INFORMATION CpuRateControl;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct JOBOBJECT_CPU_RATE_CONTROL_INFORMATION
    {
      [FieldOffset(0)]
      internal CpuCapControlFlags ControlFlags;
      [FieldOffset(4)]
      internal int CpuRate;
      [FieldOffset(4)]
      internal int Weight;
    }

    internal struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
      internal ulong PerProcessUserTimeLimit;
      internal ulong PerJobUserTimeLimit;
      internal Basic_Limit_Flags LimitFlags;
      internal UIntPtr MinimumWorkingSetSize;
      internal UIntPtr MaximumWorkingSetSize;
      internal uint ActiveProcessLimit;
      internal IntPtr Affinity;
      internal uint PriorityClass;
      internal uint SchedulingClass;
    }

    internal struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
      internal ProcessJobObject.JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
      internal ProcessJobObject.IO_COUNTERS IoInfo;
      internal UIntPtr ProcessMemoryLimit;
      internal UIntPtr JobMemoryLimit;
      internal UIntPtr PeakProcessMemoryUsed;
      internal UIntPtr PeakJobMemoryUsed;
    }

    internal struct IO_COUNTERS
    {
      internal ulong ReadOperationCount;
      internal ulong WriteOperationCount;
      internal ulong OtherOperationCount;
      internal ulong ReadTransferCount;
      internal ulong WriteTransferCount;
      internal ulong OtherTransferCount;
    }
  }
}
