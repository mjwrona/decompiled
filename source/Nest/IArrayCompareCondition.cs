// Decompiled with JetBrains decompiler
// Type: Nest.IArrayCompareCondition
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (ArrayCompareConditionFormatter))]
  public interface IArrayCompareCondition
  {
    string ArrayPath { get; set; }

    string Comparison { get; }

    string Path { get; set; }

    Nest.Quantifier? Quantifier { get; set; }

    object Value { get; set; }
  }
}
