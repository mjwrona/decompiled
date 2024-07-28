// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Types.ResultOptions
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

namespace Microsoft.VisualStudio.Services.Feed.Server.Types
{
  public class ResultOptions
  {
    public bool IncludeDescriptions { get; set; } = true;

    public bool IncludeAllVersions { get; set; }

    public override bool Equals(object obj) => obj is ResultOptions resultOptions && this.IncludeAllVersions == resultOptions.IncludeAllVersions && this.IncludeDescriptions == resultOptions.IncludeDescriptions;

    public override int GetHashCode() => (this.IncludeAllVersions ? int.MinValue : 0) | (this.IncludeDescriptions ? 32768 : 0);
  }
}
