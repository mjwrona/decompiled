// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.JobQueuerFactoryBootstrapperForMigrationJobs
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class JobQueuerFactoryBootstrapperForMigrationJobs : 
    IBootstrapper<IFactory<JobType, IFeedJobQueuer>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFeedJobQueuer changeProcessingJobQueuer;
    private readonly IFeedJobQueuer migrationProcessingJobQueuer;

    public JobQueuerFactoryBootstrapperForMigrationJobs(
      IVssRequestContext requestContext,
      IFeedJobQueuer changeProcessingJobQueuer,
      IFeedJobQueuer migrationProcessingJobQueuer)
    {
      this.requestContext = requestContext;
      this.changeProcessingJobQueuer = changeProcessingJobQueuer;
      this.migrationProcessingJobQueuer = migrationProcessingJobQueuer;
    }

    public IFactory<JobType, IFeedJobQueuer> Bootstrap() => (IFactory<JobType, IFeedJobQueuer>) new ChooseFirstNonNullInputFactory<JobType, IFeedJobQueuer>(new IFactory<JobType, IFeedJobQueuer>[2]
    {
      (IFactory<JobType, IFeedJobQueuer>) new OnPremInputFactory<JobType, IFeedJobQueuer>(this.requestContext.GetExecutionEnvironmentFacade(), (IFactory<JobType, IFeedJobQueuer>) new ReturnSameInstanceInputFactory<JobType, IFeedJobQueuer>((IFeedJobQueuer) new QueueNothingQueuer())),
      (IFactory<JobType, IFeedJobQueuer>) new DictionaryFactory<JobType, IFeedJobQueuer>()
      {
        {
          MigrationJobConstants.MigrationProcessingJobType,
          this.migrationProcessingJobQueuer
        },
        {
          ChangeProcessingJobConstants.ChangeProcessingJobType,
          this.changeProcessingJobQueuer
        }
      }
    });
  }
}
