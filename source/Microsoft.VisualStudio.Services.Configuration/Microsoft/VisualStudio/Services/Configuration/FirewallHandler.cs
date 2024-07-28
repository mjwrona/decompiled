// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.FirewallHandler
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class FirewallHandler
  {
    public static event EventHandler<FirewallPortAddedEventArgs> FirewallPortAdded;

    public static bool IsInstalled
    {
      get
      {
        INetFwMgr firewallManager = FirewallHandler.CreateFirewallManager();
        bool isInstalled = false;
        try
        {
          if (firewallManager.LocalPolicy.CurrentProfile != null)
            isInstalled = true;
        }
        catch (COMException ex)
        {
          TeamFoundationTrace.TraceException("netFirewallManager.LocalPolicy.CurrentProfile threw exception", "FirewallHandler.IsInstalled", (Exception) ex);
        }
        return isInstalled;
      }
    }

    public static bool IsEnabled
    {
      get
      {
        bool isEnabled = false;
        if (FirewallHandler.IsInstalled)
        {
          try
          {
            isEnabled = FirewallHandler.CreateFirewallManager().LocalPolicy.CurrentProfile.FirewallEnabled;
          }
          catch (COMException ex)
          {
            TeamFoundationTrace.TraceException("netFirewallManager.LocalPolicy.CurrentProfile.FirewallEnabled threw exception", "FirewallHandler.IsEnabled", (Exception) ex);
            throw;
          }
        }
        return isEnabled;
      }
    }

    public static bool ExceptionsAllowed
    {
      get
      {
        bool exceptionsAllowed = false;
        if (FirewallHandler.IsInstalled)
          exceptionsAllowed = !FirewallHandler.CreateFirewallManager().LocalPolicy.CurrentProfile.ExceptionsNotAllowed;
        return exceptionsAllowed;
      }
    }

    public static INetFwOpenPorts GloballyOpenPorts => FirewallHandler.CreateFirewallManager().LocalPolicy.CurrentProfile.GloballyOpenPorts;

    public static INetFwProfile DomainProfile => FirewallHandler.CreateFirewallManager().LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_DOMAIN);

    public static INetFwProfile StandardProfile => FirewallHandler.CreateFirewallManager().LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_STANDARD);

    public static bool AddExceptionToFirewall(string name, int port)
    {
      INetFwMgr instance = (INetFwMgr) Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
      bool flag1 = false;
      INetFwProfile profileByType1 = instance.LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_STANDARD);
      if (profileByType1 != null)
        flag1 = FirewallHandler.AddExceptionToFirewallProfile(name, port, profileByType1);
      bool flag2 = false;
      INetFwProfile profileByType2 = instance.LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_DOMAIN);
      if (profileByType2 != null)
        flag2 = FirewallHandler.AddExceptionToFirewallProfile(name, port, profileByType2);
      return flag1 | flag2;
    }

    private static bool AddExceptionToFirewallProfile(string name, int port, INetFwProfile profile)
    {
      INetFwOpenPort netFwOpenPort = (INetFwOpenPort) null;
      foreach (INetFwOpenPort globallyOpenPort in (IEnumerable) profile.GloballyOpenPorts)
      {
        if (globallyOpenPort.Port == port)
        {
          netFwOpenPort = globallyOpenPort;
          break;
        }
      }
      if (netFwOpenPort == null)
      {
        INetFwOpenPort instance = (INetFwOpenPort) Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwOpenPort"));
        instance.Enabled = true;
        instance.Name = name;
        instance.Port = port;
        instance.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
        instance.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
        profile.GloballyOpenPorts.Add(instance);
        return true;
      }
      if (!netFwOpenPort.Enabled)
        netFwOpenPort.Enabled = true;
      return false;
    }

    public static bool DeleteExceptionFromFirewall(string name, int port)
    {
      INetFwMgr firewallManager = FirewallHandler.CreateFirewallManager();
      bool flag1 = false;
      INetFwProfile profileByType1 = firewallManager.LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_STANDARD);
      if (profileByType1 != null)
        flag1 = FirewallHandler.DeleteExceptionFromFirewallProfile(name, port, profileByType1);
      bool flag2 = false;
      INetFwProfile profileByType2 = firewallManager.LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_DOMAIN);
      if (profileByType2 != null)
        flag2 = FirewallHandler.DeleteExceptionFromFirewallProfile(name, port, profileByType2);
      return flag1 | flag2;
    }

    public static bool DeleteExceptionFromFirewall(string name, string applicationName)
    {
      INetFwMgr firewallManager = FirewallHandler.CreateFirewallManager();
      bool flag1 = false;
      INetFwProfile profileByType1 = firewallManager.LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_STANDARD);
      if (profileByType1 != null)
        flag1 = FirewallHandler.DeleteExceptionFromFirewallProfile(name, applicationName, profileByType1);
      bool flag2 = false;
      INetFwProfile profileByType2 = firewallManager.LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_DOMAIN);
      if (profileByType2 != null)
        flag2 = FirewallHandler.DeleteExceptionFromFirewallProfile(name, applicationName, profileByType2);
      return flag1 | flag2;
    }

    public static bool EnableFirewallException(string ruleName, string applicationName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(ruleName, nameof (ruleName));
      ArgumentUtility.CheckStringForNullOrEmpty(applicationName, nameof (applicationName));
      INetFwPolicy2 firewallPolicy = FirewallHandler.CreateFirewallPolicy();
      int num = 3;
      foreach (INetFwRule rule in (IEnumerable) firewallPolicy.Rules)
      {
        if (string.Equals(rule.Name, ruleName, StringComparison.OrdinalIgnoreCase) && string.Equals(rule.ApplicationName, applicationName, StringComparison.Ordinal))
        {
          if ((rule.Profiles & num) != num)
            rule.Profiles |= num;
          return false;
        }
      }
      INetFwRule firewallRule = FirewallHandler.CreateFirewallRule(ruleName);
      firewallRule.ApplicationName = applicationName;
      firewallPolicy.Rules.Add(firewallRule);
      EventHandler<FirewallPortAddedEventArgs> firewallPortAdded = FirewallHandler.FirewallPortAdded;
      if (firewallPortAdded != null)
        firewallPortAdded((object) null, new FirewallPortAddedEventArgs()
        {
          ApplicationName = applicationName
        });
      return true;
    }

    public static bool EnableFirewallException(string ruleName, int port)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(ruleName, nameof (ruleName));
      INetFwPolicy2 firewallPolicy = FirewallHandler.CreateFirewallPolicy();
      int num = 3;
      string b = port.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      foreach (INetFwRule rule in (IEnumerable) firewallPolicy.Rules)
      {
        if (string.Equals(rule.Name, ruleName, StringComparison.OrdinalIgnoreCase) && string.Equals(rule.LocalPorts, b, StringComparison.Ordinal))
        {
          if ((rule.Profiles & num) != num)
            rule.Profiles |= num;
          return false;
        }
      }
      INetFwRule firewallRule = FirewallHandler.CreateFirewallRule(ruleName);
      firewallRule.LocalPorts = b;
      firewallPolicy.Rules.Add(firewallRule);
      EventHandler<FirewallPortAddedEventArgs> firewallPortAdded = FirewallHandler.FirewallPortAdded;
      if (firewallPortAdded != null)
        firewallPortAdded((object) null, new FirewallPortAddedEventArgs()
        {
          Port = port
        });
      return true;
    }

    public static bool DisableFirewallException(string ruleName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(ruleName, nameof (ruleName));
      INetFwPolicy2 firewallPolicy = FirewallHandler.CreateFirewallPolicy();
      bool flag = false;
      foreach (INetFwRule rule in (IEnumerable) firewallPolicy.Rules)
      {
        if (string.Equals(rule.Name, ruleName, StringComparison.OrdinalIgnoreCase))
        {
          firewallPolicy.Rules.Remove(ruleName);
          flag = true;
        }
      }
      foreach (INetFwRule rule in (IEnumerable) firewallPolicy.Rules)
      {
        if (string.Equals(rule.Name, ruleName, StringComparison.OrdinalIgnoreCase))
        {
          firewallPolicy.Rules.Remove(ruleName);
          flag = true;
        }
      }
      return flag;
    }

    public static INetFwOpenPort FindException(int port, string ruleNamePrefix)
    {
      INetFwProfile[] netFwProfileArray = new INetFwProfile[2]
      {
        FirewallHandler.StandardProfile,
        FirewallHandler.DomainProfile
      };
      foreach (INetFwProfile netFwProfile in netFwProfileArray)
      {
        if (netFwProfile != null)
        {
          foreach (INetFwOpenPort globallyOpenPort in (IEnumerable) netFwProfile.GloballyOpenPorts)
          {
            if (globallyOpenPort.Port == port && !globallyOpenPort.BuiltIn && globallyOpenPort.Enabled && globallyOpenPort.Scope == NET_FW_SCOPE_.NET_FW_SCOPE_ALL && globallyOpenPort.Protocol == NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP && globallyOpenPort.RemoteAddresses == "*" && globallyOpenPort.Name.StartsWith(ruleNamePrefix, StringComparison.OrdinalIgnoreCase))
              return globallyOpenPort;
          }
        }
      }
      return (INetFwOpenPort) null;
    }

    private static bool DeleteExceptionFromFirewallProfile(
      string name,
      int port,
      INetFwProfile profile)
    {
      foreach (INetFwOpenPort globallyOpenPort in (IEnumerable) profile.GloballyOpenPorts)
      {
        if (globallyOpenPort.Port == port)
        {
          if (globallyOpenPort.Name == name)
          {
            try
            {
              profile.GloballyOpenPorts.Remove(port, globallyOpenPort.Protocol);
            }
            catch (FileNotFoundException ex)
            {
              Thread.Sleep(50);
              profile.GloballyOpenPorts.Remove(port, globallyOpenPort.Protocol);
            }
            return true;
          }
        }
      }
      return false;
    }

    private static bool DeleteExceptionFromFirewallProfile(
      string name,
      string applicationName,
      INetFwProfile profile)
    {
      foreach (INetFwAuthorizedApplication authorizedApplication in (IEnumerable) profile.AuthorizedApplications)
      {
        if (authorizedApplication.Name == name)
        {
          if (string.Equals(authorizedApplication.ProcessImageFileName, applicationName, StringComparison.OrdinalIgnoreCase))
          {
            try
            {
              profile.AuthorizedApplications.Remove(authorizedApplication.ProcessImageFileName);
            }
            catch (FileNotFoundException ex)
            {
              Thread.Sleep(50);
              profile.AuthorizedApplications.Remove(authorizedApplication.ProcessImageFileName);
            }
            return true;
          }
        }
      }
      return false;
    }

    private static INetFwMgr CreateFirewallManager() => (INetFwMgr) Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));

    private static INetFwPolicy2 CreateFirewallPolicy() => (INetFwPolicy2) Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

    private static INetFwRule CreateFirewallRule(string ruleName)
    {
      INetFwRule instance = (INetFwRule) Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
      instance.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
      instance.Description = ruleName;
      instance.Profiles = 3;
      instance.Enabled = true;
      instance.Name = ruleName;
      instance.Protocol = 6;
      return instance;
    }
  }
}
