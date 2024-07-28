// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution.AggregationResolverLegacyExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution
{
  public static class AggregationResolverLegacyExtensions
  {
    public static IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped, TAgg1>(
      this IAggregationResolvingFactoryFactory resolver,
      Func<TAgg1, TBootstrapped> bootstrapFunc)
      where TBootstrapped : class
      where TAgg1 : class
    {
      return resolver.WithAggregation1<TAgg1>().FactoryFor<TBootstrapped, TAgg1>(bootstrapFunc);
    }

    public static IAsyncHandler<TIn, TOut> HandlerFor<TIn, TOut, TAgg1>(
      this IAggregationResolvingFactoryFactory resolver,
      Func<TAgg1, IAsyncHandler<TIn, TOut>> bootstrapFunc)
      where TIn : class, IFeedRequest
      where TAgg1 : class
    {
      return resolver.WithAggregation1<TAgg1>().HandlerFor<TIn, TOut, TAgg1>(bootstrapFunc);
    }

    public static IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped, TAgg1, TAgg2>(
      this IAggregationResolvingFactoryFactory resolver,
      Func<TAgg1, TAgg2, TBootstrapped> bootstrapFunc)
      where TBootstrapped : class
      where TAgg1 : class
      where TAgg2 : class
    {
      return resolver.WithAggregation1<TAgg1>().WithAggregation2<TAgg2>().FactoryFor<TBootstrapped, TAgg1, TAgg2>(bootstrapFunc);
    }

    public static IAsyncHandler<TIn, TOut> HandlerFor<TIn, TOut, TAgg1, TAgg2>(
      this IAggregationResolvingFactoryFactory resolver,
      Func<TAgg1, TAgg2, IAsyncHandler<TIn, TOut>> bootstrapFunc)
      where TIn : class, IFeedRequest
      where TAgg1 : class
      where TAgg2 : class
    {
      return resolver.WithAggregation1<TAgg1>().WithAggregation2<TAgg2>().HandlerFor<TIn, TOut, TAgg1, TAgg2>(bootstrapFunc);
    }

    public static IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped, TAgg1, TAgg2, TAgg3>(
      this IAggregationResolvingFactoryFactory resolver,
      Func<TAgg1, TAgg2, TAgg3, TBootstrapped> bootstrapFunc)
      where TBootstrapped : class
      where TAgg1 : class
      where TAgg2 : class
      where TAgg3 : class
    {
      return resolver.WithAggregation1<TAgg1>().WithAggregation2<TAgg2>().WithAggregation3<TAgg3>().FactoryFor<TBootstrapped, TAgg1, TAgg2, TAgg3>(bootstrapFunc);
    }

    public static IAsyncHandler<TIn, TOut> HandlerFor<TIn, TOut, TAgg1, TAgg2, TAgg3>(
      this IAggregationResolvingFactoryFactory resolver,
      Func<TAgg1, TAgg2, TAgg3, IAsyncHandler<TIn, TOut>> bootstrapFunc)
      where TIn : class, IFeedRequest
      where TAgg1 : class
      where TAgg2 : class
      where TAgg3 : class
    {
      return resolver.WithAggregation1<TAgg1>().WithAggregation2<TAgg2>().WithAggregation3<TAgg3>().HandlerFor<TIn, TOut, TAgg1, TAgg2, TAgg3>(bootstrapFunc);
    }
  }
}
