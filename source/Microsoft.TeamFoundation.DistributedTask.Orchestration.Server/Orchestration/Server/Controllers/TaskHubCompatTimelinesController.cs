// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubCompatTimelinesController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "timelines")]
  public sealed class TaskHubCompatTimelinesController : TaskHubCompatApiController
  {
    [HttpPost]
    public Timeline CreateTimeline(Guid planId, Timeline timeline)
    {
      timeline = this.Hub.CreateTimeline(this.TfsRequestContext, Guid.Empty, planId, timeline);
      if (timeline != null)
        timeline.UpdateLocations(this.TfsRequestContext, planId);
      return timeline;
    }

    [HttpDelete]
    public void DeleteTimeline(Guid planId, Guid timelineId) => this.Hub.DeleteTimeline(this.TfsRequestContext, Guid.Empty, planId, timelineId);

    [HttpGet]
    public Timeline GetTimeline(Guid planId, Guid timelineId, int changeId = 0, bool includeRecords = false)
    {
      Timeline timeline = this.Hub.GetTimeline(this.TfsRequestContext, Guid.Empty, planId, timelineId, changeId, includeRecords);
      if (timeline == null)
        throw new TimelineNotFoundException(TaskResources.TimelineNotFound((object) planId, (object) timelineId));
      timeline.UpdateLocations(this.TfsRequestContext, planId);
      return timeline;
    }

    [HttpGet]
    public IEnumerable<Timeline> GetTimelines(Guid planId) => (IEnumerable<Timeline>) this.Hub.GetTimelines(this.TfsRequestContext, Guid.Empty, planId).Select<Timeline, Timeline>((Func<Timeline, Timeline>) (timeline =>
    {
      timeline.UpdateLocations(this.TfsRequestContext, planId);
      return timeline;
    })).ToArray<Timeline>();
  }
}
