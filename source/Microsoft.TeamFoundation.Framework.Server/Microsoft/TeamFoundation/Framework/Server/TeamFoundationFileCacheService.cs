// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationFileCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationFileCacheService : ITeamFoundationFileCacheService, IVssFrameworkService
  {
    private FileCacheService m_fileCacheService;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.m_fileCacheService = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<FileCacheService>();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => this.m_fileCacheService = (FileCacheService) null;

    public void RetrieveFile<TFileInformation>(
      IVssRequestContext requestContext,
      TFileInformation fileInfo,
      IDownloadState<TFileInformation> downloadState,
      bool compressOutput)
      where TFileInformation : FileInformationBase
    {
      if (this.RetrieveFileFromCache<TFileInformation>(requestContext, fileInfo, downloadState, compressOutput))
        return;
      this.m_fileCacheService.RetrieveFileFromDatabase<TFileInformation>(requestContext, fileInfo, downloadState, compressOutput, (Stream) null, true);
    }

    public bool RetrieveFileFromCache<TFileInformation>(
      IVssRequestContext requestContext,
      TFileInformation fileInfo,
      IDownloadState<TFileInformation> downloadState,
      bool compressOutput)
      where TFileInformation : FileInformationBase
    {
      return this.m_fileCacheService.RetrieveFileFromCache<TFileInformation>(requestContext, fileInfo, downloadState, compressOutput);
    }

    public void RetrieveFileFromDatabase<TFileInformation>(
      IVssRequestContext requestContext,
      TFileInformation fileInfo,
      IDownloadState<TFileInformation> downloadState,
      bool compressOutput,
      Stream databaseStream,
      bool saveInCache = false)
      where TFileInformation : FileInformationBase
    {
      this.m_fileCacheService.RetrieveFileFromDatabase<TFileInformation>(requestContext, fileInfo, downloadState, compressOutput, databaseStream, saveInCache);
    }

    public void UpdateCacheSize(
      IVssRequestContext requestContext,
      string serverId,
      long cacheSize,
      int fileCount)
    {
      this.m_fileCacheService.UpdateCacheSize(requestContext, serverId, cacheSize, fileCount);
    }

    internal void ReloadConfiguration(IVssRequestContext systemRequestContext) => this.m_fileCacheService.ReloadConfiguration(systemRequestContext.To(TeamFoundationHostType.Deployment));

    public ProxyConfiguration Configuration => this.m_fileCacheService.Configuration;

    public bool IsGitCacheEnabled => this.m_fileCacheService.IsGitCacheEnabled;

    public bool IsVCCacheEnabled => this.m_fileCacheService.IsVCCacheEnabled;

    public ProxyStatistics Statistics => this.m_fileCacheService.Statistics;

    public bool IsCacheCleanupInProgress => this.m_fileCacheService.CacheCleanupInProgress;
  }
}
