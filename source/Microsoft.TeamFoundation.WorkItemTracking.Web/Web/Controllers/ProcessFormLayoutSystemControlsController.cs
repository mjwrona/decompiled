// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessFormLayoutSystemControlsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Extensions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "SystemControls", ResourceVersion = 1)]
  public class ProcessFormLayoutSystemControlsController : WorkItemTrackingApiController
  {
    [HttpGet]
    [ClientExample("GET__control.json", "GET System Controls", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control[] GetSystemControls(
      Guid processId,
      string witRefName)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      return this.TfsRequestContext.GetService<IFormLayoutService>().GetLayout(this.TfsRequestContext, processId, witRefName).DeltaLayout.SystemControls.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>) (ctrl => ctrl.GetProcessWebApiModel())).ToArray<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>();
    }

    [HttpPatch]
    [ClientExample("PATCH__control.json", "Update/Add a System Control", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control UpdateSystemControl(
      Guid processId,
      string witRefName,
      string controlId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control control)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(controlId, nameof (controlId));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>(control, nameof (control));
      control.Id = controlId;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control serverModel = control.GetServerModel();
      Layout var = this.TfsRequestContext.GetService<IFormLayoutService>().AddorEditSystemControls(this.TfsRequestContext, processId, witRefName, (ISet<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control>) new HashSet<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control>()
      {
        serverModel
      });
      ArgumentUtility.CheckForNull<Layout>(var, "layout");
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("SystemControl", "Update");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("ControlId", (object) controlId);
      data.Add("ControlLabel", (object) serverModel.Label);
      data.Add("ControlVisibility", (object) serverModel.Visible);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control actualValue;
      var.SystemControls.TryGetValue(serverModel, out actualValue);
      return actualValue.GetProcessWebApiModel();
    }

    [HttpDelete]
    [ClientExample("DELETE__control.json", "DELETE System Control", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control[] DeleteSystemControl(
      Guid processId,
      string witRefName,
      string controlId)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(controlId, nameof (controlId));
      Layout var = this.TfsRequestContext.GetService<IFormLayoutService>().RemoveSystemControls(this.TfsRequestContext, processId, witRefName, (ISet<string>) new HashSet<string>()
      {
        controlId
      });
      ArgumentUtility.CheckForNull<Layout>(var, "layout");
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("SystemControl", "Delete");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("ControlId", (object) controlId);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return var.SystemControls.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>) (ctrl => ctrl.GetProcessWebApiModel())).ToArray<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>();
    }
  }
}
