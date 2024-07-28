// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.ISearchBackupPlatform
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Nest;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public interface ISearchBackupPlatform
  {
    CreateRepositoryResponse RegisterBackupRepository(
      ExecutionContext executionContext,
      string repoName);

    GetRepositoryResponse GetBackupRepository(ExecutionContext executionContext, string repoName);

    GetRepositoryResponse GetAllBackupRepositories(ExecutionContext executionContext);

    DeleteRepositoryResponse DeleteBackupRepository(
      ExecutionContext executionContext,
      string repoName);

    SnapshotResponse CreateSnapshot(
      ExecutionContext executionContext,
      string repoName,
      string snapshotName);

    GetSnapshotResponse GetAllSnapshots(ExecutionContext executionContext, string repoName);

    Snapshot GetLatestSnapshot(ExecutionContext executionContext, string repoName);

    SnapshotStatusResponse GetSnapshotStatus(
      ExecutionContext executionContext,
      string repoName,
      string snapshotName);

    DeleteSnapshotResponse DeleteSnapshot(
      ExecutionContext executionContext,
      string repoName,
      string snapshotName);
  }
}
