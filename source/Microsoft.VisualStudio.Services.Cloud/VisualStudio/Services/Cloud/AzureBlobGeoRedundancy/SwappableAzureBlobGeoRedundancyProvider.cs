// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.SwappableAzureBlobGeoRedundancyProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class SwappableAzureBlobGeoRedundancyProvider : IAzureBlobProvider, IBlobProvider
  {
    internal AzureBlobGeoRedundancyProvider m_provider;
    private string m_drawerName;
    private string m_primaryLookupKey;
    private string m_secondaryLookupKey;
    private bool m_disableBufferManager;
    private readonly Func<IVssRequestContext, AzureBlobGeoRedundancyProvider> m_providerFactory;
    public static readonly string SwapPrimaryAndSecondaryRegistryKey = "/Service/AzureBlobGeoRedundancy/Settings/SwapPrimaryAndSecondary";
    private const string c_area = "AzureBlobGeoRedundancy";
    private const string c_layer = "SwappableAzureBlobGeoRedundancyProvider";

    public SwappableAzureBlobGeoRedundancyProvider()
      : this((Func<IVssRequestContext, AzureBlobGeoRedundancyProvider>) (rc => new AzureBlobGeoRedundancyProvider()))
    {
    }

    internal SwappableAzureBlobGeoRedundancyProvider(
      Func<IVssRequestContext, AzureBlobGeoRedundancyProvider> providerFactory)
    {
      ArgumentUtility.CheckForNull<Func<IVssRequestContext, AzureBlobGeoRedundancyProvider>>(providerFactory, nameof (providerFactory));
      this.m_providerFactory = providerFactory;
    }

    public void ServiceStart(IVssRequestContext requestContext) => this.ServiceStart(requestContext, (IDictionary<string, string>) null);

    public void ServiceStart(
      IVssRequestContext requestContext,
      IDictionary<string, string> settings)
    {
      string drawerName;
      string lookupKey;
      if (settings != null && settings.TryGetValue("DrawerName", out drawerName) && settings.TryGetValue("LookupKey", out lookupKey))
      {
        this.m_drawerName = drawerName;
        this.m_primaryLookupKey = lookupKey;
        GeoRedundantStorageAccountSettings storageAccountSettings = requestContext.GetService<IAzureBlobGeoRedundancyManagementService>().GetGeoRedundantStorageAccounts(requestContext).Where<GeoRedundantStorageAccountSettings>((Func<GeoRedundantStorageAccountSettings, bool>) (a => string.Equals(a.DrawerName, drawerName, StringComparison.Ordinal) && string.Equals(a.PrimaryLookupKey, lookupKey, StringComparison.Ordinal))).FirstOrDefault<GeoRedundantStorageAccountSettings>();
        if (storageAccountSettings != null)
          this.m_secondaryLookupKey = storageAccountSettings.SecondaryLookupKey;
      }
      if (this.m_secondaryLookupKey != null)
      {
        requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingChanged), SwappableAzureBlobGeoRedundancyProvider.SwapPrimaryAndSecondaryRegistryKey);
        this.UpdateProvider(requestContext);
        requestContext.TraceAlways(15308000, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (SwappableAzureBlobGeoRedundancyProvider), "Successfully configured. DrawerName: {0}, PrimaryLookupKey: {1}, SecondaryLookupkey: {2}", (object) this.m_drawerName, (object) this.m_primaryLookupKey, (object) this.m_secondaryLookupKey);
      }
      else
      {
        this.m_provider = this.m_providerFactory(requestContext);
        this.m_provider.ServiceStart(requestContext, settings);
        this.ConfigureBlobProvider(this.m_provider);
        requestContext.TraceAlways(15308001, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (SwappableAzureBlobGeoRedundancyProvider), "Not configured with secondary provider, swapping will not be supported. DrawerName: {0}, PrimaryLookupKey: {1}", (object) this.m_drawerName, (object) this.m_primaryLookupKey);
      }
    }

    public void ServiceEnd(IVssRequestContext requestContext) => this.m_provider.ServiceEnd(requestContext);

    public string StorageAccountName => this.m_provider.StorageAccountName;

    public void OnRegistrySettingChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.UpdateProvider(requestContext);
    }

    public void UpdateProvider(IVssRequestContext requestContext)
    {
      if (this.m_secondaryLookupKey == null)
      {
        requestContext.Trace(15308002, TraceLevel.Warning, "AzureBlobGeoRedundancy", nameof (SwappableAzureBlobGeoRedundancyProvider), "Unable to update blob providers, no secondary is configured. DrawerName: {0}, PrimaryLookupKey: {1}", (object) this.m_drawerName, (object) this.m_primaryLookupKey);
      }
      else
      {
        int num = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) SwappableAzureBlobGeoRedundancyProvider.SwapPrimaryAndSecondaryRegistryKey, false) ? 1 : 0;
        Dictionary<string, string> settings = new Dictionary<string, string>();
        settings["DrawerName"] = this.m_drawerName;
        if (num != 0)
        {
          settings["BlobStorageConnectionStringOverride"] = AzureBlobGeoRedundancyUtils.GetStorageAccountConnectionString(requestContext, this.m_drawerName, this.m_secondaryLookupKey);
          settings["LookupKey"] = this.m_secondaryLookupKey;
        }
        else
        {
          settings["BlobStorageConnectionStringOverride"] = AzureBlobGeoRedundancyUtils.GetStorageAccountConnectionString(requestContext, this.m_drawerName, this.m_primaryLookupKey);
          settings["LookupKey"] = this.m_primaryLookupKey;
        }
        AzureBlobGeoRedundancyProvider provider = this.m_providerFactory(requestContext);
        provider.ServiceStart(requestContext, (IDictionary<string, string>) settings);
        this.ConfigureBlobProvider(provider);
        this.m_provider = provider;
      }
    }

    public RemoteStoreId RemoteStoreId => this.m_provider.RemoteStoreId;

    public Uri Uri => this.m_provider.Uri;

    public bool BlobExists(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.BlobExists(requestContext, containerId, resourceId, clientTimeout);
    }

    public bool BlobExists(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.BlobExists(requestContext, containerName, resourceId, clientTimeout);
    }

    public bool ContainerExists(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.ContainerExists(requestContext, containerId, clientTimeout);
    }

    public bool DeleteBlob(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.DeleteBlob(requestContext, containerId, resourceId, clientTimeout);
    }

    public bool DeleteBlob(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.DeleteBlob(requestContext, containerName, resourceId, clientTimeout);
    }

    public List<Guid> DeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      List<Guid> resourceIds,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.DeleteBlobs(requestContext, containerId, resourceIds, clientTimeout);
    }

    public List<string> DeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      List<string> resourceIds,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.DeleteBlobs(requestContext, containerId, resourceIds, clientTimeout);
    }

    public void DeleteContainer(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null)
    {
      this.m_provider.DeleteContainer(requestContext, containerId, clientTimeout);
    }

    public void DownloadToStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      TimeSpan? clientTimeout = null)
    {
      this.m_provider.DownloadToStream(requestContext, containerId, resourceId, targetStream, clientTimeout);
    }

    public void DownloadToStreamLargeBlocks(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      TimeSpan? clientTimeout = null)
    {
      this.m_provider.DownloadToStreamLargeBlocks(requestContext, containerId, resourceId, targetStream, clientTimeout);
    }

    public IEnumerable<string> EnumerateBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.EnumerateBlobs(requestContext, containerId, clientTimeout);
    }

    public Stream GetStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.GetStream(requestContext, containerId, resourceId, clientTimeout);
    }

    public void PutChunk(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      byte[] contentBlock,
      int contentBlockLength,
      long compressedLength,
      long offset,
      bool isLastChunk,
      IDictionary<string, string> blobMetadata = null,
      TimeSpan? clientTimeout = null)
    {
      this.m_provider.PutChunk(requestContext, containerId, resourceId, contentBlock, contentBlockLength, compressedLength, offset, isLastChunk, blobMetadata, clientTimeout);
    }

    public void PutStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream stream,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      this.m_provider.PutStream(requestContext, containerId, resourceId, stream, metadata, clientTimeout);
    }

    public void PutStream(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId,
      Stream stream,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      this.m_provider.PutStream(requestContext, containerName, resourceId, stream, metadata, clientTimeout);
    }

    public IDictionary<string, string> ReadBlobMetadata(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.ReadBlobMetadata(requestContext, containerId, resourceId, clientTimeout);
    }

    public IDictionary<string, string> ReadBlobMetadata(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.ReadBlobMetadata(requestContext, containerName, resourceId, clientTimeout);
    }

    public Microsoft.TeamFoundation.Framework.Server.BlobProperties ReadBlobProperties(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.ReadBlobProperties(requestContext, containerId, resourceId, clientTimeout);
    }

    public void RenameBlob(
      IVssRequestContext requestContext,
      Guid containerId,
      string sourceResourceId,
      string targetResourceId,
      TimeSpan? clientTimeout = null)
    {
      this.m_provider.RenameBlob(requestContext, containerId, sourceResourceId, targetResourceId, clientTimeout);
    }

    public void RenameBlob(
      IVssRequestContext requestContext,
      string containerName,
      string sourceResourceId,
      string targetResourceId,
      TimeSpan? clientTimeout = null)
    {
      this.m_provider.RenameBlob(requestContext, containerName, sourceResourceId, targetResourceId, clientTimeout);
    }

    public void SetBlobHeaders(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      string cacheControl,
      string contentType,
      string contentDisposition,
      string contentEncoding,
      string contentLanguage,
      TimeSpan? clientTimeout = null)
    {
      this.m_provider.SetBlobHeaders(requestContext, containerId, resourceId, cacheControl, contentType, contentDisposition, contentEncoding, contentLanguage, clientTimeout);
    }

    public void SetBlobHeaders(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      string cacheControl,
      string contentType,
      string contentDisposition,
      string contentEncoding,
      string contentLanguage,
      TimeSpan? clientTimeout = null)
    {
      this.m_provider.SetBlobHeaders(requestContext, containerId, resourceId, cacheControl, contentType, contentDisposition, contentEncoding, contentLanguage, clientTimeout);
    }

    public void WriteBlobMetadata(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      this.m_provider.WriteBlobMetadata(requestContext, containerId, resourceId, metadata, clientTimeout);
    }

    public void WriteBlobTags(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      IDictionary<string, string> tags,
      TimeSpan? clientTimeout = null)
    {
      this.m_provider.WriteBlobTags(requestContext, containerId, resourceId, tags, clientTimeout);
    }

    public IDictionary<string, string> ReadBlobTags(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId)
    {
      return this.m_provider.ReadBlobTags(requestContext, containerId, resourceId);
    }

    public CloudBlobContainer GetCloudBlobContainer(
      IVssRequestContext requestContext,
      Guid containerId,
      bool createIfNotExists,
      TimeSpan? clientTimeout = null)
    {
      return this.m_provider.GetCloudBlobContainer(requestContext, containerId, createIfNotExists, clientTimeout);
    }

    public Page<TaggedBlobItem> FindBlobsByTags(
      IDictionary<string, string> tags,
      string containerName = null,
      string continuationToken = null)
    {
      return this.m_provider.FindBlobsByTags(tags, containerName, continuationToken);
    }

    public void PutStreamRaw(
      CloudBlobContainer container,
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout)
    {
      this.m_provider.PutStreamRaw(container, resourceId, content, metadata, clientTimeout);
    }

    public void PutStreamRawUsingBlockBlobClient(
      CloudBlobContainer container,
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      int? blockSize,
      CancellationToken cancellationToken)
    {
      this.m_provider.PutStreamRawUsingBlockBlobClient(container, resourceId, content, metadata, blockSize, cancellationToken);
    }

    public void DisableBufferManager()
    {
      this.m_disableBufferManager = true;
      this.m_provider.DisableBufferManager();
    }

    internal virtual void ConfigureBlobProvider(AzureBlobGeoRedundancyProvider provider)
    {
      if (!this.m_disableBufferManager)
        return;
      provider.DisableBufferManager();
    }
  }
}
