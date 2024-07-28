// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.ByFuncPerCollectionBootstrapper`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns
{
  public class ByFuncPerCollectionBootstrapper<TBootstrapped> : 
    IPerCollectionBootstrapper<TBootstrapped>
    where TBootstrapped : class
  {
    private readonly Func<IVssRequestContext, TBootstrapped> func;

    public ByFuncPerCollectionBootstrapper(Func<IVssRequestContext, TBootstrapped> func) => this.func = func;

    public TBootstrapped Bootstrap(IVssRequestContext collectionRequestContext) => this.func(collectionRequestContext);
  }
}
