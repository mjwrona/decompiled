// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.ByFactoryFuncPendingAggBootstrapper`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns
{
  public class ByFactoryFuncPendingAggBootstrapper<T> : IPendingAggBootstrapper<T> where T : class
  {
    private readonly Func<IVssRequestContext, IFactory<IFeedRequest, Task<T>>> func;

    public ByFactoryFuncPendingAggBootstrapper(
      Func<IVssRequestContext, IFactory<IFeedRequest, Task<T>>> func)
    {
      this.func = func;
    }

    public async Task<T> Bootstrap(IVssRequestContext requestContext, IFeedRequest feedRequest) => await this.func(requestContext).Get(feedRequest);
  }
}
