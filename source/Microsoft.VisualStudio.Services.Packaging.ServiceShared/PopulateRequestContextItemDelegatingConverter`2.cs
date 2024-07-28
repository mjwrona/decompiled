// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PopulateRequestContextItemDelegatingConverter`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class PopulateRequestContextItemDelegatingConverter<TIn, TOut> : 
    IConverter<TIn, TOut>,
    IHaveInputType<TIn>,
    IHaveOutputType<TOut>
  {
    private readonly IVssRequestContext requestContext;
    private readonly Func<TIn, TOut, string> requestItemKeyGenerator;
    private readonly IConverter<TIn, TOut> delegatedConverter;

    public PopulateRequestContextItemDelegatingConverter(
      IVssRequestContext requestContext,
      string requestItemKey,
      IConverter<TIn, TOut> delegatedConverter)
      : this(requestContext, (Func<TIn, TOut, string>) ((tIn, tOut) => requestItemKey), delegatedConverter)
    {
    }

    public PopulateRequestContextItemDelegatingConverter(
      IVssRequestContext requestContext,
      Func<TIn, TOut, string> requestItemKeyGenerator,
      IConverter<TIn, TOut> delegatedConverter)
    {
      this.requestContext = requestContext;
      this.requestItemKeyGenerator = requestItemKeyGenerator;
      this.delegatedConverter = delegatedConverter;
    }

    public TOut Convert(TIn input)
    {
      TOut @out = this.delegatedConverter.Convert(input);
      this.requestContext.Items[this.requestItemKeyGenerator(input, @out)] = (object) @out;
      return @out;
    }
  }
}
