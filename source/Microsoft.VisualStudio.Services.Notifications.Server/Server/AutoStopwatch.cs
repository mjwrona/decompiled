// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.AutoStopwatch
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class AutoStopwatch : IDisposable
  {
    private Stopwatch m_stopwatch;

    public AutoStopwatch(Stopwatch stopwatch)
    {
      this.m_stopwatch = stopwatch;
      this.m_stopwatch?.Start();
    }

    public void Dispose() => this.m_stopwatch?.Stop();
  }
}
