// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubTimelinesController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ControllerApiVersion(2.0)]
  [ClientTemporarySwaggerExclusion]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "timelines")]
  public sealed class TaskHubTimelinesController : TaskHubApiController
  {
    [HttpPost]
    public Timeline CreateTimeline(Guid planId, Timeline timeline)
    {
      timeline = this.Hub.CreateTimeline(this.TfsRequestContext, this.ScopeIdentifier, planId, timeline);
      if (timeline != null)
        timeline.UpdateLocations(this.TfsRequestContext, planId);
      return timeline;
    }

    [HttpDelete]
    public void DeleteTimeline(Guid planId, Guid timelineId) => this.Hub.DeleteTimeline(this.TfsRequestContext, this.ScopeIdentifier, planId, timelineId);

    [HttpGet]
    public Timeline GetTimeline(Guid planId, Guid timelineId, [ClientQueryParameter] int changeId = 0, [ClientQueryParameter] bool includeRecords = false)
    {
      Timeline timeline = this.Hub.GetTimeline(this.TfsRequestContext, this.ScopeIdentifier, planId, timelineId, changeId, includeRecords);
      if (timeline == null)
        throw new TimelineNotFoundException(TaskResources.TimelineNotFound((object) planId, (object) timelineId));
      timeline.UpdateLocations(this.TfsRequestContext, planId);
      return timeline;
    }

    [HttpGet]
    public IEnumerable<Timeline> GetTimelines(Guid planId)
    {
      IEnumerable<Timeline> timelines = this.Hub.GetTimelines(this.TfsRequestContext, this.ScopeIdentifier, planId);
      return timelines == null ? (IEnumerable<Timeline>) Array.Empty<Timeline>() : (IEnumerable<Timeline>) timelines.Select<Timeline, Timeline>((Func<Timeline, Timeline>) (timeline =>
      {
        timeline.UpdateLocations(this.TfsRequestContext, planId);
        return timeline;
      })).ToList<Timeline>();
    }
  }
}
