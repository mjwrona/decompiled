// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions.WorkItemResourceLinkUpdateResultExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions
{
  public static class WorkItemResourceLinkUpdateResultExtensions
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdateResult ToWorkItemResourceLinkUpdateResult(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemResourceLinkUpdateResult result,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper)
    {
      Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdateResult linkUpdateResult = new Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdateResult();
      linkUpdateResult.ChangeBy = IdentityRefBuilder.CreateFromConstantId(tfsRequestContext, result.ChangeBy, true);
      linkUpdateResult.CorrelationId = result.CorrelationId;
      linkUpdateResult.ChangedDate = result.ChangedDate;
      linkUpdateResult.ResourceId = result.ResourceId;
      linkUpdateResult.Source = new WorkItemReference(result.SourceWorkItemId, tfsRequestContext, urlHelper);
      linkUpdateResult.UpdateType = result.UpdateType.ToLinkUpdateType();
      return linkUpdateResult;
    }
  }
}
