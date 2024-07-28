// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ApiProcessConfigurationController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [SupportedRouteArea("Api", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiProcessConfigurationController : AgileAreaController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetProcessSettings()
    {
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      TfsWebContext tfsWebContext = this.TfsWebContext;
      string name = tfsWebContext.Project.Name;
      string uri = tfsWebContext.Project.Uri;
      return (ActionResult) this.Json((object) AgileDataSource.GetProcessSettings(tfsRequestContext, name, uri), JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetProcessSettingsProperty(string propertyName) => (ActionResult) this.Json((object) AgileDataSource.GetProcessSettingsProperty(this.TfsRequestContext, this.TfsWebContext.Project.Uri, propertyName), JsonRequestBehavior.AllowGet);
  }
}
