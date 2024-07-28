// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.ChainedAsyncResult
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal class ChainedAsyncResult : AsyncResult
  {
    private ChainedBeginHandler begin2;
    private ChainedEndHandler end1;
    private ChainedEndHandler end2;
    private TimeoutHelper timeoutHelper;
    private static AsyncCallback begin1Callback = new AsyncCallback(ChainedAsyncResult.Begin1Callback);
    private static AsyncCallback begin2Callback = new AsyncCallback(ChainedAsyncResult.Begin2Callback);

    protected ChainedAsyncResult(TimeSpan timeout, AsyncCallback callback, object state)
      : base(callback, state)
    {
      this.timeoutHelper = new TimeoutHelper(timeout);
    }

    public ChainedAsyncResult(
      TimeSpan timeout,
      AsyncCallback callback,
      object state,
      ChainedBeginHandler begin1,
      ChainedEndHandler end1,
      ChainedBeginHandler begin2,
      ChainedEndHandler end2)
      : base(callback, state)
    {
      this.timeoutHelper = new TimeoutHelper(timeout);
      this.Begin(begin1, end1, begin2, end2);
    }

    protected void Begin(
      ChainedBeginHandler beginOne,
      ChainedEndHandler endOne,
      ChainedBeginHandler beginTwo,
      ChainedEndHandler endTwo)
    {
      this.end1 = endOne;
      this.begin2 = beginTwo;
      this.end2 = endTwo;
      IAsyncResult result = beginOne(this.timeoutHelper.RemainingTime(), ChainedAsyncResult.begin1Callback, (object) this);
      if (!result.CompletedSynchronously || !this.Begin1Completed(result))
        return;
      this.Complete(true);
    }

    private static void Begin1Callback(IAsyncResult result)
    {
      if (result.CompletedSynchronously)
        return;
      ChainedAsyncResult asyncState = (ChainedAsyncResult) result.AsyncState;
      Exception e = (Exception) null;
      bool flag;
      try
      {
        flag = asyncState.Begin1Completed(result);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          flag = true;
          e = ex;
        }
      }
      if (!flag)
        return;
      asyncState.Complete(false, e);
    }

    private bool Begin1Completed(IAsyncResult result)
    {
      this.end1(result);
      result = this.begin2(this.timeoutHelper.RemainingTime(), ChainedAsyncResult.begin2Callback, (object) this);
      if (!result.CompletedSynchronously)
        return false;
      this.end2(result);
      return true;
    }

    private static void Begin2Callback(IAsyncResult result)
    {
      if (result.CompletedSynchronously)
        return;
      ChainedAsyncResult asyncState = (ChainedAsyncResult) result.AsyncState;
      Exception e = (Exception) null;
      try
      {
        asyncState.end2(result);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          e = ex;
      }
      asyncState.Complete(false, e);
    }

    public static void End(IAsyncResult result) => AsyncResult.End<ChainedAsyncResult>(result);
  }
}
