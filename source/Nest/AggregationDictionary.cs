// Decompiled with JetBrains decompiler
// Type: Nest.AggregationDictionary
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  [JsonFormatter(typeof (AggregationDictionaryFormatter))]
  public class AggregationDictionary : IsADictionaryBase<string, IAggregationContainer>
  {
    public AggregationDictionary()
    {
    }

    public AggregationDictionary(
      IDictionary<string, IAggregationContainer> container)
      : base(container)
    {
    }

    public AggregationDictionary(Dictionary<string, AggregationContainer> container)
      : base((IDictionary<string, IAggregationContainer>) container.ToDictionary<KeyValuePair<string, AggregationContainer>, string, IAggregationContainer>((Func<KeyValuePair<string, AggregationContainer>, string>) (kv => kv.Key), (Func<KeyValuePair<string, AggregationContainer>, IAggregationContainer>) (kv => (IAggregationContainer) kv.Value)))
    {
    }

    public static implicit operator AggregationDictionary(
      Dictionary<string, IAggregationContainer> container)
    {
      return new AggregationDictionary((IDictionary<string, IAggregationContainer>) container);
    }

    public static implicit operator AggregationDictionary(
      Dictionary<string, AggregationContainer> container)
    {
      return new AggregationDictionary(container);
    }

    public static implicit operator AggregationDictionary(AggregationBase aggregator)
    {
      if (aggregator is AggregationCombinator aggregationCombinator)
      {
        AggregationDictionary aggregationDictionary = new AggregationDictionary();
        foreach (AggregationBase aggregation1 in aggregationCombinator.Aggregations)
        {
          IAggregation aggregation2 = (IAggregation) aggregation1;
          if (aggregation2.Name.IsNullOrEmpty())
            throw new ArgumentException(aggregator.GetType().Name + " .Name is not set!");
          aggregationDictionary.Add(aggregation2.Name, (AggregationContainer) aggregation1);
        }
        return aggregationDictionary;
      }
      IAggregation aggregation = (IAggregation) aggregator;
      if (aggregation.Name.IsNullOrEmpty())
        throw new ArgumentException(aggregator.GetType().Name + " .Name is not set!");
      return new AggregationDictionary()
      {
        {
          aggregation.Name,
          (AggregationContainer) aggregator
        }
      };
    }

    public void Add(string key, AggregationContainer value) => this.BackingDictionary.Add(this.ValidateKey(key), (IAggregationContainer) value);

    protected override string ValidateKey(string key) => !((IEnumerable<string>) AggregateFormatter.AllReservedAggregationNames).Contains<string>(key) ? key : throw new ArgumentException(string.Format(AggregateFormatter.UsingReservedAggNameFormat, (object) key), nameof (key));
  }
}
