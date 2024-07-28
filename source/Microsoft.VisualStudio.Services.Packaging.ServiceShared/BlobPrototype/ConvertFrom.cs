// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ConvertFrom
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class ConvertFrom
  {
    public static ConvertFrom.IConvertFrom<T> Type<T>() => ConvertFrom.ConvertFromHolder<T>.Instance;

    public static ConvertFrom.IConvertFrom<T> TypeOf<T>(T obj) => ConvertFrom.ConvertFromHolder<T>.Instance;

    public static ConvertFrom.IConvertFrom<T> TypeOfExpression<T>(Expression<Func<T>> expression) => ConvertFrom.ConvertFromHolder<T>.Instance;

    public static ConvertFrom.IConvertFrom<TOut> OutputTypeOf<TOut>(IHaveOutputType<TOut> handler) => ConvertFrom.ConvertFromHolder<TOut>.Instance;

    public static ConvertFrom.IConvertFrom<TOut> OutputItemTypeOf<TOut>(
      IHaveOutputType<IEnumerable<TOut>> handler)
    {
      return ConvertFrom.ConvertFromHolder<TOut>.Instance;
    }

    public static ConvertFrom.IConvertFrom<TIn> InputTypeOf<TIn>(IHaveInputType<TIn> handler) => ConvertFrom.ConvertFromHolder<TIn>.Instance;

    public static ConvertFrom.IConvertFrom<TIn> InputTypeOf<TIn>(
      IBootstrapper<IHaveInputType<TIn>> bootstrapper)
    {
      return ConvertFrom.ConvertFromHolder<TIn>.Instance;
    }

    public interface IConvertFrom<TIn>
    {
      ConvertFrom.IConvertFromTo<TIn, TOut> To<TOut>();
    }

    public interface IConvertFromTo<TIn, TOut>
    {
    }

    private class ConvertFromHolder<TIn> : ConvertFrom.IConvertFrom<TIn>
    {
      public static readonly ConvertFrom.IConvertFrom<TIn> Instance = (ConvertFrom.IConvertFrom<TIn>) new ConvertFrom.ConvertFromHolder<TIn>();

      public ConvertFrom.IConvertFromTo<TIn, TOut> To<TOut>() => (ConvertFrom.IConvertFromTo<TIn, TOut>) null;
    }
  }
}
