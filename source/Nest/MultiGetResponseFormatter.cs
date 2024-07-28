// Decompiled with JetBrains decompiler
// Type: Nest.MultiGetResponseFormatter
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
  internal class MultiGetResponseFormatter : IJsonFormatter<MultiGetResponse>, IJsonFormatter
  {
    private static readonly MethodInfo MakeDelegateMethodInfo = typeof (MultiGetResponseFormatter).GetMethod("CreateMultiHit", BindingFlags.Static | BindingFlags.NonPublic);
    private readonly IMultiGetRequest _request;

    public MultiGetResponseFormatter(IMultiGetRequest request) => this._request = request;

    public MultiGetResponse Deserialize(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (this._request == null)
        return (MultiGetResponse) null;
      MultiGetResponse multiGetResponse = new MultiGetResponse();
      List<ArraySegment<byte>> first = new List<ArraySegment<byte>>();
      int count1 = 0;
      while (reader.ReadIsInObject(ref count1))
      {
        if (reader.ReadPropertyName() == "docs")
        {
          int count2 = 0;
          while (reader.ReadIsInArray(ref count2))
            first.Add(reader.ReadNextBlockSegment());
          break;
        }
        reader.ReadNextBlock();
      }
      if (first.Count == 0)
        return multiGetResponse;
      foreach (MultiGetResponseFormatter.MultiHitTuple multiHitTuple in first.Zip<ArraySegment<byte>, IMultiGetOperation, MultiGetResponseFormatter.MultiHitTuple>(this._request.Documents, (Func<ArraySegment<byte>, IMultiGetOperation, MultiGetResponseFormatter.MultiHitTuple>) ((doc, desc) => new MultiGetResponseFormatter.MultiHitTuple()
      {
        Hit = doc,
        Descriptor = desc
      })))
        formatterResolver.GetConnectionSettings().Inferrer.CreateMultiHitDelegates.GetOrAdd(multiHitTuple.Descriptor.ClrType, (Func<Type, Action<MultiGetResponseFormatter.MultiHitTuple, IJsonFormatterResolver, ICollection<IMultiGetHit<object>>>>) (t =>
        {
          MethodInfo method = MultiGetResponseFormatter.MakeDelegateMethodInfo.MakeGenericMethod(t);
          ParameterExpression[] parameterExpressionArray = new ParameterExpression[3]
          {
            Expression.Parameter(typeof (MultiGetResponseFormatter.MultiHitTuple), "tuple"),
            Expression.Parameter(typeof (IJsonFormatterResolver), nameof (formatterResolver)),
            Expression.Parameter(typeof (ICollection<IMultiGetHit<object>>), "collection")
          };
          return Expression.Lambda<Action<MultiGetResponseFormatter.MultiHitTuple, IJsonFormatterResolver, ICollection<IMultiGetHit<object>>>>((Expression) Expression.Call((Expression) null, method, (Expression[]) parameterExpressionArray), parameterExpressionArray).Compile();
        }))(multiHitTuple, formatterResolver, multiGetResponse.InternalHits);
      return multiGetResponse;
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      MultiGetResponse value,
      IJsonFormatterResolver formatterResolver)
    {
      DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<MultiGetResponse>().Serialize(ref writer, value, formatterResolver);
    }

    private static void CreateMultiHit<T>(
      MultiGetResponseFormatter.MultiHitTuple tuple,
      IJsonFormatterResolver formatterResolver,
      ICollection<IMultiGetHit<object>> collection)
      where T : class
    {
      IJsonFormatter<MultiGetHit<T>> formatter = formatterResolver.GetFormatter<MultiGetHit<T>>();
      Elasticsearch.Net.Utf8Json.JsonReader reader;
      ref Elasticsearch.Net.Utf8Json.JsonReader local = ref reader;
      ArraySegment<byte> hit = tuple.Hit;
      byte[] array = hit.Array;
      hit = tuple.Hit;
      int offset = hit.Offset;
      local = new Elasticsearch.Net.Utf8Json.JsonReader(array, offset);
      MultiGetHit<T> multiGetHit = formatter.Deserialize(ref reader, formatterResolver);
      collection.Add((IMultiGetHit<object>) multiGetHit);
    }

    internal class MultiHitTuple
    {
      public IMultiGetOperation Descriptor { get; set; }

      public ArraySegment<byte> Hit { get; set; }
    }
  }
}
