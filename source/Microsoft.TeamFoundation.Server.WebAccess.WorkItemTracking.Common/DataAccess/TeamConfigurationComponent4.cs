// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamConfigurationComponent4
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamConfigurationComponent4 : TeamConfigurationComponent2
  {
    internal override TeamConfiguration GetTeamConfiguration(Guid projectId, Guid teamId)
    {
      this.PrepareStoredProcedure("prc_GetTeamConfiguration");
      this.BindDataspaceId(projectId);
      this.BindGuid("@teamId", teamId);
      TeamConfiguration settings = new TeamConfiguration();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFieldRow>((ObjectBinder<TeamFieldRow>) new TeamFieldRowBinder());
        resultCollection.AddBinder<TeamIterationRow>((ObjectBinder<TeamIterationRow>) new IterationRowBinder());
        resultCollection.AddBinder<TeamConfigurationPropertyRow>((ObjectBinder<TeamConfigurationPropertyRow>) new TeamConfigurationPropertyRowBinder());
        resultCollection.AddBinder<TeamConfigurationWeekendRow>((ObjectBinder<TeamConfigurationWeekendRow>) new TeamConfigurationWeekendRowBinder());
        TeamConfigurationComponent.FillTeamConfigurationTeamFields(settings, (IEnumerable<TeamFieldRow>) resultCollection.GetCurrent<TeamFieldRow>().Items);
        resultCollection.NextResult();
        TeamConfigurationComponent.FillTeamConfigurationIterations(settings, (IEnumerable<TeamIterationRow>) resultCollection.GetCurrent<TeamIterationRow>().Items);
        resultCollection.NextResult();
        TeamConfigurationComponent.FillTeamConfigurationProperties(settings, (IEnumerable<TeamConfigurationPropertyRow>) resultCollection.GetCurrent<TeamConfigurationPropertyRow>().Items);
        resultCollection.NextResult();
        settings.Weekends.Days = resultCollection.GetCurrent<TeamConfigurationWeekendRow>().Items.Select<TeamConfigurationWeekendRow, DayOfWeek>((System.Func<TeamConfigurationWeekendRow, DayOfWeek>) (i => i.DayOfWeek)).ToArray<DayOfWeek>();
      }
      return settings;
    }

    internal override void SaveTeamWeekends(Guid projectId, Guid teamId, ITeamWeekends weekends)
    {
      this.PrepareStoredProcedure("prc_SetTeamConfigurationWeekends");
      this.BindDataspaceId(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindInt32Table("@weekendTable", ((IEnumerable<DayOfWeek>) weekends.Days).Select<DayOfWeek, int>((System.Func<DayOfWeek, int>) (weekEnum => (int) weekEnum)));
      this.ExecuteNonQuery();
    }
  }
}
