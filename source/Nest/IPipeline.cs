// Decompiled with JetBrains decompiler
// Type: Nest.IPipeline
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (Pipeline))]
  public interface IPipeline
  {
    [DataMember(Name = "description")]
    string Description { get; set; }

    [DataMember(Name = "on_failure")]
    IEnumerable<IProcessor> OnFailure { get; set; }

    [DataMember(Name = "processors")]
    IEnumerable<IProcessor> Processors { get; set; }

    [DataMember(Name = "version")]
    long? Version { get; set; }
  }
}
