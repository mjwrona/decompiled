// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamConfigurationComponent10
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamConfigurationComponent10 : TeamConfigurationComponent9
  {
    internal override void DeleteTeamIterations(
      Guid projectId,
      Guid teamId,
      IEnumerable<Guid> iterationIds)
    {
      this.PrepareStoredProcedure("prc_DeleteTeamConfigurationTeamIterations");
      this.BindDataspaceId(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindGuidTable("@iterationIdTable", iterationIds);
      this.ExecuteNonQuery();
    }
  }
}
