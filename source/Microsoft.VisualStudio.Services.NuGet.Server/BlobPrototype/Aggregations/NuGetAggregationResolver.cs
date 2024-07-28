// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.NuGetAggregationResolver
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations
{
  public static class NuGetAggregationResolver
  {
    public static IAggregationResolvingFactoryFactory Bootstrap(IVssRequestContext requestContext) => AggregationResolvingFactoryFactory.Bootstrap(requestContext, new NuGetMigrationDefinitionsProviderBootstrapper(requestContext).Bootstrap());

    public static IPendingBootstrapper<IAggregationResolvingFactoryFactory> PendingBootstrapper { get; } = ByFuncPendingBootstrapper.For<IAggregationResolvingFactoryFactory>(new Func<IVssRequestContext, IAggregationResolvingFactoryFactory>(NuGetAggregationResolver.Bootstrap));
  }
}
