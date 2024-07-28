// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Partitioning.Server.PartitioningHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Partitioning.Server
{
  internal static class PartitioningHelper
  {
    public static void ConfigurePartitioningFeature(
      IVssRequestContext deploymentContext,
      string partitionServiceUrl)
    {
      deploymentContext.CheckDeploymentRequestContext();
      ILocationService service = deploymentContext.GetService<ILocationService>();
      ServiceDefinition serviceDefinition1 = service.FindServiceDefinition(deploymentContext, "LocationService2", ServiceInstanceTypes.MPS);
      if (serviceDefinition1 != null)
      {
        if (partitionServiceUrl.TrimEnd('/').Equals(serviceDefinition1.GetLocationMapping(AccessMappingConstants.HostGuidAccessMappingMoniker).Location.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
          goto label_3;
      }
      Guid mps = ServiceInstanceTypes.MPS;
      List<LocationMapping> locationMappings = new List<LocationMapping>();
      locationMappings.Add(new LocationMapping(AccessMappingConstants.HostGuidAccessMappingMoniker, partitionServiceUrl));
      Guid serviceOwner = new Guid();
      ServiceDefinition serviceDefinition2 = new ServiceDefinition("LocationService2", mps, "MPS Location Service", (string) null, RelativeToSetting.FullyQualified, (string) null, "Framework", locationMappings, serviceOwner);
      service.SaveServiceDefinitions(deploymentContext, (IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
      {
        serviceDefinition2
      });
label_3:
      PartitioningHttpClient client = deploymentContext.GetClient<PartitioningHttpClient>(ServiceInstanceTypes.MPS);
      Guid? containerType = new Guid?(ServiceInstanceTypes.SPS);
      CancellationToken cancellationToken1 = deploymentContext.CancellationToken;
      bool? isAcquirable = new bool?();
      CancellationToken cancellationToken2 = cancellationToken1;
      client.QueryPartitionContainersAsync(containerType, isAcquirable: isAcquirable, cancellationToken: cancellationToken2).SyncResult<List<PartitionContainer>>();
    }
  }
}
