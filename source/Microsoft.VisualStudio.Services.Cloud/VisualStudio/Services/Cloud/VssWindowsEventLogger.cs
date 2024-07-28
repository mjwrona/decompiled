// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.VssWindowsEventLogger
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class VssWindowsEventLogger : IVssEventLogger
  {
    private EventLog m_log;

    public VssWindowsEventLogger(string source) => this.m_log = new EventLog("Application", Environment.MachineName, source);

    public void WriteEntry(string message, EventLogEntryType type, int eventId) => this.m_log.WriteEntry(message, type, eventId);

    public List<IVssEventLogEntry> Get(DateTime? startTime = null) => ((IEnumerable<IVssEventLogEntry>) this.m_log.Entries.Cast<EventLogEntry>().Where<EventLogEntry>((Func<EventLogEntry, bool>) (e =>
    {
      if (!startTime.HasValue)
        return true;
      DateTime timeGenerated = e.TimeGenerated;
      DateTime? nullable = startTime;
      return nullable.HasValue && timeGenerated >= nullable.GetValueOrDefault();
    })).Select<EventLogEntry, VssEventLogEntry>((Func<EventLogEntry, VssEventLogEntry>) (e => new VssEventLogEntry()
    {
      Source = e.Source,
      Category = e.Category,
      CategoryNumber = e.CategoryNumber,
      Data = e.Data,
      EntryType = e.EntryType,
      EventId = e.EventID,
      Index = e.Index,
      InstanceId = e.InstanceId,
      MachineName = e.MachineName,
      Message = e.Message,
      ReplacementStrings = e.ReplacementStrings,
      TimeGenerated = e.TimeGenerated,
      TimeWritten = e.TimeWritten,
      UserName = e.UserName
    }))).ToList<IVssEventLogEntry>();
  }
}
