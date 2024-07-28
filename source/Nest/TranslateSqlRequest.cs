// Decompiled with JetBrains decompiler
// Type: Nest.TranslateSqlRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SqlApi;
using System.Collections.Generic;

namespace Nest
{
  public class TranslateSqlRequest : 
    PlainRequestBase<TranslateSqlRequestParameters>,
    ITranslateSqlRequest,
    IRequest<TranslateSqlRequestParameters>,
    IRequest,
    ISqlRequest
  {
    protected ITranslateSqlRequest Self => (ITranslateSqlRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SqlTranslate;

    protected override sealed void RequestDefaults(TranslateSqlRequestParameters parameters) => parameters.CustomResponseBuilder = (CustomResponseBuilderBase) TranslateSqlResponseBuilder.Instance;

    public int? FetchSize { get; set; }

    public QueryContainer Filter { get; set; }

    public IList<object> Params { get; set; }

    public string Query { get; set; }

    public string TimeZone { get; set; }

    public IRuntimeFields RuntimeFields { get; set; }
  }
}
