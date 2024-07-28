// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.ChooseFirstNonNullInputFactory`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns
{
  public class ChooseFirstNonNullInputFactory<TIn, TOut> : IFactory<TIn, TOut>
  {
    private readonly IFactory<TIn, TOut>[] factories;

    public ChooseFirstNonNullInputFactory(params IFactory<TIn, TOut>[] factories) => this.factories = factories;

    public TOut Get(TIn input) => ((IEnumerable<IFactory<TIn, TOut>>) this.factories).Select<IFactory<TIn, TOut>, TOut>((Func<IFactory<TIn, TOut>, TOut>) (f => f.Get(input))).FirstOrDefault<TOut>((Func<TOut, bool>) (instance => (object) instance != null));
  }
}
