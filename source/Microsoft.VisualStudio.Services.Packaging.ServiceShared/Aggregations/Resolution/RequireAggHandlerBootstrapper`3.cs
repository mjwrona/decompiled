// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution.RequireAggHandlerBootstrapper`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution
{
  public abstract class RequireAggHandlerBootstrapper<TRequest, TResponse, TAgg1> : 
    RequireAggBootstrapper<IAsyncHandler<TRequest, TResponse>, TAgg1>,
    IRequireAggHandlerBootstrapper<TRequest, TResponse>,
    IHaveInputType<TRequest>,
    IHaveOutputType<TResponse>,
    IRequireAggBootstrapper<IAsyncHandler<TRequest, TResponse>>
    where TRequest : IFeedRequest
    where TAgg1 : class
  {
  }
}
