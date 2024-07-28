// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.UserHubController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Admin", NavigationContextLevels.Collection | NavigationContextLevels.Project)]
  [DemandFeature("65AC9DB3-BB0A-42fe-B584-A690FB0D817B", true)]
  public class UserHubController : AdminAreaController
  {
    private const string Featureflag = "WebAccess.UserManagement";
    private const string s_userhubPath = "/_admin/_userHub";
    private const string s_spsUserhub = "_users";
    private const string disableCommerceFeatureFlag = "VisualStudio.Services.Commerce.DisableOnPremCommerce";

    [TfsTraceFilter(504251, 504260)]
    [TfsHandleFeatureFlag("WebAccess.UserManagement", null)]
    [HttpGet]
    public ActionResult Index(bool synchronizeCommerceData = false)
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment && this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableOnPremCommerce"))
        return (ActionResult) this.HttpNotFound();
      ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
      AccessMapping accessMapping = service.DetermineAccessMapping(this.TfsRequestContext);
      this.ViewData["SpsUsersUrl"] = (object) this.GetUsersUrl(Regex.Replace(service.LocationForAccessMapping(this.TfsRequestContext, this.TfsRequestContext.RelativeUrl(), RelativeToSetting.Context, accessMapping), "/_admin/_userHub", "/", RegexOptions.IgnoreCase), synchronizeCommerceData);
      SecurityModel securityModel = this.CreateSecurityModel(new Guid?(), (string) null, (string) null, out SecurityNamespacePermissionsManager _, new bool?());
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
        this.ViewData["PermissionsData"] = (object) JsonConvert.SerializeObject((object) securityModel.ToJson());
      else
        this.ViewData["PermissionsData"] = (object) new JavaScriptSerializer().Serialize((object) securityModel.ToJson());
      return (ActionResult) this.View();
    }

    private string GetUsersUrl(string spsAccountUrl, bool synchronizeCommerceData)
    {
      UriBuilder uriBuilder = new UriBuilder(spsAccountUrl);
      NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
      queryString.Clear();
      queryString["mkt"] = CultureInfo.CurrentCulture.ToString();
      uriBuilder.Path = uriBuilder.Path.EndsWith("/") ? uriBuilder.Path + "_users" : uriBuilder.Path + "/_users";
      if (synchronizeCommerceData)
        queryString[nameof (synchronizeCommerceData)] = synchronizeCommerceData.ToString();
      uriBuilder.Query = queryString.ToString();
      return uriBuilder.ToString();
    }
  }
}
