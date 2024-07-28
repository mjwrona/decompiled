// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns.ChooseSingleNonNullResultConverter`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns
{
  public class ChooseSingleNonNullResultConverter<TIn, TOut> : 
    IConverter<TIn, TOut>,
    IHaveInputType<TIn>,
    IHaveOutputType<TOut>
  {
    private readonly IFactory<Exception> onFailureExceptionFactory;
    private readonly IConverter<TIn, TOut>[] converters;

    public ChooseSingleNonNullResultConverter(
      IFactory<Exception> onFailureExceptionFactory,
      params IConverter<TIn, TOut>[] converters)
    {
      this.onFailureExceptionFactory = onFailureExceptionFactory;
      this.converters = converters;
    }

    public TOut Convert(TIn input)
    {
      List<TOut> source = new List<TOut>();
      foreach (IConverter<TIn, TOut> converter in this.converters)
      {
        TOut @out = converter.Convert(input);
        if ((object) @out != null)
          source.Add(@out);
      }
      return source.Count == 1 ? source.Single<TOut>() : throw this.onFailureExceptionFactory.Get();
    }
  }
}
