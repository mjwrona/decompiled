// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemTypeColorAndIconController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemTypeColorAndIcon", ResourceVersion = 1)]
  [ClientInternalUseOnly(true)]
  public class WorkItemTypeColorAndIconController : TfsApiController
  {
    [HttpPost]
    [ClientLocationId("F0F8DC62-3975-48CE-8051-F636B68B52E3")]
    [ClientResponseType(typeof (List<KeyValuePair<string, List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeColorAndIcon>>>), null, null)]
    public HttpResponseMessage GetWorkItemTypeColorAndIcons([FromBody] string[] projectNames)
    {
      List<string> list = projectNames != null ? ((IEnumerable<string>) projectNames).Distinct<string>().Select<string, string>((Func<string, string>) (p => p?.Trim())).ToList<string>() : (List<string>) null;
      if (list == null || list.Any<string>((Func<string, bool>) (p => string.IsNullOrEmpty(p))))
        throw new EmptyProjectNameException();
      return list.Any<string>() ? this.Request.CreateResponse<List<KeyValuePair<string, IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeColorAndIcon>>>>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemTypeColorAndIconsByProjectNames(this.TfsRequestContext, (IReadOnlyCollection<string>) list).Select<KeyValuePair<string, IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon>>, KeyValuePair<string, IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeColorAndIcon>>>((Func<KeyValuePair<string, IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon>>, KeyValuePair<string, IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeColorAndIcon>>>) (item => new KeyValuePair<string, IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeColorAndIcon>>(item.Key, (IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeColorAndIcon>) item.Value.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeColorAndIcon>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeColorAndIcon>) (colorAndIcon => WorkItemTypeColorAndIconFactory.Create(colorAndIcon))).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeColorAndIcon>()))).ToList<KeyValuePair<string, IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeColorAndIcon>>>()) : this.Request.CreateResponse<IEnumerable<KeyValuePair<string, List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeColorAndIcon>>>>(HttpStatusCode.OK, Enumerable.Empty<KeyValuePair<string, List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeColorAndIcon>>>());
    }

    public override string ActivityLogArea => "WorkItem Tracking";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      exceptionMap.AddStatusCode<InvalidProjectNameException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemTrackingProjectNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<EmptyProjectNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProjectDoesNotExistWithNameException>(HttpStatusCode.NotFound);
    }
  }
}
