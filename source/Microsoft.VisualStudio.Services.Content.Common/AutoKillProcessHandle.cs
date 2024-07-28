// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.AutoKillProcessHandle
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class AutoKillProcessHandle : IDisposable
  {
    public readonly Process Process;
    private readonly AutoKillProcessHandle.JobHandle jobHandle;

    public AutoKillProcessHandle(Process processToKill, string jobName)
    {
      this.Process = processToKill;
      this.jobHandle = AutoKillProcessHandle.CreateJobObject(IntPtr.Zero, jobName);
      if (this.jobHandle.IsInvalid)
        throw new Win32Exception();
      AutoKillProcessHandle.JOBOBJECT_EXTENDED_LIMIT_INFORMATION lpJobObjectInfo = new AutoKillProcessHandle.JOBOBJECT_EXTENDED_LIMIT_INFORMATION()
      {
        BasicLimitInformation = new AutoKillProcessHandle.JOBOBJECT_BASIC_LIMIT_INFORMATION()
        {
          LimitFlags = 8192
        }
      };
      if (!AutoKillProcessHandle.SetExtendedInformationJobObject(this.jobHandle, AutoKillProcessHandle.JOBOBJECTINFOCLASS.JobObjectExtendedLimitInformation, ref lpJobObjectInfo, (uint) Marshal.SizeOf<AutoKillProcessHandle.JOBOBJECT_EXTENDED_LIMIT_INFORMATION>(lpJobObjectInfo)))
        throw new Win32Exception(Marshal.GetLastWin32Error());
      if (!AutoKillProcessHandle.AssignProcessToJobObject(this.jobHandle, processToKill.Handle))
        throw new Win32Exception();
    }

    public void Dispose() => this.jobHandle.Dispose();

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern AutoKillProcessHandle.JobHandle CreateJobObject(
      [In] IntPtr lpJobAttributes,
      string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AssignProcessToJobObject(
      AutoKillProcessHandle.JobHandle hJob,
      IntPtr hProcess);

    [DllImport("kernel32.dll", EntryPoint = "SetInformationJobObject", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetExtendedInformationJobObject(
      AutoKillProcessHandle.JobHandle hJob,
      AutoKillProcessHandle.JOBOBJECTINFOCLASS JobObjectInfoClass,
      ref AutoKillProcessHandle.JOBOBJECT_EXTENDED_LIMIT_INFORMATION lpJobObjectInfo,
      uint cbJobObjectInfoLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hHandle);

    private enum JOBOBJECTINFOCLASS
    {
      JobObjectBasicAccountingInformation = 1,
      JobObjectBasicLimitInformation = 2,
      JobObjectBasicProcessIdList = 3,
      JobObjectBasicUIRestrictions = 4,
      JobObjectSecurityLimitInformation = 5,
      JobObjectEndOfJobTimeInformation = 6,
      JobObjectAssociateCompletionPortInformation = 7,
      JobObjectBasicAndIoAccountingInformation = 8,
      JobObjectExtendedLimitInformation = 9,
      JobObjectJobSetInformation = 10, // 0x0000000A
      MaxJobObjectInfoClass = 11, // 0x0000000B
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
      public long PerProcessUserTimeLimit;
      public long PerJobUserTimeLimit;
      public uint LimitFlags;
      public UIntPtr MinimumWorkingSetSize;
      public UIntPtr MaximumWorkingSetSize;
      public uint ActiveProcessLimit;
      public UIntPtr Affinity;
      public uint PriorityClass;
      public uint SchedulingClass;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct IO_COUNTERS
    {
      public ulong ReadOperationCount;
      public ulong WriteOperationCount;
      public ulong OtherOperationCount;
      public ulong ReadTransferCount;
      public ulong WriteTransferCount;
      public ulong OtherTransferCount;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
      public AutoKillProcessHandle.JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
      public AutoKillProcessHandle.IO_COUNTERS IoInfo;
      public UIntPtr ProcessMemoryLimit;
      public UIntPtr JobMemoryLimit;
      public UIntPtr PeakProcessMemoryUsed;
      public UIntPtr PeakJobMemoryUsed;
    }

    private sealed class JobHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
      private JobHandle()
        : base(true)
      {
      }

      protected override bool ReleaseHandle() => AutoKillProcessHandle.CloseHandle(this.handle);
    }
  }
}
