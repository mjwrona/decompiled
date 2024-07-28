// Decompiled with JetBrains decompiler
// Type: Nest.Specification.SqlApi.SqlNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.SqlApi
{
  public class SqlNamespace : Nest.NamespacedClientProxy
  {
    internal SqlNamespace(ElasticClient client)
      : base(client)
    {
    }

    public ClearSqlCursorResponse ClearCursor(
      Func<ClearSqlCursorDescriptor, IClearSqlCursorRequest> selector)
    {
      return this.ClearCursor(selector.InvokeOrDefault<ClearSqlCursorDescriptor, IClearSqlCursorRequest>(new ClearSqlCursorDescriptor()));
    }

    public Task<ClearSqlCursorResponse> ClearCursorAsync(
      Func<ClearSqlCursorDescriptor, IClearSqlCursorRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ClearCursorAsync(selector.InvokeOrDefault<ClearSqlCursorDescriptor, IClearSqlCursorRequest>(new ClearSqlCursorDescriptor()), ct);
    }

    public ClearSqlCursorResponse ClearCursor(IClearSqlCursorRequest request) => this.DoRequest<IClearSqlCursorRequest, ClearSqlCursorResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClearSqlCursorResponse> ClearCursorAsync(
      IClearSqlCursorRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClearSqlCursorRequest, ClearSqlCursorResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public SqlDeleteResponse Delete(
      Id id,
      Func<SqlDeleteDescriptor, ISqlDeleteRequest> selector = null)
    {
      return this.Delete(selector.InvokeOrDefault<SqlDeleteDescriptor, ISqlDeleteRequest>(new SqlDeleteDescriptor(id)));
    }

    public Task<SqlDeleteResponse> DeleteAsync(
      Id id,
      Func<SqlDeleteDescriptor, ISqlDeleteRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteAsync(selector.InvokeOrDefault<SqlDeleteDescriptor, ISqlDeleteRequest>(new SqlDeleteDescriptor(id)), ct);
    }

    public SqlDeleteResponse Delete(ISqlDeleteRequest request) => this.DoRequest<ISqlDeleteRequest, SqlDeleteResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<SqlDeleteResponse> DeleteAsync(ISqlDeleteRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ISqlDeleteRequest, SqlDeleteResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public SqlGetResponse Get(Id id, Func<SqlGetDescriptor, ISqlGetRequest> selector = null) => this.Get(selector.InvokeOrDefault<SqlGetDescriptor, ISqlGetRequest>(new SqlGetDescriptor(id)));

    public Task<SqlGetResponse> GetAsync(
      Id id,
      Func<SqlGetDescriptor, ISqlGetRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetAsync(selector.InvokeOrDefault<SqlGetDescriptor, ISqlGetRequest>(new SqlGetDescriptor(id)), ct);
    }

    public SqlGetResponse Get(ISqlGetRequest request) => this.DoRequest<ISqlGetRequest, SqlGetResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<SqlGetResponse> GetAsync(ISqlGetRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ISqlGetRequest, SqlGetResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public SqlSearchStatusResponse SearchStatus(
      Id id,
      Func<SqlSearchStatusDescriptor, ISqlSearchStatusRequest> selector = null)
    {
      return this.SearchStatus(selector.InvokeOrDefault<SqlSearchStatusDescriptor, ISqlSearchStatusRequest>(new SqlSearchStatusDescriptor(id)));
    }

    public Task<SqlSearchStatusResponse> SearchStatusAsync(
      Id id,
      Func<SqlSearchStatusDescriptor, ISqlSearchStatusRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.SearchStatusAsync(selector.InvokeOrDefault<SqlSearchStatusDescriptor, ISqlSearchStatusRequest>(new SqlSearchStatusDescriptor(id)), ct);
    }

    public SqlSearchStatusResponse SearchStatus(ISqlSearchStatusRequest request) => this.DoRequest<ISqlSearchStatusRequest, SqlSearchStatusResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<SqlSearchStatusResponse> SearchStatusAsync(
      ISqlSearchStatusRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ISqlSearchStatusRequest, SqlSearchStatusResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public QuerySqlResponse Query(
      Func<QuerySqlDescriptor, IQuerySqlRequest> selector = null)
    {
      return this.Query(selector.InvokeOrDefault<QuerySqlDescriptor, IQuerySqlRequest>(new QuerySqlDescriptor()));
    }

    public Task<QuerySqlResponse> QueryAsync(
      Func<QuerySqlDescriptor, IQuerySqlRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.QueryAsync(selector.InvokeOrDefault<QuerySqlDescriptor, IQuerySqlRequest>(new QuerySqlDescriptor()), ct);
    }

    public QuerySqlResponse Query(IQuerySqlRequest request) => this.DoRequest<IQuerySqlRequest, QuerySqlResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<QuerySqlResponse> QueryAsync(IQuerySqlRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IQuerySqlRequest, QuerySqlResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public TranslateSqlResponse Translate(
      Func<TranslateSqlDescriptor, ITranslateSqlRequest> selector = null)
    {
      return this.Translate(selector.InvokeOrDefault<TranslateSqlDescriptor, ITranslateSqlRequest>(new TranslateSqlDescriptor()));
    }

    public Task<TranslateSqlResponse> TranslateAsync(
      Func<TranslateSqlDescriptor, ITranslateSqlRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.TranslateAsync(selector.InvokeOrDefault<TranslateSqlDescriptor, ITranslateSqlRequest>(new TranslateSqlDescriptor()), ct);
    }

    public TranslateSqlResponse Translate(ITranslateSqlRequest request) => this.DoRequest<ITranslateSqlRequest, TranslateSqlResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<TranslateSqlResponse> TranslateAsync(
      ITranslateSqlRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ITranslateSqlRequest, TranslateSqlResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
