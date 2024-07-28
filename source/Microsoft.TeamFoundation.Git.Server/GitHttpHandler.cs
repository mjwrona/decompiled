// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitHttpHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Routing;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  public abstract class GitHttpHandler : TeamFoundationHttpHandler
  {
    internal const AuthenticationMechanisms DefaultAuthenticationMechanisms = AuthenticationMechanisms.Basic | AuthenticationMechanisms.Windows | AuthenticationMechanisms.Federated | AuthenticationMechanisms.OAuth;
    private bool m_methodEntered;
    private static readonly Dictionary<Type, Func<Exception, HttpStatusCode>> s_exceptionResponseMap = new Dictionary<Type, Func<Exception, HttpStatusCode>>()
    {
      {
        typeof (GitRepositoryNotFoundException),
        (Func<Exception, HttpStatusCode>) (ex => HttpStatusCode.NotFound)
      },
      {
        typeof (ProjectDoesNotExistWithNameException),
        (Func<Exception, HttpStatusCode>) (ex => HttpStatusCode.NotFound)
      },
      {
        typeof (GitNeedsPermissionException),
        (Func<Exception, HttpStatusCode>) (ex => HttpStatusCode.Forbidden)
      },
      {
        typeof (GitUploadPackDisabledException),
        (Func<Exception, HttpStatusCode>) (ex => HttpStatusCode.Forbidden)
      },
      {
        typeof (GitProtocolException),
        (Func<Exception, HttpStatusCode>) (ex => HttpStatusCode.BadRequest)
      },
      {
        typeof (RequestBlockedException),
        (Func<Exception, HttpStatusCode>) (ex => ((TeamFoundationServiceException) ex).HttpStatusCode)
      },
      {
        typeof (GitRefFavoriteNotFoundException),
        (Func<Exception, HttpStatusCode>) (ex => HttpStatusCode.NotFound)
      }
    };

    protected GitHttpHandler() => this.Initialize();

    protected GitHttpHandler(HttpContextBase context)
      : base(context)
    {
      this.Initialize();
    }

    internal abstract void Execute();

    protected abstract string Layer { get; }

    protected override sealed void ProcessRequestImpl(HttpContext context)
    {
      try
      {
        this.Execute();
      }
      finally
      {
        if (!this.m_methodEntered)
        {
          try
          {
            this.EnterMethod(new MethodInformation(this.Layer + "_NeverEntered", MethodType.Normal, EstimatedMethodCost.Moderate));
          }
          catch (Exception ex)
          {
            this.RequestContext.TraceCatch(1013660, GitServerUtils.TraceArea, this.Layer, ex);
          }
        }
        this.LeaveMethod();
      }
    }

    private void Initialize() => this.RequestContext.ServiceName = "Git";

    protected bool HandleException(Exception ex, bool tryReportAction, Action<string> reportAction = null)
    {
      this.RequestContext.TraceException(1013860, GitServerUtils.TraceArea, nameof (GitHttpHandler), ex);
      if (!this.IsHandleableException(ex))
        return false;
      HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
      string message = ex.Message;
      Func<Exception, HttpStatusCode> func;
      if (GitHttpHandler.s_exceptionResponseMap.TryGetValue(ex.GetType(), out func))
        statusCode = func(ex);
      else if (ex is CircuitBreakerException)
      {
        statusCode = HttpStatusCode.ServiceUnavailable;
        message = Resources.Get("ServerUnavailableMessage");
        ex = (Exception) new GitServiceUnavailableException(message, ex);
      }
      else if (ex is HttpException)
        ex = (Exception) new RequestCanceledException(FrameworkResources.RequestCanceledErrorWithReason((object) ex.Message), HttpStatusCode.RequestTimeout);
      if (tryReportAction)
      {
        try
        {
          if (reportAction != null)
            reportAction(message);
        }
        catch (Exception ex1)
        {
          if (!this.IsHandleableException(ex1))
            throw;
        }
      }
      this.HandleException(ex, "X-TFS-Exception", (int) statusCode, false);
      return true;
    }

    protected static string GetRouteValue(HttpContextBase context, string key) => context.Request.RequestContext.RouteData.GetRouteValue<string>(key);

    internal RequestedProtocolVersion GetProtocolVersion()
    {
      string inputString = (string) null;
      string header = this.HandlerHttpContext.Request.Headers["Git-Protocol"];
      if (header != null)
      {
        string str1 = header;
        char[] chArray = new char[1]{ ':' };
        foreach (string str2 in str1.Split(chArray))
        {
          if (str2.Substring(0, "version=".Length).Equals("version=", StringComparison.Ordinal))
            inputString = str2.Substring("version=".Length);
        }
      }
      return new RequestedProtocolVersion(inputString);
    }

    private bool IsHandleableException(Exception exception)
    {
      switch (exception)
      {
        case OutOfMemoryException _:
        case StackOverflowException _:
        case ThreadAbortException _:
          return false;
        default:
          return true;
      }
    }

    protected override void EnterMethod(MethodInformation methodInfo)
    {
      base.EnterMethod(methodInfo);
      this.m_methodEntered = true;
    }
  }
}
