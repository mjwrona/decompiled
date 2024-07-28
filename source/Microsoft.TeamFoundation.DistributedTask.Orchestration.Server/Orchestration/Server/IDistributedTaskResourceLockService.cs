// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IDistributedTaskResourceLockService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [DefaultServiceImplementation(typeof (FrameworkResourceLockService))]
  public interface IDistributedTaskResourceLockService : IVssFrameworkService
  {
    ResourceLockRequest GetRequestByCheckRun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkRunId);

    List<ResourceLockRequest> GetRequestsByCheckRuns(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> checkRunIds);

    ResourceLockRequest GetActiveResourceLock(
      IVssRequestContext requestContext,
      string resourceId,
      string resourceType);

    List<ResourceLockRequest> GetActiveResourceLocks(
      IVssRequestContext requestContext,
      IEnumerable<CheckResource> resources);

    List<ResourceLockRequest> QueueResourceLockRequests(
      IVssRequestContext requestContext,
      IEnumerable<ResourceLockRequest> request);

    ResourceLockRequest UpdateResourceLockRequest(
      IVssRequestContext requestContext,
      long requestId,
      ResourceLockStatus status,
      DateTime? assignTime = null,
      DateTime? finishTime = null);

    List<ResourceLockRequest> UpdateResourceLockRequests(
      IVssRequestContext requestContext,
      IEnumerable<ResourceLockRequest> requestsToUpdate);

    Task<List<ResourceLockRequest>> FreeResourceLocksAsync(
      IVssRequestContext requestContext,
      Guid planId,
      string nodeName = null,
      int? nodeAttempt = null);

    Task<AssignResourceLockRequestsResult> AssignResourceLockRequestsAsync(
      IVssRequestContext requestContext);

    void CleanupRequestTable(IVssRequestContext requestContext);

    Task CleanupAbanonedResourceLocks(IVssRequestContext requestContext);
  }
}
