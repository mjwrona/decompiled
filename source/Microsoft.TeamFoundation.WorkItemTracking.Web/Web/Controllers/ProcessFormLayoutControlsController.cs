// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessFormLayoutControlsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Extensions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "Controls", ResourceVersion = 1)]
  [ControllerApiVersion(5.0)]
  public class ProcessFormLayoutControlsController : WorkItemTrackingApiController
  {
    [HttpPut]
    [ClientExample("PUT__control.json", "Move a control to a new group", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control MoveControlToGroup(
      Guid processId,
      string witRefName,
      string groupId,
      string controlId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control control,
      string removeFromGroupId = null)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrEmpty(controlId, nameof (controlId));
      control.Id = controlId;
      IFormLayoutService service = this.TfsRequestContext.GetService<IFormLayoutService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control serverModel = control.GetServerModel();
      Layout var = string.IsNullOrEmpty(removeFromGroupId) ? service.SetFieldControlInGroup(this.TfsRequestContext, processId, witRefName, groupId, serverModel, control.Order) : service.MoveFieldControlToGroup(this.TfsRequestContext, processId, witRefName, removeFromGroupId, groupId, serverModel, control.Order);
      ArgumentUtility.CheckForNull<Layout>(var, "layout");
      return var.FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(groupId).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control>(serverModel.Id).GetProcessWebApiModel();
    }

    [HttpPost]
    [ClientExample("POST__control.json", "Create a control in a group", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control CreateControlInGroup(
      Guid processId,
      string witRefName,
      string groupId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control control)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>(control, nameof (control));
      IFormLayoutService service = this.TfsRequestContext.GetService<IFormLayoutService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control serverModel = control.GetServerModel();
      IVssRequestContext tfsRequestContext1 = this.TfsRequestContext;
      Guid processId1 = processId;
      string witRefName1 = witRefName;
      string groupId1 = groupId;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control control1 = serverModel;
      int? order = control.Order;
      Layout var = service.SetFieldControlInGroup(tfsRequestContext1, processId1, witRefName1, groupId1, control1, order);
      ArgumentUtility.CheckForNull<Layout>(var, "layout");
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      if (control.Label.IsNullOrEmpty<char>())
      {
        IVssRequestContext tfsRequestContext2 = this.TfsRequestContext;
        string actionId = ProcessAuditConstants.GetActionId("Control", "CreateWithoutLabel");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("ProcessName", (object) processDescriptor?.Name);
        data.Add("WorkItemTypeReferenceName", (object) witRefName);
        data.Add("ControlName", (object) control.Name);
        data.Add("ControlType", (object) control.ControlType);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        tfsRequestContext2.LogAuditEvent(actionId, data, targetHostId, projectId);
      }
      else
      {
        IVssRequestContext tfsRequestContext3 = this.TfsRequestContext;
        string actionId = ProcessAuditConstants.GetActionId("Control", "Create");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("ProcessName", (object) processDescriptor?.Name);
        data.Add("WorkItemTypeReferenceName", (object) witRefName);
        data.Add("ControlLabel", (object) control.Label);
        data.Add("ControlName", (object) control.Name);
        data.Add("ControlType", (object) control.ControlType);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        tfsRequestContext3.LogAuditEvent(actionId, data, targetHostId, projectId);
      }
      return var.FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(groupId).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control>(serverModel.Id).GetProcessWebApiModel();
    }

    [HttpPatch]
    [ClientExample("PATCH__control.json", "Update a control on the work item form", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control UpdateControl(
      Guid processId,
      string witRefName,
      string groupId,
      string controlId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control control)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrEmpty(controlId, nameof (controlId));
      if (control == null)
        control = new Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control();
      control.Id = controlId;
      IFormLayoutService service = this.TfsRequestContext.GetService<IFormLayoutService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control serverModel = control.GetServerModel();
      IVssRequestContext tfsRequestContext1 = this.TfsRequestContext;
      Guid processId1 = processId;
      string witRefName1 = witRefName;
      string groupId1 = groupId;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control control1 = serverModel;
      int? order = control.Order;
      Layout var = service.SetFieldControlInGroup(tfsRequestContext1, processId1, witRefName1, groupId1, control1, order, true);
      ArgumentUtility.CheckForNull<Layout>(var, "layout");
      string empty = string.Empty;
      string enumerable = !string.IsNullOrEmpty(control.Label) ? control.Label : control.Name;
      if (string.IsNullOrEmpty(enumerable))
        enumerable = serverModel.Name ?? controlId;
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      if (enumerable.IsNullOrEmpty<char>())
      {
        IVssRequestContext tfsRequestContext2 = this.TfsRequestContext;
        string actionId = ProcessAuditConstants.GetActionId("Control", "UpdateWithoutLabel");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("ProcessName", (object) processDescriptor?.Name);
        data.Add("WorkItemTypeReferenceName", (object) witRefName);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        tfsRequestContext2.LogAuditEvent(actionId, data, targetHostId, projectId);
      }
      else
      {
        IVssRequestContext tfsRequestContext3 = this.TfsRequestContext;
        string actionId = ProcessAuditConstants.GetActionId("Control", "Update");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("ProcessName", (object) processDescriptor?.Name);
        data.Add("WorkItemTypeReferenceName", (object) witRefName);
        data.Add("ControlLabel", (object) enumerable);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        tfsRequestContext3.LogAuditEvent(actionId, data, targetHostId, projectId);
      }
      return var.FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(groupId).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control>(controlId).GetProcessWebApiModel();
    }

    [HttpDelete]
    [ClientExample("DELETE__control.json", "Remove a control from the work item form", null, null)]
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
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Control", "Delete");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
    }
  }
}
