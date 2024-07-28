// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.RowKeyFilter
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class RowKeyFilter : KnownBoundsFilter
  {
    private RowKeyFilter(IFilter<INonUserColumn> filter, string min, string max)
      : base(filter, min, max)
    {
    }

    public static RowKeyFilter CreateDisjunctiveRowKeyFilter(EqualFilter<RowKeyColumn>[] filters) => new RowKeyFilter((IFilter<INonUserColumn>) new BooleanFilter<RowKeyColumn>(BooleanOperator.Or, (IFilter<RowKeyColumn>[]) filters), (string) null, (string) null);

    public static RowKeyFilter CreateDisjunctiveRowKeyFilter(
      EqualFilter<RowKeyColumn>[] equalFilters,
      RangeFilter<RowKeyColumn> rangeFilter)
    {
      List<IFilter<RowKeyColumn>> filterList = new List<IFilter<RowKeyColumn>>();
      filterList.AddRange((IEnumerable<IFilter<RowKeyColumn>>) equalFilters);
      filterList.Add((IFilter<RowKeyColumn>) rangeFilter);
      return new RowKeyFilter((IFilter<INonUserColumn>) new BooleanFilter<RowKeyColumn>(BooleanOperator.Or, filterList.ToArray()), (string) null, (string) null);
    }

    public RowKeyFilter(NullFilter<RowKeyColumn> filter)
      : base((IFilter<INonUserColumn>) filter, (string) null, (string) null)
    {
    }

    public RowKeyFilter(EqualFilter<RowKeyColumn> filter)
      : base((IFilter<INonUserColumn>) filter, ((StringValue) filter.ColumnValue.Value).Value, ((StringValue) filter.ColumnValue.Value).Value)
    {
    }

    public RowKeyFilter(RangeFilter<RowKeyColumn> filter)
      : base((IFilter<INonUserColumn>) filter, filter.Minimum == null ? (string) null : ((StringValue) filter.Minimum.BoundaryValue.Value).Value, filter.Maximum == null ? (string) null : ((StringValue) filter.Maximum.BoundaryValue.Value).Value)
    {
    }
  }
}
