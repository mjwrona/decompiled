// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ServicingServiceExtensions
  {
    public static List<ServicingStepDetail> GetServicingDetails(
      this ITeamFoundationServicingService servicingService,
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId,
      DateTime queueTime,
      long minDetailId,
      out ServicingJobDetail jobDetails)
    {
      return servicingService.GetServicingDetails(requestContext, hostId, jobId, queueTime, ServicingStepDetailFilterOptions.SpecificQueueTime, minDetailId, out jobDetails);
    }

    public static List<ServicingStepDetail> GetServicingDetails(
      this ITeamFoundationServicingService servicingService,
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId,
      ServicingStepDetailFilterOptions filterOptions,
      long minDetailId,
      out ServicingJobDetail jobDetails)
    {
      return servicingService.GetServicingDetails(requestContext, hostId, jobId, DateTime.MinValue, filterOptions, minDetailId, out jobDetails);
    }

    public static List<ServicingStepDetail> GetServicingDetails(
      this ITeamFoundationServicingService servicingService,
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId,
      DateTime queueTime,
      out ServicingJobDetail jobDetails)
    {
      return servicingService.GetServicingDetails(requestContext, hostId, jobId, queueTime, 0L, out jobDetails);
    }

    public static List<ServicingStepDetail> GetServicingDetails(
      this ITeamFoundationServicingService servicingService,
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId,
      ServicingStepDetailFilterOptions filterOptions,
      out ServicingJobDetail jobDetails)
    {
      return servicingService.GetServicingDetails(requestContext, hostId, jobId, DateTime.MinValue, filterOptions, 0L, out jobDetails);
    }

    public static List<ServicingStepDetail> GetServicingDetails(
      this ITeamFoundationServicingService servicingService,
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId,
      ServicingStepDetailFilterOptions filterOptions)
    {
      return servicingService.GetServicingDetails(requestContext, hostId, jobId, DateTime.MinValue, filterOptions, 0L, out ServicingJobDetail _);
    }
  }
}
