// Decompiled with JetBrains decompiler
// Type: Nest.IAlias
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (Alias))]
  public interface IAlias
  {
    [DataMember(Name = "filter")]
    QueryContainer Filter { get; set; }

    [DataMember(Name = "index_routing")]
    Routing IndexRouting { get; set; }

    [DataMember(Name = "is_write_index")]
    bool? IsWriteIndex { get; set; }

    [DataMember(Name = "is_hidden")]
    bool? IsHidden { get; set; }

    [DataMember(Name = "routing")]
    Routing Routing { get; set; }

    [DataMember(Name = "search_routing")]
    Routing SearchRouting { get; set; }
  }
}
