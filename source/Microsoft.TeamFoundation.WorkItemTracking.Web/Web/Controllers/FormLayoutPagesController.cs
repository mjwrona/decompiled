// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.FormLayoutPagesController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Extensions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processDefinitions", ResourceName = "Pages", ResourceVersion = 1)]
  public class FormLayoutPagesController : WorkItemTrackingApiController
  {
    [HttpPost]
    public Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page AddPage(
      Guid processId,
      string witRefName,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page page)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page>(page, nameof (page));
      ArgumentUtility.CheckForNull<string>(page.Label, "Label");
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page serverModel = page.GetServerModel();
      return this.TfsRequestContext.GetService<IFormLayoutService>().AddPage(this.TfsRequestContext, processId, witRefName, serverModel, page.Order).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page>(serverModel.Id).GetWebApiModel();
    }

    [HttpPatch]
    public Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page EditPage(
      Guid processId,
      string witRefName,
      [FromBody] Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page page)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page>(page, nameof (page));
      ArgumentUtility.CheckStringForNullOrEmpty(page.Label, "Label");
      ArgumentUtility.EnsureIsNull((object) page.Sections, "Sections");
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page serverModel = page.GetServerModel();
      return this.TfsRequestContext.GetService<IFormLayoutService>().EditPage(this.TfsRequestContext, processId, witRefName, serverModel, page.Order).FindDescendant<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page>(serverModel.Id).GetWebApiModel();
    }

    [HttpDelete]
    public void RemovePage(Guid processId, string witRefName, string pageId)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(pageId, nameof (pageId));
      this.TfsRequestContext.GetService<IFormLayoutService>().RemovePage(this.TfsRequestContext, processId, witRefName, pageId);
    }
  }
}
