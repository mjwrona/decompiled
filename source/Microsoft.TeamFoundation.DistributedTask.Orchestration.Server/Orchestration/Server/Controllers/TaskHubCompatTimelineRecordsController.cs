// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubCompatTimelineRecordsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "records")]
  public sealed class TaskHubCompatTimelineRecordsController : TaskHubCompatApiController
  {
    [HttpGet]
    public IEnumerable<TimelineRecord> GetRecords(Guid planId, Guid timelineId, int changeId = 0)
    {
      Timeline timeline = this.Hub.GetTimeline(this.TfsRequestContext, Guid.Empty, planId, timelineId, changeId, true);
      if (timeline == null)
        return (IEnumerable<TimelineRecord>) Array.Empty<TimelineRecord>();
      foreach (TimelineRecord record in timeline.Records)
        record.UpdateLocations(this.TfsRequestContext, planId, timelineId);
      return (IEnumerable<TimelineRecord>) timeline.Records;
    }

    [HttpPatch]
    public IEnumerable<TimelineRecord> UpdateRecords(
      Guid planId,
      Guid timelineId,
      VssJsonCollectionWrapper<IEnumerable<TimelineRecord>> records)
    {
      if (records == null || records.Count == 0)
        throw new ArgumentException("no nodes passed to update");
      Timeline timeline = this.Hub.UpdateTimeline(this.TfsRequestContext, Guid.Empty, planId, timelineId, (IList<TimelineRecord>) records.Value.ToList<TimelineRecord>());
      foreach (TimelineRecord record in timeline.Records)
        record.UpdateLocations(this.TfsRequestContext, planId, timelineId);
      return (IEnumerable<TimelineRecord>) timeline.Records;
    }
  }
}
