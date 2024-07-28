// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.TeamIterationsUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  internal static class TeamIterationsUtils
  {
    internal static SprintDatesOptionsViewModel CreateSprintDatesOptions(
      SprintInformation sprint,
      IVssRequestContext requestContext,
      Guid teamId)
    {
      DateTime date = TimeZoneInfo.ConvertTime(DateTime.Now, requestContext.GetTimeZone()).Date;
      bool flag = false;
      try
      {
        if (sprint.StartDate.HasValue)
        {
          if (sprint.FinishDate.HasValue)
          {
            DateTime dateTime1 = date;
            DateTime? startDate = sprint.StartDate;
            int num;
            if ((startDate.HasValue ? (dateTime1 >= startDate.GetValueOrDefault() ? 1 : 0) : 0) != 0)
            {
              DateTime dateTime2 = date;
              DateTime? finishDate = sprint.FinishDate;
              num = finishDate.HasValue ? (dateTime2 <= finishDate.GetValueOrDefault() ? 1 : 0) : 0;
            }
            else
              num = 0;
            flag = num != 0;
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(599999, "Agile", TfsTraceLayers.Controller, ex);
      }
      return new SprintDatesOptionsViewModel()
      {
        TeamId = teamId.ToString(),
        Name = sprint.Name,
        IterationPath = sprint.IterationPath,
        IterationId = sprint.IterationId.ToString("D"),
        StartDate = sprint.StartDate,
        FinishDate = sprint.FinishDate,
        AccountCurrentDate = AgileUtils.GetTeamCapacityCollectionCurrentDate(requestContext),
        IsCurrentDateInIteration = flag
      };
    }
  }
}
