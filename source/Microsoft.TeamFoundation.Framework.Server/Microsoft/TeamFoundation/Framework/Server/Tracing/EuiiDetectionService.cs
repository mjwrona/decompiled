// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Tracing.EuiiDetectionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Performance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server.Tracing
{
  public class EuiiDetectionService : IVssFrameworkService
  {
    protected bool m_isEnabled;
    protected bool m_isRedactionEnabled;
    protected bool m_isCPUCounterEnabled;
    protected int[] m_tracepointsEnabled = Array.Empty<int>();
    protected int m_traceIntervalMs = 60000;
    protected long m_euiiEmailDetectionCount;
    protected long m_tracesCheckedCount;
    private long m_cpuCycles;
    private DateTime m_lastTraceTimestamp = DateTime.UtcNow;
    private const int c_defaultTraceIntervalMs = 60000;
    private static readonly RegistryQuery s_registryQueryRoot = new RegistryQuery(FrameworkServerConstants.EuiiDetectionServiceRegistryRootPath + "/*");
    private static readonly RegistryQuery s_registryQueryEnabledTracepoints = new RegistryQuery(FrameworkServerConstants.EuiiDetectionServiceEnabledTracepoints);
    private static readonly RegistryQuery s_registryQueryTraceInterval = new RegistryQuery(FrameworkServerConstants.EuiiDetectionServiceTraceIntervalMs);
    private static readonly RegistryQuery s_registryQueryCPUCounterEnabled = new RegistryQuery(FrameworkServerConstants.EuiiDetectionServiceCPUCounterEnabled);
    private static readonly RegistryQuery s_registryQueryRedactionEnabled = new RegistryQuery(FrameworkServerConstants.EuiiDetectionServiceRedactionEnabled);
    private const string c_wikiReference = "For details: https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/38901/EUII-Detection-and-Redaction";
    public const string RedactedEmailConstant = "**REDACTED CONTAINS EMAIL PII** For details: https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/38901/EUII-Detection-and-Redaction";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.OnRegistrySettingsChanged(systemRequestContext);
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), true, in EuiiDetectionService.s_registryQueryRoot);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    internal void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries = null)
    {
      this.m_isEnabled = requestContext.GetService<TeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, FrameworkServerConstants.EuiiDetectionServiceFeatureFlag);
      if (!this.m_isEnabled)
        return;
      foreach (RegistryItem registryItem in requestContext.GetService<IVssRegistryService>().Read(requestContext, in EuiiDetectionService.s_registryQueryRoot))
      {
        if (registryItem.Path == EuiiDetectionService.s_registryQueryTraceInterval.Path)
          this.m_traceIntervalMs = RegistryUtility.FromString<int>(registryItem.Value, 60000);
        else if (registryItem.Path == EuiiDetectionService.s_registryQueryCPUCounterEnabled.Path)
          this.m_isCPUCounterEnabled = RegistryUtility.FromString<bool>(registryItem.Value, false);
        else if (registryItem.Path == EuiiDetectionService.s_registryQueryRedactionEnabled.Path)
          this.m_isRedactionEnabled = RegistryUtility.FromString<bool>(registryItem.Value, false);
        else if (registryItem.Path == EuiiDetectionService.s_registryQueryEnabledTracepoints.Path)
        {
          string str = RegistryUtility.FromString<string>(registryItem.Value, "");
          try
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            this.m_tracepointsEnabled = ((IEnumerable<string>) str.Split(new char[1]
            {
              ','
            }, StringSplitOptions.RemoveEmptyEntries)).Select<string, int>(EuiiDetectionService.\u003C\u003EO.\u003C0\u003E__Parse ?? (EuiiDetectionService.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, int>(int.Parse))).ToArray<int>();
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(323301, TraceLevel.Error, nameof (EuiiDetectionService), nameof (OnRegistrySettingsChanged), ex, "The registry value for {0} is corrupted: {1}", (object) FrameworkServerConstants.EuiiDetectionServiceEnabledTracepoints, (object) str);
            this.m_tracepointsEnabled = Array.Empty<int>();
          }
        }
      }
    }

    public EuiiDetectionResult DetectAndRedactEuii(int tracepoint, ref string message)
    {
      if (!this.m_isEnabled)
        return new EuiiDetectionResult(false, 0L, 0L, 0L, false);
      if (((IEnumerable<int>) this.m_tracepointsEnabled).Contains<int>(tracepoint))
      {
        Interlocked.Increment(ref this.m_tracesCheckedCount);
        long cpuTime = this.m_isCPUCounterEnabled ? PerformanceNativeMethods.GetCPUTime() : 0L;
        if (EuiiUtility.ContainsEmail(message, false))
        {
          Interlocked.Increment(ref this.m_euiiEmailDetectionCount);
          if (this.m_isRedactionEnabled)
            message = "**REDACTED CONTAINS EMAIL PII** For details: https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/38901/EUII-Detection-and-Redaction";
        }
        if (this.m_isCPUCounterEnabled)
          Interlocked.Add(ref this.m_cpuCycles, PerformanceNativeMethods.GetCPUTime() - cpuTime);
      }
      DateTime utcNow = DateTime.UtcNow;
      bool shouldTraceCounterInfo = this.m_lastTraceTimestamp.AddMilliseconds((double) this.m_traceIntervalMs) <= utcNow;
      if (!shouldTraceCounterInfo)
        return new EuiiDetectionResult(shouldTraceCounterInfo, this.m_euiiEmailDetectionCount, this.m_tracesCheckedCount, this.m_cpuCycles, this.m_isRedactionEnabled);
      this.m_lastTraceTimestamp = utcNow;
      return new EuiiDetectionResult(shouldTraceCounterInfo, Interlocked.Exchange(ref this.m_euiiEmailDetectionCount, 0L), Interlocked.Exchange(ref this.m_tracesCheckedCount, 0L), Interlocked.Exchange(ref this.m_cpuCycles, 0L), this.m_isRedactionEnabled);
    }
  }
}
