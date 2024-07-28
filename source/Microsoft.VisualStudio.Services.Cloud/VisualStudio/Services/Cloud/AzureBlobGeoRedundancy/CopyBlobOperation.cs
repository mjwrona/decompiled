// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.CopyBlobOperation
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  internal class CopyBlobOperation : IRepairOperation
  {
    public string ContainerName { get; set; }

    public string BlobName { get; set; }

    public CloudBlobClient SourceClient { get; set; }

    public CloudBlobClient TargetClient { get; set; }

    public void Repair()
    {
      CloudBlobContainer containerReference1 = this.SourceClient.GetContainerReference(this.ContainerName);
      CloudBlobContainer containerReference2 = this.TargetClient.GetContainerReference(this.ContainerName);
      CloudBlob blobReference = containerReference1.GetBlobReference(this.BlobName);
      blobReference.FetchAttributes();
      CloudBlob cloudBlob1;
      CloudBlob cloudBlob2;
      switch (blobReference.BlobType)
      {
        case BlobType.PageBlob:
          cloudBlob1 = (CloudBlob) containerReference1.GetPageBlobReference(this.BlobName);
          cloudBlob2 = (CloudBlob) containerReference2.GetPageBlobReference(this.BlobName);
          break;
        case BlobType.BlockBlob:
          cloudBlob1 = (CloudBlob) containerReference1.GetBlockBlobReference(this.BlobName);
          cloudBlob2 = (CloudBlob) containerReference2.GetBlockBlobReference(this.BlobName);
          break;
        case BlobType.AppendBlob:
          cloudBlob1 = (CloudBlob) containerReference1.GetAppendBlobReference(this.BlobName);
          cloudBlob2 = (CloudBlob) containerReference2.GetAppendBlobReference(this.BlobName);
          break;
        default:
          throw new InvalidOperationException(string.Format("Blob type '{0}' is not supported", (object) blobReference.BlobType));
      }
      try
      {
        TransferManager.CopyAsync(cloudBlob1, cloudBlob2, (CopyMethod) 0, (CopyOptions) null, (SingleTransferContext) null).ConfigureAwait(false).GetAwaiter().GetResult();
      }
      catch (Exception ex) when (AzureBlobGeoRedundancyUtils.IsDualWriteBlockIdError(ex))
      {
        if (cloudBlob2 is CloudBlockBlob blob)
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          TraceMethod traceMethod = CopyBlobOperation.\u003C\u003EO.\u003C0\u003E__TraceRaw ?? (CopyBlobOperation.\u003C\u003EO.\u003C0\u003E__TraceRaw = new TraceMethod(TeamFoundationTracingService.TraceRaw));
          CancellationToken cancellationToken = new CancellationToken();
          if (AzureBlobGeoRedundancyUtils.TryRecoverDualWriteBlockIdError(blob, traceMethod, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult())
          {
            TransferManager.CopyAsync(cloudBlob1, cloudBlob2, (CopyMethod) 0, (CopyOptions) null, (SingleTransferContext) null).ConfigureAwait(false).GetAwaiter().GetResult();
            return;
          }
        }
        throw;
      }
      catch (TransferException ex)
      {
        if (((Exception) ex).InnerException is StorageException innerException)
        {
          RequestResult requestInformation = innerException.RequestInformation;
          if ((requestInformation != null ? (requestInformation.HttpStatusCode == 404 ? 1 : 0) : 0) != 0 && innerException.RequestInformation?.ExtendedErrorInformation?.ErrorCode == BlobErrorCodeStrings.ContainerNotFound)
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            AzureBlobGeoRedundancyUtils.SetupSecondaryContainer(containerReference1, containerReference2, CopyBlobOperation.\u003C\u003EO.\u003C0\u003E__TraceRaw ?? (CopyBlobOperation.\u003C\u003EO.\u003C0\u003E__TraceRaw = new TraceMethod(TeamFoundationTracingService.TraceRaw)));
            TransferManager.CopyAsync(cloudBlob1, cloudBlob2, (CopyMethod) 0, (CopyOptions) null, (SingleTransferContext) null).ConfigureAwait(false).GetAwaiter().GetResult();
            return;
          }
        }
        throw;
      }
    }
  }
}
