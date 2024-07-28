// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamConfigurationComponent15
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
  internal class TeamConfigurationComponent15 : TeamConfigurationComponent14
  {
    internal override IEnumerable<Guid> GetTeamsWithSubscribedIterations(Guid projectId)
    {
      this.PrepareStoredProcedure("prc_GetTeamsWithSubscribedIterations");
      this.BindDataspaceId(projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new TeamIdBinder());
        return (IEnumerable<Guid>) resultCollection.GetCurrent<Guid>().Items;
      }
    }

    internal override IDictionary<Guid, IEnumerable<Guid>> GetIterationSubscriptionsForTeams(
      Guid projectId,
      IEnumerable<Guid> teamIds)
    {
      this.PrepareStoredProcedure("prc_GetTeamConfigurationIterationsForTeams");
      this.BindDataspaceId(projectId);
      this.BindGuidTable("@teamIdsTable", teamIds);
      IList<TeamIdIterationIdPairTypeRow> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamIdIterationIdPairTypeRow>((ObjectBinder<TeamIdIterationIdPairTypeRow>) new TeamIdIterationIdPairTypeRowBinder());
        items = (IList<TeamIdIterationIdPairTypeRow>) resultCollection.GetCurrent<TeamIdIterationIdPairTypeRow>().Items;
      }
      return (IDictionary<Guid, IEnumerable<Guid>>) items.GroupBy<TeamIdIterationIdPairTypeRow, Guid, Guid>((System.Func<TeamIdIterationIdPairTypeRow, Guid>) (r => r.TeamId), (System.Func<TeamIdIterationIdPairTypeRow, Guid>) (r => r.IterationId)).ToDictionary<IGrouping<Guid, Guid>, Guid, IEnumerable<Guid>>((System.Func<IGrouping<Guid, Guid>, Guid>) (g => g.Key), (System.Func<IGrouping<Guid, Guid>, IEnumerable<Guid>>) (g => (IEnumerable<Guid>) g));
    }
  }
}
