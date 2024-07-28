// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.AzureBlobClientToolProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public class AzureBlobClientToolProvider : IClientToolStorageProvider
  {
    private IAzureBlobContainerFactory factory;
    private readonly string storageAccountName;

    public AzureBlobClientToolProvider(
      IVssRequestContext systemRequestContext,
      string storageAccountName)
    {
      IVssRequestContext requestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      this.storageAccountName = storageAccountName;
      this.InitializeBlobContainerFactory(requestContext);
    }

    private void InitializeBlobContainerFactory(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      IAzureCloudBlobClientProvider service = vssRequestContext.GetService<IAzureCloudBlobClientProvider>();
      if (string.IsNullOrEmpty(this.storageAccountName))
        return;
      foreach (StrongBoxConnectionString allStorageAccount in StorageAccountConfigurationFacade.ReadAllStorageAccounts(vssRequestContext))
      {
        if (this.storageAccountName.Equals(StorageAccountUtilities.GetAccountInfo(allStorageAccount.ConnectionString).Name))
        {
          this.factory = (IAzureBlobContainerFactory) service.GetBlobClient(allStorageAccount.StrongBoxItemKey);
          break;
        }
      }
    }

    public PreauthenticatedUri? GenerateUri(
      IVssRequestContext requestContext,
      string fileName,
      Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.ClientTool toolName,
      EdgeCache edgeCache)
    {
      if (this.factory == null || fileName == null)
        return new PreauthenticatedUri?();
      ICloudBlockBlob blockBlobReference = this.factory.CreateContainerReference(toolName.ToString().ToLower(), requestContext.IsFeatureEnabled("Blobstore.Features.AzureBlobTelemetry")).GetBlockBlobReference(fileName);
      IClock instance = UtcClock.Instance;
      SASUriExpiry expiry = SASUriExpiry.CreateExpiry(new SASUriExpiryPolicy(instance), requestContext);
      string compatibleSasToken = AzureBlobBlobProvider.GetAzureFrontDoorCompatibleSASToken(blockBlobReference, instance.Now + SASUriExpiryPolicy.DefaultBounds.MaxExpiry, expiry, (string) null, (SharedAccessBlobHeaders) null, ((string, Guid)[]) null);
      PreauthenticatedUri uri = new PreauthenticatedUri(new Uri(blockBlobReference.StorageUri.PrimaryUri?.ToString() + compatibleSasToken), EdgeType.NotEdge);
      return this.ConvertOriginalUri(requestContext, uri, edgeCache);
    }

    protected virtual PreauthenticatedUri? ConvertOriginalUri(
      IVssRequestContext requestContext,
      PreauthenticatedUri uri,
      EdgeCache edgeCache)
    {
      return new PreauthenticatedUri?(uri);
    }
  }
}
