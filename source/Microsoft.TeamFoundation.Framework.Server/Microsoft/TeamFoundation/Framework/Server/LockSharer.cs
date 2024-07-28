// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LockSharer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LockSharer : IDisposable
  {
    private bool m_isDisposed;
    private readonly StackTracer m_constructorStackTrace;

    public LockSharer(long requestId)
    {
      LockHelperContext.SetRequestId(requestId);
      if (!TeamFoundationTracingService.IsRawTracingEnabled(1975758951, TraceLevel.Info, "HostManagement", "BusinessLogic", (string[]) null))
        return;
      this.m_constructorStackTrace = new StackTracer();
    }

    public void Dispose()
    {
      this.m_isDisposed = true;
      LockHelperContext.ClearRequestId();
      GC.SuppressFinalize((object) this);
    }

    ~LockSharer()
    {
      if (this.m_isDisposed)
        return;
      if (this.m_constructorStackTrace != null)
        TeamFoundationTracingService.TraceRaw(1775227344, TraceLevel.Error, "HostManagement", "BusinessLogic", "LockSharer finalizer without dispose - call stack: {0}", (object) this.m_constructorStackTrace);
      else
        TeamFoundationTracingService.TraceRaw(1775227344, TraceLevel.Error, "HostManagement", "BusinessLogic", "LockSharer finalizer without dispose");
    }
  }
}
