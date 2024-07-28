// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamConfigurationComponent7
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamConfigurationComponent7 : TeamConfigurationComponent6
  {
    internal override void SaveIterationTeamDaysOff(
      Guid projectId,
      Guid teamId,
      Guid iterationId,
      IEnumerable<DateRange> teamDaysOff)
    {
      foreach (DateRange dateRange in teamDaysOff)
        dateRange.ValidateForSql();
      this.PrepareStoredProcedure("prc_SetTeamConfigurationIterationTeamDaysOff");
      this.BindDataspaceId(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindGuid("@iterationId", iterationId);
      this.BindTeamConfigurationCapacityDaysOffRangeTable("@dateRangeTable", (IEnumerable<TeamCapacityDaysOffRangeRow>) TeamConfigurationComponent.GetDateRangeRows(TeamConfigurationComponent.WholeTeamId, teamDaysOff));
      this.ExecuteNonQuery();
    }
  }
}
