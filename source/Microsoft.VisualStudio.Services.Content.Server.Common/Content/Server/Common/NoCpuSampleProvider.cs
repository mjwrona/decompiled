// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.NoCpuSampleProvider
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class NoCpuSampleProvider : ICpuPerformanceCounterProvider
  {
    public float GetCpuDelta() => 0.0f;

    public float GetCurrentCpuLoadForecast() => 0.0f;

    public int GetSampleIntervalInMs() => 1;
  }
}
