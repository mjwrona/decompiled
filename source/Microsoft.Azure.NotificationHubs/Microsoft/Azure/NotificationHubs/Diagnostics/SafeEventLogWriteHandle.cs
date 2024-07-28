// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.SafeEventLogWriteHandle
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal sealed class SafeEventLogWriteHandle : SafeHandleZeroOrMinusOneIsInvalid
  {
    private SafeEventLogWriteHandle()
      : base(true)
    {
    }

    internal static SafeEventLogWriteHandle RegisterEventSource(
      string uncServerName,
      string sourceName)
    {
      SafeEventLogWriteHandle eventLogWriteHandle = NativeMethods.RegisterEventSource(uncServerName, sourceName);
      Marshal.GetLastWin32Error();
      int num = eventLogWriteHandle.IsInvalid ? 1 : 0;
      return eventLogWriteHandle;
    }

    [DllImport("advapi32", SetLastError = true)]
    private static extern bool DeregisterEventSource(IntPtr hEventLog);

    protected override bool ReleaseHandle() => SafeEventLogWriteHandle.DeregisterEventSource(this.handle);
  }
}
