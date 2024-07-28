// Decompiled with JetBrains decompiler
// Type: Nest.TranslateSqlDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SqlApi;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class TranslateSqlDescriptor : 
    RequestDescriptorBase<TranslateSqlDescriptor, TranslateSqlRequestParameters, ITranslateSqlRequest>,
    ITranslateSqlRequest,
    IRequest<TranslateSqlRequestParameters>,
    IRequest,
    ISqlRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SqlTranslate;

    protected override sealed void RequestDefaults(TranslateSqlRequestParameters parameters) => parameters.CustomResponseBuilder = (CustomResponseBuilderBase) TranslateSqlResponseBuilder.Instance;

    int? ISqlRequest.FetchSize { get; set; }

    QueryContainer ISqlRequest.Filter { get; set; }

    IList<object> ISqlRequest.Params { get; set; }

    string ISqlRequest.Query { get; set; }

    string ISqlRequest.TimeZone { get; set; }

    IRuntimeFields ISqlRequest.RuntimeFields { get; set; }

    public TranslateSqlDescriptor Params(IEnumerable<object> parameters) => this.Assign<IEnumerable<object>>(parameters, (Action<ITranslateSqlRequest, IEnumerable<object>>) ((a, v) => a.Params = v != null ? (IList<object>) v.ToListOrNullIfEmpty<object>() : (IList<object>) null));

    public TranslateSqlDescriptor Params(IList<object> parameters) => this.Assign<IList<object>>(parameters, (Action<ITranslateSqlRequest, IList<object>>) ((a, v) => a.Params = v));

    public TranslateSqlDescriptor Params(params object[] parameters) => this.Assign<object[]>(parameters, (Action<ITranslateSqlRequest, object[]>) ((a, v) => a.Params = (IList<object>) v));

    public TranslateSqlDescriptor Query(string query) => this.Assign<string>(query, (Action<ITranslateSqlRequest, string>) ((a, v) => a.Query = v));

    public TranslateSqlDescriptor TimeZone(string timeZone) => this.Assign<string>(timeZone, (Action<ITranslateSqlRequest, string>) ((a, v) => a.TimeZone = v));

    public TranslateSqlDescriptor FetchSize(int? fetchSize) => this.Assign<int?>(fetchSize, (Action<ITranslateSqlRequest, int?>) ((a, v) => a.FetchSize = v));

    public TranslateSqlDescriptor Filter<T>(
      Func<QueryContainerDescriptor<T>, QueryContainer> querySelector)
      where T : class
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(querySelector, (Action<ITranslateSqlRequest, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Filter = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public TranslateSqlDescriptor RuntimeFields<TSource>(
      Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>> runtimeFieldsSelector)
      where TSource : class
    {
      return this.Assign<Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<ITranslateSqlRequest, Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor<TSource>())?.Value : (IRuntimeFields) null));
    }
  }
}
