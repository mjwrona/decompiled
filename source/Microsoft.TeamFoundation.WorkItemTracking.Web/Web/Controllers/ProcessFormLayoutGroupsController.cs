// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessFormLayoutGroupsController
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
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "Groups", ResourceVersion = 1)]
  [ControllerApiVersion(5.0)]
  public class ProcessFormLayoutGroupsController : WorkItemTrackingApiController
  {
    [HttpPost]
    [ClientExample("POST__group.json", "Add a group", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group AddGroup(
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group group)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sectionId, nameof (sectionId));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group>(group, nameof (group));
      ArgumentUtility.CheckForNull<string>(group.Label, "Label");
      if (group.Controls != null && group.Controls.Count > 0)
      {
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control control in (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>) group.Controls)
        {
          if (string.IsNullOrWhiteSpace(control.Id))
            control.Id = Guid.NewGuid().ToString();
        }
      }
      IFormLayoutService service = this.TfsRequestContext.GetService<IFormLayoutService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group serverModel = group.GetServerModel();
      IVssRequestContext tfsRequestContext1 = this.TfsRequestContext;
      Guid processId1 = processId;
      string witRefName1 = witRefName;
      string pageId1 = pageId;
      string sectionId1 = sectionId;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group group1 = serverModel;
      int? order = group.Order;
      Layout var = service.AddGroup(tfsRequestContext1, processId1, witRefName1, pageId1, sectionId1, group1, order);
      ArgumentUtility.CheckForNull<Layout>(var, "layout");
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext2 = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Group", "Add");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("GroupLabel", (object) group.Label);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext2.LogAuditEvent(actionId, data, targetHostId, projectId);
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group descendant = var.FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(serverModel.Id);
      return descendant == null ? (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group) null : descendant.GetProcessWebApiModel();
    }

    [HttpPatch]
    [ClientExample("PATCH__group.json", "Update group", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group UpdateGroup(
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group group)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sectionId, nameof (sectionId));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group>(group, nameof (group));
      ArgumentUtility.CheckStringForNullOrEmpty(group.Label, "Label");
      this.EnsureValidGroupIdMatch(groupId, group);
      ArgumentUtility.EnsureIsNull((object) group.Controls, "Controls");
      Layout var = this.TfsRequestContext.GetService<IFormLayoutService>().EditGroup(this.TfsRequestContext, processId, witRefName, pageId, sectionId, group.GetServerModel(), group.Order);
      ArgumentUtility.CheckForNull<Layout>(var, "layout");
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Group", "Update");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("GroupLabel", (object) group.Label);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group descendant = var.FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(groupId);
      return descendant == null ? (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group) null : descendant.GetProcessWebApiModel();
    }

    [HttpPut]
    [ClientExample("PUT__group_section.json", "Move a group to a different section", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group MoveGroupToSection(
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group group,
      string removeFromSectionId)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sectionId, nameof (sectionId));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group>(group, nameof (group));
      ArgumentUtility.CheckStringForNullOrEmpty(group.Label, "Label");
      ArgumentUtility.CheckStringForNullOrEmpty(removeFromSectionId, nameof (removeFromSectionId));
      this.EnsureValidGroupIdMatch(groupId, group);
      ArgumentUtility.EnsureIsNull((object) group.Controls, "Controls");
      Layout var = this.TfsRequestContext.GetService<IFormLayoutService>().SetGroupInSection(this.TfsRequestContext, processId, witRefName, pageId, removeFromSectionId, sectionId, group.GetServerModel(), group.Order);
      ArgumentUtility.CheckForNull<Layout>(var, "layout");
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group descendant = var.FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(groupId);
      return descendant == null ? (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group) null : descendant.GetProcessWebApiModel();
    }

    [HttpPut]
    [ClientExample("PUT__group_page.json", "Moves a group to a different page and section", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group MoveGroupToPage(
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group group,
      string removeFromPageId,
      string removeFromSectionId)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sectionId, nameof (sectionId));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group>(group, nameof (group));
      ArgumentUtility.CheckStringForNullOrEmpty(group.Label, "Label");
      ArgumentUtility.CheckStringForNullOrEmpty(removeFromPageId, nameof (removeFromPageId));
      ArgumentUtility.CheckStringForNullOrEmpty(removeFromSectionId, nameof (removeFromSectionId));
      this.EnsureValidGroupIdMatch(groupId, group);
      ArgumentUtility.EnsureIsNull((object) group.Controls, "Controls");
      Layout var = this.TfsRequestContext.GetService<IFormLayoutService>().SetGroupInPage(this.TfsRequestContext, processId, witRefName, removeFromPageId, pageId, removeFromSectionId, sectionId, group.GetServerModel(), group.Order);
      ArgumentUtility.CheckForNull<Layout>(var, "layout");
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group descendant = var.FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(groupId);
      return descendant == null ? (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group) null : descendant.GetProcessWebApiModel();
    }

    [HttpDelete]
    [ClientExample("DELETE__group.json", "Removes a group from the work item form", null, null)]
    public void RemoveGroup(
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sectionId, nameof (sectionId));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      this.TfsRequestContext.GetService<IFormLayoutService>().RemoveGroup(this.TfsRequestContext, processId, witRefName, pageId, sectionId, groupId);
    }

    private void EnsureValidGroupIdMatch(string groupId, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group group)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group group1 = group;
      if (group1.Id == null)
      {
        string str;
        group1.Id = str = groupId;
      }
      if (group.Id != groupId)
        throw new VssPropertyValidationException("Id", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.IdMismatch((object) groupId, (object) group.Id));
    }
  }
}
