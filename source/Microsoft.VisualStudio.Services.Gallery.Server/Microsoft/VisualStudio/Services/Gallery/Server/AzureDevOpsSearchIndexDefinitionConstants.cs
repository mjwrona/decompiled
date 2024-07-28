// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AzureDevOpsSearchIndexDefinitionConstants
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class AzureDevOpsSearchIndexDefinitionConstants : AzureSearchIndexDefinitionConstants
  {
    public override int DownloadCountScoringFunctionBoost { get; } = 10000;

    public override int DownloadCountScoringFunctionBoostingRangeStart { get; } = 1000;

    public override int DownloadCountScoringFunctionBoostingRangeEnd { get; } = 5000000;

    public override int InstallCountScoringFunctionBoost { get; } = 10000;

    public override int InstallCountScoringFunctionBoostingRangeStart { get; } = 1000;

    public override int InstallCountScoringFunctionBoostingRangeEnd { get; } = 5000000;
  }
}
