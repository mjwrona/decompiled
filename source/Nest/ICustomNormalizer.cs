// Decompiled with JetBrains decompiler
// Type: Nest.ICustomNormalizer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface ICustomNormalizer : INormalizer
  {
    [DataMember(Name = "char_filter")]
    [JsonFormatter(typeof (SingleOrEnumerableFormatter<string>))]
    IEnumerable<string> CharFilter { get; set; }

    [DataMember(Name = "filter")]
    [JsonFormatter(typeof (SingleOrEnumerableFormatter<string>))]
    IEnumerable<string> Filter { get; set; }
  }
}
