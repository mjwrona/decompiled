// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.JobHandle
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Ssh.Server
{
  internal sealed class JobHandle : SafeHandle
  {
    private const string s_Area = "Ssh";
    private const string s_Layer = "JobHandle";

    public JobHandle(string jobName)
      : base(IntPtr.Zero, true)
    {
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.SECURITY_ATTRIBUTES lpJobAttributes = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.SECURITY_ATTRIBUTES()
      {
        bInheritHandle = true,
        lpSecurityDescriptor = IntPtr.Zero,
        nLength = Marshal.SizeOf(typeof (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SECURITY_ATTRIBUTES))
      };
      this.SetHandle(Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreateJobObject(ref lpJobAttributes, jobName));
    }

    public bool AddProcess(Process process)
    {
      using (JobHandle.ProcessHandle hProcess = new JobHandle.ProcessHandle(process.Id))
      {
        if (!hProcess.IsInvalid)
        {
          if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.AssignProcessToJobObject((SafeHandle) this, (SafeHandle) hProcess))
            return true;
        }
      }
      return false;
    }

    protected override bool ReleaseHandle() => Microsoft.TeamFoundation.Common.Internal.NativeMethods.CloseHandle(this.handle);

    public void SetMemoryRateControlHardCap(int memoryMB)
    {
      TeamFoundationTracingService.TraceEnterRaw(13000260, "Ssh", nameof (JobHandle), nameof (SetMemoryRateControlHardCap));
      if (memoryMB < 0)
        TeamFoundationTracingService.TraceRaw(13000264, TraceLevel.Info, "Ssh", nameof (JobHandle), "Skipping SetMemoryRateControlHardCap because the currently set limit is a negative value indicating we shouldn't set memory limits.");
      else if (memoryMB < 1000)
      {
        TeamFoundationTracingService.TraceRaw(13000263, TraceLevel.Info, "Ssh", nameof (JobHandle), "Skipping SetMemoryRateControlHardCap because the registry limit is less than 1000MB which is too low.");
      }
      else
      {
        ProcessJobObject.JOBOBJECT_EXTENDED_LIMIT_INFORMATION info = new ProcessJobObject.JOBOBJECT_EXTENDED_LIMIT_INFORMATION()
        {
          BasicLimitInformation = new ProcessJobObject.JOBOBJECT_BASIC_LIMIT_INFORMATION()
          {
            LimitFlags = Basic_Limit_Flags.JOB_OBJECT_LIMIT_BREAKAWAY_OK | Basic_Limit_Flags.JOB_OBJECT_LIMIT_PROCESS_MEMORY | Basic_Limit_Flags.JOB_OBJECT_LIMIT_SILENT_BREAKAWAY_OK
          },
          ProcessMemoryLimit = (UIntPtr) (ulong) (memoryMB * 1048576)
        };
        int infoLength = Marshal.SizeOf<ProcessJobObject.JOBOBJECT_EXTENDED_LIMIT_INFORMATION>(info);
        TeamFoundationTracingService.TraceRaw(13000261, TraceLevel.Verbose, "Ssh", nameof (JobHandle), string.Format("Setting Max memory hard limit to {0}", (object) info.ProcessMemoryLimit));
        if (!SshServerNativeMethods.SetInformationJobObject(this, JobObjectInfoClass.ExtendedLimitInformation, ref info, infoLength))
          TeamFoundationTracingService.TraceRaw(13000262, TraceLevel.Error, "Ssh", nameof (JobHandle), string.Format("Error setting Max Memory limit. Last Win32 error was {0}", (object) Marshal.GetLastWin32Error()));
      }
      TeamFoundationTracingService.TraceLeaveRaw(13000265, "Ssh", nameof (JobHandle), nameof (SetMemoryRateControlHardCap));
    }

    public void SetCpuRateControlHardCap(short cpuRateInPercentage)
    {
      TeamFoundationTracingService.TraceEnterRaw(13000240, "Ssh", nameof (JobHandle), nameof (SetCpuRateControlHardCap));
      ProcessJobObject.JOBOBJECT_CPU_RATE_CONTROL_INFORMATION controlInformation = cpuRateInPercentage > (short) 0 && cpuRateInPercentage <= (short) 100 ? new ProcessJobObject.JOBOBJECT_CPU_RATE_CONTROL_INFORMATION()
      {
        ControlFlags = CpuCapControlFlags.CpuRateControlEnable | CpuCapControlFlags.CpuRateControlHardCap,
        CpuRate = (int) cpuRateInPercentage * 100
      } : throw new ArgumentOutOfRangeException(string.Format("CpuRateLimit is {0}. It should be between 0 and 100.", (object) cpuRateInPercentage));
      ProcessJobObject.CpuJobObjectInfo jobObjectInfo = new ProcessJobObject.CpuJobObjectInfo()
      {
        CpuRateControl = controlInformation
      };
      int jobObjectInfoLength = Marshal.SizeOf(typeof (ProcessJobObject.JOBOBJECT_CPU_RATE_CONTROL_INFORMATION));
      TeamFoundationTracingService.TraceRaw(13000241, TraceLevel.Verbose, "Ssh", nameof (JobHandle), string.Format("Setting Max CPU hard limit to {0}", (object) controlInformation.CpuRate));
      if (!SshServerNativeMethods.SetInformationJobObject(this, JobObjectInfoClass.CpuRateControlInformation, ref jobObjectInfo, jobObjectInfoLength))
        TeamFoundationTracingService.TraceRaw(13000242, TraceLevel.Error, "Ssh", nameof (JobHandle), string.Format("Error setting Max Cpu hard limit. Last Win32 error was {0}", (object) Marshal.GetLastWin32Error()));
      TeamFoundationTracingService.TraceLeaveRaw(13000245, "Ssh", nameof (JobHandle), nameof (SetCpuRateControlHardCap));
    }

    public override bool IsInvalid => this.IsClosed || this.handle == IntPtr.Zero;

    private class ProcessHandle : SafeHandle
    {
      private const uint PROCESS_ALL_ACCESS = 2035711;

      public ProcessHandle(int processId)
        : base(new IntPtr(-1), true)
      {
        this.SetHandle(Microsoft.TeamFoundation.Common.Internal.NativeMethods.OpenProcess(2035711U, false, (uint) processId));
      }

      protected override bool ReleaseHandle() => Microsoft.TeamFoundation.Common.Internal.NativeMethods.CloseHandle(this.handle);

      public override bool IsInvalid => this.IsClosed || this.handle == new IntPtr(-1);
    }
  }
}
