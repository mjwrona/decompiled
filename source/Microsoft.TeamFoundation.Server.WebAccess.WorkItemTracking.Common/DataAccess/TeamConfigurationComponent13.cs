// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamConfigurationComponent13
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamConfigurationComponent13 : TeamConfigurationComponent12
  {
    internal override IEnumerable<Tuple<Guid, Guid, int>> GetChangedTeamConfigurationCapacity(
      int watermark)
    {
      this.PrepareStoredProcedure("prc_GetChangedTeamConfigurationCapacity");
      this.BindInt("@watermark", watermark);
      IEnumerable<Tuple<Guid, Guid, int>> changedCapacityTeamIdIterationIdWithWatermark = (IEnumerable<Tuple<Guid, Guid, int>>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamIdIterationIdWatermarkPairTypeRow>((ObjectBinder<TeamIdIterationIdWatermarkPairTypeRow>) new TeamIdIterationIdWatermarkPairTypeRowBinder());
        TeamConfigurationComponent.FillTeamIdIterationIdAndWatermarkPair((IEnumerable<TeamIdIterationIdWatermarkPairTypeRow>) resultCollection.GetCurrent<TeamIdIterationIdWatermarkPairTypeRow>().Items, out changedCapacityTeamIdIterationIdWithWatermark);
      }
      return changedCapacityTeamIdIterationIdWithWatermark;
    }

    internal override IEnumerable<Tuple<Guid, int>> GetChangedTeamSettings(int watermark)
    {
      this.PrepareStoredProcedure("prc_GetChangedTeamSettings");
      this.BindInt("@watermark", watermark);
      IEnumerable<Tuple<Guid, int>> changedTeamIdWithWatermark = (IEnumerable<Tuple<Guid, int>>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamIdWatermarkPairTypeRow>((ObjectBinder<TeamIdWatermarkPairTypeRow>) new TeamIdWatermarkPairTypeRowBinder());
        TeamConfigurationComponent.FillTeamIdWatermarkPair((IEnumerable<TeamIdWatermarkPairTypeRow>) resultCollection.GetCurrent<TeamIdWatermarkPairTypeRow>().Items, out changedTeamIdWithWatermark);
      }
      return changedTeamIdWithWatermark;
    }
  }
}
