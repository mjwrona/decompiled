// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.ISearchPlatformFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public interface ISearchPlatformFactory
  {
    ISearchPlatform Create(string connectionString, string platformSettings, bool isOnPrem);

    ISearchClusterManagementService CreateSearchClusterManagementService(
      string connectionString,
      string platformSettings,
      bool isOnPrem);

    ISearchBackupPlatform CreateBackupPlatform(
      string connectionString,
      string platformSettings,
      bool isOnPrem);
  }
}
