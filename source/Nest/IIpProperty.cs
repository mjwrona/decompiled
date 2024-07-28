// Decompiled with JetBrains decompiler
// Type: Nest.IIpProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IIpProperty : IDocValuesProperty, ICoreProperty, IProperty, IFieldMapping
  {
    [Obsolete("The server always treated this as a noop and has been removed in 7.10")]
    [DataMember(Name = "boost")]
    double? Boost { get; set; }

    [DataMember(Name = "index")]
    bool? Index { get; set; }

    [DataMember(Name = "null_value")]
    string NullValue { get; set; }

    [DataMember(Name = "ignore_malformed")]
    bool? IgnoreMalformed { get; set; }

    [DataMember(Name = "script")]
    IInlineScript Script { get; set; }

    [DataMember(Name = "on_script_error")]
    Nest.OnScriptError? OnScriptError { get; set; }
  }
}
