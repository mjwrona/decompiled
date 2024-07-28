// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class AggregationMethodDefinition
  {
    public static AggregationMethodDefinition Sum = new AggregationMethodDefinition(AggregationMethod.Sum);
    public static AggregationMethodDefinition Min = new AggregationMethodDefinition(AggregationMethod.Min);
    public static AggregationMethodDefinition Max = new AggregationMethodDefinition(AggregationMethod.Max);
    public static AggregationMethodDefinition Average = new AggregationMethodDefinition(AggregationMethod.Average);
    public static AggregationMethodDefinition CountDistinct = new AggregationMethodDefinition(AggregationMethod.CountDistinct);
    public static AggregationMethodDefinition VirtualPropertyCount = new AggregationMethodDefinition(AggregationMethod.VirtualPropertyCount);

    private AggregationMethodDefinition(AggregationMethod aggregationMethodType) => this.MethodKind = aggregationMethodType;

    public AggregationMethod MethodKind { get; private set; }

    public string MethodLabel { get; private set; }

    public static AggregationMethodDefinition Custom(string customMethodLabel)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(customMethodLabel, nameof (customMethodLabel));
      return new AggregationMethodDefinition(AggregationMethod.Custom)
      {
        MethodLabel = customMethodLabel
      };
    }
  }
}
