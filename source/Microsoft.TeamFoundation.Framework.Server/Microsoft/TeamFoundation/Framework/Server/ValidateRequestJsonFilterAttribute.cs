// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ValidateRequestJsonFilterAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ValidateRequestJsonFilterAttribute : ActionFilterAttribute
  {
    private readonly string[] s_mediaTypesToCheck = new string[2]
    {
      "application/json",
      "application/json-patch+json"
    };
    private const string c_area = "Microsoft.VisualStudio.Services.RestApi";
    private const string c_layer = "ValidateRequestJsonFilter";
    private const int c_defaultBufferSize = 1024;

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      IVssRequestContext ivssRequestContext = actionContext.Request.GetIVssRequestContext();
      try
      {
        Version apiVersion = actionContext.Request.GetApiVersion();
        if (apiVersion != (Version) null)
        {
          if (apiVersion < new Version(5, 1))
            return;
        }
      }
      catch (VssInvalidApiResourceVersionException ex)
      {
      }
      if (actionContext.Request.Content?.Headers?.ContentType == null || !actionContext.Request.Content.Headers.ContentLength.HasValue)
        return;
      long? contentLength = actionContext.Request.Content.Headers.ContentLength;
      long num = 0;
      object obj;
      if (contentLength.GetValueOrDefault() <= num & contentLength.HasValue || !((IEnumerable<string>) this.s_mediaTypesToCheck).Any<string>((Func<string, bool>) (m => m.Equals(actionContext.Request.Content.Headers.ContentType.MediaType, StringComparison.OrdinalIgnoreCase))) || actionContext.ControllerContext.Controller.GetType().GetCustomAttributes<ValidateModelAttribute>(true).Any<ValidateModelAttribute>() || !actionContext.Request.Properties.TryGetValue(TfsApiPropertyKeys.HttpContext, out obj) || !(obj is HttpContextBase httpContextBase))
        return;
      if (httpContextBase is HttpRequestMessageContextWrapper)
        return;
      try
      {
        this.ValidateJson(actionContext.Request, httpContextBase.Request.InputStream);
      }
      catch (JsonReaderException ex)
      {
        actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, (Exception) ex, actionContext.ControllerContext.Controller);
      }
      catch (Exception ex)
      {
        this.TraceError(actionContext, ivssRequestContext, ex);
      }
    }

    private void ValidateJson(HttpRequestMessage request, Stream stream)
    {
      stream.Seek(0L, SeekOrigin.Begin);
      Stack<JsonToken> jsonTokenStack = new Stack<JsonToken>();
      using (request.GetIVssRequestContext().CreateTimeToFirstPageExclusionBlock())
      {
        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true))
        {
          using (JsonTextReader jsonTextReader = new JsonTextReader((TextReader) reader))
          {
            while (jsonTextReader.Read())
            {
              if (jsonTextReader.TokenType == JsonToken.StartObject || jsonTextReader.TokenType == JsonToken.StartArray)
                jsonTokenStack.Push(jsonTextReader.TokenType);
              else if (jsonTextReader.TokenType == JsonToken.EndObject || jsonTextReader.TokenType == JsonToken.EndArray)
              {
                int num = (int) jsonTokenStack.Pop();
              }
            }
            if (jsonTokenStack.Count > 0)
              throw new JsonReaderException(string.Format("Unterminated JsonType {0}.  Path '{1}', line {2}, position {3}.", (object) (jsonTokenStack.Peek() == JsonToken.StartObject ? "Object" : "Array"), (object) jsonTextReader.Path, (object) jsonTextReader.LineNumber, (object) jsonTextReader.LinePosition), jsonTextReader.Path, jsonTextReader.LineNumber, jsonTextReader.LinePosition, (Exception) null);
          }
        }
      }
    }

    private void TraceError(
      HttpActionContext actionContext,
      IVssRequestContext requestContext,
      Exception ex)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        ["action"] = actionContext.ActionDescriptor.ActionName,
        ["controller"] = actionContext.ControllerContext.Controller.GetType().FullName,
        ["exceptionType"] = ex.GetType().Name,
        ["exceptionMessage"] = ex.Message,
        ["fullException"] = ex.ToReadableStackTrace()
      };
      if (actionContext.ActionDescriptor is ReflectedHttpActionDescriptor actionDescriptor)
        dictionary["methodInfo"] = actionDescriptor.MethodInfo.ToString();
      string format = JsonConvert.SerializeObject((object) dictionary);
      requestContext.TraceAlways(214051748, TraceLevel.Warning, "Microsoft.VisualStudio.Services.RestApi", "ValidateRequestJsonFilter", format);
    }
  }
}
