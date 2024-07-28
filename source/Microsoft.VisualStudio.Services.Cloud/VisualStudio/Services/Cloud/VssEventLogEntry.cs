// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.VssEventLogEntry
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class VssEventLogEntry : IVssEventLogEntry
  {
    public string Category { get; set; }

    public short CategoryNumber { get; set; }

    public byte[] Data { get; set; }

    public EventLogEntryType EntryType { get; set; }

    public int EventId { get; set; }

    public int Index { get; set; }

    public long InstanceId { get; set; }

    public string MachineName { get; set; }

    public string Message { get; set; }

    public string[] ReplacementStrings { get; set; }

    public string Source { get; set; }

    public DateTime TimeGenerated { get; set; }

    public DateTime TimeWritten { get; set; }

    public string UserName { get; set; }
  }
}
