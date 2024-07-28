// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.FeatureFlagCheckingJobHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs
{
  public class FeatureFlagCheckingJobHandler<T> : 
    IAsyncHandler<T, JobResult>,
    IHaveInputType<T>,
    IHaveOutputType<JobResult>
  {
    private readonly string featureFlagToCheck;
    private readonly bool expectedFlagStateToBlockJob;
    private readonly IFeatureFlagService featureFlagService;

    public FeatureFlagCheckingJobHandler(
      IFeatureFlagService featureFlagService,
      string featureFlagToCheck,
      bool expectedFlagStateToBlockJob)
    {
      this.featureFlagToCheck = featureFlagToCheck;
      this.featureFlagService = featureFlagService;
      this.expectedFlagStateToBlockJob = expectedFlagStateToBlockJob;
    }

    public Task<JobResult> Handle(T request)
    {
      if (this.featureFlagService.IsEnabled(this.featureFlagToCheck) != this.expectedFlagStateToBlockJob)
        return Task.FromResult<JobResult>((JobResult) null);
      string str = this.expectedFlagStateToBlockJob ? "enabled" : "disabled";
      return Task.FromResult<JobResult>(JobResult.Blocked(new JobTelemetry()
      {
        Message = "Feature flag " + this.featureFlagToCheck + " " + str + " so job did not do anything."
      }));
    }
  }
}
