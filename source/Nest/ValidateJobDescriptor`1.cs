// Decompiled with JetBrains decompiler
// Type: Nest.ValidateJobDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.MachineLearningApi;
using System;

namespace Nest
{
  public class ValidateJobDescriptor<TDocument> : 
    RequestDescriptorBase<ValidateJobDescriptor<TDocument>, ValidateJobRequestParameters, IValidateJobRequest>,
    IValidateJobRequest,
    IRequest<ValidateJobRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningValidateJob;

    IAnalysisConfig IValidateJobRequest.AnalysisConfig { get; set; }

    IAnalysisLimits IValidateJobRequest.AnalysisLimits { get; set; }

    IDataDescription IValidateJobRequest.DataDescription { get; set; }

    string IValidateJobRequest.Description { get; set; }

    IModelPlotConfig IValidateJobRequest.ModelPlotConfig { get; set; }

    long? IValidateJobRequest.ModelSnapshotRetentionDays { get; set; }

    IndexName IValidateJobRequest.ResultsIndexName { get; set; }

    public ValidateJobDescriptor<TDocument> AnalysisConfig(
      Func<AnalysisConfigDescriptor<TDocument>, IAnalysisConfig> selector)
    {
      return this.Assign<Func<AnalysisConfigDescriptor<TDocument>, IAnalysisConfig>>(selector, (Action<IValidateJobRequest, Func<AnalysisConfigDescriptor<TDocument>, IAnalysisConfig>>) ((a, v) => a.AnalysisConfig = v != null ? v(new AnalysisConfigDescriptor<TDocument>()) : (IAnalysisConfig) null));
    }

    public ValidateJobDescriptor<TDocument> AnalysisLimits(
      Func<AnalysisLimitsDescriptor, IAnalysisLimits> selector)
    {
      return this.Assign<Func<AnalysisLimitsDescriptor, IAnalysisLimits>>(selector, (Action<IValidateJobRequest, Func<AnalysisLimitsDescriptor, IAnalysisLimits>>) ((a, v) => a.AnalysisLimits = v != null ? v(new AnalysisLimitsDescriptor()) : (IAnalysisLimits) null));
    }

    public ValidateJobDescriptor<TDocument> DataDescription(
      Func<DataDescriptionDescriptor<TDocument>, IDataDescription> selector)
    {
      return this.Assign<IDataDescription>(selector.InvokeOrDefault<DataDescriptionDescriptor<TDocument>, IDataDescription>(new DataDescriptionDescriptor<TDocument>()), (Action<IValidateJobRequest, IDataDescription>) ((a, v) => a.DataDescription = v));
    }

    public ValidateJobDescriptor<TDocument> Description(string description) => this.Assign<string>(description, (Action<IValidateJobRequest, string>) ((a, v) => a.Description = v));

    public ValidateJobDescriptor<TDocument> ModelPlot(
      Func<ModelPlotConfigDescriptor<TDocument>, IModelPlotConfig> selector)
    {
      return this.Assign<Func<ModelPlotConfigDescriptor<TDocument>, IModelPlotConfig>>(selector, (Action<IValidateJobRequest, Func<ModelPlotConfigDescriptor<TDocument>, IModelPlotConfig>>) ((a, v) => a.ModelPlotConfig = v != null ? v(new ModelPlotConfigDescriptor<TDocument>()) : (IModelPlotConfig) null));
    }

    public ValidateJobDescriptor<TDocument> ModelSnapshotRetentionDays(
      long? modelSnapshotRetentionDays)
    {
      return this.Assign<long?>(modelSnapshotRetentionDays, (Action<IValidateJobRequest, long?>) ((a, v) => a.ModelSnapshotRetentionDays = v));
    }

    public ValidateJobDescriptor<TDocument> ResultsIndexName(IndexName indexName) => this.Assign<IndexName>(indexName, (Action<IValidateJobRequest, IndexName>) ((a, v) => a.ResultsIndexName = v));

    public ValidateJobDescriptor<TDocument> ResultsIndexName<TIndex>() => this.Assign<Type>(typeof (TIndex), (Action<IValidateJobRequest, Type>) ((a, v) => a.ResultsIndexName = (IndexName) v));
  }
}
