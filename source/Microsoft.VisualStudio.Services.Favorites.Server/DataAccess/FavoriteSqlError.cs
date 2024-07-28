// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.DataAccess.FavoriteSqlError
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

namespace Microsoft.VisualStudio.Services.Favorites.Server.DataAccess
{
  internal class FavoriteSqlError
  {
    public const int SqlServerDefaultUserMessage = 50000;
    public const int GenericDatabaseFailure = 1730000;
    public const int OwnerIdConflict = 1730001;
    public const int UpdateOfDeletedFavorite = 1730002;
    public const int MAX_SQL_ERROR = 1730002;
  }
}
