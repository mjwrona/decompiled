// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CustomerIntelligenceEntryPoint
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [GenerateAllConstants(null)]
  public enum CustomerIntelligenceEntryPoint : byte
  {
    Unknown = 0,
    TfsOnline = 1,
    Azure = 2,
    VsIDE = 3,
    Ibiza = 4,
    VsoPublish = 5,
    DataImport = 6,
    MobileCenter = 7,
    VSCode = 8,
    IntelliJ = 9,
    IbizaCD = 11, // 0x0B
    IbizaDevOps = 12, // 0x0C
    WebSuite = 13, // 0x0D
    WebRepos = 14, // 0x0E
    WebBoards = 15, // 0x0F
    WebPipelines = 16, // 0x10
    GitHubMarketplace = 17, // 0x11
    GitHubPipelines = 18, // 0x12
    GitHubMarketplaceBoards = 19, // 0x13
    GitHubMarketplacePipelines = 20, // 0x14
    GitHubMarketplaceConnect = 21, // 0x15
    WebArtifacts = 22, // 0x16
    GitHubServices = 23, // 0x17
    GitHubActions = 24, // 0x18
  }
}
