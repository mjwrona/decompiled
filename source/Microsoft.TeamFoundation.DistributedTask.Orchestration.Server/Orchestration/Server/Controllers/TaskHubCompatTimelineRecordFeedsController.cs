// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubCompatTimelineRecordFeedsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

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
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "feed")]
  public class TaskHubCompatTimelineRecordFeedsController : TaskHubCompatApiController
  {
    [HttpPost]
    public void PostLines(
      Guid planId,
      Guid timelineId,
      Guid recordId,
      VssJsonCollectionWrapper<IEnumerable<string>> lines)
    {
      List<string> list = lines.Value.ToList<string>();
      if (list.Count <= 0)
        return;
      this.Hub.FeedReceived(this.TfsRequestContext, Guid.Empty, planId, timelineId, recordId, Guid.Empty, (IList<string>) list);
    }
  }
}
