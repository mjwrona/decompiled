// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.FrameworkGeographyManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.IdentityModel.Tokens;
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
  public class FrameworkGeographyManagementService : GeographyManagementServiceBase
  {
    private const string c_area = "HostManagement";
    private const string c_layer = "FrameworkGeographyManagementService";

    public override void ServiceStart(IVssRequestContext context) => this.ValidateRequestContext(context);

    public override void ServiceEnd(IVssRequestContext context)
    {
    }

    public override Geography GetGeography(IVssRequestContext context, string code)
    {
      this.ValidateRequestContext(context);
      GeographyManagementServiceBase.VerifyValidGeographyCode(code);
      GeographyCacheService service = context.GetService<GeographyCacheService>();
      Geography geography1;
      if (service.TryGetValue(context, code, out geography1))
      {
        context.Trace(7620063, TraceLevel.Info, "HostManagement", nameof (FrameworkGeographyManagementService), "Cache hit. Key: " + code + ", cachedItem: " + (geography1 != null ? geography1.Serialize<Geography>() : (string) null) + ".");
        return geography1;
      }
      context.Trace(7620064, TraceLevel.Info, "HostManagement", nameof (FrameworkGeographyManagementService), "Cache miss. Key: " + code + ".");
      Geography geography2 = (Geography) null;
      foreach (PartitionContainer spsContainer1 in (IEnumerable<PartitionContainer>) FrameworkGeographyManagementService.GetSpsContainers(context))
      {
        PartitionContainer spsContainer = spsContainer1;
        if (spsContainer == null || spsContainer.Address.IsNullOrEmpty<char>())
        {
          IVssRequestContext requestContext = context;
          PartitionContainer partitionContainer = spsContainer;
          string message = "Found invalid SPS container: " + (partitionContainer != null ? partitionContainer.Serialize<PartitionContainer>() : (string) null) + ".";
          requestContext.Trace(7620065, TraceLevel.Error, "HostManagement", nameof (FrameworkGeographyManagementService), message);
        }
        else
        {
          GetGeographyCircuitBreakerSettings circuitBreakerSettings = new GetGeographyCircuitBreakerSettings(spsContainer.Address);
          CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) circuitBreakerSettings.CommandGroupKey).AndCommandKey((CommandKey) circuitBreakerSettings.CommandKeyForGetGeography).AndCommandPropertiesDefaults(circuitBreakerSettings.CircuitBreakerSettingsForGetGeography);
          geography2 = new CommandService<Geography>(context, setter, (Func<Geography>) (() => FrameworkGeographyManagementService.GetGeographyFromSpsInstance(context, spsContainer.Address, code)), (Func<Geography>) (() =>
          {
            context.Trace(7620056, TraceLevel.Error, "HostManagement", nameof (FrameworkGeographyManagementService), "Failed to get region: " + code + " from SPS instance: " + spsContainer.Serialize<PartitionContainer>() + ".");
            return (Geography) null;
          })).Execute();
          this.AddOrUpdateGeography(context, service, geography2);
        }
      }
      return geography2;
    }

    private void AddOrUpdateGeography(
      IVssRequestContext context,
      GeographyCacheService geographyCacheService,
      Geography geography)
    {
      if (geography == null)
        return;
      Geography geography1;
      if (geographyCacheService.TryGetValue(context, geography.Code, out geography1))
        geography1.IsActive &= geography.IsActive;
      else
        geographyCacheService.TryAdd(context, geography.Code, geography);
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckDeploymentRequestContext();
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

    private static Geography GetGeographyFromSpsInstance(
      IVssRequestContext context,
      string spsInstanceUri,
      string code)
    {
      try
      {
        return FrameworkGeographyManagementService.GetHttpClient(context, spsInstanceUri).GetGeographyAsync(code).SyncResult<Geography>();
      }
      catch (InvalidRegionCodeException ex)
      {
        ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
        throw;
      }
    }
  }
}
