// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamConfigurationComponent9
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamConfigurationComponent9 : TeamConfigurationComponent8
  {
    internal override void SetCFDProperties(
      Guid projectId,
      Guid teamId,
      IDictionary<string, CumulativeFlowDiagramSettings> cfdSettings)
    {
      MemoryStream memoryStream = new MemoryStream();
      new DataContractSerializer(typeof (Dictionary<string, CumulativeFlowDiagramSettings>)).WriteObject((Stream) memoryStream, (object) cfdSettings);
      memoryStream.Close();
      string str = Encoding.UTF8.GetString(memoryStream.ToArray());
      this.SetTeamConfigurationProperties(projectId, teamId, (IEnumerable<KeyValuePair<string, string>>) new Dictionary<string, string>()
      {
        {
          "CumulativeFlowDiagramSettings",
          str
        }
      });
    }

    internal override IList<ITeamSettings> GetTeamConfigurations(
      IList<Tuple<Guid, Guid>> teamAndProjectIds)
    {
      this.PrepareStoredProcedure("prc_GetTeamConfigurations");
      IEnumerable<Tuple<Guid, int>> tuples = teamAndProjectIds.Select<Tuple<Guid, Guid>, Tuple<Guid, int>>((System.Func<Tuple<Guid, Guid>, Tuple<Guid, int>>) (tuple => Tuple.Create<Guid, int>(tuple.Item1, this.GetDataspaceId(tuple.Item2))));
      this.BindGuidInt32Table("@teamDataspaceIdTable", tuples);
      IDictionary<Tuple<Guid, int>, TeamConfiguration> settings = (IDictionary<Tuple<Guid, int>, TeamConfiguration>) tuples.ToDictionary<Tuple<Guid, int>, Tuple<Guid, int>, TeamConfiguration>((System.Func<Tuple<Guid, int>, Tuple<Guid, int>>) (id => id), (System.Func<Tuple<Guid, int>, TeamConfiguration>) (id => new TeamConfiguration()));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFieldRow>((ObjectBinder<TeamFieldRow>) new FullTeamFieldRowBinder());
        resultCollection.AddBinder<TeamIterationRow>((ObjectBinder<TeamIterationRow>) new FullIterationRowBinder());
        resultCollection.AddBinder<TeamConfigurationPropertyRow>((ObjectBinder<TeamConfigurationPropertyRow>) new FullTeamConfigurationPropertyRowBinder());
        resultCollection.AddBinder<TeamConfigurationWeekendRow>((ObjectBinder<TeamConfigurationWeekendRow>) new FullTeamConfigurationWeekendRowBinder());
        TeamConfigurationComponent9.FillTeamConfigurationTeamFields(settings, (IEnumerable<TeamFieldRow>) resultCollection.GetCurrent<TeamFieldRow>().Items);
        resultCollection.NextResult();
        TeamConfigurationComponent9.FillTeamConfigurationIterations(settings, (IEnumerable<TeamIterationRow>) resultCollection.GetCurrent<TeamIterationRow>().Items);
        resultCollection.NextResult();
        TeamConfigurationComponent9.FillTeamConfigurationProperties(settings, (IEnumerable<TeamConfigurationPropertyRow>) resultCollection.GetCurrent<TeamConfigurationPropertyRow>().Items);
        resultCollection.NextResult();
        TeamConfigurationComponent9.FillTeamConfigurationWeekends(settings, (IEnumerable<TeamConfigurationWeekendRow>) resultCollection.GetCurrent<TeamConfigurationWeekendRow>().Items);
      }
      return (IList<ITeamSettings>) ((IEnumerable<ITeamSettings>) tuples.Select<Tuple<Guid, int>, TeamConfiguration>((System.Func<Tuple<Guid, int>, TeamConfiguration>) (key => settings[key]))).AsEnumerable<ITeamSettings>().ToList<ITeamSettings>();
    }

    protected static void FillTeamConfigurationTeamFields(
      IDictionary<Tuple<Guid, int>, TeamConfiguration> settings,
      IEnumerable<TeamFieldRow> rows)
    {
      foreach (IGrouping<Tuple<Guid, int>, TeamFieldRow> rows1 in rows.GroupBy<TeamFieldRow, Tuple<Guid, int>>((System.Func<TeamFieldRow, Tuple<Guid, int>>) (row => Tuple.Create<Guid, int>(row.TeamId, row.DataspaceId))).Select<IGrouping<Tuple<Guid, int>, TeamFieldRow>, IGrouping<Tuple<Guid, int>, TeamFieldRow>>((System.Func<IGrouping<Tuple<Guid, int>, TeamFieldRow>, IGrouping<Tuple<Guid, int>, TeamFieldRow>>) (group => group)))
      {
        TeamConfiguration settings1;
        if (settings.TryGetValue(rows1.Key, out settings1))
          TeamConfigurationComponent.FillTeamConfigurationTeamFields(settings1, (IEnumerable<TeamFieldRow>) rows1);
      }
    }

    protected static void FillTeamConfigurationIterations(
      IDictionary<Tuple<Guid, int>, TeamConfiguration> settings,
      IEnumerable<TeamIterationRow> rows)
    {
      foreach (IGrouping<Tuple<Guid, int>, TeamIterationRow> rows1 in rows.GroupBy<TeamIterationRow, Tuple<Guid, int>>((System.Func<TeamIterationRow, Tuple<Guid, int>>) (row => Tuple.Create<Guid, int>(row.TeamId, row.DataspaceId))).Select<IGrouping<Tuple<Guid, int>, TeamIterationRow>, IGrouping<Tuple<Guid, int>, TeamIterationRow>>((System.Func<IGrouping<Tuple<Guid, int>, TeamIterationRow>, IGrouping<Tuple<Guid, int>, TeamIterationRow>>) (group => group)))
      {
        TeamConfiguration settings1;
        if (settings.TryGetValue(rows1.Key, out settings1))
          TeamConfigurationComponent.FillTeamConfigurationIterations(settings1, (IEnumerable<TeamIterationRow>) rows1);
      }
    }

    protected static void FillTeamConfigurationProperties(
      IDictionary<Tuple<Guid, int>, TeamConfiguration> settings,
      IEnumerable<TeamConfigurationPropertyRow> rows)
    {
      foreach (IGrouping<Tuple<Guid, int>, TeamConfigurationPropertyRow> rows1 in rows.GroupBy<TeamConfigurationPropertyRow, Tuple<Guid, int>>((System.Func<TeamConfigurationPropertyRow, Tuple<Guid, int>>) (row => Tuple.Create<Guid, int>(row.TeamId, row.DataspaceId))).Select<IGrouping<Tuple<Guid, int>, TeamConfigurationPropertyRow>, IGrouping<Tuple<Guid, int>, TeamConfigurationPropertyRow>>((System.Func<IGrouping<Tuple<Guid, int>, TeamConfigurationPropertyRow>, IGrouping<Tuple<Guid, int>, TeamConfigurationPropertyRow>>) (group => group)))
      {
        TeamConfiguration settings1;
        if (settings.TryGetValue(rows1.Key, out settings1))
          TeamConfigurationComponent.FillTeamConfigurationProperties(settings1, (IEnumerable<TeamConfigurationPropertyRow>) rows1);
      }
    }

    protected static void FillTeamConfigurationWeekends(
      IDictionary<Tuple<Guid, int>, TeamConfiguration> settings,
      IEnumerable<TeamConfigurationWeekendRow> rows)
    {
      foreach (IGrouping<Tuple<Guid, int>, TeamConfigurationWeekendRow> source in rows.GroupBy<TeamConfigurationWeekendRow, Tuple<Guid, int>>((System.Func<TeamConfigurationWeekendRow, Tuple<Guid, int>>) (row => Tuple.Create<Guid, int>(row.TeamId, row.DataspaceId))).Select<IGrouping<Tuple<Guid, int>, TeamConfigurationWeekendRow>, IGrouping<Tuple<Guid, int>, TeamConfigurationWeekendRow>>((System.Func<IGrouping<Tuple<Guid, int>, TeamConfigurationWeekendRow>, IGrouping<Tuple<Guid, int>, TeamConfigurationWeekendRow>>) (group => group)))
      {
        TeamConfiguration teamConfiguration;
        if (settings.TryGetValue(source.Key, out teamConfiguration))
          teamConfiguration.Weekends.Days = source.Select<TeamConfigurationWeekendRow, DayOfWeek>((System.Func<TeamConfigurationWeekendRow, DayOfWeek>) (row => row.DayOfWeek)).ToArray<DayOfWeek>();
      }
    }
  }
}
