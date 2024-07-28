// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationServicingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationServicingService))]
  public interface ITeamFoundationServicingService : IVssFrameworkService
  {
    ServicingOperation GetServicingOperation(
      IVssRequestContext requestContext,
      string servicingOperation);

    ServicingJobDetail QueueServicingJob(
      IVssRequestContext requestContext,
      ServicingJobData servicingJobData,
      JobPriorityClass priorityClass = JobPriorityClass.AboveNormal,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal,
      Guid? jobId = null);

    ServicingJobDetail PerformServicingJob(
      IVssRequestContext requestContext,
      ServicingJobData servicingJobData,
      Guid? jobId = null,
      ITFLogger logger = null);

    ServicingJobDetail PerformServicingJob(
      IVssRequestContext requestContext,
      ServicingJobData servicingJobData,
      Guid jobId,
      DateTime jobQueueTime,
      ITFLogger logger = null);

    ServicingJobInfo GetServicingJobInfo(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId);

    List<ServicingJobInfo> GetServicingJobsInfo(IVssRequestContext requestContext, Guid jobId);

    List<ServicingStepDetail> GetServicingDetails(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId,
      DateTime queueTime,
      ServicingStepDetailFilterOptions filterOptions,
      long minDetailId,
      out ServicingJobDetail jobDetails);

    List<ServicingJobDetail> QueueUpgradeHosts(
      IVssRequestContext deploymentContext,
      Guid[] hostIds,
      JobPriorityClass jobPriorityClass,
      List<List<string>> operations = null,
      List<KeyValuePair<string, string>> additionalTokens = null);

    ServicingJobDetail PerformUpgradeDatabase(
      IVssRequestContext deploymentContext,
      int databaseId,
      Guid jobId,
      ITFLogger logger = null);

    void RequeueServicingJob(IVssRequestContext requestContext, Guid hostId, Guid jobId);
  }
}
