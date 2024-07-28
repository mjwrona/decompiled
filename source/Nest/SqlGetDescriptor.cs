// Decompiled with JetBrains decompiler
// Type: Nest.SqlGetDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SqlApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class SqlGetDescriptor : 
    RequestDescriptorBase<SqlGetDescriptor, SqlGetRequestParameters, ISqlGetRequest>,
    ISqlGetRequest,
    IRequest<SqlGetRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SqlGet;

    public SqlGetDescriptor(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected SqlGetDescriptor()
    {
    }

    Id ISqlGetRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public SqlGetDescriptor Delimiter(string delimiter) => this.Qs(nameof (delimiter), (object) delimiter);

    public SqlGetDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public SqlGetDescriptor KeepAlive(Time keepalive) => this.Qs("keep_alive", (object) keepalive);

    public SqlGetDescriptor WaitForCompletionTimeout(Time waitforcompletiontimeout) => this.Qs("wait_for_completion_timeout", (object) waitforcompletiontimeout);
  }
}
