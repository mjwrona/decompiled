// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubTimelineRecordsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "records")]
  public sealed class TaskHubTimelineRecordsController : TaskHubApiController
  {
    [HttpGet]
    [ClientTemporarySwaggerExclusion]
    public IEnumerable<TimelineRecord> GetRecords(Guid planId, Guid timelineId, [ClientQueryParameter] int changeId = 0)
    {
      Timeline timeline = this.Hub.GetTimeline(this.TfsRequestContext, this.ScopeIdentifier, planId, timelineId, changeId, true);
      if (timeline == null)
        return (IEnumerable<TimelineRecord>) Array.Empty<TimelineRecord>();
      foreach (TimelineRecord record in timeline.Records)
        record.UpdateLocations(this.TfsRequestContext, planId, timelineId);
      return (IEnumerable<TimelineRecord>) timeline.Records;
    }

    [HttpPatch]
    [ClientResponseType(typeof (IEnumerable<TimelineRecord>), null, null)]
    [ClientExample("PATCH__distributedtask_UpdateRecords_.json", "Update timeline's records", null, null)]
    public async Task<IEnumerable<TimelineRecord>> UpdateRecords(
      Guid planId,
      Guid timelineId,
      VssJsonCollectionWrapper<IEnumerable<TimelineRecord>> records)
    {
      TaskHubTimelineRecordsController recordsController = this;
      ArgumentUtility.CheckForNull<VssJsonCollectionWrapper<IEnumerable<TimelineRecord>>>(records, nameof (records), "DistributedTask");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) records.Value, nameof (records), "DistributedTask");
      Timeline timeline = await recordsController.Hub.UpdateTimelineAsync(recordsController.TfsRequestContext, recordsController.ScopeIdentifier, planId, timelineId, (IList<TimelineRecord>) records.Value.ToList<TimelineRecord>(), true);
      foreach (TimelineRecord record in timeline.Records)
        record.UpdateLocations(recordsController.TfsRequestContext, planId, timelineId);
      return (IEnumerable<TimelineRecord>) timeline.Records;
    }
  }
}
