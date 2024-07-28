// Decompiled with JetBrains decompiler
// Type: Nest.RareTermsAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class RareTermsAggregation : 
    BucketAggregationBase,
    IRareTermsAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal RareTermsAggregation()
    {
    }

    public RareTermsAggregation(string name)
      : base(name)
    {
    }

    public TermsExclude Exclude { get; set; }

    public Field Field { get; set; }

    public TermsInclude Include { get; set; }

    public long? MaximumDocumentCount { get; set; }

    public object Missing { get; set; }

    public double? Precision { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.RareTerms = (IRareTermsAggregation) this;
  }
}
