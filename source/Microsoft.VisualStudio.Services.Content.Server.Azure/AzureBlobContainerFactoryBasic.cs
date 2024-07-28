// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.AzureBlobContainerFactoryBasic
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Net;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class AzureBlobContainerFactoryBasic : AzureContainerFactory
  {
    protected readonly int maxConnectionsPerProc;
    internal Lazy<CloudBlobClient> azureBlobClient;

    public StrongBoxConnectionString StrongBoxConnectionString { get; }

    public AzureBlobContainerFactoryBasic(
      StrongBoxConnectionString sbConnectionString,
      string locationMode,
      int maxConnectionsPerProc)
    {
      this.StrongBoxConnectionString = sbConnectionString;
      this.LocationMode = StorageAccountUtilities.ParseLocationMode(locationMode);
      this.maxConnectionsPerProc = maxConnectionsPerProc;
      this.azureBlobClient = new Lazy<CloudBlobClient>((Func<CloudBlobClient>) (() => this.CreateBlobClient()));
    }

    public override ICloudBlobContainer CreateContainerReference(
      string containerName,
      bool enableTracing)
    {
      return (ICloudBlobContainer) new CloudBlobContainerWrapper(this.azureBlobClient.Value.GetContainerReference(containerName), enableTracing);
    }

    public override void OnSecretChanged(
      string keyOfChangedStrongboxItem,
      string newStrongBoxItemValue)
    {
      if (string.Equals("LocationMode", keyOfChangedStrongboxItem))
      {
        this.LocationMode = StorageAccountUtilities.ParseLocationMode(newStrongBoxItemValue);
        this.azureBlobClient.Value.DefaultRequestOptions.LocationMode = this.LocationMode;
      }
      else
      {
        if (!string.Equals(this.StrongBoxConnectionString.StrongBoxItemKey, keyOfChangedStrongboxItem, StringComparison.Ordinal))
          return;
        this.azureBlobClient.Value.Credentials.UpdateKey(CloudStorageAccount.Parse(newStrongBoxItemValue).Credentials.ExportBase64EncodedKey());
      }
    }

    protected virtual CloudBlobClient CreateBlobClient()
    {
      CloudStorageAccount account = CloudStorageAccount.Parse(this.StrongBoxConnectionString.ConnectionString);
      ServicePoint servicePoint1 = ServicePointManager.FindServicePoint(account.BlobStorageUri.PrimaryUri);
      ServicePoint servicePoint2 = ServicePointManager.FindServicePoint(account.BlobStorageUri.SecondaryUri);
      servicePoint1.ConnectionLimit = servicePoint2.ConnectionLimit = Environment.ProcessorCount * this.maxConnectionsPerProc;
      servicePoint1.UseNagleAlgorithm = servicePoint2.UseNagleAlgorithm = false;
      CloudBlobClient cloudBlobClient = account.CreateCloudBlobClient();
      cloudBlobClient.DefaultRequestOptions.LocationMode = this.LocationMode;
      return cloudBlobClient;
    }
  }
}
