// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityProperties.TeamFavoriteOrderHelper
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.IdentityProperties
{
  public class TeamFavoriteOrderHelper
  {
    private static readonly string[] s_identityProperties = new string[2]
    {
      TeamFavoriteOrderHelper.TileOrderPropertyName,
      TeamConstants.TeamPropertyName
    };
    private static readonly string TileOrderPropertyName = TeamConstants.TileZoneTileOrderPropertyName + ".tile-zone-team-favorites";

    public static List<string> GetFavoritesTileOrder(
      IVssRequestContext requestContext,
      WebApiTeam team)
    {
      string propertyValue;
      requestContext.GetService<ITeamService>().TryGetTeamViewProperty<string>(requestContext, team.Id, TeamFavoriteOrderHelper.TileOrderPropertyName, out propertyValue);
      List<string> favoritesTileOrder = new List<string>();
      if (propertyValue != null)
        favoritesTileOrder = ((IEnumerable<string>) propertyValue.Split(';')).ToList<string>();
      return favoritesTileOrder;
    }

    public static void SetTeamFavoritesTileOrder(
      IVssRequestContext requestContext,
      WebApiTeam team,
      string[] tileData)
    {
      string propertyValue = string.Join(";", tileData);
      requestContext.GetService<ITeamService>().SetTeamViewProperty(requestContext, team.Id, TeamFavoriteOrderHelper.TileOrderPropertyName, propertyValue);
    }
  }
}
