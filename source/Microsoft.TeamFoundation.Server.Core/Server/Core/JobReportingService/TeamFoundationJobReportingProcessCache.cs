// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.JobReportingService.TeamFoundationJobReportingProcessCache
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core.JobReportingService
{
  internal class TeamFoundationJobReportingProcessCache
  {
    private Dictionary<Guid, TeamFoundationServiceHostProcess> m_processes = new Dictionary<Guid, TeamFoundationServiceHostProcess>();

    internal TeamFoundationServiceHostProcess GetHostProcess(
      IVssRequestContext requestContext,
      Guid processId)
    {
      if (!this.m_processes.ContainsKey(processId))
      {
        foreach (TeamFoundationServiceHostProcess serviceHostProcess in requestContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProcesses(requestContext, Guid.Empty))
        {
          if (!this.m_processes.ContainsKey(serviceHostProcess.ProcessId))
          {
            lock (this.m_processes)
              this.m_processes.Add(serviceHostProcess.ProcessId, serviceHostProcess);
          }
        }
      }
      return this.m_processes[processId];
    }
  }
}
