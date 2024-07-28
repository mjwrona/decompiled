// Decompiled with JetBrains decompiler
// Type: Nest.QuerySqlDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SqlApi;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class QuerySqlDescriptor : 
    RequestDescriptorBase<QuerySqlDescriptor, QuerySqlRequestParameters, IQuerySqlRequest>,
    IQuerySqlRequest,
    IRequest<QuerySqlRequestParameters>,
    IRequest,
    ISqlRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SqlQuery;

    public QuerySqlDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    bool? IQuerySqlRequest.Columnar { get; set; }

    string IQuerySqlRequest.Cursor { get; set; }

    int? ISqlRequest.FetchSize { get; set; }

    QueryContainer ISqlRequest.Filter { get; set; }

    IList<object> ISqlRequest.Params { get; set; }

    string ISqlRequest.Query { get; set; }

    IRuntimeFields ISqlRequest.RuntimeFields { get; set; }

    string ISqlRequest.TimeZone { get; set; }

    Time IQuerySqlRequest.WaitForCompletionTimeout { get; set; }

    public QuerySqlDescriptor Params(IEnumerable<object> parameters) => this.Assign<IEnumerable<object>>(parameters, (Action<IQuerySqlRequest, IEnumerable<object>>) ((a, v) => a.Params = v != null ? (IList<object>) v.ToListOrNullIfEmpty<object>() : (IList<object>) null));

    public QuerySqlDescriptor Params(IList<object> parameters) => this.Assign<IList<object>>(parameters, (Action<IQuerySqlRequest, IList<object>>) ((a, v) => a.Params = v));

    public QuerySqlDescriptor Params(params object[] parameters) => this.Assign<object[]>(parameters, (Action<IQuerySqlRequest, object[]>) ((a, v) => a.Params = (IList<object>) v));

    public QuerySqlDescriptor Query(string query) => this.Assign<string>(query, (Action<IQuerySqlRequest, string>) ((a, v) => a.Query = v));

    public QuerySqlDescriptor TimeZone(string timeZone) => this.Assign<string>(timeZone, (Action<IQuerySqlRequest, string>) ((a, v) => a.TimeZone = v));

    public QuerySqlDescriptor FetchSize(int? fetchSize) => this.Assign<int?>(fetchSize, (Action<IQuerySqlRequest, int?>) ((a, v) => a.FetchSize = v));

    public QuerySqlDescriptor Filter<T>(
      Func<QueryContainerDescriptor<T>, QueryContainer> querySelector)
      where T : class
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(querySelector, (Action<IQuerySqlRequest, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Filter = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public QuerySqlDescriptor Cursor(string cursor) => this.Assign<string>(cursor, (Action<IQuerySqlRequest, string>) ((a, v) => a.Cursor = v));

    public QuerySqlDescriptor Columnar(bool? columnar = true) => this.Assign<bool?>(columnar, (Action<IQuerySqlRequest, bool?>) ((a, v) => a.Columnar = v));

    public QuerySqlDescriptor RuntimeFields(
      Func<RuntimeFieldsDescriptor, IPromise<IRuntimeFields>> runtimeFieldsSelector)
    {
      return this.Assign<Func<RuntimeFieldsDescriptor, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<IQuerySqlRequest, Func<RuntimeFieldsDescriptor, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor())?.Value : (IRuntimeFields) null));
    }

    public QuerySqlDescriptor WaitForCompletionTimeout(Time frequency) => this.Assign<Time>(frequency, (Action<IQuerySqlRequest, Time>) ((a, v) => a.WaitForCompletionTimeout = v));
  }
}
