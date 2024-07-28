// Decompiled with JetBrains decompiler
// Type: Nest.FunctionScoreQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class FunctionScoreQueryDescriptor<T> : 
    QueryDescriptorBase<FunctionScoreQueryDescriptor<T>, IFunctionScoreQuery>,
    IFunctionScoreQuery,
    IQuery
    where T : class
  {
    private bool _forcedConditionless;

    protected override bool Conditionless => FunctionScoreQuery.IsConditionless((IFunctionScoreQuery) this, this._forcedConditionless);

    FunctionBoostMode? IFunctionScoreQuery.BoostMode { get; set; }

    IEnumerable<IScoreFunction> IFunctionScoreQuery.Functions { get; set; }

    double? IFunctionScoreQuery.MaxBoost { get; set; }

    double? IFunctionScoreQuery.MinScore { get; set; }

    QueryContainer IFunctionScoreQuery.Query { get; set; }

    FunctionScoreMode? IFunctionScoreQuery.ScoreMode { get; set; }

    public FunctionScoreQueryDescriptor<T> ConditionlessWhen(bool isConditionless)
    {
      this._forcedConditionless = isConditionless;
      return this;
    }

    public FunctionScoreQueryDescriptor<T> Query(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<IFunctionScoreQuery, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public FunctionScoreQueryDescriptor<T> Functions(
      Func<ScoreFunctionsDescriptor<T>, IPromise<IList<IScoreFunction>>> functions)
    {
      return this.Assign<Func<ScoreFunctionsDescriptor<T>, IPromise<IList<IScoreFunction>>>>(functions, (Action<IFunctionScoreQuery, Func<ScoreFunctionsDescriptor<T>, IPromise<IList<IScoreFunction>>>>) ((a, v) => a.Functions = v != null ? (IEnumerable<IScoreFunction>) v(new ScoreFunctionsDescriptor<T>())?.Value : (IEnumerable<IScoreFunction>) null));
    }

    public FunctionScoreQueryDescriptor<T> Functions(IEnumerable<IScoreFunction> functions) => this.Assign<IEnumerable<IScoreFunction>>(functions, (Action<IFunctionScoreQuery, IEnumerable<IScoreFunction>>) ((a, v) => a.Functions = v));

    public FunctionScoreQueryDescriptor<T> ScoreMode(FunctionScoreMode? mode) => this.Assign<FunctionScoreMode?>(mode, (Action<IFunctionScoreQuery, FunctionScoreMode?>) ((a, v) => a.ScoreMode = v));

    public FunctionScoreQueryDescriptor<T> BoostMode(FunctionBoostMode? mode) => this.Assign<FunctionBoostMode?>(mode, (Action<IFunctionScoreQuery, FunctionBoostMode?>) ((a, v) => a.BoostMode = v));

    public FunctionScoreQueryDescriptor<T> MaxBoost(double? maxBoost) => this.Assign<double?>(maxBoost, (Action<IFunctionScoreQuery, double?>) ((a, v) => a.MaxBoost = v));

    public FunctionScoreQueryDescriptor<T> MinScore(double? minScore) => this.Assign<double?>(minScore, (Action<IFunctionScoreQuery, double?>) ((a, v) => a.MinScore = v));
  }
}
