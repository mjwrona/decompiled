// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.PermaLinkController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  public class PermaLinkController : TfsController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(507000, 507010)]
    [ValidateInput(false)]
    public ActionResult RedirectRoute(string relativePath)
    {
      this.Trace(507001, TraceLevel.Verbose, "Redirect permalink route: {0}", (object) relativePath);
      TfsLocator fromPairs = TfsLocator.CreateFromPairs(this.Request.Url, this.Request.Params);
      fromPairs.InitializeHostId(this.TfsWebContext);
      bool disposeContext = false;
      IVssRequestContext vssRequestContext = (IVssRequestContext) null;
      try
      {
        vssRequestContext = this.GetCollectionContext(fromPairs.CollectionGuid, out disposeContext);
        ProjectInfo projectInfo = fromPairs.GetProjectInfo(vssRequestContext);
        WebApiTeam webApiTeam = (WebApiTeam) null;
        if (projectInfo != null)
          webApiTeam = fromPairs.GetTeam(vssRequestContext, projectInfo.Id);
        StringBuilder stringBuilder = new StringBuilder(TfsLocator.GetCollectionUrl(vssRequestContext));
        if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] != '/')
          stringBuilder.Append('/');
        if (projectInfo != null)
        {
          stringBuilder.Append(projectInfo.Name);
          stringBuilder.Append('/');
          if (webApiTeam != null)
          {
            stringBuilder.Append(webApiTeam.Name);
            stringBuilder.Append('/');
          }
        }
        if (!string.IsNullOrEmpty(relativePath))
          stringBuilder.Append(relativePath);
        NameValueCollection nameValueCollection = TfsLocator.FilterQueryString(this.Request.Url.Query);
        if (nameValueCollection.Count > 0)
        {
          stringBuilder.Append('?');
          stringBuilder.Append(nameValueCollection.ToString());
        }
        return (ActionResult) this.Redirect(stringBuilder.ToString());
      }
      finally
      {
        if (disposeContext && vssRequestContext != null)
          vssRequestContext.Dispose();
      }
    }

    protected override void OnException(ExceptionContext filterContext)
    {
      if (filterContext != null && filterContext.Exception != null && filterContext.Exception is UnauthorizedAccessException)
        throw new HttpException(404, filterContext.Exception.Message);
      base.OnException(filterContext);
    }

    private IVssRequestContext GetCollectionContext(Guid collectionId, out bool disposeContext)
    {
      if (this.TfsRequestContext.ServiceHost.InstanceId == collectionId)
      {
        disposeContext = false;
        return this.TfsRequestContext;
      }
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      TeamFoundationHostManagementService service = this.TfsRequestContext.GetService<TeamFoundationHostManagementService>();
      disposeContext = true;
      IVssRequestContext requestContext = vssRequestContext;
      Guid instanceId = collectionId;
      IdentityDescriptor userContext = vssRequestContext.UserContext;
      return service.BeginUserRequest(requestContext, instanceId, userContext, false);
    }
  }
}
