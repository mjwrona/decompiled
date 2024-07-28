// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.DataAccess.FavoriteSqlResourceComponent2
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Favorites.Server.DataAccess
{
  internal class FavoriteSqlResourceComponent2 : FavoriteSqlResourceComponent
  {
    public override List<Favorite> CreateUpdateFavorites(
      Guid ownerIdentity,
      bool ownerIsTeam,
      IEnumerable<Favorite> entries)
    {
      this.PrepareStoredProcedure("Favorites.prc_CreateUpdateFavorites");
      this.BindGuid("@ownerIdentity", ownerIdentity);
      this.BindBoolean("@ownerIsTeam", ownerIsTeam);
      this.BindFavoriteTable("@favorites", entries);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Favorite>((ObjectBinder<Favorite>) new FavoriteBinder());
      return resultCollection.GetCurrent<Favorite>().Items;
    }
  }
}
