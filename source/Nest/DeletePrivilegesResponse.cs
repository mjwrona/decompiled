// Decompiled with JetBrains decompiler
// Type: Nest.DeletePrivilegesResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [JsonFormatter(typeof (DictionaryResponseFormatter<DeletePrivilegesResponse, string, IDictionary<string, FoundUserPrivilege>>))]
  public class DeletePrivilegesResponse : 
    DictionaryResponseBase<string, IDictionary<string, FoundUserPrivilege>>
  {
    [IgnoreDataMember]
    public IReadOnlyDictionary<string, IDictionary<string, FoundUserPrivilege>> Applications => this.Self.BackingDictionary;
  }
}
