// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.ProjectProcessConfigurationComponent5
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class ProjectProcessConfigurationComponent5 : ProjectProcessConfigurationComponent4
  {
    internal override IDictionary<Guid, string> GetTeamFieldsForProjects(
      IEnumerable<Guid> projectIds)
    {
      this.PrepareStoredProcedure("prc_GetTeamFieldsForProjects");
      return this.GetTeamFieldsForProjectsInternal(projectIds);
    }
  }
}
