// Decompiled with JetBrains decompiler
// Type: Nest.QuerySqlResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class QuerySqlResponse : ResponseBase
  {
    [DataMember(Name = "columns")]
    public IReadOnlyCollection<SqlColumn> Columns { get; internal set; } = EmptyReadOnly<SqlColumn>.Collection;

    [DataMember(Name = "cursor")]
    public string Cursor { get; internal set; }

    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "is_partial")]
    public bool IsPartial { get; internal set; }

    [DataMember(Name = "is_running")]
    public bool IsRunning { get; internal set; }

    [DataMember(Name = "rows")]
    public IReadOnlyCollection<SqlRow> Rows { get; internal set; } = EmptyReadOnly<SqlRow>.Collection;

    [DataMember(Name = "values")]
    public IReadOnlyCollection<SqlRow> Values { get; internal set; } = EmptyReadOnly<SqlRow>.Collection;
  }
}
