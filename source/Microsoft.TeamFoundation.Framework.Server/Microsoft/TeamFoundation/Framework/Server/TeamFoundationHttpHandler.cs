// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationHttpHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class TeamFoundationHttpHandler : IHttpHandler
  {
    private VisualStudioServicesApplication m_teamFoundationApplication;
    private IVssRequestContext m_requestContext;
    private MethodInformation m_methodInformation;
    private const string c_area = "TeamFoundationHttpHandler";
    private const string c_layer = "HttpHandler";

    protected TeamFoundationHttpHandler()
      : this((HttpContextBase) new HttpContextWrapper(HttpContext.Current))
    {
    }

    protected TeamFoundationHttpHandler(HttpContextBase httpContext)
    {
      this.HandlerHttpContext = httpContext;
      this.m_teamFoundationApplication = httpContext.ApplicationInstance as VisualStudioServicesApplication;
      this.m_requestContext = this.m_teamFoundationApplication.VssRequestContext;
      this.m_requestContext.ServiceName = this.GetType().Name;
      if (!this.AllowSimplePostRequests && StringComparer.Ordinal.Equals(httpContext.Request.HttpMethod, "POST"))
      {
        string x = httpContext.Request.ContentType;
        if (x != null)
        {
          int length = x.IndexOf(';');
          if (length >= 0)
            x = x.Substring(0, length);
        }
        if (string.IsNullOrEmpty(x) || StringComparer.OrdinalIgnoreCase.Equals(x, "application/x-www-form-urlencoded") || StringComparer.OrdinalIgnoreCase.Equals(x, "multipart/form-data") || StringComparer.OrdinalIgnoreCase.Equals(x, "text/plain"))
          throw new InvalidAccessException(FrameworkResources.InvalidAccessException());
      }
      if (!this.m_requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && this.m_requestContext.ServiceHost.HasDatabaseAccess)
        this.m_requestContext.CheckOnPremisesDeployment(true);
      if ((this.m_requestContext.WebRequestContextInternal().RequestRestrictions.AllowedHandlers & AllowedHandler.TfsHttpHandler) == AllowedHandler.None)
        throw new InvalidAccessException(FrameworkResources.InvalidAccessException());
    }

    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
      this.m_requestContext.RequestTimer.SetPreControllerTime();
      ((IEnumerable<RequestRestrictionsAttribute>) this.GetType().GetCustomAttributes(typeof (RequestRestrictionsAttribute), true)).ApplyRequestRestrictions(this.m_requestContext, (IDictionary<string, object>) context.Request.RequestContext.RouteData.Values);
      this.m_requestContext.ValidateIdentity();
      this.ProcessRequestImpl(context);
      this.m_requestContext.RequestTimer.SetControllerTime();
    }

    protected abstract void ProcessRequestImpl(HttpContext context);

    protected virtual bool AllowSimplePostRequests => false;

    protected IVssRequestContext RequestContext => this.m_requestContext;

    protected HttpContextBase HandlerHttpContext { get; private set; }

    protected virtual void EnterMethod(MethodInformation methodInformation)
    {
      if (this.m_requestContext == null)
        return;
      this.m_methodInformation = methodInformation;
      this.m_requestContext.EnterMethod(methodInformation);
    }

    protected virtual void LeaveMethod()
    {
      if (this.m_requestContext == null)
        return;
      this.m_requestContext.LeaveMethod();
    }

    protected virtual Exception HandleException(
      Exception exception,
      string exceptionHeader,
      int statusCode,
      bool closeResponse)
    {
      try
      {
        if (this.HandlerHttpContext != null)
        {
          try
          {
            this.HandlerHttpContext.Response.StatusCode = statusCode;
            if (!string.IsNullOrEmpty(exceptionHeader))
              this.HandlerHttpContext.Response.AddHeader(exceptionHeader, exception.GetType().Name);
            this.HandlerHttpContext.Response.TrySkipIisCustomErrors = true;
            this.HandlerHttpContext.Response.ContentType = "text/plain";
            this.HandlerHttpContext.Response.Write(UserFriendlyError.GetMessageFromException(exception));
          }
          catch (Exception ex)
          {
            closeResponse = false;
            this.HandlerHttpContext.Response.Close();
          }
        }
        bool flag = true;
        int eventId = TeamFoundationEventId.DefaultExceptionEventId;
        if (this.m_requestContext != null)
        {
          this.m_requestContext.Status = exception;
          this.m_requestContext.TraceException(7533, TraceLevel.Warning, nameof (TeamFoundationHttpHandler), "HttpHandler", exception);
        }
        else
          TeamFoundationTracingService.TraceExceptionRaw(7533, TraceLevel.Warning, nameof (TeamFoundationHttpHandler), "HttpHandler", exception);
        switch (exception)
        {
          case TeamFoundationServiceException _:
            TeamFoundationServiceException serviceException = exception as TeamFoundationServiceException;
            flag = serviceException.LogException;
            eventId = serviceException.EventId;
            break;
          case ThreadAbortException _:
            flag = false;
            break;
          case SqlException _:
            eventId = TeamFoundationEventId.UnexpectedDatabaseResultException;
            break;
          case ArgumentException _:
          case HttpException _:
          case NotSupportedException _:
          case SecurityException _:
          case UnauthorizedAccessException _:
            flag = false;
            break;
        }
        if (flag)
        {
          TeamFoundationEventLog.Default.LogException(this.m_requestContext, FrameworkResources.UnhandledExceptionError(), exception, eventId, EventLogEntryType.Error);
          if (this.m_requestContext != null)
            this.m_requestContext.TraceException(7535, nameof (TeamFoundationHttpHandler), "HttpHandler", exception);
        }
      }
      catch (Exception ex)
      {
        if (this.m_requestContext != null)
          this.m_requestContext.TraceException(7537, nameof (TeamFoundationHttpHandler), "HttpHandler", ex);
        throw;
      }
      finally
      {
        if (closeResponse && this.HandlerHttpContext != null)
          this.HandlerHttpContext.Response.Close();
      }
      return exception;
    }
  }
}
