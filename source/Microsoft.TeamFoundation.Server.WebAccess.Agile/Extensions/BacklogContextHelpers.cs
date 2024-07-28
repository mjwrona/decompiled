// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Extensions.BacklogContextHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Extensions
{
  internal static class BacklogContextHelpers
  {
    public static BacklogContext GetBacklogContext(
      IVssRequestContext requestContext,
      BacklogConfiguration backlogConfiguration,
      Team team,
      string level,
      bool showInProgress,
      bool allowHidden = false)
    {
      BacklogContext backlogContext = BacklogContextHelpers.CreateBacklogContext(requestContext, team, level, backlogConfiguration, showInProgress);
      if (!allowHidden)
        backlogContext.CheckBacklogLevelVisible(backlogConfiguration);
      return backlogContext;
    }

    public static BacklogContext CreateBacklogContext(
      IVssRequestContext requestContext,
      Team team,
      string backlogName,
      BacklogConfiguration backlogConfiguration,
      bool showInProgress)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "BacklogsController.CreateBacklogContext"))
      {
        BacklogLevelConfiguration backlogLevel = (BacklogLevelConfiguration) null;
        if (!backlogConfiguration.TryGetBacklogByName(backlogName, out backlogLevel))
          throw new BacklogInvalidContextException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.TeamFoundation.Agile.Server.AgileResources.BacklogHubContextInvalid, (object) backlogName), backlogName);
        if (backlogLevel.IsTaskBacklog)
          throw new TaskBacklogInvalidContextException();
        return new BacklogContext(team, backlogLevel, BacklogContextHelpers.GetBacklogPortfolioNames(requestContext, backlogConfiguration))
        {
          ShowInProgress = showInProgress,
          IncludeParents = false
        };
      }
    }

    public static string[] GetBacklogPortfolioNames(
      IVssRequestContext requestContext,
      BacklogConfiguration backlogConfiguration)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "BacklogsController.GetBacklogPortfolioNames"))
        return requestContext.FeatureContext().GetFeatureMode(LicenseFeatures.PortfolioBacklogManagementId) == FeatureMode.Off ? new string[1]
        {
          backlogConfiguration.RequirementBacklog.Name
        } : backlogConfiguration.PortfolioBacklogs.Concat<BacklogLevelConfiguration>((IEnumerable<BacklogLevelConfiguration>) new BacklogLevelConfiguration[1]
        {
          backlogConfiguration.RequirementBacklog
        }).Select<BacklogLevelConfiguration, string>((Func<BacklogLevelConfiguration, string>) (p => p.Name)).ToArray<string>();
    }

    private static void CheckBacklogLevelVisible(
      this BacklogContext context,
      BacklogConfiguration backlogConfiguration)
    {
      if (context.CurrentLevelConfiguration == null)
        throw new BacklogInvalidContextException(Microsoft.TeamFoundation.Agile.Server.AgileResources.BacklogContextNotSet);
      if (!backlogConfiguration.IsBacklogVisible(context.CurrentLevelConfiguration.Id))
      {
        string name = context.CurrentLevelConfiguration.Name;
        throw new BacklogInvalidContextException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.TeamFoundation.Agile.Server.AgileResources.BacklogLevelInvisible, (object) name), name);
      }
    }
  }
}
