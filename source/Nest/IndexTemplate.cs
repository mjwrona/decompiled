// Decompiled with JetBrains decompiler
// Type: Nest.IndexTemplate
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class IndexTemplate
  {
    [DataMember(Name = "index_patterns")]
    public IEnumerable<string> IndexPatterns { get; set; }

    [DataMember(Name = "composed_of")]
    public IEnumerable<string> ComposedOf { get; set; }

    [DataMember(Name = "template")]
    public ITemplate Template { get; set; }

    [DataMember(Name = "data_stream")]
    public DataStream DataStream { get; set; }

    [DataMember(Name = "priority")]
    public int? Priority { get; set; }

    [DataMember(Name = "version")]
    public long? Version { get; set; }

    [DataMember(Name = "_meta")]
    public IDictionary<string, object> Meta { get; set; }

    [DataMember(Name = "allow_auto_create")]
    public bool? AllowAutoCreate { get; set; }
  }
}
