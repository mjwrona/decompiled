// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class IntervalsQuery : 
    FieldNameQueryBase,
    IIntervalsQuery,
    IFieldNameQuery,
    IQuery,
    IIntervalsContainer
  {
    public IIntervalsAllOf AllOf { get; set; }

    public IIntervalsAnyOf AnyOf { get; set; }

    public IIntervalsMatch Match { get; set; }

    public IIntervalsFuzzy Fuzzy { get; set; }

    public IIntervalsPrefix Prefix { get; set; }

    public IIntervalsWildcard Wildcard { get; set; }

    protected override bool Conditionless => IntervalsQuery.IsConditionless((IIntervalsQuery) this);

    internal static bool IsConditionless(IIntervalsQuery q)
    {
      if (q.Field.IsConditionless())
        return true;
      return q.Match == null && q.AllOf == null && q.AnyOf == null && q.Prefix == null && q.Wildcard == null && q.Fuzzy == null;
    }

    internal override void InternalWrapInContainer(IQueryContainer container) => container.Intervals = (IIntervalsQuery) this;
  }
}
