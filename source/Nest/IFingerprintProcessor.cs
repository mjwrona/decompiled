// Decompiled with JetBrains decompiler
// Type: Nest.IFingerprintProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IFingerprintProcessor : IProcessor
  {
    [DataMember(Name = "fields")]
    Fields Fields { get; set; }

    [DataMember(Name = "ignore_missing")]
    bool? IgnoreMissing { get; set; }

    [DataMember(Name = "method")]
    string Method { get; set; }

    [DataMember(Name = "salt")]
    string Salt { get; set; }

    [DataMember(Name = "target_field")]
    Field TargetField { get; set; }
  }
}
