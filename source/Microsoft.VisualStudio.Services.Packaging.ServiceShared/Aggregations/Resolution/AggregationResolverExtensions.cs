// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution.AggregationResolverExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution
{
  public static class AggregationResolverExtensions
  {
    public static AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory> WithPolicy(
      this IAggregationResolvingFactoryFactory core,
      AggregationHandlerPolicy policy)
    {
      return new AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>(core, new AggregationHandlerPolicy?(policy));
    }

    public static AggregationResolverExtensions.PolicyCapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>> WithPolicy(
      this IPendingBootstrapper<IAggregationResolvingFactoryFactory> core,
      AggregationHandlerPolicy policy)
    {
      return new AggregationResolverExtensions.PolicyCapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>>(core, new AggregationHandlerPolicy?(policy));
    }

    public static AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1> WithAggregation1<TAgg1>(
      this IAggregationResolvingFactoryFactory core)
    {
      return new AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1>(core, new AggregationHandlerPolicy?());
    }

    public static AggregationResolverExtensions.AggregationTypeCapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>, TAgg1> WithAggregation1<TAgg1>(
      this IPendingBootstrapper<IAggregationResolvingFactoryFactory> core)
    {
      return new AggregationResolverExtensions.AggregationTypeCapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>, TAgg1>(core, new AggregationHandlerPolicy?());
    }

    public static IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped>(
      in this AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory> capture,
      IRequireAggBootstrapper<TBootstrapped> singleBootstrapper)
      where TBootstrapped : class
    {
      return AggregationResolverExtensions.CreateFactoryCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TBootstrapped>(in capture, singleBootstrapper);
    }

    public static IAsyncHandler<TIn, TOut> HandlerFor<TIn, TOut>(
      in this AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory> capture,
      IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>> singleBootstrapper)
      where TIn : class, IFeedRequest
    {
      return AggregationResolverExtensions.CreateHandlerCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TIn, TOut>(in capture, singleBootstrapper);
    }

    public static IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped>(
      this IAggregationResolvingFactoryFactory capture,
      IRequireAggBootstrapper<TBootstrapped> singleBootstrapper)
      where TBootstrapped : class
    {
      return AggregationResolverExtensions.CreateFactoryCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TBootstrapped>(AggregationResolverExtensions.CaptureDefaultPolicy<IAggregationResolvingFactoryFactory>(capture), singleBootstrapper);
    }

    public static IAsyncHandler<TIn, TOut> HandlerFor<TIn, TOut>(
      this IAggregationResolvingFactoryFactory resolver,
      IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>> singleBootstrapper)
      where TIn : class, IFeedRequest
    {
      return AggregationResolverExtensions.CreateHandlerCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TIn, TOut>(AggregationResolverExtensions.CaptureDefaultPolicy<IAggregationResolvingFactoryFactory>(resolver), singleBootstrapper);
    }

    public static IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped>(
      in this AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory> capture,
      IRequireAggBootstrapper<TBootstrapped> firstBootstrapper,
      params IRequireAggBootstrapper<TBootstrapped>[] additionalBootstrappers)
      where TBootstrapped : class
    {
      return AggregationResolverExtensions.CreateFactoryCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TBootstrapped>(in capture, ((IEnumerable<IRequireAggBootstrapper<TBootstrapped>>) additionalBootstrappers).Prepend<IRequireAggBootstrapper<TBootstrapped>>(firstBootstrapper));
    }

    public static IAsyncHandler<TIn, TOut> HandlerFor<TIn, TOut>(
      this AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory> capture,
      IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>> firstBootstrapper,
      params IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>>[] additionalBootstrappers)
      where TIn : class, IFeedRequest
    {
      return AggregationResolverExtensions.CreateHandlerCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TIn, TOut>(in capture, ((IEnumerable<IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>>>) additionalBootstrappers).Prepend<IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>>>(firstBootstrapper));
    }

    public static IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped>(
      this IAggregationResolvingFactoryFactory resolver,
      IRequireAggBootstrapper<TBootstrapped> firstBootstrapper,
      params IRequireAggBootstrapper<TBootstrapped>[] additionalBootstrappers)
      where TBootstrapped : class
    {
      return AggregationResolverExtensions.CreateFactoryCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TBootstrapped>(AggregationResolverExtensions.CaptureDefaultPolicy<IAggregationResolvingFactoryFactory>(resolver), ((IEnumerable<IRequireAggBootstrapper<TBootstrapped>>) additionalBootstrappers).Prepend<IRequireAggBootstrapper<TBootstrapped>>(firstBootstrapper));
    }

    public static IAsyncHandler<TIn, TOut> HandlerFor<TIn, TOut>(
      this IAggregationResolvingFactoryFactory resolver,
      IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>> firstBootstrapper,
      params IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>>[] additionalBootstrappers)
      where TIn : class, IFeedRequest
    {
      return AggregationResolverExtensions.CreateHandlerCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TIn, TOut>(AggregationResolverExtensions.CaptureDefaultPolicy<IAggregationResolvingFactoryFactory>(resolver), ((IEnumerable<IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>>>) additionalBootstrappers).Prepend<IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>>>(firstBootstrapper));
    }

    public static IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped>(
      in this AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory> capture,
      IEnumerable<IRequireAggBootstrapper<TBootstrapped>> bootstrapperSequence)
      where TBootstrapped : class
    {
      return AggregationResolverExtensions.CreateFactoryCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TBootstrapped>(in capture, bootstrapperSequence);
    }

    public static IAsyncHandler<TIn, TOut> HandlerFor<TIn, TOut>(
      in this AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory> capture,
      IEnumerable<IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>>> bootstrapperSequence)
      where TIn : class, IFeedRequest
    {
      return AggregationResolverExtensions.CreateHandlerCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TIn, TOut>(in capture, bootstrapperSequence);
    }

    public static IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped>(
      this IAggregationResolvingFactoryFactory resolver,
      IEnumerable<IRequireAggBootstrapper<TBootstrapped>> bootstrapperSequence)
      where TBootstrapped : class
    {
      return AggregationResolverExtensions.CreateFactoryCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TBootstrapped>(AggregationResolverExtensions.CaptureDefaultPolicy<IAggregationResolvingFactoryFactory>(resolver), bootstrapperSequence);
    }

    public static IAsyncHandler<TIn, TOut> HandlerFor<TIn, TOut>(
      this IAggregationResolvingFactoryFactory resolver,
      IEnumerable<IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>>> bootstrapperSequence)
      where TIn : class, IFeedRequest
    {
      return AggregationResolverExtensions.CreateHandlerCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TIn, TOut>(AggregationResolverExtensions.CaptureDefaultPolicy<IAggregationResolvingFactoryFactory>(resolver), bootstrapperSequence);
    }

    public static IFactory<IFeedRequest, Task<TAgg>> Factory<TAgg>(
      in this AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg> capture)
      where TAgg : class
    {
      return AggregationResolverExtensions.CreateFactoryCore<AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg>, TAgg>(in capture, (IEnumerable<IRequireAggBootstrapper<TAgg>>) AggregationResolverExtensions.JustReturnTheAggregationRequireAggBootstrapper<TAgg>.InstanceArray);
    }

    public static IFactory<IFeedRequest, Task<TAgg>> FactoryFor<TAgg>(
      this IAggregationResolvingFactoryFactory resolver)
      where TAgg : class
    {
      return AggregationResolverExtensions.CreateFactoryCore<AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>, TAgg>(AggregationResolverExtensions.CaptureDefaultPolicy<IAggregationResolvingFactoryFactory>(resolver), (IEnumerable<IRequireAggBootstrapper<TAgg>>) AggregationResolverExtensions.JustReturnTheAggregationRequireAggBootstrapper<TAgg>.InstanceArray);
    }

    public static IPendingAggBootstrapper<TAgg> PendingAggBootstrapper<TAgg>(
      this AggregationResolverExtensions.AggregationTypeCapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>, TAgg> capture)
      where TAgg : class
    {
      return (IPendingAggBootstrapper<TAgg>) ByFactoryFuncPendingAggBootstrapper.For<TAgg>((Func<IVssRequestContext, IFactory<IFeedRequest, Task<TAgg>>>) (rc => capture.Core.Bootstrap(rc).CopyPolicyFrom<AggregationResolverExtensions.AggregationTypeCapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>, TAgg>>(in capture).WithAggregation1<TAgg>().Factory<TAgg>()));
    }

    public static IPendingAggBootstrapper<TAgg> PendingAggBootstrapperFor<TAgg>(
      this IPendingBootstrapper<IAggregationResolvingFactoryFactory> pendingBootstrapper)
      where TAgg : class
    {
      return (IPendingAggBootstrapper<TAgg>) ByFactoryFuncPendingAggBootstrapper.For<TAgg>((Func<IVssRequestContext, IFactory<IFeedRequest, Task<TAgg>>>) (rc => pendingBootstrapper.Bootstrap(rc).WithAggregation1<TAgg>().Factory<TAgg>()));
    }

    public static IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped, TAgg1>(
      in this AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1> capture,
      Func<TAgg1, TBootstrapped> bootstrapFunc)
      where TBootstrapped : class
      where TAgg1 : class
    {
      return AggregationResolverExtensions.CreateFactoryCore<AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1>, TBootstrapped>(in capture, ByFuncRequireAggBootstrapper.For<TBootstrapped, TAgg1>(bootstrapFunc));
    }

    public static IAsyncHandler<TIn, TOut> HandlerFor<TIn, TOut, TAgg1>(
      in this AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1> capture,
      Func<TAgg1, IAsyncHandler<TIn, TOut>> bootstrapFunc)
      where TIn : class, IFeedRequest
      where TAgg1 : class
    {
      return AggregationResolverExtensions.CreateHandlerCore<AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1>, TIn, TOut>(in capture, ByFuncRequireAggBootstrapper.For<IAsyncHandler<TIn, TOut>, TAgg1>(bootstrapFunc));
    }

    public static IPendingAggBootstrapper<TBootstrapped> PendingAggBootstrapperFor<TBootstrapped, TAgg1>(
      this AggregationResolverExtensions.AggregationTypeCapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>, TAgg1> capture,
      Func<IVssRequestContext, TAgg1, TBootstrapped> bootstrapFunc)
      where TBootstrapped : class
      where TAgg1 : class
    {
      return (IPendingAggBootstrapper<TBootstrapped>) ByFactoryFuncPendingAggBootstrapper.For<TBootstrapped>((Func<IVssRequestContext, IFactory<IFeedRequest, Task<TBootstrapped>>>) (rc => capture.Core.Bootstrap(rc).CopyPolicyFrom<AggregationResolverExtensions.AggregationTypeCapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>, TAgg1>>(in capture).WithAggregation1<TAgg1>().FactoryFor<TBootstrapped, TAgg1>((Func<TAgg1, TBootstrapped>) (agg1 => bootstrapFunc(rc, agg1)))));
    }

    public static IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped, TAgg1, TAgg2>(
      in this AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1, TAgg2> capture,
      Func<TAgg1, TAgg2, TBootstrapped> bootstrapFunc)
      where TBootstrapped : class
      where TAgg1 : class
      where TAgg2 : class
    {
      return AggregationResolverExtensions.CreateFactoryCore<AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1, TAgg2>, TBootstrapped>(in capture, ByFuncRequireAggBootstrapper.For<TBootstrapped, TAgg1, TAgg2>(bootstrapFunc));
    }

    public static IAsyncHandler<TIn, TOut> HandlerFor<TIn, TOut, TAgg1, TAgg2>(
      in this AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1, TAgg2> capture,
      Func<TAgg1, TAgg2, IAsyncHandler<TIn, TOut>> bootstrapFunc)
      where TIn : class, IFeedRequest
      where TAgg1 : class
      where TAgg2 : class
    {
      return AggregationResolverExtensions.CreateHandlerCore<AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1, TAgg2>, TIn, TOut>(in capture, ByFuncRequireAggBootstrapper.For<IAsyncHandler<TIn, TOut>, TAgg1, TAgg2>(bootstrapFunc));
    }

    public static IPendingAggBootstrapper<TBootstrapped> PendingAggBootstrapperFor<TBootstrapped, TAgg1, TAgg2>(
      this AggregationResolverExtensions.AggregationTypeCapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>, TAgg1, TAgg2> capture,
      Func<IVssRequestContext, TAgg1, TAgg2, TBootstrapped> bootstrapFunc)
      where TBootstrapped : class
      where TAgg1 : class
      where TAgg2 : class
    {
      return (IPendingAggBootstrapper<TBootstrapped>) ByFactoryFuncPendingAggBootstrapper.For<TBootstrapped>((Func<IVssRequestContext, IFactory<IFeedRequest, Task<TBootstrapped>>>) (rc => capture.Core.Bootstrap(rc).CopyPolicyFrom<AggregationResolverExtensions.AggregationTypeCapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>, TAgg1, TAgg2>>(in capture).WithAggregation1<TAgg1>().WithAggregation2<TAgg2>().FactoryFor<TBootstrapped, TAgg1, TAgg2>((Func<TAgg1, TAgg2, TBootstrapped>) ((agg1, agg2) => bootstrapFunc(rc, agg1, agg2)))));
    }

    public static IFactory<IFeedRequest, Task<TBootstrapped>> FactoryFor<TBootstrapped, TAgg1, TAgg2, TAgg3>(
      in this AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1, TAgg2, TAgg3> capture,
      Func<TAgg1, TAgg2, TAgg3, TBootstrapped> bootstrapFunc)
      where TBootstrapped : class
      where TAgg1 : class
      where TAgg2 : class
      where TAgg3 : class
    {
      return AggregationResolverExtensions.CreateFactoryCore<AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1, TAgg2, TAgg3>, TBootstrapped>(in capture, ByFuncRequireAggBootstrapper.For<TBootstrapped, TAgg1, TAgg2, TAgg3>(bootstrapFunc));
    }

    public static IAsyncHandler<TIn, TOut> HandlerFor<TIn, TOut, TAgg1, TAgg2, TAgg3>(
      in this AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1, TAgg2, TAgg3> capture,
      Func<TAgg1, TAgg2, TAgg3, IAsyncHandler<TIn, TOut>> bootstrapFunc)
      where TIn : class, IFeedRequest
      where TAgg1 : class
      where TAgg2 : class
      where TAgg3 : class
    {
      return AggregationResolverExtensions.CreateHandlerCore<AggregationResolverExtensions.AggregationTypeCapture<IAggregationResolvingFactoryFactory, TAgg1, TAgg2, TAgg3>, TIn, TOut>(in capture, ByFuncRequireAggBootstrapper.For<IAsyncHandler<TIn, TOut>, TAgg1, TAgg2, TAgg3>(bootstrapFunc));
    }

    public static IPendingAggBootstrapper<TBootstrapped> PendingAggBootstrapperFor<TBootstrapped, TAgg1, TAgg2, TAgg3>(
      this AggregationResolverExtensions.AggregationTypeCapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>, TAgg1, TAgg2, TAgg3> capture,
      Func<IVssRequestContext, TAgg1, TAgg2, TAgg3, TBootstrapped> bootstrapFunc)
      where TBootstrapped : class
      where TAgg1 : class
      where TAgg2 : class
      where TAgg3 : class
    {
      return (IPendingAggBootstrapper<TBootstrapped>) ByFactoryFuncPendingAggBootstrapper.For<TBootstrapped>((Func<IVssRequestContext, IFactory<IFeedRequest, Task<TBootstrapped>>>) (rc => capture.Core.Bootstrap(rc).CopyPolicyFrom<AggregationResolverExtensions.AggregationTypeCapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>, TAgg1, TAgg2, TAgg3>>(in capture).WithAggregation1<TAgg1>().WithAggregation2<TAgg2>().WithAggregation3<TAgg3>().FactoryFor<TBootstrapped, TAgg1, TAgg2, TAgg3>((Func<TAgg1, TAgg2, TAgg3, TBootstrapped>) ((agg1, agg2, agg3) => bootstrapFunc(rc, agg1, agg2, agg3)))));
    }

    private static AggregationResolverExtensions.PolicyCapture<TCore> CaptureDefaultPolicy<TCore>(
      TCore core)
    {
      return new AggregationResolverExtensions.PolicyCapture<TCore>(core, new AggregationHandlerPolicy?());
    }

    private static AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory> CopyPolicyFrom<TCapture>(
      this IAggregationResolvingFactoryFactory factory,
      in TCapture otherCapture)
      where TCapture : struct, AggregationResolverExtensions.ICapture<IPendingBootstrapper<IAggregationResolvingFactoryFactory>>
    {
      return new AggregationResolverExtensions.PolicyCapture<IAggregationResolvingFactoryFactory>(factory, otherCapture.Policy);
    }

    private static IFactory<IFeedRequest, Task<TBootstrapped>> CreateFactoryCore<TCapture, TBootstrapped>(
      in TCapture capture,
      IRequireAggBootstrapper<TBootstrapped> singleBootstrapper)
      where TCapture : struct, AggregationResolverExtensions.ICapture<IAggregationResolvingFactoryFactory>
      where TBootstrapped : class
    {
      TCapture capture1 = capture;
      IAggregationResolvingFactoryFactory core = capture1.Core;
      capture1 = capture;
      int valueOrDefault = (int) capture1.Policy.GetValueOrDefault();
      IRequireAggBootstrapper<TBootstrapped>[] bootstrapperSequence = new IRequireAggBootstrapper<TBootstrapped>[1]
      {
        singleBootstrapper
      };
      return core.FactoryFor<TBootstrapped>((AggregationHandlerPolicy) valueOrDefault, (IEnumerable<IRequireAggBootstrapper<TBootstrapped>>) bootstrapperSequence);
    }

    private static IAsyncHandler<TIn, TOut> CreateHandlerCore<TCapture, TIn, TOut>(
      in TCapture capture,
      IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>> singleBootstrapper)
      where TCapture : struct, AggregationResolverExtensions.ICapture<IAggregationResolvingFactoryFactory>
      where TIn : class, IFeedRequest
    {
      return AggregationResolverExtensions.CreateFactoryCore<TCapture, IAsyncHandler<TIn, TOut>>(in capture, singleBootstrapper).FlattenHandlerFactoryToHandler<TIn, TOut>();
    }

    private static IFactory<IFeedRequest, Task<TBootstrapped>> CreateFactoryCore<TCapture, TBootstrapped>(
      in TCapture capture,
      IEnumerable<IRequireAggBootstrapper<TBootstrapped>> bootstrapperSequence)
      where TCapture : struct, AggregationResolverExtensions.ICapture<IAggregationResolvingFactoryFactory>
      where TBootstrapped : class
    {
      TCapture capture1 = capture;
      IAggregationResolvingFactoryFactory core = capture1.Core;
      capture1 = capture;
      int policy = (int) capture1.Policy ?? 1;
      IEnumerable<IRequireAggBootstrapper<TBootstrapped>> bootstrapperSequence1 = bootstrapperSequence;
      return core.FactoryFor<TBootstrapped>((AggregationHandlerPolicy) policy, bootstrapperSequence1);
    }

    private static IAsyncHandler<TIn, TOut> CreateHandlerCore<TCapture, TIn, TOut>(
      in TCapture capture,
      IEnumerable<IRequireAggBootstrapper<IAsyncHandler<TIn, TOut>>> bootstrapperSequence)
      where TCapture : struct, AggregationResolverExtensions.ICapture<IAggregationResolvingFactoryFactory>
      where TIn : class, IFeedRequest
    {
      return AggregationResolverExtensions.CreateFactoryCore<TCapture, IAsyncHandler<TIn, TOut>>(in capture, bootstrapperSequence).FlattenHandlerFactoryToHandler<TIn, TOut>();
    }

    private static IAsyncHandler<TIn, TOut> FlattenHandlerFactoryToHandler<TIn, TOut>(
      this IFactory<IFeedRequest, Task<IAsyncHandler<TIn, TOut>>> factory)
      where TIn : class, IFeedRequest
    {
      return (IAsyncHandler<TIn, TOut>) new AggregationResolverExtensions.FlattenHandlerFactoryToHandlerImpl<TIn, TOut>(factory.SingleElementCache<IFeedRequest, Guid, Task<IAsyncHandler<TIn, TOut>>>((Func<IFeedRequest, Guid>) (request => request.Feed.Id)));
    }

    private interface ICapture<out TCore>
    {
      TCore Core { get; }

      AggregationHandlerPolicy? Policy { get; }
    }

    public readonly struct PolicyCapture<TCore> : AggregationResolverExtensions.ICapture<TCore>
    {
      public PolicyCapture(TCore core, AggregationHandlerPolicy? policy)
      {
        this.Core = core;
        this.Policy = policy;
      }

      public TCore Core { get; }

      public AggregationHandlerPolicy? Policy { get; }

      public AggregationResolverExtensions.AggregationTypeCapture<TCore, TArg1> WithAggregation1<TArg1>() => new AggregationResolverExtensions.AggregationTypeCapture<TCore, TArg1>(this.Core, this.Policy);
    }

    public readonly struct AggregationTypeCapture<TCore, TArg1> : 
      AggregationResolverExtensions.ICapture<TCore>
    {
      public AggregationTypeCapture(TCore core, AggregationHandlerPolicy? policy)
      {
        this.Core = core;
        this.Policy = policy;
      }

      public AggregationHandlerPolicy? Policy { get; }

      public TCore Core { get; }

      public AggregationResolverExtensions.AggregationTypeCapture<TCore, TArg1, TArg2> WithAggregation2<TArg2>() => new AggregationResolverExtensions.AggregationTypeCapture<TCore, TArg1, TArg2>(this.Core, this.Policy);

      public AggregationResolverExtensions.AggregationTypeCapture<TCore, TArg1> WithPolicy(
        AggregationHandlerPolicy policy)
      {
        return new AggregationResolverExtensions.AggregationTypeCapture<TCore, TArg1>(this.Core, new AggregationHandlerPolicy?(policy));
      }
    }

    public readonly struct AggregationTypeCapture<TCore, TArg1, TArg2> : 
      AggregationResolverExtensions.ICapture<TCore>
    {
      public AggregationTypeCapture(TCore core, AggregationHandlerPolicy? policy)
      {
        this.Core = core;
        this.Policy = policy;
      }

      public AggregationHandlerPolicy? Policy { get; }

      public TCore Core { get; }

      public AggregationResolverExtensions.AggregationTypeCapture<TCore, TArg1, TArg2, TArg3> WithAggregation3<TArg3>() => new AggregationResolverExtensions.AggregationTypeCapture<TCore, TArg1, TArg2, TArg3>(this.Core, this.Policy);

      public AggregationResolverExtensions.AggregationTypeCapture<TCore, TArg1, TArg2> WithPolicy(
        AggregationHandlerPolicy policy)
      {
        return new AggregationResolverExtensions.AggregationTypeCapture<TCore, TArg1, TArg2>(this.Core, new AggregationHandlerPolicy?(policy));
      }
    }

    public readonly struct AggregationTypeCapture<TCore, TArg1, TArg2, TArg3> : 
      AggregationResolverExtensions.ICapture<TCore>
    {
      public AggregationTypeCapture(TCore core, AggregationHandlerPolicy? policy)
      {
        this.Core = core;
        this.Policy = policy;
      }

      public AggregationHandlerPolicy? Policy { get; }

      public TCore Core { get; }

      public AggregationResolverExtensions.AggregationTypeCapture<TCore, TArg1, TArg2, TArg3> WithPolicy(
        AggregationHandlerPolicy policy)
      {
        return new AggregationResolverExtensions.AggregationTypeCapture<TCore, TArg1, TArg2, TArg3>(this.Core, new AggregationHandlerPolicy?(policy));
      }
    }

    private class JustReturnTheAggregationRequireAggBootstrapper<TAgg> : 
      RequireAggBootstrapper<TAgg, TAgg>
      where TAgg : class
    {
      public static IRequireAggBootstrapper<TAgg>[] InstanceArray { get; } = new IRequireAggBootstrapper<TAgg>[1]
      {
        (IRequireAggBootstrapper<TAgg>) new AggregationResolverExtensions.JustReturnTheAggregationRequireAggBootstrapper<TAgg>()
      };

      protected override TAgg Bootstrap(TAgg agg1) => agg1;
    }

    private class FlattenHandlerFactoryToHandlerImpl<TIn, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
      where TIn : class, IFeedRequest
    {
      private readonly IFactory<IFeedRequest, Task<IAsyncHandler<TIn, TOut>>> factory;

      public FlattenHandlerFactoryToHandlerImpl(
        IFactory<IFeedRequest, Task<IAsyncHandler<TIn, TOut>>> factory)
      {
        this.factory = factory;
      }

      public async Task<TOut> Handle(TIn request) => await (await this.factory.Get((IFeedRequest) request)).Handle(request);
    }
  }
}
