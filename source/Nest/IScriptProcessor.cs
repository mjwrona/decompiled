// Decompiled with JetBrains decompiler
// Type: Nest.IScriptProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IScriptProcessor : IProcessor
  {
    [DataMember(Name = "id")]
    string Id { get; set; }

    [DataMember(Name = "lang")]
    string Lang { get; set; }

    [DataMember(Name = "params")]
    [JsonFormatter(typeof (VerbatimDictionaryKeysFormatter<string, object>))]
    Dictionary<string, object> Params { get; set; }

    [DataMember(Name = "source")]
    string Source { get; set; }
  }
}
