// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Security.SafeBCryptHashHandle2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.TeamFoundation.Server.Core.Security
{
  [SecurityCritical(SecurityCriticalScope.Everything)]
  internal sealed class SafeBCryptHashHandle2 : SafeHandleZeroOrMinusOneIsInvalid
  {
    private IntPtr m_hashObject;

    private SafeBCryptHashHandle2()
      : base(true)
    {
    }

    internal IntPtr HashObject
    {
      get => this.m_hashObject;
      set => this.m_hashObject = value;
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [SuppressUnmanagedCodeSecurity]
    [DllImport("bcrypt.dll")]
    private static extern BCryptNative2.ErrorCode BCryptDestroyHash(IntPtr hHash);

    protected override bool ReleaseHandle()
    {
      int num = SafeBCryptHashHandle2.BCryptDestroyHash(this.handle) == BCryptNative2.ErrorCode.Success ? 1 : 0;
      if (!(this.m_hashObject != IntPtr.Zero))
        return num != 0;
      Marshal.FreeCoTaskMem(this.m_hashObject);
      return num != 0;
    }
  }
}
