// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationFileCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationFileCacheService))]
  public interface ITeamFoundationFileCacheService : IVssFrameworkService
  {
    ProxyConfiguration Configuration { get; }

    bool IsGitCacheEnabled { get; }

    bool IsVCCacheEnabled { get; }

    bool IsCacheCleanupInProgress { get; }

    void RetrieveFile<TFileInformation>(
      IVssRequestContext requestContext,
      TFileInformation fileInfo,
      IDownloadState<TFileInformation> downloadState,
      bool compressOutput)
      where TFileInformation : FileInformationBase;

    bool RetrieveFileFromCache<TFileInformation>(
      IVssRequestContext requestContext,
      TFileInformation fileInfo,
      IDownloadState<TFileInformation> downloadState,
      bool compressOutput)
      where TFileInformation : FileInformationBase;

    void RetrieveFileFromDatabase<TFileInformation>(
      IVssRequestContext requestContext,
      TFileInformation fileInfo,
      IDownloadState<TFileInformation> downloadState,
      bool compressOutput,
      Stream databaseStream,
      bool saveInCache = false)
      where TFileInformation : FileInformationBase;

    ProxyStatistics Statistics { get; }

    void UpdateCacheSize(
      IVssRequestContext requestContext,
      string serverId,
      long cacheSize,
      int fileCount);
  }
}
