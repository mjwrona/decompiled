// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobWrappers.CloudStorageAccountWrapper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.BlobWrappers
{
  public class CloudStorageAccountWrapper : ICloudStorageAccountWrapper
  {
    private CloudStorageAccount m_account;

    public CloudStorageAccountWrapper(string connectionString) => this.m_account = CloudStorageAccount.Parse(connectionString);

    public CloudStorageAccountWrapper(string sasToken, Uri accountUri)
    {
      string[] strArray = accountUri.Host.Split(new char[1]
      {
        '.'
      }, StringSplitOptions.None);
      string accountName = strArray[0];
      string endpointSuffix = string.Join(".", strArray, 2, strArray.Length - 2);
      this.m_account = new CloudStorageAccount(new StorageCredentials(sasToken), accountName, endpointSuffix, true);
    }

    public CloudStorageAccountWrapper(CloudStorageAccount account) => this.m_account = account;

    public CloudStorageAccountWrapper()
    {
    }

    public ICloudBlobClientWrapper CreateCloudBlobClient()
    {
      CloudBlobClient client = this.m_account != null ? this.m_account.CreateCloudBlobClient() : throw new InvalidOperationException("Must configure an account to create a cloud blob client");
      client.DefaultRequestOptions.MaximumExecutionTime = new TimeSpan?(TimeSpan.FromSeconds(3600.0));
      client.DefaultRequestOptions.ServerTimeout = new TimeSpan?(TimeSpan.FromSeconds(300.0));
      return (ICloudBlobClientWrapper) new CloudBlobClientWrapper(client);
    }

    public ICloudBlobWrapper CreateBlobWrapper(IListBlobItem blob) => (ICloudBlobWrapper) new CloudBlobWrapper(blob as ICloudBlob);

    public ICloudBlobContainerWrapper CreateContainerWrapper(CloudBlobContainer container) => (ICloudBlobContainerWrapper) new CloudBlobContainerWrapper(container);

    public bool IsValidBlobType(IListBlobItem item) => item is ICloudBlob;
  }
}
