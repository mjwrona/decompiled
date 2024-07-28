// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.ServiceControlManagerUtility
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.TeamFoundation.Common
{
  public class ServiceControlManagerUtility : IDisposable
  {
    private ServiceHandle m_manager;

    [SecurityCritical]
    public ServiceControlManagerUtility()
    {
      this.m_manager = Microsoft.TeamFoundation.Common.Internal.NativeMethods.OpenSCManager((string) null, (string) null, Microsoft.TeamFoundation.Common.Internal.NativeMethods.ServiceControlAccessRights.SC_MANAGER_CONNECT);
      if (this.m_manager.IsInvalid)
        throw new Win32Exception(Marshal.GetLastWin32Error(), TFCommonResources.ServiceControlManagerOpenError());
    }

    private ServiceHandle OpenService(
      string serviceName,
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.ServiceAccessRights desiredAccess)
    {
      ServiceHandle serviceHandle = Microsoft.TeamFoundation.Common.Internal.NativeMethods.OpenService(this.m_manager, serviceName, desiredAccess);
      return !serviceHandle.IsInvalid ? serviceHandle : throw new Win32Exception(Marshal.GetLastWin32Error(), TFCommonResources.ServiceOpenError((object) serviceName));
    }

    public void SetRestartsOnFailure(string serviceName, int resetDays, int delayMinutes) => this.SetRestartsOnFailure(serviceName, resetDays, delayMinutes, false);

    public void SetRestartsOnFailure(
      string serviceName,
      int resetDays,
      int delayMinutes,
      bool restartOnNonCrashFailures)
    {
      this.SetRestartActions(serviceName, 3, resetDays, TimeSpan.FromMinutes((double) delayMinutes), restartOnNonCrashFailures);
    }

    public void SetRestartsOnFailure(
      string serviceName,
      int resetDays,
      TimeSpan delay,
      bool restartOnNonCrashFailures)
    {
      this.SetRestartActions(serviceName, 3, resetDays, delay, restartOnNonCrashFailures);
    }

    public void RemoveRecoveryActions(string serviceName) => this.SetRestartActions(serviceName, 0, 0, TimeSpan.Zero, false);

    [SecurityCritical]
    private void SetRestartActions(
      string serviceName,
      int restartActionsCount,
      int resetDays,
      TimeSpan delay,
      bool restartOnNonCrashFailures)
    {
      uint totalMilliseconds = (uint) delay.TotalMilliseconds;
      uint num1 = (uint) (resetDays * 24 * 60 * 60);
      ServiceHandle serviceHandle = (ServiceHandle) null;
      IntPtr num2 = IntPtr.Zero;
      IntPtr num3 = IntPtr.Zero;
      IntPtr hglobal = IntPtr.Zero;
      try
      {
        serviceHandle = this.OpenService(serviceName, Microsoft.TeamFoundation.Common.Internal.NativeMethods.ServiceAccessRights.SERVICE_CHANGE_CONFIG | Microsoft.TeamFoundation.Common.Internal.NativeMethods.ServiceAccessRights.SERVICE_START);
        hglobal = Marshal.AllocHGlobal(checked (Marshal.SizeOf(typeof (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SC_ACTION)) * restartActionsCount));
        for (int index = 0; index < restartActionsCount; ++index)
          Marshal.StructureToPtr<Microsoft.TeamFoundation.Common.Internal.NativeMethods.SC_ACTION>(new Microsoft.TeamFoundation.Common.Internal.NativeMethods.SC_ACTION()
          {
            Type = Microsoft.TeamFoundation.Common.Internal.NativeMethods.SC_ACTION_TYPE.SC_ACTION_RESTART,
            Delay = totalMilliseconds
          }, (IntPtr) ((long) hglobal + (long) (index * Marshal.SizeOf(typeof (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SC_ACTION)))), false);
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.SERVICE_FAILURE_ACTIONS structure1 = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.SERVICE_FAILURE_ACTIONS();
        structure1.dwResetPeriod = num1;
        structure1.cActions = (uint) restartActionsCount;
        structure1.lpsaActions = hglobal;
        structure1.lpRebootMsg = (string) null;
        structure1.lpCommand = (string) null;
        num2 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SERVICE_FAILURE_ACTIONS)));
        Marshal.StructureToPtr<Microsoft.TeamFoundation.Common.Internal.NativeMethods.SERVICE_FAILURE_ACTIONS>(structure1, num2, false);
        if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.ChangeServiceConfig2(serviceHandle, Microsoft.TeamFoundation.Common.Internal.NativeMethods.ServiceConfig2InfoLevel.SERVICE_CONFIG_FAILURE_ACTIONS, num2) == 0)
          throw new Win32Exception(Marshal.GetLastWin32Error(), TFCommonResources.ServiceChangeError((object) serviceName));
        if (!restartOnNonCrashFailures)
          return;
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.SERVICE_FAILURE_ACTIONS_FLAG structure2 = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.SERVICE_FAILURE_ACTIONS_FLAG();
        structure2.FailureActionsOnNonCrashFailures = true;
        num3 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SERVICE_FAILURE_ACTIONS_FLAG)));
        Marshal.StructureToPtr<Microsoft.TeamFoundation.Common.Internal.NativeMethods.SERVICE_FAILURE_ACTIONS_FLAG>(structure2, num3, false);
        if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.ChangeServiceConfig2(serviceHandle, Microsoft.TeamFoundation.Common.Internal.NativeMethods.ServiceConfig2InfoLevel.SERVICE_CONFIG_FAILURE_ACTIONS_FLAG, num3) == 0)
          throw new Win32Exception(Marshal.GetLastWin32Error(), TFCommonResources.ServiceChangeError((object) serviceName));
      }
      finally
      {
        if (num2 != IntPtr.Zero)
          Marshal.FreeHGlobal(num2);
        if (hglobal != IntPtr.Zero)
          Marshal.FreeHGlobal(hglobal);
        if (num3 != IntPtr.Zero)
          Marshal.FreeHGlobal(num3);
        serviceHandle?.Dispose();
      }
    }

    public void Dispose()
    {
      if (this.m_manager == null)
        return;
      this.m_manager.Dispose();
      this.m_manager = (ServiceHandle) null;
    }
  }
}
