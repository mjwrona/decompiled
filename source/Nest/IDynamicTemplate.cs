// Decompiled with JetBrains decompiler
// Type: Nest.IDynamicTemplate
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (DynamicTemplate))]
  public interface IDynamicTemplate
  {
    [DataMember(Name = "mapping")]
    IProperty Mapping { get; set; }

    [DataMember(Name = "match")]
    string Match { get; set; }

    [DataMember(Name = "match_mapping_type")]
    string MatchMappingType { get; set; }

    [DataMember(Name = "match_pattern")]
    MatchType? MatchPattern { get; set; }

    [DataMember(Name = "path_match")]
    string PathMatch { get; set; }

    [DataMember(Name = "path_unmatch")]
    string PathUnmatch { get; set; }

    [DataMember(Name = "unmatch")]
    string Unmatch { get; set; }
  }
}
