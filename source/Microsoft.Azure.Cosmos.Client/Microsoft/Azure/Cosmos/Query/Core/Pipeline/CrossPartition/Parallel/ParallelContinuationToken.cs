// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel.ParallelContinuationToken
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel
{
  internal sealed class ParallelContinuationToken : IPartitionedToken
  {
    public ParallelContinuationToken(string token, Microsoft.Azure.Documents.Routing.Range<string> range)
    {
      this.Token = token;
      this.Range = range ?? throw new ArgumentNullException(nameof (range));
    }

    public string Token { get; }

    public Microsoft.Azure.Documents.Routing.Range<string> Range { get; }

    public static CosmosElement ToCosmosElement(
      ParallelContinuationToken parallelContinuationToken)
    {
      return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
      {
        {
          "token",
          parallelContinuationToken.Token == null ? (CosmosElement) CosmosNull.Create() : (CosmosElement) CosmosString.Create(parallelContinuationToken.Token)
        },
        {
          "range",
          (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
          {
            {
              "min",
              (CosmosElement) CosmosString.Create(parallelContinuationToken.Range.Min)
            },
            {
              "max",
              (CosmosElement) CosmosString.Create(parallelContinuationToken.Range.Max)
            }
          })
        }
      });
    }

    public static TryCatch<ParallelContinuationToken> TryCreateFromCosmosElement(
      CosmosElement cosmosElement)
    {
      if (!(cosmosElement is CosmosObject cosmosObject))
        return TryCatch<ParallelContinuationToken>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} is not an object: {1}", (object) nameof (ParallelContinuationToken), (object) cosmosElement)));
      CosmosElement cosmosElement1;
      if (!cosmosObject.TryGetValue("token", out cosmosElement1))
        return TryCatch<ParallelContinuationToken>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} is missing field: '{1}': {2}", (object) nameof (ParallelContinuationToken), (object) "token", (object) cosmosElement)));
      string token = !(cosmosElement1 is CosmosString cosmosString) ? (string) null : UtfAnyString.op_Implicit(cosmosString.Value);
      CosmosObject typedCosmosElement1;
      if (!cosmosObject.TryGetValue<CosmosObject>("range", out typedCosmosElement1))
        return TryCatch<ParallelContinuationToken>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} is missing field: '{1}': {2}", (object) nameof (ParallelContinuationToken), (object) "range", (object) cosmosElement)));
      CosmosString typedCosmosElement2;
      if (!typedCosmosElement1.TryGetValue<CosmosString>("min", out typedCosmosElement2))
        return TryCatch<ParallelContinuationToken>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} is missing field: '{1}': {2}", (object) nameof (ParallelContinuationToken), (object) "min", (object) cosmosElement)));
      string min = UtfAnyString.op_Implicit(typedCosmosElement2.Value);
      CosmosString typedCosmosElement3;
      if (!typedCosmosElement1.TryGetValue<CosmosString>("max", out typedCosmosElement3))
        return TryCatch<ParallelContinuationToken>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} is missing field: '{1}': {2}", (object) nameof (ParallelContinuationToken), (object) "max", (object) cosmosElement)));
      string max = UtfAnyString.op_Implicit(typedCosmosElement3.Value);
      Microsoft.Azure.Documents.Routing.Range<string> range = new Microsoft.Azure.Documents.Routing.Range<string>(min, max, true, false);
      return TryCatch<ParallelContinuationToken>.FromResult(new ParallelContinuationToken(token, range));
    }

    private static class PropertyNames
    {
      public const string Token = "token";
      public const string Range = "range";
      public const string Min = "min";
      public const string Max = "max";
    }
  }
}
