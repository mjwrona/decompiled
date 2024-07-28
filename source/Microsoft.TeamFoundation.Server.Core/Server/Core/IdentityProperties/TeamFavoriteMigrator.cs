// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityProperties.TeamFavoriteMigrator
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.Core.IdentityProperties
{
  public class TeamFavoriteMigrator
  {
    private Dictionary<string, string> m_scopeMap;

    public TeamFavoriteMigrator()
    {
      this.m_scopeMap = new Dictionary<string, string>();
      this.ToPinScope("Build.Definitions");
      this.ToPinScope("VersionControl.Paths");
      this.ToPinScope("WorkItemTracking.Queries");
    }

    public static void PerformMigration(
      IVssRequestContext requestContext,
      ref string resultMessage,
      ref int numFailedTeams,
      ref int numFavoritesMigrated)
    {
      ITeamService service1 = requestContext.GetService<ITeamService>();
      IProjectService service2 = requestContext.GetService<IProjectService>();
      TeamFavoriteMigrator favoriteMigrator = new TeamFavoriteMigrator();
      IVssRequestContext requestContext1 = requestContext;
      foreach (ProjectInfo project in service2.GetProjects(requestContext1, ProjectState.WellFormed))
      {
        foreach (WebApiTeam team in (IEnumerable<WebApiTeam>) service1.QueryTeamsInProject(requestContext, project.Id))
        {
          if (team != null)
          {
            try
            {
              List<string> favoritesTileOrder = TeamFavoriteOrderHelper.GetFavoritesTileOrder(requestContext, team);
              List<string> source = new List<string>();
              IDictionary<string, string> dictionary = favoriteMigrator.MigrateTeamFavorites(requestContext, project.Id, team.Id, ref numFavoritesMigrated);
              foreach (string key in favoritesTileOrder)
              {
                string str;
                if (dictionary.TryGetValue(key, out str))
                  source.Add(str);
              }
              TeamFavoriteOrderHelper.SetTeamFavoritesTileOrder(requestContext, team, source.ToArray<string>());
            }
            catch (Exception ex)
            {
              resultMessage += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Migration failed for {0}/{1}:\n {2}\n", (object) project.Name, (object) team.Name, (object) ex.StackTrace);
              ++numFailedTeams;
            }
          }
        }
      }
    }

    private IDictionary<string, string> MigrateTeamFavorites(
      IVssRequestContext tfsRequestContext,
      Guid projectGuid,
      Guid teamGuid,
      ref int numFavoritesMigrated)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> scope in this.m_scopeMap)
      {
        string key1 = scope.Key;
        string scopeName = scope.Value;
        IEnumerable<FavoriteItem> favorites = IdentityPropertiesView.CreateView<IdentityFavorites>(tfsRequestContext, teamGuid, TeamFavoriteMigrator.GetScope(projectGuid, teamGuid, key1)).GetFavorites(tfsRequestContext);
        IdentityFavorites view = IdentityPropertiesView.CreateView<IdentityFavorites>(tfsRequestContext, teamGuid, TeamFavoriteMigrator.GetScope(projectGuid, teamGuid, scopeName));
        HashSet<string> stringSet = new HashSet<string>(view.GetFavorites(tfsRequestContext).Select<FavoriteItem, string>((Func<FavoriteItem, string>) (o => o.Data)));
        List<FavoriteItem> favoriteItemList = new List<FavoriteItem>();
        foreach (FavoriteItem favoriteItem in favorites)
        {
          if (!stringSet.Contains(favoriteItem.Data))
          {
            string key2 = favoriteItem.Id.ToString();
            favoriteItem.Id = Guid.NewGuid();
            favoriteItemList.Add(favoriteItem);
            ++numFavoritesMigrated;
            dictionary.Add(key2, favoriteItem.Id.ToString());
          }
        }
        if (favoriteItemList.Any<FavoriteItem>())
        {
          view.UpdateFavoriteItems(tfsRequestContext, (IEnumerable<FavoriteItem>) favoriteItemList);
          view.Update(tfsRequestContext);
        }
      }
      return dictionary;
    }

    private void ToPinScope(string source) => this.m_scopeMap.Add(source, "Pinning." + source);

    private static string GetScope(Guid projectGuid, Guid teamGuid, string scopeName)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('.');
      stringBuilder.Append(projectGuid.ToString());
      stringBuilder.Append('.');
      stringBuilder.Append(teamGuid.ToString());
      stringBuilder.Append('.');
      stringBuilder.Append(scopeName);
      return stringBuilder.ToString();
    }
  }
}
