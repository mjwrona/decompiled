// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.FormLayoutControlsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Extensions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processDefinitions", ResourceName = "Controls", ResourceVersion = 1)]
  public class FormLayoutControlsController : WorkItemTrackingApiController
  {
    [HttpPut]
    public Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control SetControlInGroup(
      Guid processId,
      string witRefName,
      string groupId,
      string controlId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control control,
      string removeFromGroupId = null)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrEmpty(controlId, nameof (controlId));
      control.Id = controlId;
      IFormLayoutService service = this.TfsRequestContext.GetService<IFormLayoutService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control serverModel = control.GetServerModel();
      return (string.IsNullOrEmpty(removeFromGroupId) ? (LayoutNodeContainer<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page>) service.SetFieldControlInGroup(this.TfsRequestContext, processId, witRefName, groupId, serverModel, control.Order) : (LayoutNodeContainer<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page>) service.MoveFieldControlToGroup(this.TfsRequestContext, processId, witRefName, removeFromGroupId, groupId, serverModel, control.Order)).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(groupId).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control>(serverModel.Id).GetWebApiModel();
    }

    [HttpPost]
    public Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control AddControlToGroup(
      Guid processId,
      string witRefName,
      string groupId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control control)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      IFormLayoutService service = this.TfsRequestContext.GetService<IFormLayoutService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control serverModel = control.GetServerModel();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid processId1 = processId;
      string witRefName1 = witRefName;
      string groupId1 = groupId;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control control1 = serverModel;
      int? order = control.Order;
      return service.SetFieldControlInGroup(tfsRequestContext, processId1, witRefName1, groupId1, control1, order).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(groupId).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control>(serverModel.Id).GetWebApiModel();
    }

    [HttpPatch]
    public Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control EditControl(
      Guid processId,
      string witRefName,
      string groupId,
      string controlId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control control)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrEmpty(controlId, nameof (controlId));
      control.Id = controlId;
      IFormLayoutService service = this.TfsRequestContext.GetService<IFormLayoutService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control serverModel = control.GetServerModel();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid processId1 = processId;
      string witRefName1 = witRefName;
      string groupId1 = groupId;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control control1 = serverModel;
      int? order = control.Order;
      return service.SetFieldControlInGroup(tfsRequestContext, processId1, witRefName1, groupId1, control1, order, true).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(groupId).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control>(controlId).GetWebApiModel();
    }

    [HttpDelete]
    public void RemoveControlFromGroup(
      Guid processId,
      string witRefName,
      string groupId,
      string controlId)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrEmpty(controlId, nameof (controlId));
      Layout layout = this.TfsRequestContext.GetService<IFormLayoutService>().RemoveFieldControlFromGroup(this.TfsRequestContext, processId, witRefName, groupId, controlId);
      this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().UpdateCustomForm(this.TfsRequestContext, processId, witRefName, layout);
    }
  }
}
