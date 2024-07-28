// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryActivity
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  public sealed class TelemetryActivity : TelemetryEvent
  {
    private readonly Guid correlationId;
    private readonly Guid parentCorrelationId;

    public Guid CorrelationId => this.correlationId;

    internal Guid ParentCorrelationId => this.parentCorrelationId;

    internal DateTime StartTime { get; private set; }

    internal DateTime EndTime { get; private set; }

    public TelemetryActivity(string eventName)
      : this(eventName, Guid.Empty)
    {
    }

    public TelemetryActivity(string eventName, Guid parentCorrelationId)
      : base(eventName)
    {
      this.correlationId = Guid.NewGuid();
      this.parentCorrelationId = parentCorrelationId;
      this.StartTime = DateTime.MinValue;
      this.EndTime = DateTime.MinValue;
    }

    public void Start()
    {
      this.StartTime = !(this.StartTime != DateTime.MinValue) ? DateTime.UtcNow : throw new InvalidOperationException("Activity is already started.");
      TelemetryService.TelemetryEventSource.WriteActivityStartEvent(this);
    }

    public void End()
    {
      if (this.StartTime == DateTime.MinValue)
        throw new InvalidOperationException("Activity is not yet started.");
      this.EndTime = !(this.EndTime != DateTime.MinValue) ? DateTime.UtcNow : throw new InvalidOperationException("Activity is already ended.");
      TelemetryService.TelemetryEventSource.WriteActivityStopEvent(this);
    }

    public void End(TimeSpan duration)
    {
      if (this.StartTime != DateTime.MinValue)
        throw new InvalidOperationException("Activity is already started and can not be ended with known duration.");
      this.EndTime = !(this.EndTime != DateTime.MinValue) ? DateTime.UtcNow : throw new InvalidOperationException("Activity is already ended.");
      this.StartTime = this.EndTime - duration;
      TelemetryService.TelemetryEventSource.WriteActivityEndWithDurationEvent(this);
    }

    protected override IEnumerable<KeyValuePair<string, object>> GetDefaultEventProperties(
      long eventTime,
      long processStartTime,
      string sessionId)
    {
      foreach (KeyValuePair<string, object> defaultEventProperty in base.GetDefaultEventProperties(eventTime, processStartTime, sessionId))
        yield return defaultEventProperty;
      if (this.StartTime > DateTime.MinValue)
        yield return new KeyValuePair<string, object>("Reserved.Activity.StartTime", (object) Math.Round(new TimeSpan(this.StartTime.Ticks - processStartTime).TotalMilliseconds));
      else
        yield return new KeyValuePair<string, object>("Reserved.Activity.StartTime", (object) -1);
      if (this.EndTime > DateTime.MinValue)
      {
        yield return new KeyValuePair<string, object>("Reserved.Activity.EndTime", (object) Math.Round(new TimeSpan(this.EndTime.Ticks - processStartTime).TotalMilliseconds));
        TimeSpan timeSpan;
        ref TimeSpan local = ref timeSpan;
        DateTime dateTime = this.EndTime;
        long ticks1 = dateTime.Ticks;
        dateTime = this.StartTime;
        long ticks2 = dateTime.Ticks;
        long ticks3 = ticks1 - ticks2;
        local = new TimeSpan(ticks3);
        yield return new KeyValuePair<string, object>("Reserved.Activity.Duration", (object) Math.Round(timeSpan.TotalMilliseconds));
      }
      else
      {
        yield return new KeyValuePair<string, object>("Reserved.Activity.EndTime", (object) -1);
        yield return new KeyValuePair<string, object>("Reserved.Activity.Duration", (object) -1);
      }
      yield return new KeyValuePair<string, object>("Reserved.Activity.CorrelationId", (object) this.CorrelationId);
      if (this.ParentCorrelationId != Guid.Empty)
        yield return new KeyValuePair<string, object>("Reserved.Activity.ParentCorrelationId", (object) this.ParentCorrelationId);
    }
  }
}
