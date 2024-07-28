// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.SshUtil
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Ssh.Server
{
  internal static class SshUtil
  {
    private static readonly TimeSpan s_logQuietPeriod = new TimeSpan(0, 5, 0);
    private const string s_area = "SshServer";
    private const string s_layer = "SshUtil";

    public static void RetryOperationsUntilSuccessful(
      RetryOperations operations,
      ref int delayOnExceptionSeconds)
    {
      SshUtil.RetryOperationsUntilSuccessful(operations, -1, ref delayOnExceptionSeconds);
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
          TeamFoundationTracingService.TraceRaw(13000500, TraceLevel.Error, "SshServer", nameof (SshUtil), "Attempt #{0}: {1}", (object) num, (object) ex);
          if (maxTries == -1 || num < maxTries)
          {
            if (!ex.GetType().Equals(o) || dateTime + SshUtil.s_logQuietPeriod < DateTime.Now)
            {
              TeamFoundationEventLog.Default.LogException(FrameworkResources.ErrorAttemptingRetryableOperation(), ex);
              o = ex.GetType();
              dateTime = DateTime.Now;
            }
            Thread.Sleep(1000 * delayOnExceptionSeconds);
          }
          else
          {
            TeamFoundationTracingService.TraceRaw(13000501, TraceLevel.Error, "SshServer", nameof (SshUtil), "Rethrowing after attempt #{0}: {1}", (object) num, (object) ex);
            Thread.Sleep(1000 * delayOnExceptionSeconds);
            throw;
          }
        }
        ++num;
      }
    }
  }
}
