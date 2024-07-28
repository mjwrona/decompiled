// Decompiled with JetBrains decompiler
// Type: Nest.EstimateModelMemoryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.MachineLearningApi;
using System;

namespace Nest
{
  public class EstimateModelMemoryDescriptor<TDocument> : 
    RequestDescriptorBase<EstimateModelMemoryDescriptor<TDocument>, EstimateModelMemoryRequestParameters, IEstimateModelMemoryRequest>,
    IEstimateModelMemoryRequest,
    IRequest<EstimateModelMemoryRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningEstimateModelMemory;

    IAnalysisConfig IEstimateModelMemoryRequest.AnalysisConfig { get; set; }

    IOverallCardinality IEstimateModelMemoryRequest.OverallCardinality { get; set; }

    IMaxBucketCardinality IEstimateModelMemoryRequest.MaxBucketCardinality { get; set; }

    public EstimateModelMemoryDescriptor<TDocument> AnalysisConfig(
      Func<AnalysisConfigDescriptor<TDocument>, IAnalysisConfig> selector)
    {
      return this.Assign<Func<AnalysisConfigDescriptor<TDocument>, IAnalysisConfig>>(selector, (Action<IEstimateModelMemoryRequest, Func<AnalysisConfigDescriptor<TDocument>, IAnalysisConfig>>) ((a, v) => a.AnalysisConfig = v != null ? v(new AnalysisConfigDescriptor<TDocument>()) : (IAnalysisConfig) null));
    }

    public EstimateModelMemoryDescriptor<TDocument> OverallCardinality(
      Func<OverallCardinalityDescriptor<TDocument>, IPromise<IOverallCardinality>> analyzerSelector)
    {
      return this.Assign<Func<OverallCardinalityDescriptor<TDocument>, IPromise<IOverallCardinality>>>(analyzerSelector, (Action<IEstimateModelMemoryRequest, Func<OverallCardinalityDescriptor<TDocument>, IPromise<IOverallCardinality>>>) ((a, v) => a.OverallCardinality = v != null ? v(new OverallCardinalityDescriptor<TDocument>())?.Value : (IOverallCardinality) null));
    }

    public EstimateModelMemoryDescriptor<TDocument> MaxBucketCardinality(
      Func<MaxBucketCardinalityDescriptor<TDocument>, IPromise<IMaxBucketCardinality>> analyzerSelector)
    {
      return this.Assign<Func<MaxBucketCardinalityDescriptor<TDocument>, IPromise<IMaxBucketCardinality>>>(analyzerSelector, (Action<IEstimateModelMemoryRequest, Func<MaxBucketCardinalityDescriptor<TDocument>, IPromise<IMaxBucketCardinality>>>) ((a, v) => a.MaxBucketCardinality = v != null ? v(new MaxBucketCardinalityDescriptor<TDocument>())?.Value : (IMaxBucketCardinality) null));
    }
  }
}
