// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown.PackagingStorageBreakdownServiceBase
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown
{
  public abstract class PackagingStorageBreakdownServiceBase : IVssFrameworkService
  {
    protected const string RootBasePath = "/JobOutput/BlobStore/PackagingBreakdown";

    protected abstract string DedupTypeFolderName { get; }

    protected string BasePath => "/JobOutput/BlobStore/PackagingBreakdown/" + this.DedupTypeFolderName;

    protected abstract Guid AggregationJobId { get; }

    protected string DomainPath(string domainId) => this.BasePath + "/D-" + domainId;

    protected string PartitionResultPathPrefix(string domainId) => this.DomainPath(domainId) + "/PartitionResult-";

    protected string TotalPartitionsPath(string domainId) => this.DomainPath(domainId) + "/TotalPartitions";

    protected string RunIdPath(string domainId) => this.DomainPath(domainId) + "/RunId";

    protected string PartitionResultPath(string domainId, int partitionNumber) => this.PartitionResultPathPrefix(domainId) + partitionNumber.ToString();

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    protected void SavePartitionResult<T>(
      IVssRequestContext requestContext,
      T result,
      JobParameters jobParameters)
    {
      requestContext.GetService<IVssRegistryService>().Write(requestContext, (IEnumerable<RegistryItem>) new RegistryItem[1]
      {
        new RegistryItem(this.PartitionResultPath(jobParameters.DomainId, jobParameters.PartitionId), JsonSerializer.Serialize<T>(result))
      });
    }

    protected IEnumerable<T> GetPartitionResults<T>(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      return requestContext.GetService<IVssRegistryService>().Read(requestContext, (RegistryQuery) (this.PartitionResultPathPrefix(domainId.Serialize()) + "*")).Select<RegistryItem, T>((Func<RegistryItem, T>) (x => JsonSerializer.Deserialize<T>(x.Value)));
    }

    public void SaveRunResult(IVssRequestContext requestContext, JobParameters jobParameters) => requestContext.GetService<IVssRegistryService>().Write(requestContext, (IEnumerable<RegistryItem>) new RegistryItem[2]
    {
      new RegistryItem(this.RunIdPath(jobParameters.DomainId), jobParameters.RunId),
      new RegistryItem(this.TotalPartitionsPath(jobParameters.DomainId), jobParameters.TotalPartitions.ToString())
    });

    public (string runId, int totalPartitions) GetRunResult(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      return (service.GetValue(requestContext, (RegistryQuery) this.RunIdPath(domainId.Serialize()), (string) null), service.GetValue<int>(requestContext, (RegistryQuery) this.TotalPartitionsPath(domainId.Serialize()), -1));
    }

    public void ResetResultsIfNeeded(IVssRequestContext requestContext, JobParameters jobParameters)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (service.GetValue<int>(requestContext, (RegistryQuery) this.TotalPartitionsPath(jobParameters.DomainId), -1) == jobParameters.TotalPartitions)
        return;
      service.DeleteEntries(requestContext, this.DomainPath(jobParameters.DomainId) + "/*");
    }

    public void QueueAggregationJob(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationJobService>().QueueJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
    {
      new TeamFoundationJobReference(this.AggregationJobId, JobPriorityClass.Normal)
    }, JobPriorityLevel.Normal, 0, false);
  }
}
