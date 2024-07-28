// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssSiteWarmUpService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssSiteWarmUpService : IVssSiteWarmUpService, IVssFrameworkService
  {
    private int m_state;
    private static readonly string s_area = "WarmUp";
    private static readonly string s_layer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool CheckAndStartWarmup(IVssRequestContext requestContext)
    {
      try
      {
        requestContext.TraceEnter(10025000, VssSiteWarmUpService.s_area, VssSiteWarmUpService.s_layer, nameof (CheckAndStartWarmup));
        VssSiteWarmUpService.WarmupState warmupState = (VssSiteWarmUpService.WarmupState) Interlocked.CompareExchange(ref this.m_state, 1, 0);
        switch (warmupState)
        {
          case VssSiteWarmUpService.WarmupState.NotStarted:
            try
            {
              requestContext.GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTaskCallback(this.Warmup));
            }
            catch (Exception ex)
            {
              this.m_state = 0;
              requestContext.TraceException(1002505, VssSiteWarmUpService.s_area, VssSiteWarmUpService.s_layer, ex);
            }
            return false;
          case VssSiteWarmUpService.WarmupState.Started:
            return false;
          case VssSiteWarmUpService.WarmupState.Completed:
            return true;
          default:
            requestContext.Trace(10025004, TraceLevel.Warning, VssSiteWarmUpService.s_area, VssSiteWarmUpService.s_layer, string.Format("Warmup in unhandled state: {0}", (object) warmupState));
            return false;
        }
      }
      finally
      {
        requestContext.TraceLeave(10025003, VssSiteWarmUpService.s_area, VssSiteWarmUpService.s_layer, nameof (CheckAndStartWarmup));
      }
    }

    private void Warmup(IVssRequestContext systemRequestContext, object _)
    {
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      systemRequestContext.TraceAlways(311902501, TraceLevel.Info, VssSiteWarmUpService.s_area, VssSiteWarmUpService.s_layer, "Running Warmup Extensions");
      try
      {
        using (IDisposableReadOnlyList<IVssWarmUpExtension> extensions = systemRequestContext.GetExtensions<IVssWarmUpExtension>(throwOnError: true))
        {
          foreach (IVssWarmUpExtension vssWarmUpExtension in (IEnumerable<IVssWarmUpExtension>) extensions)
          {
            try
            {
              Stopwatch stopwatch2 = Stopwatch.StartNew();
              vssWarmUpExtension.WarmUp(systemRequestContext);
              systemRequestContext.TraceAlways(10025001, TraceLevel.Info, VssSiteWarmUpService.s_area, VssSiteWarmUpService.s_layer, "Run Warmup Extension: {0}, Completed in {1}", (object) vssWarmUpExtension.GetType().FullName, (object) stopwatch2.Elapsed);
            }
            catch (Exception ex)
            {
              systemRequestContext.TraceAlways(10025002, TraceLevel.Warning, VssSiteWarmUpService.s_area, VssSiteWarmUpService.s_layer, "Warmup Extension: {0}, failed", (object) vssWarmUpExtension.GetType().FullName);
              systemRequestContext.TraceException(10025002, VssSiteWarmUpService.s_area, VssSiteWarmUpService.s_layer, ex);
            }
          }
          this.m_state = 2;
          systemRequestContext.TraceAlways(10025003, TraceLevel.Info, VssSiteWarmUpService.s_area, VssSiteWarmUpService.s_layer, string.Format("All warmup extensions completed in {0}", (object) stopwatch1.Elapsed));
        }
      }
      catch (Exception ex)
      {
        this.m_state = 0;
        systemRequestContext.TraceAlways(10025005, TraceLevel.Error, VssSiteWarmUpService.s_area, VssSiteWarmUpService.s_layer, "GetExtensions failed in the WarmUp. We will retry WarmUp");
        systemRequestContext.TraceException(10025006, VssSiteWarmUpService.s_area, VssSiteWarmUpService.s_layer, ex);
      }
    }

    private enum WarmupState
    {
      NotStarted,
      Started,
      Completed,
    }
  }
}
