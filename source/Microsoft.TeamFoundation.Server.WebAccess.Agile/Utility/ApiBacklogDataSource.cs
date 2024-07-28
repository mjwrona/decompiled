// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.ApiBacklogDataSource
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  public static class ApiBacklogDataSource
  {
    public static JsObject GetEffortDataViewModel(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      int[] workItemIds)
    {
      using (requestContext.TraceBlock(290500, 290501, "Agile", nameof (ApiBacklogDataSource), nameof (GetEffortDataViewModel)))
        return new BacklogViewModelProvider().GetEffortDataViewModel(requestContext, settings, (IEnumerable<int>) (workItemIds ?? Array.Empty<int>())).ToJson();
    }

    public static JsObject PageBoardWorkItems(
      IVssRequestContext TfsRequestContext,
      IAgileSettings settings,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      WebApiTeam team,
      string backlogLevelId,
      int[] workItemIds)
    {
      using (TfsRequestContext.TraceBlock(15283039, 15283040, "Agile", nameof (ApiBacklogDataSource), nameof (PageBoardWorkItems)))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(backlogLevelId, nameof (backlogLevelId));
        ArgumentUtility.CheckForNull<int[]>(workItemIds, nameof (workItemIds));
        if (workItemIds.Length > BoardFactory.GetMaxPageSize(TfsRequestContext))
          throw new BoardPageWorkItemsLimitException(workItemIds.Length);
        BacklogContext backlogContext = ApiBacklogControllerHelper.GetBacklogContext(settings, backlogLevelId);
        HierarchyDataReader boardWorkItems = BoardFactory.GetBoardWorkItems(TfsRequestContext, project, team, backlogContext, settings, (IEnumerable<int>) workItemIds);
        JsObject jsObject = new JsObject();
        jsObject.Add("payload", (object) boardWorkItems.ToJson(false, false));
        return jsObject;
      }
    }
  }
}
