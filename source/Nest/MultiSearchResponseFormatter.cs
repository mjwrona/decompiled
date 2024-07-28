// Decompiled with JetBrains decompiler
// Type: Nest.MultiSearchResponseFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Nest
{
  internal class MultiSearchResponseFormatter : IJsonFormatter<MultiSearchResponse>, IJsonFormatter
  {
    private static readonly MethodInfo MakeDelegateMethodInfo = typeof (MultiSearchResponseFormatter).GetMethod("CreateSearchResponse", BindingFlags.Static | BindingFlags.NonPublic);
    private readonly IRequest _request;

    public MultiSearchResponseFormatter(IRequest request) => this._request = request;

    public MultiSearchResponse Deserialize(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (this._request == null)
        return (MultiSearchResponse) null;
      MultiSearchResponse multiSearchResponse = new MultiSearchResponse();
      List<ArraySegment<byte>> first = new List<ArraySegment<byte>>();
      int count1 = 0;
      while (reader.ReadIsInObject(ref count1))
      {
        switch (reader.ReadPropertyName())
        {
          case "responses":
            int count2 = 0;
            while (reader.ReadIsInArray(ref count2))
              first.Add(reader.ReadNextBlockSegment());
            goto label_10;
          case "took":
            multiSearchResponse.Took = reader.ReadInt64();
            continue;
          default:
            reader.ReadNextBlock();
            continue;
        }
      }
label_10:
      if (first.Count == 0)
        return multiSearchResponse;
      IEnumerable<MultiSearchResponseFormatter.SearchHitTuple> searchHitTuples;
      switch (this._request)
      {
        case IMultiSearchRequest multiSearchRequest:
          searchHitTuples = first.Zip<ArraySegment<byte>, KeyValuePair<string, ISearchRequest>, MultiSearchResponseFormatter.SearchHitTuple>((IEnumerable<KeyValuePair<string, ISearchRequest>>) multiSearchRequest.Operations, (Func<ArraySegment<byte>, KeyValuePair<string, ISearchRequest>, MultiSearchResponseFormatter.SearchHitTuple>) ((doc, desc) => new MultiSearchResponseFormatter.SearchHitTuple()
          {
            Hit = doc,
            Descriptor = new KeyValuePair<string, ITypedSearchRequest>(desc.Key, (ITypedSearchRequest) desc.Value)
          }));
          break;
        case IMultiSearchTemplateRequest searchTemplateRequest:
          searchHitTuples = first.Zip<ArraySegment<byte>, KeyValuePair<string, ISearchTemplateRequest>, MultiSearchResponseFormatter.SearchHitTuple>((IEnumerable<KeyValuePair<string, ISearchTemplateRequest>>) searchTemplateRequest.Operations, (Func<ArraySegment<byte>, KeyValuePair<string, ISearchTemplateRequest>, MultiSearchResponseFormatter.SearchHitTuple>) ((doc, desc) => new MultiSearchResponseFormatter.SearchHitTuple()
          {
            Hit = doc,
            Descriptor = new KeyValuePair<string, ITypedSearchRequest>(desc.Key, (ITypedSearchRequest) desc.Value)
          }));
          break;
        default:
          throw new InvalidOperationException("Request must be an instance of IMultiSearchRequest or IMultiSearchTemplateRequest");
      }
      IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
      foreach (MultiSearchResponseFormatter.SearchHitTuple searchHitTuple in searchHitTuples)
      {
        Type type = searchHitTuple.Descriptor.Value.ClrType;
        if ((object) type == null)
          type = typeof (object);
        Type key = type;
        connectionSettings.Inferrer.CreateSearchResponseDelegates.GetOrAdd(key, (Func<Type, Action<MultiSearchResponseFormatter.SearchHitTuple, IJsonFormatterResolver, IDictionary<string, IResponse>>>) (t =>
        {
          MethodInfo method = MultiSearchResponseFormatter.MakeDelegateMethodInfo.MakeGenericMethod(t);
          ParameterExpression[] parameterExpressionArray = new ParameterExpression[3]
          {
            Expression.Parameter(typeof (MultiSearchResponseFormatter.SearchHitTuple), "tuple"),
            Expression.Parameter(typeof (IJsonFormatterResolver), nameof (formatterResolver)),
            Expression.Parameter(typeof (IDictionary<string, IResponse>), "collection")
          };
          return Expression.Lambda<Action<MultiSearchResponseFormatter.SearchHitTuple, IJsonFormatterResolver, IDictionary<string, IResponse>>>((Expression) Expression.Call((Expression) null, method, (Expression[]) parameterExpressionArray), parameterExpressionArray).Compile();
        }))(searchHitTuple, formatterResolver, multiSearchResponse.Responses);
      }
      return multiSearchResponse;
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      MultiSearchResponse value,
      IJsonFormatterResolver formatterResolver)
    {
      DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<MultiSearchResponse>().Serialize(ref writer, value, formatterResolver);
    }

    private static void CreateSearchResponse<T>(
      MultiSearchResponseFormatter.SearchHitTuple tuple,
      IJsonFormatterResolver formatterResolver,
      IDictionary<string, IResponse> collection)
      where T : class
    {
      IJsonFormatter<SearchResponse<T>> formatter = formatterResolver.GetFormatter<SearchResponse<T>>();
      Elasticsearch.Net.Utf8Json.JsonReader reader;
      ref Elasticsearch.Net.Utf8Json.JsonReader local = ref reader;
      ArraySegment<byte> hit = tuple.Hit;
      byte[] array = hit.Array;
      hit = tuple.Hit;
      int offset = hit.Offset;
      local = new Elasticsearch.Net.Utf8Json.JsonReader(array, offset);
      SearchResponse<T> searchResponse = formatter.Deserialize(ref reader, formatterResolver);
      collection.Add(tuple.Descriptor.Key, (IResponse) searchResponse);
    }

    internal class SearchHitTuple
    {
      public KeyValuePair<string, ITypedSearchRequest> Descriptor { get; set; }

      public ArraySegment<byte> Hit { get; set; }
    }
  }
}
