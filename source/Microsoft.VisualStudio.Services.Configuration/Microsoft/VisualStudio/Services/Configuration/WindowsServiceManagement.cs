// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.WindowsServiceManagement
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class WindowsServiceManagement
  {
    public const string HklmServicesKey = "SYSTEM\\CurrentControlSet\\Services\\";
    public const string HklmServiceControlKey = "SYSTEM\\CurrentControlSet\\Control\\";
    private const string c_cimv2Scope = "root\\CIMV2";
    private static readonly TimeSpan s_maximumTimeForServiceProcessToTerminate = new TimeSpan(0, 0, 60);

    public static string PathToInstallUtil => NetFramework.Get40FrameworkFilePath("InstallUtil.exe");

    public static string PathToSC => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "sc.exe");

    public static void StartWindowsService(string serviceName, ITFLogger logger) => WindowsServiceManagement.StartWindowsService(serviceName, ComputerInfo.MachineName, logger);

    public static void StartWindowsService(string serviceName, string hostName, ITFLogger logger) => WindowsServiceManagement.StartWindowsService(serviceName, hostName, Array.Empty<string>(), logger);

    public static void StartWindowsService(
      string serviceName,
      string hostName,
      string[] args,
      ITFLogger logger)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      ArgumentUtility.CheckStringForNullOrEmpty(hostName, nameof (hostName));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      logger.Info("StartWindowsService {0} on {1}", (object) serviceName, (object) hostName);
      using (ServiceController serviceController = new ServiceController(serviceName, hostName))
      {
        logger.Info("Current State {0}", (object) serviceController.Status);
        if (serviceController.Status == ServiceControllerStatus.StopPending)
        {
          logger.Info("StopPending - wait for stop");
          WindowsServiceManagement.WaitForStatus(serviceController, ServiceControllerStatus.Stopped, logger);
        }
        if (serviceController.Status != ServiceControllerStatus.Running)
        {
          if (serviceController.Status != ServiceControllerStatus.StartPending)
          {
            logger.Info(ConfigurationResources.StartService((object) serviceName));
            WindowsServiceManagement.StartService(serviceController, args, logger);
          }
          WindowsServiceManagement.WaitForStatus(serviceController, ServiceControllerStatus.Running, logger);
        }
        logger.Info("Success");
      }
    }

    public static void StopWindowsService(string serviceName, ITFLogger logger) => WindowsServiceManagement.StopWindowsService(serviceName, ComputerInfo.MachineName, logger);

    public static void StopWindowsService(string serviceName, string hostname, ITFLogger logger) => WindowsServiceManagement.StopWindowsService(serviceName, hostname, TimeSpan.Zero, logger);

    public static void StopWindowsService(
      string serviceName,
      string hostname,
      TimeSpan retryDelay,
      ITFLogger logger)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      ArgumentUtility.CheckStringForNullOrEmpty(hostname, nameof (hostname));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      logger.Info("StopWindowsService {0} on {1}", (object) serviceName, (object) hostname);
      using (ServiceController serviceController = new ServiceController(serviceName, hostname))
      {
        logger.Info("Current State {0}", (object) serviceController.Status);
        if (serviceController.Status == ServiceControllerStatus.StartPending)
        {
          logger.Info("StartPending - wait for start");
          WindowsServiceManagement.WaitForStatus(serviceController, ServiceControllerStatus.Running, logger);
        }
        if (serviceController.Status != ServiceControllerStatus.Stopped)
        {
          if (serviceController.Status != ServiceControllerStatus.StopPending)
          {
            if (!serviceController.CanStop)
              throw new InvalidOperationException(ConfigurationResources.ServiceCantBeStopped((object) serviceName, (object) serviceController.Status));
            logger.Info(ConfigurationResources.StopService((object) serviceName));
            WindowsServiceManagement.StopService(serviceController, logger, retryDelay);
          }
          WindowsServiceManagement.WaitForStatus(serviceController, ServiceControllerStatus.Stopped, logger);
        }
        logger.Info("Success stopping service.");
      }
      if (!ComputerInfo.IsLocalMachine(hostname))
        return;
      string pathForService = WindowsServiceManagement.GetPathForService(serviceName);
      if (pathForService == null)
        return;
      string fileName = Path.GetFileName(pathForService);
      Stopwatch stopwatch = Stopwatch.StartNew();
      while (stopwatch.Elapsed < WindowsServiceManagement.s_maximumTimeForServiceProcessToTerminate && WindowsServiceManagement.GetServiceProcess(pathForService, logger) != null)
      {
        logger.Info(ConfigurationResources.ServiceExecutableStillRunning((object) fileName, (object) serviceName));
        Thread.Sleep(1000);
      }
      if (WindowsServiceManagement.GetServiceProcess(pathForService, logger) != null)
        logger.Warning(ConfigurationResources.ServiceExecutableStillRunning((object) fileName, (object) serviceName));
      else
        logger.Info("Service process is gone.");
    }

    private static Process GetServiceProcess(string path, ITFLogger logger)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      string withoutExtension = Path.GetFileNameWithoutExtension(path);
      bool flag1 = true;
      int num1 = 0;
      HashSet<int> intSet = new HashSet<int>();
      while (flag1)
      {
        flag1 = false;
        Process[] processesByName = Process.GetProcessesByName(withoutExtension);
        List<Process> source = new List<Process>();
        foreach (Process process in processesByName)
        {
          int num2 = -1;
          try
          {
            num2 = process.Id;
            string path1 = (string) null;
            try
            {
              path1 = process.MainModule.FileName;
            }
            catch (Win32Exception ex1)
            {
              try
              {
                using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", string.Format("SELECT ExecutablePath FROM Win32_Process WHERE ProcessId = {0}", (object) num2)))
                {
                  using (ManagementObjectCollection objectCollection = managementObjectSearcher.Get())
                  {
                    logger.Info(string.Format("ManagementObjectSearcher found {0} objects with ProcessId {1}", (object) objectCollection.Count, (object) num2));
                    using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = objectCollection.GetEnumerator())
                    {
                      if (enumerator.MoveNext())
                      {
                        path1 = (string) enumerator.Current["ExecutablePath"];
                        if (string.IsNullOrEmpty(path1))
                          intSet.Add(num2);
                      }
                    }
                    logger.Info("ExecutablePath obtained from ManagementObjectSearcher: " + path1);
                  }
                }
              }
              catch (Exception ex2)
              {
                logger.Error("Exception while attempting to deal with the Win32Exception thrown by process.MainModule.FileName:");
                logger.Error(ex2);
              }
              if (string.IsNullOrEmpty(path1))
                throw;
            }
            if (Path.GetFullPath(path1).Equals(Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase))
              source.Add(process);
          }
          catch (Exception ex)
          {
            logger.Info("Failed to query MainModules.FileName of {0} for pid {1}.  Exception: {2} {3}", (object) withoutExtension, (object) num2, (object) ex.Message, (object) ex.StackTrace);
            ++num1;
            if (num1 > 3)
            {
              if (IntPtr.Size == 4)
                logger.Info("This code is running in a 32-bit process.");
              bool flag2 = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
              logger.Info(string.Format("This process currently elevated: {0}", (object) flag2));
              if (intSet.Contains(num2))
                logger.Info(string.Format("Ignoring {0}, were unable to obtain Process's executable path", (object) num2));
              else
                throw;
            }
            else
              flag1 = !process.WaitForExit((int) TimeSpan.FromMinutes(1.0).TotalMilliseconds);
          }
        }
        if (!flag1 && source.Count > 0)
        {
          if (source.Count == 1)
            return source[0];
          List<Process> list = source.Where<Process>((Func<Process, bool>) (p => p.SessionId == 0)).ToList<Process>();
          if (list.Count == 1)
            return list[0];
        }
      }
      return (Process) null;
    }

    public static void UninstallServiceIfInstalled(
      string serviceName,
      bool bestEffort,
      ITFLogger logger)
    {
      WindowsServiceManagement.UninstallServiceIfInstalled(serviceName, bestEffort, logger, new string[0]);
    }

    public static void UninstallServiceIfInstalled(
      string serviceName,
      bool bestEffort,
      ITFLogger logger,
      params string[] uninstallArgs)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      Func<string, ProcessHandler> getUninstaller = (Func<string, ProcessHandler>) (path =>
      {
        CommandLineBuilder commandLineBuilder = new CommandLineBuilder();
        foreach (string uninstallArg in uninstallArgs)
          commandLineBuilder.Append(uninstallArg);
        commandLineBuilder.Append("/u");
        commandLineBuilder.Append(path);
        commandLineBuilder.Append("/LogFile=");
        commandLineBuilder.Append("/LogToConsole=true");
        return new ProcessHandler(new ProcessStartInfo()
        {
          FileName = WindowsServiceManagement.PathToInstallUtil,
          Arguments = commandLineBuilder.ToString()
        }, commandLineBuilder.ToHiddenString(), ProcessHandlerOptions.None);
      });
      WindowsServiceManagement.UninstallServiceIfInstalled(serviceName, bestEffort, logger, getUninstaller);
    }

    public static void UninstallServiceIfInstalled(
      string serviceName,
      bool bestEffort,
      ITFLogger logger,
      Func<string, ProcessHandler> getUninstaller)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      ArgumentUtility.CheckForNull<Func<string, ProcessHandler>>(getUninstaller, nameof (getUninstaller));
      string pathForService = WindowsServiceManagement.GetPathForService(serviceName);
      if (string.IsNullOrEmpty(pathForService))
        logger.Info(ConfigurationResources.SkippingUninstallOfService((object) serviceName));
      else if (!File.Exists(pathForService))
      {
        WindowsServiceManagement.UninstallServiceUsingSC(serviceName, bestEffort, logger);
      }
      else
      {
        try
        {
          logger.Info(ConfigurationResources.AttemptingToRemoveRecoveryActions((object) serviceName));
          using (ServiceControlManagerUtility controlManagerUtility = new ServiceControlManagerUtility())
            controlManagerUtility.RemoveRecoveryActions(serviceName);
        }
        catch (Exception ex)
        {
          logger.Info(ConfigurationResources.ProblemRemovingRecoveryActions((object) serviceName, (object) ex.Message));
        }
        try
        {
          logger.Info(ConfigurationResources.AttemptingToStopService((object) serviceName));
          WindowsServiceManagement.StopWindowsService(serviceName, ComputerInfo.MachineName, logger);
        }
        catch (Exception ex)
        {
          logger.Info(ConfigurationResources.ProblemStoppingService((object) serviceName, (object) ex.Message));
        }
        Process serviceProcess = WindowsServiceManagement.GetServiceProcess(pathForService, logger);
        if (serviceProcess != null)
          WindowsServiceManagement.KillProcess(serviceProcess, ComputerInfo.MachineName, logger);
        logger.Info("Uninstalling {0}", (object) serviceName);
        string fileName = Path.GetFileName(pathForService);
        Path.GetFileNameWithoutExtension(pathForService);
        if (WindowsServiceManagement.GetServiceProcess(pathForService, logger) != null)
          logger.Warning(ConfigurationResources.ServiceExecutableStillRunning((object) fileName, (object) serviceName));
        logger.Info(ConfigurationResources.UninstallingService((object) serviceName));
        if (getUninstaller(pathForService).Run(logger).ExitCode != 0 && !bestEffort)
        {
          if (!string.IsNullOrEmpty(WindowsServiceManagement.GetPathForService(serviceName)))
            throw new ConfigurationException(ConfigurationResources.ServiceUninstallFailed((object) serviceName));
          logger.Info(serviceName + " is already uninstalled.");
        }
        for (int index = 0; index < 30; ++index)
        {
          using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\" + serviceName))
          {
            if (registryKey == null)
              break;
          }
          Thread.Sleep(1000);
        }
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\" + serviceName))
        {
          if (registryKey == null)
            return;
          logger.Info(ConfigurationResources.ServiceStillDefined((object) serviceName, registryKey.GetValue("Start"), registryKey.GetValue("DeleteFlag")));
          WindowsServiceManagement.UninstallServiceUsingSC(serviceName, bestEffort, logger);
        }
      }
    }

    public static string GetPathForService(string serviceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      string pathForService = (string) null;
      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\" + serviceName))
      {
        if (registryKey != null)
        {
          pathForService = Convert.ToString(registryKey.GetValue("ImagePath"), (IFormatProvider) CultureInfo.InvariantCulture);
          int length1 = pathForService.Length;
          string str = ".exe";
          int num = pathForService.IndexOf(str, 0, StringComparison.OrdinalIgnoreCase);
          if (num >= 0)
          {
            int length2 = num + str.Length;
            if (length2 < pathForService.Length && pathForService[num + str.Length] == '"')
              ++length2;
            pathForService = pathForService.Substring(0, length2);
          }
          pathForService = pathForService.Replace("\"", string.Empty);
        }
      }
      return pathForService;
    }

    public static bool IsServiceRunning(string machine, string serviceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(machine, nameof (machine));
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      bool flag = false;
      foreach (ServiceController service in ServiceController.GetServices(machine))
      {
        if (string.Equals(service.ServiceName, serviceName, StringComparison.OrdinalIgnoreCase))
        {
          if (service.Status == ServiceControllerStatus.Running)
          {
            flag = true;
            break;
          }
          break;
        }
      }
      return flag;
    }

    public static bool IsServiceInstalled(string machine, string serviceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(machine, nameof (machine));
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      foreach (ServiceController service in ServiceController.GetServices(machine))
      {
        if (string.Equals(service.ServiceName, serviceName, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private static void KillProcess(Process processToKill, string hostname, ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (processToKill));
      ArgumentUtility.CheckStringForNullOrEmpty(hostname, nameof (hostname));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      logger.Info("KillProcess {0} on {1}", (object) processToKill.ProcessName, (object) hostname);
      try
      {
        processToKill.Kill();
        logger.Info("Successfully Killed process {0} on {1}.", (object) processToKill.ProcessName, (object) hostname);
      }
      catch (Win32Exception ex)
      {
        logger.Warning("Process {0} could not be terminated because it's either terminating or it's a Win16 executable.", (object) processToKill.ProcessName);
      }
      catch (NotSupportedException ex)
      {
        logger.Warning("Process {0} could not be terminated because it's running on a remote computer.", (object) processToKill.ProcessName);
      }
      catch (InvalidOperationException ex)
      {
        logger.Warning("Process {0} could not be terminated because it has already existed or there's no currently running process.", (object) processToKill.ProcessName);
      }
    }

    private static void StartService(
      ServiceController serviceController,
      string[] args,
      ITFLogger logger)
    {
      new RetryManager(3, (Action<Exception>) (ex => logger.Warning(ex))).Invoke((Action) (() => serviceController.Start(args)));
    }

    private static void StopService(
      ServiceController serviceController,
      ITFLogger logger,
      TimeSpan retryDelay)
    {
      new RetryManager(3, retryDelay, (Action<Exception>) (ex => logger.Warning(ex))).Invoke((Action) (() =>
      {
        serviceController.Refresh();
        if (serviceController.Status != ServiceControllerStatus.Running)
          return;
        serviceController.Stop();
      }));
    }

    private static void UninstallServiceUsingSC(
      string serviceName,
      bool bestEffort,
      ITFLogger logger)
    {
      logger.Info(ConfigurationResources.UninstallingService((object) serviceName));
      CommandLineBuilder args1 = new CommandLineBuilder();
      args1.Append("delete");
      args1.Append(serviceName);
      ProcessOutput processOutput = ProcessHandler.RunExe(WindowsServiceManagement.PathToSC, args1, logger);
      if (bestEffort || string.IsNullOrEmpty(WindowsServiceManagement.GetPathForService(serviceName)))
        return;
      if (processOutput.ExitCode != 1072)
        throw new ConfigurationException(ConfigurationResources.ServiceUninstallFailed((object) serviceName));
      logger.Info("Failed to uninstall the service. Collecting diagnostics.");
      CommandLineBuilder args2 = new CommandLineBuilder();
      args2.Append("queryex");
      args2.Append(serviceName);
      ProcessHandler.RunExe(WindowsServiceManagement.PathToSC, args2, logger);
    }

    private static void WaitForStatus(
      ServiceController serviceController,
      ServiceControllerStatus status,
      ITFLogger logger)
    {
      if (status != ServiceControllerStatus.Running && status != ServiceControllerStatus.Stopped)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Wrong parameter value: (0)", (object) status.ToString()), nameof (status));
      int num = 0;
label_3:
      logger.Info("Attempt={0}", (object) num);
      ++num;
      try
      {
        serviceController.WaitForStatus(status, TimeSpan.FromMinutes(status == ServiceControllerStatus.Stopped ? 3.0 : 1.0));
      }
      catch (System.ServiceProcess.TimeoutException ex)
      {
        logger.Warning((Exception) ex);
        if (num >= 3)
          throw;
        else
          goto label_3;
      }
    }
  }
}
