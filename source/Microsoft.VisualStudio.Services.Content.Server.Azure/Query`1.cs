// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Query`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public abstract class Query<TEntity> where TEntity : ITableEntity, new()
  {
    internal readonly IFilter<IColumn> filter;

    protected Query(
      PartitionKeyFilter partitionFilter,
      RowKeyFilter rowFilter,
      IFilter<INonUserColumn> nonUserColumnFilter,
      IFilter<IUserColumn> userColumnFilter,
      int? maxRowsToTake)
    {
      this.PartitionKeyMax = partitionFilter.Max;
      this.PartitionKeyMin = partitionFilter.Min;
      this.MaxRowsToTake = maxRowsToTake;
      this.filter = (IFilter<IColumn>) partitionFilter;
      if (rowFilter != null)
      {
        this.RowKeyMax = rowFilter.Max;
        this.RowKeyMin = rowFilter.Min;
        this.filter = (IFilter<IColumn>) new BooleanFilter<IColumn>(BooleanOperator.And, new IFilter<IColumn>[2]
        {
          this.filter,
          (IFilter<IColumn>) rowFilter
        });
      }
      if (nonUserColumnFilter != null)
      {
        this.filter = (IFilter<IColumn>) new BooleanFilter<IColumn>(BooleanOperator.And, new IFilter<IColumn>[2]
        {
          this.filter,
          (IFilter<IColumn>) nonUserColumnFilter
        });
        this.HasExtraFilters = true;
      }
      if (userColumnFilter == null)
        return;
      this.filter = (IFilter<IColumn>) new BooleanFilter<IColumn>(BooleanOperator.And, new IFilter<IColumn>[2]
      {
        this.filter,
        (IFilter<IColumn>) userColumnFilter
      });
      this.HasExtraFilters = true;
    }

    protected Query(
      PartitionKeyFilter partitionFilter,
      RowKeyFilter rowFilter,
      IFilter<IUserColumn> userColumnFilter,
      int? maxRowsToTake)
      : this(partitionFilter, rowFilter, (IFilter<INonUserColumn>) null, userColumnFilter, maxRowsToTake)
    {
    }

    public string PartitionKeyMax { get; private set; }

    public string PartitionKeyMin { get; private set; }

    public string RowKeyMax { get; private set; }

    public string RowKeyMin { get; private set; }

    public List<string> Columns { get; set; }

    public int? MaxRowsToTake { get; set; }

    public bool HasExtraFilters { get; }

    public TableQuery<TEntity> CreateTableQuery()
    {
      TableQuery<TEntity> tableQuery1 = new TableQuery<TEntity>();
      StringBuilder builder = new StringBuilder();
      this.filter.CreateFilter(builder);
      tableQuery1.FilterString = builder.ToString();
      if (this.MaxRowsToTake.HasValue)
      {
        int? maxRowsToTake = this.MaxRowsToTake;
        if (maxRowsToTake.Value != int.MaxValue)
        {
          TableQuery<TEntity> tableQuery2 = tableQuery1;
          maxRowsToTake = this.MaxRowsToTake;
          int? nullable = new int?(maxRowsToTake.Value);
          tableQuery2.TakeCount = nullable;
        }
      }
      if (this.Columns != null)
        tableQuery1.SelectColumns = (IList<string>) this.Columns;
      return tableQuery1;
    }

    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();
      if (this.MaxRowsToTake.HasValue)
        builder.AppendFormat("MaxRows: {0}", (object) this.MaxRowsToTake).AppendLine();
      if (this.Columns != null)
        builder.AppendFormat("Columns: {0}", (object) string.Join(", ", (IEnumerable<string>) this.Columns)).AppendLine();
      builder.Append("FilterString: ");
      this.filter.CreateFilter(builder);
      return builder.ToString();
    }
  }
}
