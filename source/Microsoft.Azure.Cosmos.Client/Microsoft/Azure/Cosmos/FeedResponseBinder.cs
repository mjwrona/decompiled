// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedResponseBinder
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Documents;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Cosmos
{
  internal static class FeedResponseBinder
  {
    public static DocumentFeedResponse<T> Convert<T>(DocumentFeedResponse<object> dynamicFeed)
    {
      if (typeof (T) == typeof (object))
        return (DocumentFeedResponse<T>) dynamicFeed;
      IList<T> result = (IList<T>) new List<T>();
      foreach (object obj1 in dynamicFeed)
      {
        // ISSUE: reference to a compiler-generated field
        if (FeedResponseBinder.\u003C\u003Eo__0<T>.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          FeedResponseBinder.\u003C\u003Eo__0<T>.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (T), typeof (FeedResponseBinder)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        T obj2 = FeedResponseBinder.\u003C\u003Eo__0<T>.\u003C\u003Ep__0.Target((CallSite) FeedResponseBinder.\u003C\u003Eo__0<T>.\u003C\u003Ep__0, obj1);
        result.Add(obj2);
      }
      return new DocumentFeedResponse<T>((IEnumerable<T>) result, dynamicFeed.Count, dynamicFeed.Headers, dynamicFeed.UseETagAsContinuation, dynamicFeed.QueryMetrics, dynamicFeed.RequestStatistics, responseLengthBytes: dynamicFeed.ResponseLengthBytes);
    }

    public static DocumentFeedResponse<T> ConvertCosmosElementFeed<T>(
      DocumentFeedResponse<CosmosElement> dynamicFeed,
      ResourceType resourceType,
      JsonSerializerSettings settings)
    {
      if (dynamicFeed.Count == 0)
        return new DocumentFeedResponse<T>((IEnumerable<T>) new List<T>(), dynamicFeed.Count, dynamicFeed.Headers, dynamicFeed.UseETagAsContinuation, dynamicFeed.QueryMetrics, dynamicFeed.RequestStatistics, dynamicFeed.DisallowContinuationTokenMessage, dynamicFeed.ResponseLengthBytes);
      IJsonWriter jsonWriter = Microsoft.Azure.Cosmos.Json.JsonWriter.Create(JsonSerializationFormat.Text);
      jsonWriter.WriteArrayStart();
      foreach (CosmosElement cosmosElement in dynamicFeed)
        cosmosElement.WriteTo(jsonWriter);
      jsonWriter.WriteArrayEnd();
      string str = Utf8StringHelpers.ToString(jsonWriter.GetResult());
      return new DocumentFeedResponse<T>(resourceType != ResourceType.Offer || !typeof (T).IsSubclassOf(typeof (Resource)) && !(typeof (T) == typeof (object)) ? (IEnumerable<T>) JsonConvert.DeserializeObject<List<T>>(str, settings) : JsonConvert.DeserializeObject<List<OfferV2>>(str, settings).Cast<T>(), dynamicFeed.Count, dynamicFeed.Headers, dynamicFeed.UseETagAsContinuation, dynamicFeed.QueryMetrics, dynamicFeed.RequestStatistics, dynamicFeed.DisallowContinuationTokenMessage, dynamicFeed.ResponseLengthBytes);
    }
  }
}
