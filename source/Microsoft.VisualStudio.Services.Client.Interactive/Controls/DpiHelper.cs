// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.DpiHelper
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  internal static class DpiHelper
  {
    internal const double LogicalDpi = 96.0;

    static DpiHelper()
    {
      IntPtr dc = ClientNativeMethods.GetDC(IntPtr.Zero);
      if (dc != IntPtr.Zero)
      {
        DpiHelper.DeviceDpiX = (double) ClientNativeMethods.GetDeviceCaps(dc, 88);
        ClientNativeMethods.ReleaseDC(IntPtr.Zero, dc);
      }
      else
        DpiHelper.DeviceDpiX = 96.0;
    }

    internal static double DeviceDpiX { get; set; }
  }
}
