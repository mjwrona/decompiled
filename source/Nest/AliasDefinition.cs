// Decompiled with JetBrains decompiler
// Type: Nest.AliasDefinition
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class AliasDefinition
  {
    [DataMember(Name = "filter")]
    public IQueryContainer Filter { get; internal set; }

    [DataMember(Name = "index_routing")]
    public string IndexRouting { get; internal set; }

    [DataMember(Name = "is_write_index")]
    public bool? IsWriteIndex { get; internal set; }

    [DataMember(Name = "routing")]
    public string Routing { get; internal set; }

    [DataMember(Name = "search_routing")]
    public string SearchRouting { get; internal set; }
  }
}
