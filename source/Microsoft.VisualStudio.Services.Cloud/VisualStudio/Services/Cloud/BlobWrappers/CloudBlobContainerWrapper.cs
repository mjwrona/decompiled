// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobWrappers.CloudBlobContainerWrapper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud.BlobWrappers
{
  public class CloudBlobContainerWrapper : ICloudBlobContainerWrapper
  {
    internal CloudBlobContainer m_container;
    internal StorageCredentials m_credentials;
    private object m_updateCredentialsLock = new object();
    internal DateTime m_lastSuccessfulUpdateCredentials = DateTime.UtcNow;

    public CloudBlobContainerWrapper(CloudBlobContainer container, StorageCredentials credentials = null)
    {
      this.m_container = container;
      this.m_credentials = credentials;
    }

    public string Name => this.m_container.Name;

    public Uri Uri => this.m_container.Uri;

    public string StorageAccountName => this.m_container.Uri.Host.Split('.')[0];

    public virtual Func<StorageCredentials> UpdateCredentialsCallback { get; internal set; }

    public bool UpdateCredentials()
    {
      Func<StorageCredentials> credentialsCallback = this.UpdateCredentialsCallback;
      if (credentialsCallback == null)
        return false;
      lock (this.m_updateCredentialsLock)
      {
        if (DateTime.UtcNow.Subtract(this.m_lastSuccessfulUpdateCredentials) <= TimeSpan.FromMinutes(1.0))
          return true;
        StorageCredentials storageCredentials = credentialsCallback();
        int num = this.m_credentials == null ? 1 : (!this.m_credentials.Equals(storageCredentials) ? 1 : 0);
        if (num != 0)
        {
          this.SetCredentials(storageCredentials);
          this.m_lastSuccessfulUpdateCredentials = DateTime.UtcNow;
        }
        return num != 0;
      }
    }

    public virtual void SetCredentials(StorageCredentials newCredentials)
    {
      this.m_container = newCredentials != null ? new CloudBlobContainer(this.m_container.Uri, newCredentials) : new CloudBlobContainer(this.m_container.Uri);
      this.m_credentials = newCredentials;
    }

    public Uri GetUriWithCredentials(ICloudBlobWrapper blob)
    {
      StorageCredentials credentials = this.m_credentials;
      if (credentials == null)
        return blob.Uri;
      UriBuilder uriBuilder = new UriBuilder(blob.Uri.AbsoluteUri);
      string query = uriBuilder.Query;
      StringBuilder stringBuilder = new StringBuilder();
      if (query != null && query.Length > 1)
      {
        stringBuilder.Append(query.Substring(1));
        stringBuilder.Append('&');
      }
      string str = credentials.IsSAS ? credentials.SASToken : throw new NotSupportedException("Only SAS credentials are supported");
      stringBuilder.Append(str.Substring(str.StartsWith("?", StringComparison.Ordinal) ? 1 : 0));
      uriBuilder.Query = stringBuilder.ToString();
      return uriBuilder.Uri;
    }

    public BlobContainerPermissions GetPermissions() => this.m_container.GetPermissions();

    public bool Exists() => this.m_container.Exists();

    public void CreateIfNotExists(BlobContainerPermissions permissions)
    {
      bool flag = false;
      CloudBlobContainer container = this.m_container;
      try
      {
        flag = container.CreateIfNotExists();
      }
      catch (StorageException ex)
      {
        if (ex?.RequestInformation?.HttpStatusCode.GetValueOrDefault() == 409)
        {
          if (string.Equals(ex?.RequestInformation?.HttpStatusMessage, StorageErrorCodeStrings.ContainerAlreadyExists, StringComparison.OrdinalIgnoreCase))
            goto label_6;
        }
        throw;
      }
label_6:
      if (!flag)
        return;
      container.SetPermissions(permissions);
    }

    public ICloudBlobWrapper GetBlockBlobReference(string blobName) => (ICloudBlobWrapper) new CloudBlobWrapper((ICloudBlob) this.m_container.GetBlockBlobReference(blobName));

    public ICloudBlobWrapper GetPageBlobReference(string blobName, long length)
    {
      CloudPageBlob pageBlobReference = this.m_container.GetPageBlobReference(blobName);
      pageBlobReference.Create(length);
      return (ICloudBlobWrapper) new CloudBlobWrapper((ICloudBlob) pageBlobReference);
    }

    public ICloudBlobWrapper GetBlobReferenceFromServer(
      string blobName,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return (ICloudBlobWrapper) new CloudBlobWrapper(this.m_container.GetBlobReferenceFromServer(blobName, accessCondition, options, operationContext));
    }

    public IBlobResultSegmentWrapper ListBlobsSegmented(
      string prefix,
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return (IBlobResultSegmentWrapper) new BlobResultSegmentWrapper(this.m_container.ListBlobsSegmented(prefix, useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext));
    }
  }
}
