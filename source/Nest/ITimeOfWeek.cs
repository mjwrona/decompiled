// Decompiled with JetBrains decompiler
// Type: Nest.ITimeOfWeek
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (TimeOfWeek))]
  public interface ITimeOfWeek
  {
    [DataMember(Name = "at")]
    [JsonFormatter(typeof (SingleOrEnumerableFormatter<string>))]
    IEnumerable<string> At { get; set; }

    [DataMember(Name = "on")]
    [JsonFormatter(typeof (SingleOrEnumerableFormatter<Day>))]
    IEnumerable<Day> On { get; set; }
  }
}
