// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.PartitionSpecificRowsQuery`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class PartitionSpecificRowsQuery<T> : Query<T> where T : ITableEntity, new()
  {
    public PartitionSpecificRowsQuery(
      StringColumnValue<PartitionKeyColumn> partitionKey,
      params StringColumnValue<RowKeyColumn>[] rowKeys)
      : base(new PartitionKeyFilter(new EqualFilter<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) partitionKey)), RowKeyFilter.CreateDisjunctiveRowKeyFilter(((IEnumerable<StringColumnValue<RowKeyColumn>>) rowKeys).Select<StringColumnValue<RowKeyColumn>, EqualFilter<RowKeyColumn>>((Func<StringColumnValue<RowKeyColumn>, EqualFilter<RowKeyColumn>>) (rowKey => new EqualFilter<RowKeyColumn>((IColumnValue<RowKeyColumn>) rowKey))).ToArray<EqualFilter<RowKeyColumn>>()), (IFilter<IUserColumn>) null, new int?())
    {
      if (rowKeys.Length > 14)
        throw new ArgumentException("Azure Table only support 15 comparisons per query. One is needed for the PartitionKey.");
    }
  }
}
