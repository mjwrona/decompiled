// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.CompletedAsyncResult`1
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Common
{
  [Serializable]
  internal class CompletedAsyncResult<T> : AsyncResult
  {
    private T data;

    public CompletedAsyncResult(T data, AsyncCallback callback, object state)
      : base(callback, state)
    {
      this.data = data;
      this.Complete(true);
    }

    public static T End(IAsyncResult result)
    {
      Fx.AssertAndThrowFatal(result.IsCompleted, "CompletedAsyncResult<T> was not completed!");
      return AsyncResult.End<CompletedAsyncResult<T>>(result).data;
    }
  }
}
