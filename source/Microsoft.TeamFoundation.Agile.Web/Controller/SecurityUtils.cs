// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.SecurityUtils
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  public static class SecurityUtils
  {
    public static bool UserHasPermission(
      this WebApiTeam team,
      IVssRequestContext requestContext,
      int permission)
    {
      string key = "UserHasPermissions_" + team.Id.ToString() + "_" + permission.ToString();
      object obj;
      bool flag;
      if (!requestContext.Items.TryGetValue(key, out obj))
      {
        flag = requestContext.GetService<ITeamService>().HasTeamPermission(requestContext, team.Identity, permission, true);
        requestContext.Items[key] = (object) flag;
      }
      else
        flag = (bool) obj;
      return flag;
    }

    public static bool UserIsTeamAdmin(this WebApiTeam team, IVssRequestContext requestContext) => team.UserHasPermission(requestContext, 8);
  }
}
