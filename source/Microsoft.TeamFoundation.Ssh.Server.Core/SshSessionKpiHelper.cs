// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Core.SshSessionKpiHelper
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Ssh.Server.Core
{
  public static class SshSessionKpiHelper
  {
    public static string KpiArea => "Ssh";

    public static void Increment(
      IVssRequestContext requestContext,
      SshSessionKpiHelper.SshSessionKpi kpi,
      bool throwOnFailure = false)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.TraceEnter(13000400, SshSessionKpiHelper.s_Area, SshSessionKpiHelper.s_Layer, nameof (Increment));
      try
      {
        vssRequestContext.GetService<IKpiService>().Publish(vssRequestContext, SshSessionKpiHelper.KpiArea, kpi.ToString(), 1.0);
      }
      catch (Exception ex)
      {
        vssRequestContext.TraceException(13000408, SshSessionKpiHelper.s_Area, SshSessionKpiHelper.s_Layer, ex);
        if (!throwOnFailure)
          return;
        throw;
      }
      finally
      {
        vssRequestContext.TraceLeave(13000409, SshSessionKpiHelper.s_Area, SshSessionKpiHelper.s_Layer, nameof (Increment));
      }
    }

    private static string s_Layer => "Ssh";

    private static string s_Area => nameof (SshSessionKpiHelper);

    public enum SshSessionKpi
    {
      AuthAttempt,
      AuthFailure,
      CommandExecution,
      CommandFailure,
    }
  }
}
