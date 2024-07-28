// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Threading.VssScheduler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server.Threading
{
  internal abstract class VssScheduler : IVssScheduler
  {
    [ThreadStatic]
    private static VssScheduler s_current;

    public static VssScheduler Current
    {
      get => VssScheduler.s_current;
      protected set => VssScheduler.s_current = value;
    }

    public bool IsBlockedInWait { get; private set; }

    public bool EnterWait()
    {
      if (this.IsWaitSafe())
        return false;
      this.IsBlockedInWait = true;
      return true;
    }

    public void LeaveWait() => this.IsBlockedInWait = false;

    protected virtual bool IsWaitSafe() => false;

    public abstract void Run(SendOrPostCallback callback, object state);

    public abstract void RunAsync(SendOrPostCallback callback, object state);
  }
}
