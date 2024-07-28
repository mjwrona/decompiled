// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestManagementTeamsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestManagementTeamsDataProvider : IExtensionDataProvider
  {
    protected Func<IVssRequestContext, ContextIdentifier> GetProject;

    public TestManagementTeamsDataProvider() => this.GetProject = (Func<IVssRequestContext, ContextIdentifier>) (requestContext => WebPageDataProviderUtil.GetPageSource(requestContext).Project);

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      ContextIdentifier project = this.GetProject(requestContext);
      WebApiTeam defaultTeam = Utils.GetDefaultTeam(requestContext, project);
      IEnumerable<WebApiTeam> teams = Utils.GetTeams(requestContext, project);
      if (defaultTeam == null)
        return (object) null;
      return (object) new TestManagementTeamsData()
      {
        DefaultTeam = new Team()
        {
          Id = defaultTeam.Id.ToString(),
          Name = defaultTeam.Name
        },
        AllTeams = (IList<Team>) teams.Select<WebApiTeam, Team>((Func<WebApiTeam, Team>) (team => new Team()
        {
          Id = team.Id.ToString(),
          Name = team.Name
        })).ToList<Team>()
      };
    }

    public string Name => "TestManagement.Provider.Teams";
  }
}
