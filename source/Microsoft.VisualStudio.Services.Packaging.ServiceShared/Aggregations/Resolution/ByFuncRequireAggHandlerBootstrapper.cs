// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution.ByFuncRequireAggHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution
{
  public static class ByFuncRequireAggHandlerBootstrapper
  {
    public static IRequireAggHandlerBootstrapper<TRequest, TResponse> For<TRequest, TResponse, TAgg1>(
      Func<TAgg1, IAsyncHandler<TRequest, TResponse>> bootstrapperFunc)
      where TRequest : IFeedRequest
      where TAgg1 : class
    {
      return (IRequireAggHandlerBootstrapper<TRequest, TResponse>) new ByFuncRequireAggHandlerBootstrapper.Impl<TRequest, TResponse, TAgg1>(bootstrapperFunc);
    }

    public static IRequireAggHandlerBootstrapper<TRequest, TResponse> For<TRequest, TResponse, TAgg1, TAgg2>(
      Func<TAgg1, TAgg2, IAsyncHandler<TRequest, TResponse>> bootstrapperFunc)
      where TRequest : IFeedRequest
      where TAgg1 : class
      where TAgg2 : class
    {
      return (IRequireAggHandlerBootstrapper<TRequest, TResponse>) new ByFuncRequireAggHandlerBootstrapper.Impl<TRequest, TResponse, TAgg1, TAgg2>(bootstrapperFunc);
    }

    public static IRequireAggHandlerBootstrapper<TRequest, TResponse> For<TRequest, TResponse, TAgg1, TAgg2, TAgg3>(
      Func<TAgg1, TAgg2, TAgg3, IAsyncHandler<TRequest, TResponse>> bootstrapperFunc)
      where TRequest : IFeedRequest
      where TAgg1 : class
      where TAgg2 : class
      where TAgg3 : class
    {
      return (IRequireAggHandlerBootstrapper<TRequest, TResponse>) new ByFuncRequireAggHandlerBootstrapper.Impl<TRequest, TResponse, TAgg1, TAgg2, TAgg3>(bootstrapperFunc);
    }

    private class Impl<TRequest, TResponse, TAgg1> : 
      RequireAggHandlerBootstrapper<TRequest, TResponse, TAgg1>
      where TRequest : IFeedRequest
      where TAgg1 : class
    {
      private readonly Func<TAgg1, IAsyncHandler<TRequest, TResponse>> bootstrapperFunc;

      public Impl(
        Func<TAgg1, IAsyncHandler<TRequest, TResponse>> bootstrapperFunc)
      {
        this.bootstrapperFunc = bootstrapperFunc;
      }

      protected override IAsyncHandler<TRequest, TResponse> Bootstrap(TAgg1 agg1) => this.bootstrapperFunc(agg1);
    }

    private class Impl<TRequest, TResponse, TAgg1, TAgg2> : 
      RequireAggHandlerBootstrapper<TRequest, TResponse, TAgg1, TAgg2>
      where TRequest : IFeedRequest
      where TAgg1 : class
      where TAgg2 : class
    {
      private readonly Func<TAgg1, TAgg2, IAsyncHandler<TRequest, TResponse>> bootstrapperFunc;

      public Impl(
        Func<TAgg1, TAgg2, IAsyncHandler<TRequest, TResponse>> bootstrapperFunc)
      {
        this.bootstrapperFunc = bootstrapperFunc;
      }

      protected override IAsyncHandler<TRequest, TResponse> Bootstrap(TAgg1 agg1, TAgg2 agg2) => this.bootstrapperFunc(agg1, agg2);
    }

    private class Impl<TRequest, TResponse, TAgg1, TAgg2, TAgg3> : 
      RequireAggHandlerBootstrapper<TRequest, TResponse, TAgg1, TAgg2, TAgg3>
      where TRequest : IFeedRequest
      where TAgg1 : class
      where TAgg2 : class
      where TAgg3 : class
    {
      private readonly Func<TAgg1, TAgg2, TAgg3, IAsyncHandler<TRequest, TResponse>> bootstrapperFunc;

      public Impl(
        Func<TAgg1, TAgg2, TAgg3, IAsyncHandler<TRequest, TResponse>> bootstrapperFunc)
      {
        this.bootstrapperFunc = bootstrapperFunc;
      }

      protected override IAsyncHandler<TRequest, TResponse> Bootstrap(
        TAgg1 agg1,
        TAgg2 agg2,
        TAgg3 agg3)
      {
        return this.bootstrapperFunc(agg1, agg2, agg3);
      }
    }
  }
}
