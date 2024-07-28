// Decompiled with JetBrains decompiler
// Type: Nest.IndexMappings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class IndexMappings
  {
    [Obsolete("Mapping are no longer grouped by type, this indexer is ignored and simply returns Mappings")]
    public TypeMapping this[string type] => this.Mappings;

    [DataMember(Name = "mappings")]
    public TypeMapping Mappings { get; internal set; }
  }
}
