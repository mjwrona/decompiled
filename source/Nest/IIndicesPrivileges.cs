// Decompiled with JetBrains decompiler
// Type: Nest.IIndicesPrivileges
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (IndicesPrivileges))]
  public interface IIndicesPrivileges
  {
    [DataMember(Name = "field_security")]
    IFieldSecurity FieldSecurity { get; set; }

    [DataMember(Name = "names")]
    [JsonFormatter(typeof (IndicesFormatter))]
    Indices Names { get; set; }

    [DataMember(Name = "privileges")]
    IEnumerable<string> Privileges { get; set; }

    [DataMember(Name = "query")]
    QueryContainer Query { get; set; }
  }
}
