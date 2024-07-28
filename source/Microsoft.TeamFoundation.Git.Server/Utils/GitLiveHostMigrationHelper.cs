// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Utils.GitLiveHostMigrationHelper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Utils
{
  public static class GitLiveHostMigrationHelper
  {
    internal static bool CopyGitContainerContents(
      IVssRequestContext requestContext,
      OdbId odbId,
      StorageMigration sourceContainerSasTokenInfo)
    {
      try
      {
        string containerName = odbId.Value.ToString("n");
        StorageCredentials credentials = new StorageCredentials(sourceContainerSasTokenInfo.SasToken);
        CloudBlobContainer cloudBlobContainer = new CloudBlobContainer(new Uri(sourceContainerSasTokenInfo.Uri), credentials);
        CloudBlobContainer containerReference = CloudStorageAccount.Parse(GitLiveHostMigrationHelper.GetGitStorageConnectionString(requestContext)).CreateCloudBlobClient().GetContainerReference(containerName);
        containerReference.CreateIfNotExists();
        requestContext.Trace(14621, TraceLevel.Info, nameof (GitLiveHostMigrationHelper), nameof (CopyGitContainerContents), "Setting up blob copy for container" + containerName);
        cloudBlobContainer.ListBlobs(useFlatBlobListing: true);
        List<CloudBlockBlob> cloudBlockBlobList = new List<CloudBlockBlob>();
        int num1 = 0;
        int num2 = 0;
        foreach (IListBlobItem listBlob in cloudBlobContainer.ListBlobs(useFlatBlobListing: true))
        {
          try
          {
            string fileName = Path.GetFileName(listBlob.Uri.ToString());
            CloudBlockBlob blockBlobReference1 = cloudBlobContainer.GetBlockBlobReference(fileName);
            CloudBlockBlob blockBlobReference2 = containerReference.GetBlockBlobReference(fileName);
            blockBlobReference2.StartCopyAsync(blockBlobReference1, requestContext.CancellationToken);
            cloudBlockBlobList.Add(blockBlobReference2);
            ++num1;
          }
          catch (StorageException ex)
          {
            ++num2;
            requestContext.Trace(14622, TraceLevel.Warning, nameof (GitLiveHostMigrationHelper), nameof (CopyGitContainerContents), string.Format("Failed to request blob copy for : {0}, ErrorMessage: {1}, StackTrace: {2}", (object) 0, (object) 1, (object) 2), (object) listBlob.Uri.ToString(), (object) ex.Message, (object) ex.StackTrace);
          }
        }
        requestContext.Trace(14621, TraceLevel.Info, nameof (GitLiveHostMigrationHelper), nameof (CopyGitContainerContents), string.Format("Blob copy setup complete. Total copies requested: {0}. Copy Failures: {1}. Monitoring for copy completion.", (object) 0, (object) 1), (object) num1, (object) num2);
        bool flag = true;
        while (flag)
        {
          requestContext.CancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(5.0));
          flag = false;
          int num3 = 0;
          int num4 = 0;
          int num5 = 0;
          foreach (CloudBlockBlob cloudBlockBlob in cloudBlockBlobList)
          {
            try
            {
              cloudBlockBlob.FetchAttributes((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null);
              if (cloudBlockBlob.CopyState != null)
              {
                if (cloudBlockBlob.CopyState.Status == CopyStatus.Pending)
                {
                  ++num4;
                  flag = true;
                }
                else if (cloudBlockBlob.CopyState.Status == CopyStatus.Success)
                {
                  ++num3;
                }
                else
                {
                  if (cloudBlockBlob.CopyState.Status != CopyStatus.Failed && cloudBlockBlob.CopyState.Status != CopyStatus.Aborted)
                  {
                    if (cloudBlockBlob.CopyState.Status != CopyStatus.Invalid)
                      continue;
                  }
                  ++num5;
                }
              }
            }
            catch (StorageException ex)
            {
              requestContext.Trace(14622, TraceLevel.Warning, nameof (GitLiveHostMigrationHelper), nameof (CopyGitContainerContents), string.Format("Failure occured during monitoring : {0}, ErrorMessage: {1}, StackTrace: {2}", (object) 0, (object) 1, (object) 2), (object) cloudBlockBlob.Uri.ToString(), (object) ex.Message, (object) ex.StackTrace);
            }
          }
          requestContext.Trace(14621, TraceLevel.Info, nameof (GitLiveHostMigrationHelper), nameof (CopyGitContainerContents), string.Format("There are still {0} copies pending completion. Total blobs copied so far {1}", (object) 0, (object) 1), (object) num4, (object) num3);
          if (num5 > 0)
            requestContext.TraceAlways(14622, TraceLevel.Warning, nameof (GitLiveHostMigrationHelper), nameof (CopyGitContainerContents), string.Format("There are {0} blobs reporting either failed, aborted or invalid state", (object) 0), (object) num5);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(14622, TraceLevel.Warning, nameof (GitLiveHostMigrationHelper), nameof (CopyGitContainerContents), string.Format("An exception occurred copying container {0}. Error Message: {1}, StackTrace {2}", (object) 0, (object) 1, (object) 2), (object) odbId.Value.ToString("n"), (object) ex.Message, (object) ex.StackTrace);
        return false;
      }
      return true;
    }

    public static string GetGitStorageConnectionString(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, "ConfigurationSecrets", "GitBlobStorageConnectionString", true);
      return service.GetString(vssRequestContext, itemInfo);
    }

    public static StorageMigration GetSasTokenForSourceContainer(
      IVssRequestContext rc,
      OdbId odbId,
      TargetHostMigration latestMigration)
    {
      try
      {
        StorageMigration storageMigration = ((IEnumerable<StorageMigration>) latestMigration.StorageResources).FirstOrDefault<StorageMigration>((Func<StorageMigration, bool>) (x => x.VsoArea == "Git"));
        int startIndex = storageMigration.Uri.LastIndexOf('/') + 1;
        storageMigration.Uri = storageMigration.Uri.Remove(startIndex).Insert(startIndex, odbId.Value.ToString("N"));
        return HostMigrationUtil.UpdateSasToken(rc, latestMigration, storageMigration, (Action<string>) (logMsg => rc.Trace(14622, TraceLevel.Info, "AzureGitBlobProvider", nameof (GetSasTokenForSourceContainer), logMsg)));
      }
      catch (Exception ex)
      {
        rc.Trace(14622, TraceLevel.Warning, "AzureGitBlobProvider", nameof (GetSasTokenForSourceContainer), string.Format("Failed to get SAS token for container. Error message: {0}, StackTrace: {1}", (object) 0, (object) 1), (object) ex.Message, (object) ex.StackTrace);
        return (StorageMigration) null;
      }
    }

    public static TargetHostMigration GetLatestTargetMigrationForHost(
      IVssRequestContext rc,
      Guid hostId)
    {
      TargetHostMigration migrationForHost = (TargetHostMigration) null;
      using (HostMigrationComponent component = rc.CreateComponent<HostMigrationComponent>())
      {
        Guid migrationId = component.QueryLatestTargetMigrationByHostId(hostId);
        if (migrationId != Guid.Empty)
        {
          migrationForHost = component.GetTargetMigration(migrationId);
          migrationForHost.HostProperties = new MigrationHostProperties()
          {
            Id = migrationForHost.HostId,
            HostType = migrationForHost.HostType
          };
        }
      }
      return migrationForHost;
    }
  }
}
