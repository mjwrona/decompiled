// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.NoPumpSyncContext
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry.WindowsErrorReporting
{
  internal class NoPumpSyncContext : SynchronizationContext
  {
    private static readonly SynchronizationContext DefaultInstance = (SynchronizationContext) new NoPumpSyncContext();

    public NoPumpSyncContext() => this.SetWaitNotificationRequired();

    public static SynchronizationContext Default => NoPumpSyncContext.DefaultInstance;

    public override int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout) => NativeMethods.WaitForMultipleObjects((uint) waitHandles.Length, waitHandles, waitAll, (uint) millisecondsTimeout);
  }
}
