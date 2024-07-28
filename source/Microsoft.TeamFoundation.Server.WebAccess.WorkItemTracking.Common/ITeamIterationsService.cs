// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ITeamIterationsService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DefaultServiceImplementation(typeof (TeamIterationsService))]
  public interface ITeamIterationsService : IVssFrameworkService
  {
    SortedIterationSubscriptions GetSortedTeamIterations(
      IVssRequestContext context,
      Guid projectId,
      ITeamIterationsCollection iterations);

    IDictionary<Guid, SortedIterationSubscriptions> GetSortedTeamIterations(
      IVssRequestContext context,
      Guid projectId,
      IEnumerable<WebApiTeam> teams);

    IReadOnlyCollection<WebApiTeam> GetTeamsWithIterations(
      IVssRequestContext context,
      ProjectInfo project);

    IReadOnlyCollection<WebApiTeam> FilterToTeamsWithIterations(
      IVssRequestContext context,
      ProjectInfo project,
      IEnumerable<WebApiTeam> teams);
  }
}
