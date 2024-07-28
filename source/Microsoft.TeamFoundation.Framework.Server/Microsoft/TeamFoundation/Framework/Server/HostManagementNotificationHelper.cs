// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostManagementNotificationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HostManagementNotificationHelper
  {
    public static void FlushSqlNotificationsForJobAgents(
      IVssRequestContext requestContext,
      int timeToWaitForNotificationsInSeconds = 900,
      Action<string> LogInfo = null)
    {
      TeamFoundationHostManagementService hostManagementService = requestContext.GetService<TeamFoundationHostManagementService>();
      HostManagementNotificationHelper.FlushSqlNotificationsForSelectProcesses(requestContext, (Func<List<TeamFoundationServiceHostProcess>>) (() => hostManagementService.QueryServiceHostProcesses(requestContext, Guid.Empty).Where<TeamFoundationServiceHostProcess>((Func<TeamFoundationServiceHostProcess, bool>) (p => ((IEnumerable<string>) FrameworkServerConstants.JobAgentProcessNames).Any<string>((Func<string, bool>) (ja => StringComparer.InvariantCultureIgnoreCase.Equals(ja, p.ProcessName))))).ToList<TeamFoundationServiceHostProcess>()), timeToWaitForNotificationsInSeconds, LogInfo);
    }

    public static void FlushSqlNotificationsForAllProcesses(
      IVssRequestContext requestContext,
      int timeToWaitForNotificationsInSeconds = 900,
      Action<string> LogInfo = null)
    {
      TeamFoundationHostManagementService hostManagementService = requestContext.GetService<TeamFoundationHostManagementService>();
      HostManagementNotificationHelper.FlushSqlNotificationsForSelectProcesses(requestContext, (Func<List<TeamFoundationServiceHostProcess>>) (() => hostManagementService.QueryServiceHostProcesses(requestContext, Guid.Empty).ToList<TeamFoundationServiceHostProcess>()), timeToWaitForNotificationsInSeconds, LogInfo);
    }

    private static void FlushSqlNotificationsForSelectProcesses(
      IVssRequestContext requestContext,
      Func<List<TeamFoundationServiceHostProcess>> getLiveProcesses,
      int timeToWaitForNotificationsInSeconds = 900,
      Action<string> LogInfo = null)
    {
      TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
      List<TeamFoundationServiceHostProcess> source = getLiveProcesses();
      if (source.Count > 0)
      {
        if (LogInfo != null)
          LogInfo("Found " + string.Join(", ", source.Select<TeamFoundationServiceHostProcess, string>((Func<TeamFoundationServiceHostProcess, string>) (j => j.ProcessId.ToString()))) + " processes to ping");
      }
      else if (LogInfo != null)
        LogInfo("Found no active processes");
      int num = timeToWaitForNotificationsInSeconds / 30 + 1;
      while (source.Count > 0 && num-- > 0)
      {
        List<Guid> happyProcesses = new List<Guid>();
        List<Guid> zombieProcesses = new List<Guid>();
        foreach (TeamFoundationServiceHostProcess serviceHostProcess in source)
        {
          if (service.PingHostProcess(requestContext, serviceHostProcess.ProcessId, TimeSpan.FromSeconds((double) Math.Min(30, timeToWaitForNotificationsInSeconds))))
          {
            if (LogInfo != null)
              LogInfo(string.Format("Process {0} on {1} acknowledged", (object) serviceHostProcess.ProcessId, (object) serviceHostProcess.MachineName));
            happyProcesses.Add(serviceHostProcess.ProcessId);
          }
        }
        if (happyProcesses.Count > 0)
          source.RemoveAll((Predicate<TeamFoundationServiceHostProcess>) (p => happyProcesses.Contains(p.ProcessId)));
        if (source.Count > 0)
        {
          List<TeamFoundationServiceHostProcess> liveProcesses = getLiveProcesses();
          zombieProcesses.AddRange(source.Where<TeamFoundationServiceHostProcess>((Func<TeamFoundationServiceHostProcess, bool>) (p => !liveProcesses.Any<TeamFoundationServiceHostProcess>((Func<TeamFoundationServiceHostProcess, bool>) (lp => lp.ProcessId == p.ProcessId)))).Select<TeamFoundationServiceHostProcess, Guid>((Func<TeamFoundationServiceHostProcess, Guid>) (p => p.ProcessId)));
          if (zombieProcesses.Count > 0)
          {
            foreach (Guid guid in zombieProcesses)
            {
              if (LogInfo != null)
                LogInfo(string.Format("Process {0} has disappeared", (object) guid));
            }
            source.RemoveAll((Predicate<TeamFoundationServiceHostProcess>) (p => zombieProcesses.Contains(p.ProcessId)));
          }
        }
      }
      if (source.Count > 0)
      {
        string str = string.Join(", ", source.Select<TeamFoundationServiceHostProcess, string>((Func<TeamFoundationServiceHostProcess, string>) (j => j.ProcessId.ToString())));
        if (LogInfo != null)
          LogInfo("Processes " + str + " did not acknowledge registry change");
        throw new HostProcessNotFoundException("Failed to update the following host process " + str);
      }
    }
  }
}
