// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ApiControllerExtension
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public static class ApiControllerExtension
  {
    public static HttpResponseMessage CreateResponse<T>(
      this ApiController apiController,
      T obj,
      HttpStatusCode httpStatusCode = HttpStatusCode.OK)
    {
      ContentNegotiationResult negotiationResult = apiController.Configuration.Services.GetContentNegotiator().Negotiate(typeof (T), apiController.Request, (IEnumerable<MediaTypeFormatter>) apiController.Configuration.Formatters);
      MediaTypeFormatter formatter = negotiationResult.Formatter;
      string mediaType = negotiationResult.MediaType.MediaType;
      return new HttpResponseMessage()
      {
        Content = (HttpContent) new ObjectContent<T>(obj, formatter),
        StatusCode = httpStatusCode
      };
    }

    public static HttpResponseMessage CreateResponse(
      this ApiController apiController,
      HttpStatusCode httpStatusCode = HttpStatusCode.OK)
    {
      return new HttpResponseMessage()
      {
        StatusCode = httpStatusCode,
        RequestMessage = apiController.Request
      };
    }
  }
}
