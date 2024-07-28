// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.UsersContributionPreloadHandler
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using Newtonsoft.Json.Linq;
using System;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class UsersContributionPreloadHandler : 
    IPreExecuteContributedRequestHandler,
    IContributedRequestHandler
  {
    private const string FastContextSwitchParam = "_xhr";
    private const string HandlerName = "ms.vss-aex-user-management-web.collection-user-management-hub-handler";
    private const string OldUserHubUrl = "~/_user";
    private const string TraceArea = "Account";
    private const string TraceLayer = "UsersContributionPreloadHandler";

    public string Name => "ms.vss-aex-user-management-web.collection-user-management-hub-handler";

    public void OnPreExecute(
      IVssRequestContext requestContext,
      ActionExecutingContext actionExecutingContext,
      JObject properties)
    {
      try
      {
        if (!requestContext.IsHosted() || requestContext.IsFeatureEnabled("VisualStudio.UserManagement.Web.UserHub") || !(WebContextFactory.GetCurrentRequestWebContext<WebContext>()?.RequestContext?.HttpContext?.Request["_xhr"] != "true"))
          return;
        actionExecutingContext.Result = (ActionResult) new RedirectResult(VirtualPathUtility.ToAbsolute("~/_user"));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(504255, "Account", nameof (UsersContributionPreloadHandler), ex);
      }
    }
  }
}
