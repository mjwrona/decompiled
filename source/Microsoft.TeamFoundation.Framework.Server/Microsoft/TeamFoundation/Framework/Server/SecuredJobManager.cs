// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuredJobManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecuredJobManager : IVssFrameworkService
  {
    private TeamFoundationSecurityService m_securityService;
    private TeamFoundationJobService m_jobService;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_securityService = systemRequestContext.GetService<TeamFoundationSecurityService>();
      this.m_jobService = systemRequestContext.GetService<TeamFoundationJobService>();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void UpdateJobDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobUpdates)
    {
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.JobNamespaceId).CheckPermission(requestContext, FrameworkSecurity.JobNamespaceToken, 4);
      this.m_jobService.UpdateJobDefinitions(requestContext, jobsToDelete, jobUpdates, false);
    }

    public List<TeamFoundationJobDefinition> QueryJobDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds)
    {
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.JobNamespaceId).CheckPermission(requestContext, FrameworkSecurity.JobNamespaceToken, 1);
      return this.m_jobService.QueryJobDefinitions(requestContext, this.NullIfEmpty<Guid>(jobIds));
    }

    public int QueueJobsInternal(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds,
      bool highPriority,
      int maxDelaySeconds)
    {
      if (highPriority && maxDelaySeconds != 0)
        throw new ArgumentException(FrameworkResources.ScheduledHighPriorityJobsProhibited());
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.JobNamespaceId).CheckPermission(requestContext, FrameworkSecurity.JobNamespaceToken, 2);
      List<TeamFoundationJobReference> jobReferences = TeamFoundationJobReference.ConvertJobIdsToJobReferences(jobIds);
      JobPriorityLevel level = TeamFoundationJobService.ConvertPriorityBitToLevel(highPriority);
      return this.m_jobService.QueueJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences, level, maxDelaySeconds, false);
    }

    public bool StopJob(IVssRequestContext requestContext, Guid jobId)
    {
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.JobNamespaceId).CheckPermission(requestContext, FrameworkSecurity.JobNamespaceToken, 2);
      return this.m_jobService.StopJob(requestContext, jobId);
    }

    public List<TeamFoundationJobHistoryEntry> QueryJobHistory(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds)
    {
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.JobNamespaceId).CheckPermission(requestContext, FrameworkSecurity.JobNamespaceToken, 1);
      return this.m_jobService.QueryJobHistory(requestContext, this.NullIfEmpty<Guid>(jobIds));
    }

    public List<TeamFoundationJobHistoryEntry> QueryLatestJobHistory(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds)
    {
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.JobNamespaceId).CheckPermission(requestContext, FrameworkSecurity.JobNamespaceToken, 1);
      return this.m_jobService.QueryLatestJobHistory(requestContext, this.NullIfEmpty<Guid>(jobIds));
    }

    private IEnumerable<T> NullIfEmpty<T>(IEnumerable<T> sequence) => sequence != null && !sequence.Any<T>() ? (IEnumerable<T>) null : sequence;
  }
}
