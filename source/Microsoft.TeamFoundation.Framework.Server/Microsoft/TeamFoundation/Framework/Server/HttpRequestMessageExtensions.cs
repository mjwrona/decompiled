// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpRequestMessageExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HttpRequestMessageExtensions
  {
    private static readonly bool s_isHosted = false;
    private const string c_methodOverrideHeader = "X-HTTP-Method-Override";
    private static readonly IReadOnlyCollection<string> s_overrideMethods = (IReadOnlyCollection<string>) new string[4]
    {
      "DELETE",
      "OPTIONS",
      "PATCH",
      "PUT"
    };

    static HttpRequestMessageExtensions()
    {
      try
      {
        string appSetting = WebConfigurationManager.AppSettings["IsHosted"];
        HttpRequestMessageExtensions.s_isHosted = !string.IsNullOrEmpty(appSetting) && string.Equals(appSetting, "true", StringComparison.OrdinalIgnoreCase);
      }
      catch
      {
      }
    }

    public static IVssRequestContext GetIVssRequestContext(this HttpRequestMessage request)
    {
      ArgumentUtility.CheckForNull<HttpRequestMessage>(request, nameof (request));
      object obj = (object) null;
      return !request.Properties.TryGetValue(TfsApiPropertyKeys.HttpContext, out obj) ? (IVssRequestContext) request.Properties[HttpContextConstants.IVssRequestContext] : (IVssRequestContext) ((HttpContextBase) obj).Items[(object) HttpContextConstants.IVssRequestContext];
    }

    public static bool TryGetHeaderGuid(HttpHeaders headers, string key, out Guid value)
    {
      value = Guid.Empty;
      IEnumerable<string> values;
      return headers.TryGetValues(key, out values) && Guid.TryParse(values.FirstOrDefault<string>(), out value);
    }

    public static string GetSimpleHeaderValue(this HttpRequestMessage request, string name)
    {
      IEnumerable<string> values = (IEnumerable<string>) null;
      return !string.IsNullOrEmpty(name) && request.Headers.TryGetValues(name, out values) ? values.FirstOrDefault<string>() : (string) null;
    }

    public static IDictionary<string, string> GetSimpleNonEmptyHeaders(
      this HttpRequestMessage request)
    {
      Dictionary<string, string> simpleNonEmptyHeaders = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Headers)
      {
        if (!simpleNonEmptyHeaders.ContainsKey(header.Key))
        {
          string str = header.Value.FirstOrDefault<string>();
          if (!string.IsNullOrEmpty(str))
            simpleNonEmptyHeaders.Add(header.Key, str);
        }
      }
      return (IDictionary<string, string>) simpleNonEmptyHeaders;
    }

    public static string ReadBodyAsString(
      this HttpRequestMessage httpRequest,
      int maxPostRequestSize)
    {
      return HttpRequestExtensions.ReadBodyAsString(httpRequest.Content.ReadAsStreamAsync().SyncResult<Stream>(), maxPostRequestSize);
    }

    public static HttpResponseMessage CreateErrorResponse(
      this HttpRequestMessage request,
      Exception exception,
      IHttpController controller)
    {
      ArgumentUtility.CheckForNull<Exception>(exception, nameof (exception));
      ArgumentUtility.CheckForNull<IHttpController>(controller, nameof (controller));
      return request.CreateErrorResponse(exception, controller, new CancellationToken(), out Exception _);
    }

    public static HttpResponseMessage CreateErrorResponse(
      this HttpRequestMessage request,
      HttpStatusCode statusCode,
      Exception exception,
      IHttpController controller)
    {
      ArgumentUtility.CheckForNull<Exception>(exception, nameof (exception));
      ArgumentUtility.CheckForNull<IHttpController>(controller, nameof (controller));
      return HttpRequestMessageExtensions.CreateErrorResponseImpl(request, statusCode, exception, controller, new CancellationToken(), out Exception _);
    }

    public static HttpResponseMessage CreateErrorResponse(
      this HttpRequestMessage request,
      Exception exception,
      IHttpController controller,
      CancellationToken cancellationToken,
      out Exception finalException)
    {
      ArgumentUtility.CheckForNull<Exception>(exception, nameof (exception));
      ArgumentUtility.CheckForNull<IHttpController>(controller, nameof (controller));
      HttpStatusCode statusCode = HttpRequestMessageExtensions.MapStatusCode(exception, controller);
      return HttpRequestMessageExtensions.CreateErrorResponseImpl(request, statusCode, exception, controller, cancellationToken, out finalException);
    }

    internal static HttpResponseMessage CreateErrorResponse(
      HttpRequestMessage request,
      HttpStatusCode statusCode,
      Exception exception)
    {
      ArgumentUtility.CheckForNull<Exception>(exception, nameof (exception));
      return HttpRequestMessageExtensions.CreateErrorResponseImpl(request, statusCode, exception, (IHttpController) null, new CancellationToken(), out Exception _);
    }

    private static HttpResponseMessage CreateErrorResponseImpl(
      HttpRequestMessage request,
      HttpStatusCode statusCode,
      Exception exception,
      IHttpController controller,
      CancellationToken cancellationToken,
      out Exception finalException)
    {
      bool includeErrorDetail = request.ShouldIncludeErrorDetail();
      Guid activityId = Guid.Empty;
      Guid empty = Guid.Empty;
      finalException = exception;
      if (HttpRequestMessageExtensions.s_isHosted)
        includeErrorDetail = request.GetIVssRequestContext()?.ExecutionEnvironment.IsDevFabricDeployment.GetValueOrDefault();
      if (controller is TfsApiController tfsApiController && tfsApiController.TfsRequestContext != null && !(exception is HttpException))
      {
        cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, tfsApiController.TfsRequestContext.CancellationToken).Token;
        finalException = HttpRequestMessageExtensions.TranslateException(exception, controller, cancellationToken);
        activityId = tfsApiController.TfsRequestContext.ActivityId;
        Guid uniqueIdentifier = tfsApiController.TfsRequestContext.UniqueIdentifier;
        if (!(finalException is UnauthorizedRequestException))
          tfsApiController.TfsRequestContext.TraceException(103200, tfsApiController.TraceArea, tfsApiController.ControllerContext.ControllerDescriptor.ControllerName, finalException);
        if (tfsApiController.TfsRequestContext.Status == null || tfsApiController.TfsRequestContext.Status is System.OperationCanceledException)
          tfsApiController.TfsRequestContext.Status = finalException;
        finalException = tfsApiController.TranslateException(finalException);
      }
      WrappedException wrappedException = new WrappedException(finalException, includeErrorDetail, request.GetApiVersionSafe());
      if (HttpRequestMessageExtensions.s_isHosted && !includeErrorDetail)
        wrappedException.Message = UserFriendlyError.GetMessageFromException(finalException, statusCode, activityId);
      return request.CreateResponse<WrappedException>(statusCode, wrappedException);
    }

    private static HttpStatusCode MapStatusCode(Exception exception, IHttpController controller)
    {
      if (exception is HttpException httpException)
        return (HttpStatusCode) httpException.GetHttpCode();
      if (exception is HttpResponseException responseException && responseException.Response != null)
        return responseException.Response.StatusCode;
      return controller is TfsApiController tfsApiController ? tfsApiController.MapException(exception) : HttpStatusCode.InternalServerError;
    }

    private static Exception TranslateException(
      Exception exception,
      IHttpController controller,
      CancellationToken cancellationToken)
    {
      System.OperationCanceledException innerException = exception as System.OperationCanceledException;
      if (cancellationToken.IsCancellationRequested && innerException != null)
        exception = (Exception) new RequestCanceledException(FrameworkResources.ClientDisconnectedCancelReason(), (Exception) innerException);
      return exception;
    }

    internal static string GetQueryStringValue(this HttpRequestMessage request, string key)
    {
      try
      {
        return request.GetQueryNameValuePairs().FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (pair => string.Equals(pair.Key, key, StringComparison.OrdinalIgnoreCase))).Value;
      }
      catch (ArgumentException ex)
      {
        throw new InvalidQueryStringException(ex.Message, (Exception) ex);
      }
    }

    private static string GetHttpMethodFromMethodOverrideHeader(
      string httpMethod,
      string headerValue)
    {
      return (httpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) || httpMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase)) && headerValue != null && HttpRequestMessageExtensions.s_overrideMethods.Contains<string>(headerValue, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? headerValue.ToUpperInvariant() : (string) null;
    }

    internal static string GetHttpMethodFromMethodOverrideHeader(
      this HttpRequestBase request,
      string requestHttpMethod = null)
    {
      string httpMethod = requestHttpMethod ?? request.HttpMethod;
      string[] values = request.Headers.GetValues("X-HTTP-Method-Override");
      string headerValue = values != null ? ((IEnumerable<string>) values).FirstOrDefault<string>() : (string) null;
      return HttpRequestMessageExtensions.GetHttpMethodFromMethodOverrideHeader(httpMethod, headerValue);
    }

    internal static string GetHttpMethodFromMethodOverrideHeader(this HttpRequestMessage request)
    {
      string methodOverrideHeader = (string) null;
      IEnumerable<string> values;
      if (request.Headers.TryGetValues("X-HTTP-Method-Override", out values))
        methodOverrideHeader = HttpRequestMessageExtensions.GetHttpMethodFromMethodOverrideHeader(request.Method.Method, values.FirstOrDefault<string>());
      return methodOverrideHeader;
    }
  }
}
