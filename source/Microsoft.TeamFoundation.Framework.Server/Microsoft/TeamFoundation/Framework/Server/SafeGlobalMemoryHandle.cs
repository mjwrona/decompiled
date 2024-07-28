// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SafeGlobalMemoryHandle
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SafeGlobalMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
  {
    internal SafeGlobalMemoryHandle()
      : base(true)
    {
    }

    internal SafeGlobalMemoryHandle(IntPtr existingHandle, bool ownsHandle)
      : base(ownsHandle)
    {
      this.SetHandle(existingHandle);
    }

    internal SafeGlobalMemoryHandle(uint sizeInBytes)
      : base(true)
    {
      this.SetHandle(Marshal.AllocHGlobal((int) sizeInBytes));
    }

    protected override bool ReleaseHandle()
    {
      if (!(this.handle != IntPtr.Zero))
        return false;
      Marshal.FreeHGlobal(this.handle);
      return true;
    }
  }
}
