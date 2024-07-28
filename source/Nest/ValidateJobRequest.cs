// Decompiled with JetBrains decompiler
// Type: Nest.ValidateJobRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.MachineLearningApi;

namespace Nest
{
  public class ValidateJobRequest : 
    PlainRequestBase<ValidateJobRequestParameters>,
    IValidateJobRequest,
    IRequest<ValidateJobRequestParameters>,
    IRequest
  {
    protected IValidateJobRequest Self => (IValidateJobRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningValidateJob;

    public IAnalysisConfig AnalysisConfig { get; set; }

    public IAnalysisLimits AnalysisLimits { get; set; }

    public IDataDescription DataDescription { get; set; }

    public string Description { get; set; }

    public IModelPlotConfig ModelPlotConfig { get; set; }

    public long? ModelSnapshotRetentionDays { get; set; }

    public IndexName ResultsIndexName { get; set; }
  }
}
