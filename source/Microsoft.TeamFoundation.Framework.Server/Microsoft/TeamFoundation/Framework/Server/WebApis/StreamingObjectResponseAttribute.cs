// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebApis.StreamingObjectResponseAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server.WebApis
{
  public class StreamingObjectResponseAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      HttpResponseMessage response = actionExecutedContext.Response;
      if (response == null)
        return;
      ObjectContent responseContent = response.Content as ObjectContent;
      object serializedObject = responseContent.Value;
      if (serializedObject == null)
        throw new ArgumentNullException("actionExecutedContext.Response.Value");
      IHttpController controller = actionExecutedContext.ActionContext.ControllerContext.Controller;
      response.Content = (HttpContent) new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          MediaTypeFormatter formatter = responseContent.Formatter;
          Encoding encoding = formatter.SelectCharacterEncoding(httpContent.Headers);
          using (new StreamWriter(stream, encoding, 1024, true))
            formatter.WriteToStreamAsync(serializedObject.GetType(), serializedObject, stream, httpContent, transportContext);
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue("application/json"));
    }
  }
}
