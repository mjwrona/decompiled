// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.SingleGroupAggregator
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Query.Aggregation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Documents.Query
{
  internal abstract class SingleGroupAggregator
  {
    public abstract void AddValues(JToken values);

    public abstract object GetResult();

    public static SingleGroupAggregator Create(
      AggregateOperator[] aggregates,
      IReadOnlyDictionary<string, AggregateOperator?> aggregateAliasToAggregateType,
      IReadOnlyList<string> orderedAliases,
      bool hasSelectValue)
    {
      return !hasSelectValue ? (SingleGroupAggregator) SingleGroupAggregator.SelectListAggregateValues.Create(aggregateAliasToAggregateType, orderedAliases) : (aggregates == null || !((IEnumerable<AggregateOperator>) aggregates).Any<AggregateOperator>() ? (SingleGroupAggregator) SingleGroupAggregator.SelectValueAggregateValues.Create(new AggregateOperator?()) : (SingleGroupAggregator) SingleGroupAggregator.SelectValueAggregateValues.Create(new AggregateOperator?(aggregates[0])));
    }

    private sealed class SelectValueAggregateValues : SingleGroupAggregator
    {
      private readonly SingleGroupAggregator.AggregateValue AggregateValue;

      private SelectValueAggregateValues(
        SingleGroupAggregator.AggregateValue AggregateValue)
      {
        this.AggregateValue = AggregateValue != null ? AggregateValue : throw new ArgumentNullException(nameof (AggregateValue));
      }

      public static SingleGroupAggregator.SelectValueAggregateValues Create(
        AggregateOperator? aggregateOperator)
      {
        return new SingleGroupAggregator.SelectValueAggregateValues(SingleGroupAggregator.AggregateValue.Create(aggregateOperator));
      }

      public override void AddValues(JToken values) => this.AggregateValue.AddValue(values);

      public override object GetResult() => this.AggregateValue.Result;

      public override string ToString() => this.AggregateValue.ToString();
    }

    private sealed class SelectListAggregateValues : SingleGroupAggregator
    {
      private readonly IReadOnlyDictionary<string, SingleGroupAggregator.AggregateValue> aliasToValue;
      private readonly IReadOnlyList<string> orderedAliases;

      private SelectListAggregateValues(
        IReadOnlyDictionary<string, SingleGroupAggregator.AggregateValue> aliasToValue,
        IReadOnlyList<string> orderedAliases)
      {
        this.aliasToValue = aliasToValue;
        this.orderedAliases = orderedAliases;
      }

      public override object GetResult()
      {
        JObject result1 = new JObject();
        foreach (string orderedAlias in (IEnumerable<string>) this.orderedAliases)
        {
          SingleGroupAggregator.AggregateValue aggregateValue = this.aliasToValue[orderedAlias];
          object result2 = aggregateValue.Result;
          if (result2 != Undefined.Value)
          {
            JToken jtoken = result2 != null ? JToken.FromObject(aggregateValue.Result) : (JToken) JValue.CreateNull();
            result1[orderedAlias] = jtoken;
          }
        }
        return (object) result1;
      }

      public static SingleGroupAggregator.SelectListAggregateValues Create(
        IReadOnlyDictionary<string, AggregateOperator?> AggregateAliasToAggregateType,
        IReadOnlyList<string> orderedAliases)
      {
        Dictionary<string, SingleGroupAggregator.AggregateValue> aliasToValue = new Dictionary<string, SingleGroupAggregator.AggregateValue>();
        foreach (KeyValuePair<string, AggregateOperator?> keyValuePair in (IEnumerable<KeyValuePair<string, AggregateOperator?>>) AggregateAliasToAggregateType)
        {
          string key = keyValuePair.Key;
          AggregateOperator? aggregateOperator = keyValuePair.Value;
          aliasToValue[key] = SingleGroupAggregator.AggregateValue.Create(aggregateOperator);
        }
        return new SingleGroupAggregator.SelectListAggregateValues((IReadOnlyDictionary<string, SingleGroupAggregator.AggregateValue>) aliasToValue, orderedAliases);
      }

      public override void AddValues(JToken values)
      {
        if (!(values is JObject jobject))
          throw new ArgumentException("values is not an object.");
        foreach (KeyValuePair<string, SingleGroupAggregator.AggregateValue> keyValuePair in (IEnumerable<KeyValuePair<string, SingleGroupAggregator.AggregateValue>>) this.aliasToValue)
        {
          string key = keyValuePair.Key;
          keyValuePair.Value.AddValue(jobject[key]);
        }
      }

      public override string ToString() => JsonConvert.SerializeObject((object) this.aliasToValue);
    }

    private abstract class AggregateValue
    {
      public abstract void AddValue(JToken AggregateValue);

      public abstract object Result { get; }

      public override string ToString() => this.Result.ToString();

      public static SingleGroupAggregator.AggregateValue Create(AggregateOperator? aggregateOperator) => !aggregateOperator.HasValue ? (SingleGroupAggregator.AggregateValue) SingleGroupAggregator.AggregateValue.ScalarAggregateValue.Create() : (SingleGroupAggregator.AggregateValue) SingleGroupAggregator.AggregateValue.AggregateAggregateValue.Create(aggregateOperator.Value);

      private sealed class AggregateAggregateValue : SingleGroupAggregator.AggregateValue
      {
        private readonly IAggregator aggregator;

        public override object Result => this.aggregator.GetResult();

        private AggregateAggregateValue(IAggregator aggregator) => this.aggregator = aggregator != null ? aggregator : throw new ArgumentNullException(nameof (aggregator));

        public override void AddValue(JToken AggregateValue) => this.aggregator.Aggregate(AggregateValue.ToObject<AggregateItem>().GetItem());

        public static SingleGroupAggregator.AggregateValue.AggregateAggregateValue Create(
          AggregateOperator aggregateOperator)
        {
          IAggregator aggregator;
          switch (aggregateOperator)
          {
            case AggregateOperator.Average:
              aggregator = (IAggregator) new AverageAggregator();
              break;
            case AggregateOperator.Count:
              aggregator = (IAggregator) new CountAggregator();
              break;
            case AggregateOperator.Max:
              aggregator = (IAggregator) new MinMaxAggregator(false);
              break;
            case AggregateOperator.Min:
              aggregator = (IAggregator) new MinMaxAggregator(true);
              break;
            case AggregateOperator.Sum:
              aggregator = (IAggregator) new SumAggregator();
              break;
            default:
              throw new ArgumentException(string.Format("Unknown {0}: {1}.", (object) "AggregateOperator", (object) aggregateOperator));
          }
          return new SingleGroupAggregator.AggregateValue.AggregateAggregateValue(aggregator);
        }
      }

      private sealed class ScalarAggregateValue : SingleGroupAggregator.AggregateValue
      {
        private JToken value;
        private bool initialized;

        private ScalarAggregateValue()
        {
          this.value = (JToken) null;
          this.initialized = false;
        }

        public override object Result
        {
          get
          {
            if (!this.initialized)
              throw new InvalidOperationException("ScalarAggregateValue is not yet initialized.");
            return this.value != null ? (object) this.value : (object) Undefined.Value;
          }
        }

        public static SingleGroupAggregator.AggregateValue.ScalarAggregateValue Create() => new SingleGroupAggregator.AggregateValue.ScalarAggregateValue();

        public override void AddValue(JToken AggregateValue)
        {
          if (this.initialized)
            return;
          this.value = AggregateValue;
          this.initialized = true;
        }
      }
    }
  }
}
