// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.Aggregators.SingleGroupAggregator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.Aggregators
{
  internal abstract class SingleGroupAggregator
  {
    public abstract void AddValues(CosmosElement values);

    public abstract CosmosElement GetResult();

    public abstract CosmosElement GetCosmosElementContinuationToken();

    public static TryCatch<SingleGroupAggregator> TryCreate(
      IReadOnlyList<AggregateOperator> aggregates,
      IReadOnlyDictionary<string, AggregateOperator?> aggregateAliasToAggregateType,
      IReadOnlyList<string> orderedAliases,
      bool hasSelectValue,
      CosmosElement continuationToken)
    {
      if (aggregates == null)
        throw new ArgumentNullException(nameof (aggregates));
      if (aggregateAliasToAggregateType == null)
        throw new ArgumentNullException(nameof (aggregates));
      return !hasSelectValue ? SingleGroupAggregator.SelectListAggregateValues.TryCreate(aggregateAliasToAggregateType, orderedAliases, continuationToken) : (aggregates == null || !aggregates.Any<AggregateOperator>() ? SingleGroupAggregator.SelectValueAggregateValues.TryCreate(new AggregateOperator?(), continuationToken) : SingleGroupAggregator.SelectValueAggregateValues.TryCreate(new AggregateOperator?(aggregates[0]), continuationToken));
    }

    private sealed class SelectValueAggregateValues : SingleGroupAggregator
    {
      private readonly SingleGroupAggregator.AggregateValue aggregateValue;

      private SelectValueAggregateValues(
        SingleGroupAggregator.AggregateValue aggregateValue)
      {
        this.aggregateValue = aggregateValue ?? throw new ArgumentNullException("AggregateValue");
      }

      public static TryCatch<SingleGroupAggregator> TryCreate(
        AggregateOperator? aggregateOperator,
        CosmosElement continuationToken)
      {
        return SingleGroupAggregator.AggregateValue.TryCreate(aggregateOperator, continuationToken).Try<SingleGroupAggregator>((Func<SingleGroupAggregator.AggregateValue, SingleGroupAggregator>) (aggregateValue => (SingleGroupAggregator) new SingleGroupAggregator.SelectValueAggregateValues(aggregateValue)));
      }

      public override void AddValues(CosmosElement values) => this.aggregateValue.AddValue(values);

      public override CosmosElement GetResult() => this.aggregateValue.Result;

      public override CosmosElement GetCosmosElementContinuationToken() => this.aggregateValue.GetCosmosElementContinuationToken();

      public override string ToString() => this.aggregateValue.ToString();
    }

    private sealed class SelectListAggregateValues : SingleGroupAggregator
    {
      private readonly IReadOnlyDictionary<string, SingleGroupAggregator.AggregateValue> aliasToValue;
      private readonly IReadOnlyList<string> orderedAliases;

      private SelectListAggregateValues(
        IReadOnlyDictionary<string, SingleGroupAggregator.AggregateValue> aliasToValue,
        IReadOnlyList<string> orderedAliases)
      {
        this.aliasToValue = aliasToValue ?? throw new ArgumentNullException(nameof (aliasToValue));
        this.orderedAliases = orderedAliases ?? throw new ArgumentNullException(nameof (orderedAliases));
      }

      public override CosmosElement GetResult()
      {
        Dictionary<string, CosmosElement> dictionary = new Dictionary<string, CosmosElement>(this.orderedAliases.Count);
        List<string> keyOrdering = new List<string>(this.orderedAliases.Count);
        foreach (string orderedAlias in (IEnumerable<string>) this.orderedAliases)
        {
          SingleGroupAggregator.AggregateValue aggregateValue = this.aliasToValue[orderedAlias];
          if (!(aggregateValue.Result is CosmosUndefined))
          {
            dictionary[orderedAlias] = aggregateValue.Result;
            keyOrdering.Add(orderedAlias);
          }
        }
        return (CosmosElement) new SingleGroupAggregator.SelectListAggregateValues.OrderedCosmosObject(dictionary, (IReadOnlyList<string>) keyOrdering);
      }

      public override CosmosElement GetCosmosElementContinuationToken()
      {
        Dictionary<string, CosmosElement> dictionary = new Dictionary<string, CosmosElement>();
        foreach (KeyValuePair<string, SingleGroupAggregator.AggregateValue> keyValuePair in (IEnumerable<KeyValuePair<string, SingleGroupAggregator.AggregateValue>>) this.aliasToValue)
          dictionary.Add(keyValuePair.Key, keyValuePair.Value.GetCosmosElementContinuationToken());
        return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) dictionary);
      }

      public static TryCatch<SingleGroupAggregator> TryCreate(
        IReadOnlyDictionary<string, AggregateOperator?> aggregateAliasToAggregateType,
        IReadOnlyList<string> orderedAliases,
        CosmosElement continuationToken)
      {
        if (aggregateAliasToAggregateType == null)
          throw new ArgumentNullException(nameof (aggregateAliasToAggregateType));
        if (orderedAliases == null)
          throw new ArgumentNullException(nameof (orderedAliases));
        CosmosObject cosmosObject1;
        if (continuationToken != (CosmosElement) null)
        {
          if (!(continuationToken is CosmosObject cosmosObject2))
            return TryCatch<SingleGroupAggregator>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} continuation token is malformed: {1}.", (object) nameof (SelectListAggregateValues), (object) continuationToken)));
          cosmosObject1 = cosmosObject2;
        }
        else
          cosmosObject1 = (CosmosObject) null;
        Dictionary<string, SingleGroupAggregator.AggregateValue> aliasToValue = new Dictionary<string, SingleGroupAggregator.AggregateValue>();
        foreach (KeyValuePair<string, AggregateOperator?> keyValuePair in (IEnumerable<KeyValuePair<string, AggregateOperator?>>) aggregateAliasToAggregateType)
        {
          string key = keyValuePair.Key;
          TryCatch<SingleGroupAggregator.AggregateValue> tryCatch = SingleGroupAggregator.AggregateValue.TryCreate(keyValuePair.Value, !((CosmosElement) cosmosObject1 != (CosmosElement) null) ? (CosmosElement) null : cosmosObject1[key]);
          if (!tryCatch.Succeeded)
            return TryCatch<SingleGroupAggregator>.FromException(tryCatch.Exception);
          aliasToValue[key] = tryCatch.Result;
        }
        return TryCatch<SingleGroupAggregator>.FromResult((SingleGroupAggregator) new SingleGroupAggregator.SelectListAggregateValues((IReadOnlyDictionary<string, SingleGroupAggregator.AggregateValue>) aliasToValue, orderedAliases));
      }

      public override void AddValues(CosmosElement values)
      {
        if (!(values is CosmosObject cosmosObject))
          throw new ArgumentException("values is not an object.");
        foreach (KeyValuePair<string, SingleGroupAggregator.AggregateValue> keyValuePair in (IEnumerable<KeyValuePair<string, SingleGroupAggregator.AggregateValue>>) this.aliasToValue)
        {
          string key = keyValuePair.Key;
          SingleGroupAggregator.AggregateValue aggregateValue1 = keyValuePair.Value;
          CosmosElement cosmosElement;
          if (!cosmosObject.TryGetValue(key, out cosmosElement))
            cosmosElement = (CosmosElement) CosmosUndefined.Create();
          CosmosElement aggregateValue2 = cosmosElement;
          aggregateValue1.AddValue(aggregateValue2);
        }
      }

      public override string ToString() => JsonConvert.SerializeObject((object) this.aliasToValue);

      private sealed class OrderedCosmosObject : CosmosObject
      {
        private readonly Dictionary<string, CosmosElement> dictionary;
        private readonly IReadOnlyList<string> keyOrdering;

        public OrderedCosmosObject(
          Dictionary<string, CosmosElement> dictionary,
          IReadOnlyList<string> keyOrdering)
        {
          this.dictionary = dictionary;
          this.keyOrdering = keyOrdering;
          if (dictionary.Count != keyOrdering.Count)
            throw new ArgumentException("key counts don't add up.");
        }

        public override CosmosElement this[string key] => this.dictionary[key];

        public override CosmosObject.KeyCollection Keys => new CosmosObject.KeyCollection(this.dictionary.Keys);

        public override CosmosObject.ValueCollection Values => new CosmosObject.ValueCollection(this.dictionary.Values);

        public override int Count => this.dictionary.Count;

        public override bool ContainsKey(string key) => this.dictionary.ContainsKey(key);

        public override CosmosObject.Enumerator GetEnumerator() => new CosmosObject.Enumerator(this.dictionary.GetEnumerator());

        public override bool TryGetValue(string key, out CosmosElement value) => this.dictionary.TryGetValue(key, out value);

        public override void WriteTo(IJsonWriter jsonWriter)
        {
          jsonWriter.WriteObjectStart();
          foreach (string str in (IEnumerable<string>) this.keyOrdering)
          {
            CosmosElement cosmosElement = this[str];
            if (!(cosmosElement is CosmosUndefined))
            {
              jsonWriter.WriteFieldName(str);
              cosmosElement.WriteTo(jsonWriter);
            }
          }
          jsonWriter.WriteObjectEnd();
        }
      }
    }

    private abstract class AggregateValue
    {
      public abstract void AddValue(CosmosElement aggregateValue);

      public abstract CosmosElement Result { get; }

      public abstract CosmosElement GetCosmosElementContinuationToken();

      public override string ToString() => this.Result.ToString();

      public static TryCatch<SingleGroupAggregator.AggregateValue> TryCreate(
        AggregateOperator? aggregateOperator,
        CosmosElement continuationToken)
      {
        return !aggregateOperator.HasValue ? SingleGroupAggregator.AggregateValue.ScalarAggregateValue.TryCreate(continuationToken) : SingleGroupAggregator.AggregateValue.AggregateAggregateValue.TryCreate(aggregateOperator.Value, continuationToken);
      }

      private sealed class AggregateAggregateValue : SingleGroupAggregator.AggregateValue
      {
        private readonly IAggregator aggregator;

        public override CosmosElement Result => this.aggregator.GetResult();

        private AggregateAggregateValue(IAggregator aggregator) => this.aggregator = aggregator ?? throw new ArgumentNullException(nameof (aggregator));

        public override void AddValue(CosmosElement aggregateValue) => this.aggregator.Aggregate(new AggregateItem(aggregateValue).Item);

        public override CosmosElement GetCosmosElementContinuationToken() => this.aggregator.GetCosmosElementContinuationToken();

        public static TryCatch<SingleGroupAggregator.AggregateValue> TryCreate(
          AggregateOperator aggregateOperator,
          CosmosElement continuationToken)
        {
          TryCatch<IAggregator> tryCatch;
          switch (aggregateOperator)
          {
            case AggregateOperator.Average:
              tryCatch = AverageAggregator.TryCreate(continuationToken);
              break;
            case AggregateOperator.Count:
              tryCatch = CountAggregator.TryCreate(continuationToken);
              break;
            case AggregateOperator.Max:
              tryCatch = MinMaxAggregator.TryCreateMaxAggregator(continuationToken);
              break;
            case AggregateOperator.Min:
              tryCatch = MinMaxAggregator.TryCreateMinAggregator(continuationToken);
              break;
            case AggregateOperator.Sum:
              tryCatch = SumAggregator.TryCreate(continuationToken);
              break;
            default:
              throw new ArgumentException(string.Format("Unknown {0}: {1}.", (object) "AggregateOperator", (object) aggregateOperator));
          }
          return tryCatch.Try<SingleGroupAggregator.AggregateValue>((Func<IAggregator, SingleGroupAggregator.AggregateValue>) (aggregator => (SingleGroupAggregator.AggregateValue) new SingleGroupAggregator.AggregateValue.AggregateAggregateValue(aggregator)));
        }
      }

      private sealed class ScalarAggregateValue : SingleGroupAggregator.AggregateValue
      {
        private CosmosElement value;
        private bool initialized;

        private ScalarAggregateValue(CosmosElement initialValue, bool initialized)
        {
          this.value = initialValue;
          this.initialized = initialized;
        }

        public override CosmosElement Result
        {
          get
          {
            if (!this.initialized)
              throw new InvalidOperationException("ScalarAggregateValue is not yet initialized.");
            return this.value;
          }
        }

        public override CosmosElement GetCosmosElementContinuationToken()
        {
          Dictionary<string, CosmosElement> dictionary = new Dictionary<string, CosmosElement>()
          {
            {
              "initialized",
              (CosmosElement) CosmosBoolean.Create(this.initialized)
            }
          };
          if (this.value != (CosmosElement) null)
            dictionary.Add("value", this.value);
          return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) dictionary);
        }

        public static TryCatch<SingleGroupAggregator.AggregateValue> TryCreate(
          CosmosElement continuationToken)
        {
          CosmosElement initialValue;
          bool initialized;
          if (continuationToken != (CosmosElement) null)
          {
            if (!(continuationToken is CosmosObject cosmosObject))
              return TryCatch<SingleGroupAggregator.AggregateValue>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Invalid {0}: {1}", (object) nameof (ScalarAggregateValue), (object) continuationToken)));
            CosmosBoolean typedCosmosElement;
            if (!cosmosObject.TryGetValue<CosmosBoolean>("initialized", out typedCosmosElement))
              return TryCatch<SingleGroupAggregator.AggregateValue>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Invalid {0}: {1}", (object) nameof (ScalarAggregateValue), (object) continuationToken)));
            if (!cosmosObject.TryGetValue("value", out initialValue))
              initialValue = (CosmosElement) CosmosUndefined.Create();
            initialized = typedCosmosElement.Value;
          }
          else
          {
            initialValue = (CosmosElement) null;
            initialized = false;
          }
          return TryCatch<SingleGroupAggregator.AggregateValue>.FromResult((SingleGroupAggregator.AggregateValue) new SingleGroupAggregator.AggregateValue.ScalarAggregateValue(initialValue, initialized));
        }

        public override void AddValue(CosmosElement aggregateValue)
        {
          if (this.initialized)
            return;
          this.value = aggregateValue;
          this.initialized = true;
        }
      }
    }
  }
}
