// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.CloudBlobClientAdapter
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class CloudBlobClientAdapter : ICloudBlobClient, IAzureBlobContainerFactory
  {
    internal readonly CloudBlobClient blobClient;
    private readonly string containerNamePrefix;

    public CloudBlobClientAdapter(CloudBlobClient blobClient, string containerNamePrefix = null)
    {
      this.blobClient = blobClient;
      this.containerNamePrefix = containerNamePrefix ?? string.Empty;
    }

    public CloudBlobClientAdapter(
      CloudBlobClientAdapter blobClientAdapter,
      string containerNamePrefix)
    {
      this.blobClient = blobClientAdapter.blobClient;
      this.containerNamePrefix = containerNamePrefix;
    }

    public Microsoft.Azure.Storage.RetryPolicies.LocationMode? LocationMode
    {
      get => this.blobClient.DefaultRequestOptions.LocationMode;
      set => this.blobClient.DefaultRequestOptions.LocationMode = value;
    }

    public ICloudBlobContainer CreateContainerReference(string containerName, bool enableTracing) => (ICloudBlobContainer) new CloudBlobContainerWrapper(this.blobClient.GetContainerReference(this.containerNamePrefix + containerName), enableTracing);

    public void UpdateConnectionString(string connectionString) => this.blobClient.Credentials.UpdateKey(CloudStorageAccount.Parse(connectionString).Credentials.ExportBase64EncodedKey());
  }
}
