// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.CargoAggregationResolver
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations
{
  public static class CargoAggregationResolver
  {
    public static IAggregationResolvingFactoryFactory Bootstrap(IVssRequestContext requestContext) => AggregationResolvingFactoryFactory.Bootstrap(requestContext, new CargoMigrationDefinitionsProviderBootstrapper(requestContext).Bootstrap());

    public static IPendingBootstrapper<IAggregationResolvingFactoryFactory> PendingBootstrapper { get; } = ByFuncPendingBootstrapper.For<IAggregationResolvingFactoryFactory>(new Func<IVssRequestContext, IAggregationResolvingFactoryFactory>(CargoAggregationResolver.Bootstrap));
  }
}
