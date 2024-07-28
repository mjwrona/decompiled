// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.JobRegisteredStatus
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class JobRegisteredStatus
  {
    private Guid m_jobId;
    private bool m_isRegistered;
    private Stopwatch m_lastCheckStopwatch = new Stopwatch();

    internal JobRegisteredStatus(Guid jobId) => this.m_jobId = jobId;

    public bool IsJobRegistered(IVssRequestContext requestContext, int registeredCheckFrequency)
    {
      if (!this.m_isRegistered && (!this.m_lastCheckStopwatch.IsRunning || this.m_lastCheckStopwatch.ElapsedMilliseconds >= (long) registeredCheckFrequency))
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        string str = string.Format(FrameworkServerConstants.NotificationRootPath + "/NotificationJobs/{0}/IsRegistered", (object) this.m_jobId.ToString());
        try
        {
          this.m_isRegistered = service.GetValue<bool>(requestContext, (RegistryQuery) str, false);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1002116, "Notifications", "JobRegistered", ex);
        }
        if (!this.m_isRegistered)
        {
          this.m_isRegistered = requestContext.GetService<TeamFoundationJobService>().QueryJobDefinition(requestContext, this.m_jobId) != null;
          if (this.m_isRegistered)
            service.SetValue<bool>(requestContext, str, true);
          this.m_lastCheckStopwatch.Reset();
        }
        this.m_lastCheckStopwatch.Start();
      }
      return this.m_isRegistered;
    }
  }
}
