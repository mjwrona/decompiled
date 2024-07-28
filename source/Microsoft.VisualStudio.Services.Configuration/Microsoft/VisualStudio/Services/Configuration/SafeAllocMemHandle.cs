// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SafeAllocMemHandle
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
  internal class SafeAllocMemHandle : SafeHandleZeroOrMinusOneIsInvalid
  {
    private SafeAllocMemHandle()
      : base(true)
    {
    }

    internal SafeAllocMemHandle(IntPtr ptr)
      : base(true)
    {
      this.SetHandle(ptr);
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    protected override bool ReleaseHandle()
    {
      if (this.handle != IntPtr.Zero)
      {
        Marshal.FreeCoTaskMem(this.handle);
        this.handle = IntPtr.Zero;
      }
      return true;
    }
  }
}
