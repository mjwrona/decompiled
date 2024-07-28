// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.FavoriteMigrationState
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  public class FavoriteMigrationState
  {
    public const string FavoritesMigrationCompletedFlag = "Favorites.MigrationCompleted";
    public const string FavoritesMigrationActiveFlag = "Favorites.MigrationActiveSuppressWrites";

    public static void ThrowIfMigrationActive(IVssRequestContext requestContext)
    {
      if (FavoriteMigrationState.IsMigrationActive(requestContext))
        throw new FavoriteWritesDisabledDuringMigrationException(FavoritesWebApiResources.FavoriteWritesDisabledDuringMigrationExceptionMessage());
    }

    public static bool IsMigrationActive(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Favorites.MigrationActiveSuppressWrites");

    public static bool IsMigrationCompleted(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Favorites.MigrationCompleted");

    public static void StartMigration(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(requestContext, "Favorites.MigrationActiveSuppressWrites", FeatureAvailabilityState.On);

    public static void EndMigration(IVssRequestContext requestContext, bool migrationWasSuccessful)
    {
      ITeamFoundationFeatureAvailabilityService service = requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      service.SetFeatureState(requestContext, "Favorites.MigrationActiveSuppressWrites", FeatureAvailabilityState.Off);
      service.SetFeatureState(requestContext, "Favorites.MigrationCompleted", migrationWasSuccessful ? FeatureAvailabilityState.On : FeatureAvailabilityState.Off);
    }

    private static void EnableFlag(IVssRequestContext requestContext, string featureName) => requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(requestContext, featureName, FeatureAvailabilityState.On);
  }
}
