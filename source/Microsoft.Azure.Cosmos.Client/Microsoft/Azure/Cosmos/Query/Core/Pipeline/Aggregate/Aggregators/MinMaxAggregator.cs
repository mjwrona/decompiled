// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.Aggregators.MinMaxAggregator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate.Aggregators
{
  internal sealed class MinMaxAggregator : IAggregator
  {
    private readonly bool isMinAggregation;
    private CosmosElement globalMinMax;

    private MinMaxAggregator(bool isMinAggregation, CosmosElement globalMinMax)
    {
      this.isMinAggregation = isMinAggregation;
      this.globalMinMax = globalMinMax;
    }

    public void Aggregate(CosmosElement localMinMax)
    {
      if (this.globalMinMax is CosmosUndefined)
        return;
      if (localMinMax is CosmosUndefined)
      {
        this.globalMinMax = (CosmosElement) CosmosUndefined.Create();
      }
      else
      {
        CosmosNumber typedCosmosElement;
        if (localMinMax is CosmosObject cosmosObject && cosmosObject.TryGetValue<CosmosNumber>("count", out typedCosmosElement))
        {
          if (Number64.ToLong(typedCosmosElement.Value) == 0L)
            return;
          CosmosElement cosmosElement1;
          if (!cosmosObject.TryGetValue("min", out cosmosElement1))
            cosmosElement1 = (CosmosElement) null;
          CosmosElement cosmosElement2;
          if (!cosmosObject.TryGetValue("max", out cosmosElement2))
            cosmosElement2 = (CosmosElement) null;
          localMinMax = !(cosmosElement1 != (CosmosElement) null) ? (!(cosmosElement2 != (CosmosElement) null) ? (CosmosElement) CosmosUndefined.Create() : cosmosElement2) : cosmosElement1;
        }
        if (!ItemComparer.IsMinOrMax(this.globalMinMax) && (!MinMaxAggregator.IsPrimitve(localMinMax) || !MinMaxAggregator.IsPrimitve(this.globalMinMax)))
          this.globalMinMax = (CosmosElement) CosmosUndefined.Create();
        else if (this.isMinAggregation)
        {
          if (ItemComparer.Instance.Compare(localMinMax, this.globalMinMax) >= 0)
            return;
          this.globalMinMax = localMinMax;
        }
        else
        {
          if (ItemComparer.Instance.Compare(localMinMax, this.globalMinMax) <= 0)
            return;
          this.globalMinMax = localMinMax;
        }
      }
    }

    public CosmosElement GetResult() => this.globalMinMax == (CosmosElement) ItemComparer.MinValue || this.globalMinMax == (CosmosElement) ItemComparer.MaxValue ? (CosmosElement) CosmosUndefined.Create() : this.globalMinMax;

    public static TryCatch<IAggregator> TryCreateMinAggregator(CosmosElement continuationToken) => MinMaxAggregator.TryCreate(true, continuationToken);

    public static TryCatch<IAggregator> TryCreateMaxAggregator(CosmosElement continuationToken) => MinMaxAggregator.TryCreate(false, continuationToken);

    private static TryCatch<IAggregator> TryCreate(
      bool isMinAggregation,
      CosmosElement continuationToken)
    {
      CosmosElement globalMinMax;
      if (continuationToken != (CosmosElement) null)
      {
        TryCatch<MinMaxAggregator.MinMaxContinuationToken> fromCosmosElement = MinMaxAggregator.MinMaxContinuationToken.TryCreateFromCosmosElement(continuationToken);
        if (!fromCosmosElement.Succeeded)
          return TryCatch<IAggregator>.FromException(fromCosmosElement.Exception);
        switch (fromCosmosElement.Result.Type)
        {
          case MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.MinValue:
            globalMinMax = (CosmosElement) ItemComparer.MinValue;
            break;
          case MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.MaxValue:
            globalMinMax = (CosmosElement) ItemComparer.MaxValue;
            break;
          case MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.Undefined:
            globalMinMax = (CosmosElement) CosmosUndefined.Create();
            break;
          case MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.Value:
            globalMinMax = fromCosmosElement.Result.Value;
            break;
          default:
            throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}", (object) "MinMaxContinuationTokenType", (object) fromCosmosElement.Result.Type));
        }
      }
      else
        globalMinMax = isMinAggregation ? (CosmosElement) ItemComparer.MaxValue : (CosmosElement) ItemComparer.MinValue;
      return TryCatch<IAggregator>.FromResult((IAggregator) new MinMaxAggregator(isMinAggregation, globalMinMax));
    }

    private static bool IsPrimitve(CosmosElement cosmosElement) => cosmosElement.Accept<bool>((ICosmosElementVisitor<bool>) MinMaxAggregator.IsPrimitiveCosmosElementVisitor.Singleton);

    public CosmosElement GetCosmosElementContinuationToken() => MinMaxAggregator.MinMaxContinuationToken.ToCosmosElement(!(this.globalMinMax == (CosmosElement) ItemComparer.MinValue) ? (!(this.globalMinMax == (CosmosElement) ItemComparer.MaxValue) ? (!(this.globalMinMax is CosmosUndefined) ? MinMaxAggregator.MinMaxContinuationToken.CreateValueContinuationToken(this.globalMinMax) : MinMaxAggregator.MinMaxContinuationToken.CreateUndefinedValueContinuationToken()) : MinMaxAggregator.MinMaxContinuationToken.CreateMaxValueContinuationToken()) : MinMaxAggregator.MinMaxContinuationToken.CreateMinValueContinuationToken());

    private sealed class IsPrimitiveCosmosElementVisitor : ICosmosElementVisitor<bool>
    {
      public static readonly MinMaxAggregator.IsPrimitiveCosmosElementVisitor Singleton = new MinMaxAggregator.IsPrimitiveCosmosElementVisitor();

      private IsPrimitiveCosmosElementVisitor()
      {
      }

      public bool Visit(CosmosArray cosmosArray) => false;

      public bool Visit(CosmosBinary cosmosBinary) => true;

      public bool Visit(CosmosBoolean cosmosBoolean) => true;

      public bool Visit(CosmosGuid cosmosGuid) => true;

      public bool Visit(CosmosNull cosmosNull) => true;

      public bool Visit(CosmosNumber cosmosNumber) => true;

      public bool Visit(CosmosObject cosmosObject) => false;

      public bool Visit(CosmosString cosmosString) => true;

      public bool Visit(CosmosUndefined cosmosUndefined) => false;
    }

    private sealed class MinMaxContinuationToken
    {
      private MinMaxContinuationToken(
        MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType type,
        CosmosElement value)
      {
        switch (type)
        {
          case MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.MinValue:
          case MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.MaxValue:
          case MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.Undefined:
            if (value != (CosmosElement) null)
              throw new ArgumentException(string.Format("{0} must be set with type: {1}.", (object) nameof (value), (object) type));
            break;
          case MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.Value:
            if (value == (CosmosElement) null)
              throw new ArgumentException(string.Format("{0} must not be set with type: {1}.", (object) nameof (value), (object) type));
            break;
          default:
            throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}.", (object) nameof (type), (object) type));
        }
        this.Type = type;
        this.Value = value;
      }

      public MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType Type { get; }

      public CosmosElement Value { get; }

      public static MinMaxAggregator.MinMaxContinuationToken CreateMinValueContinuationToken() => new MinMaxAggregator.MinMaxContinuationToken(MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.MinValue, (CosmosElement) null);

      public static MinMaxAggregator.MinMaxContinuationToken CreateMaxValueContinuationToken() => new MinMaxAggregator.MinMaxContinuationToken(MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.MaxValue, (CosmosElement) null);

      public static MinMaxAggregator.MinMaxContinuationToken CreateUndefinedValueContinuationToken() => new MinMaxAggregator.MinMaxContinuationToken(MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.Undefined, (CosmosElement) null);

      public static MinMaxAggregator.MinMaxContinuationToken CreateValueContinuationToken(
        CosmosElement value)
      {
        return new MinMaxAggregator.MinMaxContinuationToken(MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.Value, value);
      }

      public static CosmosElement ToCosmosElement(
        MinMaxAggregator.MinMaxContinuationToken minMaxContinuationToken)
      {
        Dictionary<string, CosmosElement> dictionary = minMaxContinuationToken != null ? new Dictionary<string, CosmosElement>()
        {
          {
            "type",
            (CosmosElement) MinMaxAggregator.MinMaxContinuationToken.EnumToCosmosString.ConvertEnumToCosmosString(minMaxContinuationToken.Type)
          }
        } : throw new ArgumentNullException(nameof (minMaxContinuationToken));
        if (minMaxContinuationToken.Value != (CosmosElement) null)
          dictionary.Add("value", minMaxContinuationToken.Value);
        return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) dictionary);
      }

      public static TryCatch<MinMaxAggregator.MinMaxContinuationToken> TryCreateFromCosmosElement(
        CosmosElement cosmosElement)
      {
        if (!(cosmosElement is CosmosObject cosmosObject))
          return TryCatch<MinMaxAggregator.MinMaxContinuationToken>.FromException((Exception) new MalformedContinuationTokenException("MinMaxContinuationToken was not an object."));
        CosmosString typedCosmosElement;
        if (!cosmosObject.TryGetValue<CosmosString>("type", out typedCosmosElement))
          return TryCatch<MinMaxAggregator.MinMaxContinuationToken>.FromException((Exception) new MalformedContinuationTokenException("MinMaxContinuationToken is missing property: type."));
        MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType result;
        if (!Enum.TryParse<MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType>(UtfAnyString.op_Implicit(typedCosmosElement.Value), out result))
          return TryCatch<MinMaxAggregator.MinMaxContinuationToken>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} has malformed '{1}': {2}.", (object) nameof (MinMaxContinuationToken), (object) "type", (object) typedCosmosElement.Value)));
        CosmosElement cosmosElement1;
        if (result == MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.Value)
        {
          if (!cosmosObject.TryGetValue("value", out cosmosElement1))
            return TryCatch<MinMaxAggregator.MinMaxContinuationToken>.FromException((Exception) new MalformedContinuationTokenException("MinMaxContinuationToken is missing property: value."));
        }
        else
          cosmosElement1 = (CosmosElement) null;
        return TryCatch<MinMaxAggregator.MinMaxContinuationToken>.FromResult(new MinMaxAggregator.MinMaxContinuationToken(result, cosmosElement1));
      }

      private static class PropertyNames
      {
        public const string Type = "type";
        public const string Value = "value";
      }

      private static class EnumToCosmosString
      {
        private static readonly CosmosString MinValueCosmosString = CosmosString.Create(MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.MinValue.ToString());
        private static readonly CosmosString MaxValueCosmosString = CosmosString.Create(MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.MaxValue.ToString());
        private static readonly CosmosString UndefinedCosmosString = CosmosString.Create(MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.Undefined.ToString());
        private static readonly CosmosString ValueCosmosString = CosmosString.Create(MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.Value.ToString());

        public static CosmosString ConvertEnumToCosmosString(
          MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType type)
        {
          switch (type)
          {
            case MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.MinValue:
              return MinMaxAggregator.MinMaxContinuationToken.EnumToCosmosString.MinValueCosmosString;
            case MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.MaxValue:
              return MinMaxAggregator.MinMaxContinuationToken.EnumToCosmosString.MaxValueCosmosString;
            case MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.Undefined:
              return MinMaxAggregator.MinMaxContinuationToken.EnumToCosmosString.UndefinedCosmosString;
            case MinMaxAggregator.MinMaxContinuationToken.MinMaxContinuationTokenType.Value:
              return MinMaxAggregator.MinMaxContinuationToken.EnumToCosmosString.ValueCosmosString;
            default:
              throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}", (object) "MinMaxContinuationTokenType", (object) type));
          }
        }
      }

      public enum MinMaxContinuationTokenType
      {
        MinValue,
        MaxValue,
        Undefined,
        Value,
      }
    }
  }
}
