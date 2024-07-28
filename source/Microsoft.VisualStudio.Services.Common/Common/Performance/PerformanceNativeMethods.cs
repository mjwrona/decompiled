// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Performance.PerformanceNativeMethods
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Common.Performance
{
  public class PerformanceNativeMethods
  {
    [DllImport("kernel32")]
    internal static extern int GetCurrentThreadId();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern PerformanceNativeMethods.SafeThreadHandle OpenThread(
      int access,
      bool inherit,
      int threadId);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool QueryThreadCycleTime(
      PerformanceNativeMethods.SafeThreadHandle threadHandle,
      out ulong cycleTime);

    public static long GetCPUTime()
    {
      ulong cycleTime = 0;
      try
      {
        using (PerformanceNativeMethods.SafeThreadHandle threadHandle = PerformanceNativeMethods.OpenThread(64, false, PerformanceNativeMethods.GetCurrentThreadId()))
        {
          if (!threadHandle.IsInvalid)
            PerformanceNativeMethods.QueryThreadCycleTime(threadHandle, out cycleTime);
        }
      }
      catch (Exception ex)
      {
      }
      return (long) cycleTime;
    }

    internal sealed class SafeThreadHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
      internal SafeThreadHandle()
        : base(true)
      {
      }

      internal void InitialSetHandle(IntPtr h) => this.SetHandle(h);

      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      internal static extern bool CloseHandle(IntPtr handle);

      protected override bool ReleaseHandle() => PerformanceNativeMethods.SafeThreadHandle.CloseHandle(this.handle);
    }
  }
}
