// Decompiled with JetBrains decompiler
// Type: Nest.ICsvProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface ICsvProcessor : IProcessor
  {
    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "target_fields")]
    Fields TargetFields { get; set; }

    [DataMember(Name = "separator")]
    string Separator { get; set; }

    [DataMember(Name = "quote")]
    string Quote { get; set; }

    [DataMember(Name = "ignore_missing")]
    bool? IgnoreMissing { get; set; }

    [DataMember(Name = "trim")]
    bool? Trim { get; set; }

    [DataMember(Name = "empty_value")]
    object EmptyValue { get; set; }
  }
}
