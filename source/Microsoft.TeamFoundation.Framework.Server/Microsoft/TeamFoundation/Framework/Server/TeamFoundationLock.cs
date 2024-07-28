// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationLock
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationLock : IDisposable
  {
    private LockingComponent m_lockingComponent;
    private bool m_useReleaseLocks;
    private static readonly string s_area = "Locking";
    private static readonly string s_layer = nameof (TeamFoundationLock);

    internal TeamFoundationLock(
      LockingComponent component,
      TeamFoundationLockMode lockMode,
      bool useReleaseLocks,
      string[] resources)
    {
      this.m_lockingComponent = component;
      this.LockMode = lockMode;
      this.m_useReleaseLocks = useReleaseLocks;
      this.Resources = resources;
    }

    public bool IsDisposed { get; private set; }

    public TeamFoundationLockMode LockMode { get; internal set; }

    public string[] Resources { get; internal set; }

    public void Dispose()
    {
      try
      {
        if (this.m_lockingComponent == null && this.Resources.Length == 0)
        {
          this.IsDisposed = true;
          TeamFoundationTracingService.TraceRaw(98200, TraceLevel.Info, TeamFoundationLock.s_area, TeamFoundationLock.s_layer, "Disposed of no-op TeamFoundationLock.");
        }
        else if (this.m_lockingComponent != null && this.m_lockingComponent.IsDisposed)
          TeamFoundationTracingService.TraceRaw(98202, TraceLevel.Error, TeamFoundationLock.s_area, TeamFoundationLock.s_layer, "Component was disposed before lock was released");
        else if (this.m_useReleaseLocks)
        {
          LockingComponent2 lockingComponent = (LockingComponent2) this.m_lockingComponent;
          if (lockingComponent == null)
            return;
          this.m_lockingComponent = (LockingComponent) null;
          try
          {
            lockingComponent.ReleaseLocks(this.Resources);
          }
          finally
          {
            this.IsDisposed = true;
            if (lockingComponent.LocksAcquiredCount == 0)
              lockingComponent.Dispose();
          }
        }
        else
        {
          LockingComponent lockingComponent = this.m_lockingComponent;
          if (lockingComponent == null)
            return;
          this.m_lockingComponent = (LockingComponent) null;
          try
          {
            lockingComponent.ReleaseLock(this.Resources[0]);
          }
          finally
          {
            this.IsDisposed = true;
            if (lockingComponent.LocksAcquiredCount == 0)
              lockingComponent.Dispose();
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(98201, TeamFoundationLock.s_area, TeamFoundationLock.s_layer, ex);
      }
    }
  }
}
