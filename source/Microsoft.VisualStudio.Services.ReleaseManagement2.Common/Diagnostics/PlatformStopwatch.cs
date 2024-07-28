// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics.PlatformStopwatch
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics
{
  public class PlatformStopwatch : IStopwatch
  {
    private readonly Stopwatch stopwatch;

    internal PlatformStopwatch() => this.stopwatch = new Stopwatch();

    public long ElapsedMilliseconds => this.stopwatch.ElapsedMilliseconds;

    public void Start() => this.stopwatch.Start();

    public void Stop() => this.stopwatch.Stop();
  }
}
