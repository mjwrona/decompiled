// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.UserController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  [SupportedRouteArea(NavigationContextLevels.Application | NavigationContextLevels.Collection)]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("65AC9DB3-BB0A-42fe-B584-A690FB0D817B", true)]
  public class UserController : AccountAreaController
  {
    private const string Featureflag = "WebAccess.UserManagement";

    [TfsTraceFilter(504251, 504260)]
    [TfsHandleFeatureFlag("WebAccess.UserManagement", null)]
    [HttpGet]
    public ActionResult Index(string id)
    {
      IVssRequestContext vssRequestContext1 = this.TfsRequestContext.Elevate().To(TeamFoundationHostType.Deployment);
      bool flag1 = this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && this.TfsRequestContext.IsFeatureEnabled("VisualStudio.UserManagement.Web.UserHub");
      string spsAccountUrl = !this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment || flag1 ? vssRequestContext1.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext1, this.TfsRequestContext.ServiceHost.InstanceId, ServiceInstanceTypes.TFS).AbsoluteUri : this.TfsRequestContext.GetService<ILocationService>().GetLocationServiceUrl(this.TfsRequestContext, ServiceInstanceTypes.SPS, AccessMappingConstants.PublicAccessMappingMoniker);
      if (!spsAccountUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
        spsAccountUrl += "/";
      if (flag1)
        this.Response.Redirect(this.GetNewUsersUrl(id, spsAccountUrl));
      this.ViewData["SpsUsersUrl"] = (object) this.GetUsersUrl(id, spsAccountUrl);
      this.ViewData["SpsAccountUrl"] = (object) spsAccountUrl;
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return (ActionResult) this.View();
      SecurityModel securityModel = new SecurityModel();
      IVssRequestContext vssRequestContext2 = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      bool flag2 = vssRequestContext2.Elevate().GetService<IdentityService>().IsMember(vssRequestContext2, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, this.TfsRequestContext.UserContext);
      if (!flag2)
      {
        TeamFoundationHostManagementService service = vssRequestContext1.GetService<TeamFoundationHostManagementService>();
        Guid id1 = (vssRequestContext2.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(vssRequestContext2.Elevate(), (IList<Guid>) null, ServiceHostFilterFlags.None).Last<TeamProjectCollectionProperties>() ?? throw new ArgumentException(string.Format(string.Empty))).Id;
        IVssRequestContext requestContext = vssRequestContext1;
        Guid instanceId = id1;
        using (IVssRequestContext vssRequestContext3 = service.BeginRequest(requestContext, instanceId, RequestContextType.SystemContext, true, false))
          flag2 = vssRequestContext3.GetService<TeamFoundationIdentityService>().IsMember(vssRequestContext3, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, vssRequestContext3.UserContext);
      }
      securityModel.isAdmin = flag2;
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
        this.ViewData["PermissionsData"] = (object) JsonConvert.SerializeObject((object) securityModel.ToJson());
      else
        this.ViewData["PermissionsData"] = (object) new JavaScriptSerializer().Serialize((object) securityModel.ToJson());
      return (ActionResult) this.View();
    }

    private string GetUsersUrl(string id, string spsAccountUrl) => this.CreateUsersUrl(id, spsAccountUrl, AccountServerResources.spsUserhub);

    private string GetNewUsersUrl(string id, string spsAccountUrl) => this.CreateUsersUrl(id, spsAccountUrl, AccountServerResources.spsAexUserhub);

    private string CreateUsersUrl(string id, string spsAccountUrl, string userhubQuery)
    {
      string uri = string.Format("{0}{1}", (object) spsAccountUrl, (object) userhubQuery);
      if (id.IsNullOrEmpty<char>())
        return uri;
      UriBuilder uriBuilder = new UriBuilder(uri);
      NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
      queryString[nameof (id)] = id;
      uriBuilder.Query = queryString.ToString();
      return uriBuilder.ToString();
    }
  }
}
