// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessAdminBehaviorsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processAdmin", ResourceName = "behaviors", ResourceVersion = 1)]
  public class ProcessAdminBehaviorsController : WorkItemTrackingApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (AdminBehavior), null, null)]
    [ClientLocationId("90BF9317-3571-487B-BC8C-A523BA0E05D7")]
    [ClientExample("GET__admin_behavior.json", null, null, null)]
    public HttpResponseMessage GetBehavior(Guid processId, string behaviorRefName)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(behaviorRefName))
        throw new VssPropertyValidationException(nameof (behaviorRefName), ResourceStrings.NullOrEmptyParameter((object) nameof (behaviorRefName)));
      return this.Request.CreateResponse<AdminBehavior>(HttpStatusCode.OK, AdminBehavior.Create(this.TfsRequestContext, this.TfsRequestContext.GetService<IBehaviorService>().GetBehavior(this.TfsRequestContext, processId, behaviorRefName, bypassCache: true)));
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<AdminBehavior>), null, null)]
    [ClientLocationId("90BF9317-3571-487B-BC8C-A523BA0E05D7")]
    [ClientExample("GET__admin_behaviors_list.json", null, null, null)]
    public HttpResponseMessage GetBehaviors(Guid processId)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      return this.Request.CreateResponse<IEnumerable<AdminBehavior>>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IBehaviorService>().GetBehaviors(this.TfsRequestContext, processId, bypassCache: true).Select<Behavior, AdminBehavior>((Func<Behavior, AdminBehavior>) (b => AdminBehavior.Create(this.TfsRequestContext, b))));
    }
  }
}
