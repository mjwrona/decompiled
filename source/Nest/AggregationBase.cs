// Decompiled with JetBrains decompiler
// Type: Nest.AggregationBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public abstract class AggregationBase : IAggregation
  {
    internal AggregationBase()
    {
    }

    protected AggregationBase(string name) => ((IAggregation) this).Name = name;

    public IDictionary<string, object> Meta { get; set; }

    string IAggregation.Name { get; set; }

    internal abstract void WrapInContainer(AggregationContainer container);

    public static bool operator false(AggregationBase a) => false;

    public static bool operator true(AggregationBase a) => false;

    public static AggregationBase operator &(AggregationBase left, AggregationBase right) => (AggregationBase) new AggregationCombinator((string) null, left, right);
  }
}
