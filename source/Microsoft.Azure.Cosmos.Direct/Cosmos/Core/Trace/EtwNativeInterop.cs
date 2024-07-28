// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Core.Trace.EtwNativeInterop
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Core.Trace
{
  internal static class EtwNativeInterop
  {
    [DllImport("advapi32.dll")]
    internal static extern uint EventRegister(
      in Guid providerId,
      IntPtr enableCallback,
      IntPtr callbackContext,
      ref EtwNativeInterop.ProviderHandle registrationHandle);

    [DllImport("advapi32.dll")]
    internal static extern uint EventUnregister(IntPtr registrationHandle);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    public static extern uint EventWriteString(
      EtwNativeInterop.ProviderHandle registrationHandle,
      byte level,
      long keywords,
      string message);

    internal class ProviderHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
      public ProviderHandle()
        : base(true)
      {
      }

      protected override bool ReleaseHandle() => EtwNativeInterop.EventUnregister(this.handle) == 0U;
    }
  }
}
