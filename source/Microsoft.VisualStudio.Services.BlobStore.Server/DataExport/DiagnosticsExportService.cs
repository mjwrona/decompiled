// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DataExport.DiagnosticsExportService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DataExport
{
  public class DiagnosticsExportService : IDiagnosticsExportService, IVssFrameworkService
  {
    public static readonly string DiagnosticsConnectionStringSecretDrawer = FrameworkServerConstants.ConfigurationSecretsDrawerName;
    public const string DiagnosticsConnectionStringSecretName = "DiagnosticsConnectionString";
    public const string DataExportContainer = "artifactservicesdataexport";
    private CloudBlobClient cloudBlobClient;

    public void ServiceStart(IVssRequestContext deploymentRequestContext)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      CloudStorageAccount account = CloudStorageAccount.Parse(this.ReadConnectionString(deploymentRequestContext));
      deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(deploymentRequestContext, new StrongBoxItemChangedCallback(this.OnConnectionStringSecretChanged), DiagnosticsExportService.DiagnosticsConnectionStringSecretDrawer, (IEnumerable<string>) new string[1]
      {
        "DiagnosticsConnectionString"
      });
      this.cloudBlobClient = account.CreateCloudBlobClient();
    }

    public void ServiceEnd(IVssRequestContext deploymentRequestContext) => deploymentRequestContext.CheckDeploymentRequestContext();

    private void OnConnectionStringSecretChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      IVssRequestContext deploymentRequestContext = requestContext.GetElevatedDeploymentRequestContext();
      foreach (StrongBoxItemName itemName in itemNames)
      {
        if (itemName.LookupKey == "DiagnosticsConnectionString")
          this.cloudBlobClient.Credentials.UpdateKey(CloudStorageAccount.Parse(this.ReadConnectionString(deploymentRequestContext)).Credentials.ExportBase64EncodedKey());
      }
    }

    private string ReadConnectionString(IVssRequestContext deploymentRequestContext)
    {
      IVssRequestContext deploymentRequestContext1 = deploymentRequestContext.GetElevatedDeploymentRequestContext();
      ITeamFoundationStrongBoxService service = deploymentRequestContext1.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(deploymentRequestContext1, DiagnosticsExportService.DiagnosticsConnectionStringSecretDrawer, true);
      StrongBoxItemInfo itemInfo = service.GetItemInfo(deploymentRequestContext1, drawerId, "DiagnosticsConnectionString", true);
      return service.GetString(deploymentRequestContext1, itemInfo);
    }

    public CloudBlobContainer GetContainerReference(string containerName) => this.cloudBlobClient.GetContainerReference(containerName);

    public async Task AppendLogAsync(
      string containerName,
      string logFileName,
      ArraySegment<byte> payloadBytes,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CloudBlobContainer container = this.GetContainerReference(containerName);
      int num1 = await container.CreateIfNotExistsAsync(cancellationToken).ConfigureAwait(false) ? 1 : 0;
      CloudAppendBlob appendBlob = container.GetAppendBlobReference(logFileName);
      await this.CreateIfNotExistsAppendBlob(appendBlob, cancellationToken).ConfigureAwait(false);
      long num2 = await appendBlob.AppendBlockAsync((Stream) new MemoryStream(payloadBytes.Array, payloadBytes.Offset, payloadBytes.Count), (string) null, cancellationToken).ConfigureAwait(false);
      container = (CloudBlobContainer) null;
      appendBlob = (CloudAppendBlob) null;
    }

    public async Task WriteFullLogAsync(
      string containerName,
      string logFileName,
      ArraySegment<byte> payloadBytes,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CloudBlobContainer container = this.GetContainerReference(containerName);
      ConfiguredTaskAwaitable<bool> configuredTaskAwaitable = container.CreateIfNotExistsAsync(cancellationToken).ConfigureAwait(false);
      int num1 = await configuredTaskAwaitable ? 1 : 0;
      CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(logFileName);
      configuredTaskAwaitable = cloudBlockBlob.DeleteIfExistsAsync(cancellationToken).ConfigureAwait(false);
      int num2 = await configuredTaskAwaitable ? 1 : 0;
      await cloudBlockBlob.UploadFromStreamAsync((Stream) new MemoryStream(payloadBytes.Array, payloadBytes.Offset, payloadBytes.Count), cancellationToken).ConfigureAwait(false);
      container = (CloudBlobContainer) null;
      cloudBlockBlob = (CloudBlockBlob) null;
    }

    private async Task CreateIfNotExistsAppendBlob(
      CloudAppendBlob appendBlob,
      CancellationToken cancellationToken)
    {
      try
      {
        await appendBlob.CreateOrReplaceAsync(AccessCondition.GenerateIfNotExistsCondition(), (BlobRequestOptions) null, (OperationContext) null, cancellationToken).ConfigureAwait(false);
      }
      catch (StorageException ex) when (
      {
        // ISSUE: unable to correctly present filter
        RequestResult requestInformation = ex.RequestInformation;
        if (requestInformation != null && requestInformation.HttpStatusCode == 409)
        {
          SuccessfulFiltering;
        }
        else
          throw;
      }
      )
      {
      }
    }
  }
}
