// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Partitioning.Server.PartitionedClientHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Partitioning.Server
{
  public static class PartitionedClientHelper
  {
    public static TClient GetSpsClientForHostId<TClient>(
      IVssRequestContext deploymentContext,
      Guid hostId)
      where TClient : class, IVssHttpClient
    {
      PartitionedClientHelper.Validate(deploymentContext, hostId);
      Guid instanceForHostId = PartitionedClientHelper.GetSpsInstanceForHostId(deploymentContext, hostId);
      return deploymentContext.GetClient<TClient>(instanceForHostId);
    }

    public static Guid GetSpsInstanceForHostId(IVssRequestContext deploymentContext, Guid hostId)
    {
      PartitionedClientHelper.Validate(deploymentContext, hostId);
      ServiceDefinition definitionForHost = LocationServiceHelper.GetSpsDefinitionForHost(deploymentContext, hostId);
      if (definitionForHost == null || definitionForHost.ParentIdentifier == Guid.Empty)
        throw new InvalidOperationException(string.Format("The required SPS service definition could not be found or was unexpected. HostId: {0}", (object) hostId));
      return definitionForHost.ParentIdentifier;
    }

    public static Uri GetSpsUriForHostId(IVssRequestContext deploymentContext, Guid hostId)
    {
      PartitionedClientHelper.Validate(deploymentContext, hostId);
      return new Uri(LocationServiceHelper.GetRootLocationServiceUrl(deploymentContext, hostId));
    }

    private static void Validate(IVssRequestContext deploymentContext, Guid hostId)
    {
      deploymentContext.CheckDeploymentRequestContext();
      if (hostId == deploymentContext.ServiceHost.InstanceId)
        throw new InvalidOperationException(string.Format("This method cannot be called with a deployment hostId: {0}", (object) hostId));
    }
  }
}
