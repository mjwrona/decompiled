// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessFormLayoutPagesController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Extensions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "Pages", ResourceVersion = 1)]
  [ControllerApiVersion(5.0)]
  public class ProcessFormLayoutPagesController : WorkItemTrackingApiController
  {
    [HttpPost]
    [ClientExample("POST__page.json", "Add a page", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page AddPage(
      Guid processId,
      string witRefName,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page page)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page>(page, nameof (page));
      ArgumentUtility.CheckForNull<string>(page.Label, "Label");
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page serverModel = page.GetServerModel();
      Layout var = this.TfsRequestContext.GetService<IFormLayoutService>().AddPage(this.TfsRequestContext, processId, witRefName, serverModel, page.Order);
      ArgumentUtility.CheckForNull<Layout>(var, "layout");
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Page", "Add");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("PageName", (object) page.Label);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return var.FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page>(serverModel.Id).GetProcessWebApiModel();
    }

    [HttpPatch]
    [ClientExample("PATCH__page.json", "Update page", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page UpdatePage(
      Guid processId,
      string witRefName,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page page)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page>(page, nameof (page));
      ArgumentUtility.CheckStringForNullOrEmpty(page.Label, "Label");
      ArgumentUtility.EnsureIsNull((object) page.Sections, "Sections");
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page serverModel = page.GetServerModel();
      Layout var = this.TfsRequestContext.GetService<IFormLayoutService>().EditPage(this.TfsRequestContext, processId, witRefName, serverModel, page.Order);
      ArgumentUtility.CheckForNull<Layout>(var, "layout");
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Page", "Update");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("PageName", (object) page.Label);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return var.FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page>(serverModel.Id).GetProcessWebApiModel();
    }

    [HttpDelete]
    [ClientExample("DELETE__page.json", "Delete the page", null, null)]
    public void RemovePage(Guid processId, string witRefName, string pageId)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      IFormLayoutService service = this.TfsRequestContext.GetService<IFormLayoutService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page descendant = service.GetLayout(this.TfsRequestContext, processId, witRefName).ComposedLayout.FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page>(pageId);
      service.RemovePage(this.TfsRequestContext, processId, witRefName, pageId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Page", "Delete");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("PageName", (object) descendant.Label);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
    }
  }
}
