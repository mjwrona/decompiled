// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.ITelemetryScheduler
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal interface ITelemetryScheduler
  {
    void Schedule(Action action, CancellationToken? token = null);

    void Schedule(Func<Task> actionTask, CancellationToken? token = null);

    void InitializeTimed(TimeSpan delay);

    void ScheduleTimed(Action action, bool recurring = false);

    void ScheduleTimed(Func<Task> actionTask, bool recurring = false);

    bool CanEnterTimedDelegate();

    void ExitTimedDelegate();

    void CancelTimed(bool wait = false);
  }
}
