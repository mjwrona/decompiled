// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Interop.SafeEventLogWriteHandle
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Azure.NotificationHubs.Common.Interop
{
  [SecurityCritical]
  internal sealed class SafeEventLogWriteHandle : SafeHandleZeroOrMinusOneIsInvalid
  {
    [SecurityCritical]
    private SafeEventLogWriteHandle()
      : base(true)
    {
    }

    [SecurityCritical]
    public static SafeEventLogWriteHandle RegisterEventSource(
      string uncServerName,
      string sourceName)
    {
      SafeEventLogWriteHandle eventLogWriteHandle = UnsafeNativeMethods.RegisterEventSource(uncServerName, sourceName);
      Marshal.GetLastWin32Error();
      int num = eventLogWriteHandle.IsInvalid ? 1 : 0;
      return eventLogWriteHandle;
    }

    [DllImport("advapi32", SetLastError = true)]
    private static extern bool DeregisterEventSource(IntPtr hEventLog);

    [SecurityCritical]
    protected override bool ReleaseHandle() => SafeEventLogWriteHandle.DeregisterEventSource(this.handle);
  }
}
