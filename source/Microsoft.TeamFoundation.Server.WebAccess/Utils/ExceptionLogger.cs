// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Utils.ExceptionLogger
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.Utils
{
  internal class ExceptionLogger
  {
    private readonly Action<IVssRequestContext, string, Exception, int, EventLogEntryType> logInEventLog;
    private readonly Action<int, TraceLevel, string, string, string, Exception> logExceptionRawInTeamFoundation;

    public ExceptionLogger(
      Action<IVssRequestContext, string, Exception, int, EventLogEntryType> logInEventLog,
      Action<int, TraceLevel, string, string, string, Exception> logExceptionRawInTeamFoundation)
    {
      this.logInEventLog = logInEventLog;
      this.logExceptionRawInTeamFoundation = logExceptionRawInTeamFoundation;
    }

    public void LogException(HttpContextBase httpContext, Exception exception)
    {
      bool flag = true;
      int num1 = TeamFoundationEventId.DefaultExceptionEventId;
      switch (exception)
      {
        case TeamFoundationServiceException _:
          TeamFoundationServiceException serviceException = exception as TeamFoundationServiceException;
          flag = serviceException.LogException;
          num1 = serviceException.EventId;
          break;
        case SqlException _:
          num1 = TeamFoundationEventId.UnexpectedDatabaseResultException;
          break;
        case ArgumentException _:
        case HttpException _:
        case NotSupportedException _:
        case UnauthorizedAccessException _:
        case AccessCheckException _:
          flag = false;
          break;
      }
      IVssRequestContext requestContext = httpContext.TfsRequestContext();
      if (requestContext != null)
      {
        requestContext.Status = exception;
        int num2;
        switch (exception)
        {
          case UnauthorizedRequestException _:
          case AccessCheckException _:
            num2 = 2;
            break;
          default:
            num2 = 1;
            break;
        }
        TraceLevel level = (TraceLevel) num2;
        requestContext.TraceException(520010, level, "WebAccess", TfsTraceLayers.Controller, exception);
        if (!flag)
          return;
        this.logInEventLog(requestContext, FrameworkResources.UnhandledExceptionError(), exception, num1, EventLogEntryType.Error);
      }
      else
      {
        if (!flag)
          return;
        this.logExceptionRawInTeamFoundation(599999, TraceLevel.Info, "WebAccess", TfsTraceLayers.Controller, "WebAccessModule", exception);
      }
    }
  }
}
