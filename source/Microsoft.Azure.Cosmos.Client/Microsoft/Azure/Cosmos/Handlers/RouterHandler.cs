// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Handlers.RouterHandler
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Handlers
{
  internal class RouterHandler : RequestHandler
  {
    private readonly RequestHandler documentFeedHandler;
    private readonly RequestHandler pointOperationHandler;

    public RouterHandler(RequestHandler documentFeedHandler, RequestHandler pointOperationHandler)
    {
      this.documentFeedHandler = documentFeedHandler ?? throw new ArgumentNullException(nameof (documentFeedHandler));
      this.pointOperationHandler = pointOperationHandler ?? throw new ArgumentNullException(nameof (pointOperationHandler));
    }

    public override async Task<ResponseMessage> SendAsync(
      RequestMessage request,
      CancellationToken cancellationToken)
    {
      RequestHandler requestHandler = request.IsPartitionKeyRangeHandlerRequired ? this.documentFeedHandler : this.pointOperationHandler;
      ITrace trace = request.Trace;
      ITrace childTrace = request.Trace.StartChild(requestHandler.FullHandlerName, TraceComponent.RequestHandler, TraceLevel.Info);
      ResponseMessage responseMessage;
      try
      {
        request.Trace = childTrace;
        responseMessage = await requestHandler.SendAsync(request, cancellationToken);
      }
      finally
      {
        childTrace.Dispose();
        request.Trace = trace;
      }
      trace = (ITrace) null;
      childTrace = (ITrace) null;
      return responseMessage;
    }
  }
}
