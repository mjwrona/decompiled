// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance.ColumnStoreFragmentationStatisticRow
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

namespace Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance
{
  public class ColumnStoreFragmentationStatisticRow
  {
    public string DBName { get; set; }

    public string Action { get; set; }

    public string Name { get; set; }

    public int PartitionId { get; set; }

    public int RowGroupId { get; set; }

    public long TotalRows { get; set; }

    public long DeletedRows { get; set; }

    public long SizeInBytes { get; set; }

    public string State { get; set; }

    public string TrimReason { get; set; }

    public string TransToCompressedDesc { get; set; }

    public int Fragmentation { get; set; }
  }
}
