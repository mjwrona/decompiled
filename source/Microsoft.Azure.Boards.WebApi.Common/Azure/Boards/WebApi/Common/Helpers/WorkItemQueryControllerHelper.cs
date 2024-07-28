// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Helpers.WorkItemQueryControllerHelper
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Helpers
{
  public static class WorkItemQueryControllerHelper
  {
    public static IEnumerable<QueryHierarchyItem> GetQueries(
      IVssRequestContext requestContext,
      Guid projectId,
      QueryExpand expand = QueryExpand.None,
      int depth = 0,
      bool includeDeleted = false,
      bool excludeUrls = false)
    {
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      int maxGetQueriesDepth = witRequestContext.ServerSettings.MaxGetQueriesDepth;
      if (depth < 0 || depth > maxGetQueriesDepth)
        throw new VssPropertyValidationException(nameof (depth), ResourceStrings.QueryParameterOutOfRangeWithRangeValues((object) nameof (depth), (object) 0, (object) maxGetQueriesDepth));
      WorkItemQueryControllerHelper.CheckForValidProject(requestContext, projectId);
      QueryResponseOptions options = QueryResponseOptions.Create(expand, excludeUrls);
      ITeamFoundationQueryItemService service = requestContext.GetService<ITeamFoundationQueryItemService>();
      List<QueryItem> list = service.GetRootQueries(requestContext, projectId, new int?(depth), options.IncludeWiql, includeDeleted, true).ToList<QueryItem>();
      if (WorkItemTrackingFeatureFlags.IsVisualStudio(requestContext))
        service.StripOutCurrentIterationTeamParameter(requestContext, (IEnumerable<QueryItem>) list);
      return (IEnumerable<QueryHierarchyItem>) QueryHierarchyItemFactory.Create(witRequestContext, (IList<QueryItem>) list, false, options);
    }

    public static void CheckForValidProject(
      IVssRequestContext requestContext,
      Guid projectId,
      string queryReference = null)
    {
      if ((string.IsNullOrEmpty(queryReference) || !Guid.TryParse(queryReference, out Guid _)) && projectId == Guid.Empty)
        throw new ArgumentException(ResourceStrings.ProjectNotFound()).Expected(requestContext.ServiceName);
    }
  }
}
