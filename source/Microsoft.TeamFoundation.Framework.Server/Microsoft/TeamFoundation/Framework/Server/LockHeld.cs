// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LockHeld
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LockHeld : IDisposable
  {
    private ILockName m_lockName;
    private LockManager m_lockManager;
    private LockManager.LockType m_lockType;
    private long m_requestId;
    private bool m_isDisposed;
    private static readonly string s_area = "HostManagement";
    private static readonly string s_layer = nameof (LockHeld);

    public LockHeld(
      LockManager lockManager,
      ILockName lockName,
      LockManager.LockType lockType,
      long requestId)
    {
      this.m_lockName = lockName;
      this.m_lockType = lockType;
      this.m_lockManager = lockManager;
      this.m_isDisposed = false;
      this.m_requestId = requestId;
    }

    public void Dispose()
    {
      try
      {
        if (this.m_isDisposed)
          return;
        this.m_lockManager.ReleaseLock(this.m_lockName, this.m_lockType, this.m_requestId);
        this.m_isDisposed = true;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(58676, LockHeld.s_area, LockHeld.s_layer, ex);
        throw;
      }
    }

    public ILockName LockName => this.m_lockName;

    public LockManager.LockType LockType => this.m_lockType;

    public long RequestId => this.m_requestId;
  }
}
