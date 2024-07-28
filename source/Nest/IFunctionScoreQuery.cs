// Decompiled with JetBrains decompiler
// Type: Nest.IFunctionScoreQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (FunctionScoreQuery))]
  public interface IFunctionScoreQuery : IQuery
  {
    [DataMember(Name = "boost_mode")]
    FunctionBoostMode? BoostMode { get; set; }

    [DataMember(Name = "functions")]
    IEnumerable<IScoreFunction> Functions { get; set; }

    [DataMember(Name = "max_boost")]
    double? MaxBoost { get; set; }

    [DataMember(Name = "min_score")]
    double? MinScore { get; set; }

    [DataMember(Name = "query")]
    QueryContainer Query { get; set; }

    [DataMember(Name = "score_mode")]
    FunctionScoreMode? ScoreMode { get; set; }
  }
}
