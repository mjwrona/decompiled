// Decompiled with JetBrains decompiler
// Type: Nest.IPagerDutyContext
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (PagerDutyContext))]
  public interface IPagerDutyContext
  {
    [DataMember(Name = "href")]
    string Href { get; set; }

    [DataMember(Name = "src")]
    string Src { get; set; }

    [DataMember(Name = "type")]
    PagerDutyContextType Type { get; set; }
  }
}
