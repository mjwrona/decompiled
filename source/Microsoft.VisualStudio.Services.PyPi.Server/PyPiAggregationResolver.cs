// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PyPiAggregationResolver
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.Migration;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server
{
  public static class PyPiAggregationResolver
  {
    public static IAggregationResolvingFactoryFactory Bootstrap(IVssRequestContext requestContext) => AggregationResolvingFactoryFactory.Bootstrap(requestContext, new PyPiMigrationDefinitionsProviderBootstrapper(requestContext).Bootstrap());

    public static IPendingBootstrapper<IAggregationResolvingFactoryFactory> PendingBootstrapper { get; } = ByFuncPendingBootstrapper.For<IAggregationResolvingFactoryFactory>(new Func<IVssRequestContext, IAggregationResolvingFactoryFactory>(PyPiAggregationResolver.Bootstrap));
  }
}
