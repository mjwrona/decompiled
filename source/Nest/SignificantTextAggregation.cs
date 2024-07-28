// Decompiled with JetBrains decompiler
// Type: Nest.SignificantTextAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class SignificantTextAggregation : 
    BucketAggregationBase,
    ISignificantTextAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal SignificantTextAggregation()
    {
    }

    public SignificantTextAggregation(string name)
      : base(name)
    {
    }

    public QueryContainer BackgroundFilter { get; set; }

    public IChiSquareHeuristic ChiSquare { get; set; }

    public IncludeExclude Exclude { get; set; }

    public TermsAggregationExecutionHint? ExecutionHint { get; set; }

    public Field Field { get; set; }

    public bool? FilterDuplicateText { get; set; }

    public IGoogleNormalizedDistanceHeuristic GoogleNormalizedDistance { get; set; }

    public IncludeExclude Include { get; set; }

    public long? MinimumDocumentCount { get; set; }

    public IMutualInformationHeuristic MutualInformation { get; set; }

    public IPercentageScoreHeuristic PercentageScore { get; set; }

    public IScriptedHeuristic Script { get; set; }

    public long? ShardMinimumDocumentCount { get; set; }

    public int? ShardSize { get; set; }

    public int? Size { get; set; }

    public Fields SourceFields { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.SignificantText = (ISignificantTextAggregation) this;
  }
}
