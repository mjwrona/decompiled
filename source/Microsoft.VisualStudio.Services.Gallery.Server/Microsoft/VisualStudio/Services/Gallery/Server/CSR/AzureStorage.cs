// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CSR.AzureStorage
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CSR
{
  public class AzureStorage : IAzureStorage, IVssFrameworkService
  {
    private CloudBlobClient _cloudBlobClient;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public AzureStorage(string connectionString)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(connectionString, nameof (connectionString));
      this._cloudBlobClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
    }

    public async Task<CloudBlockBlob> UploadContentToBlobStorageAsync(
      Stream blobContent,
      string containerName,
      string blobName)
    {
      ArgumentUtility.CheckForNull<Stream>(blobContent, nameof (blobContent));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(containerName, nameof (containerName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(blobName, nameof (blobName));
      CloudBlockBlob cloudBlockBlob = this._cloudBlobClient.GetContainerReference(containerName).GetBlockBlobReference(blobName);
      await cloudBlockBlob.UploadFromStreamAsync(blobContent);
      CloudBlockBlob blobStorageAsync = cloudBlockBlob;
      cloudBlockBlob = (CloudBlockBlob) null;
      return blobStorageAsync;
    }

    public async Task<List<IListBlobItem>> ListAllBlobsWithPrefix(
      string containerName,
      string prefix)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(containerName, nameof (containerName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(prefix, nameof (prefix));
      BlobContinuationToken currentToken = (BlobContinuationToken) null;
      List<IListBlobItem> results = new List<IListBlobItem>();
      CloudBlobContainer cloudBlobContainer = this._cloudBlobClient.GetContainerReference(containerName);
      do
      {
        BlobResultSegment blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(prefix, currentToken);
        currentToken = blobResultSegment.ContinuationToken;
        if (blobResultSegment != null)
          results.AddRange(blobResultSegment.Results);
      }
      while (currentToken != null);
      List<IListBlobItem> listBlobItemList = results;
      results = (List<IListBlobItem>) null;
      cloudBlobContainer = (CloudBlobContainer) null;
      return listBlobItemList;
    }
  }
}
