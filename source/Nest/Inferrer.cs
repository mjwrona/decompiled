// Decompiled with JetBrains decompiler
// Type: Nest.Inferrer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nest
{
  public class Inferrer
  {
    private readonly IConnectionSettingsValues _connectionSettings;

    public Inferrer(IConnectionSettingsValues connectionSettings)
    {
      connectionSettings.ThrowIfNull<IConnectionSettingsValues>(nameof (connectionSettings));
      this._connectionSettings = connectionSettings;
      this.IdResolver = new IdResolver(connectionSettings);
      this.IndexNameResolver = new IndexNameResolver(connectionSettings);
      this.RelationNameResolver = new RelationNameResolver(connectionSettings);
      this.FieldResolver = new FieldResolver(connectionSettings);
      this.RoutingResolver = new RoutingResolver(connectionSettings, this.IdResolver);
      this.CreateMultiHitDelegates = new ConcurrentDictionary<Type, Action<MultiGetResponseFormatter.MultiHitTuple, IJsonFormatterResolver, ICollection<IMultiGetHit<object>>>>();
      this.CreateSearchResponseDelegates = new ConcurrentDictionary<Type, Action<MultiSearchResponseFormatter.SearchHitTuple, IJsonFormatterResolver, IDictionary<string, IResponse>>>();
    }

    internal ConcurrentDictionary<Type, Action<MultiGetResponseFormatter.MultiHitTuple, IJsonFormatterResolver, ICollection<IMultiGetHit<object>>>> CreateMultiHitDelegates { get; }

    internal ConcurrentDictionary<Type, Action<MultiSearchResponseFormatter.SearchHitTuple, IJsonFormatterResolver, IDictionary<string, IResponse>>> CreateSearchResponseDelegates { get; }

    private FieldResolver FieldResolver { get; }

    private IdResolver IdResolver { get; }

    private IndexNameResolver IndexNameResolver { get; }

    private RelationNameResolver RelationNameResolver { get; }

    private RoutingResolver RoutingResolver { get; }

    public string Resolve(IUrlParameter urlParameter) => urlParameter.GetString((IConnectionConfigurationValues) this._connectionSettings);

    public string Field(Nest.Field field) => this.FieldResolver.Resolve(field);

    public string PropertyName(Nest.PropertyName property) => this.FieldResolver.Resolve(property);

    public string IndexName<T>() where T : class => this.IndexNameResolver.Resolve<T>();

    public string IndexName(Nest.IndexName index) => this.IndexNameResolver.Resolve(index);

    public string Id<T>(T instance) where T : class => this.IdResolver.Resolve<T>(instance);

    public string Id(Type type, object instance) => this.IdResolver.Resolve(type, instance);

    public string RelationName<T>() where T : class => this.RelationNameResolver.Resolve<T>();

    public string RelationName(Nest.RelationName type) => this.RelationNameResolver.Resolve(type);

    public string Routing<T>(T document) => this.RoutingResolver.Resolve<T>(document);

    public string Routing(Type type, object instance) => this.RoutingResolver.Resolve(type, instance);
  }
}
