// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ActivityLogPerfTimer
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public sealed class ActivityLogPerfTimer : IDisposable
  {
    private PerformanceTimer perfTimer;
    private bool disposedValue;

    public ActivityLogPerfTimer(VssRequestPump.Processor processor, string groupName) => this.disposedValue = true;

    public void Dispose()
    {
      if (this.disposedValue)
        return;
      this.disposedValue = true;
      this.perfTimer.End();
      this.perfTimer.Dispose();
    }
  }
}
