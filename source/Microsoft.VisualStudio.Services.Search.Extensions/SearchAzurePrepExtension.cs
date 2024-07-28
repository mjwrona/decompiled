// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.SearchAzurePrepExtension
// Assembly: Microsoft.VisualStudio.Services.Search.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1D8FF195-304B-4BBA-9D1C-F4A6093CE2E1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Extensions.dll

using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Configuration;
using Microsoft.VisualStudio.Services.VssAzurePrep;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Search.Extensions
{
  public class SearchAzurePrepExtension : IVssAzurePrepExtension
  {
    private const string SearchEnableIpSecEnvVar = "ALMSearch_EnableIPSec";
    private const string SearchEnforceIpSecEnvVar = "ALMSearch_EnforceIPSec";
    private const string SearchEnableIpSecValue = "1";
    private const string SearchEnforceIpSecValue = "1";
    private const string c_processName = "RunParser.exe";
    private const string c_firewallRuleName = "AlmSearch:BlockInternetForRunParser";

    public void Unconfigure()
    {
    }

    public void Configure()
    {
      string processPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar.ToString() + "RunParser.exe";
      Trace.TraceInformation("ConfigureRulesForRunParser started for {0}.", (object) processPath);
      SearchAzurePrepExtension.FirewallRuleToBlockNetworkAccess("AlmSearch:BlockInternetForRunParser", processPath);
      SearchAzurePrepExtension.GetConfigurationSetting("HostedServiceName");
      Trace.TraceInformation("ConfigureRulesForRunParser finished for {0}.", (object) processPath);
    }

    private static string GetConfigurationSetting(string settingName)
    {
      string str = "";
      return AzureRoleUtil.Configuration.Settings.TryGetValue(settingName, out str) ? str : "";
    }

    private static void FirewallRuleToBlockNetworkAccess(string ruleName, string processPath)
    {
      try
      {
        INetFwPolicy2 instance1 = (INetFwPolicy2) Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
        foreach (INetFwRule rule in (IEnumerable) instance1.Rules)
        {
          if (string.Equals(rule.Name, ruleName, StringComparison.OrdinalIgnoreCase))
            instance1.Rules.Remove(ruleName);
        }
        INetFwRule instance2 = (INetFwRule) Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
        instance2.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
        instance2.Description = "Used to block all internet access for RunParser process.";
        instance2.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
        instance2.Enabled = true;
        instance2.ApplicationName = processPath;
        instance2.InterfaceTypes = "All";
        instance2.Name = ruleName;
        if (!File.Exists(processPath))
          return;
        instance1.Rules.Add(instance2);
      }
      catch (Exception ex)
      {
        Trace.TraceInformation("Adding firewall rule failed with exception: {0}", (object) ex);
      }
    }
  }
}
