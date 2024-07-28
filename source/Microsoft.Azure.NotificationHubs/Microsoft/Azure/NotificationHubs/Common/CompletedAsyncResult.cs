// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.CompletedAsyncResult
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Common
{
  [Serializable]
  internal class CompletedAsyncResult : AsyncResult
  {
    public CompletedAsyncResult(AsyncCallback callback, object state)
      : base(callback, state)
    {
      this.Complete(true);
    }

    public CompletedAsyncResult(Exception exception, AsyncCallback callback, object state)
      : base(callback, state)
    {
      this.Complete(true, exception);
    }

    public static void End(IAsyncResult result)
    {
      Fx.AssertAndThrowFatal(result.IsCompleted, "CompletedAsyncResult was not completed!");
      AsyncResult.End<CompletedAsyncResult>(result);
    }
  }
}
