// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TimelineExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class TimelineExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.Timeline ToBuildTimeline(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline taskTimeline,
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      ISecuredObject securedObject)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "TimelineExtensions.ToBuildTimeline"))
      {
        ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
        if (taskTimeline == null)
          return (Microsoft.TeamFoundation.Build.WebApi.Timeline) null;
        IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
        Microsoft.TeamFoundation.Build.WebApi.Timeline buildTimeline = new Microsoft.TeamFoundation.Build.WebApi.Timeline(securedObject);
        buildTimeline.ChangeId = taskTimeline.ChangeId;
        buildTimeline.Id = taskTimeline.Id;
        buildTimeline.LastChangedBy = taskTimeline.LastChangedBy;
        buildTimeline.LastChangedOn = taskTimeline.LastChangedOn;
        buildTimeline.Url = service.GetTimelineRestUrl(requestContext, projectId, buildId, new Guid?(taskTimeline.Id));
        buildTimeline.Records.AddRange(taskTimeline.Records.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Microsoft.TeamFoundation.Build.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, Microsoft.TeamFoundation.Build.WebApi.TimelineRecord>) (r => r.ToBuildTimelineRecord(requestContext, projectId, buildId, securedObject))));
        return buildTimeline;
      }
    }
  }
}
