// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyUtils
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public static class AzureBlobGeoRedundancyUtils
  {
    private static readonly string s_baseRegistryPath = "/Service/AzureBlobGeoRedundancy/Settings/Utils";
    private static readonly RegistryQuery s_queueConnectionLimit = (RegistryQuery) (AzureBlobGeoRedundancyUtils.s_baseRegistryPath + "/QueueConnectionLimit");
    private static readonly RegistryQuery s_tableConnectionLimit = (RegistryQuery) (AzureBlobGeoRedundancyUtils.s_baseRegistryPath + "/TableConnectionLimit");
    private static readonly RegistryQuery s_blobConnectionLimit = (RegistryQuery) (AzureBlobGeoRedundancyUtils.s_baseRegistryPath + "/BlobConnectionLimit");
    private const string c_area = "AzureBlobGeoRedundancy";
    private const string c_layer = "AzureBlobGeoRedundancyUtils";

    public static void CheckValidBlobActionMessageValues(BlobActionMessage blobActionMessage)
    {
      if (string.IsNullOrEmpty(blobActionMessage.BlobName) && blobActionMessage.Action != BlobAction.CreateContainer && blobActionMessage.Action != BlobAction.DeleteContainer)
        throw new ArgumentException(string.Format("BlobName is empty, this must have a value for blob specific calls. Action being attempted {0}", (object) blobActionMessage.Action.ToString()));
      if (string.IsNullOrEmpty(blobActionMessage.ContainerName))
        throw new ArgumentException(string.Format("ContainerName is empty, this must always have a value for blob and container specific calls. Action being attempted {0}", (object) blobActionMessage.Action.ToString()));
    }

    public static string GetQueueName(int index) => string.Format("{0}{1}", (object) "azureblobgeoredundancyqueue", (object) index);

    public static string GetStorageAccountConnectionString(
      IVssRequestContext requestContext,
      string drawerName,
      string lookupKey,
      bool throwIfNotFound = true)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, drawerName, lookupKey, throwIfNotFound);
      return itemInfo == null ? (string) null : service.GetString(requestContext, itemInfo);
    }

    public static bool StorageAccountConnectionStringExists(
      IVssRequestContext requestContext,
      string drawerName,
      string lookupKey)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, drawerName, lookupKey, false);
      return itemInfo != null && !string.IsNullOrEmpty(service.GetString(requestContext, itemInfo));
    }

    public static void OptimizeQueueServiceEndpoint(
      IVssRequestContext requestContext,
      CloudStorageAccount account)
    {
      ServicePoint servicePoint = ServicePointManager.FindServicePoint(account.QueueEndpoint);
      servicePoint.UseNagleAlgorithm = false;
      servicePoint.Expect100Continue = false;
      AzureBlobGeoRedundancyUtils.SetConnectionLimit(requestContext, servicePoint, AzureBlobGeoRedundancyUtils.s_queueConnectionLimit);
    }

    public static void OptimizeTableServiceEndpoint(
      IVssRequestContext requestContext,
      CloudStorageAccount account)
    {
      ServicePoint servicePoint = ServicePointManager.FindServicePoint(account.TableEndpoint);
      servicePoint.UseNagleAlgorithm = false;
      servicePoint.Expect100Continue = false;
      AzureBlobGeoRedundancyUtils.SetConnectionLimit(requestContext, servicePoint, AzureBlobGeoRedundancyUtils.s_tableConnectionLimit);
    }

    public static void OptimizeBlobServiceEndpoint(
      IVssRequestContext requestContext,
      CloudStorageAccount account)
    {
      ServicePoint servicePoint = ServicePointManager.FindServicePoint(account.BlobEndpoint);
      servicePoint.UseNagleAlgorithm = false;
      servicePoint.Expect100Continue = false;
      AzureBlobGeoRedundancyUtils.SetConnectionLimit(requestContext, servicePoint, AzureBlobGeoRedundancyUtils.s_blobConnectionLimit, new int?(512));
    }

    public static void SetConnectionLimit(
      IVssRequestContext requestContext,
      ServicePoint servicePoint,
      RegistryQuery registryPath,
      int? defaultValue = null)
    {
      int? nullable = requestContext.GetService<IVssRegistryService>().GetValue<int?>(requestContext, in registryPath, defaultValue);
      if (!nullable.HasValue)
        return;
      servicePoint.ConnectionLimit = nullable.Value;
    }

    public static bool SetupSecondaryContainer(
      CloudBlobContainer localContainer,
      CloudBlobContainer remoteContainer,
      TraceMethod traceMethod)
    {
      if (remoteContainer.Exists())
        return false;
      BlobContainerPublicAccessType? nullable = new BlobContainerPublicAccessType?();
      try
      {
        localContainer.FetchAttributes();
        nullable = localContainer.Properties.PublicAccess;
      }
      catch (StorageException ex)
      {
        traceMethod(15306000, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyUtils), "Unable to read blob container properties to determine public access type. Defaulting to private. Account: {0}, Container: {1}, Exception: {2}", new object[3]
        {
          (object) localContainer.ServiceClient.Credentials?.AccountName,
          (object) localContainer.Name,
          (object) ex
        });
      }
      return remoteContainer.CreateIfNotExists(nullable.GetValueOrDefault());
    }

    public static async Task<bool> SetupSecondaryContainerAsync(
      CloudBlobContainer localContainer,
      CloudBlobContainer remoteContainer,
      TraceMethod traceMethod,
      CancellationToken cancellationToken)
    {
      if (await remoteContainer.ExistsAsync(cancellationToken))
        return false;
      BlobContainerPublicAccessType? publicAccessType = new BlobContainerPublicAccessType?();
      try
      {
        await localContainer.FetchAttributesAsync(cancellationToken);
        publicAccessType = localContainer.Properties.PublicAccess;
      }
      catch (StorageException ex)
      {
        traceMethod(15306000, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyUtils), "Unable to read blob container properties to determine public access type. Defaulting to private. Account: {0}, Container: {1}, Exception: {2}", new object[3]
        {
          (object) localContainer.ServiceClient.Credentials?.AccountName,
          (object) localContainer.Name,
          (object) ex
        });
      }
      return await remoteContainer.CreateIfNotExistsAsync(publicAccessType.GetValueOrDefault(), (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    public static bool IsDualWriteBlockIdError(Exception e)
    {
      if (e is TransferException transferException && ((Exception) transferException).InnerException is StorageException)
        e = ((Exception) transferException).InnerException;
      return e is StorageException storageException && storageException.RequestInformation?.ExtendedErrorInformation?.ErrorCode == BlobErrorCodeStrings.InvalidBlobOrBlock;
    }

    public static async Task<bool> TryRecoverDualWriteBlockIdError(
      CloudBlockBlob blob,
      TraceMethod traceMethod,
      CancellationToken cancellationToken)
    {
      traceMethod(15306001, TraceLevel.Warning, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyUtils), "Attempting to recover bad blob caused by dual-write block ids. Account: {0}, Container: {1}, Blob: {2}", new object[3]
      {
        (object) blob.ServiceClient.Credentials?.AccountName,
        (object) blob.Container.Name,
        (object) blob.Name
      });
      if (await blob.ExistsAsync(cancellationToken))
      {
        traceMethod(15306002, TraceLevel.Warning, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyUtils), "Failed to recover bad blob caused by dual-write block ids. The destination blob already exists. Account: {0}, Container: {1}, Blob: {2}.", new object[3]
        {
          (object) blob.ServiceClient.Credentials?.AccountName,
          (object) blob.Container.Name,
          (object) blob.Name
        });
        return false;
      }
      try
      {
        await blob.PutBlockListAsync((IEnumerable<string>) Array.Empty<string>(), cancellationToken);
        int num = await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.None, AccessCondition.GenerateIfMatchCondition(blob.Properties.ETag), (BlobRequestOptions) null, (OperationContext) null, cancellationToken) ? 1 : 0;
        traceMethod(15306003, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyUtils), "Successfully repaired bad blob caused by dual-write block ids. Account: {0}, Container: {1}, Blob: {2}.", new object[3]
        {
          (object) blob.ServiceClient.Credentials?.AccountName,
          (object) blob.Container.Name,
          (object) blob.Name
        });
        return true;
      }
      catch (Exception ex)
      {
        traceMethod(15306002, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyUtils), "Failed to recover bad blob caused by dual-write block ids. Account: {0}, Container: {1}, Blob: {2}, Exception: {3}", new object[4]
        {
          (object) blob.ServiceClient.Credentials?.AccountName,
          (object) blob.Container.Name,
          (object) blob.Name,
          (object) ex
        });
        return false;
      }
    }

    public static Guid IndexToCatchupJobId(int index)
    {
      ArgumentUtility.CheckForOutOfRange(index, nameof (index), 0, (int) ushort.MaxValue);
      return Guid.Parse(string.Format("00a2c097-{0:x4}-36b0-4207-c097a11b10b5", (object) index));
    }
  }
}
