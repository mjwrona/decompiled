// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.RequestHandler
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  public abstract class RequestHandler
  {
    internal readonly string FullHandlerName;

    public RequestHandler InnerHandler { get; set; }

    protected RequestHandler() => this.FullHandlerName = this.GetType().FullName;

    public virtual async Task<ResponseMessage> SendAsync(
      RequestMessage request,
      CancellationToken cancellationToken)
    {
      if (this.InnerHandler == null)
        throw new ArgumentNullException("InnerHandler");
      ITrace trace = request.Trace;
      ITrace childTrace = request.Trace.StartChild(this.InnerHandler.FullHandlerName, TraceComponent.RequestHandler, TraceLevel.Info);
      ResponseMessage responseMessage;
      try
      {
        request.Trace = childTrace;
        responseMessage = await this.InnerHandler.SendAsync(request, cancellationToken);
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
