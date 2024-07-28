// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution.NoAggregationRequiredReturnSameInstanceBootstrapper`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution
{
  public class NoAggregationRequiredReturnSameInstanceBootstrapper<TRequest, TResponse> : 
    IRequireAggHandlerBootstrapper<TRequest, TResponse>,
    IHaveInputType<TRequest>,
    IHaveOutputType<TResponse>,
    IRequireAggBootstrapper<IAsyncHandler<TRequest, TResponse>>
    where TRequest : IFeedRequest
  {
    private readonly IAsyncHandler<TRequest, TResponse> handler;

    public NoAggregationRequiredReturnSameInstanceBootstrapper(
      IAsyncHandler<TRequest, TResponse> handler)
    {
      this.handler = handler;
    }

    public IAsyncHandler<TRequest, TResponse>? Bootstrap(
      IReadOnlyCollection<IAggregationAccessor> aggTypeProvider)
    {
      return this.handler;
    }

    public IEnumerable<string> GetAggregationNamesForDiagnostics() => Enumerable.Empty<string>();
  }
}
