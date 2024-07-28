// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SmartRouter.IpResolution.IpAddressUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;


#nullable enable
namespace Microsoft.TeamFoundation.Framework.Server.SmartRouter.IpResolution
{
  internal static class IpAddressUtility
  {
    private const string c_AzureInstanceMetadataVersion = "2021-02-01";

    internal static (string? ipAddress, IPAddress? parsedIpAddress) GetLocalHostIpAddress(
      IVssRequestContext requestContext)
    {
      IpAddressUtility.IpAddressValues valueOrDefault = IpAddressUtility.TryGetLocalHostIPAddress(requestContext).GetValueOrDefault();
      return (valueOrDefault.IpAddress, valueOrDefault.ParsedIpAddress);
    }

    private static IpAddressUtility.IpAddressValues? TryGetLocalHostIPAddress(
      IVssRequestContext requestContext)
    {
      IpAddressUtility.IpAddressValues? localHostIpAddress = IpAddressUtility.CachedLocalHostIpAddress;
      if (localHostIpAddress.HasValue)
        return localHostIpAddress;
      string str = (string) null;
      if (requestContext.ExecutionEnvironment.IsCloudDeployment)
      {
        str = IpAddressUtility.GetAzureInstanceNetworkMetadataPrivateIpAddress();
        requestContext.Trace(34005510, TraceLevel.Info, "SmartRouter", "Server", "cloud hosted ipAddress='{0}'", (object) str);
        if (string.IsNullOrEmpty(str))
          requestContext.Trace(34005511, TraceLevel.Warning, "SmartRouter", "Server", "no private ip address found for cloud hosted environment");
      }
      else if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
      {
        str = new Uri(IpAddressUtility.GetAzureInstanceMappingAccessPoint(requestContext)).Host;
        requestContext.Trace(34005510, TraceLevel.Info, "SmartRouter", "Server", "devFabric host={0}", (object) str);
      }
      else
        requestContext.Trace(34005511, TraceLevel.Warning, "SmartRouter", "Server", "no ipAddress implemented for on-premise deployment: {0}", (object) requestContext.ExecutionEnvironment.Flags.ToString());
      IpAddressUtility.CachedLocalHostIpAddress = new IpAddressUtility.IpAddressValues?();
      IPAddress address;
      if (!string.IsNullOrEmpty(str) && IPAddress.TryParse(str, out address))
        IpAddressUtility.CachedLocalHostIpAddress = new IpAddressUtility.IpAddressValues?(new IpAddressUtility.IpAddressValues(str, address));
      return IpAddressUtility.CachedLocalHostIpAddress;
    }

    private static string? GetAzureInstanceNetworkMetadataPrivateIpAddress()
    {
      string privateIpAddress = (string) null;
      try
      {
        NetworkMetadata network = JsonUtilities.Deserialize<AzureInstanceMetadata>(IpAddressUtility.MetadataProvider.GetMetadata("instance", new Dictionary<string, string>()
        {
          {
            "format",
            "json"
          }
        })).Network;
        string str;
        if (network == null)
        {
          str = (string) null;
        }
        else
        {
          NetworkMetadata.InterfaceMetadata interfaceMetadata = network.Interfaces.FirstOrDefault<NetworkMetadata.InterfaceMetadata>();
          if (interfaceMetadata == null)
          {
            str = (string) null;
          }
          else
          {
            NetworkMetadata.IPv4Metadata ipv4 = interfaceMetadata.IPv4;
            if (ipv4 == null)
            {
              str = (string) null;
            }
            else
            {
              List<NetworkMetadata.IPAddressMetadata> ipAddress = ipv4.IPAddress;
              str = ipAddress != null ? ipAddress.FirstOrDefault<NetworkMetadata.IPAddressMetadata>()?.PrivateIpAddress : (string) null;
            }
          }
        }
        privateIpAddress = str;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(34005512, TraceLevel.Error, "SmartRouter", "Server", ex);
      }
      return privateIpAddress;
    }

    private static IpAddressUtility.IpAddressValues? CachedLocalHostIpAddress { get; set; }

    private static string GetAzureInstanceMappingAccessPoint(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, AccessMappingConstants.AzureInstanceMappingMoniker).AccessPoint;

    private static IAzureInstanceMetadataProvider MetadataProvider { get; } = (IAzureInstanceMetadataProvider) new AzureInstanceMetadataProvider(new HttpClient(), "2021-02-01");

    private struct IpAddressValues
    {
      public IpAddressValues(string ipAddress, IPAddress parsedIpAddress)
      {
        this.IpAddress = ipAddress;
        this.ParsedIpAddress = parsedIpAddress;
      }

      public string IpAddress { get; }

      public IPAddress ParsedIpAddress { get; }
    }
  }
}
