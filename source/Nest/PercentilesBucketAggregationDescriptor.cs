// Decompiled with JetBrains decompiler
// Type: Nest.PercentilesBucketAggregationDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class PercentilesBucketAggregationDescriptor : 
    PipelineAggregationDescriptorBase<PercentilesBucketAggregationDescriptor, IPercentilesBucketAggregation, SingleBucketsPath>,
    IPercentilesBucketAggregation,
    IPipelineAggregation,
    IAggregation
  {
    IEnumerable<double> IPercentilesBucketAggregation.Percents { get; set; }

    public PercentilesBucketAggregationDescriptor Percents(IEnumerable<double> percentages) => this.Assign<List<double>>(percentages != null ? percentages.ToList<double>() : (List<double>) null, (Action<IPercentilesBucketAggregation, List<double>>) ((a, v) => a.Percents = (IEnumerable<double>) v));

    public PercentilesBucketAggregationDescriptor Percents(params double[] percentages) => this.Assign<List<double>>(percentages != null ? ((IEnumerable<double>) percentages).ToList<double>() : (List<double>) null, (Action<IPercentilesBucketAggregation, List<double>>) ((a, v) => a.Percents = (IEnumerable<double>) v));
  }
}
