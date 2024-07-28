// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamConfigurationComponent12
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamConfigurationComponent12 : TeamConfigurationComponent11
  {
    internal override IDictionary<string, IDictionary<Guid, bool>> GetAllTeamFieldsForProject(
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_GetAllTeamConfigurationTeamFieldsForProject");
      this.BindDataspaceId(projectId);
      IDictionary<string, IDictionary<Guid, bool>> result = (IDictionary<string, IDictionary<Guid, bool>>) new Dictionary<string, IDictionary<Guid, bool>>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFieldRow>((ObjectBinder<TeamFieldRow>) new TeamFieldConfigurationRowBinder());
        List<TeamFieldRow> items = resultCollection.GetCurrent<TeamFieldRow>().Items;
        TeamConfigurationComponent.FillTeamConfigurationTeamFields(result, (IList<TeamFieldRow>) items);
      }
      return result;
    }
  }
}
