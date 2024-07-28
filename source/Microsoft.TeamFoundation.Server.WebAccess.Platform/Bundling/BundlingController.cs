// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundlingController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  [SupportedRouteArea("Public", NavigationContextLevels.All)]
  public class BundlingController : WebPlatformController
  {
    private const string c_corsResponseHeaderAllowOrigin = "Access-Control-Allow-Origin";

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "BundleContent")]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public ActionResult Content(string bundle)
    {
      this.AddCorsHeader();
      return this.GetBundleContent(bundle, "text/javascript");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "BundleContent")]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public ActionResult CssContent(string bundle)
    {
      this.AddCorsHeader();
      return this.TfsRequestContext == null ? (ActionResult) new HttpNotFoundResult("NoVssRequestContext") : this.GetBundleContent(bundle, "text/css");
    }

    private ActionResult GetBundleContent(string bundle, string contentType)
    {
      if (DateTime.TryParse(this.HttpContext.Request.Headers["If-Modified-Since"], (IFormatProvider) null, DateTimeStyles.AdjustToUniversal, out DateTime _))
        return (ActionResult) new HttpStatusCodeResult(HttpStatusCode.NotModified);
      this.Response.Cookies.Clear();
      this.Response.Cache.SetOmitVaryStar(true);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      if (tfsRequestContext.IntendedHostType() != TeamFoundationHostType.Deployment)
        tfsRequestContext = tfsRequestContext.To(TeamFoundationHostType.Deployment);
      BundlingService service = tfsRequestContext.GetService<BundlingService>();
      try
      {
        Stream fileStream = service.RetrieveBundleContent(tfsRequestContext, bundle);
        return fileStream != null ? (ActionResult) new FileStreamResult(fileStream, contentType) : (ActionResult) this.HttpNotFound("Bundle not found");
      }
      catch (ArgumentException ex)
      {
        throw new HttpException(400, ex.Message, (Exception) ex);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(Location = OutputCacheLocation.Client, Duration = 31536000, VaryByParam = "*")]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public ActionResult DynamicBundles(
      string v,
      string scripts = null,
      string excludePaths = null,
      string exclude = null,
      string includeCss = null,
      string excludeCss = null,
      bool lwp = false)
    {
      this.AddCorsHeader();
      WebContext webContext = WebContextFactory.GetWebContext(this.HttpContext.Request.RequestContext);
      IEnumerable<string> cssModulePrefixes = (IEnumerable<string>) WebContextFactory.GetContributedServiceContext(this.HttpContext.Request.RequestContext).CssModulePrefixes;
      try
      {
        HashSet<string> setFromParam = BundlingHelper.GetSetFromParam(excludePaths);
        if (lwp)
          setFromParam.UnionWith(BundlingHelper.ExcludedNewPlatformPaths);
        return (ActionResult) new RestApiJsonResult((object) BundlingHelper.RegisterDynamicBundles(webContext, "async", (ISet<string>) BundlingHelper.GetSetFromParam(scripts), (ISet<string>) BundlingHelper.GetSetFromParam(exclude), (ISet<string>) BundlingHelper.GetSetFromParam(includeCss), (ISet<string>) BundlingHelper.GetSetFromParam(excludeCss), (IEnumerable<string>) setFromParam, cssModulePrefixes, v));
      }
      catch (ArgumentException ex)
      {
        throw new HttpException(400, ex.Message, (Exception) ex);
      }
      catch (PathTooLongException ex)
      {
        throw new HttpException(400, ex.Message, (Exception) ex);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    private void AddCorsHeader()
    {
      if (this.Response.Headers["Access-Control-Allow-Origin"] != null)
        return;
      this.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    }
  }
}
