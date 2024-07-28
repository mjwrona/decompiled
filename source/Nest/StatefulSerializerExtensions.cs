// Decompiled with JetBrains decompiler
// Type: Nest.StatefulSerializerExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal static class StatefulSerializerExtensions
  {
    public static DefaultHighLevelSerializer CreateStateful<T>(
      this IElasticsearchSerializer serializer,
      IJsonFormatter<T> formatter)
    {
      IJsonFormatterResolver formatterResolver;
      if (!(serializer is IInternalSerializer internalSerializer) || !internalSerializer.TryGetJsonFormatter(out formatterResolver))
        throw new Exception(string.Format("Can not create a stateful serializer because {0} does not yield a json formatter", (object) serializer.GetType()));
      return new DefaultHighLevelSerializer((IJsonFormatterResolver) new StatefulSerializerExtensions.StatefulFormatterResolver<T>(formatter, formatterResolver));
    }

    private class StatefulFormatterResolver<TStateful> : 
      IJsonFormatterResolver,
      IJsonFormatterResolverWithSettings
    {
      private readonly IJsonFormatter<TStateful> _jsonFormatter;
      private readonly IJsonFormatterResolver _formatterResolver;

      public StatefulFormatterResolver(
        IJsonFormatter<TStateful> jsonFormatter,
        IJsonFormatterResolver formatterResolver)
      {
        this._jsonFormatter = jsonFormatter;
        this._formatterResolver = formatterResolver;
      }

      public IJsonFormatter<T> GetFormatter<T>() => typeof (T) == typeof (TStateful) ? (IJsonFormatter<T>) this._jsonFormatter : this._formatterResolver.GetFormatter<T>();

      public IConnectionSettingsValues Settings => ((IJsonFormatterResolverWithSettings) this._formatterResolver).Settings;
    }
  }
}
