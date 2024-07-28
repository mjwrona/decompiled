// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildSourceProviderServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class BuildSourceProviderServiceExtensions
  {
    public static IEnumerable<IBuildSourceProvider> GetSourceProvidersForExecutionEnvironment(
      this IBuildSourceProviderService buildSourceProviderService,
      IVssRequestContext requestContext)
    {
      return buildSourceProviderService.GetSourceProviders(requestContext).GetSourceProvidersForExecutionEnvironment(requestContext);
    }

    public static IEnumerable<IBuildSourceProvider> GetSourceProvidersForExecutionEnvironment(
      this IEnumerable<IBuildSourceProvider> providers,
      IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        return providers.Where<IBuildSourceProvider>((Func<IBuildSourceProvider, bool>) (i => i.GetAttributes(requestContext).Availability.HasFlag((Enum) SourceProviderAvailability.Hosted)));
      return requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? providers.Where<IBuildSourceProvider>((Func<IBuildSourceProvider, bool>) (i => i.GetAttributes(requestContext).Availability.HasFlag((Enum) SourceProviderAvailability.OnPremises))) : providers;
    }
  }
}
