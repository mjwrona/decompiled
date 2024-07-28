// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.PartitionScanQuery`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class PartitionScanQuery<T> : Query<T> where T : ITableEntity, new()
  {
    public PartitionScanQuery(
      StringColumnValue<PartitionKeyColumn> partitionKey,
      IFilter<IUserColumn> userColumnFilter = null,
      int? maxRowsToTake = null)
      : base(new PartitionKeyFilter(new EqualFilter<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) partitionKey)), new RowKeyFilter(NullFilter<RowKeyColumn>.Instance), userColumnFilter, maxRowsToTake)
    {
    }
  }
}
