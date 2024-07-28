// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.PiiTraceSource
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Diagnostics;
using System.ServiceModel.Configuration;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal class PiiTraceSource : TraceSource
  {
    private string eventSourceName = string.Empty;
    internal const string LogPii = "logKnownPii";
    private bool shouldLogPii;
    private bool initialized;
    private object localSyncObject = new object();

    internal PiiTraceSource(string name, string eventSourceName)
      : base(name)
    {
      this.eventSourceName = eventSourceName;
    }

    internal PiiTraceSource(string name, string eventSourceName, SourceLevels levels)
      : base(name, levels)
    {
      this.eventSourceName = eventSourceName;
    }

    private void Initialize()
    {
      if (this.initialized)
        return;
      lock (this.localSyncObject)
      {
        if (this.initialized)
          return;
        string attribute = this.Attributes["logKnownPii"];
        bool result = false;
        if (!string.IsNullOrEmpty(attribute) && !bool.TryParse(attribute, out result))
          result = false;
        if (result)
        {
          EventLogger eventLogger = new EventLogger(this.eventSourceName, (object) null);
          if (MachineSettingsSection.EnableLoggingKnownPii)
          {
            eventLogger.LogEvent(TraceEventType.Information, EventLogCategory.MessageLogging, EventLogEventId.PiiLoggingOn, false);
            this.shouldLogPii = true;
          }
          else
            eventLogger.LogEvent(TraceEventType.Error, EventLogCategory.MessageLogging, EventLogEventId.PiiLoggingNotAllowed, false);
        }
        this.initialized = true;
      }
    }

    protected override string[] GetSupportedAttributes() => new string[1]
    {
      "logKnownPii"
    };

    internal bool ShouldLogPii
    {
      get
      {
        if (!this.initialized)
          this.Initialize();
        return this.shouldLogPii;
      }
      set
      {
        this.initialized = true;
        this.shouldLogPii = value;
      }
    }
  }
}
