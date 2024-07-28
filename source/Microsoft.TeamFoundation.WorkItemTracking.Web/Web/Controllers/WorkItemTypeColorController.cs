// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemTypeColorController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workitemTypeColor", ResourceVersion = 1)]
  [ClientInternalUseOnly(true)]
  public class WorkItemTypeColorController : TfsApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      exceptionMap.AddStatusCode<InvalidProjectNameException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemTrackingProjectNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<EmptyProjectNameException>(HttpStatusCode.BadRequest);
    }

    [HttpPost]
    [ClientLocationId("958FDE80-115E-43FB-BD65-749C48057FAF")]
    [ClientResponseType(typeof (List<KeyValuePair<string, List<WorkItemTypeColor>>>), null, null)]
    public HttpResponseMessage GetWorkItemTypeColors([FromBody] string[] projectNames)
    {
      List<string> list = ((IEnumerable<string>) projectNames).Distinct<string>().Select<string, string>((Func<string, string>) (p => p.Trim())).ToList<string>();
      if (list.Any<string>((Func<string, bool>) (p => string.IsNullOrEmpty(p))))
        throw new EmptyProjectNameException();
      return list.Any<string>() ? this.Request.CreateResponse<List<KeyValuePair<string, IReadOnlyCollection<WorkItemTypeColor>>>>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemTypeColorsByProjectNames(this.TfsRequestContext, (IReadOnlyCollection<string>) list).Select<KeyValuePair<string, IReadOnlyCollection<WorkItemColor>>, KeyValuePair<string, IReadOnlyCollection<WorkItemTypeColor>>>((Func<KeyValuePair<string, IReadOnlyCollection<WorkItemColor>>, KeyValuePair<string, IReadOnlyCollection<WorkItemTypeColor>>>) (item => new KeyValuePair<string, IReadOnlyCollection<WorkItemTypeColor>>(item.Key, (IReadOnlyCollection<WorkItemTypeColor>) item.Value.Select<WorkItemColor, WorkItemTypeColor>((Func<WorkItemColor, WorkItemTypeColor>) (color => WorkItemTypeColorFactory.TransformToWorkItemTypeColor(color))).ToList<WorkItemTypeColor>()))).ToList<KeyValuePair<string, IReadOnlyCollection<WorkItemTypeColor>>>()) : this.Request.CreateResponse<IEnumerable<KeyValuePair<string, List<WorkItemTypeColor>>>>(HttpStatusCode.OK, Enumerable.Empty<KeyValuePair<string, List<WorkItemTypeColor>>>());
    }

    public override string ActivityLogArea => "WorkItem Tracking";
  }
}
