// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.CompatibilityController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class CompatibilityController : TfsController
  {
    private const string c_queryParamName = "collectionName";
    private TimeSpan RegexReplaceTimeout = TimeSpan.FromSeconds(30.0);
    private static List<CompatibilityController.RouteBackwardCompatibileUrl> sm_routers = new List<CompatibilityController.RouteBackwardCompatibileUrl>();

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(507011, 507020)]
    public ActionResult RouteRooms()
    {
      string name = this.TfsWebContext.TfsRequestContext.ServiceHost.Name;
      this.Trace(507012, TraceLevel.Verbose, "Route rooms:{0}", (object) name);
      this.ViewData["Url"] = (object) string.Format("{0}#{1}={2}", (object) Regex.Replace(this.TfsWebContext.TfsRequestContext.RequestUri().ToString(), name + "/", "", RegexOptions.IgnoreCase, this.RegexReplaceTimeout), (object) "collectionName", (object) HttpUtility.UrlEncode(name));
      return (ActionResult) this.View("RedirectWithHash", (object) this.ViewData);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(507000, 507010)]
    [ValidateInput(false)]
    public ActionResult Route(string page)
    {
      this.Trace(507001, TraceLevel.Verbose, "Route: page:{0}", (object) page);
      if (string.IsNullOrEmpty(page))
        return (ActionResult) new RedirectResult(this.Url.Action("index", "home", (object) new
        {
          routeArea = ""
        }), false);
      TfsLocator fromPairs = TfsLocator.CreateFromPairs(this.Request.Url, this.Request.Params);
      fromPairs.InitializeHostId(this.TfsWebContext);
      IVssRequestContext requestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      TeamFoundationHostManagementService service = this.TfsRequestContext.GetService<TeamFoundationHostManagementService>();
      try
      {
        using (IVssRequestContext collectionRequestContext = service.BeginUserRequest(requestContext, fromPairs.HostGuid, requestContext.UserContext, false))
        {
          ProjectInfo projectInfo = fromPairs.GetProjectInfo(collectionRequestContext);
          if (page.EndsWith("index.aspx", StringComparison.OrdinalIgnoreCase))
            return (ActionResult) new RedirectResult(projectInfo == null ? this.Url.Action("index", "home", (object) new
            {
              routeArea = "",
              serviceHost = collectionRequestContext.ServiceHost
            }) : (string.IsNullOrEmpty(fromPairs.TeamName) ? this.Url.Action("index", "home", (object) new
            {
              routeArea = "",
              serviceHost = collectionRequestContext.ServiceHost,
              project = projectInfo.Name
            }) : this.Url.Action("index", "home", (object) new
            {
              routeArea = "",
              serviceHost = collectionRequestContext.ServiceHost,
              project = projectInfo.Name,
              team = fromPairs.TeamName
            })), false);
          fromPairs.ThrowIfHostIsNotCollection();
          foreach (CompatibilityController.RouteBackwardCompatibileUrl smRouter in CompatibilityController.sm_routers)
          {
            string url = smRouter(this, page, fromPairs, projectInfo);
            if (!string.IsNullOrEmpty(url))
              return (ActionResult) new RedirectResult(url, false);
          }
          throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.UnkownCompatibilityUrl, (object) this.Request.Url.ToString()));
        }
      }
      catch (HostDoesNotExistException ex)
      {
        throw new HttpException(404, ex.Message, (Exception) ex);
      }
      catch (HostInstanceDoesNotExistException ex)
      {
        throw new HttpException(404, ex.Message, (Exception) ex);
      }
      catch (TeamFoundationServiceException ex)
      {
        if (ex.HttpStatusCode != HttpStatusCode.InternalServerError)
          throw new HttpException((int) ex.HttpStatusCode, ex.Message, (Exception) ex);
        throw;
      }
    }

    public static void RegisterCompatRouter(
      CompatibilityController.RouteBackwardCompatibileUrl router)
    {
      CompatibilityController.sm_routers.Add(router);
    }

    public delegate string RouteBackwardCompatibileUrl(
      CompatibilityController controller,
      string page,
      TfsLocator locator,
      ProjectInfo projectInfo);
  }
}
