// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.FavoriteProviderController
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  [VersionedApiControllerCustomName("Favorite", "FavoriteProviders", 1)]
  public class FavoriteProviderController : TfsApiController
  {
    public override string TraceArea => "FavoriteProviders";

    public override string ActivityLogArea => "Favorites";

    [TraceFilter(15160009, 15160010)]
    [HttpGet]
    [ClientInternalUseOnly(false)]
    public IEnumerable<FavoriteProvider> GetFavoriteProviders(bool faultInMissingHost = false) => this.TfsRequestContext.GetService<IFavoriteProviderService>().GetFavoriteProviders(this.TfsRequestContext, faultInMissingHost).Where<FavoriteProvider>((Func<FavoriteProvider, bool>) (o => !this.IsUnrenderableProvider(o)));

    private bool IsUnrenderableProvider(FavoriteProvider provider) => provider.PluralName == null && provider.IconClass == null && provider.IconName == null;
  }
}
