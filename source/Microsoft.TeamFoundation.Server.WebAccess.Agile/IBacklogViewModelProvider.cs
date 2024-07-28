// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.IBacklogViewModelProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public interface IBacklogViewModelProvider
  {
    BacklogViewModel GetBacklogViewModel(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      WebApiTeam team,
      BacklogContext backlogContext,
      IEnumerable<string> additionalFields = null,
      bool? forecastVisibleOverride = null,
      bool? showInProgress = null,
      bool? showCompletedChildItems = null,
      IAgileSettings agileSettings = null,
      SortedIterationSubscriptions sortedTeamIterationNodes = null,
      IDictionary<string, bool> teamAutomationRulesStates = null);

    EffortDataViewModel GetEffortDataViewModel(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      IEnumerable<int> workItemIds);
  }
}
