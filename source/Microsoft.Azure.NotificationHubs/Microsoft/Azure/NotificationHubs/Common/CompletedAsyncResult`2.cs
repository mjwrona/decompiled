// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.CompletedAsyncResult`2
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Common
{
  [Serializable]
  internal class CompletedAsyncResult<TResult, TParameter> : AsyncResult
  {
    private TResult resultData;
    private TParameter parameter;

    public CompletedAsyncResult(
      TResult resultData,
      TParameter parameter,
      AsyncCallback callback,
      object state)
      : base(callback, state)
    {
      this.resultData = resultData;
      this.parameter = parameter;
      this.Complete(true);
    }

    public static TResult End(IAsyncResult result, out TParameter parameter)
    {
      Fx.AssertAndThrowFatal(result.IsCompleted, "CompletedAsyncResult<T> was not completed!");
      CompletedAsyncResult<TResult, TParameter> completedAsyncResult = AsyncResult.End<CompletedAsyncResult<TResult, TParameter>>(result);
      parameter = completedAsyncResult.parameter;
      return completedAsyncResult.resultData;
    }
  }
}
