// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.PartitionRangeRowRangeQuery`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class PartitionRangeRowRangeQuery<TEntity> : Query<TEntity> where TEntity : ITableEntity, new()
  {
    public PartitionRangeRowRangeQuery(
      RangeFilter<PartitionKeyColumn> partitionRange,
      RangeFilter<RowKeyColumn> rowRange,
      IFilter<UserColumn> userColumnFilter = null,
      int? maxRowsToTake = null)
      : this(partitionRange, rowRange, (IFilter<INonUserColumn>) null, userColumnFilter, maxRowsToTake)
    {
    }

    public PartitionRangeRowRangeQuery(
      RangeFilter<PartitionKeyColumn> partitionRange,
      RangeFilter<RowKeyColumn> rowRange,
      IFilter<INonUserColumn> nonUserColumnFilter,
      IFilter<UserColumn> userColumnFilter = null,
      int? maxRowsToTake = null)
      : this(partitionRange, rowRange, nonUserColumnFilter, (IFilter<DomainColumn>) null, userColumnFilter, maxRowsToTake)
    {
    }

    public PartitionRangeRowRangeQuery(
      RangeFilter<PartitionKeyColumn> partitionRange,
      RangeFilter<RowKeyColumn> rowRange,
      IFilter<INonUserColumn> nonUserColumnFilter,
      IFilter<DomainColumn> domainColumnFilter,
      IFilter<UserColumn> userColumnFilter,
      int? maxRowsToTake = null)
      : base(new PartitionKeyFilter(partitionRange), rowRange != null ? new RowKeyFilter(rowRange) : (RowKeyFilter) null, nonUserColumnFilter, (IFilter<IUserColumn>) new BooleanFilter<IUserColumn>(BooleanOperator.And, new IFilter<IUserColumn>[2]
      {
        (IFilter<IUserColumn>) userColumnFilter,
        (IFilter<IUserColumn>) domainColumnFilter
      }), maxRowsToTake)
    {
    }
  }
}
