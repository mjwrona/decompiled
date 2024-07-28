// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.WITHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class WITHelper
  {
    public static string GetWorkItemUrlById(
      IVssRequestContext requestContext,
      int id,
      ILocationService locationService)
    {
      string workItemUrlById = string.Empty;
      if (locationService != null)
        workItemUrlById = locationService.GetResourceUri(requestContext, "wit", WitConstants.WorkItemTrackingLocationIds.WorkItems, (object) new
        {
          id = id
        }).AbsoluteUri;
      return workItemUrlById;
    }

    public static int GetMaxItemsBetweenBuildsForWorkItems(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/MaxItemsBetweenBuildsForWorkItems", true, 1000);
  }
}
