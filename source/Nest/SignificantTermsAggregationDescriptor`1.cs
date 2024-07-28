// Decompiled with JetBrains decompiler
// Type: Nest.SignificantTermsAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class SignificantTermsAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<SignificantTermsAggregationDescriptor<T>, ISignificantTermsAggregation, T>,
    ISignificantTermsAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    QueryContainer ISignificantTermsAggregation.BackgroundFilter { get; set; }

    IChiSquareHeuristic ISignificantTermsAggregation.ChiSquare { get; set; }

    IncludeExclude ISignificantTermsAggregation.Exclude { get; set; }

    TermsAggregationExecutionHint? ISignificantTermsAggregation.ExecutionHint { get; set; }

    Nest.Field ISignificantTermsAggregation.Field { get; set; }

    IGoogleNormalizedDistanceHeuristic ISignificantTermsAggregation.GoogleNormalizedDistance { get; set; }

    IncludeExclude ISignificantTermsAggregation.Include { get; set; }

    long? ISignificantTermsAggregation.MinimumDocumentCount { get; set; }

    IMutualInformationHeuristic ISignificantTermsAggregation.MutualInformation { get; set; }

    IPercentageScoreHeuristic ISignificantTermsAggregation.PercentageScore { get; set; }

    IScriptedHeuristic ISignificantTermsAggregation.Script { get; set; }

    long? ISignificantTermsAggregation.ShardMinimumDocumentCount { get; set; }

    int? ISignificantTermsAggregation.ShardSize { get; set; }

    int? ISignificantTermsAggregation.Size { get; set; }

    public SignificantTermsAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ISignificantTermsAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public SignificantTermsAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<ISignificantTermsAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public SignificantTermsAggregationDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<ISignificantTermsAggregation, int?>) ((a, v) => a.Size = v));

    public SignificantTermsAggregationDescriptor<T> ExecutionHint(
      TermsAggregationExecutionHint? hint)
    {
      return this.Assign<TermsAggregationExecutionHint?>(hint, (Action<ISignificantTermsAggregation, TermsAggregationExecutionHint?>) ((a, v) => a.ExecutionHint = v));
    }

    public SignificantTermsAggregationDescriptor<T> Include(string includePattern) => this.Assign<IncludeExclude>(new IncludeExclude(includePattern), (Action<ISignificantTermsAggregation, IncludeExclude>) ((a, v) => a.Include = v));

    public SignificantTermsAggregationDescriptor<T> Include(IEnumerable<string> values) => this.Assign<IncludeExclude>(new IncludeExclude(values), (Action<ISignificantTermsAggregation, IncludeExclude>) ((a, v) => a.Include = v));

    public SignificantTermsAggregationDescriptor<T> Exclude(string excludePattern) => this.Assign<IncludeExclude>(new IncludeExclude(excludePattern), (Action<ISignificantTermsAggregation, IncludeExclude>) ((a, v) => a.Exclude = v));

    public SignificantTermsAggregationDescriptor<T> Exclude(IEnumerable<string> values) => this.Assign<IncludeExclude>(new IncludeExclude(values), (Action<ISignificantTermsAggregation, IncludeExclude>) ((a, v) => a.Exclude = v));

    public SignificantTermsAggregationDescriptor<T> ShardSize(int? shardSize) => this.Assign<int?>(shardSize, (Action<ISignificantTermsAggregation, int?>) ((a, v) => a.ShardSize = v));

    public SignificantTermsAggregationDescriptor<T> MinimumDocumentCount(long? minimumDocumentCount) => this.Assign<long?>(minimumDocumentCount, (Action<ISignificantTermsAggregation, long?>) ((a, v) => a.MinimumDocumentCount = v));

    public SignificantTermsAggregationDescriptor<T> ShardMinimumDocumentCount(
      long? shardMinimumDocumentCount)
    {
      return this.Assign<long?>(shardMinimumDocumentCount, (Action<ISignificantTermsAggregation, long?>) ((a, v) => a.ShardMinimumDocumentCount = v));
    }

    public SignificantTermsAggregationDescriptor<T> MutualInformation(
      Func<MutualInformationHeuristicDescriptor, IMutualInformationHeuristic> mutualInformationSelector = null)
    {
      return this.Assign<IMutualInformationHeuristic>(mutualInformationSelector.InvokeOrDefault<MutualInformationHeuristicDescriptor, IMutualInformationHeuristic>(new MutualInformationHeuristicDescriptor()), (Action<ISignificantTermsAggregation, IMutualInformationHeuristic>) ((a, v) => a.MutualInformation = v));
    }

    public SignificantTermsAggregationDescriptor<T> ChiSquare(
      Func<ChiSquareHeuristicDescriptor, IChiSquareHeuristic> chiSquareSelector)
    {
      return this.Assign<IChiSquareHeuristic>(chiSquareSelector.InvokeOrDefault<ChiSquareHeuristicDescriptor, IChiSquareHeuristic>(new ChiSquareHeuristicDescriptor()), (Action<ISignificantTermsAggregation, IChiSquareHeuristic>) ((a, v) => a.ChiSquare = v));
    }

    public SignificantTermsAggregationDescriptor<T> GoogleNormalizedDistance(
      Func<GoogleNormalizedDistanceHeuristicDescriptor, IGoogleNormalizedDistanceHeuristic> gndSelector)
    {
      return this.Assign<IGoogleNormalizedDistanceHeuristic>(gndSelector.InvokeOrDefault<GoogleNormalizedDistanceHeuristicDescriptor, IGoogleNormalizedDistanceHeuristic>(new GoogleNormalizedDistanceHeuristicDescriptor()), (Action<ISignificantTermsAggregation, IGoogleNormalizedDistanceHeuristic>) ((a, v) => a.GoogleNormalizedDistance = v));
    }

    public SignificantTermsAggregationDescriptor<T> PercentageScore(
      Func<PercentageScoreHeuristicDescriptor, IPercentageScoreHeuristic> percentageScoreSelector)
    {
      return this.Assign<IPercentageScoreHeuristic>(percentageScoreSelector.InvokeOrDefault<PercentageScoreHeuristicDescriptor, IPercentageScoreHeuristic>(new PercentageScoreHeuristicDescriptor()), (Action<ISignificantTermsAggregation, IPercentageScoreHeuristic>) ((a, v) => a.PercentageScore = v));
    }

    public SignificantTermsAggregationDescriptor<T> Script(
      Func<ScriptedHeuristicDescriptor, IScriptedHeuristic> scriptSelector)
    {
      return this.Assign<Func<ScriptedHeuristicDescriptor, IScriptedHeuristic>>(scriptSelector, (Action<ISignificantTermsAggregation, Func<ScriptedHeuristicDescriptor, IScriptedHeuristic>>) ((a, v) => a.Script = v != null ? v(new ScriptedHeuristicDescriptor()) : (IScriptedHeuristic) null));
    }

    public SignificantTermsAggregationDescriptor<T> BackgroundFilter(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<ISignificantTermsAggregation, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.BackgroundFilter = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }
  }
}
