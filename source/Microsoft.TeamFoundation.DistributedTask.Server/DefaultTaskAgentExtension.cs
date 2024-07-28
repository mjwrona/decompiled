// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DefaultTaskAgentExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class DefaultTaskAgentExtension : ITaskAgentExtension
  {
    public Task<(int MaxParallelism, int RequestTimeout)> CheckBillingResourcesAsync(
      IVssRequestContext requestContext,
      int poolId,
      Guid scopeId,
      Guid planId,
      string parallelismTag,
      bool throwException = true)
    {
      return Task.FromResult<(int, int)>((1, 0));
    }

    public Task FilterCapabilitiesAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgent agent)
    {
      return Task.CompletedTask;
    }

    public async Task JobAssignedAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest jobRequest,
      bool isAgentCloudBacked)
    {
      DistributedTaskResourceService resourceService = requestContext.GetService<DistributedTaskResourceService>();
      await resourceService.RaiseEventAsync<JobAssignedEvent>(requestContext.Elevate(), jobRequest.ServiceOwner, jobRequest.HostId, jobRequest.ScopeId, jobRequest.PlanType, jobRequest.PlanId, new JobAssignedEvent(jobRequest.JobId, jobRequest));
      resourceService.GetTaskAgentPoolExtension(requestContext, poolId).AgentRequestAssigned(requestContext, poolId, jobRequest);
      resourceService = (DistributedTaskResourceService) null;
    }

    public void JobCompleted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest jobRequest,
      bool isAgentCloudBacked)
    {
    }

    public void QueueAgentRequestAssignmentJob(IVssRequestContext requestContext)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      try
      {
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          TaskConstants.AgentRequestAssignmentJob
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015131, "ResourceService", ex);
        throw;
      }
    }

    public Task<Stream> GetAgentPoolMetadataAsync(
      IVssRequestContext requestContext,
      string poolName,
      int? poolMetadataFileId)
    {
      Stream stream = (Stream) null;
      if (poolMetadataFileId.HasValue)
      {
        CompressionType compressionType;
        stream = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) poolMetadataFileId.Value, out compressionType);
        switch (compressionType)
        {
          case CompressionType.None:
            break;
          case CompressionType.GZip:
            using (stream)
              return Task.FromResult<Stream>(DefaultTaskAgentExtension.Decompress(stream));
          default:
            using (stream)
              throw new DistributedTaskException(TaskResources.UnexpectedMetadataCompressionType());
        }
      }
      return Task.FromResult<Stream>(stream);
    }

    private static Stream Decompress(Stream compressedStream)
    {
      MemoryStream destination = new MemoryStream();
      using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
      {
        gzipStream.CopyTo((Stream) destination);
        destination.Position = 0L;
        return (Stream) destination;
      }
    }
  }
}
