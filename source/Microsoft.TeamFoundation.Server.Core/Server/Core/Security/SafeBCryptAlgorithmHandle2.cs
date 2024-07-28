// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Security.SafeBCryptAlgorithmHandle2
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
  internal sealed class SafeBCryptAlgorithmHandle2 : SafeHandleZeroOrMinusOneIsInvalid
  {
    private SafeBCryptAlgorithmHandle2()
      : base(true)
    {
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [SuppressUnmanagedCodeSecurity]
    [DllImport("bcrypt.dll")]
    private static extern BCryptNative2.ErrorCode BCryptCloseAlgorithmProvider(
      IntPtr hAlgorithm,
      int flags);

    protected override bool ReleaseHandle() => SafeBCryptAlgorithmHandle2.BCryptCloseAlgorithmProvider(this.handle, 0) == BCryptNative2.ErrorCode.Success;
  }
}
