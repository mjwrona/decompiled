// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler.RetryHandler
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler
{
  public static class RetryHandler
  {
    public static void Do(Action action, TimeSpan retryInterval, int retryCount = 3) => RetryHandler.Do<object>((Func<object>) (() =>
    {
      action();
      return (object) null;
    }), retryInterval, retryCount);

    public static T Do<T>(Func<T> action, TimeSpan retryInterval, int retryCount = 3)
    {
      List<Exception> innerExceptions = new List<Exception>();
      for (int index = 0; index < retryCount; ++index)
      {
        try
        {
          return action();
        }
        catch (Exception ex)
        {
          innerExceptions.Add(ex);
          Thread.Sleep(retryInterval);
        }
      }
      throw new AggregateException((IEnumerable<Exception>) innerExceptions);
    }
  }
}
