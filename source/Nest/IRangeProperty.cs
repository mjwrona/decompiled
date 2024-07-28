// Decompiled with JetBrains decompiler
// Type: Nest.IRangeProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IRangeProperty : IDocValuesProperty, ICoreProperty, IProperty, IFieldMapping
  {
    [Obsolete("The server always treated this as a noop and has been removed in 7.10")]
    [DataMember(Name = "boost")]
    double? Boost { get; set; }

    [DataMember(Name = "coerce")]
    bool? Coerce { get; set; }

    [DataMember(Name = "index")]
    bool? Index { get; set; }
  }
}
