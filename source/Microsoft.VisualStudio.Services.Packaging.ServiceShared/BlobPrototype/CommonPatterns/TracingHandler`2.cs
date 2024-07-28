// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns.TracingHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns
{
  public abstract class TracingHandler<TReq, TRes> : 
    IAsyncHandler<TReq, TRes>,
    IHaveInputType<TReq>,
    IHaveOutputType<TRes>
  {
    private readonly ITracerService tracerService;

    protected TracingHandler(ITracerService tracerService) => this.tracerService = tracerService;

    public async Task<TRes> Handle(TReq request)
    {
      TracingHandler<TReq, TRes> sendInTheThisObject = this;
      TRes res;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
        res = await sendInTheThisObject.Handle(request, tracer);
      return res;
    }

    public abstract Task<TRes> Handle(TReq request, ITracerBlock tracer);
  }
}
