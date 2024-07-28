// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.PropagateNullHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns
{
  public class PropagateNullHandler<TIn, TOut> : 
    IAsyncHandler<TIn, TOut>,
    IHaveInputType<TIn>,
    IHaveOutputType<TOut>
    where TOut : class
  {
    private readonly IAsyncHandler<TIn, TOut> ifNonNullHandler;

    public PropagateNullHandler(IAsyncHandler<TIn, TOut> ifNonNullHandler) => this.ifNonNullHandler = ifNonNullHandler;

    public async Task<TOut> Handle(TIn request) => (object) request == null ? default (TOut) : await this.ifNonNullHandler.Handle(request);
  }
}
