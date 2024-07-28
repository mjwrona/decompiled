// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance.ColumnStoreFragmentationStatistics
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

namespace Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance
{
  public class ColumnStoreFragmentationStatistics
  {
    public string Action { get; set; }

    public string IndexName { get; set; }

    public long OpenRowGroupMaxSizeInBytes { get; set; }

    public long OpenRowGroupMaxRowCount { get; set; }

    public int OpenRowGroupsCount { get; set; }

    public long DictonarySizeRowGroupCount { get; set; }

    public long TotalCount { get; set; }

    public long FragmentedRowGroupCount { get; set; }

    public int MaxFragmentation { get; set; }
  }
}
