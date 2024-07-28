// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ByFuncAsyncHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class ByFuncAsyncHandler
  {
    public static ByFuncAsyncHandler<TIn, TOut> For<TIn, TOut>(Func<TIn, TOut> func) => new ByFuncAsyncHandler<TIn, TOut>(func);

    public static ByFuncAsyncHandler<TIn> For<TIn>(Action<TIn> action) => new ByFuncAsyncHandler<TIn>(action);

    public static ByFuncAsyncHandler<T, T> Identity<T>() => ByFuncAsyncHandler.For<T, T>((Func<T, T>) (a => a));
  }
}
