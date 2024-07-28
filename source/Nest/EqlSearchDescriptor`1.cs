// Decompiled with JetBrains decompiler
// Type: Nest.EqlSearchDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.EqlApi;
using System;
using System.Linq.Expressions;

namespace Nest
{
  public class EqlSearchDescriptor<TInferDocument> : 
    RequestDescriptorBase<EqlSearchDescriptor<TInferDocument>, EqlSearchRequestParameters, IEqlSearchRequest<TInferDocument>>,
    IEqlSearchRequest<TInferDocument>,
    IEqlSearchRequest,
    IRequest<EqlSearchRequestParameters>,
    IRequest,
    ITypedSearchRequest
    where TInferDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.EqlSearch;

    public EqlSearchDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    public EqlSearchDescriptor()
      : this((Indices) typeof (TInferDocument))
    {
    }

    Indices IEqlSearchRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public EqlSearchDescriptor<TInferDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<IEqlSearchRequest<TInferDocument>, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public EqlSearchDescriptor<TInferDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IEqlSearchRequest<TInferDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public EqlSearchDescriptor<TInferDocument> AllIndices() => this.Index(Indices.All);

    public EqlSearchDescriptor<TInferDocument> KeepAlive(Time keepalive) => this.Qs("keep_alive", (object) keepalive);

    public EqlSearchDescriptor<TInferDocument> KeepOnCompletion(bool? keeponcompletion = true) => this.Qs("keep_on_completion", (object) keeponcompletion);

    public EqlSearchDescriptor<TInferDocument> WaitForCompletionTimeout(
      Time waitforcompletiontimeout)
    {
      return this.Qs("wait_for_completion_timeout", (object) waitforcompletiontimeout);
    }

    Type ITypedSearchRequest.ClrType => typeof (TInferDocument);

    Field IEqlSearchRequest.EventCategoryField { get; set; }

    int? IEqlSearchRequest.FetchSize { get; set; }

    Nest.Fields IEqlSearchRequest.Fields { get; set; }

    QueryContainer IEqlSearchRequest.Filter { get; set; }

    string IEqlSearchRequest.Query { get; set; }

    EqlResultPosition? IEqlSearchRequest.ResultPosition { get; set; }

    IRuntimeFields IEqlSearchRequest.RuntimeFields { get; set; }

    float? IEqlSearchRequest.Size { get; set; }

    Field IEqlSearchRequest.TiebreakerField { get; set; }

    Field IEqlSearchRequest.TimestampField { get; set; }

    public EqlSearchDescriptor<TInferDocument> EventCategoryField(Field eventCategoryField) => this.Assign<Field>(eventCategoryField, (Action<IEqlSearchRequest<TInferDocument>, Field>) ((a, v) => a.EventCategoryField = v));

    public EqlSearchDescriptor<TInferDocument> EventCategoryField<TValue>(
      Expression<Func<TInferDocument, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<TInferDocument, TValue>>>(objectPath, (Action<IEqlSearchRequest<TInferDocument>, Expression<Func<TInferDocument, TValue>>>) ((a, v) => a.EventCategoryField = (Field) (Expression) v));
    }

    public EqlSearchDescriptor<TInferDocument> FetchSize(int? fetchSize) => this.Assign<int?>(fetchSize, (Action<IEqlSearchRequest<TInferDocument>, int?>) ((a, v) => a.FetchSize = v));

    public EqlSearchDescriptor<TInferDocument> Fields(
      Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>(fields, (Action<IEqlSearchRequest<TInferDocument>, Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<TInferDocument>())?.Value : (Nest.Fields) null));
    }

    public EqlSearchDescriptor<TInferDocument> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<IEqlSearchRequest<TInferDocument>, Nest.Fields>) ((a, v) => a.Fields = v));

    public EqlSearchDescriptor<TInferDocument> Filter(
      Func<QueryContainerDescriptor<TInferDocument>, QueryContainer> filter)
    {
      return this.Assign<Func<QueryContainerDescriptor<TInferDocument>, QueryContainer>>(filter, (Action<IEqlSearchRequest<TInferDocument>, Func<QueryContainerDescriptor<TInferDocument>, QueryContainer>>) ((a, v) => a.Filter = v != null ? v(new QueryContainerDescriptor<TInferDocument>()) : (QueryContainer) null));
    }

    public EqlSearchDescriptor<TInferDocument> Query(string query) => this.Assign<string>(query, (Action<IEqlSearchRequest<TInferDocument>, string>) ((a, v) => a.Query = v));

    public EqlSearchDescriptor<TInferDocument> ResultPosition(EqlResultPosition? resultPosition) => this.Assign<EqlResultPosition?>(resultPosition, (Action<IEqlSearchRequest<TInferDocument>, EqlResultPosition?>) ((a, v) => a.ResultPosition = v));

    public EqlSearchDescriptor<TInferDocument> RuntimeFields(
      Func<RuntimeFieldsDescriptor, IPromise<IRuntimeFields>> runtimeFieldsSelector)
    {
      return this.Assign<Func<RuntimeFieldsDescriptor, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<IEqlSearchRequest<TInferDocument>, Func<RuntimeFieldsDescriptor, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor())?.Value : (IRuntimeFields) null));
    }

    public EqlSearchDescriptor<TInferDocument> Size(float? size) => this.Assign<float?>(size, (Action<IEqlSearchRequest<TInferDocument>, float?>) ((a, v) => a.Size = v));

    public EqlSearchDescriptor<TInferDocument> TiebreakerField(Field tiebreakerField) => this.Assign<Field>(tiebreakerField, (Action<IEqlSearchRequest<TInferDocument>, Field>) ((a, v) => a.TiebreakerField = v));

    public EqlSearchDescriptor<TInferDocument> TiebreakerField<TValue>(
      Expression<Func<TInferDocument, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<TInferDocument, TValue>>>(objectPath, (Action<IEqlSearchRequest<TInferDocument>, Expression<Func<TInferDocument, TValue>>>) ((a, v) => a.TiebreakerField = (Field) (Expression) v));
    }

    public EqlSearchDescriptor<TInferDocument> TimestampField(Field timestampField) => this.Assign<Field>(timestampField, (Action<IEqlSearchRequest<TInferDocument>, Field>) ((a, v) => a.TimestampField = v));

    public EqlSearchDescriptor<TInferDocument> TimestampField<TValue>(
      Expression<Func<TInferDocument, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<TInferDocument, TValue>>>(objectPath, (Action<IEqlSearchRequest<TInferDocument>, Expression<Func<TInferDocument, TValue>>>) ((a, v) => a.TimestampField = (Field) (Expression) v));
    }
  }
}
