// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.ConverterExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns
{
  public static class ConverterExtensions
  {
    public static IAsyncHandler<TIn, TOut> AsAsyncHandler<TIn, TOut>(
      this IConverter<TIn, TOut> converter)
    {
      return (IAsyncHandler<TIn, TOut>) new ConverterAsAsyncHandler<TIn, TOut>(converter);
    }

    public static IConverter<TIn, (TIn In, TOut Out)> KeepInput<TIn, TOut>(
      this IConverter<TIn, TOut> converter)
    {
      return (IConverter<TIn, (TIn, TOut)>) new ByFuncConverter<TIn, (TIn, TOut)>((Func<TIn, (TIn, TOut)>) (input => (input, converter.Convert(input))));
    }

    public static IConverter<TIn, TOut> Select<TIn, TIntermediate, TOut>(
      this IConverter<TIn, TIntermediate> converter,
      Func<TIntermediate, TOut> selectionFunc)
    {
      return (IConverter<TIn, TOut>) new ByFuncConverter<TIn, TOut>((Func<TIn, TOut>) (input => selectionFunc(converter.Convert(input))));
    }

    public static IConverter<TIn, TOut> ValidateResultWith<TIn, TOut, TVal>(
      this IConverter<TIn, TOut> innerConverter,
      IValidator<TVal> validator)
      where TOut : TVal
    {
      return (IConverter<TIn, TOut>) new ConverterExtensions.ResultValidatingConverter<TIn, TOut, TVal>(innerConverter, validator);
    }

    private class ResultValidatingConverter<TIn, TOut, TVal> : 
      IConverter<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
      where TOut : TVal
    {
      private readonly IConverter<TIn, TOut> innerConverter;
      private readonly IValidator<TVal> validator;

      public ResultValidatingConverter(
        IConverter<TIn, TOut> innerConverter,
        IValidator<TVal> validator)
      {
        this.innerConverter = innerConverter;
        this.validator = validator;
      }

      public TOut Convert(TIn input)
      {
        TOut valueToValidate = this.innerConverter.Convert(input);
        this.validator.Validate((TVal) valueToValidate);
        return valueToValidate;
      }
    }
  }
}
