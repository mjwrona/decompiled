// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.FavoriteMetadataEngine
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  internal class FavoriteMetadataEngine : IFavoriteMetadataEngine, IVssFrameworkService
  {
    private Dictionary<string, IMetadataProvider> metadataProvidersMap;

    public IEnumerable<QueuedFavoriteEdit> ProcessMetadata(
      IVssRequestContext requestContext,
      IEnumerable<Favorite> favorites)
    {
      if (favorites.IsNullOrEmpty<Favorite>())
        return (IEnumerable<QueuedFavoriteEdit>) null;
      List<QueuedFavoriteEdit> queuedEdits = new List<QueuedFavoriteEdit>();
      foreach (IGrouping<string, Favorite> grouping in favorites.GroupBy<Favorite, string>((Func<Favorite, string>) (o => o.ArtifactType)))
      {
        using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "Favorites", "FavoritesExtended"))
        {
          string key = grouping.Key;
          timedCiEvent["type"] = (object) key;
          timedCiEvent["itemCount"] = (object) grouping.Count<Favorite>();
          IMetadataProvider metadataProvider;
          if (this.TryGetProvider(key, out metadataProvider))
            this.ProcessMetadataInBatch(requestContext, grouping, metadataProvider, key, queuedEdits);
        }
      }
      return (IEnumerable<QueuedFavoriteEdit>) queuedEdits;
    }

    public void AddArtifactLinks(IVssRequestContext requestContext, IEnumerable<Favorite> favorites)
    {
      if (favorites.IsNullOrEmpty<Favorite>())
        return;
      foreach (Favorite favorite in favorites)
      {
        IMetadataProvider metadataProvider;
        if (this.TryGetProvider(favorite.ArtifactType, out metadataProvider))
        {
          try
          {
            favorite.Links = new ReferenceLinks();
            favorite.Links.AddLink("page", metadataProvider.GetArtifactPageLink(requestContext, favorite));
          }
          catch (Exception ex)
          {
            this.LogProviderException(requestContext, "GetArtifactPageLinkFailed", favorite.ArtifactType, ex);
          }
        }
      }
    }

    public IEnumerable<Favorite> SelectWhitelistedFavoriteTypes(IEnumerable<Favorite> favorites) => favorites.Where<Favorite>((Func<Favorite, bool>) (fav => this.HasProvider(fav.ArtifactType)));

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (this.metadataProvidersMap != null)
        return;
      this.metadataProvidersMap = new Dictionary<string, IMetadataProvider>();
      foreach (IMetadataProvider extension in (IEnumerable<IMetadataProvider>) systemRequestContext.GetExtensions<IMetadataProvider>(ExtensionLifetime.Service))
        this.metadataProvidersMap.Add(extension.GetArtifactType(), extension);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public FavoriteMetadataEngine()
    {
    }

    public FavoriteMetadataEngine(
      Dictionary<string, IMetadataProvider> metadataProviderMap)
    {
      this.metadataProvidersMap = metadataProviderMap;
    }

    public void ProcessMetadataInBatch(
      IVssRequestContext requestContext,
      IGrouping<string, Favorite> typeGroup,
      IMetadataProvider metadataProvider,
      string favoriteType,
      List<QueuedFavoriteEdit> queuedEdits)
    {
      Dictionary<Guid?, FavoriteMetadataSnapshot> dictionary = typeGroup.ToDictionary<Favorite, Guid?, FavoriteMetadataSnapshot>((Func<Favorite, Guid?>) (favorite => favorite.Id), (Func<Favorite, FavoriteMetadataSnapshot>) (favorite => new FavoriteMetadataSnapshot(favorite)));
      try
      {
        List<Favorite> list = typeGroup.ToList<Favorite>().Select<Favorite, Favorite>((Func<Favorite, Favorite>) (e =>
        {
          e.ArtifactIsDeleted = false;
          return e;
        })).ToList<Favorite>();
        metadataProvider.UpdateMetadata(requestContext, (IEnumerable<Favorite>) list);
      }
      catch (Exception ex)
      {
        this.LogProviderException(requestContext, "ProcessMetadataFailed", favoriteType, ex);
      }
      foreach (Favorite favorite in (IEnumerable<Favorite>) typeGroup)
        this.DetectEdits(favorite, queuedEdits, dictionary[favorite.Id]);
    }

    public IEnumerable<Favorite> AppendScopeInfo(
      IVssRequestContext requestContext,
      IScopeProvider scopeNameProvider,
      IEnumerable<Favorite> favorites)
    {
      try
      {
        return scopeNameProvider.AppendScopeInfo(requestContext, favorites);
      }
      catch (Exception ex)
      {
        this.LogProviderException(requestContext, "AppendScopeNameFailed", scopeNameProvider.GetType().ToString(), ex);
        return favorites;
      }
    }

    private void DetectEdits(
      Favorite favorite,
      List<QueuedFavoriteEdit> queuedEdits,
      FavoriteMetadataSnapshot oldMetadataSnapshot)
    {
      if (oldMetadataSnapshot.IsEqual(favorite))
        return;
      queuedEdits.Add(new QueuedFavoriteEdit()
      {
        Item = favorite,
        IsEdited = true,
        UpdatedArtifactName = favorite.ArtifactName,
        UpdatedArtifactProperties = favorite.ArtifactProperties,
        UpdatedArtifactIsDeleted = favorite.ArtifactIsDeleted
      });
    }

    private void LogProviderException(
      IVssRequestContext requestContext,
      string feature,
      string providerType,
      Exception e)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("ProviderType", providerType);
      intelligenceData.Add("ExceptionMessage", e?.Message);
      IVssRequestContext requestContext1 = requestContext;
      string feature1 = feature;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "Favorites", feature1, properties);
    }

    private bool TryGetProvider(string favoriteType, out IMetadataProvider metadataProvider)
    {
      metadataProvider = (IMetadataProvider) null;
      return !string.IsNullOrWhiteSpace(favoriteType) && this.metadataProvidersMap.TryGetValue(favoriteType, out metadataProvider);
    }

    private bool HasProvider(string provider) => !string.IsNullOrWhiteSpace(provider) && this.metadataProvidersMap.ContainsKey(provider);
  }
}
