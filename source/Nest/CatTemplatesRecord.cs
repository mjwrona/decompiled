// Decompiled with JetBrains decompiler
// Type: Nest.CatTemplatesRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatTemplatesRecord : ICatRecord
  {
    [DataMember(Name = "index_patterns")]
    public string IndexPatterns { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "order")]
    [JsonFormatter(typeof (StringLongFormatter))]
    public long Order { get; set; }

    [DataMember(Name = "version")]
    [JsonFormatter(typeof (NullableStringLongFormatter))]
    public long? Version { get; set; }
  }
}
