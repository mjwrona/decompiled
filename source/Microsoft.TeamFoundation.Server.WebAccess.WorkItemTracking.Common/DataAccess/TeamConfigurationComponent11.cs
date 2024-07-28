// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamConfigurationComponent11
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
  internal class TeamConfigurationComponent11 : TeamConfigurationComponent10
  {
    internal override IEnumerable<Tuple<Guid, Guid>> GetTeamIterationsWithCapacitiesForCollection()
    {
      this.PrepareStoredProcedure("prc_GetAllTeamsAndIterationsPairs");
      IEnumerable<Tuple<Guid, Guid>> allTeamIdAndIterationIdTuples = (IEnumerable<Tuple<Guid, Guid>>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamIdIterationIdPairTypeRow>((ObjectBinder<TeamIdIterationIdPairTypeRow>) new TeamIdIterationIdPairTypeRowBinder());
        this.FillTeamIdAndIterationIdPair((IEnumerable<TeamIdIterationIdPairTypeRow>) resultCollection.GetCurrent<TeamIdIterationIdPairTypeRow>().Items, out allTeamIdAndIterationIdTuples);
      }
      return allTeamIdAndIterationIdTuples;
    }

    internal override IEnumerable<TeamCapacity> GetBulkCapacityData(
      IEnumerable<Tuple<Guid, Guid>> teamIdAndIterationIdPairs)
    {
      this.PrepareStoredProcedure("prc_GetBulkTeamConfigurationIterationCapacity");
      this.BindGuidGuidTable("@teamIdAndIterationIdPairs", teamIdAndIterationIdPairs);
      IDictionary<Tuple<Guid, Guid>, TeamCapacity> capacityCollectionMap = (IDictionary<Tuple<Guid, Guid>, TeamCapacity>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamCapacityRow2>((ObjectBinder<TeamCapacityRow2>) new TeamCapacityRowBinder2());
        resultCollection.AddBinder<TeamCapacityDaysOffRangeRow2>((ObjectBinder<TeamCapacityDaysOffRangeRow2>) new TeamCapacityDaysOffRangeRowBinder2());
        TeamConfigurationComponent11.FillMultipleTeamCapacity((IEnumerable<TeamCapacityRow2>) resultCollection.GetCurrent<TeamCapacityRow2>().Items, out capacityCollectionMap);
        resultCollection.NextResult();
        TeamConfigurationComponent11.FillMultipleTeamCapacityDaysOff((IEnumerable<TeamCapacityDaysOffRangeRow2>) resultCollection.GetCurrent<TeamCapacityDaysOffRangeRow2>().Items, capacityCollectionMap);
      }
      return this.GetOrderedTeamCapacity(teamIdAndIterationIdPairs, capacityCollectionMap);
    }

    internal static void FillMultipleTeamCapacity(
      IEnumerable<TeamCapacityRow2> rows,
      out IDictionary<Tuple<Guid, Guid>, TeamCapacity> capacityCollectionMap)
    {
      IEnumerable<IGrouping<\u003C\u003Ef__AnonymousType5<Guid, Guid>, TeamCapacityRow2>> groupings = rows.GroupBy(r => new
      {
        TeamId = r.TeamId,
        IterationId = r.IterationId
      });
      capacityCollectionMap = (IDictionary<Tuple<Guid, Guid>, TeamCapacity>) new Dictionary<Tuple<Guid, Guid>, TeamCapacity>();
      foreach (IGrouping<\u003C\u003Ef__AnonymousType5<Guid, Guid>, TeamCapacityRow2> rows1 in groupings)
      {
        TeamCapacity capacity = new TeamCapacity();
        TeamConfigurationComponent.FillTeamCapacity(capacity, (IEnumerable<TeamCapacityRow>) rows1);
        capacityCollectionMap.Add(new Tuple<Guid, Guid>(rows1.Key.TeamId, rows1.Key.IterationId), capacity);
      }
    }

    internal static void FillMultipleTeamCapacityDaysOff(
      IEnumerable<TeamCapacityDaysOffRangeRow2> rows,
      IDictionary<Tuple<Guid, Guid>, TeamCapacity> capacityCollectionMap)
    {
      foreach (IGrouping<\u003C\u003Ef__AnonymousType5<Guid, Guid>, TeamCapacityDaysOffRangeRow2> rows1 in rows.GroupBy(r => new
      {
        TeamId = r.TeamId,
        IterationId = r.IterationId
      }))
      {
        Tuple<Guid, Guid> key = new Tuple<Guid, Guid>(rows1.Key.TeamId, rows1.Key.IterationId);
        if (!capacityCollectionMap.ContainsKey(key))
          throw new KeyNotFoundException(string.Format(Resources.FillMultipleTeamCapacityDaysOff_CapacityDataNotFound, (object) key.Item1, (object) key.Item2));
        TeamConfigurationComponent.FillTeamCapacityDaysOff(capacityCollectionMap[key], (IEnumerable<TeamCapacityDaysOffRangeRow>) rows1);
      }
    }

    private void FillTeamIdAndIterationIdPair(
      IEnumerable<TeamIdIterationIdPairTypeRow> rows,
      out IEnumerable<Tuple<Guid, Guid>> allTeamIdAndIterationIdTuples)
    {
      allTeamIdAndIterationIdTuples = rows.Select<TeamIdIterationIdPairTypeRow, Tuple<Guid, Guid>>((System.Func<TeamIdIterationIdPairTypeRow, Tuple<Guid, Guid>>) (row => new Tuple<Guid, Guid>(row.TeamId, row.IterationId)));
    }

    private IEnumerable<TeamCapacity> GetOrderedTeamCapacity(
      IEnumerable<Tuple<Guid, Guid>> teamIdAndIterationIdPairs,
      IDictionary<Tuple<Guid, Guid>, TeamCapacity> allCapacitiesMap)
    {
      List<TeamCapacity> orderedTeamCapacity = new List<TeamCapacity>(teamIdAndIterationIdPairs.Count<Tuple<Guid, Guid>>());
      foreach (Tuple<Guid, Guid> andIterationIdPair in teamIdAndIterationIdPairs)
      {
        Tuple<Guid, Guid> key = new Tuple<Guid, Guid>(andIterationIdPair.Item1, andIterationIdPair.Item2);
        if (allCapacitiesMap.ContainsKey(key))
          orderedTeamCapacity.Add(allCapacitiesMap[key]);
        else
          orderedTeamCapacity.Add(new TeamCapacity());
      }
      return (IEnumerable<TeamCapacity>) orderedTeamCapacity;
    }
  }
}
