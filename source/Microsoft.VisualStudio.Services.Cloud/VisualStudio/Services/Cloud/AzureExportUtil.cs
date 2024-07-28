// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureExportUtil
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class AzureExportUtil
  {
    private const string c_pattern = "data\\.([A-Za-z]{2})([A-Za-z0-9\\-]+)\\.database\\.windows\\.net";
    private const string c_sterlingPattern = "([A-Za-z]+[0-9]+)-([A-Za-z]+)\\.control\\.database\\.windows\\.net";
    private static readonly List<Regex> s_regex = new List<Regex>()
    {
      new Regex("data\\.([A-Za-z]{2})([A-Za-z0-9\\-]+)\\.database\\.windows\\.net", RegexOptions.Compiled),
      new Regex("([A-Za-z]+[0-9]+)-([A-Za-z]+)\\.control\\.database\\.windows\\.net", RegexOptions.Compiled)
    };
    private static readonly Dictionary<string, string> s_exportServiceLookup = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "CH",
        "https://ch1prod-dacsvc.azure.com/DACWebService.svc"
      },
      {
        "SN",
        "https://sn1prod-dacsvc.azure.com/DACWebService.svc"
      },
      {
        "DB",
        "https://db3prod-dacsvc.azure.com/DACWebService.svc"
      },
      {
        "AM",
        "https://am1prod-dacsvc.azure.com/DACWebService.svc"
      },
      {
        "HK",
        "https://hkgprod-dacsvc.azure.com/DACWebService.svc"
      },
      {
        "SG",
        "https://sg1prod-dacsvc.azure.com/DACWebService.svc"
      },
      {
        "BL",
        "https://bl2prod-dacsvc.azure.com/DACWebService.svc"
      },
      {
        "BN",
        "https://bn1prod-dacsvc.azure.com/DACWebService.svc"
      },
      {
        "BY",
        "https://by1prod-dacsvc.azure.com/DACWebService.svc"
      },
      {
        "EASTUS",
        "https://bl2prod-dacsvc.azure.com/DACWebService.svc"
      },
      {
        "EASTUS1",
        "https://bl2prod-dacsvc.azure.com/DACWebService.svc"
      },
      {
        "EASTUS2",
        "https://bn1prod-dacsvc.azure.com/DACWebService.svc"
      },
      {
        "southeastasia1",
        "https://sg1prod-dacsvc.azure.com/DACWebService.svc"
      }
    };

    public static DACWebServiceClient CreateDacClient(ITFLogger logger, string sqlInstance)
    {
      sqlInstance = TeamFoundationDataTierService.RemoveProtocol(sqlInstance);
      return new DACWebServiceClient("dacWebServicesEndpoint", AzureExportUtil.GetServiceEndpointFromServerName(logger, sqlInstance));
    }

    public static string GetServiceEndpointFromServerName(ITFLogger logger, string serverName)
    {
      logger.Info("Getting export service endpoint for server name {0}", (object) serverName);
      string endpointFromServerName1 = "https://ch1prod-dacsvc.azure.com/DACWebService.svc";
      IPHostEntry hostEntry;
      try
      {
        hostEntry = Dns.GetHostEntry(serverName);
      }
      catch (Exception ex)
      {
        logger.Info("Exception looking for host entry: {0}. Returning default service url.", (object) ex);
        return endpointFromServerName1;
      }
      Match match = (Match) null;
      foreach (Regex regex in AzureExportUtil.s_regex)
      {
        match = regex.Match(hostEntry.HostName);
        if (match.Success)
          break;
      }
      if (match != null && !match.Success)
      {
        logger.Info("Unable to detect data center for the server name {0} with host name {1}. Returning default service url.", (object) serverName, (object) hostEntry.HostName);
        return endpointFromServerName1;
      }
      string str = match.Groups[1].Value;
      logger.Info("Looking up service Uri for alias: {0}", (object) str);
      string endpointFromServerName2;
      if (AzureExportUtil.s_exportServiceLookup.TryGetValue(str.ToUpper(), out endpointFromServerName2))
      {
        logger.Info("Found service Uri {0}", (object) endpointFromServerName2);
        return endpointFromServerName2;
      }
      logger.Info("Did not find service Uri, returning default service url");
      return endpointFromServerName1;
    }
  }
}
