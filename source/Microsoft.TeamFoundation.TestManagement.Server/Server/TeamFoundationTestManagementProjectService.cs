// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementProjectService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementProjectService : 
    TfsTestManagementService,
    ITeamFoundationTestManagementProjectService,
    IVssFrameworkService
  {
    public TeamFoundationTestManagementProjectService()
    {
    }

    public TeamFoundationTestManagementProjectService(TfsTestManagementRequestContext requestContext)
      : base((TestManagementRequestContext) requestContext)
    {
    }

    public IDictionary<Guid, int> QueryProjects(
      IVssRequestContext requestContext,
      bool queryDeleted = false)
    {
      return (IDictionary<Guid, int>) this.ExecuteAction<Dictionary<Guid, int>>(requestContext, "TeamFoundationTestManagementSessionService.QueryProjects", (Func<Dictionary<Guid, int>>) (() =>
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
          return managementDatabase.QueryProjectsWithDataspaceIds(queryDeleted).ToDictionary<KeyValuePair<GuidAndString, int>, Guid, int>((Func<KeyValuePair<GuidAndString, int>, Guid>) (p => p.Key.GuidId), (Func<KeyValuePair<GuidAndString, int>, int>) (p => p.Value));
      }), 1015500, "TestManagement", "BusinessLayer");
    }
  }
}
