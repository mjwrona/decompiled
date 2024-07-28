// Decompiled with JetBrains decompiler
// Type: Nest.IAttachmentProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IAttachmentProcessor : IProcessor
  {
    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "ignore_missing")]
    bool? IgnoreMissing { get; set; }

    [DataMember(Name = "indexed_chars")]
    long? IndexedCharacters { get; set; }

    [DataMember(Name = "indexed_chars_field")]
    Field IndexedCharactersField { get; set; }

    [DataMember(Name = "properties")]
    IEnumerable<string> Properties { get; set; }

    [DataMember(Name = "target_field")]
    Field TargetField { get; set; }

    [DataMember(Name = "resource_name")]
    Field ResourceName { get; set; }
  }
}
