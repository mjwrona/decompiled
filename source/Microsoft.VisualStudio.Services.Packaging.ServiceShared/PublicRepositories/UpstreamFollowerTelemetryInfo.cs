// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.UpstreamFollowerTelemetryInfo
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public class UpstreamFollowerTelemetryInfo
  {
    public UpstreamFollowerExitReason ExitReason { get; set; }

    public int? InterestingPackageNamesCount { get; set; }

    public int? ChangesProvided { get; set; }

    public string? InitialCursorPosition { get; set; }

    public string? FinalCursorPosition { get; set; }

    public int ChangesAttempted { get; set; }

    public int PackagesInvalidated { get; set; }

    public int CursorPositionsProcessed { get; set; }

    public int JobsQueued { get; set; }

    public IReadOnlyCollection<string>? LastUpdatedPackageNames { get; set; }
  }
}
