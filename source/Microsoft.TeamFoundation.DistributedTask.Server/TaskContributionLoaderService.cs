// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskContributionLoaderService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class TaskContributionLoaderService : 
    ITaskContributionLoaderService,
    IVssFrameworkService
  {
    internal TaskContributionLoaderService()
      : this((ITaskContributionPackageStore) new TaskContributionLoaderService.GalleryPackageStore())
    {
    }

    internal TaskContributionLoaderService(ITaskContributionPackageStore packageStore) => this.PackageStore = packageStore;

    public ITaskContributionPackageStore PackageStore { get; }

    public async Task<TaskContributionPackage> LoadAsync(
      IVssRequestContext requestContext,
      ExtensionIdentifier extensionId,
      string version)
    {
      TaskContributionPackage contributionPackage;
      using (new MethodScope(requestContext, "TaskContributionPackage", nameof (LoadAsync)))
      {
        ArgumentUtility.CheckForNull<ExtensionIdentifier>(extensionId, nameof (extensionId));
        ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version));
        if (!Version.TryParse(version, out Version _))
          throw new ArgumentException("The value " + version + " is not a valid version", nameof (version));
        Stream packageStream = (Stream) null;
        TaskContributionPackage package = (TaskContributionPackage) null;
        ITaskPackageReader packageReader = (ITaskPackageReader) null;
        try
        {
          packageStream = await this.GetPackageStreamAsync(requestContext, extensionId, version);
          packageReader = (ITaskPackageReader) new ZipPackageReader(new ZipArchive(packageStream, ZipArchiveMode.Read, false), (string) null, true);
          package = TaskContributionPackage.Create(requestContext, extensionId, version, packageReader);
        }
        finally
        {
          if (package == null)
          {
            packageStream?.Dispose();
            packageReader?.Dispose();
          }
        }
        contributionPackage = package;
      }
      return contributionPackage;
    }

    private async Task<Stream> GetPackageStreamAsync(
      IVssRequestContext requestContext,
      ExtensionIdentifier identifier,
      string version)
    {
      using (new MethodScope(requestContext, nameof (TaskContributionLoaderService), nameof (GetPackageStreamAsync)))
      {
        TaskContributionPackageCacheFile fileInfo = new TaskContributionPackageCacheFile(requestContext, identifier, version);
        ITeamFoundationFileCacheService fileCacheService = requestContext.GetService<ITeamFoundationFileCacheService>();
        using (TaskContributionLoaderService.VsixCacheDownloadState downloadState = new TaskContributionLoaderService.VsixCacheDownloadState())
        {
          if (fileCacheService.RetrieveFileFromCache<TaskContributionPackageCacheFile>(requestContext, fileInfo, (IDownloadState<TaskContributionPackageCacheFile>) downloadState, true))
            return downloadState.GetFileStream();
          Stream fileStream = (Stream) null;
          Stream assetStream = (Stream) null;
          bool successful = false;
          try
          {
            assetStream = await this.PackageStore.GetPackageStreamAsync(requestContext, identifier, version);
            fileStream = (Stream) System.IO.File.Create(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")), 81920, FileOptions.DeleteOnClose);
            await assetStream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            fileInfo.Length = fileStream.Length;
            fileStream.Seek(0L, SeekOrigin.Begin);
            fileCacheService.RetrieveFileFromDatabase<TaskContributionPackageCacheFile>(requestContext, fileInfo, (IDownloadState<TaskContributionPackageCacheFile>) downloadState, true, fileStream, true);
            fileStream.Seek(0L, SeekOrigin.Begin);
            successful = true;
          }
          finally
          {
            assetStream?.Dispose();
            if (!successful)
              fileStream?.Dispose();
          }
          return fileStream;
        }
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    private class GalleryPackageStore : ITaskContributionPackageStore
    {
      public async Task<Stream> GetPackageStreamAsync(
        IVssRequestContext requestContext,
        ExtensionIdentifier identifier,
        string version)
      {
        MethodScope methodScope = new MethodScope(requestContext, nameof (GalleryPackageStore), nameof (GetPackageStreamAsync));
        try
        {
          string accountToken = await requestContext.GetClient<ExtensionManagementHttpClient>().GetTokenAsync();
          IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
          GalleryHttpClient galleryClient = deploymentContext.GetClient<GalleryHttpClient>();
          AssetHttpClient assetClient = AssetHttpClientFactory.CreateClient(requestContext);
          bool getExtensionsFromCDN = requestContext.IsFeatureEnabled("DistributedTask.GetExtensionsFromCDN");
          int num;
          try
          {
            if (!getExtensionsFromCDN)
              return await galleryClient.GetAssetByNameAsync(identifier.PublisherName, identifier.ExtensionName, version, "Microsoft.VisualStudio.Services.VSIXPackage", accountTokenHeader: accountToken);
            string assetUri = (await galleryClient.GetExtensionAsync(identifier.PublisherName, identifier.ExtensionName, version, new ExtensionQueryFlags?(ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeAssetUri), accountTokenHeader: accountToken)).Versions[0].AssetUri;
            char[] chArray = new char[1]{ '/' };
            return await (await assetClient.GetAsset(assetUri.TrimEnd(chArray) + "/Microsoft.VisualStudio.Services.VSIXPackage")).Content.ReadAsStreamAsync();
          }
          catch (VssServiceResponseException ex)
          {
            num = 1;
          }
          if (num == 1)
          {
            if (ex.HttpStatusCode == HttpStatusCode.Found)
            {
              ConnectionData connectionDataAsync = await deploymentContext.GetClient<LocationHttpClient>(new Guid("00000029-0000-8888-8000-000000000000")).GetConnectionDataAsync(ConnectOptions.None, 0L);
              if (!getExtensionsFromCDN)
                return await galleryClient.GetAssetByNameAsync(identifier.PublisherName, identifier.ExtensionName, version, "Microsoft.VisualStudio.Services.VSIXPackage", accountTokenHeader: accountToken);
              string assetUri = (await galleryClient.GetExtensionAsync(identifier.PublisherName, identifier.ExtensionName, version, new ExtensionQueryFlags?(ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeAssetUri), accountTokenHeader: accountToken)).Versions[0].AssetUri;
              char[] chArray = new char[1]{ '/' };
              return await (await assetClient.GetAsset(assetUri.TrimEnd(chArray) + "/Microsoft.VisualStudio.Services.VSIXPackage")).Content.ReadAsStreamAsync();
            }
            if (!((object) ex is Exception source))
              throw (object) ex;
            ExceptionDispatchInfo.Capture(source).Throw();
          }
          accountToken = (string) null;
          deploymentContext = (IVssRequestContext) null;
          galleryClient = (GalleryHttpClient) null;
          assetClient = (AssetHttpClient) null;
        }
        finally
        {
          methodScope.Dispose();
        }
        methodScope = new MethodScope();
        Stream packageStreamAsync;
        return packageStreamAsync;
      }
    }

    internal class VsixCacheDownloadState : 
      IDownloadState<TaskContributionPackageCacheFile>,
      IDisposable
    {
      private string m_path;
      private int m_offset;
      private long m_length;

      public Stream CacheMiss(
        FileCacheService fileCacheService,
        TaskContributionPackageCacheFile fileInfo,
        bool compressOutput)
      {
        throw new NotSupportedException();
      }

      public void Dispose()
      {
      }

      public Stream GetFileStream() => (Stream) new TaskContributionLoaderService.VsixCacheDownloadState.CacheFileStream(this.m_path, this.m_offset, this.m_length);

      public bool TransmitChunk(
        TaskContributionPackageCacheFile fileInformation,
        byte[] chunk,
        long offset,
        long length)
      {
        return true;
      }

      public bool TransmitFile(
        TaskContributionPackageCacheFile fileInformation,
        string path,
        long offset,
        long length)
      {
        this.m_path = path;
        this.m_offset = (int) offset;
        this.m_length = length;
        return true;
      }

      private sealed class CacheFileStream : Stream
      {
        private long m_length;
        private int m_baseOffset;
        private Stream m_fileStream;

        public CacheFileStream(string filePath, int offset, long length)
        {
          this.m_length = length;
          this.m_baseOffset = offset;
          this.m_fileStream = (Stream) System.IO.File.OpenRead(filePath);
          this.m_fileStream.Seek((long) this.m_baseOffset, SeekOrigin.Begin);
        }

        public override void Flush() => this.m_fileStream.Flush();

        public override bool CanRead => this.m_fileStream.CanRead;

        public override bool CanSeek => this.m_fileStream.CanSeek;

        public override bool CanWrite => false;

        public override long Length => this.m_length;

        public override long Position
        {
          get => this.m_fileStream.Position - (long) this.m_baseOffset;
          set => this.m_fileStream.Position = (long) this.m_baseOffset + value;
        }

        protected override void Dispose(bool disposing)
        {
          if (disposing && this.m_fileStream != null)
          {
            this.m_fileStream.Dispose();
            this.m_fileStream = (Stream) null;
          }
          base.Dispose(disposing);
        }

        public override long Seek(long offset, SeekOrigin origin) => origin == SeekOrigin.Begin ? this.m_fileStream.Seek((long) this.m_baseOffset + offset, origin) - (long) this.m_baseOffset : this.m_fileStream.Seek(offset, origin) - (long) this.m_baseOffset;

        public override void SetLength(long value) => throw new NotSupportedException();

        public override int Read(byte[] buffer, int offset, int count) => this.m_fileStream.Read(buffer, offset, count);

        public override Task<int> ReadAsync(
          byte[] buffer,
          int offset,
          int count,
          CancellationToken cancellationToken)
        {
          return this.m_fileStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
      }
    }
  }
}
