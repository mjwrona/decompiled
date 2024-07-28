// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Threading.VssActivityScope
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Diagnostics.Eventing;

namespace Microsoft.TeamFoundation.Framework.Server.Threading
{
  public struct VssActivityScope : IDisposable
  {
    private bool m_disposed;
    private Guid m_activityId;
    private Guid m_previousActivityId;

    public VssActivityScope(Guid activityId)
    {
      this.m_disposed = false;
      this.m_activityId = activityId;
      this.m_previousActivityId = Trace.CorrelationManager.ActivityId;
      if (!(this.m_previousActivityId != this.m_activityId))
        return;
      EventProvider.SetActivityId(ref this.m_activityId);
    }

    public void Dispose()
    {
      if (this.m_disposed)
        return;
      this.m_disposed = true;
      if (!(this.m_previousActivityId != this.m_activityId))
        return;
      EventProvider.SetActivityId(ref this.m_previousActivityId);
    }
  }
}
