// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload.RealStopwatch
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload
{
  public class RealStopwatch : IStopwatch
  {
    private readonly Stopwatch stopWatch;

    public RealStopwatch() => this.stopWatch = new Stopwatch();

    public TimeSpan Elapsed => this.stopWatch.Elapsed;

    public void Start() => this.stopWatch.Start();

    public void Stop() => this.stopWatch.Stop();

    public void Reset() => this.stopWatch.Reset();

    public void Restart() => this.stopWatch.Restart();
  }
}
