// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TfsApiController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ApiVersionValidationFilter]
  [RequestContentTypeRestriction]
  [ApiVersionResponseFilter]
  [ValidateRequestJsonFilter]
  [LeaveActivityLog]
  [VssSecurableActionFilter]
  [AccessReadConsistencyFilter(null)]
  [ControllerTimerFilter]
  [LogExceptionFilter]
  public class TfsApiController : ApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (AccessCheckException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (UnauthorizedAccessException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (InvalidAccessException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentNullException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentOutOfRangeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidOperationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (NotImplementedException),
        HttpStatusCode.NotImplemented
      },
      {
        typeof (VssResourceNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (VssInvalidApiResourceVersionException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (VssVersionNotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (VssVersionNotSpecifiedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (VssVersionOutOfRangeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (VssInvalidPreviewVersionException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (VssRequestContentTypeNotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (CircuitBreakerExceededConcurrencyException),
        HttpStatusCode.ServiceUnavailable
      },
      {
        typeof (CircuitBreakerExceededExecutionLimitException),
        HttpStatusCode.ServiceUnavailable
      },
      {
        typeof (CircuitBreakerShortCircuitException),
        HttpStatusCode.ServiceUnavailable
      },
      {
        typeof (AadThrottlingException),
        (HttpStatusCode) 429
      },
      {
        typeof (ClientRequestThrottledException),
        (HttpStatusCode) 429
      }
    };
    private ApiExceptionMapping m_exceptionMapping;

    public virtual IVssRequestContext TfsRequestContext { get; set; }

    protected override sealed void Initialize(HttpControllerContext controllerContext)
    {
      base.Initialize(controllerContext);
      this.TfsRequestContext = (IVssRequestContext) controllerContext.Request.Properties[TfsApiPropertyKeys.TfsRequestContext];
      object obj;
      if (controllerContext.RouteData != null && this.ControllerContext.RouteData.Values.TryGetValue(TfsApiPropertyKeys.TfsUseApplicationHost, out obj) && (bool) obj)
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      this.TfsRequestContext.ServiceName = this.ActivityLogArea;
      try
      {
        this.InitializeInternal(controllerContext);
      }
      catch (Exception ex) when (this.TraceException(ex))
      {
      }
    }

    protected virtual void InitializeInternal(HttpControllerContext controllerContext)
    {
    }

    private bool TraceException(Exception e)
    {
      this.TfsRequestContext.TraceException(103211, this.TraceArea, "Controller", e);
      return false;
    }

    public virtual string TraceArea => "WebApi";

    public virtual string ActivityLogArea => "Web-Api";

    public IDictionary<Type, HttpStatusCode> BaseHttpExceptions => (IDictionary<Type, HttpStatusCode>) TfsApiController.s_httpExceptions;

    public virtual IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) null;

    public ApiExceptionMapping ExceptionMapping
    {
      get
      {
        if (this.m_exceptionMapping == null)
        {
          ApiExceptionMapping exceptionMap = new ApiExceptionMapping();
          foreach (KeyValuePair<Type, HttpStatusCode> httpException in TfsApiController.s_httpExceptions)
            exceptionMap.AddStatusCode(httpException.Key, httpException.Value);
          IDictionary<Type, HttpStatusCode> httpExceptions = this.HttpExceptions;
          if (this.HttpExceptions != null)
          {
            foreach (KeyValuePair<Type, HttpStatusCode> keyValuePair in (IEnumerable<KeyValuePair<Type, HttpStatusCode>>) httpExceptions)
              exceptionMap.AddStatusCode(keyValuePair.Key, keyValuePair.Value);
          }
          Dictionary<Type, Type> dictionary = new Dictionary<Type, Type>();
          if (this.TranslatedExceptions != null)
          {
            foreach (KeyValuePair<Type, Type> translatedException in (IEnumerable<KeyValuePair<Type, Type>>) this.TranslatedExceptions)
              exceptionMap.AddTranslation(translatedException.Key, translatedException.Value);
          }
          this.InitializeExceptionMap(exceptionMap);
          this.m_exceptionMapping = exceptionMap;
        }
        return this.m_exceptionMapping;
      }
    }

    [NonAction]
    protected virtual void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
    }

    [NonAction]
    public HttpStatusCode MapException(Exception ex, HttpStatusCode defaultStatusCode = HttpStatusCode.InternalServerError)
    {
      if (ex == null)
        return defaultStatusCode;
      HttpStatusCode? nullable = this.ExceptionMapping.GetStatusCode(ex.GetType());
      if (!nullable.HasValue)
        nullable = !(ex is TeamFoundationServiceException serviceException) ? new HttpStatusCode?(defaultStatusCode) : new HttpStatusCode?(serviceException.HttpStatusCode);
      return nullable.Value;
    }

    public virtual IDictionary<Type, Type> TranslatedExceptions => (IDictionary<Type, Type>) null;

    protected internal virtual bool ExemptFromGlobalExceptionFormatting => false;

    [NonAction]
    public virtual Exception TranslateException(Exception ex) => this.ExceptionMapping.TranslateException(ex);

    [NonAction]
    protected void AddDisposableResource(IDisposable resource) => this.TfsRequestContext.AddDisposableResource(resource);
  }
}
