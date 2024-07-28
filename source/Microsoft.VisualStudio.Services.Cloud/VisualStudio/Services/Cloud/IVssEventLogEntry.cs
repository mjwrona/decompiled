// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.IVssEventLogEntry
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public interface IVssEventLogEntry
  {
    string Category { get; set; }

    short CategoryNumber { get; set; }

    EventLogEntryType EntryType { get; set; }

    int EventId { get; set; }

    long InstanceId { get; set; }

    string MachineName { get; set; }

    string Message { get; set; }

    string Source { get; set; }

    DateTime TimeGenerated { get; set; }
  }
}
