// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.FrameworkExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using System;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework
{
  public static class FrameworkExtensions
  {
    private static string ObjectCacheKey = "Packaging.ObjectCacheKey";

    internal static T CacheSingle<T>(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, T> cacheFunc)
    {
      object obj1;
      if (!requestContext.Items.TryGetValue(FrameworkExtensions.ObjectCacheKey, out obj1) || !(obj1 is ConcurrentDictionary<Type, object> concurrentDictionary))
      {
        concurrentDictionary = new ConcurrentDictionary<Type, object>();
        requestContext.Items[FrameworkExtensions.ObjectCacheKey] = (object) concurrentDictionary;
      }
      Type key = typeof (T);
      object obj2;
      if (concurrentDictionary.TryGetValue(key, out obj2))
        return (T) obj2;
      T obj3 = cacheFunc(requestContext);
      concurrentDictionary.TryAdd(key, (object) obj3);
      return obj3;
    }

    public static IExecutionEnvironment GetExecutionEnvironmentFacade(
      this IVssRequestContext requestContext)
    {
      return (IExecutionEnvironment) FrameworkExtensions.CacheSingle<ExecutionEnvironmentFacade>(requestContext, (Func<IVssRequestContext, ExecutionEnvironmentFacade>) (rc => new ExecutionEnvironmentFacade(rc)));
    }

    public static IFeatureFlagService GetFeatureFlagFacade(this IVssRequestContext requestContext) => (IFeatureFlagService) FrameworkExtensions.CacheSingle<FeatureFlagFacade>(requestContext, (Func<IVssRequestContext, FeatureFlagFacade>) (rc => new FeatureFlagFacade(rc)));

    public static ILocationFacade GetLocationFacade(this IVssRequestContext requestContext) => (ILocationFacade) FrameworkExtensions.CacheSingle<LocationServiceFacade>(requestContext, (Func<IVssRequestContext, LocationServiceFacade>) (rc => new LocationServiceFacade(rc)));

    public static ITracerService GetTracerFacade(this IVssRequestContext requestContext) => (ITracerService) FrameworkExtensions.CacheSingle<TracerFacade>(requestContext, (Func<IVssRequestContext, TracerFacade>) (rc => new TracerFacade(rc, FeatureAvailabilityConstants.AllowStoredTraces.Bootstrap(rc), PackagingServerConstants.MaxStoredTraces.Bootstrap(rc))));

    public static IRegistryWriterService GetRegistryFacade(this IVssRequestContext requestContext) => (IRegistryWriterService) FrameworkExtensions.CacheSingle<RegistryServiceFacade>(requestContext, (Func<IVssRequestContext, RegistryServiceFacade>) (rc => new RegistryServiceFacade(rc)));

    public static IPackagingTraces GetPackagingTracesFacade(this IVssRequestContext requestContext) => (IPackagingTraces) FrameworkExtensions.CacheSingle<PackagingTracesFacade>(requestContext, (Func<IVssRequestContext, PackagingTracesFacade>) (rc => new PackagingTracesFacade(rc)));
  }
}
