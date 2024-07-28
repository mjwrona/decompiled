// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Threading.VssRequestLockScope
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server.Threading
{
  internal struct VssRequestLockScope : IDisposable
  {
    private long m_newRequestId;
    private long? m_previousRequestId;

    public VssRequestLockScope(long requestId)
    {
      this.m_previousRequestId = new long?();
      this.m_newRequestId = requestId;
      if (LockHelperContext.RequestIdIsSet)
        this.m_previousRequestId = new long?(LockHelperContext.RequestId);
      if (this.m_previousRequestId.HasValue)
      {
        long newRequestId = this.m_newRequestId;
        long? previousRequestId = this.m_previousRequestId;
        long valueOrDefault = previousRequestId.GetValueOrDefault();
        if (newRequestId == valueOrDefault & previousRequestId.HasValue)
          return;
      }
      if (this.m_previousRequestId.HasValue)
        LockHelperContext.ClearRequestId();
      if (this.m_newRequestId == 0L)
        return;
      LockHelperContext.SetRequestId(this.m_newRequestId);
    }

    public void Dispose()
    {
      if (this.m_previousRequestId.HasValue)
      {
        long newRequestId = this.m_newRequestId;
        long? previousRequestId = this.m_previousRequestId;
        long valueOrDefault = previousRequestId.GetValueOrDefault();
        if (newRequestId == valueOrDefault & previousRequestId.HasValue)
          return;
      }
      if (LockHelperContext.RequestIdIsSet)
        LockHelperContext.ClearRequestId();
      if (!this.m_previousRequestId.HasValue)
        return;
      LockHelperContext.SetRequestId(this.m_previousRequestId.Value);
    }
  }
}
