// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssExceptionHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssExceptionHandler : IExceptionHandler
  {
    public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
    {
      if (!context.CatchBlock.IsTopLevel)
        return Task.CompletedTask;
      if (context.CatchBlock.Name == "HttpWebRoute" && (context.Exception is InvalidQueryStringException || context.Exception is VssVersionOutOfRangeException))
      {
        context.Result = (IHttpActionResult) new ResponseMessageResult(HttpRequestMessageExtensions.CreateErrorResponse(context.Request, HttpStatusCode.BadRequest, context.Exception));
      }
      else
      {
        HttpContextBase current = HttpContextFactory.Current;
        if ((current != null ? (current.Items.Contains((object) HttpContextConstants.SecurityTrackingException) ? 1 : 0) : 0) != 0)
        {
          context.Result = (IHttpActionResult) new ResponseMessageResult(HttpRequestMessageExtensions.CreateErrorResponse(context.Request, HttpStatusCode.InternalServerError, context.Exception));
        }
        else
        {
          HttpStatusCode statusCode = context.Exception is TeamFoundationServiceException exception ? exception.HttpStatusCode : HttpStatusCode.InternalServerError;
          context.Result = (IHttpActionResult) new ResponseMessageResult(HttpRequestMessageExtensions.CreateErrorResponse(context.Request, statusCode, context.Exception));
        }
      }
      return Task.CompletedTask;
    }
  }
}
