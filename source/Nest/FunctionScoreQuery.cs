// Decompiled with JetBrains decompiler
// Type: Nest.FunctionScoreQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class FunctionScoreQuery : QueryBase, IFunctionScoreQuery, IQuery
  {
    public FunctionBoostMode? BoostMode { get; set; }

    public IEnumerable<IScoreFunction> Functions { get; set; }

    public double? MaxBoost { get; set; }

    public double? MinScore { get; set; }

    public QueryContainer Query { get; set; }

    public FunctionScoreMode? ScoreMode { get; set; }

    protected override bool Conditionless => FunctionScoreQuery.IsConditionless((IFunctionScoreQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.FunctionScore = (IFunctionScoreQuery) this;

    internal static bool IsConditionless(IFunctionScoreQuery q, bool force = false) => force || !q.Functions.HasAny<IScoreFunction>();
  }
}
