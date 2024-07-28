// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.ApiWebSettingsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using System.Diagnostics;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.ApplicationAll)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiWebSettingsController : TfsController
  {
    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(506000, 506010)]
    public ActionResult SetInteger(string path, int value, WebSettingsScope scope = WebSettingsScope.User)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      this.Trace(506001, TraceLevel.Verbose, "SetInteger: path:{0}  value: {1}, scope: {2}", (object) path, (object) value, (object) scope);
      this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, "Microsoft.TeamFoundation.WebAccess", "StoreSetting", path, (double) value);
      using (ISettingsProvider webSettings = this.GetWebSettings(scope))
        webSettings.SetSetting<int>(path, value);
      return (ActionResult) this.Json((object) new
      {
        success = true
      }, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(506020, 506030)]
    public ActionResult SetBoolean(string path, bool value, WebSettingsScope scope = WebSettingsScope.User)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      this.Trace(506025, TraceLevel.Verbose, "SetBoolean: path:{0}  value: {1}, scope: {2}", (object) path, (object) value, (object) scope);
      this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, "Microsoft.TeamFoundation.WebAccess", "StoreSetting", path, value);
      using (ISettingsProvider webSettings = this.GetWebSettings(scope))
        webSettings.SetSetting<bool>(path, value);
      return (ActionResult) this.Json((object) new
      {
        success = true
      }, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(506040, 506050)]
    [ValidateInput(false)]
    public ActionResult SetString(string path, string value, WebSettingsScope scope = WebSettingsScope.User)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      this.Trace(506041, TraceLevel.Verbose, "SetString: path:{0}  value: {1}, scope: {2}", (object) path, (object) value, (object) scope);
      using (ISettingsProvider webSettings = this.GetWebSettings(scope))
        webSettings.SetSetting<string>(path, value);
      return (ActionResult) this.Json((object) new
      {
        success = true
      }, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(506060, 506070)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult ReadSetting(string path, WebSettingsScope scope = WebSettingsScope.User)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      this.Trace(506061, TraceLevel.Verbose, "ReadSetting: path:{0}, scope: {1}", (object) path, (object) scope);
      string setting;
      using (ISettingsProvider webSettings = this.GetWebSettings(scope))
        setting = webSettings.GetSetting<string>(path, string.Empty);
      return (ActionResult) this.Json((object) new
      {
        value = setting
      }, JsonRequestBehavior.AllowGet);
    }

    private ISettingsProvider GetWebSettings(WebSettingsScope scope) => WebSettings.GetWebSettings(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid, this.TfsWebContext.Team, scope);
  }
}
