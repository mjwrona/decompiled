// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.FrameworkRegionManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  public class FrameworkRegionManagementService : IRegionManagementService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private const string c_area = "HostManagement";
    private const string c_layer = "FrameworkRegionManagementService";

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public Region GetRegion(IVssRequestContext context, string regionCode)
    {
      this.ValidateRequestContext(context);
      RegionCodeValidationHelper.VerifyValidRegionCode(regionCode);
      RegionCacheService service = context.GetService<RegionCacheService>();
      Region region1;
      if (service.TryGetValue(context, regionCode, out region1))
      {
        context.Trace(7620053, TraceLevel.Info, "HostManagement", nameof (FrameworkRegionManagementService), "Cache hit. Key: " + regionCode + ", cachedItem: " + (region1 != null ? region1.Serialize<Region>() : (string) null) + ".");
        return region1;
      }
      context.Trace(7620054, TraceLevel.Info, "HostManagement", nameof (FrameworkRegionManagementService), "Cache miss. Key: " + regionCode + ".");
      foreach (PartitionContainer spsContainer1 in (IEnumerable<PartitionContainer>) FrameworkRegionManagementService.GetSpsContainers(context))
      {
        PartitionContainer spsContainer = spsContainer1;
        if (spsContainer == null || spsContainer.Address.IsNullOrEmpty<char>())
        {
          IVssRequestContext requestContext = context;
          PartitionContainer partitionContainer = spsContainer;
          string message = "Found invalid SPS container: " + (partitionContainer != null ? partitionContainer.Serialize<PartitionContainer>() : (string) null) + ".";
          requestContext.Trace(7620055, TraceLevel.Error, "HostManagement", nameof (FrameworkRegionManagementService), message);
        }
        else
        {
          GetRegionCircuitBreakerSettings circuitBreakerSettings = new GetRegionCircuitBreakerSettings(spsContainer.Address);
          CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) circuitBreakerSettings.CommandGroupKey).AndCommandKey((CommandKey) circuitBreakerSettings.CommandKeyForGetRegion).AndCommandPropertiesDefaults(circuitBreakerSettings.CircuitBreakerSettingsForGetRegion);
          Region region2 = new CommandService<Region>(context, setter, (Func<Region>) (() => FrameworkRegionManagementService.GetRegionFromSpsInstance(context, spsContainer.Address, regionCode)), (Func<Region>) (() =>
          {
            context.Trace(7620056, TraceLevel.Error, "HostManagement", nameof (FrameworkRegionManagementService), "Failed to get region: " + regionCode + " from SPS instance: " + spsContainer.Serialize<PartitionContainer>() + ".");
            return (Region) null;
          })).Execute();
          if (region2 != null)
          {
            service.Set(context, regionCode, region2);
            return region2;
          }
        }
      }
      return (Region) null;
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckDeploymentRequestContext();
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private static IList<PartitionContainer> GetSpsContainers(IVssRequestContext context)
    {
      IVssRequestContext vssRequestContext = context.Elevate();
      return vssRequestContext.GetService<IPartitioningService>().QueryPartitionContainers(vssRequestContext, ServiceInstanceTypes.SPS);
    }

    private static HostManagementHttpClient GetHttpClient(
      IVssRequestContext context,
      string baseUri)
    {
      return (context.ClientProvider as ICreateClient).CreateClient<HostManagementHttpClient>(context, new Uri(baseUri), "HostManagement", (ApiResourceLocationCollection) null);
    }

    private static Region GetRegionFromSpsInstance(
      IVssRequestContext context,
      string spsInstanceUri,
      string regionCode)
    {
      try
      {
        return FrameworkRegionManagementService.GetHttpClient(context, spsInstanceUri).GetRegionAsync(regionCode).SyncResult<Region>();
      }
      catch (InvalidRegionCodeException ex)
      {
        ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
        throw;
      }
    }
  }
}
