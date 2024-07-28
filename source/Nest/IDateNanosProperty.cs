// Decompiled with JetBrains decompiler
// Type: Nest.IDateNanosProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IDateNanosProperty : IDocValuesProperty, ICoreProperty, IProperty, IFieldMapping
  {
    [DataMember(Name = "boost")]
    double? Boost { get; set; }

    [DataMember(Name = "format")]
    string Format { get; set; }

    [DataMember(Name = "ignore_malformed")]
    bool? IgnoreMalformed { get; set; }

    [DataMember(Name = "index")]
    bool? Index { get; set; }

    [DataMember(Name = "null_value")]
    DateTime? NullValue { get; set; }
  }
}
