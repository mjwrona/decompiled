// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.ComputerInfo
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Microsoft.TeamFoundation.Common
{
  public static class ComputerInfo
  {
    private const int c_errorBadNetPath = 53;
    private static string s_computerDomain;
    private static string s_injectMachineName;

    public static string ComputerDomain
    {
      get
      {
        if (ComputerInfo.s_computerDomain == null && !OSDetails.IsMachineInWorkgroup())
        {
          string domain;
          Microsoft.TeamFoundation.Common.Internal.NativeMethods.NetGetJoinInformation((string) null, out domain, out Microsoft.TeamFoundation.Common.Internal.NativeMethods.NetJoinStatus _);
          ComputerInfo.s_computerDomain = domain;
        }
        return ComputerInfo.s_computerDomain;
      }
    }

    public static string MachineName
    {
      get
      {
        string machineName = ComputerInfo.s_injectMachineName;
        if (string.IsNullOrEmpty(machineName))
          machineName = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetComputerNameEx(Microsoft.TeamFoundation.Common.Internal.NativeMethods.COMPUTER_NAME_FORMAT.ComputerNameDnsHostname);
        return machineName;
      }
    }

    public static bool IsSameMachine(string hostname1, string hostname2)
    {
      try
      {
        AdminTraceLogger.Default.Verbose("comparing '{0}' to '{1}'", (object) hostname1, (object) hostname2);
        return UriUtility.IsSameMachine(hostname1, hostname2);
      }
      catch (Exception ex)
      {
        AdminTraceLogger.Default.Error("Failed IsSameMachine {0}:{1} - {2}", new object[3]
        {
          (object) hostname1,
          (object) hostname2,
          (object) ex.ToString()
        });
        throw;
      }
    }

    public static bool DnsSafeIsSameMachine(string hostname1, string hostname2)
    {
      if (ComputerInfo.IsSqlAzureHost(hostname1) || ComputerInfo.IsSqlAzureHost(hostname2))
        return false;
      bool flag = false;
      if (!string.IsNullOrEmpty(ComputerInfo.s_injectMachineName) && (string.Equals(hostname1, ComputerInfo.s_injectMachineName, StringComparison.OrdinalIgnoreCase) || string.Equals(hostname2, ComputerInfo.s_injectMachineName, StringComparison.OrdinalIgnoreCase)))
      {
        flag = string.Equals(hostname1, hostname2, StringComparison.OrdinalIgnoreCase);
      }
      else
      {
        try
        {
          flag = UriUtility.IsSameMachine(hostname1, hostname2);
        }
        catch (SocketException ex)
        {
          AdminTraceLogger.Default.Warning("IsSameMachine threw exception");
          AdminTraceLogger.Default.Error((Exception) ex);
        }
      }
      return flag;
    }

    public static bool IsLocalMachine(string hostname)
    {
      AdminTraceLogger.Default.Verbose("IsLocalMachine: comparing '{0}' to matching name '{1}'", (object) hostname, (object) ComputerInfo.MachineName);
      bool flag = true;
      if (string.Compare(hostname, "localhost", StringComparison.OrdinalIgnoreCase) != 0)
      {
        flag = ComputerInfo.DnsSafeIsSameMachine(ComputerInfo.MachineName, hostname);
        if (!flag && ComputerInfo.MachineName.Length > 15)
          flag = ComputerInfo.DnsSafeIsSameMachine(UserNameUtil.NetBiosName, hostname);
      }
      AdminTraceLogger.Default.Verbose("isSameMachine={0}", (object) flag);
      return flag;
    }

    public static bool IsCNameForLocalMachine(string hostName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hostName, nameof (hostName));
      try
      {
        return ComputerInfo.AreHostAddressesLocal(Array.FindAll<IPAddress>(Dns.GetHostAddresses(hostName), (Predicate<IPAddress>) (address => address.AddressFamily == AddressFamily.InterNetwork)), ((IEnumerable<NetworkInterface>) NetworkInterface.GetAllNetworkInterfaces()).SelectMany<NetworkInterface, UnicastIPAddressInformation>((Func<NetworkInterface, IEnumerable<UnicastIPAddressInformation>>) (ni => (IEnumerable<UnicastIPAddressInformation>) ni.GetIPProperties().UnicastAddresses)).Select<UnicastIPAddressInformation, IPAddress>((Func<UnicastIPAddressInformation, IPAddress>) (u => u.Address)).ToArray<IPAddress>());
      }
      catch (Exception ex)
      {
        AdminTraceLogger.Default.Info("Exception while trying to match host IP addresses to local machine IP addresses. Exception: " + ex.Message);
        return false;
      }
    }

    [Browsable(false)]
    internal static bool AreHostAddressesLocal(
      IPAddress[] hostAddresses,
      IPAddress[] localMachineAddresses)
    {
      ArgumentUtility.CheckForNull<IPAddress[]>(hostAddresses, nameof (hostAddresses));
      ArgumentUtility.CheckForNull<IPAddress[]>(localMachineAddresses, nameof (localMachineAddresses));
      bool flag = false;
      foreach (IPAddress hostAddress in hostAddresses)
      {
        IPAddress hostAddr = hostAddress;
        if (hostAddr.AddressFamily == AddressFamily.InterNetwork)
        {
          if (hostAddr.ToString().StartsWith("127."))
          {
            AdminTraceLogger.Default.Info(string.Format("Found a loopback IP address {0}. Continue..", (object) hostAddr));
            flag = true;
          }
          else
          {
            if (!Array.Exists<IPAddress>(localMachineAddresses, (Predicate<IPAddress>) (addr => addr.Equals((object) hostAddr))))
            {
              AdminTraceLogger.Default.Info(string.Format("IP address {0} for given host, doesn't match to any local machine IP addresses.", (object) hostAddr));
              return false;
            }
            flag = true;
            AdminTraceLogger.Default.Info(string.Format("IP address {0} for a given host is local.", (object) hostAddr));
          }
        }
      }
      return flag;
    }

    public static bool IsLocalMachine(Uri uri) => ComputerInfo.IsLocalMachine(uri.DnsSafeHost);

    public static string GetComputerDomain(string computerNameOrAddress, ITFLogger logger = null)
    {
      logger = logger ?? (ITFLogger) new TraceLogger();
      ArgumentUtility.CheckStringForNullOrEmpty(computerNameOrAddress, nameof (computerNameOrAddress));
      if (OSDetails.IsMachineInWorkgroup())
        throw new InvalidOperationException("GetComputerDomain method is only supported on machines joined to a domain.");
      string computerDomain;
      if (computerNameOrAddress.Equals(".", StringComparison.Ordinal))
      {
        computerDomain = ComputerInfo.ComputerDomain;
      }
      else
      {
        try
        {
          computerDomain = UserNameUtil.GetDomainName(ComputerInfo.GetComputerAccountUsingDsCrackNames(computerNameOrAddress, logger));
        }
        catch (Exception ex)
        {
          if (!ComputerInfo.TryGetComputerDomainUsingNetGetJoinInformation(computerNameOrAddress, out computerDomain, logger))
            throw;
        }
      }
      return computerDomain;
    }

    public static string GetComputerAccount(string computerNameOrAddress, ITFLogger logger = null)
    {
      logger = logger ?? (ITFLogger) new TraceLogger();
      ArgumentUtility.CheckStringForNullOrEmpty(computerNameOrAddress, nameof (computerNameOrAddress));
      if (OSDetails.IsMachineInWorkgroup())
        throw new InvalidOperationException("GetComputerAccount method is only supported on machines joined to a domain.");
      return !computerNameOrAddress.Equals(".", StringComparison.Ordinal) ? ComputerInfo.GetComputerAccountUsingDsCrackNames(computerNameOrAddress, logger) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}$", (object) ComputerInfo.ComputerDomain, (object) Environment.MachineName);
    }

    public static bool IsSqlAzureHost(string hostname) => hostname != null && hostname.EndsWith("database.windows.net", StringComparison.OrdinalIgnoreCase);

    private static bool TryGetComputerDomainUsingNetGetJoinInformation(
      string computerNameOrAddress,
      out string computerDomain,
      ITFLogger logger)
    {
      bool getJoinInformation;
      try
      {
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.NetJoinStatus joinStatus;
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.NetGetJoinInformation(computerNameOrAddress, out computerDomain, out joinStatus);
        logger.Info("joinStatus = {0}", (object) joinStatus);
        logger.Info("Domain = {0}", (object) computerDomain);
        getJoinInformation = true;
      }
      catch (Win32Exception ex1)
      {
        if (ex1.NativeErrorCode == 53)
        {
          if (!IPAddress.TryParse(computerNameOrAddress, out IPAddress _))
          {
            logger.Info("Resolving {0} to IPHostEntry", (object) computerNameOrAddress);
            IPHostEntry hostEntry = Dns.GetHostEntry(computerNameOrAddress);
            logger.Info("HostName: {0}", (object) hostEntry.HostName);
            IPAddress ipAddress = ((IEnumerable<IPAddress>) hostEntry.AddressList).FirstOrDefault<IPAddress>();
            if (ipAddress != null)
            {
              try
              {
                logger.Info("Caling NetGetJoinInformation with {0}", (object) ipAddress);
                Microsoft.TeamFoundation.Common.Internal.NativeMethods.NetJoinStatus joinStatus;
                Microsoft.TeamFoundation.Common.Internal.NativeMethods.NetGetJoinInformation(ipAddress.ToString(), out computerDomain, out joinStatus);
                logger.Info("joinStatus = {0}", (object) joinStatus);
                logger.Info("Domain = {0}", (object) computerDomain);
                getJoinInformation = true;
              }
              catch (Win32Exception ex2)
              {
                logger.Error((Exception) ex2);
                computerDomain = (string) null;
                getJoinInformation = false;
              }
            }
            else
            {
              computerDomain = (string) null;
              getJoinInformation = false;
            }
          }
          else
          {
            computerDomain = (string) null;
            getJoinInformation = false;
          }
        }
        else
        {
          logger.Info("NetGetJoinInformation({0}) failed. Error: {1}. Message: {2}", (object) computerNameOrAddress, (object) ex1.NativeErrorCode, (object) ex1.Message);
          computerDomain = (string) null;
          getJoinInformation = false;
        }
      }
      return getJoinInformation;
    }

    private static string GetComputerAccountUsingDsCrackNames(
      string computerNameOrAddress,
      ITFLogger logger)
    {
      logger.Info("Resolving {0} to IPHostEntry", (object) computerNameOrAddress);
      IPHostEntry hostEntry = Dns.GetHostEntry(computerNameOrAddress);
      logger.Info("HostName: {0}", (object) hostEntry.HostName);
      string str = "host/" + hostEntry.HostName;
      IntPtr phDS;
      uint num1 = Microsoft.TeamFoundation.Common.Internal.NativeMethods.DsBind((string) null, (string) null, out phDS);
      if (num1 != 0U)
      {
        logger.Error("NativeMethods.DsBind failed. Error: {0}", (object) num1);
        throw new ApplicationException(TFCommonResources.CannotResolveServerHostUsingAD((object) str, (object) num1));
      }
      try
      {
        string[] names = new string[1]{ str };
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_RESULT_ITEM[] dsNameResultItemArray = Microsoft.TeamFoundation.Common.Internal.NativeMethods.DsCrackNames(phDS, Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_FLAGS.DS_NAME_NO_FLAGS, Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_FORMAT.DS_SERVICE_PRINCIPAL_NAME, Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_FORMAT.DS_NT4_ACCOUNT_NAME, names);
        if (dsNameResultItemArray[0].status != Microsoft.TeamFoundation.Common.Internal.NativeMethods.DS_NAME_ERROR.DS_NAME_NO_ERROR)
          throw new ApplicationException(TFCommonResources.CannotResolveServerHostUsingAD((object) str, (object) dsNameResultItemArray[0].status));
        logger.Info("Account = {0}", (object) dsNameResultItemArray[0].pName);
        return dsNameResultItemArray[0].pName;
      }
      finally
      {
        int num2 = (int) Microsoft.TeamFoundation.Common.Internal.NativeMethods.DsUnBind(ref phDS);
      }
    }
  }
}
