// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemStateColorController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
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
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workitemStateColor", ResourceVersion = 1)]
  [ClientInternalUseOnly(true)]
  public class WorkItemStateColorController : TfsApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      exceptionMap.AddStatusCode<InvalidProjectNameException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemTrackingProjectNotFoundException>(HttpStatusCode.NotFound);
    }

    [HttpPost]
    [ClientLocationId("0B83DF8A-3496-4DDB-BA44-63634F4CDA61")]
    [ClientResponseType(typeof (List<ProjectWorkItemStateColors>), null, null)]
    public HttpResponseMessage GetWorkItemStateColors([FromBody] string[] projectNames)
    {
      List<string> list = ((IEnumerable<string>) projectNames).Distinct<string>().ToList<string>();
      if (!list.Any<string>())
        return this.Request.CreateResponse<IEnumerable<ProjectWorkItemStateColors>>(HttpStatusCode.OK, Enumerable.Empty<ProjectWorkItemStateColors>());
      IWorkItemMetadataFacadeService service = this.TfsRequestContext.GetService<IWorkItemMetadataFacadeService>();
      List<string> stringList = this.ResolveProjectNames(this.TfsRequestContext, list);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<string> projectNames1 = stringList;
      return this.Request.CreateResponse<List<ProjectWorkItemStateColors>>(HttpStatusCode.OK, service.GetWorkItemStateColorsByProjectName(tfsRequestContext, (IReadOnlyCollection<string>) projectNames1).Select<KeyValuePair<string, IReadOnlyDictionary<string, IReadOnlyCollection<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemStateColor>>>, ProjectWorkItemStateColors>((Func<KeyValuePair<string, IReadOnlyDictionary<string, IReadOnlyCollection<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemStateColor>>>, ProjectWorkItemStateColors>) (projectEntry => new ProjectWorkItemStateColors()
      {
        ProjectName = projectEntry.Key,
        workItemTypeStateColors = (ICollection<WorkItemTypeStateColors>) projectEntry.Value.Select<KeyValuePair<string, IReadOnlyCollection<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemStateColor>>, WorkItemTypeStateColors>((Func<KeyValuePair<string, IReadOnlyCollection<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemStateColor>>, WorkItemTypeStateColors>) (witColors => new WorkItemTypeStateColors()
        {
          WorkItemTypeName = witColors.Key,
          StateColors = (ICollection<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>) witColors.Value.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemStateColor, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemStateColor, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>) (v => new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor()
          {
            Name = v.Name,
            Color = v.Color,
            Category = v.Category
          })).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>()
        })).ToList<WorkItemTypeStateColors>()
      })).ToList<ProjectWorkItemStateColors>());
    }

    public override string ActivityLogArea => "WorkItem Tracking";

    private List<string> ResolveProjectNames(
      IVssRequestContext requestContext,
      List<string> nameOrIds)
    {
      List<string> stringList = new List<string>();
      foreach (string nameOrId in nameOrIds)
      {
        Guid result;
        if (Guid.TryParse(nameOrId, out result))
          stringList.Add(requestContext.GetService<IProjectService>().GetProjectName(requestContext, result));
        else
          stringList.Add(nameOrId);
      }
      return stringList;
    }
  }
}
