// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.FrameworkResourceLockService
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
  public class FrameworkResourceLockService : 
    IDistributedTaskResourceLockService,
    IVssFrameworkService
  {
    public ResourceLockRequest GetRequestByCheckRun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkRunId)
    {
      throw new NotSupportedException();
    }

    public List<ResourceLockRequest> GetRequestsByCheckRuns(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> checkRunIds)
    {
      throw new NotSupportedException();
    }

    public ResourceLockRequest GetActiveResourceLock(
      IVssRequestContext requestContext,
      string resourceId,
      string resourceType)
    {
      throw new NotSupportedException();
    }

    public List<ResourceLockRequest> GetActiveResourceLocks(
      IVssRequestContext requestContext,
      IEnumerable<CheckResource> resources)
    {
      throw new NotSupportedException();
    }

    public Task<List<ResourceLockRequest>> FreeResourceLocksAsync(
      IVssRequestContext requestContext,
      Guid planId,
      string nodeName = null,
      int? nodeAttempt = null)
    {
      throw new NotSupportedException();
    }

    public List<ResourceLockRequest> QueueResourceLockRequests(
      IVssRequestContext requestContext,
      IEnumerable<ResourceLockRequest> requests)
    {
      throw new NotSupportedException();
    }

    public ResourceLockRequest UpdateResourceLockRequest(
      IVssRequestContext requestContext,
      long requestId,
      ResourceLockStatus status,
      DateTime? assignTime = null,
      DateTime? finishTime = null)
    {
      throw new NotSupportedException();
    }

    public List<ResourceLockRequest> UpdateResourceLockRequests(
      IVssRequestContext requestContext,
      IEnumerable<ResourceLockRequest> requestsToUpdate)
    {
      throw new NotSupportedException();
    }

    public Task<AssignResourceLockRequestsResult> AssignResourceLockRequestsAsync(
      IVssRequestContext requestContext)
    {
      throw new NotSupportedException();
    }

    public void CleanupRequestTable(IVssRequestContext requestContext) => throw new NotSupportedException();

    public Task CleanupAbanonedResourceLocks(IVssRequestContext requestContext) => throw new NotImplementedException();

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
