// Decompiled with JetBrains decompiler
// Type: Nest.SignificantTextAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class SignificantTextAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<SignificantTextAggregationDescriptor<T>, ISignificantTextAggregation, T>,
    ISignificantTextAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    QueryContainer ISignificantTextAggregation.BackgroundFilter { get; set; }

    IChiSquareHeuristic ISignificantTextAggregation.ChiSquare { get; set; }

    IncludeExclude ISignificantTextAggregation.Exclude { get; set; }

    TermsAggregationExecutionHint? ISignificantTextAggregation.ExecutionHint { get; set; }

    Nest.Field ISignificantTextAggregation.Field { get; set; }

    bool? ISignificantTextAggregation.FilterDuplicateText { get; set; }

    IGoogleNormalizedDistanceHeuristic ISignificantTextAggregation.GoogleNormalizedDistance { get; set; }

    IncludeExclude ISignificantTextAggregation.Include { get; set; }

    long? ISignificantTextAggregation.MinimumDocumentCount { get; set; }

    IMutualInformationHeuristic ISignificantTextAggregation.MutualInformation { get; set; }

    IPercentageScoreHeuristic ISignificantTextAggregation.PercentageScore { get; set; }

    IScriptedHeuristic ISignificantTextAggregation.Script { get; set; }

    long? ISignificantTextAggregation.ShardMinimumDocumentCount { get; set; }

    int? ISignificantTextAggregation.ShardSize { get; set; }

    int? ISignificantTextAggregation.Size { get; set; }

    Fields ISignificantTextAggregation.SourceFields { get; set; }

    public SignificantTextAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ISignificantTextAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public SignificantTextAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<ISignificantTextAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public SignificantTextAggregationDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<ISignificantTextAggregation, int?>) ((a, v) => a.Size = v));

    public SignificantTextAggregationDescriptor<T> ExecutionHint(TermsAggregationExecutionHint? hint) => this.Assign<TermsAggregationExecutionHint?>(hint, (Action<ISignificantTextAggregation, TermsAggregationExecutionHint?>) ((a, v) => a.ExecutionHint = v));

    public SignificantTextAggregationDescriptor<T> Include(string includePattern) => this.Assign<IncludeExclude>(new IncludeExclude(includePattern), (Action<ISignificantTextAggregation, IncludeExclude>) ((a, v) => a.Include = v));

    public SignificantTextAggregationDescriptor<T> Include(IEnumerable<string> values) => this.Assign<IncludeExclude>(new IncludeExclude(values), (Action<ISignificantTextAggregation, IncludeExclude>) ((a, v) => a.Include = v));

    public SignificantTextAggregationDescriptor<T> Exclude(string excludePattern) => this.Assign<IncludeExclude>(new IncludeExclude(excludePattern), (Action<ISignificantTextAggregation, IncludeExclude>) ((a, v) => a.Exclude = v));

    public SignificantTextAggregationDescriptor<T> Exclude(IEnumerable<string> values) => this.Assign<IncludeExclude>(new IncludeExclude(values), (Action<ISignificantTextAggregation, IncludeExclude>) ((a, v) => a.Exclude = v));

    public SignificantTextAggregationDescriptor<T> ShardSize(int? shardSize) => this.Assign<int?>(shardSize, (Action<ISignificantTextAggregation, int?>) ((a, v) => a.ShardSize = v));

    public SignificantTextAggregationDescriptor<T> MinimumDocumentCount(long? minimumDocumentCount) => this.Assign<long?>(minimumDocumentCount, (Action<ISignificantTextAggregation, long?>) ((a, v) => a.MinimumDocumentCount = v));

    public SignificantTextAggregationDescriptor<T> ShardMinimumDocumentCount(
      long? shardMinimumDocumentCount)
    {
      return this.Assign<long?>(shardMinimumDocumentCount, (Action<ISignificantTextAggregation, long?>) ((a, v) => a.ShardMinimumDocumentCount = v));
    }

    public SignificantTextAggregationDescriptor<T> MutualInformation(
      Func<MutualInformationHeuristicDescriptor, IMutualInformationHeuristic> mutualInformationSelector = null)
    {
      return this.Assign<IMutualInformationHeuristic>(mutualInformationSelector.InvokeOrDefault<MutualInformationHeuristicDescriptor, IMutualInformationHeuristic>(new MutualInformationHeuristicDescriptor()), (Action<ISignificantTextAggregation, IMutualInformationHeuristic>) ((a, v) => a.MutualInformation = v));
    }

    public SignificantTextAggregationDescriptor<T> ChiSquare(
      Func<ChiSquareHeuristicDescriptor, IChiSquareHeuristic> chiSquareSelector)
    {
      return this.Assign<IChiSquareHeuristic>(chiSquareSelector.InvokeOrDefault<ChiSquareHeuristicDescriptor, IChiSquareHeuristic>(new ChiSquareHeuristicDescriptor()), (Action<ISignificantTextAggregation, IChiSquareHeuristic>) ((a, v) => a.ChiSquare = v));
    }

    public SignificantTextAggregationDescriptor<T> GoogleNormalizedDistance(
      Func<GoogleNormalizedDistanceHeuristicDescriptor, IGoogleNormalizedDistanceHeuristic> gndSelector)
    {
      return this.Assign<IGoogleNormalizedDistanceHeuristic>(gndSelector.InvokeOrDefault<GoogleNormalizedDistanceHeuristicDescriptor, IGoogleNormalizedDistanceHeuristic>(new GoogleNormalizedDistanceHeuristicDescriptor()), (Action<ISignificantTextAggregation, IGoogleNormalizedDistanceHeuristic>) ((a, v) => a.GoogleNormalizedDistance = v));
    }

    public SignificantTextAggregationDescriptor<T> PercentageScore(
      Func<PercentageScoreHeuristicDescriptor, IPercentageScoreHeuristic> percentageScoreSelector)
    {
      return this.Assign<IPercentageScoreHeuristic>(percentageScoreSelector.InvokeOrDefault<PercentageScoreHeuristicDescriptor, IPercentageScoreHeuristic>(new PercentageScoreHeuristicDescriptor()), (Action<ISignificantTextAggregation, IPercentageScoreHeuristic>) ((a, v) => a.PercentageScore = v));
    }

    public SignificantTextAggregationDescriptor<T> Script(
      Func<ScriptedHeuristicDescriptor, IScriptedHeuristic> scriptSelector)
    {
      return this.Assign<Func<ScriptedHeuristicDescriptor, IScriptedHeuristic>>(scriptSelector, (Action<ISignificantTextAggregation, Func<ScriptedHeuristicDescriptor, IScriptedHeuristic>>) ((a, v) => a.Script = v != null ? v(new ScriptedHeuristicDescriptor()) : (IScriptedHeuristic) null));
    }

    public SignificantTextAggregationDescriptor<T> BackgroundFilter(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<ISignificantTextAggregation, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.BackgroundFilter = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public SignificantTextAggregationDescriptor<T> FilterDuplicateText(bool? filterDuplicateText = true) => this.Assign<bool?>(filterDuplicateText, (Action<ISignificantTextAggregation, bool?>) ((a, v) => a.FilterDuplicateText = v));

    public SignificantTextAggregationDescriptor<T> SourceFields(
      Func<FieldsDescriptor<T>, IPromise<Fields>> sourceFields)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(sourceFields, (Action<ISignificantTextAggregation, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.SourceFields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));
    }

    public SignificantTextAggregationDescriptor<T> SourceFields(Fields sourceFields) => this.Assign<Fields>(sourceFields, (Action<ISignificantTextAggregation, Fields>) ((a, v) => a.SourceFields = v));
  }
}
