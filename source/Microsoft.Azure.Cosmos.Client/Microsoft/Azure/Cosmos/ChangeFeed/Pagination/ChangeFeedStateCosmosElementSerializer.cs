// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedStateCosmosElementSerializer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Pagination
{
  internal static class ChangeFeedStateCosmosElementSerializer
  {
    private const string TypePropertyName = "type";
    private const string ValuePropertyName = "value";
    private const string BeginningTypeValue = "beginning";
    private const string TimeTypeValue = "time";
    private const string ContinuationTypeValue = "continuation";
    private const string NowTypeValue = "now";

    public static TryCatch<ChangeFeedState> MonadicFromCosmosElement(CosmosElement cosmosElement)
    {
      if (cosmosElement == (CosmosElement) null)
        throw new ArgumentNullException(nameof (cosmosElement));
      if (!(cosmosElement is CosmosObject cosmosObject))
        return TryCatch<ChangeFeedState>.FromException((Exception) new FormatException(string.Format("expected change feed state to be an object: {0}", (object) cosmosElement)));
      CosmosString typedCosmosElement1;
      if (!cosmosObject.TryGetValue<CosmosString>("type", out typedCosmosElement1))
        return TryCatch<ChangeFeedState>.FromException((Exception) new FormatException(string.Format("expected change feed state to have a string type property: {0}", (object) cosmosElement)));
      ChangeFeedState result1;
      switch (UtfAnyString.op_Implicit(typedCosmosElement1.Value))
      {
        case "beginning":
          result1 = ChangeFeedState.Beginning();
          break;
        case "time":
          CosmosString typedCosmosElement2;
          if (!cosmosObject.TryGetValue<CosmosString>("value", out typedCosmosElement2))
            return TryCatch<ChangeFeedState>.FromException((Exception) new FormatException(string.Format("expected change feed state to have a string value property: {0}", (object) cosmosElement)));
          DateTime result2;
          if (!DateTime.TryParse(UtfAnyString.op_Implicit(typedCosmosElement2.Value), (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result2))
            return TryCatch<ChangeFeedState>.FromException((Exception) new FormatException(string.Format("failed to parse start time value: {0}", (object) cosmosElement)));
          result1 = ChangeFeedState.Time(result2);
          break;
        case "continuation":
          CosmosString typedCosmosElement3;
          if (!cosmosObject.TryGetValue<CosmosString>("value", out typedCosmosElement3))
            return TryCatch<ChangeFeedState>.FromException((Exception) new FormatException(string.Format("expected change feed state to have a string value property: {0}", (object) cosmosElement)));
          result1 = ChangeFeedState.Continuation((CosmosElement) typedCosmosElement3);
          break;
        case "now":
          result1 = ChangeFeedState.Now();
          break;
        default:
          throw new InvalidOperationException();
      }
      return TryCatch<ChangeFeedState>.FromResult(result1);
    }

    public static CosmosElement ToCosmosElement(ChangeFeedState changeFeedState) => changeFeedState != null ? changeFeedState.Accept<CosmosElement>((IChangeFeedStateTransformer<CosmosElement>) ChangeFeedStateCosmosElementSerializer.ChangeFeedToCosmosElementVisitor.Singleton) : throw new ArgumentNullException(nameof (changeFeedState));

    private sealed class ChangeFeedToCosmosElementVisitor : 
      IChangeFeedStateTransformer<CosmosElement>
    {
      public static readonly ChangeFeedStateCosmosElementSerializer.ChangeFeedToCosmosElementVisitor Singleton = new ChangeFeedStateCosmosElementSerializer.ChangeFeedToCosmosElementVisitor();
      private static readonly CosmosElement BegininningSingleton = (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
      {
        {
          "type",
          (CosmosElement) CosmosString.Create("beginning")
        }
      });
      private static readonly CosmosElement NowSingleton = (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
      {
        {
          "type",
          (CosmosElement) CosmosString.Create("now")
        }
      });
      private static readonly CosmosString TimeTypeValueSingleton = CosmosString.Create("time");
      private static readonly CosmosString ContinuationTypeValueSingleton = CosmosString.Create("continuation");

      private ChangeFeedToCosmosElementVisitor()
      {
      }

      public CosmosElement Transform(ChangeFeedStateBeginning changeFeedStateBeginning) => ChangeFeedStateCosmosElementSerializer.ChangeFeedToCosmosElementVisitor.BegininningSingleton;

      public CosmosElement Transform(ChangeFeedStateTime changeFeedStateTime) => (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
      {
        {
          "type",
          (CosmosElement) ChangeFeedStateCosmosElementSerializer.ChangeFeedToCosmosElementVisitor.TimeTypeValueSingleton
        },
        {
          "value",
          (CosmosElement) CosmosString.Create(changeFeedStateTime.StartTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture))
        }
      });

      public CosmosElement Transform(
        ChangeFeedStateContinuation changeFeedStateContinuation)
      {
        return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
        {
          {
            "type",
            (CosmosElement) ChangeFeedStateCosmosElementSerializer.ChangeFeedToCosmosElementVisitor.ContinuationTypeValueSingleton
          },
          {
            "value",
            changeFeedStateContinuation.ContinuationToken
          }
        });
      }

      public CosmosElement Transform(ChangeFeedStateNow changeFeedStateNow) => ChangeFeedStateCosmosElementSerializer.ChangeFeedToCosmosElementVisitor.NowSingleton;
    }
  }
}
