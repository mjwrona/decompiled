// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.FormLayoutGroupsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

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
  [VersionedApiControllerCustomName(Area = "processDefinitions", ResourceName = "Groups", ResourceVersion = 1)]
  public class FormLayoutGroupsController : WorkItemTrackingApiController
  {
    [HttpPost]
    public Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group AddGroup(
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group group)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sectionId, nameof (sectionId));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group>(group, nameof (group));
      ArgumentUtility.CheckForNull<string>(group.Label, "Label");
      if (group.Controls != null && group.Controls.Count > 0)
      {
        foreach (Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control control in (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control>) group.Controls)
        {
          if (string.IsNullOrWhiteSpace(control.Id))
            control.Id = Guid.NewGuid().ToString();
        }
      }
      IFormLayoutService service = this.TfsRequestContext.GetService<IFormLayoutService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group serverModel = group.GetServerModel();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid processId1 = processId;
      string witRefName1 = witRefName;
      string pageId1 = pageId;
      string sectionId1 = sectionId;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group group1 = serverModel;
      int? order = group.Order;
      return service.AddGroup(tfsRequestContext, processId1, witRefName1, pageId1, sectionId1, group1, order).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(serverModel.Id).GetWebApiModel();
    }

    [HttpPatch]
    public Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group EditGroup(
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group group)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sectionId, nameof (sectionId));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group>(group, nameof (group));
      ArgumentUtility.CheckStringForNullOrEmpty(group.Label, "Label");
      ArgumentUtility.EnsureIsNull((object) group.Controls, "Controls");
      return this.TfsRequestContext.GetService<IFormLayoutService>().EditGroup(this.TfsRequestContext, processId, witRefName, pageId, sectionId, group.GetServerModel(), group.Order).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(groupId).GetWebApiModel();
    }

    [HttpPut]
    public Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group SetGroupInSection(
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group group,
      string removeFromSectionId)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sectionId, nameof (sectionId));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group>(group, nameof (group));
      ArgumentUtility.CheckStringForNullOrEmpty(group.Label, "Label");
      ArgumentUtility.CheckStringForNullOrEmpty(removeFromSectionId, nameof (removeFromSectionId));
      ArgumentUtility.EnsureIsNull((object) group.Controls, "Controls");
      return this.TfsRequestContext.GetService<IFormLayoutService>().SetGroupInSection(this.TfsRequestContext, processId, witRefName, pageId, removeFromSectionId, sectionId, group.GetServerModel(), group.Order).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(groupId).GetWebApiModel();
    }

    [HttpPut]
    public Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group SetGroupInPage(
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group group,
      string removeFromPageId,
      string removeFromSectionId)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      ArgumentUtility.CheckStringForNullOrEmpty(sectionId, nameof (sectionId));
      ArgumentUtility.CheckStringForNullOrEmpty(groupId, nameof (groupId));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group>(group, nameof (group));
      ArgumentUtility.CheckStringForNullOrEmpty(group.Label, "Label");
      ArgumentUtility.CheckStringForNullOrEmpty(removeFromPageId, nameof (removeFromPageId));
      ArgumentUtility.CheckStringForNullOrEmpty(removeFromSectionId, nameof (removeFromSectionId));
      ArgumentUtility.EnsureIsNull((object) group.Controls, "Controls");
      return this.TfsRequestContext.GetService<IFormLayoutService>().SetGroupInPage(this.TfsRequestContext, processId, witRefName, removeFromPageId, pageId, removeFromSectionId, sectionId, group.GetServerModel(), group.Order).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group>(groupId).GetWebApiModel();
    }

    [HttpDelete]
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
  }
}
