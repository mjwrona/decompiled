// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityFavorites
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server.Core.IdentityProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class IdentityFavorites : IdentityPropertiesView
  {
    public FavoriteViewScopeInformation ScopeInformation { get; set; }

    public IEnumerable<FavoriteItem> GetFavorites(IVssRequestContext requestContext)
    {
      using (TimedCiEvent ciEvent = this.StartCIEvent(requestContext, "FavoritesQueried"))
      {
        IEnumerable<FavoriteItem> favoritesImpl = this.GetFavoritesImpl();
        this.GatherCIDetails(ciEvent, favoritesImpl);
        return favoritesImpl;
      }
    }

    private IEnumerable<FavoriteItem> GetFavoritesImpl()
    {
      IEnumerable<KeyValuePair<string, object>> viewProperties = this.GetViewProperties(IdentityPropertyScope.Local);
      List<FavoriteItem> favoritesImpl = new List<FavoriteItem>();
      HashSet<Guid> guidSet = new HashSet<Guid>();
      foreach (KeyValuePair<string, object> keyValuePair in viewProperties)
      {
        FavoriteItem favoriteItem = (FavoriteItem) null;
        try
        {
          string str = keyValuePair.Value as string;
          if (!string.IsNullOrEmpty(str))
            favoriteItem = FavoriteItem.Deserialize(str);
        }
        catch (SerializationException ex)
        {
        }
        if (favoriteItem != null && !guidSet.Contains(favoriteItem.Id))
        {
          favoritesImpl.Add(favoriteItem);
          guidSet.Add(favoriteItem.Id);
        }
      }
      return (IEnumerable<FavoriteItem>) favoritesImpl;
    }

    public void DeleteFavoriteItems(IVssRequestContext requestContext, IEnumerable<Guid> favIds)
    {
      using (TimedCiEvent ciEvent = this.StartCIEvent(requestContext, "FavoritesDeleted"))
      {
        requestContext.GetService<TeamFoundationEventService>();
        if (favIds == null || !favIds.Any<Guid>() || !favIds.All<Guid>((Func<Guid, bool>) (id => id != Guid.Empty)))
        {
          this.ClearViewProperties(IdentityPropertyScope.Local);
        }
        else
        {
          Dictionary<Guid, FavoriteItem> dict = this.GetFavoritesImpl().ToDictionary<FavoriteItem, Guid>((Func<FavoriteItem, Guid>) (f => f.Id));
          HashSet<Guid> guidSet = new HashSet<Guid>(favIds);
          foreach (FavoriteItem favoriteItem in dict.Values)
            this.FixTree(favoriteItem, dict, guidSet);
          foreach (Guid guid in guidSet)
            this.RemoveViewProperty(IdentityPropertyScope.Local, guid.ToString());
          IEnumerable<FavoriteItem> items = guidSet.Where<Guid>((Func<Guid, bool>) (k => dict.ContainsKey(k))).Select<Guid, FavoriteItem>((Func<Guid, FavoriteItem>) (k => dict[k]));
          this.GatherCIDetails(ciEvent, items);
        }
      }
    }

    private bool FixTree(
      FavoriteItem item,
      Dictionary<Guid, FavoriteItem> items,
      HashSet<Guid> itemsToDelete)
    {
      FavoriteItem favoriteItem = (FavoriteItem) null;
      bool flag = false;
      if (itemsToDelete.Contains(item.Id))
        return flag;
      if (item.ParentId == Guid.Empty)
        flag = true;
      else if (items.TryGetValue(item.ParentId, out favoriteItem))
        flag = this.FixTree(favoriteItem, items, itemsToDelete);
      if (!flag)
        itemsToDelete.Add(item.Id);
      return flag;
    }

    public void UpdateFavoriteItems(
      IVssRequestContext requestContext,
      IEnumerable<FavoriteItem> favItems)
    {
      using (TimedCiEvent ciEvent = this.StartCIEvent(requestContext, "FavoritesUpdated"))
      {
        Dictionary<Guid, FavoriteItem> dictionary = this.GetFavoritesImpl().ToDictionary<FavoriteItem, Guid>((Func<FavoriteItem, Guid>) (f => f.Id));
        requestContext.GetService<TeamFoundationEventService>();
        foreach (FavoriteItem favItem1 in favItems)
        {
          FavoriteItem favItem = favItem1;
          if (favItem.ParentId != Guid.Empty && !dictionary.ContainsKey(favItem.ParentId))
            throw new TeamFoundationServiceException(FrameworkResources.ParentFavoriteItemCouldNotBeFound());
          if (!dictionary.Values.Where<FavoriteItem>((Func<FavoriteItem, bool>) (f => f.ParentId == favItem.ParentId)).Any<FavoriteItem>((Func<FavoriteItem, bool>) (f => f.Id != favItem.Id && StringComparer.OrdinalIgnoreCase.Equals(f.Type ?? "", favItem.Type ?? "") && StringComparer.OrdinalIgnoreCase.Equals(f.Data, favItem.Data))))
          {
            try
            {
              this.SetViewProperty(IdentityPropertyScope.Local, favItem.Id.ToString(), (object) favItem.Serialize());
            }
            catch (Exception ex)
            {
              requestContext.TraceException(TracePoint.UpdateFavoriteItemFailed, nameof (IdentityFavorites), nameof (UpdateFavoriteItems), ex);
            }
          }
        }
        this.GatherCIDetails(ciEvent, favItems);
      }
    }

    protected TimedCiEvent StartCIEvent(IVssRequestContext requestContext, string operationName)
    {
      TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "Favorites", operationName);
      if (this.ScopeInformation != null)
      {
        timedCiEvent["project"] = (object) this.ScopeInformation.ProjectGuid;
        timedCiEvent["featureScope"] = (object) this.ScopeInformation.FeatureScope;
        timedCiEvent["isPersonal"] = (object) this.ScopeInformation.IsPersonal;
      }
      return timedCiEvent;
    }

    private void GatherCIDetails(TimedCiEvent ciEvent, IEnumerable<FavoriteItem> items)
    {
      ciEvent["favoriteItemCount"] = (object) items.Count<FavoriteItem>();
      FavoriteItem favoriteItem = items.FirstOrDefault<FavoriteItem>();
      if (favoriteItem == null)
        return;
      ciEvent["favoriteType"] = (object) favoriteItem.Type;
    }
  }
}
