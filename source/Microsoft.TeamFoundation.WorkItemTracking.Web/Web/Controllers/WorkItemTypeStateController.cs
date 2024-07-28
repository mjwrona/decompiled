// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemTypeStateController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemTypeStates", ResourceVersion = 1)]
  public class WorkItemTypeStateController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5920000;

    public override string TraceArea => "workItemTypeStates";

    [TraceFilter(5920000, 5920010)]
    [HttpGet]
    [ClientExample("GET__workitem_type_states.json", "Get work item type states", null, null)]
    [PublicProjectRequestRestrictions("5.0")]
    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor> GetWorkItemTypeStates(
      string type)
    {
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor> source = this.TfsRequestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemStateColors(this.TfsRequestContext, this.ProjectId, type).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemStateColor, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemStateColor, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>) (c => new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor((ISecuredObject) c)
      {
        Name = c.Name,
        Color = c.Color,
        Category = c.Category
      }));
      return source == null ? (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>) null : (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>) source.ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>();
    }
  }
}
