// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationStopwatch
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class NotificationStopwatch : Stopwatch
  {
    public NotificationStopwatch(string name) => this.Name = name;

    public string Name { get; private set; }

    public override string ToString() => string.Format("{0}={1}", (object) this.Name, (object) this.ElapsedMilliseconds);

    public static void WriteTimings(
      Dictionary<string, int> timings,
      params NotificationStopwatch[] stopwatches)
    {
      foreach (NotificationStopwatch stopwatch in stopwatches)
        timings[stopwatch.Name] = (int) stopwatch.ElapsedMilliseconds;
    }
  }
}
