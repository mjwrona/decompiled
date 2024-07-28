// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessBehaviorsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "behaviors", ResourceVersion = 1)]
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  public class ProcessBehaviorsController : WorkItemTrackingApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (WorkItemBehavior), null, null)]
    [ClientLocationId("D1800200-F184-4E75-A5F2-AD0B04B4373E")]
    [ClientExample("GET__behavior.json", "Get the process behavior definition", null, null)]
    public HttpResponseMessage GetBehavior(
      Guid processId,
      string behaviorRefName,
      [FromUri(Name = "$expand")] GetBehaviorsExpand expand = GetBehaviorsExpand.None)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(behaviorRefName))
        throw new VssPropertyValidationException(nameof (behaviorRefName), ResourceStrings.NullOrEmptyParameter((object) nameof (behaviorRefName)));
      Behavior behavior = this.TfsRequestContext.GetService<IBehaviorService>().GetBehavior(this.TfsRequestContext, processId, behaviorRefName);
      bool includeFields = (expand & GetBehaviorsExpand.Fields) == GetBehaviorsExpand.Fields;
      return this.Request.CreateResponse<WorkItemBehavior>(HttpStatusCode.OK, WorkItemBehaviorModelFactory.Create(this.TfsRequestContext, processId, behavior, includeFields));
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<WorkItemBehavior>), null, null)]
    [ClientLocationId("D1800200-F184-4E75-A5F2-AD0B04B4373E")]
    [ClientExample("GET__behaviors_list.json", "Get the list of process behaviors", null, null)]
    public HttpResponseMessage GetBehaviors(Guid processId, [FromUri(Name = "$expand")] GetBehaviorsExpand expand = GetBehaviorsExpand.None)
    {
      IReadOnlyCollection<Behavior> behaviors = !(processId == Guid.Empty) ? this.TfsRequestContext.GetService<IBehaviorService>().GetBehaviors(this.TfsRequestContext, processId) : throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      bool includeFields = (expand & GetBehaviorsExpand.Fields) == GetBehaviorsExpand.Fields;
      return this.Request.CreateResponse<IEnumerable<WorkItemBehavior>>(HttpStatusCode.OK, WorkItemBehaviorModelFactory.Create(this.TfsRequestContext, processId, (IEnumerable<Behavior>) behaviors, includeFields));
    }
  }
}
