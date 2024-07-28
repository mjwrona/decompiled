// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessFormLayoutController
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
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "layout", ResourceVersion = 1)]
  [ControllerApiVersion(5.0)]
  public class ProcessFormLayoutController : WorkItemTrackingApiController
  {
    [HttpGet]
    [ClientExample("GET__layout.json", "Get the form layout", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.FormLayout GetFormLayout(
      Guid processId,
      string witRefName)
    {
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witRefName, nameof (witRefName));
      LayoutInfo layout = this.TfsRequestContext.GetService<IFormLayoutService>().GetLayout(this.TfsRequestContext, processId, witRefName);
      ArgumentUtility.CheckForNull<LayoutInfo>(layout, "layoutInfo");
      return layout.ComposedLayout.GetProcessWebApiModel();
    }
  }
}
