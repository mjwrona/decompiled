// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.ApiFavoritesController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.ApplicationAll)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiFavoritesController : TfsController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult List(Guid? identityId, string scope)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetFavoriteStore(identityId, scope).GetFavorites(this.TfsRequestContext).ToArray<FavoriteItem>());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put | HttpVerbs.Delete)]
    public ActionResult Delete(Guid? identityId, string scope, Guid[] favItemIds)
    {
      IdentityFavorites favoriteStore = this.GetFavoriteStore(identityId, scope);
      this.CheckIfTeamAdminNeeded(identityId);
      if (identityId.HasValue)
      {
        Guid empty = Guid.Empty;
      }
      else
      {
        Guid masterId = this.TfsWebContext.CurrentUserIdentity.MasterId;
      }
      favoriteStore.DeleteFavoriteItems(this.TfsRequestContext, (IEnumerable<Guid>) favItemIds);
      favoriteStore.Update(this.TfsRequestContext);
      return (ActionResult) this.Json((object) new
      {
        success = true
      });
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    public ActionResult Update(Guid? identityId, string scope, string[] favItemJsons)
    {
      FavoriteItem[] array = ((IEnumerable<string>) favItemJsons).Select<string, FavoriteItem>((Func<string, FavoriteItem>) (favItemJson => FavoriteItem.Deserialize(favItemJson))).ToArray<FavoriteItem>();
      IdentityFavorites favoriteStore = this.GetFavoriteStore(identityId, scope);
      System.Collections.Generic.List<JsObject> data = new System.Collections.Generic.List<JsObject>();
      int num = 0;
      foreach (FavoriteItem favoriteItem in array)
      {
        if (favoriteItem.Id == Guid.Empty)
        {
          favoriteItem.Id = Guid.NewGuid();
          System.Collections.Generic.List<JsObject> jsObjectList = data;
          JsObject jsObject = new JsObject();
          jsObject.Add("index", (object) num);
          jsObject.Add("id", (object) favoriteItem.Id);
          jsObjectList.Add(jsObject);
        }
        ++num;
      }
      this.CheckIfTeamAdminNeeded(identityId);
      if (identityId.HasValue)
      {
        Guid empty = Guid.Empty;
      }
      else
      {
        Guid masterId = this.TfsWebContext.CurrentUserIdentity.MasterId;
      }
      favoriteStore.UpdateFavoriteItems(this.TfsRequestContext, (IEnumerable<FavoriteItem>) array);
      favoriteStore.Update(this.TfsRequestContext);
      return (ActionResult) this.Json((object) data);
    }

    private void CheckIfTeamAdminNeeded(Guid? identityId)
    {
      if (!identityId.HasValue)
        return;
      ITeamService service = this.TfsRequestContext.GetService<ITeamService>();
      if (!service.UserIsTeamAdmin(this.TfsRequestContext, service.GetTeamInProject(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid, identityId.Value.ToString()).Identity))
        throw new InvalidAccessException(WACommonResources.UserNeedsToBeTeamAdmin);
    }

    private IdentityFavorites GetFavoriteStore(Guid? identityId, string scope)
    {
      if (!identityId.HasValue)
        return this.TfsWebContext.FavoriteStore(identityId, scope);
      WebApiTeam teamInProject = this.TfsRequestContext.GetService<ITeamService>().GetTeamInProject(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid, identityId.Value.ToString());
      return this.TfsWebContext.FavoriteStore(identityId.Value, this.TfsWebContext.Project, teamInProject, scope);
    }
  }
}
