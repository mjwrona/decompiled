// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.SafeWerReportHandle
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Telemetry.WindowsErrorReporting
{
  public class SafeWerReportHandle : SafeHandleZeroOrMinusOneIsInvalid
  {
    public SafeWerReportHandle()
      : base(true)
    {
    }

    public SafeWerReportHandle(IntPtr handle)
      : base(true)
    {
      this.SetHandle(handle);
    }

    protected override bool ReleaseHandle() => SafeWerReportHandle.WerReportCloseHandle(this.handle) == 0;

    [DllImport("wer.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int WerReportCloseHandle(IntPtr phReportHandle);
  }
}
