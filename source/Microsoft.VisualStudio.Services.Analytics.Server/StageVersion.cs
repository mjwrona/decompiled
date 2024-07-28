// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StageVersion
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class StageVersion
  {
    internal const int NextServiceVersion = -1;

    public int MinServiceVersion { get; }

    public int? MaxServiceVersion { get; }

    public int Version { get; }

    public StageVersion(int minServiceVersion, int? maxServiceVersion, int stageVersion)
    {
      this.MinServiceVersion = minServiceVersion;
      this.MaxServiceVersion = maxServiceVersion;
      this.Version = stageVersion;
    }
  }
}
