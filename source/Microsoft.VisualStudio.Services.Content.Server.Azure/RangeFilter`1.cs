// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.RangeFilter`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class RangeFilter<TColumn> : IFilter<TColumn> where TColumn : IColumn
  {
    public readonly RangeMinimumBoundary<TColumn> Minimum;
    public readonly RangeMaximumBoundary<TColumn> Maximum;
    private readonly IFilter<TColumn> innerFilter;

    public RangeFilter(RangeMinimumBoundary<TColumn> minimum, RangeMaximumBoundary<TColumn> maximum)
    {
      this.Minimum = minimum;
      this.Maximum = maximum;
      this.innerFilter = (IFilter<TColumn>) new BooleanFilter<TColumn>(BooleanOperator.And, new IFilter<TColumn>[2]
      {
        RangeBoundaryBase<TColumn>.CreateComparisonFilter((RangeBoundaryBase<TColumn>) minimum),
        RangeBoundaryBase<TColumn>.CreateComparisonFilter((RangeBoundaryBase<TColumn>) maximum)
      });
    }

    public bool IsNull => this.innerFilter.IsNull;

    public StringBuilder CreateFilter(StringBuilder builder)
    {
      this.innerFilter.CreateFilter(builder);
      return builder;
    }

    public bool IsMatch(ITableEntityWithColumns entity) => this.innerFilter.IsMatch(entity);
  }
}
