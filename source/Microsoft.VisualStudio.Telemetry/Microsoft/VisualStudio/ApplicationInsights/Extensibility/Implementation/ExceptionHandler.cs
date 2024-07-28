// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.ExceptionHandler
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal static class ExceptionHandler
  {
    public static void Start(Func<Task> asyncMethod)
    {
      try
      {
        asyncMethod().ContinueWith((Action<Task>) (task => CoreEventSource.Log.LogError(task.Exception.ToString())), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
      }
      catch (Exception ex)
      {
        CoreEventSource.Log.LogError(ex.ToString());
      }
    }
  }
}
