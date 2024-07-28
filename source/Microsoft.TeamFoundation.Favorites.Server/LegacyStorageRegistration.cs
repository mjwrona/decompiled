// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.LegacyStorageRegistration
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Favorites
{
  public static class LegacyStorageRegistration
  {
    private static Dictionary<string, string> s_TypeToScopeMapping = LegacyStorageRegistration.InitializeTypeToScopeMapping();
    private static IEnumerable<string> s_emptyList = (IEnumerable<string>) new List<string>();
    private static Dictionary<string, string> s_emptyDictionary = new Dictionary<string, string>();

    public static IEnumerable<string> GetLegacyFavoriteTypes(IVssRequestContext requestContext) => FavoriteMigrationState.IsMigrationCompleted(requestContext) ? LegacyStorageRegistration.s_emptyList : (IEnumerable<string>) LegacyStorageRegistration.s_TypeToScopeMapping.Keys;

    public static IEnumerable<string> GetLegacyFavoriteScopes(IVssRequestContext requestContext) => FavoriteMigrationState.IsMigrationCompleted(requestContext) ? LegacyStorageRegistration.s_emptyList : (IEnumerable<string>) LegacyStorageRegistration.s_TypeToScopeMapping.Values;

    public static Dictionary<string, string> GetTypeToScopeMapping(IVssRequestContext requestContext) => FavoriteMigrationState.IsMigrationCompleted(requestContext) ? LegacyStorageRegistration.s_emptyDictionary : LegacyStorageRegistration.s_TypeToScopeMapping;

    private static Dictionary<string, string> InitializeTypeToScopeMapping() => new Dictionary<string, string>()
    {
      {
        "Microsoft.TeamFoundation.WorkItemTracking.QueryItem",
        "WorkItemTracking.Queries"
      },
      {
        "Microsoft.TeamFoundation.Build.Definition",
        "Build.Definitions"
      },
      {
        "Microsoft.TeamFoundation.Build.BuildDefinitionInitialized",
        "Build.Definitions"
      },
      {
        "Microsoft.TeamFoundation.Tfvc.Repository",
        "VersionControl.Repositories"
      },
      {
        "Microsoft.TeamFoundation.Git.Repository",
        "VersionControl.Repositories"
      },
      {
        "Microsoft.TeamFoundation.TestManagement.Plan",
        "TestManagement.Plans"
      },
      {
        "Microsoft.TeamFoundation.Classification.TeamProject",
        "Classification.TeamProjects"
      },
      {
        "Microsoft.TeamFoundation.Teams.Team",
        "Team.Teams"
      },
      {
        "Microsoft.TeamFoundation.Work.Plans",
        "Work.Plans"
      }
    };

    public static List<string> GetWhiteListedTypes() => new List<string>()
    {
      "Microsoft.TeamFoundation.WorkItemTracking.QueryItem",
      "Microsoft.TeamFoundation.Build.Definition",
      "Microsoft.TeamFoundation.Build.BuildDefinitionInitialized",
      "Microsoft.TeamFoundation.Tfvc.Repository",
      "Microsoft.TeamFoundation.Git.Repository",
      "Microsoft.TeamFoundation.TestManagement.Plan",
      "Microsoft.TeamFoundation.Classification.TeamProject",
      "Microsoft.TeamFoundation.Teams.Team",
      "Microsoft.TeamFoundation.Work.Plans"
    };
  }
}
