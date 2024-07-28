// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.AccountHomeController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Presentation;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Globalization;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  [SupportedRouteArea(NavigationContextLevels.Application | NavigationContextLevels.Collection)]
  public class AccountHomeController : AccountAreaController
  {
    private const string IdeSignedPath = "/_signedin?protocol=javascriptnotify";
    private const string ChromelessHeaderKey = "__chromeless";

    [TfsTraceFilter(504611, 504620)]
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult CreateProject(string compact, string scenario = null)
    {
      this.EnsureCurrentUserHasGlobalReadAccess();
      if (!this.IsCompact(compact))
      {
        string createNewProjectUrl = ProjectCreationUrlHelper.GetCreateNewProjectUrl(this.TfsWebContext, this.TfsRequestContext);
        if (!string.IsNullOrEmpty(createNewProjectUrl))
          return (ActionResult) this.Redirect(createNewProjectUrl);
      }
      CreateProjectViewModel model = new CreateProjectViewModel()
      {
        NextUrl = this.GetNextUrl(compact),
        IsCompact = this.IsCompact(compact),
        Scenario = scenario
      };
      if (this.IsCompact(compact))
        this.ViewData["__chromeless"] = (object) true;
      this.ViewData["IsReponsiveLayout"] = (object) true;
      return (ActionResult) this.View((object) model);
    }

    private void EnsureCurrentUserHasGlobalReadAccess()
    {
      if (!this.TfsWebContext.CurrentUserHasGlobalReadAccess)
      {
        InvalidAccessException invalidAccessException = new InvalidAccessException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, AccountServerResources.ErrorApplicationLevelAccessDenied, (object) this.TfsRequestContext.AuthenticatedUserName));
        invalidAccessException.ReportException = false;
        throw invalidAccessException;
      }
    }

    private bool IsCompact(string compact) => compact != null && compact == "1";

    private string GetNextUrl(string compact)
    {
      if (!this.IsCompact(compact))
        return string.Empty;
      ArgumentUtility.CheckForNull<IVssRequestContext>(this.TfsRequestContext, "request context");
      return this.TfsRequestContext.GetService<ILocationService>().DetermineAccessMapping(this.TfsRequestContext).AccessPoint + "/_signedin?protocol=javascriptnotify";
    }
  }
}
