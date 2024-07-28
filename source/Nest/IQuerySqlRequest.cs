// Decompiled with JetBrains decompiler
// Type: Nest.IQuerySqlRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SqlApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("sql.query.json")]
  [ReadAs(typeof (QuerySqlRequest))]
  public interface IQuerySqlRequest : IRequest<QuerySqlRequestParameters>, IRequest, ISqlRequest
  {
    [DataMember(Name = "columnar")]
    bool? Columnar { get; set; }

    [DataMember(Name = "cursor")]
    string Cursor { get; set; }

    [DataMember(Name = "wait_for_completion_timeout")]
    Time WaitForCompletionTimeout { get; set; }
  }
}
