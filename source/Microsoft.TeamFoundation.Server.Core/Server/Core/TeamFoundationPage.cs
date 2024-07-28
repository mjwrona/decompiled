// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationPage
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security;
using System.Web;
using System.Web.UI;

namespace Microsoft.TeamFoundation.Server.Core
{
  public abstract class TeamFoundationPage : Page
  {
    private TeamFoundationApplication m_teamFoundationApplication;
    private IVssRequestContext m_requestContext;
    private MethodInformation m_methodInformation;
    private static readonly string s_area = "WebAccess";
    private static readonly string s_layer = "TeamFoundationWebPage";

    protected TeamFoundationPage()
    {
      this.m_teamFoundationApplication = HttpContext.Current.ApplicationInstance as TeamFoundationApplication;
      this.m_requestContext = this.m_teamFoundationApplication.VssRequestContext;
      this.m_requestContext.ValidateIdentity();
      this.m_requestContext.ServiceName = this.GetType().Name;
      this.Page.Init += new EventHandler(this.Page_Init);
    }

    protected IVssRequestContext RequestContext => this.m_requestContext;

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

    protected virtual Exception HandleException(Exception exception)
    {
      bool flag = true;
      int eventId = TeamFoundationEventId.DefaultExceptionEventId;
      TeamFoundationTrace.TraceException(exception);
      if (this.m_requestContext != null)
      {
        this.m_requestContext.Status = exception;
        this.m_requestContext.TraceException(7521, TraceLevel.Warning, TeamFoundationPage.s_area, TeamFoundationPage.s_layer, exception);
      }
      switch (exception)
      {
        case TeamFoundationServiceException _:
          TeamFoundationServiceException serviceException = exception as TeamFoundationServiceException;
          flag = serviceException.LogException;
          eventId = serviceException.EventId;
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
          this.m_requestContext.TraceException(7524, TeamFoundationPage.s_area, TeamFoundationPage.s_layer, exception);
      }
      return exception;
    }

    private void Page_Init(object sender, EventArgs e)
    {
      if (!this.User.Identity.IsAuthenticated)
        return;
      this.ViewStateUserKey = this.User.Identity.Name;
    }
  }
}
