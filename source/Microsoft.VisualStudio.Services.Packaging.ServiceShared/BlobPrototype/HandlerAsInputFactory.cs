// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.HandlerAsInputFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class HandlerAsInputFactory
  {
    public static IFactory<TIn, IAsyncHandler<TIn, TOut>> Create<TIn, TOut>(
      IAsyncHandler<TIn, TOut> returnAlways)
      where TIn : class
    {
      return (IFactory<TIn, IAsyncHandler<TIn, TOut>>) new HandlerAsInputFactory.Impl<TIn, TOut>(returnAlways);
    }

    private class Impl<TIn, TOut> : IFactory<TIn, IAsyncHandler<TIn, TOut>> where TIn : class
    {
      private readonly IAsyncHandler<TIn, TOut> returnAlways;

      public Impl(IAsyncHandler<TIn, TOut> returnAlways) => this.returnAlways = returnAlways;

      public IAsyncHandler<TIn, TOut> Get(TIn input) => this.returnAlways;
    }
  }
}
