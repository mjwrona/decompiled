// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.FavoriteStorageMigrator
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.Server;
using Microsoft.VisualStudio.Services.Favorites.Server.Service;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Favorites
{
  public class FavoriteStorageMigrator
  {
    public int NumFailedOwners { get; private set; }

    public int NumFavoritesMigrated { get; private set; }

    public StringBuilder MigrationLog { get; private set; }

    private HashSet<string> WhiteListedFavoriteTypes { get; set; }

    public FavoriteStorageMigrator()
    {
      this.NumFailedOwners = 0;
      this.NumFavoritesMigrated = 0;
      this.MigrationLog = new StringBuilder();
      this.WhiteListedFavoriteTypes = new HashSet<string>((IEnumerable<string>) LegacyStorageRegistration.GetWhiteListedTypes(), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
    }

    public void PerformMigration(IVssRequestContext requestContext)
    {
      FavoriteHolders favoriteHolders;
      using (FavoriteMigrationSqlResourceComponent component = requestContext.CreateComponent<FavoriteMigrationSqlResourceComponent>())
        favoriteHolders = component.EnumeratePartitionFavoriteHolders();
      foreach (Guid user in favoriteHolders.Users)
      {
        try
        {
          OwnerScope owner = OwnerScope.SpecifiedUser(user);
          this.NumFavoritesMigrated += this.MigrateFavorites(requestContext, owner);
        }
        catch (Exception ex)
        {
          this.MigrationLog.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Migration failed for User:{0}:\n {1}\n", (object) user, (object) ex.StackTrace));
          ++this.NumFailedOwners;
        }
      }
      foreach (Guid team in favoriteHolders.Teams)
      {
        try
        {
          OwnerScope owner = OwnerScope.Team(team);
          this.NumFavoritesMigrated += this.MigrateFavorites(requestContext, owner);
        }
        catch (Exception ex)
        {
          this.MigrationLog.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Migration failed for Team:{0}:\n {1}\n", (object) team, (object) ex.StackTrace));
          ++this.NumFailedOwners;
        }
      }
    }

    public string SummarizeFailure() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Migration failed.\nFailed identities: {0}.\nFavorites migrated: {1}.\n{2}\n", (object) this.NumFailedOwners, (object) this.NumFavoritesMigrated, (object) this.MigrationLog.ToString());

    public string SummarizeSuccess() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Migration successful.\n Favorites migrated: {0}.\n", (object) this.NumFavoritesMigrated);

    private int MigrateFavorites(IVssRequestContext tfsRequestContext, OwnerScope owner)
    {
      IEnumerable<TfsFavorite> accountScopedFavorites = new FavoriteStoreClientFactory().GetAccountStore(tfsRequestContext, owner).GetAccountScopedFavorites(tfsRequestContext, owner);
      List<Favorite> favoriteList = new List<Favorite>();
      foreach (TfsFavorite favorite in accountScopedFavorites)
      {
        if (this.WhiteListedFavoriteTypes.Contains(favorite.Type))
        {
          Favorite modernFavorite = favorite.ToModernFavorite(owner.GetIdentityRef(tfsRequestContext));
          favoriteList.Add(modernFavorite);
        }
      }
      if (favoriteList.Any<Favorite>())
        new DefaultFavoriteStorage().CreateUpdateFavorites(tfsRequestContext, owner, (IEnumerable<Favorite>) favoriteList);
      return favoriteList.Count;
    }
  }
}
