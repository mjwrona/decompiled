// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.InstanceAllocationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server.Commerce;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class InstanceAllocationHelper
  {
    private const string c_Area = "InstanceAllocation";
    private const string c_layer = "InstanceAllocationHelper";

    internal static ServiceDefinition ComputeInstanceAllocation(
      IVssRequestContext requestContext,
      Guid identifier)
    {
      requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        try
        {
          Collection collection = requestContext.GetService<ICollectionService>().GetCollection(requestContext.Elevate(), (IEnumerable<string>) null);
          if (!requestContext.IsInfrastructureHost())
          {
            if (collection != null)
            {
              if (collection.Status != CollectionStatus.MarkedForPhysicalDelete)
                goto label_6;
            }
            return (ServiceDefinition) null;
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(66058, "InstanceAllocation", nameof (InstanceAllocationHelper), ex);
        }
      }
label_6:
      HostInstanceMapping instanceAllocation = requestContext.GetService<IInstanceAllocationService>().ComputeHostInstanceAllocation(requestContext, identifier);
      if (instanceAllocation == null)
        return (ServiceDefinition) null;
      return new ServiceDefinition("LocationService2", identifier, TFCommonResources.LocationService(), (string) null, RelativeToSetting.FullyQualified, TFCommonResources.LocationService(), "Framework")
      {
        Status = ServiceStatus.Assigned,
        ParentServiceType = "LocationService2",
        ParentIdentifier = instanceAllocation.ServiceInstance.InstanceId
      };
    }

    public static bool IsInfrastructureHost(IVssRequestContext requestContext)
    {
      ServiceHostTags serviceHostTags = ServiceHostTags.FromString(requestContext.To(TeamFoundationHostType.Application).ServiceHost.Description);
      return serviceHostTags != ServiceHostTags.EmptyServiceHostTags && serviceHostTags.HasTag(WellKnownServiceHostTags.IsInfrastructureHost);
    }
  }
}
