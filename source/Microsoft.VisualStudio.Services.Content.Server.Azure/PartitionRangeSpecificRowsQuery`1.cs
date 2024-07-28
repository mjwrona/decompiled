// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.PartitionRangeSpecificRowsQuery`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class PartitionRangeSpecificRowsQuery<T> : Query<T> where T : ITableEntity, new()
  {
    public PartitionRangeSpecificRowsQuery(
      RangeFilter<PartitionKeyColumn> partitionRange,
      StringColumnValue<RowKeyColumn>[] rowKeys,
      RangeFilter<RowKeyColumn> rowRange)
      : base(new PartitionKeyFilter(partitionRange), RowKeyFilter.CreateDisjunctiveRowKeyFilter(((IEnumerable<StringColumnValue<RowKeyColumn>>) rowKeys).Select<StringColumnValue<RowKeyColumn>, EqualFilter<RowKeyColumn>>((Func<StringColumnValue<RowKeyColumn>, EqualFilter<RowKeyColumn>>) (rowKey => new EqualFilter<RowKeyColumn>((IColumnValue<RowKeyColumn>) rowKey))).ToArray<EqualFilter<RowKeyColumn>>(), rowRange), (IFilter<IUserColumn>) null, new int?())
    {
    }
  }
}
