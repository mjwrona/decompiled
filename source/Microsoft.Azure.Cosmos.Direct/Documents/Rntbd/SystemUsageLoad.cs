// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.SystemUsageLoad
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal struct SystemUsageLoad
  {
    public readonly DateTime Timestamp;
    public readonly float? CpuUsage;
    public readonly long? MemoryAvailable;
    public readonly ThreadInformation ThreadInfo;
    public readonly int? NumberOfOpenTcpConnections;

    public SystemUsageLoad(
      DateTime timestamp,
      ThreadInformation threadInfo,
      float? cpuUsage = null,
      long? memoryAvailable = null,
      int? numberOfOpenTcpConnection = 0)
    {
      this.Timestamp = timestamp;
      this.CpuUsage = cpuUsage;
      this.MemoryAvailable = memoryAvailable;
      this.ThreadInfo = threadInfo ?? throw new ArgumentNullException("Thread Information can not be null");
      this.NumberOfOpenTcpConnections = numberOfOpenTcpConnection;
    }

    public void AppendJsonString(StringBuilder stringBuilder)
    {
      stringBuilder.Append("{\"dateUtc\":\"");
      stringBuilder.Append(this.Timestamp.ToString("O", (IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.Append("\",\"cpu\":");
      stringBuilder.Append(!this.CpuUsage.HasValue || float.IsNaN(this.CpuUsage.Value) ? "\"no info\"" : this.CpuUsage.Value.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.Append(",\"memory\":");
      stringBuilder.Append(this.MemoryAvailable.HasValue ? this.MemoryAvailable.Value.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture) : "\"no info\"");
      stringBuilder.Append(",\"threadInfo\":");
      if (this.ThreadInfo != null)
        this.ThreadInfo.AppendJsonString(stringBuilder);
      else
        stringBuilder.Append("\"no info\"");
      stringBuilder.Append(",\"numberOfOpenTcpConnection\":");
      stringBuilder.Append(this.NumberOfOpenTcpConnections.HasValue ? this.NumberOfOpenTcpConnections.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture) : "\"no info\"");
      stringBuilder.Append("}");
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0:O} => CpuUsage :{1:F3}, MemoryAvailable :{2:F3} {3:F3}, NumberOfOpenTcpConnection : {4} )", (object) this.Timestamp, (object) this.CpuUsage, (object) this.MemoryAvailable, (object) this.ThreadInfo.ToString(), (object) this.NumberOfOpenTcpConnections);
  }
}
