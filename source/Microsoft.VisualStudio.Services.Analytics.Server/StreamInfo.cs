// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StreamInfo
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class StreamInfo
  {
    public string TableName { get; set; }

    public int ProviderShardId { get; set; }

    public int StreamId { get; set; }

    public bool Enabled { get; set; }

    public int Priority { get; set; }

    public bool Current { get; set; }

    public string Watermark { get; set; }

    public bool Maintenance { get; set; }

    public string MaintenanceReason { get; set; }

    public DateTime? MaintenceChangedDate { get; set; }

    public bool Disposed { get; set; }

    public DateTime CreateTime { get; set; }

    public DateTime? LoadedTime { get; set; }

    public int? InitialContentVersion { get; set; }

    public int? LatestContentVersion { get; set; }

    public bool KeysOnly { get; set; }
  }
}
