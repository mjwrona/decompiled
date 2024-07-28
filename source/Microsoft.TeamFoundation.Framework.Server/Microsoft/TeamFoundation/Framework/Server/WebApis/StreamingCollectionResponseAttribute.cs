// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebApis.StreamingCollectionResponseAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server.WebApis
{
  public class StreamingCollectionResponseAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      HttpResponseMessage response = actionExecutedContext.Response;
      if (response == null)
        return;
      ObjectContent responseContent = response.Content as ObjectContent;
      IEnumerator items = ((IEnumerable) responseContent.Value).GetEnumerator();
      bool hasItems = items.MoveNext();
      TfsApiController tfsApiController = actionExecutedContext.ActionContext.ControllerContext.Controller as TfsApiController;
      response.Content = (HttpContent) new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          MediaTypeFormatter formatter = responseContent.Formatter;
          Encoding encoding = formatter.SelectCharacterEncoding(httpContent.Headers);
          using (StreamWriter streamWriter = new StreamWriter(stream, encoding, 1024, true))
          {
            streamWriter.Write("{ \"value\": [ ");
            streamWriter.Flush();
            bool flag = true;
            Type type = (Type) null;
            int num = 0;
            if (hasItems)
            {
              do
              {
                if (flag)
                {
                  flag = false;
                  type = items.Current == null ? typeof (object) : items.Current.GetType();
                  if (tfsApiController != null)
                    tfsApiController.TfsRequestContext.UpdateTimeToFirstPage();
                }
                else
                {
                  streamWriter.Write(",");
                  streamWriter.Flush();
                }
                formatter.WriteToStreamAsync(type, items.Current, stream, httpContent, transportContext);
                ++num;
              }
              while (items.MoveNext());
            }
            streamWriter.Write("], \"count\": " + num.ToString() + " }");
            streamWriter.Flush();
          }
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue("application/json"));
    }
  }
}
