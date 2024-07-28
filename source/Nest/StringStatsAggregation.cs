// Decompiled with JetBrains decompiler
// Type: Nest.StringStatsAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class StringStatsAggregation : AggregationBase, IStringStatsAggregation, IAggregation
  {
    internal StringStatsAggregation()
    {
    }

    public StringStatsAggregation(string name, Field field)
      : base(name)
    {
      this.Field = field;
    }

    internal override void WrapInContainer(AggregationContainer c) => c.StringStats = (IStringStatsAggregation) this;

    public Field Field { get; set; }

    public object Missing { get; set; }

    public IScript Script { get; set; }

    public bool? ShowDistribution { get; set; }
  }
}
