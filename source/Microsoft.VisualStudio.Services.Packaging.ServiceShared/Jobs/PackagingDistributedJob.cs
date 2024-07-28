// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.PackagingDistributedJob
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs
{
  public abstract class PackagingDistributedJob : VssAsyncJobExtension
  {
    public abstract IAsyncHandler<DistributedJobRequest, NullResult> BootstrapHandlerFor(
      IVssRequestContext collectionContext);

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      PackagingDistributedJob sendInTheThisObject = this;
      requestContext.CheckDeploymentRequestContext();
      DistributedJobRequest request = TeamFoundationSerializationUtility.Deserialize<DistributedJobRequest>(jobDefinition.Data);
      ITeamFoundationHostManagementService hostMgmtSvc = requestContext.GetService<ITeamFoundationHostManagementService>();
      ITracerService tracerFacade = requestContext.GetTracerFacade();
      List<Guid> failedCollections = new List<Guid>();
      DistributedJobTelemetry jobTelemetry = new DistributedJobTelemetry();
      VssJobResult vssJobResult;
      using (ITracerBlock tracer = tracerFacade.Enter((object) sendInTheThisObject, nameof (RunAsync)))
      {
        foreach (Guid collectionId1 in request.CollectionIds)
        {
          Guid collectionId = collectionId1;
          try
          {
            using (IVssRequestContext collectionContext = hostMgmtSvc.BeginRequest(requestContext, collectionId, RequestContextType.SystemContext))
            {
              NullResult nullResult = await sendInTheThisObject.BootstrapHandlerFor(collectionContext).Handle(new DistributedJobRequest()
              {
                CollectionIds = new List<Guid>()
                {
                  collectionId
                },
                Data = request.Data,
                Feeds = request.Feeds
              });
              ++jobTelemetry.SuccessCount;
            }
          }
          catch (HostDoesNotExistException ex)
          {
            tracer.TraceError(string.Format("Could not initiate host {0} because it does not exist (probably migrated or deleted).", (object) collectionId));
            ++jobTelemetry.InactiveCount;
          }
          catch (HostShutdownException ex)
          {
            tracer.TraceError(string.Format("Could not initiate host {0} because it is shut down (probably migrating).", (object) collectionId));
            ++jobTelemetry.InactiveCount;
          }
          catch (Exception ex)
          {
            failedCollections.Add(collectionId);
            if (jobTelemetry.Exception != null)
              jobTelemetry.LogException(ex);
          }
          collectionId = new Guid();
        }
        jobTelemetry.FailedIdsMax20 = new List<Guid>(failedCollections.Take<Guid>(20));
        vssJobResult = new JobResult()
        {
          Result = (failedCollections.Any<Guid>() ? TeamFoundationJobExecutionResult.Failed : TeamFoundationJobExecutionResult.Succeeded),
          Telemetry = ((JobTelemetry) jobTelemetry)
        }.ToVssJobResult();
      }
      request = (DistributedJobRequest) null;
      hostMgmtSvc = (ITeamFoundationHostManagementService) null;
      failedCollections = (List<Guid>) null;
      jobTelemetry = (DistributedJobTelemetry) null;
      return vssJobResult;
    }
  }
}
