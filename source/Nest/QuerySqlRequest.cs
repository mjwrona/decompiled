// Decompiled with JetBrains decompiler
// Type: Nest.QuerySqlRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SqlApi;
using System.Collections.Generic;

namespace Nest
{
  public class QuerySqlRequest : 
    PlainRequestBase<QuerySqlRequestParameters>,
    IQuerySqlRequest,
    IRequest<QuerySqlRequestParameters>,
    IRequest,
    ISqlRequest
  {
    protected IQuerySqlRequest Self => (IQuerySqlRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SqlQuery;

    public string Format
    {
      get => this.Q<string>("format");
      set => this.Q("format", (object) value);
    }

    public bool? Columnar { get; set; }

    public string Cursor { get; set; }

    public int? FetchSize { get; set; }

    public QueryContainer Filter { get; set; }

    public IList<object> Params { get; set; }

    public string Query { get; set; }

    public IRuntimeFields RuntimeFields { get; set; }

    public string TimeZone { get; set; }

    public Time WaitForCompletionTimeout { get; set; }
  }
}
