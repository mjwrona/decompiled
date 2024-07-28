// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.RestApiJsonResult
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class RestApiJsonResult : SecureJsonResult
  {
    private static readonly VssJsonMediaTypeFormatter s_defaultFormatter = new VssJsonMediaTypeFormatter();
    private static readonly VssJsonMediaTypeFormatter s_jsonIslandFormatter = new VssJsonMediaTypeFormatter();

    static RestApiJsonResult() => RestApiJsonResult.s_jsonIslandFormatter.SerializerSettings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;

    public RestApiJsonResult() => this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

    public RestApiJsonResult(object data)
      : this()
    {
      this.Data = data;
    }

    public static string SerializeRestApiData(
      IVssRequestContext requestContext,
      object data,
      RestApiJsonResult.Destination destination = RestApiJsonResult.Destination.Default)
    {
      try
      {
        return JsonConvert.SerializeObject(data, Formatting.None, RestApiJsonResult.GetRestApiJsonSerializerSettings(destination));
      }
      catch (JsonSerializationException ex)
      {
        requestContext.TraceException(520061, "WebAccess", TfsTraceLayers.Framework, (Exception) ex);
        if (ex.InnerException != null && ex.InnerException.GetType() == typeof (RequestCanceledException))
          throw ex.InnerException;
        throw;
      }
    }

    private static JsonSerializerSettings GetRestApiJsonSerializerSettings(
      RestApiJsonResult.Destination destination = RestApiJsonResult.Destination.Default)
    {
      return destination == RestApiJsonResult.Destination.JsonIsland ? RestApiJsonResult.s_jsonIslandFormatter.SerializerSettings : RestApiJsonResult.s_defaultFormatter.SerializerSettings;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && StringComparer.OrdinalIgnoreCase.Equals(context.HttpContext.Request.HttpMethod, "GET"))
        throw new InvalidOperationException("Json get is not allowed");
      HttpResponseBase response = context.HttpContext.Response;
      response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;
      if (this.ContentEncoding != null)
      {
        response.ContentEncoding = this.ContentEncoding;
      }
      else
      {
        response.Charset = "utf-8";
        response.ContentEncoding = Encoding.UTF8;
      }
      if (this.Data == null)
        return;
      object secureData = this.GetSecureData();
      using (TextWriter textWriter = (TextWriter) new StreamWriter(response.OutputStream))
        JsonSerializer.Create(RestApiJsonResult.GetRestApiJsonSerializerSettings()).Serialize(textWriter, secureData);
    }

    public enum Destination
    {
      Default,
      JsonIsland,
    }
  }
}
