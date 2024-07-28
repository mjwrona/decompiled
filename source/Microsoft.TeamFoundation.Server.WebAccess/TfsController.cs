// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Controllers;
using Microsoft.TeamFoundation.Server.WebAccess.Utils;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [VssfMvcAuthorizationFilter(Order = 0)]
  [DemandFeature("D91355E2-2A55-4CBE-9636-4D73F70FBA7C", false, Order = 1)]
  [TfsActivityLogFilter(Order = 2)]
  [TfsAntiForgeryValidation]
  [ActivityId]
  [SecurityHeaders(true, true, true)]
  [CdnFallback]
  [MvcControllerTimerFilter]
  [WebPerformanceTimer(Order = 100)]
  public abstract class TfsController : AsyncController, ITfsController, IPlatformController
  {
    private TfsWebContext m_tfsWebContext;
    private PerformanceTimer? m_actionTimer;
    private IClientHostCapabilities m_clientHostCapabilities;
    protected bool m_executeContributedRequestHandlers;
    private const string c_TfsWebAccessRequestTag = "_IsTfsWebAccessRequest";
    private const string c_TfsControllerContextItemsKey = "_MvcTfsController";
    private ExceptionLogger exceptionLogger;

    public TfsController()
      : this(new Action<IVssRequestContext, string, Exception, int, EventLogEntryType>(TeamFoundationEventLog.Default.LogException), TfsController.\u003C\u003EO.\u003C0\u003E__TraceExceptionRaw ?? (TfsController.\u003C\u003EO.\u003C0\u003E__TraceExceptionRaw = new Action<int, TraceLevel, string, string, string, Exception>(TeamFoundationTracingService.TraceExceptionRaw)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    internal TfsController(
      Action<IVssRequestContext, string, Exception, int, EventLogEntryType> logInEventLog,
      Action<int, TraceLevel, string, string, string, Exception> logExceptionRawInTeamFoundation)
    {
      this.exceptionLogger = new ExceptionLogger(logInEventLog, logExceptionRawInTeamFoundation);
    }

    protected override void Initialize(System.Web.Routing.RequestContext requestContext)
    {
      base.Initialize(requestContext);
      IVssRequestContext vssRequestContext = requestContext.TfsRequestContext();
      if (vssRequestContext == null)
        return;
      vssRequestContext.Items["webRequestContext"] = (object) requestContext;
    }

    public void DoInitialize(System.Web.Routing.RequestContext requestContext, TfsWebContext webContext)
    {
      this.m_tfsWebContext = webContext;
      base.Initialize(requestContext);
    }

    public static bool IsCurrentRequestFromWebAccess() => System.Web.HttpContext.Current != null && new HttpContextWrapper(System.Web.HttpContext.Current).Request.RequestContext.HttpContext.Items.Contains((object) "_IsTfsWebAccessRequest");

    public static TfsController GetTfsControllerFromCurrentRequest() => System.Web.HttpContext.Current != null ? new HttpContextWrapper(System.Web.HttpContext.Current).Request.RequestContext.HttpContext.Items[(object) "_MvcTfsController"] as TfsController : (TfsController) null;

    public virtual IVssRequestContext TfsRequestContext => this.TfsWebContext.TfsRequestContext;

    public virtual TfsWebContext TfsWebContext
    {
      get
      {
        if (this.m_tfsWebContext == null)
          this.m_tfsWebContext = this.Request.RequestContext.TfsWebContext();
        return this.m_tfsWebContext;
      }
    }

    public NavigationContext NavigationContext => this.TfsWebContext.NavigationContext;

    public virtual string TraceArea => "WebAccess";

    protected override void HandleUnknownAction(string actionName) => throw new HttpException(404, WACommonResources.PageNotFound);

    public virtual string GetActivityLogCommandPrefix() => (string) null;

    public virtual void EnterMethod(MethodInformation methodInformation)
    {
      if (this.TfsWebContext == null)
        return;
      this.TfsWebContext.EnterMethod(methodInformation);
    }

    public virtual void LogException(Exception exception) => this.exceptionLogger.LogException(this.HttpContext, exception);

    public virtual void LogError(string message, int eventId) => this.LogErrorInternal(message, eventId);

    public string ClientHost => this.TfsWebContext != null ? ConfigurationContext.GetClientHost(this.TfsWebContext.RequestContext) : (string) null;

    protected IClientHostCapabilities ClientHostCapabilities
    {
      get
      {
        if (this.m_clientHostCapabilities == null)
          this.m_clientHostCapabilities = !string.IsNullOrEmpty(this.ClientHost) ? (!this.ClientHost.Equals("tee") ? (IClientHostCapabilities) new IdeClientHostCapabilities() : (IClientHostCapabilities) new IdeClientHostCapabilities()) : (IClientHostCapabilities) new BrowserClientHostCapabilities();
        return this.m_clientHostCapabilities;
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    protected virtual ActionResult ETag(
      Func<string> etagCalculator,
      Func<ActionResult> action,
      TimeSpan? timeSpan = null,
      Action<HttpContextBase> doFinally = null)
    {
      TimeSpan maxAge = timeSpan.HasValue ? timeSpan.Value : TimeSpan.Zero;
      return (ActionResult) new ETaggedResult(etagCalculator, action, maxAge, HttpCacheability.Private, doFinally);
    }

    internal static void TagRequestAsTfsWebRequest(HttpRequestBase request) => request.RequestContext.HttpContext.Items[(object) "_IsTfsWebAccessRequest"] = (object) true;

    protected override void OnActionExecuting(ActionExecutingContext ctx)
    {
      this.m_actionTimer = new PerformanceTimer?(WebPerformanceTimer.StartMeasure(ctx.RequestContext, "TfsController.ActionEndToEnd"));
      base.OnActionExecuting(ctx);
      TfsController.TagRequestAsTfsWebRequest(this.Request);
      this.Request.RequestContext.HttpContext.Items[(object) "_MvcTfsController"] = (object) this;
      foreach (KeyValuePair<string, System.Web.Mvc.ModelState> keyValuePair in this.ModelState)
      {
        ModelError modelError = keyValuePair.Value.Errors.FirstOrDefault<ModelError>();
        if (modelError != null)
        {
          Exception innerException = modelError.Exception == null ? (Exception) new InvalidArgumentValueException(keyValuePair.Key, modelError.ErrorMessage) : (!(modelError.Exception is ArgumentException) ? (Exception) new InvalidArgumentValueException(keyValuePair.Key, modelError.Exception) : modelError.Exception);
          throw new HttpException(400, innerException.Message, innerException);
        }
      }
      using (WebPerformanceTimer.StartMeasure(ctx.RequestContext, "TfsController.SetPreferredCulture"))
        TeamFoundationApplicationCore.SetPreferredCulture(this.HttpContext, this.TfsRequestContext);
      if (!this.m_executeContributedRequestHandlers)
        return;
      using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "PreRequestHandlers"))
      {
        foreach (ContributedRequestHandler<IPreExecuteContributedRequestHandler> requestHandler in this.TfsRequestContext.GetService<IContributionRoutingService>().GetRequestHandlers<IPreExecuteContributedRequestHandler>(this.TfsRequestContext))
        {
          using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "PreRequestHandler", requestHandler.Id))
            requestHandler.Handler.OnPreExecute(this.TfsRequestContext, ctx, requestHandler.Properties);
          if (ctx.Result != null)
            break;
        }
      }
    }

    protected override void OnActionExecuted(ActionExecutedContext context)
    {
      base.OnActionExecuted(context);
      this.ViewData["IsReponsiveLayout"] = (object) false;
      if (this.m_tfsWebContext != null)
      {
        this.m_tfsWebContext.NavigationContext.CommandName = this.m_tfsWebContext.TfsRequestContext.Method?.Name;
        if (!string.IsNullOrEmpty(this.m_tfsWebContext.RequestProjectName))
        {
          this.AddGlobalMessage(string.Format(WACommonResources.TeamProjectRenamedGlobalMessage, (object) this.m_tfsWebContext.RequestProjectName, (object) this.m_tfsWebContext.Project.Name));
          if (!string.IsNullOrEmpty(this.m_tfsWebContext.RequestProjectName) && Attribute.GetCustomAttribute((MemberInfo) this.GetType(), typeof (RemoveMruEntryOnRenamedProjectAttribute)) != null)
            MRUNavigationContextEntryManager.RemoveMRUNavigationContextsForProject(this.TfsRequestContext, this.m_tfsWebContext.RequestProjectName);
        }
      }
      if (!this.m_actionTimer.HasValue)
        return;
      this.m_actionTimer.Value.End();
    }

    protected override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      base.OnResultExecuted(filterContext);
      if (!this.m_executeContributedRequestHandlers)
        return;
      using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "PostRequestHandlers"))
      {
        foreach (ContributedRequestHandler<IPostExecuteContributedRequestHandler> requestHandler in this.TfsRequestContext.GetService<IContributionRoutingService>().GetRequestHandlers<IPostExecuteContributedRequestHandler>(this.TfsRequestContext))
        {
          using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "PostRequestHandler", requestHandler.Id))
            requestHandler.Handler.OnPostExecute(this.TfsRequestContext, filterContext, requestHandler.Properties);
        }
      }
    }

    protected override JsonResult Json(
      object data,
      string contentType,
      Encoding contentEncoding,
      JsonRequestBehavior behavior)
    {
      SecureJsonResult secureJsonResult = new SecureJsonResult();
      secureJsonResult.Data = data;
      secureJsonResult.ContentType = contentType;
      secureJsonResult.ContentEncoding = contentEncoding;
      secureJsonResult.JsonRequestBehavior = behavior;
      return (JsonResult) secureJsonResult;
    }

    protected JsonResult HttpStatusCodeWithMessage(HttpStatusCode httpStatusCode, string message)
    {
      this.HttpContext.Response.StatusCode = (int) httpStatusCode;
      return (JsonResult) new RestApiJsonResult((object) new
      {
        message = message
      });
    }
  }
}
