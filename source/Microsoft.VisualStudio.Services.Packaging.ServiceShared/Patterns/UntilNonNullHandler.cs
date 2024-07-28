// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.UntilNonNullHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns
{
  public static class UntilNonNullHandler
  {
    public static IAsyncHandler<TIn, TOut> Create<TIn, TOut>(
      params IAsyncHandler<TIn, TOut>[] handlers)
    {
      return (IAsyncHandler<TIn, TOut>) new UntilNonNullHandler.Impl<TIn, TOut>((IEnumerable<IAsyncHandler<TIn, TOut>>) handlers);
    }

    private class Impl<TIn, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private readonly IReadOnlyList<IAsyncHandler<TIn, TOut>> handlers;

      public Impl(IEnumerable<IAsyncHandler<TIn, TOut>> handlers) => this.handlers = (IReadOnlyList<IAsyncHandler<TIn, TOut>>) handlers.Where<IAsyncHandler<TIn, TOut>>((Func<IAsyncHandler<TIn, TOut>, bool>) (x => x != null)).ToList<IAsyncHandler<TIn, TOut>>();

      public async Task<TOut> Handle(TIn request)
      {
        foreach (IAsyncHandler<TIn, TOut> handler in (IEnumerable<IAsyncHandler<TIn, TOut>>) this.handlers)
        {
          TOut @out = await handler.Handle(request);
          if ((object) @out != null)
            return @out;
        }
        return default (TOut);
      }
    }
  }
}
