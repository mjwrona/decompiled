// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.RangeBoundaryBase`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public abstract class RangeBoundaryBase<T> where T : IColumn
  {
    public readonly IColumnValue<T> BoundaryValue;
    public readonly RangeBoundaryType BoundaryType;

    protected RangeBoundaryBase(IColumnValue<T> boundaryValue, RangeBoundaryType boundaryType)
    {
      this.BoundaryValue = boundaryValue;
      this.BoundaryType = boundaryType;
    }

    protected abstract ComparisonOperator ExclusiveComparisonOperator { get; }

    protected abstract ComparisonOperator InclusiveComparisonOperator { get; }

    public static IFilter<T> CreateComparisonFilter(RangeBoundaryBase<T> boundary) => boundary != null ? (IFilter<T>) new ComparisonFilter<T>(boundary.BoundaryValue, boundary.BoundaryType == RangeBoundaryType.Exclusive ? boundary.ExclusiveComparisonOperator : boundary.InclusiveComparisonOperator) : (IFilter<T>) NullFilter<T>.Instance;
  }
}
