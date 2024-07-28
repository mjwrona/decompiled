// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.AzureGitBlobProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.Azure.Storage;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Microsoft.VisualStudio.Services.Cloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class AzureGitBlobProvider : ITfsGitBlobProvider
  {
    private const string c_layer = "AzureGitBlobProvider";

    public void CreateRepository(IVssRequestContext rc, OdbId odbId)
    {
    }

    public void DeleteBlob(IVssRequestContext rc, OdbId odbId, string resourceId)
    {
      odbId.CheckValid();
      this.GetBlobProvider(rc).DeleteBlob(rc, odbId.Value, resourceId);
    }

    public void DeleteContainer(IVssRequestContext rc, OdbId odbId, bool throwIfContainerNotExists = true)
    {
      odbId.CheckValid();
      try
      {
        this.GetBlobProvider(rc).DeleteContainer(rc, odbId.Value);
      }
      catch (StorageException ex) when (!throwIfContainerNotExists && this.StorageExceptionIsForMissingItem(ex))
      {
      }
    }

    public bool DownloadToStream(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      Stream destination,
      bool throwIfNotFound = true)
    {
      odbId.CheckValid();
      try
      {
        this.GetBlobProvider(rc).DownloadToStreamLargeBlocks(rc, odbId.Value, resourceId, destination);
        return true;
      }
      catch (StorageException ex)
      {
        rc.TraceCatch(1013946, "FileService", "BlobStorage", (Exception) ex);
        TargetHostMigration migrationForHost = GitLiveHostMigrationHelper.GetLatestTargetMigrationForHost(rc.To(TeamFoundationHostType.Deployment), rc.ServiceHost.InstanceId);
        if (migrationForHost != null)
          return this.GetFilesFromMigrationSource(rc, odbId, resourceId, destination, throwIfNotFound, migrationForHost);
        if (!throwIfNotFound && this.StorageExceptionIsForMissingItem(ex))
          return false;
        throw ex;
      }
    }

    public IEnumerable<string> EnumerateBlobs(
      IVssRequestContext rc,
      OdbId odbId,
      TimeSpan? enumerateBlobsClientTimeout = null)
    {
      odbId.CheckValid();
      return this.GetBlobProvider(rc).EnumerateBlobs(rc, odbId.Value, enumerateBlobsClientTimeout);
    }

    public Stream GetStream(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      bool throwIfNotFound = true)
    {
      odbId.CheckValid();
      try
      {
        return this.GetBlobProvider(rc).GetStream(rc, odbId.Value, resourceId);
      }
      catch (StorageException ex) when (!throwIfNotFound && this.StorageExceptionIsForMissingItem(ex))
      {
        return (Stream) null;
      }
    }

    public BlobProperties GetProperties(
      IVssRequestContext rc,
      OdbId odbId,
      string blobName,
      bool throwIfNotFound = true)
    {
      odbId.CheckValid();
      try
      {
        return this.GetBlobProvider(rc).ReadBlobProperties(rc, odbId.Value, blobName);
      }
      catch (StorageException ex) when (!throwIfNotFound && this.StorageExceptionIsForMissingItem(ex))
      {
        return (BlobProperties) null;
      }
    }

    public void PutChunk(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      byte[] contentBlock,
      int contentBlockLength,
      long fileLength,
      long offset,
      bool isLastChunk)
    {
      odbId.CheckValid();
      this.GetBlobProvider(rc).PutChunk(rc, odbId.Value, resourceId, contentBlock, contentBlockLength, fileLength, offset, isLastChunk);
    }

    public void PutStream(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      Stream stream,
      long streamLength)
    {
      odbId.CheckValid();
      this.GetBlobProvider(rc).PutStream(rc, odbId.Value, resourceId, stream, (IDictionary<string, string>) null);
    }

    public void RenameBlob(
      IVssRequestContext rc,
      OdbId odbId,
      string sourceResourceId,
      string targetResourceId)
    {
      odbId.CheckValid();
      try
      {
        this.GetBlobProvider(rc).RenameBlob(rc, odbId.Value, sourceResourceId, targetResourceId);
      }
      catch (StorageException ex) when (BlobCopyUtil.HasHttpStatusCode((Exception) ex, HttpStatusCode.Forbidden))
      {
        if (rc.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(rc, FrameworkServerConstants.EnableCompositeBlobProviderRenameGenerateSasTokenFF))
        {
          TargetHostMigration migrationForHost = GitLiveHostMigrationHelper.GetLatestTargetMigrationForHost(rc.To(TeamFoundationHostType.Deployment), rc.ServiceHost.InstanceId);
          if (migrationForHost != null)
          {
            AzureGitBlobProvider.GetSasTokenForContainer(rc, odbId, migrationForHost);
            this.GetBlobProvider(rc).RenameBlob(rc, odbId.Value, sourceResourceId, targetResourceId);
            return;
          }
        }
        throw;
      }
    }

    private static StorageMigration GetSasTokenForContainer(
      IVssRequestContext rc,
      OdbId odbId,
      TargetHostMigration latestMigration)
    {
      IVssRequestContext vssRequestContext = rc.To(TeamFoundationHostType.Deployment);
      using (IVssRequestContext rc1 = vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, vssRequestContext.ServiceHost.InstanceId, RequestContextType.ServicingContext, throwIfShutdown: false))
        return GitLiveHostMigrationHelper.GetSasTokenForSourceContainer(rc1, odbId, latestMigration);
    }

    private bool GetFilesFromMigrationSource(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      Stream destination,
      bool throwIfNotFound,
      TargetHostMigration latestMigration)
    {
      StorageMigration tokenForContainer = AzureGitBlobProvider.GetSasTokenForContainer(rc, odbId, latestMigration);
      bool flag = false;
      IVssRequestContext vssRequestContext = rc.To(TeamFoundationHostType.Deployment);
      using (IVssRequestContext requestContext = vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, vssRequestContext.ServiceHost.InstanceId, RequestContextType.ServicingContext, throwIfShutdown: false))
      {
        if (tokenForContainer == null)
          return false;
        flag = GitLiveHostMigrationHelper.CopyGitContainerContents(requestContext, odbId, tokenForContainer);
      }
      if (!flag)
        return false;
      try
      {
        this.GetBlobProvider(rc).DownloadToStreamLargeBlocks(rc, odbId.Value, resourceId, destination);
        return true;
      }
      catch (StorageException ex) when (!throwIfNotFound && this.StorageExceptionIsForMissingItem(ex))
      {
        return false;
      }
    }

    private bool StorageExceptionIsForMissingItem(StorageException se) => se.RequestInformation != null && se.RequestInformation.HttpStatusCode == 404 || se.InnerException is WebException innerException && innerException.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.NotFound;

    private IBlobProvider GetBlobProvider(IVssRequestContext rc)
    {
      IVssRequestContext vssRequestContext = rc.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<AzureGitBlobProviderService>().GetBlobProvider(vssRequestContext);
    }
  }
}
