// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance.ColumnStoreOverlapStatistics
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

namespace Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance
{
  public class ColumnStoreOverlapStatistics
  {
    public string DBName { get; set; }

    public int PartitionNumber { get; set; }

    public int PartitionId { get; set; }

    public int Overlaps { get; set; }

    public long SegmentsInPartition { get; set; }

    public string TableName { get; set; }
  }
}
