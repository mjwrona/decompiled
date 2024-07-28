// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ByAsyncFuncAsyncHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class ByAsyncFuncAsyncHandler<TIn, TOut> : 
    IAsyncHandler<TIn, TOut>,
    IHaveInputType<TIn>,
    IHaveOutputType<TOut>
  {
    private readonly Func<TIn, Task<TOut>> func;

    public ByAsyncFuncAsyncHandler(Func<TIn, Task<TOut>> func) => this.func = func;

    public async Task<TOut> Handle(TIn request) => await this.func(request);
  }
}
