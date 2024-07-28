// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AlertPublishingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class AlertPublishingService : IVssFrameworkService
  {
    private IDisposableReadOnlyList<IAlertPublisher> m_alertPlugins;
    private static readonly string s_area = "AlertPublishing";
    private static readonly string s_layer = "AlertPublishingUtility";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      if (!systemRequestContext.ServiceHost.HasDatabaseAccess || string.IsNullOrEmpty(systemRequestContext.ServiceHost.PlugInDirectory))
        return;
      systemRequestContext.GetService<ITeamFoundationTaskService>().AddTask(systemRequestContext, (TeamFoundationTaskCallback) ((requestContext, taskArgs) =>
      {
        try
        {
          this.m_alertPlugins = VssExtensionManagementService.GetExtensionsRaw<IAlertPublisher>(requestContext.ServiceHost.PlugInDirectory);
          foreach (IAlertPublisher alertPlugin in (IEnumerable<IAlertPublisher>) this.m_alertPlugins)
          {
            try
            {
              alertPlugin.Initialize(requestContext);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(9408, AlertPublishingService.s_area, AlertPublishingService.s_layer, ex);
            }
          }
          TeamFoundationEventLog.Default.OnLog += new EventLogEntryLoggedHandler(this.EvaluateLogEntry);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(9403, AlertPublishingService.s_area, AlertPublishingService.s_layer, ex);
        }
      }));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      TeamFoundationEventLog.Default.OnLog -= new EventLogEntryLoggedHandler(this.EvaluateLogEntry);
      if (this.m_alertPlugins == null)
        return;
      this.m_alertPlugins.Dispose();
      this.m_alertPlugins = (IDisposableReadOnlyList<IAlertPublisher>) null;
    }

    internal void EvaluateLogEntry(
      IVssRequestContext requestContext,
      string eventSource,
      int eventId,
      EventLogEntryType eventType,
      string message,
      object[] eventValues)
    {
      try
      {
        message = SecretUtility.ScrubSecrets(message, false);
        TeamFoundationTracingService.TraceRaw(9400, TraceLevel.Verbose, AlertPublishingService.s_area, AlertPublishingService.s_layer, "TryEvaluateLogEntry called. EventSource: {0}, eventId: {1}, eventType: {2}, message: {3}, eventValues: {4}", (object) eventSource, (object) eventId, (object) eventType, (object) message, (object) eventValues);
        if (this.m_alertPlugins == null)
          return;
        foreach (IAlertPublisher alertPlugin in (IEnumerable<IAlertPublisher>) this.m_alertPlugins)
        {
          try
          {
            alertPlugin.EvaluateLogEntry(requestContext, eventSource, eventId, eventType, message, eventValues);
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(9401, AlertPublishingService.s_area, AlertPublishingService.s_layer, ex);
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(9402, AlertPublishingService.s_area, AlertPublishingService.s_layer, ex);
      }
    }
  }
}
