// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Plugins.MacroArgumentHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Plugins
{
  public static class MacroArgumentHelper
  {
    private static readonly Regex m_teamRegex = new Regex("^\\[(.+)\\]\\\\(.+?)( <.+>)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static void ParseTeam(
      IVssRequestContext requestContext,
      string teamParameter,
      out ProjectInfo project,
      out WebApiTeam team)
    {
      Match match = MacroArgumentHelper.m_teamRegex.Match(teamParameter);
      if (!match.Success)
        throw new WorkItemTrackingQueryException(string.Format(Resources.MalformedTeamName, (object) teamParameter));
      string projectName = match.Groups[1].Value;
      string teamIdOrName = match.Groups[2].Value;
      IProjectService service1 = requestContext.GetService<IProjectService>();
      project = service1.GetProject(requestContext, projectName);
      ITeamService service2 = requestContext.GetService<ITeamService>();
      team = service2.GetTeamInProject(requestContext, project.Id, teamIdOrName);
    }
  }
}
