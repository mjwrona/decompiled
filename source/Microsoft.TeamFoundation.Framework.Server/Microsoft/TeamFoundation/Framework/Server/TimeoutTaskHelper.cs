// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TimeoutTaskHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class TimeoutTaskHelper
  {
    private static TimeSpan Min(TimeSpan one, TimeSpan two) => !(one > two) ? one : two;

    private static void Trace(TraceLevel level, string message, params object[] args) => TeamFoundationTracingService.TraceRaw(57406, level, "HostManagement", nameof (TimeoutTaskHelper), message, args);

    public static async Task DoWork(
      Func<CancellationToken, Task<DateTime>> work,
      DateTime expiry,
      TimeSpan interval,
      Func<Exception, bool> isPermanent,
      CancellationToken cancellationToken)
    {
      try
      {
        while (true)
        {
          TimeSpan delay = TimeoutTaskHelper.Min(interval, expiry.Subtract(DateTime.UtcNow));
          if (!(delay < TimeSpan.Zero))
          {
            using (CancellationTokenSource timeoutSource = new CancellationTokenSource(delay))
            {
              using (CancellationTokenSource combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken))
              {
                try
                {
                  expiry = await work(combinedTokenSource.Token).ConfigureAwait(false);
                  TimeoutTaskHelper.Trace(TraceLevel.Info, "work successful, expiry is now: {0}", (object) expiry);
                }
                catch (System.OperationCanceledException ex)
                {
                  if (cancellationToken.IsCancellationRequested)
                    throw;
                  else
                    TimeoutTaskHelper.Trace(TraceLevel.Warning, "Worker timed out");
                }
                catch (Exception ex)
                {
                  TimeoutTaskHelper.Trace(TraceLevel.Error, "Caught exception while calling delegate: {0}", (object) ex);
                  if (isPermanent(ex))
                  {
                    TimeoutTaskHelper.Trace(TraceLevel.Error, "Exception {0} is permanent, bailing out", (object) ex.GetType());
                    throw;
                  }
                }
              }
            }
            if (!(expiry.Subtract(DateTime.UtcNow) < interval))
              await Task.Delay(interval, cancellationToken).ConfigureAwait(false);
            else
              goto label_22;
          }
          else
            break;
        }
        TimeoutTaskHelper.Trace(TraceLevel.Error, "Effective time out is negative, throwing timeoutexception");
        throw new TimeoutException();
label_22:
        TimeoutTaskHelper.Trace(TraceLevel.Error, "Timeout expired - throwing TimeoutException");
        throw new TimeoutException();
      }
      catch (System.OperationCanceledException ex)
      {
        TimeoutTaskHelper.Trace(TraceLevel.Info, "Caller requested cancellation - bailing out");
      }
    }
  }
}
