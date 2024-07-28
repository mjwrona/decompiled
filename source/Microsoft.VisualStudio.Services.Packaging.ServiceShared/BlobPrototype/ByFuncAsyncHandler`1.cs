// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ByFuncAsyncHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class ByFuncAsyncHandler<TIn> : 
    IAsyncHandler<TIn>,
    IAsyncHandler<TIn, NullResult>,
    IHaveInputType<TIn>,
    IHaveOutputType<NullResult>
  {
    private readonly Action<TIn> action;

    public ByFuncAsyncHandler(Action<TIn> action) => this.action = action;

    public Task<NullResult> Handle(TIn request)
    {
      this.action(request);
      return NullResult.NullTask;
    }
  }
}
