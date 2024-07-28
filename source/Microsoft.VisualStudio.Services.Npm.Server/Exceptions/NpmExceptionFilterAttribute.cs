// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Exceptions.NpmExceptionFilterAttribute
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Configuration;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Npm.Server.Exceptions
{
  internal class NpmExceptionFilterAttribute : ExceptionFilterAttribute
  {
    private static readonly bool IsHosted;

    private JsonMediaTypeFormatter JsonFormatter
    {
      get
      {
        JsonMediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
        jsonFormatter.SerializerSettings.ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver();
        return jsonFormatter;
      }
    }

    static NpmExceptionFilterAttribute()
    {
      try
      {
        string appSetting = WebConfigurationManager.AppSettings[nameof (IsHosted)];
        NpmExceptionFilterAttribute.IsHosted = !string.IsNullOrEmpty(appSetting) && string.Equals(appSetting, "true", StringComparison.OrdinalIgnoreCase);
      }
      catch
      {
      }
    }

    public override void OnException(HttpActionExecutedContext actionExecutedContext)
    {
      TfsApiController controller = actionExecutedContext.ActionContext.ControllerContext.Controller as TfsApiController;
      Exception exception = actionExecutedContext.Exception;
      if (exception != null)
      {
        Guid activityId = Guid.Empty;
        HttpStatusCode httpStatusCode;
        if (exception is HttpException)
          httpStatusCode = (HttpStatusCode) ((HttpException) exception).GetHttpCode();
        else if (controller != null)
        {
          if (controller.TfsRequestContext != null)
          {
            activityId = controller.TfsRequestContext.ActivityId;
            controller.TfsRequestContext.TraceException(103200, controller.TraceArea, actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName, exception);
          }
          httpStatusCode = controller.MapException(exception);
          exception = controller.TranslateException(exception);
          actionExecutedContext.Exception = exception;
        }
        else
          httpStatusCode = HttpStatusCode.InternalServerError;
        HttpRequestMessage request = actionExecutedContext.Request;
        NpmResponseMessage npmResponseMessage = NpmResponseMessage.FromWrappedException(this.WrapException(exception, request.ShouldIncludeErrorDetail(), this.IsInternalError(httpStatusCode), activityId, request.Version), httpStatusCode);
        actionExecutedContext.Response = request.CreateResponse<NpmResponseMessage>(httpStatusCode, npmResponseMessage, (MediaTypeFormatter) this.JsonFormatter);
        if (actionExecutedContext.Response == null || actionExecutedContext.ActionContext == null || actionExecutedContext.ActionContext.ControllerContext == null || actionExecutedContext.ActionContext.ControllerContext.Controller == null || controller == null || controller.TfsRequestContext == null || actionExecutedContext.Response.Headers == null)
          return;
        string str1 = controller.TfsRequestContext.ActivityId.ToString("D");
        IEnumerable<string> values1;
        if (!actionExecutedContext.Response.Headers.Contains("ActivityId") || !actionExecutedContext.Response.Headers.TryGetValues("ActivityId", out values1) || values1 == null || !values1.Contains<string>(str1))
          actionExecutedContext.Response.Headers.Add("ActivityId", str1);
        string str2 = controller.TfsRequestContext.UniqueIdentifier.ToString("D");
        IEnumerable<string> values2;
        if (actionExecutedContext.Response.Headers.Contains("X-TFS-Session") && actionExecutedContext.Response.Headers.TryGetValues("X-TFS-Session", out values2) && values2 != null && values2.Contains<string>(str2))
          return;
        actionExecutedContext.Response.Headers.Add("X-TFS-Session", str2);
      }
      else
        base.OnException(actionExecutedContext);
    }

    private WrappedException WrapException(
      Exception exception,
      bool includeErrorDetails,
      bool isInternalError,
      Guid activityId,
      Version version)
    {
      if (NpmExceptionFilterAttribute.IsHosted)
        includeErrorDetails = false;
      WrappedException wrappedException = new WrappedException(exception, includeErrorDetails, version);
      if (isInternalError && NpmExceptionFilterAttribute.IsHosted && !includeErrorDetails)
        wrappedException.Message = UserFriendlyError.GetMessageFromException(exception, activityId: activityId);
      return wrappedException;
    }

    private bool IsInternalError(HttpStatusCode responseCode) => (int) responseCode / 100 == 5;
  }
}
