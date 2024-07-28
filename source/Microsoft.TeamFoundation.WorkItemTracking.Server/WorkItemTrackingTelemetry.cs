// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal abstract class WorkItemTrackingTelemetry
  {
    private ClientTraceData m_ciData;
    private string m_feature;
    private IVssRequestContext m_requestContext;
    private string m_timeRegistryPath;
    private int m_defaultTimeInMilliSec;

    public WorkItemTrackingTelemetry(
      IVssRequestContext requestContext,
      string feature,
      string timeRegistryPath,
      int defaultTimeInMilliSec)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(feature, nameof (feature));
      this.m_requestContext = requestContext;
      this.m_ciData = new ClientTraceData();
      this.m_feature = feature;
      this.m_timeRegistryPath = timeRegistryPath;
      this.m_defaultTimeInMilliSec = defaultTimeInMilliSec;
    }

    public ClientTraceData ClientTraceData => this.m_ciData;

    public virtual void AddData(params object[] param)
    {
    }

    public virtual void TraceData(params object[] param)
    {
      long elapsedClockTimeMs = this.GetElapsedClockTimeMs();
      if (!this.PublishData(elapsedClockTimeMs))
        return;
      this.AddBasicData(elapsedClockTimeMs);
      this.AddData(param);
      this.m_requestContext.GetService<ClientTraceService>().Publish(this.m_requestContext, "WorkItemService", this.m_feature, this.ClientTraceData);
    }

    private long GetElapsedClockTimeMs() => (long) (DateTime.UtcNow - this.m_requestContext.RequestTimer.StartTime).TotalMilliseconds;

    private bool PublishData(long elapsedClockTimeMs) => this.m_requestContext.LastTracedBlockElapsedMilliseconds() != 0.0 && elapsedClockTimeMs > (long) this.GetThresholdTime(this.m_timeRegistryPath, this.m_defaultTimeInMilliSec);

    private void AddBasicData(long elapsedClockTimeMs)
    {
      this.ClientTraceData.Add("ActivityId", (object) this.m_requestContext.ActivityId);
      this.ClientTraceData.Add("ElapsedTime", (object) this.m_requestContext.LastTracedBlockElapsedMilliseconds());
      this.ClientTraceData.Add("ClockTime", (object) elapsedClockTimeMs);
    }

    private int GetThresholdTime(string registryPath, int defaultTime) => !string.IsNullOrEmpty(registryPath) ? this.m_requestContext.GetService<IVssRegistryService>().GetValue<int>(this.m_requestContext, (RegistryQuery) registryPath, true, defaultTime) : defaultTime;

    public static void TraceCustomerIntelligence(
      IVssRequestContext requestContext,
      string feature,
      params object[] param)
    {
      WorkItemTrackingTelemetryFactory.GetTelemetryObject(requestContext, feature)?.TraceData(param);
    }
  }
}
