// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.CpuPerformanceCounterProvider`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public static class CpuPerformanceCounterProvider<T> where T : ICpuPerformanceCounterProvider, new()
  {
    private static readonly Lazy<T> lazySamplingTask = new Lazy<T>(LazyThreadSafetyMode.ExecutionAndPublication);

    public static ICpuPerformanceCounterProvider Instance
    {
      get
      {
        try
        {
          return (ICpuPerformanceCounterProvider) CpuPerformanceCounterProvider<T>.lazySamplingTask.Value;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(5750101, "Artifact", nameof (T), ex);
        }
        return (ICpuPerformanceCounterProvider) new NoCpuSampleProvider();
      }
    }
  }
}
