// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.FavoriteMigrationSqlResourceComponent
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Favorites
{
  internal class FavoriteMigrationSqlResourceComponent : TeamFoundationSqlResourceComponent
  {
    public FavoriteHolders EnumeratePartitionFavoriteHolders() => new FavoriteHolders()
    {
      Teams = this.RunGuidListQuery(this.GetPartitionTeamsQuery()),
      Users = this.RunGuidListQuery(this.GetPartitionUsersQuery())
    };

    private List<Guid> RunGuidListQuery(string sqlBatch)
    {
      FavoriteHolders favoriteHolders = new FavoriteHolders();
      this.PrepareSqlBatch(sqlBatch.Length, true);
      this.AddStatement(sqlBatch);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      List<Guid> guidList = new List<Guid>();
      while (sqlDataReader.Read())
        guidList.Add(sqlDataReader.GetGuid(0));
      return guidList;
    }

    private string GetPartitionUsersQuery() => "SELECT DISTINCT Cast(substring(Name,62,36) as UNIQUEIDENTIFIER) AS OwnerGuid\r\n                FROM [tbl_PropertyDefinition]  \r\n                WHERE PartitionId = @partitionId\r\n                AND Name like 'Microsoft.TeamFoundation.Framework.Server.IdentityFavorites.%'\r\n                OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))";

    private string GetPartitionTeamsQuery() => "SELECT DISTINCT TRY_CAST(substring(Name,99,36) AS uniqueidentifier) AS OwnerGuid\r\n                FROM [tbl_PropertyDefinition]\r\n                WHERE PartitionId = @partitionId\r\n                AND Name like 'Microsoft.TeamFoundation.Framework.Server.IdentityFavorites.%'\r\n                OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))";
  }
}
