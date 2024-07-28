// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.MultiJobHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class MultiJobHelper
  {
    private const int MaxTotalPartitions = 4096;
    private readonly string JobExtensionName;
    private readonly string JobNamePrefix;
    private readonly JobParameters ParentJobParameters;
    private readonly JobPriorityLevel JobPriority;
    private readonly TimeSpan JobSchedulingInterval;

    public MultiJobHelper(
      JobParameters parentJobParameters,
      string jobExtensionName,
      string jobNamePrefix,
      TimeSpan jobSchedulingInterval,
      JobPriorityLevel jobPriority = JobPriorityLevel.Idle)
    {
      this.ParentJobParameters = parentJobParameters;
      this.JobExtensionName = jobExtensionName;
      this.JobNamePrefix = jobNamePrefix;
      this.JobPriority = jobPriority;
      this.JobSchedulingInterval = jobSchedulingInterval;
    }

    public static JobParameters GetPartitionInfo(XmlNode jobData)
    {
      JobParameters partitionInfo = TeamFoundationSerializationUtility.Deserialize<JobParameters>(jobData);
      if (partitionInfo == null)
        throw new ArgumentException("The JobData does not contain JobParameters");
      if (partitionInfo.TotalPartitions < 1 || partitionInfo.TotalPartitions > 4096)
        throw new ArgumentException(string.Format("The totalPartitions '{0}' should be a non-negative number less than {1}", (object) partitionInfo.TotalPartitions, (object) 4096));
      if (partitionInfo.PartitionId < 0 || partitionInfo.PartitionId >= partitionInfo.TotalPartitions)
        throw new ArgumentException(string.Format("The Partition value '{0}' should be a non-negative number less than {1}", (object) partitionInfo.PartitionId, (object) partitionInfo.TotalPartitions));
      return partitionInfo;
    }

    public async Task<IEnumerable<Guid>> ScheduleMultipleJobsAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      bool enabledForMultiDomain)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      Dictionary<int, Guid> partitionsWithRunningJob = new Dictionary<int, Guid>();
      foreach (TeamFoundationJobDefinition foundationJobDefinition in (IEnumerable<TeamFoundationJobDefinition>) service.QueryJobDefinitions(requestContext, service.QueryJobQueue(requestContext, (IEnumerable<Guid>) null).Where<TeamFoundationJobQueueEntry>((Func<TeamFoundationJobQueueEntry, bool>) (job => job != null && job.QueueTime < DateTime.UtcNow)).Select<TeamFoundationJobQueueEntry, Guid>((Func<TeamFoundationJobQueueEntry, Guid>) (job => job.JobId))) ?? Enumerable.Empty<TeamFoundationJobDefinition>())
      {
        if (foundationJobDefinition != null && foundationJobDefinition.Name.StartsWith(this.JobNamePrefix) && (!enabledForMultiDomain || !(TeamFoundationSerializationUtility.Deserialize<JobParameters>(foundationJobDefinition.Data).DomainId != this.ParentJobParameters.DomainId)))
        {
          JobParameters partitionInfo = MultiJobHelper.GetPartitionInfo(foundationJobDefinition.Data);
          Guid guid;
          if (partitionsWithRunningJob.TryGetValue(partitionInfo.PartitionId, out guid))
            tracer.TraceAlways(string.Format("Multiple Jobs running for partition: {0}, prevJob: {1}, current: {2}", (object) partitionInfo.PartitionId, (object) guid, (object) foundationJobDefinition.JobId));
          else
            partitionsWithRunningJob.Add(partitionInfo.PartitionId, foundationJobDefinition.JobId);
        }
      }
      List<Guid> createdOrRunningJobs = new List<Guid>();
      for (int partition = 0; partition < this.ParentJobParameters.TotalPartitions; ++partition)
      {
        Guid guid;
        if (partitionsWithRunningJob.TryGetValue(partition, out guid))
        {
          tracer.TraceAlways(string.Format("Skipping scheduling job for partition: {0}, as another job (id: {1}) is already in progress", (object) partition, (object) guid));
          createdOrRunningJobs.Add(guid);
        }
        else
        {
          guid = this.QueueJobWithParameters(requestContext, partition, enabledForMultiDomain);
          createdOrRunningJobs.Add(guid);
          tracer.TraceInfo(string.Format("Scheduled job for partition: {0}, jobId: {1}", (object) partition, (object) guid));
          try
          {
            await Task.Delay(this.JobSchedulingInterval, requestContext.CancellationToken).ConfigureAwait(true);
          }
          catch (TaskCanceledException ex)
          {
            break;
          }
        }
      }
      tracer.TraceAlways(string.Format("Completed scheduling {0} jobs for {1} partitions", (object) createdOrRunningJobs.Count, (object) this.ParentJobParameters.TotalPartitions));
      IEnumerable<Guid> guids = (IEnumerable<Guid>) createdOrRunningJobs;
      partitionsWithRunningJob = (Dictionary<int, Guid>) null;
      createdOrRunningJobs = (List<Guid>) null;
      return guids;
    }

    private Guid QueueJobWithParameters(
      IVssRequestContext requestContext,
      int partitionId,
      bool enabledForMultiDomain)
    {
      JobParameters objectToSerialize = new JobParameters()
      {
        PartitionId = partitionId,
        TotalPartitions = this.ParentJobParameters.TotalPartitions,
        RunId = this.ParentJobParameters.RunId
      };
      if (enabledForMultiDomain)
        objectToSerialize.DomainId = this.ParentJobParameters.DomainId;
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize);
      return requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext, this.JobNamePrefix + partitionId.ToString(), this.JobExtensionName, xml, this.JobPriority);
    }
  }
}
