// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.ServiceHandle
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Security.Permissions;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
  public sealed class ServiceHandle : SafeHandleZeroOrMinusOneIsInvalid
  {
    internal ServiceHandle()
      : base(true)
    {
    }

    protected override bool ReleaseHandle()
    {
      if (!(this.handle != IntPtr.Zero))
        return false;
      int num = NativeMethods.CloseServiceHandle(this.handle) ? 1 : 0;
      this.handle = IntPtr.Zero;
      return num != 0;
    }
  }
}
