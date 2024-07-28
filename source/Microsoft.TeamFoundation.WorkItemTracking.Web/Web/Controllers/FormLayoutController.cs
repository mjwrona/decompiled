// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.FormLayoutController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Extensions;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processDefinitions", ResourceName = "layout", ResourceVersion = 1)]
  public class FormLayoutController : WorkItemTrackingApiController
  {
    [HttpGet]
    public Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.FormLayout GetFormLayout(
      Guid processId,
      string witRefName)
    {
      return this.TfsRequestContext.GetService<IFormLayoutService>().GetLayout(this.TfsRequestContext, processId, witRefName).ComposedLayout.GetWebApiModel();
    }
  }
}
