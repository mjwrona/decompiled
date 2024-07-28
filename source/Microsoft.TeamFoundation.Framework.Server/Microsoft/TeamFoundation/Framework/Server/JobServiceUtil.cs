// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobServiceUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class JobServiceUtil
  {
    private static readonly TimeSpan s_logQuietPeriod = new TimeSpan(0, 5, 0);
    private const string s_area = "JobAgent";
    private const string s_layer = "JobServiceUtil";

    public static void RetryOperationsUntilSuccessful(
      RetryOperations operations,
      ref int delayOnExceptionSeconds)
    {
      JobServiceUtil.RetryOperationsUntilSuccessful(operations, -1, ref delayOnExceptionSeconds);
    }

    public static bool IsServiceHostIdle(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.ServiceHostInternal().SubStatus == ServiceHostSubStatus.Idle)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        if (vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(vssRequestContext, requestContext.ServiceHost.InstanceId).SubStatus == ServiceHostSubStatus.Idle)
          return true;
      }
      return false;
    }

    public static void RetryOperationsUntilSuccessful(
      RetryOperations operations,
      int maxTries,
      ref int delayOnExceptionSeconds)
    {
      Type o = (Type) null;
      DateTime dateTime = DateTime.MinValue;
      int num = 0;
      while (true)
      {
        try
        {
          operations();
          break;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceRaw(1600, TraceLevel.Error, "JobAgent", nameof (JobServiceUtil), "Attempt #{0}: {1}", (object) num, (object) ex);
          if (maxTries == -1 || num < maxTries)
          {
            if (!ex.GetType().Equals(o) || dateTime + JobServiceUtil.s_logQuietPeriod < DateTime.Now)
            {
              TeamFoundationEventLog.Default.LogException(FrameworkResources.ErrorAttemptingRetryableOperation(), ex);
              o = ex.GetType();
              dateTime = DateTime.Now;
            }
            Thread.Sleep(1000 * delayOnExceptionSeconds);
          }
          else
          {
            TeamFoundationTracingService.TraceRaw(1601, TraceLevel.Error, "JobAgent", nameof (JobServiceUtil), "Rethrowing after attempt #{0}: {1}", (object) num, (object) ex);
            Thread.Sleep(1000 * delayOnExceptionSeconds);
            throw;
          }
        }
        ++num;
      }
    }
  }
}
