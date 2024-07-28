// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Aggregations.MavenAggregationResolver
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Maven.Server.Aggregations
{
  public static class MavenAggregationResolver
  {
    public static IAggregationResolvingFactoryFactory Bootstrap(IVssRequestContext requestContext) => AggregationResolvingFactoryFactory.Bootstrap(requestContext, new MavenMigrationDefinitionsProviderBootstrapper(requestContext).Bootstrap());

    public static IPendingBootstrapper<IAggregationResolvingFactoryFactory> PendingBootstrapper { get; } = ByFuncPendingBootstrapper.For<IAggregationResolvingFactoryFactory>(new Func<IVssRequestContext, IAggregationResolvingFactoryFactory>(MavenAggregationResolver.Bootstrap));
  }
}
