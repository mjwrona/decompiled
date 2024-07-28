// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.NotImplementedHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class NotImplementedHandler
  {
    public static IAsyncHandler<TIn, TOut> SameHandlerTypeAs<TIn, TOut>(
      IAsyncHandler<TIn, TOut> example)
    {
      return (IAsyncHandler<TIn, TOut>) new NotImplementedHandler.Impl<TIn, TOut>();
    }

    private class Impl<TIn, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      public Task<TOut> Handle(TIn request) => throw new NotImplementedException();
    }
  }
}
